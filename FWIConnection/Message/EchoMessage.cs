using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIConnection.Message
{
    public class EchoMessage : ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.Echo;
        public string Text { get; set; }

        public byte[] Serialize() => Serialize(false);
        public byte[] Serialize(bool debug = false)
        {
            var writer = new ByteWriter();
            writer.WriteShort(Op);
            writer.WriteText(Text);

            if (debug)
            {
                writer.Write($"#Echo");
                writer.Write($"?{Text}");
            }

            return writer.ToBytes();
        }

        public static EchoMessage Deserialize(ByteReader reader)
        {
            var echoMessage = new EchoMessage();
            if (reader.ReadShort() != Op) throw new DeserializeFailException();
            echoMessage.Text = reader.ReadText();

            return echoMessage;
        }
    }
}
