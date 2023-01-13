using FWI.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class WindowInfo
    {
        string title;
        string name;
        string alias;
        DateTime date;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public string GetAliasOrName()
        {
            if (alias == "") return name;
            else return alias;
        }

        public WindowInfo(string name, string alias = null, string title = null, DateTime? date = null)
        {
            this.name = name;
            this.title = title ?? name;
            this.alias = alias ?? "";
            this.date = date ?? DateTime.Now;
        }
        public virtual WindowInfo Copy() {
            return new WindowInfo(name: name, alias: alias, title: title, date: date);
        }

        public static WindowInfo Empty(DateTime? date = null) => new WindowInfo(title: "", name: "", alias: "", date: date);

        public static bool operator !=(WindowInfo obj1, WindowInfo obj2)
        {
            if (obj1 is null && obj2 is null) return false;
            else if (obj1 is null || obj2 is null) return true;
            else return !obj1.Equals(obj2);
            
        }
        public static bool operator ==(WindowInfo obj1, WindowInfo obj2)
        {
            if (obj1 is null && obj2 is null) return true;
            else if (obj1 is null || obj2 is null) return false;
            else return obj1.Equals(obj2);
        }

        public override bool Equals(object o)
        {
            if (o is WindowInfo)
            {
                WindowInfo wi = (WindowInfo)o;
                return (title == wi.title && name == wi.name && DateFormat(date) == DateFormat(wi.date));
            }
            return false;
        }
        
        public override string ToString()
        {
            return $"<FWI.WindowInfo'{title}','{name}','{alias}',({date.ToString("yyyy.MM.dd hh:mm:ss")})>";
        }

        public string Encoding()
        {
            return $"{date:yyyyMMddhhmmss}|{StringEncode(name)}|{StringEncode(title)}|{StringEncode(alias ?? "")}";
        }
        static public WindowInfo Decode(string encoded)
        {
            try
            {
                string[] splited = encoded.Split('|');
                string date = StringDecode(splited[0]);
                string name = StringDecode(splited[1]);
                string title = StringDecode(splited[2]);
                string alias = StringDecode(splited[3]);
                if (alias == "") alias = null;

                return new WindowInfo(title: title, name: name, alias: alias, date: GetDateByFormat(date));
            }
            catch (IndexOutOfRangeException)
            {
                throw new ParseException($"WindowInfo.Decode() : Decode Fail ('{encoded}')");
            }
        }
        static string DateFormat(DateTime date) => date.ToString("yyyyMMddhhmmss");
        static string StringEncode(string value) => value.Replace(@"\", @"\\").Replace("|", @"\<split>");
        static string StringDecode(string value) => value.Replace(@"\<split>", @"|").Replace(@"\\", @"\");
        static DateTime GetDateByFormat(string format) => DateTime.ParseExact(format, "yyyyMMddhhmmss", null);

        public override int GetHashCode() => base.GetHashCode();

        public int GetContentsHashCode()
        {
            var hash = (name + title + alias).GetHashCode();
            hash ^= DateFormat(date).GetHashCode();

            return hash;
        }
    }
}
