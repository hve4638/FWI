using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIConnection.Message
{
    public class RequestRankMessage : ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.RequestRank;

        public byte[] Serialize() => Serialize(false);
        public byte[] Serialize(bool debug = false)
        {
            var writer = new ByteWriter();
            writer.WriteShort(Op);

            if (debug)
            {
                writer.WriteString($"#RequestTimeline");
            }
            return writer.ToBytes();
        }

        public static RequestRankMessage Deserialize(ByteReader reader)
        {
            var message = new RequestRankMessage();
            if (reader.ReadShort() != Op) throw new DeserializeFailException();

            return message;
        }
    }
}
