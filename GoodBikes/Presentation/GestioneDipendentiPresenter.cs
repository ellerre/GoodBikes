using GoodBikes.Services;
using Model;
using Model.Elementi;
using Model.Persone;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using View;

namespace GoodBikes.Presentation
{
    internal class GestioneDipendentiPresenter
    {
        private readonly ListForm _target;

        public GestioneDipendentiPresenter(ListForm target)
        {   
            //Controlli
            if (target == null)
                throw new ArgumentNullException("target");
            _target = target;

            //Label e titolo
            Label label = new Label();
            label.Text = "Dipendenti presenti nel sistema";
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Anchor = AnchorStyles.Top;
            label.AutoSize = true;
            _target.SetFilter(label);
            _target.Size = new Size(568, 355);
            _target.Text = "Gestione dipendenti";

            // Popolo la grid
            _target.DataSource = Negozio.Dipendenti.ToList();

            // Registrazione eventi
            _target.AddButton.Click += AggiungiDipendenteAlSistema;
            _target.EditButton.Click += ModificaDipendente;
            _target.DeleteButton.Click += RimuoviDipendenteDalSistema;
            Negozio.Changed += RefreshDataGrid;
        }

        private void RefreshDataGrid(object sender, ChangedEventArgs e)
        {
            if (e.TipoEvento == TipoEvento.InserimentoNuovoDipendente || e.TipoEvento == TipoEvento.RimozioneDipendente || e.TipoEvento == TipoEvento.ModificaDipendente)
            {
                _target.DataGridView.DataSource = typeof(BindingList<>);
                _target.DataGridView.DataSource = Negozio.Dipendenti.ToList();
            }

        }

        public ListForm Target
        {
            get { return _target; }
        }

        private static Negozio Negozio
        {
            get { return Negozio.GetInstance(); }
        }

        private static void AggiungiDipendenteAlSistema(object sender, EventArgs e)
        {
            NoleggioServices.AggiungiDipendenti();
        }

        public void ModificaDipendente(object sender, EventArgs e)
        {

            if (_target.DataGridView.CurrentRow != null)
            {
                IDipendente daModificare = (IDipendente)_target.DataGridView.CurrentRow.DataBoundItem;
                if (NoleggioServices.Modifica(daModificare, false))
                {
                    Negozio.ModificaDipendente(daModificare);
                }
            }
        }

        private void RimuoviDipendenteDalSistema(object sender, EventArgs e)
        {
            if (_target.DataGridView.CurrentRow != null)
            {
                IDipendente daRimuovere = (IDipendente)_target.DataGridView.CurrentRow.DataBoundItem;
                if (MessageBox.Show(
                    "Procedere con l'eliminazione del dipendente selezionato?",
                    "Conferma eliminazione",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    if (daRimuovere.NomeUtente != "admin")
                        Negozio.RimuoviDipendente(daRimuovere);
                    else
                        MessageBox.Show(
                                        "Errore",
                                        "\"admin\" non può essere eliminato",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                }
            }
        }

        
    }
}