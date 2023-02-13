using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FWI
{
    public class FormatOutputStream : IOutputStream
    {
        readonly IOutputStream outputStream;
        public FormatOutputStream(IOutputStream outputStream)
        {
            this.outputStream = outputStream;
        }
        public void Write(string value)
        {
            outputStream.Write(ParseText(value));
        }
        public void WriteLine(string value)
        {
            outputStream.WriteLine(ParseText(value));
        }
        public void Flush()
        {
            outputStream.Flush();
        }

        string ParseText(string text)
        {
            string result = "";

            int index = 0;
            var re = new Regex(@"\[[A-Z]\]");
            while (index + 3 < text.Length)
            {
                var s = text.Substring(index, 3);
                if (re.IsMatch(s))
                {
                    if (TryParseKey(s[1], out string parsed)) result += parsed;
                    else result = s;

                    index += 3;
                }
                else
                {
                    break;
                }
            }

            if (index < text.Length) result += text.Substring(index);
            return result;
        }

        protected virtual bool TryParseKey(char key, out string text)
        {
            switch(key)
            {
                case 'D':
                    text = $"{DateTime.Now:HH:mm:ss} ";
                    return true;
                case 'A':
                    text = "[알림]";
                    return true;
                case 'I':
                    text = "[정보]";
                    return true;
                case 'W':
                    text = "[경고]";
                    return true;
                default:
                    text = null;
                    return false;
            };
        }
    }
}
