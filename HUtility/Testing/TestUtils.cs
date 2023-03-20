using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility.Testing
{
    public class TestUtils
    {
        static public DateTime MakeDate(string format) => DateTime.ParseExact(format, "yyMMdd", null);
        static public DateTime MakeDateTime(string format) => DateTime.ParseExact(format, "yyMMdd HHmmss", null);
        static public DateRange MakeDateRange(string begin, string end) => new DateRange(MakeDate(begin), MakeDate(end));
        static public DateRange MakeDateTimeRange(string begin, string end) => new DateRange(MakeDateTime(begin), MakeDateTime(end));
    }
}