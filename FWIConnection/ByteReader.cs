using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIConnection
{
    public class ByteReader
    {
        byte[] buf;
        readonly int size;
        int offset;

        public ByteReader(in byte[] buf, int size)
        {
            this.buf = buf;
            this.size = size;
            offset = 0;
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


        public short ReadShort()
        {
            short num = BitConverter.ToInt16(buf, offset);
            offset += 2;
            return num;
        }
        public int ReadInt()
        {
            int num = BitConverter.ToInt32(buf, offset);
            offset += 4;
            return num;
        }

        public string ReadString(int? length=null)
        {
            string data = Encoding.UTF8.GetString(buf, offset, length ?? (size - offset));
            offset += data.Length;

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
            int start = offset ?? this.offset;
            FileStream fs = new FileStream(filename, FileMode.Create);
            fs.Write(buf, start, size-start);
            fs.Close();
        }
    }
}
