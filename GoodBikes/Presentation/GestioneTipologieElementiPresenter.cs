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
    class GestioneTipologieElementiPresenter
    {
        private readonly ListForm _target;
        private ComboBox _filterBox;

        public GestioneTipologieElementiPresenter(ListForm target)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            _target = target;
            _target.Text = "Gestione tipologie elementi";
            _target.AddButton.Click += AggiungiElementoAlSistema;
            _target.EditButton.Click += ModificaTipoElemento;
            _target.DeleteButton.Click += DisattivaTipoElemento;

            //Inizializzazione filtri e registrazione eventi relativi
            _filterBox = new ComboBox();
            target.AddFilter(_filterBox);
            initFilters(_target);
            _filterBox.SelectionChangeCommitted += RefreshCategoria;
            Negozio.Changed += RefreshDataGrid;
        }

        private void RefreshDataGrid(object sender, ChangedEventArgs e)
        {
           if (e.TipoEvento == TipoEvento.InserimentoNuovoTipoElemento || e.TipoEvento == TipoEvento.ModificaTipoElemento|| e.TipoEvento == TipoEvento.DisattivazioneTipoElemento || e.TipoEvento == TipoEvento.CambiamentoFiltro)
            {
                _target.DataGridView.DataSource = typeof(BindingList<>);
                _target.DataGridView.DataSource = Negozio.GetTipiPerCategoria(_filterBox.SelectedItem.ToString()).ToList();                    
            }
        }

        private void RefreshCategoria(object sender, EventArgs e)
        {
            RefreshDataGrid(this, ChangedEventArgs.AggiornaFiltro());
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
            NoleggioServices.AggiungiTipologieElementi();
        }

        private void DisattivaTipoElemento(object sender, EventArgs e)
        {
            if (_target.DataGridView.CurrentRow != null) {
                TipoElemento daDisattivare = (TipoElemento)_target.DataGridView.CurrentRow.DataBoundItem;        
                if (MessageBox.Show(
                    "Procedere con la disattivazione del tipo selezionato? Verranno disattivati anche tutti gli elementi di tale tipo.", 
                    "Conferma eliminazione", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    try
                    {
                        Negozio.DisattivaTipoElemento(daDisattivare);
                    } catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void ModificaTipoElemento(object sender, EventArgs e)
        {
            //  Selezionare l'entità che si vuole modificare (SelezionaDa)
            //  Invocare il metodo Modifica
            //  In caso di successo, Invocare il metodo Modifica di Negozio

            if ( _target.DataGridView.CurrentRow != null )
            {
                TipoElemento daModificare = (TipoElemento)_target.DataGridView.CurrentRow.DataBoundItem;

                foreach (Elemento ele in Negozio.GetInstance().Elementi.Where(el => el.Tipo == daModificare))
                    if (ele.Stato == FactoryStatiElemento.GetStato("Noleggiabile") && !ele.IsLibero)
                    {
                        MessageBox.Show("Impossibile modificare mentre ci sono elementi di questo tipo con un noleggio in corso",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                if (NoleggioServices.Modifica(daModificare, false))
                {
                    Negozio.ModificaTipoElemento(daModificare);
                }
            }

        }

        private void initFilters(ListForm target)
        {
            //Popolo il filtro
            _filterBox.Items.AddRange(Negozio.Categorie.ToArray());
            _filterBox.SelectedItem = _filterBox.Items.OfType<object>().First();

            //Chiama l'aggiornamento
            RefreshDataGrid(this, ChangedEventArgs.AggiornaFiltro());
        }
    }
}
