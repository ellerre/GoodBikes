using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model.Noleggi;

namespace GoodBikes.Adapters
{
    class NoleggioAdapted : AAdapted<Noleggio>
    {
        public NoleggioAdapted() : base() { }
        public NoleggioAdapted(Noleggio item) : base(item) { }

        public string NomeCliente { get { return GetWrapped().Cliente.ToString(); } }
        public string DataOraInizio { get { return GetWrapped().DataOraInizio.ToString(); } }
        public string DataOraFineStimata { get { return GetWrapped().DataOraFineStimata.ToString(); } }
        public string DataOraFine { get { return GetWrapped().DataOraFine != default(DateTime) ? GetWrapped().DataOraFine.ToString() : null; } }
        public string PrezzoPagato { get { return GetWrapped().PrezzoPagato != -1 ? GetWrapped().PrezzoPagato.ToString() : null; } }
        public string Durata { get { return DataOraFine != null ? GetWrapped().Durata.ToString() : null; } }
        //public string IsChiuso { get { return GetWrapped().IsChiuso.ToString(); } }
    }
}
