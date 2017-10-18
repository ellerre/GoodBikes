using GoodBikes.Services;
using Model;
using Model.Elementi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using View;

namespace GoodBikes.Presentation
{
    class GestioneElementiPresenter
    {
        private readonly ListForm _target;
        private FilterPresenter _filterPresenter;

        public GestioneElementiPresenter(ListForm target)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            _target = target;
            _target.Text = "Gestione elementi";
            _target.AddButton.Click += AggiungiElementoAlSistema;
            _target.EditButton.Click += ModificaElemento;
            _target.DeleteButton.Click += RimuoviElementoDalSistema;

            //Inizializzazione filtri           
            _filterPresenter = new FilterPresenter(_target.DataGridView);
            target.AddFilter(_filterPresenter.InitializeFilters());
            Negozio.Changed += RefreshDataGrid;
        }

        private void RefreshDataGrid(object sender, ChangedEventArgs e)
        {
                _filterPresenter.RefreshDataGrid(sender, e);
        }

        
        public ListForm Target
        {
            get { return _target; }
        }

        private static Negozio Negozio
        {
            get { return Negozio.GetInstance(); }
        }

        private static void AggiungiElementoAlSistema(object sender, EventArgs e)
        {
            NoleggioServices.AggiungiElementi();
        }

        private void RimuoviElementoDalSistema(object sender, EventArgs e)
        {
            if (_target.DataGridView.CurrentRow != null) {
                Elemento daRimuovere = (Elemento)_target.DataGridView.CurrentRow.DataBoundItem;        
                if (MessageBox.Show(
                    "Procedere con l'eliminazione dell'elemento selezionato?", 
                    "Conferma eliminazione", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    Negozio.RimuoviElemento(daRimuovere);
                }
            }
        }

        public void ModificaElemento(object sender, EventArgs e)
        {
            //  Selezionare la persona che si vuole modificare (SelezionaDa)
            //  Invocare il metodo Modifica
            //  In caso di successo, Invocare il metodo Modifica di Document

            if ( _target.DataGridView.CurrentRow != null )
            {
                Elemento daModificare = (Elemento)_target.DataGridView.CurrentRow.DataBoundItem;
                if (NoleggioServices.Modifica(daModificare, false))
                {
                    Negozio.ModificaElemento(daModificare);
                }
            }

        }
    }
}
