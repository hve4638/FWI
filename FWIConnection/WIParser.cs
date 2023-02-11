using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FWIConnection
{
    static public class WIParser
    {
        static public void WriteWI(this ByteWriter bw, string name, string title, DateTime date)
        {
            bw.WriteDateTime(date);
            bw.WriteText(name);
            bw.WriteText(title);
        }

        static public void ReadWI(this ByteReader br, out string name, out string title, out DateTime date)
        {
            br.ReadDateTime(out date);
            br.ReadText(out name);
            br.ReadText(out title);
        }

        static public void WriteDateTime(this ByteWriter bw, DateTime date)
        {
            bw.WriteInt(date.Year);
            bw.WriteInt(date.Month);
            bw.WriteInt(date.Day);
            bw.WriteInt(date.Hour);
            bw.WriteInt(date.Minute);
            bw.WriteInt(date.Second);
        }

        static public void ReadDateTime(this ByteReader br, out DateTime date)
        {
            int year, month, day, hour, minute, second;
            year = br.ReadInt();
            month = br.ReadInt();
            day = br.ReadInt();
            hour = br.ReadInt();
            minute = br.ReadInt();
            second = br.ReadInt();
            date = new DateTime(year, month, day, hour, minute, second);
        }

        static public void WriteText(this ByteWriter bw, string value)
        {
            bw.WriteInt(Encoding.Default.GetByteCount(value));
            bw.WriteString(value);
        }

        static public void ReadText(this ByteReader br, out string value)
        {
            var size = br.ReadInt();
            value = br.ReadString(ref size);
        }

        static public string ReadText(this ByteReader br)
        {
            var size = br.ReadInt();
            return br.ReadString(ref size);
        }
    }
}
