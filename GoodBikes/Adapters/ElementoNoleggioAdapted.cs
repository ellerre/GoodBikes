using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model.Noleggi;

namespace GoodBikes.Adapters
{
    class ElementoNoleggioAdapted : AAdapted<ElementoNoleggio>
    {
        public ElementoNoleggioAdapted() : base() { }
        public ElementoNoleggioAdapted(ElementoNoleggio item) : base(item) { }

        public string Corrente { get { return GetWrapped().Corrente.Id; } }
        //public IEnumerable<ISostituzione> Sostituzioni { get { return _sostituzioni; } }
        public string Originario { get { return GetWrapped().Originario.Id; } }
        public string AgevolazioneNormale { get { return GetWrapped().AgevolazioneNormale.ToString(); } }
        public string AgevolazioneEccezionale { get { return GetWrapped().AgevolazioneEccezionale != null ? GetWrapped().AgevolazioneEccezionale.ToString() : null; } }
        public string Tipo { get { return GetWrapped().Corrente.Tipo.ToString(); } }
    }
}
