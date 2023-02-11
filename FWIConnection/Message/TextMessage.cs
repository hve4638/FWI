using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIConnection.Message
{
    public class TextMessage : ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.Message;
        public string Text { get; set; }

        public byte[] Serialize() => Serialize(false);
        public byte[] Serialize(bool debug = false)
        {
            var writer = new ByteWriter();
            writer.WriteShort(Op);
            writer.WriteText(Text);

            if (debug)
            {
                writer.WriteString($"#Message");
            }
            return writer.ToBytes();
        }

        public static TextMessage Deserialize(ByteReader reader)
        {
            var message = new TextMessage();
            if (reader.ReadShort() != Op) throw new DeserializeFailException();
            
            message.Text = reader.ReadText();
            return message;
        }
    }
}
