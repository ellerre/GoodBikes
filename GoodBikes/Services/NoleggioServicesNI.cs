using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoodBikes.Services
{
    //NOT IMPLEMENTED METHODS
    using Attributes;
    static partial class NoleggioServices //VERIFICARE GLI ATTRIBUTI DIPENDENTE/ADMIN
    {
        private static void Throw()
        { MessageBox.Show("Feature non implementata!", "Operazione fallita", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }

        [Admin]
        [MenuItem("Aggiungi Agevolazioni", "Agevolazioni")]
        public static void AggiungiAgevolazioni() { Throw(); }

        [Dipendente]
        [MenuItem("Elenco Agevolazioni", "Agevolazioni")]
        public static void ElencoAgevolazioni() { Throw(); }

        [Admin]
        [MenuItem("Modifica Agevolazioni", "Agevolazioni")]
        public static void ModificaAgevolazioni() { Throw(); }
    }
}
