using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWIConnection;

namespace FWI.Message
{
    public class NoAFKMessage : ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.SetNoAFK;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public byte[] Serialize() => Serialize(false);
        public byte[] Serialize(bool debug = false)
        {
            var writer = new ByteWriter();
            writer.Write(Op);
            writer.WriteDateTime(FromDate);
            writer.WriteDateTime(ToDate);
            if (debug)
            {
                writer.Write($"#NoAFK");
                writer.Write($"?{FromDate}");
                writer.Write($"?{ToDate}");
            }

            return writer.ToBytes();
        }

        public static NoAFKMessage Deserialize(ByteReader reader)
        {
            var message = new NoAFKMessage();
            if (reader.ReadShort() != Op) throw new DeserializeFailException();

            reader.ReadDateTime(out DateTime fromDate);
            message.FromDate = fromDate;

            reader.ReadDateTime(out DateTime toDate);
            message.ToDate = toDate;
            return message;
        }
    }
}
