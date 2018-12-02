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
            ChallengeRequest,
            Challenge,
            ChallengeResponse,
            ChallengeApproved,
            AuthRequest,
            Auth,
            AuthSuccess,
            FileUploadRequest,
            FileUploadApproved,
            FileUpload,
            FileUploadFinished,
            FileUploadSuccess,
            Sequence
        }

        public enum FieldType
        {
            String,
            Int,
            Raw,
        }

        public MessageType Type { get; private set; }
        public bool IsError { get; private set; }
        public List<object> Fields { get; private set; }

        public object this[int i] => Fields[i];

        public Message(MessageType type, IEnumerable<object> fields, bool isError = false)
        {
            Type = type;
            Fields = fields.ToList();
            IsError = isError;
        }

        /*public Message(MessageType type, params object[] fields)
        {
            Type = type;
            Fields = fields.ToList();
            IsError = false;
        }*/

        public Message(MessageType type, bool isError, params object[] fields)
        {
            Type = type;
            Fields = fields.ToList();
            IsError = isError;
        }

        public Message(byte[] bytes)
        {
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
                FieldType type = (FieldType)((0b1100000000000000 & head) / 0b0100000000000000);

                byte[] data = new byte[length == 0 ? mem.Length - mem.Position : length];
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

                    default:
                        throw new Exception("Type not recognized");
                }

                Fields.Add(field);
            }
        }

        public byte[] ToByte()
        {
            int totalLength = 1;

            foreach(object o in Fields)
            {
                totalLength += GetFieldLength(o);
            }

            byte[] bytes = new byte[totalLength];

            bytes[0] = (byte)((byte)Type | (IsError ? 0b1000_0000 : 0));

            int copied = 1;
            for (int i = 0; i < Fields.Count; i++)
            {
                object obj = Fields[i];
                FieldType fieldType;
                byte[] data;

                if (obj is int)
                {
                    fieldType = FieldType.Int;
                    data = BitConverter.GetBytes((int)obj);
                }
                else if (obj is string)
                {
                    fieldType = FieldType.String;
                    data = Encoding.UTF8.GetBytes((string)obj);
                }
                else if (obj is byte[])
                {
                    fieldType = FieldType.Raw;
                    data = (byte[])obj;
                }
                else
                {
                    throw new ArgumentException("Only int, byte[] and string are valid Fields");
                }

                short length;
                if (i == Fields.Count - 1)
                {
                    length = 0;
                }
                else
                {
                    if (data.Length > 0b0011111111111111)
                        throw new ArgumentException("Field " + i + " too big!");
                    length = (short)data.Length;
                }

                length = (short)(length & ((int)fieldType * 0b0100000000000000));
                var lengthArray = BitConverter.GetBytes(length);
                
                Array.Copy(lengthArray, 0, bytes, copied, 2);
                Array.Copy(data, 0, bytes, 2 + copied, data.Length);

                copied += 2 + data.Length;
            }

            return bytes;
        }

        private int GetFieldLength(object obj)
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
                return ((byte[])obj).Length;
            }
            else
            {
                throw new ArgumentException("Only int, byte[] and string are valid Fields");
            }
        }

        public override string ToString()
        {
            string str = IsError ? "Error!\n" : "";

            str += Type + "\n";

            foreach(object obj in Fields)
            {
                str += obj + "\n";
            }

            return str;
        }
    }
}
