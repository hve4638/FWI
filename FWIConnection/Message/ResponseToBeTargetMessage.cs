using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIConnection.Message
{
    public class ResponseToBeTargetMessage : ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.ResponseToBeTarget;
        public short Id { get; set; }
        public bool Accepted { get; set; }

        public byte[] Serialize() => Serialize(false);
        public byte[] Serialize(bool debug = false)
        {
            var writer = new ByteWriter();
            writer.WriteShort(Op);
            writer.WriteShort(Id);
            writer.WriteShort((short)(Accepted ? 1 : 0));

            if (debug)
            {
                writer.WriteString($"#ResponseToBeTarget");
                writer.WriteString("?");
                writer.WriteString(Accepted ? "Accept" : "Deny");
            }
            return writer.ToBytes();
        }

        public static ResponseToBeTargetMessage Deserialize(ByteReader reader)
        {
            var message = new ResponseToBeTargetMessage();
            if (reader.ReadShort() != Op) throw new DeserializeFailException();

            message.Id = reader.ReadShort();
            var accepted = reader.ReadShort();
            switch(accepted)
            {
                case 0:
                    message.Accepted = false;
                    break;
                case 1:
                    message.Accepted = true;
                    break;
                default:
                    throw new DeserializeFailException();
            }

            return message;
        }
    }
}
