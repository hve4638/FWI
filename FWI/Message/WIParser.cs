using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FWIConnection;
#nullable enable 

namespace FWI
{
    static public class WIParser
    {
        public static void WriteWI(this ByteWriter bw, WindowInfoLegacy wi)
        {
            bw.WriteDateTime(wi.Date);
            bw.WriteText(wi.Title);
            bw.WriteText(wi.Name);
            bw.WriteText(wi.Alias);
        }
        public static WindowInfoLegacy ReadWI(this ByteReader br)
        {
            br.ReadDateTime(out DateTime date);
            br.ReadText(out string name);
            br.ReadText(out string title);
            br.ReadText(out string alias);

            return new WindowInfoLegacy(name: name, alias: alias, title: title, date: date);
        }

        public static void WriteWI(this ByteWriter bw, string name, string title, DateTime date)
        {
            bw.WriteDateTime(date);
            bw.WriteText(name);
            bw.WriteText(title);
        }

        public static void ReadWI(this ByteReader br, out string name, out string title, out DateTime date)
        {
            br.ReadDateTime(out date);
            br.ReadText(out name);
            br.ReadText(out title);
        }

        public static DateTime? ReadDateTimeNullable(this ByteReader br)
        {
            var isNotNull = br.ReadBoolean();

            if (isNotNull) return br.ReadDateTime();
            else return null;
        }

        public static void WriteDateTimeNullable(this ByteWriter bw, DateTime? date)
        {
            if (date == null) bw.WriteBoolean(false);
            else
            {
                bw.WriteBoolean(true);
                bw.WriteDateTime((DateTime)date);
            }
        }

        public static void ReadDateTimeNullable(this ByteReader br, out DateTime? date)
        {
            var isNotNull = br.ReadBoolean();

            if (isNotNull) date = br.ReadDateTime();
            else date = null;
        }

        public static void WriteDateTime(this ByteWriter bw, DateTime date)
        {
            bw.WriteInt(date.Year);
            bw.WriteInt(date.Month);
            bw.WriteInt(date.Day);
            bw.WriteInt(date.Hour);
            bw.WriteInt(date.Minute);
            bw.WriteInt(date.Second);
        }

        public static void ReadDateTime(this ByteReader br, out DateTime date)
        {
            date = br.ReadDateTime();
        }

        public static DateTime ReadDateTime(this ByteReader br)
        {
            int year, month, day, hour, minute, second;
            year = br.ReadInt();
            month = br.ReadInt();
            day = br.ReadInt();
            hour = br.ReadInt();
            minute = br.ReadInt();
            second = br.ReadInt();
            return new DateTime(year, month, day, hour, minute, second);
        }

        public static void WriteText(this ByteWriter bw, string value)
        {
            bw.WriteInt(Encoding.Default.GetByteCount(value));
            bw.WriteString(value);
        }

        public static void ReadText(this ByteReader br, out string value)
        {
            var size = br.ReadInt();
            value = br.ReadString(ref size);
        }

        public static string ReadText(this ByteReader br)
        {
            var size = br.ReadInt();
            return br.ReadString(ref size);
        }

        public static void WriteTextNullable(this ByteWriter bw, string? value)
        {
            //bw.WriteShort();
            bw.WriteInt(Encoding.Default.GetByteCount(value));
            bw.WriteString(value);
        }

        public static string? ReadTextNullable(this ByteReader br)
        {
            var size = br.ReadInt();
            return br.ReadString(ref size);
        }
    }
}
