using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIConnection.Message
{
    public class ServerCallMessage : ISerializableMessage
    {
        public readonly static short Op = (short)MessageOp.ServerCall;
        public string Command { get; set; }

        public byte[] Serialize() => Serialize(false);
        public byte[] Serialize(bool debug = false)
        {
            var writer = new ByteWriter();
            writer.WriteShort(Op);
            writer.WriteText(Command);

            if (debug)
            {
                writer.WriteString($"#ServerCall");
                writer.WriteString($"?{Command}");
            }
            return writer.ToBytes();
        }

        public static ServerCallMessage Deserialize(ByteReader reader)
        {
            var message = new ServerCallMessage();
            if (reader.ReadShort() != Op) throw new DeserializeFailException();

            message.Command = reader.ReadText();
            return message;
        }
    }
}
