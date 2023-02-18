using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIConnection.Message
{
    public class RequestEchoMessage : ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.RequestEcho;
        public int Id { get; set; }
        public string Text { get; set; }

        public byte[] Serialize() => Serialize(false);
        public byte[] Serialize(bool debug = false)
        {
            var writer = new ByteWriter();
            writer.WriteShort(Op);
            writer.WriteInt(Id);
            writer.WriteText(Text);

            if (debug)
            {
                writer.Write($"#Echo");
                writer.Write($"?{Text}");
            }

            return writer.ToBytes();
        }

        public static RequestEchoMessage Deserialize(ByteReader reader)
        {
            var echoMessage = new RequestEchoMessage();
            if (reader.ReadShort() != Op) throw new DeserializeFailException();
            echoMessage.Id = reader.ReadInt();
            echoMessage.Text = reader.ReadText();

            return echoMessage;
        }
    }
}
