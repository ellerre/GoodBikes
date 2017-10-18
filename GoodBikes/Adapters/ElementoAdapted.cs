using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model.Elementi;

namespace GoodBikes.Adapters
{
    class ElementoAdapted : AAdapted<Elemento>
    {
        public ElementoAdapted() : base() { }
        public ElementoAdapted(Elemento item) : base(item) { }

        public string Id { get { return GetWrapped().Id; } }
        public string Descrizione { get { return GetWrapped().Descrizione; } }
        public string Tipo { get { return GetWrapped().Tipo.ToString(); } }
        //public string Stato { get { return GetWrapped().Stato.ToString(); } }
    }
}
