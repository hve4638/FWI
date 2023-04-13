using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI
{

    class TimelineFinder
    {
        readonly Timeline target;
        public TimelineFinder(Timeline timeline)
        {
            target = timeline;
        }

        public int Find(DateTime date)
        {
            if (target.Count == 0) return -1;

            var index = FindNoisy(date);
            var wi = target[index];

            if (wi.Date == date) return index;
            else return -1;
        }
        public int FindNearestPast(DateTime date)
        {
            if (target.Count == 0) return -1;

            var index = FindNoisy(date);
            WindowInfo wi;
            wi = target[index];

            if (wi.Date == date || wi.Date < date) return index;
            else if (wi.Date > date && index - 1 >= 0) return index - 1;
            else return -1;
        }
        public int FindNearestFuture(DateTime date)
        {
            if (target.Count == 0) return -1;

            var index = FindNoisy(date);
            WindowInfo wi;
            wi = target[index];

            if (wi.Date == date || wi.Date > date) return index;
            else if (wi.Date < date && index + 1 < target.Count) return index + 1;
            else return -1;
        }
        public int FindNearest(DateTime date)
        {
            if (target.Count == 0) return -1;

            var index = FindNoisy(date);
            int index2;
            WindowInfo wi, wi2;
            wi = target[index];

            if (wi.Date == date) return index;
            else if (wi.Date < date) index2 = index + 1;
            else index2 = index - 1;

            if (index2 < 0 || index2 >= target.Count) return index;
            if (index > index2) (index, index2) = (index2, index);

            wi = target[index];
            wi2 = target[index2];
            double dif1 = Math.Abs((date - wi.Date).TotalMilliseconds);
            double dif2 = Math.Abs((date - wi2.Date).TotalMilliseconds);

            if (dif1 <= dif2) return index;
            else return index2;
        }

        int FindNoisy(DateTime date)
        {
            int min = 0;
            int max = target.Count - 1;
            int result = 0;

            while (min <= max)
            {
                result = (min + max) / 2;

                if (target[result].Date == date) break;
                else if (target[result].Date < date) min = result + 1;
                else max = result - 1;
            }

            if (result < 0) result = 0;
            else if (result >= target.Count) result = target.Count - 1;

            return result;
        }
    }
}
