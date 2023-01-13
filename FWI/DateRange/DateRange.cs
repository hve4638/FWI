using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{
    public class DateRange
    {
        static readonly DateRange emptyDateRange;
        protected DateTime begin;
        protected DateTime end;
        static DateRange()
        {
            emptyDateRange = new DateRange(new DateTime(2000, 1, 1), new DateTime(1990, 1, 1));
        }

        /// <summary>
        /// [begin, end] 의 범위를 가진 DateTime 범위 클래스
        /// </summary>
        public DateRange(DateTime begin, DateTime end)
        {
            this.begin = begin;
            this.end = end;
        }
        static public DateRange Empty
        {
            get { return emptyDateRange; }
        }

        static public DateRange operator &(DateRange o1, DateRange o2)
        {
            if (o1.Begin > o2.Begin) Swap(ref o1, ref o2);

            if (o1.End < o2.Begin || o1.Begin > o2.End) return DateRange.Empty;
            else if (o1.Begin <= o2.Begin && o1.End >= o2.End) return o2;
            else return new DateRange(o2.Begin, o1.End);
        }

        static void Swap(ref DateRange o1, ref DateRange o2)
        {
            DateRange o1Copy = o1;
            o1 = o2;
            o2 = o1Copy;
        }

        static public bool operator !=(DateRange d1, DateRange d2) => !d1.Equals(d2);
        static public bool operator ==(DateRange d1, DateRange d2) => d1.Equals(d2);
        static public bool operator <(DateRange dateRange, DateTime dt) => (dateRange.End <= dt);
        static public bool operator >(DateTime dt, DateRange dateRange) => (dateRange.End <= dt);
        static public bool operator >(DateRange dateRange, DateTime dt) => (dateRange.Begin > dt);
        static public bool operator <(DateTime dt, DateRange dateRange) => (dateRange.Begin > dt);
        static public bool operator <=(DateRange dateRange, DateTime dt) => (dateRange.End <= dt);
        static public bool operator >=(DateTime dt, DateRange dateRange) => (dateRange.End <= dt);
        static public bool operator >=(DateRange dateRange, DateTime dt) => (dateRange.Begin >= dt);
        static public bool operator <=(DateTime dt, DateRange dateRange) => (dateRange.Begin >= dt);

        public override bool Equals(object o)
        {
            if (o is DateRange) return Equals(o as DateRange);
            else return false;

        }
        public bool Equals(DateRange other)
        {
            if (IsEmpty && other.IsEmpty) return true;
            else if (IsEmpty || other.IsEmpty) return false;
            else return (begin == other.Begin && end == other.End);
        }
        public bool Contains(DateTime dateTime) => (dateTime >= begin && dateTime <= end);
        public bool Contains(DateRange dateRange)
        {
            if (IsEmpty) return false;
            else if (dateRange.IsEmpty) return false;
            else if (begin <= dateRange.Begin && end >= dateRange.End) return true;
            else return false;
        }

        public DateTime Begin
        {
            get { return begin; }
        }
        public DateTime End
        {
            get { return end; }
        }
        public TimeSpan Time
        {
            get { return end - begin; }
        }
        public bool IsEmpty
        {
            get { return begin > end; }
        }

        public override string ToString()
        {
            if (IsEmpty) return "Empty";
            else return $"{begin.ToString("yyMMdd")}-{end.ToString("yyMMdd")}";
        }
        public override int GetHashCode() => base.GetHashCode();
    }
}
