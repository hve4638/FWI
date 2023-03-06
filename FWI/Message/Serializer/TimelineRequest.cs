using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWIConnection;

namespace FWI.Message
{
    public class TimelineRequest : ISerializableMessage
    {
        public readonly static int Op = (int)MessageOp.RequestTimeline;
        public int Id { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        public byte[] Serialize() => Serialize(false);
        public byte[] Serialize(bool debug = false)
        {
            var writer = new ByteWriter();
            writer.WriteInt(Op);
            writer.WriteInt(Id);
            writer.WriteDateTimeNullable(BeginDate);
            writer.WriteDateTimeNullable(EndDate);

            if (debug)
            {
                writer.WriteString($"#RequestTimeline");
            }

            return writer.ToBytes();
        }

        public static TimelineRequest Deserialize(ByteReader reader)
        {
            if (reader.ReadInt() != Op) throw new DeserializeFailException();

            var message = new TimelineRequest();
            message.Id = reader.ReadInt();
            message.BeginDate = reader.ReadDateTimeNullable();
            message.EndDate = reader.ReadDateTimeNullable();
            
            return message;
        }
    }
}
