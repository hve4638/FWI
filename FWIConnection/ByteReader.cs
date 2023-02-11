using System;
using System.IO;
using System.Linq;
using System.Text;

namespace FWIConnection
{
    public class ByteReader
    {
        byte[] buf;
        readonly int size;
        public int Offset { get; set; }

        public ByteReader(in byte[] buf, int size)
        {
            this.buf = buf;
            this.size = size;
            Offset = 0;
        }

        public ByteReader(string value) : this(new ByteWriter(value))
        {

        }

        public ByteReader(ByteWriter bw) : this(bw.ToBytes())
        {

        }

        public ByteReader(in byte[] buf) : this(buf, buf.Length)
        {

        }


        public byte[] PeekBytes()
        {
            int? length = null;
            return PeekBytes(ref length);
        }
        public byte[] PeekBytes(ref int? length)
        {
            int len = length ?? size - Offset;
            var data = PeekBytes(ref len);
            length = len;

            return data;
        }
        public byte[] PeekBytes(ref int length)
        {
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = buf[Offset + i];
            }
            return bytes;
        }
        public short PeekShort()
        {
            short num = BitConverter.ToInt16(buf, Offset);
            return num;
        }
        public int PeekInt()
        {
            int num = BitConverter.ToInt32(buf, Offset);
            return num;
        }
        public string PeekString()
        {
            int? length = null;

            return PeekString(ref length);
        }
        public string PeekString(ref int? length)
        {
            int len = length ?? size - Offset;
            var data = PeekString(ref len);
            length = len;

            return data;
        }
        public string PeekString(ref int length)
        {
            string data = Encoding.UTF8.GetString(buf, Offset, (int)length);
            length = data.Length;

            return data;
        }


        public byte[] ReadBytes(int? length = null)
        {
            length = length ?? (size - Offset);
            var data = PeekBytes(ref length);
            Offset += data.Length;

            return data;
        }
        public short ReadShort()
        {
            short num = PeekShort();
            Offset += 2;
            return num;
        }
        public int ReadInt()
        {
            int num = PeekInt();
            Offset += 4;
            return num;
        }
        public string ReadString()
        {
            int? length = null;
            return ReadString(ref length);
        }
        public string ReadString(ref int? length)
        {
            string data = PeekString(ref length);
            Offset += data.Length;

            return data;
        }
        public string ReadString(ref int length)
        {
            string data = PeekString(ref length);
            Offset += data.Length;

            return data;
        }

        public override string ToString()
        {
            var bslice = new ArraySegment<byte>(buf, 0, size);
            var sb = new StringBuilder();
            var s = BitConverter.ToString(bslice.ToArray()).Replace("-", ", 0x");

            sb.Append("Bytes [ 0x");
            sb.Append(s);
            sb.Append(" ]");

            return sb.ToString();
        }

        public void ExportFile(string filename, int? offset = null)
        {
            int start = offset ?? this.Offset;
            FileStream fs = new FileStream(filename, FileMode.Create);
            fs.Write(buf, start, size-start);
            fs.Close();
        }
    }
}
