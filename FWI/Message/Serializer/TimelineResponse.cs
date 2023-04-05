using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWIConnection;

namespace FWI.Message
{
    public class TimelineResponse : ResponseMessage, ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.ResponseTimeline;
        public List<Tuple<WindowInfoLegacy, DateTime>> Timeline { get; set; }

        public byte[] Serialize() => Serialize(false);
        public byte[] Serialize(bool debug = false)
        {
            var writer = new ByteWriter();
            writer.WriteShort(Op);
            writer.WriteInt(Timeline.Count);

            foreach(var (wi, date) in this.Timeline)
            {
                writer.WriteDateTime(date);
                writer.WriteWI(wi);
            }

            return writer.ToBytes();
        }

        public static TimelineResponse Deserialize(ByteReader reader)
        {
            if (reader.ReadShort() != Op) throw new DeserializeFailException();
            var message = new TimelineResponse();
            var count = reader.ReadInt();

            message.Timeline = new List<Tuple<WindowInfoLegacy, DateTime>>();
            for(var i = 0; i < count; i++)
            {
                var date = reader.ReadDateTime();
                var wi = reader.ReadWI();

                message.Timeline.Add(Tuple.Create(wi, date));
            } 

            return message;
        }

        public static TimelineResponse Fail()
        {
            return new TimelineResponse()
            {
                Failed = true,
            };
        }
    }
}
