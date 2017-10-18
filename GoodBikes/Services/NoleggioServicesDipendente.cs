using GoodBikes.Presentation;
using Model;
using Model.Elementi;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using View;
using System2;
using System;
using System.Reflection;

namespace GoodBikes.Services
{
    using Attributes;
    static partial class NoleggioServices
    {
        [Dipendente]
        [MenuItem("Elenco Stati Elementi", "Elementi")]
        public static void ElencoStatiElementi()
        {
            using (ListForm listForm = new ListForm())
            {
                listForm.EditButton.Enabled = false;
                listForm.AddButton.Enabled = false;
                listForm.DeleteButton.Enabled = false;
                listForm.DataSource = Negozio.StatiElemento.ToList();
                listForm.Text = "Elenco stati elementi";
                Label label = new Label();
                label.Text = "Stati in cui un elmento si può trovare";
                label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                label.Anchor = AnchorStyles.Top;
                label.AutoSize = true;
                listForm.SetFilter(label);
                listForm.Size = new Size(850, 355);
                listForm.ShowDialog();
            }
        }

        [Dipendente]
        [MenuItem("Elenco Categorie", "Categorie")]
        public static void ElencoCategorie()
        {
            using (ListForm listForm = new ListForm())
            {

                listForm.EditButton.Enabled = false;
                listForm.AddButton.Enabled = false;
                listForm.DeleteButton.Enabled = false;
                listForm.DataSource = Negozio.Categorie;
                listForm.Text = "Elenco categorie";
                Label label = new Label();
                label.Text = "Categorie di elementi presenti nel sistema";
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Anchor = AnchorStyles.Top;
                label.AutoSize = true;
                listForm.SetFilter(label);
                listForm.Size = new Size(568, 355);
                listForm.ShowDialog();
            }
        }

        [Dipendente]
        public static void InserisciNuovoNoleggio()
        {
            if (!TentaInserimentoNuovoNoleggio())
                MessageBox.Show("Nessun noleggio è stato inserito", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private static bool TentaInserimentoNuovoNoleggio()
        {
            using (NoleggioFormCreate creator = new NoleggioFormCreate())
            {
                GestioneNoleggioPresenter gestioneNoleggioPresenter = new GestioneNoleggioPresenter(creator);
                if (creator.ShowDialog() == DialogResult.OK)
                {
                    gestioneNoleggioPresenter.AvviaNoleggio();
                    return true;
                }
            }
            return false;
        }

        [Dipendente]
        [MenuItem("Elenco Fasce Orarie", "Fasce Orarie")]
        public static void ElencoFasceOrarie()
        {
            using (ListForm listForm = new ListForm())
            {
                listForm.EditButton.Enabled = false;
                listForm.AddButton.Enabled = false;
                listForm.DeleteButton.Enabled = false;
                listForm.DataSource = Negozio.FasceOrarie.ToList();
                listForm.Text = "Elenco fasce orarie";
                Label label = new Label();
                label.Text = "Fasce orarie in base a cui avviene la tariffazione";
                label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                label.Anchor = AnchorStyles.Top;
                label.AutoSize = true;
                listForm.SetFilter(label);
                listForm.Size = new Size(568, 355);
                listForm.ShowDialog();
            }
        }

        [Dipendente]
        [MenuItem("Elenco Elementi", "Elementi")]
        public static void ElencoElementi()
        {
            using (ListForm listForm = new ListForm())
            {
                GestioneElementiPresenter presenter = new GestioneElementiPresenter(listForm);
                listForm.ShowDialog();
            }
        }
    }
}