using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWIConnection;

namespace FWI.Message
{
    public class RequestTimelineMessage : ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.RequestTimeline;

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

        public static RequestTimelineMessage Deserialize(ByteReader reader)
        {
            var message = new RequestTimelineMessage();
            if (reader.ReadShort() != Op) throw new DeserializeFailException();

            return message;
        }
    }
}
