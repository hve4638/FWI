using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FWI
{
    public static class WindowInfoExtender
    {
        public static bool IsAFK(this WindowInfo wi) => wi.Special == WindowInfoType.AFK;
        public static bool IsNoWindow(this WindowInfo wi) => wi.Special == WindowInfoType.NoWindow;
        public static bool IsNormal(this WindowInfo wi) => wi.Special == WindowInfoType.Normal;
        public static bool IsEmpty(this WindowInfo wi)
        {
            if (wi.Special != WindowInfoType.Normal || wi.Title != "" || wi.Name != "") return false;
            else return true;
        }
        // 구조체는 copy-by-value기 때문에 Copy()가 필요없으나
        // WindowInfo가 추후 Class로 변경될 가능성 때문에 Copy를 이용
        public static WindowInfo Copy(this WindowInfo wi)
        {
            return new WindowInfo()
            {
                Title = wi.Title,
                Name = wi.Name,
                Alias = wi.Alias,
                Date = wi.Date,
                Duration = wi.Duration,
                Special = wi.Special,
            };
        }
    }
}
