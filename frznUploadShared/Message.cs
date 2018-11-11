using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Shared
{
    public class Message
    {
        public enum  MessageType : byte
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

        enum FieldType
        {
            String,
            Int,
            Raw,
        }

        public MessageType Type { get; private set; }
        public bool IsError { get; private set; }
        public List<object> Fields { get; private set; }

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


    }
}
