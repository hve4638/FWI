using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIConnection.Message
{
    [Obsolete]
    static public class WindowInfoMessage
    {
        static public void Make(ByteWriter bw, string name, string title, DateTime? date = null) {
            bw.Write((short)MessageOp.UpdateCurrentWI);
            bw.Write(GetDateTimeBytes(date ?? DateTime.Now));
            bw.WriteInt(Encoding.Default.GetByteCount(name));
            bw.WriteString(name);
            bw.WriteInt(Encoding.Default.GetByteCount(title));
            bw.WriteString(title);
        }

        static public void MakeDateTime(in ByteWriter bw, DateTime date)
        {
            bw.WriteInt(date.Year);
            bw.WriteInt(date.Month);
            bw.WriteInt(date.Day);
            bw.WriteInt(date.Hour);
            bw.WriteInt(date.Minute);
            bw.WriteInt(date.Second);
        }

        static public void Parse(ByteReader br, out string name, out string title, out DateTime date)
        {
            int nameSize, titleSize;
            int year, month, day, hour, minute, second;
            year = br.ReadInt();
            month = br.ReadInt();
            day = br.ReadInt();
            hour = br.ReadInt();
            minute = br.ReadInt();
            second = br.ReadInt();

            nameSize = br.ReadInt();
            name = br.ReadString(nameSize);

            titleSize = br.ReadInt();
            title = br.ReadString(titleSize);
            date = new DateTime(year, month, day, hour, minute, second);
        }

        static public byte[] GetDateTimeBytes(DateTime date)
        {
            var bw = new ByteWriter();
            bw.WriteInt(date.Year);
            bw.WriteInt(date.Month);
            bw.WriteInt(date.Day);
            bw.WriteInt(date.Hour);
            bw.WriteInt(date.Minute);
            bw.WriteInt(date.Second);

            return bw.ToBytes();
        }
    }
}
