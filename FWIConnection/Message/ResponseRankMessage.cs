using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIConnection.Message
{
    public class ResponseRankMessage : ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.ResponseRank;
        public readonly Dictionary<int, >

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

        public static ResponseRankMessage Deserialize(ByteReader reader)
        {
            var message = new ResponseRankMessage();
            if (reader.ReadShort() != Op) throw new DeserializeFailException();

            return message;
        }
    }
}
