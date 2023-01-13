using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIConnection
{
    public class ByteWriter
    {
        List<byte> buf;

        public ByteWriter()
        {
            buf = new List<byte>();
        }

        public ByteWriter(byte[] buf)
        {
            this.buf = new List<byte>();
            this.buf.AddRange(buf);
        }
        public ByteWriter(string str) : this()
        {
            WriteString(str);
        }

        public void Write(short num) => WriteShort(num);
        public void Write(int num) => WriteInt(num);
        public void Write(string str) => WriteString(str);
        public void Write(byte[] bytes) => WriteBytes(bytes);
        public void Write(ByteWriter bw)
        {
            this.WriteBytes(bw.ToBytes());
        }

        public void WriteShort(short num)
        {
            byte[] bytes = BitConverter.GetBytes(num);
            buf.AddRange(bytes);
        }
        public void WriteInt(int num)
        {
            byte[] bytes = BitConverter.GetBytes(num);
            buf.AddRange(bytes);
        }
        public void WriteString(string str) {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            buf.AddRange(bytes);
        }

        public void WriteBytes(byte[] bytes)
        {
            buf.AddRange(bytes);
        }

        public byte[] ToBytes()
        {
            return buf.ToArray();
        }

        public int Length
        {
            get { return buf.Count; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var s = BitConverter.ToString(buf.ToArray()).Replace("-", ", 0x");

            sb.Append("Bytes [ 0x");
            sb.Append(s);
            sb.Append(" ]");

            return sb.ToString();
        }

        public void ExportFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            fs.Write(buf.ToArray(), 0, buf.Count);
            fs.Close();
        }
    }
}
