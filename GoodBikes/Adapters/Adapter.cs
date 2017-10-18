using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBikes.Adapters
{
    static class Adapter<T1, T2> where T2 : AAdapted<T1>, new()
    {
        public static T2 Convert(T1 item)
        {
            T2 t2 = new T2();
            t2.SetWrapped(item);
            return t2;
        }
        public static IEnumerable<T2> Convert(IEnumerable<T1> items)
        {
            IList<T2> ret = new List<T2>();
            foreach (T1 item in items)
                ret.Add(Convert(item));
            return ret;
        }

        public static T1 Revert(T2 item)
        {
            return item.GetWrapped();
        }
        public static IEnumerable<T1> Revert(IEnumerable<T2> items)
        {
            IList<T1> ret = new List<T1>();
            foreach (T2 item in items)
                ret.Add(Revert(item));
            return ret;
        }
    }
}
