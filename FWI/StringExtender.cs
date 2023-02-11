using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public static class StringExtender
    {
        public static string PadCenter(this string text, int totalWidth)
        {
            var length = text.Length;
            var pad = totalWidth - length;

            if (pad > 0)
            {
                string paddedString = text.PadLeft(length + pad / 2).PadRight(length + pad);
                return paddedString;
            }
            else
            {
                return text;
            }
        }

        public static string Truncate(this string input, int maxLength)
        {
            var right = (maxLength > 0);
            maxLength = Math.Abs(maxLength);
            
            if (input.Length <= maxLength) return input;
            else if (right) return input.Substring(0, maxLength) + "...";
            else return "..." + input.Substring(input.Length-maxLength, maxLength);
        }
    }
}
