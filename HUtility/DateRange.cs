using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    /// <summary>
    /// 두 DateTime의 범위를 가지는 Immutable 클래스
    /// </summary>
    public class DateRange
    {
        static readonly DateRange emptyDateRange;
        protected DateTime begin;
        protected DateTime end;
        static DateRange()
        {
            emptyDateRange = new DateRange(new DateTime(2000, 1, 1), new DateTime(1990, 1, 1));
        }

        public DateTime Begin => begin;
        public DateTime End => end;

        /// <summary>
        /// [begin, end] 의 범위를 가진 DateTime 범위 클래스
        /// </summary>
        public DateRange(DateTime begin, DateTime end)
        {
            this.begin = begin;
            this.end = end;
        }
        /// <summary>
        /// 범위가 공집합인 DateRange
        /// </summary>
        static public DateRange Empty => emptyDateRange;

        static public DateRange operator &(DateRange o1, DateRange o2)
        {
            if (o1.Begin > o2.Begin) (o2, o1) = (o1, o2);

            if (o1.End < o2.Begin || o1.Begin > o2.End) return Empty;
            else if (o1.Begin <= o2.Begin && o1.End >= o2.End) return o2;
            else return new DateRange(o2.Begin, o1.End);
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
            else return (Begin == other.Begin && End == other.End);
        }
        public bool Contains(DateTime dateTime) => (dateTime >= Begin && dateTime <= End);
        public bool Contains(DateRange dateRange)
        {
            if (IsEmpty) return false;
            else if (dateRange.IsEmpty) return false;
            else if (Begin <= dateRange.Begin && End >= dateRange.End) return true;
            else return false;
        }

        public TimeSpan Time => End - Begin;
        public bool IsEmpty => Begin > End;

        public override string ToString()
        {
            if (IsEmpty) return "Empty";
            else return $"{Begin:yyMMdd}-{End:yyMMdd}";
        }
        public override int GetHashCode() => base.GetHashCode();
    }
}
