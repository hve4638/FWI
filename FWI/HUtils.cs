using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    static public class HUtils 
    {
        static public ReadOnlyCollection<WindowInfo> CutWindowInfoList(ReadOnlyCollection<WindowInfo> list, DateRange range)
        {
            var dateRange = GetWindowInfoListDateRange(list);
            var intersection = dateRange & range;

            if (intersection.IsEmpty) return new List<WindowInfo>().AsReadOnly();
            else if (intersection == dateRange) return list;
            else
            {
                var dr = new DynamicDateRange(dateRange.End, dateRange.End);
                int? st = null;
                int? ed = null;

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var item = list[i];
                    dr.End = dr.Begin.AddSeconds(-1);
                    dr.Begin = item.Date;

                    if ((dr & range).IsEmpty)
                    {
                        if (ed == null) continue;
                        else break;
                    }
                    else
                    {
                        if (ed == null) ed = i;
                        else st = i;
                    }
                }

                var ls = new List<WindowInfo>();
                for (int i = (int)st; i <= ed; i++)
                {
                    ls.Add(list[i]);
                }
                return ls.AsReadOnly();
            }
        }

        static private DateRange GetWindowInfoListDateRange(IList<WindowInfo> list)
        {
            DateTime dt1, dt2;
            if (list.Count == 0) return DateRange.Empty;
            else if (list.Count == 1)
            {
                dt1 = list[0].Date;
                return new DateRange(dt1, dt1.AddMinutes(1));
            }
            else
            {
                var length = list.Count;
                dt1 = list[0].Date;
                dt2 = list[length - 1].Date;
                return new DateRange(dt1, dt2);
            }
        }

        static public Tuple<string, string> SplitPath(string path)
        {
            if (path == "") return Tuple.Create("", "");

            try
            {
                string directory = Path.GetDirectoryName(path);
                string fileName = Path.GetFileName(path);
                return Tuple.Create(directory, fileName);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"잘못된 경로 : {path}");
            }
        }

        static public string EncodeVertical(string plain)
        {
            return plain.Replace(@"\", @"\\").Replace("|", @"\<__split>");
        }

        static public string DecodeVertical(string encoded)
        {
            return encoded.Replace(@"\<__split>", @"|").Replace(@"\\", @"\");
        }

        static public string MergePath(string path, string name)
        {
            if (path == "") return name;
            else return path + @"\" + name;
        }
    }
}
