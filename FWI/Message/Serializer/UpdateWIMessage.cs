using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWIConnection;

namespace FWI.Message
{
    public class UpdateWIMessage : ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.UpdateWI;
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }

        public byte[] Serialize() => Serialize(false);
        public byte[] Serialize(bool debug = false)
        {
            var bw = new ByteWriter();
            bw.WriteShort(Op);
            bw.WriteWI(name: Name, title: Title, date: Date);

            if (debug)
            {
                bw.WriteString($"#UpdateWI");
                bw.WriteString($"?Name:{Name}");
                bw.WriteString($"?Title:{Title}");
                bw.WriteString($"?Date:{Date}");
            }

            return bw.ToBytes();
        }

        public static UpdateWIMessage Deserialize(ByteReader reader)
        {
            var message = new UpdateWIMessage();

            if (reader.ReadShort() != Op) throw new DeserializeFailException();
            reader.ReadWI(name: out string name, title: out string title, date: out DateTime date);
            message.Name = name;
            message.Title = title;
            message.Date = date;
            return message;
        }
    }
}
