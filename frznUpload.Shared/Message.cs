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
            Error = 1,
            AuthRequest = 2,
            Auth = 4,
            KeyExchange = 6,
            AuthSuccess = 8,
            FileUploadRequest = 10,
            FileUploadApproved = 12,
            FileUpload = 14,
            FileUploadFinished = 16,
            FileUploadSuccess = 18,
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

        public Message(MessageType type, IEnumerable<object> fields)
        {
            Type = type;
            Fields = fields.ToList();
        }

        public Message(byte[] bytes)
        {
            var mem = new MemoryStream(bytes);

            Type = (MessageType)mem.ReadByte();
            IsError = (Type & MessageType.Error) == MessageType.Error;

            byte[] headBuffer = new byte[2];

            while (mem.Read(headBuffer, 0, 2) == 2)
            {
                short head = BitConverter.ToInt16(headBuffer, 0);
                int length = 0b0011111111111111 & head;
                FieldType type = (FieldType)((0b1100000000000000 & head) / 0b0100000000000000);

                byte[] data = new byte[length == 0 ? int.MaxValue : length];
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
            int totalLength = 0;

            foreach(object o in Fields)
            {
                totalLength += GetFieldLength(o);
            }

            byte[] bytes = new byte[totalLength];


            int copied = 0;
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



    }
}
