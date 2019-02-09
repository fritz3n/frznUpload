using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace frznUpload.Shared
{
    public class Message
    {
        public enum MessageType : byte
        {
            Ping,
            Pong,
            ChallengeRequest, // client requests a challenge to prove ownership of an authenticated public key
            Challenge, // server sends a challenge to the client to prove ownership of an authenticated public key
            ChallengeResponse, // client sends the signed challenge to the server
            ChallengeApproved, // server approves of the challengeresponse, or doesnt if the error flag is set. Also sends username
            AuthRequest, // server asks the client to authenticate a public key //NOT YET USED//
            Auth, // client authenticates a public key with a username and password
            AuthSuccess, // server approves of the authentication, the pub key is now authenticated but the client isnt yet
            DeauthRequest,
            DeauthSuccess,
            FileUploadRequest,
            FileUploadApproved,
            FileUpload,
            FileUploadFinished,
            FileUploadSuccess,
            ShareRequest, // client requests a fileshare to be made
            ShareResponse, // server sends the fileshare id
            FileListRequest,
            FileList, // A list of Fileinfo Messages
            FileInfo,
            ShareListRequest,
            ShareList, // A list of shareinfo messages
            ShareInfo,
            Version,
            Sequence, // Used for a sequence error message, when the wrong message type is received
            None // General Message type, Mainly used for Errors
        }

        public enum FieldType
        {
            String,
            Int,
            Raw,
            Message,
        }

        public MessageType Type { get; private set; }
        public bool IsError { get; private set; }
        public List<dynamic> Fields { get; private set; }

        public int BinaryLength { get
            {
                int totalLength = 1;

                foreach (object o in Fields)
                {
                    totalLength += GetFieldLength(o);
                }

                return totalLength;
            } }

        public int Count { get => Fields.Count; }

        public dynamic this[int i] => Fields[i];

        public List<FieldType> FieldTypes { get; private set; }

        public Message(MessageType type, IEnumerable<object> fields, bool isError = false)
        {
            Type = type;
            Fields = fields.ToList();
            IsError = isError;

            IndexTypes();
        }

        public Message(MessageType type, bool isError, params dynamic[] fields)
        {
            Type = type;
            Fields = fields.ToList();
            IsError = isError;

            IndexTypes();
        }

        public Message(MessageType type, bool isError = false)
        {
            Type = type;
            Fields = new List<dynamic>();
            IsError = isError;

            IndexTypes();
        }

        private void IndexTypes()
        {
            FieldTypes = new List<FieldType>();

            for (int i = 0; i < Fields.Count; i++)
            {
                object obj = Fields[i];
                FieldType fieldType;

                if (obj is int)
                {
                    fieldType = FieldType.Int;
                }
                else if (obj is string)
                {
                    fieldType = FieldType.String;
                }
                else if (obj is byte[])
                {
                    fieldType = FieldType.Raw;
                }
                else if (obj is Message)
                {
                    fieldType = FieldType.Message;
                }
                else
                {
                    throw new ArgumentException("Only int, byte[], string and Message are valid Fields");
                }

                FieldTypes.Add(fieldType);
            }
        }

        public Message(byte[] bytes)
        {
            FieldTypes = new List<FieldType>();
            var mem = new MemoryStream(bytes);

            byte typeByte = (byte)mem.ReadByte();

            Type = (MessageType)(typeByte & 0b0111_1111);
            IsError = (typeByte & 0b1000_0000) > 0;
            Fields = new List<object>();

            byte[] headBuffer = new byte[2];

            while (mem.Read(headBuffer, 0, 2) == 2)
            {
                short head = BitConverter.ToInt16(headBuffer, 0);
                int length = 0b0011111111111111 & head;
                FieldType type = (FieldType)((0b1100000000000000 & head) >> 14);

                byte[] data = new byte[length == 0b0011111111111111 ? mem.Length - mem.Position : length];
                length = mem.Read(data, 0, data.Length);


                object field;

                switch (type)
                {
                    case FieldType.Int:
                        field = BitConverter.ToInt32(data, 0);
                        break;

                    case FieldType.String:
                        field = Encoding.UTF8.GetString(data);
                        break;

                    case FieldType.Raw:
                        field = data;
                        break;

                    case FieldType.Message:
                        field = new Message(data);
                        break;

                    default:
                        throw new Exception("Type not recognized");
                }

                Fields.Add(field);
                FieldTypes.Add(type);
            }
        }

        public byte[] ToByte()
        {
            byte[] bytes = new byte[BinaryLength];

            bytes[0] = (byte)((byte)Type | (IsError ? 0b1000_0000 : 0));

            int copied = 1;
            for (int i = 0; i < Fields.Count; i++)
            {
                object obj = Fields[i];
                FieldType fieldType = FieldTypes[i];
                byte[] data;

                switch (fieldType) {
                    case FieldType.Int:
                        data = BitConverter.GetBytes((int)obj);
                        break;

                    case FieldType.String:
                        data = Encoding.UTF8.GetBytes((string)obj);
                        break;

                    case FieldType.Raw:
                        data = (byte[])obj;
                        break;

                    case FieldType.Message:
                        data = (obj as Message).ToByte();
                        break;

                    default:
                        throw new ArgumentException("Type of Field " + i + " is undefined:\n" + obj.ToString());
                }

                ushort length;
                if (i == Fields.Count - 1)
                {
                    length = 0b0011111111111111;
                }
                else
                {
                    if (data.Length > 0b0011111111111110)
                        throw new ArgumentException("Field " + i + " too big!");
                    length = (ushort)data.Length;
                }
                
                length = (ushort)(length | ((int)fieldType << 14));
                var lengthArray = BitConverter.GetBytes(length);
                
                Array.Copy(lengthArray, 0, bytes, copied, 2);
                Array.Copy(data, 0, bytes, 2 + copied, data.Length);

                copied += 2 + data.Length;
            }

            return bytes;
        }

        private static int GetFieldLength(object obj)
        {

            if (obj is int)
            {
                return 6;
            }
            else if (obj is string)
            {
                return 2 + Encoding.UTF8.GetByteCount((string)obj);
            }
            else if (obj is byte[])
            {
                return 2 + ((byte[])obj).Length;
            }
            else if (obj is Message)
            {
                return 2 + (obj as Message).BinaryLength;
            }
            else
            {
                throw new ArgumentException("Only int, byte[], string and Message are valid Fields");
            }
        }

        public override string ToString()
        {
            string str = IsError ? "Error: \n" : "";

            str += Type + ": \n";

            foreach(object obj in Fields)
            {
                str += obj + "; \n";
            }

            return str;
        }
    }
}
