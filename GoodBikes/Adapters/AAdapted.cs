using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBikes.Adapters
{
    abstract class AAdapted<T>
    {
        private T _wrapped;

        protected AAdapted() { }
        public void SetWrapped(T item)
        {
            this._wrapped = item;
        }
        public T GetWrapped()
        {
            return this._wrapped;
        }

        protected AAdapted(T item)
        {
            SetWrapped(item);
        }
    }
}
