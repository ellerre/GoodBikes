using GoodBikes.Presentation;
using Model;
using System.Windows.Forms;
using View;

namespace GoodBikes.Services
{
    using Attributes;
    using Model.Elementi;
    using Model.Persone;
    using System.Drawing;

    static partial class NoleggioServices
    {
        [Admin]
        [MenuItem("Aggiungi Elementi", "Elementi")]
        public static void AggiungiElementi()
        {
            //  Creare un nuovo elemento (servizio di Negozio)
            //  Invocare il metodo Modifica
            //  In caso di successo, aggiungere l'elemento al negozio (servizio di Document)

            string categoria = null;

            //Faccio scegliere la categoria
            using (ListForm listForm = new ListForm())
            {
                listForm.EditButton.Enabled = false;
                listForm.AddButton.Enabled = false;
                listForm.DeleteButton.Enabled = false;
                listForm.DataSource = Negozio.Categorie;
                listForm.Text = "Scelta categoria elemento";
                Label label = new Label();
                label.Text = "Scegliere la categoria dell'elemento";
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Anchor = AnchorStyles.Top;
                label.AutoSize = true;
                listForm.SetFilter(label);
                listForm.Size = new Size(568, 355);
                listForm.OkButton.DialogResult = DialogResult.OK;
                if (listForm.ShowDialog() == DialogResult.OK && listForm.DataGridView.CurrentRow != null)
                {
                    categoria = listForm.DataGridView.CurrentRow.DataBoundItem.ToString();
                }
                else return;
            }

            Elemento elemento = Negozio.NuovoElemento(categoria);
            if (NoleggioServices.Modifica(elemento, true))
            {
                Negozio.InserisciNuovoElemento(elemento);
            }
        }

        [Admin]
        [MenuItem("Aggiungi Dipendenti", "Dipendenti")]
        public static void AggiungiDipendenti()
        {
            //  Creare un nuovo elemento (servizio di Negozio)
            //  Invocare il metodo Modifica
            //  In caso di successo, aggiungere l'elemento al negozio (servizio di Negozio)

            IDipendente nuovo = Negozio.NuovoDipendente();
            if (NoleggioServices.Modifica(nuovo, true))
            {
                if (!Negozio.InserisciNuovoDipendente(nuovo))
                    MessageBox.Show("Impossibile aggiungere questo dipendente. Il nome utente esiste già.", "Errore inserimento dipendente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [Admin]
        [MenuItem("Elenco Tipologie Elementi", "Tipologie Elementi")]
        public static void ElencoTipologieElementi()
        {
            using (ListForm listForm = new ListForm())
            {
                GestioneTipologieElementiPresenter presenter = new GestioneTipologieElementiPresenter(listForm);
                listForm.ShowDialog();
            }
        }

        [Admin]
        [MenuItem("Elenco Dipendenti", "Dipendenti")]
        public static void ElencoDipendenti()
        {
            using (ListForm listForm = new ListForm())
            {
                GestioneDipendentiPresenter presenter = new GestioneDipendentiPresenter(listForm);
                listForm.ShowDialog();

            }
        }

        [Admin]
        [MenuItem("Aggiungi Tipologie Elementi", "Tipologie Elementi")]
        public static void AggiungiTipologieElementi()
        {
            string categoria = null;

            //Faccio scegliere la categoria
            using (ListForm listForm = new ListForm())
            {
                listForm.EditButton.Enabled = false;
                listForm.AddButton.Enabled = false;
                listForm.DeleteButton.Enabled = false;
                listForm.DataSource = Negozio.Categorie;
                listForm.Text = "Scelta categoria elemento";
                Label label = new Label();
                label.Text = "Scegliere la categoria dell'elemento";
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Anchor = AnchorStyles.Top;
                label.AutoSize = true;
                listForm.SetFilter(label);
                listForm.Size = new Size(568, 355);
                listForm.OkButton.DialogResult = DialogResult.OK;
                if (listForm.ShowDialog() == DialogResult.OK && listForm.DataGridView.CurrentRow != null)
                {
                    categoria = listForm.DataGridView.CurrentRow.DataBoundItem.ToString();
                }
                else return;
            }
            TipoElemento tipo_elemento = Negozio.NuovoTipoElemento(categoria);
            if (NoleggioServices.Modifica(tipo_elemento, true))
            {
                Negozio.InserisciNuovoTipoElemento(tipo_elemento);
            }
        }
    }
}