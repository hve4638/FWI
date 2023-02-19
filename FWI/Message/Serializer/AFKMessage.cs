using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWIConnection;

namespace FWI.Message
{
    public class AFKMessage : ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.SetAFK;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public byte[] Serialize() => Serialize(false);
        public byte[] Serialize(bool debug)
        {
            var writer = new ByteWriter();
            writer.WriteShort(Op);
            writer.WriteDateTime(FromDate);
            writer.WriteDateTime(ToDate);

            if (debug)
            {
                writer.Write($"#AFK");
                writer.Write($"?{FromDate}");
                writer.Write($"?{ToDate}");
            }

            return writer.ToBytes();
        }

        public static AFKMessage Deserialize(ByteReader reader)
        {
            var message = new AFKMessage();
            if (reader.ReadShort() != Op) throw new DeserializeFailException();

            reader.ReadDateTime(out DateTime fromDate);
            message.FromDate = fromDate;

            reader.ReadDateTime(out DateTime toDate);
            message.ToDate = toDate;
            return message;
        }
    }
}
