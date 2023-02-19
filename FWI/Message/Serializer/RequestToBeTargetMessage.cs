using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWIConnection;

namespace FWI.Message
{
    public class RequestToBeTargetMessage : ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.RequestToBeTarget;
        public short Id { get; set; }

        public byte[] Serialize() => Serialize(false);
        public byte[] Serialize(bool debug = false)
        {
            var writer = new ByteWriter();
            writer.WriteShort(Op);
            writer.WriteShort(Id);

            if (debug)
            {
                writer.WriteString($"#RequestToBeTarget");
            }
            return writer.ToBytes();
        }

        public static RequestToBeTargetMessage Deserialize(ByteReader reader)
        {
            var message = new RequestToBeTargetMessage();
            if (reader.ReadShort() != Op) throw new DeserializeFailException();

            message.Id = reader.ReadShort();
            return message;
        }
    }
}
