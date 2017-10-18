using GoodBikes.Services;
using Model;
using Model.Agevolazioni;
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
    class GestioneAgevolazioniPresenter
    {
        private readonly ListForm _target;
        private ComboBox _filterBox;

        public GestioneAgevolazioniPresenter(ListForm target)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            _target = target;
            _target.Text = "Gestione agevolazioni";
            _target.AddButton.Click += AggiungiAgevolazioneAlSistema;
            _target.EditButton.Click += ModificaAgevolazione;
            _target.DeleteButton.Click += RimuoviAgevolazione;

            //Inizializzazione filtri e registrazione eventi relativi
            _filterBox = new ComboBox();
            target.AddFilter(_filterBox);
            initFilters(_target);
            _filterBox.SelectionChangeCommitted += RefreshTipoAgevolazione;
            Negozio.Changed += RefreshDataGrid;
        }

        private void RefreshDataGrid(object sender, ChangedEventArgs e)
        {
           if (e.TipoEvento == TipoEvento.InserimentoNuovaAgevolazione || e.TipoEvento == TipoEvento.ModificaAgevolazione|| e.TipoEvento == TipoEvento.RimozioneAgevolazione || e.TipoEvento == TipoEvento.CambiamentoFiltro)
            {
                _target.DataGridView.DataSource = typeof(BindingList<>);
                /* TO DO
                 * In base all'agevolazione selezionata, invocare la corretta proprietà di negozio
                 * per popolare la grid 
                */                  
            }
        }

        private void RefreshTipoAgevolazione(object sender, EventArgs e)
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

        private static void AggiungiAgevolazioneAlSistema(object sender, EventArgs e)
        {
            string tipologiaAgevolazione = null;

            //Faccio scegliere la categoria
            using (ListForm listForm = new ListForm())
            {
                listForm.EditButton.Enabled = false;
                listForm.AddButton.Enabled = false;
                listForm.DeleteButton.Enabled = false;
                //listForm.DataSource = Negozio.TipologieAgevolazioni; >> TO DO: implementare il metodo in Negozio! 
                listForm.Text = "Scelta tipologia agevolazione";
                Label label = new Label();
                label.Text = "Scegliere la tipologia di agevolazione";
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Anchor = AnchorStyles.Top;
                label.AutoSize = true;
                listForm.SetFilter(label);
                listForm.Size = new Size(568, 355);
                listForm.OkButton.DialogResult = DialogResult.OK;
                if (listForm.ShowDialog() == DialogResult.OK && listForm.DataGridView.CurrentRow != null)
                {
                    tipologiaAgevolazione = listForm.DataGridView.CurrentRow.DataBoundItem.ToString();
                }
                else return;
            }

            //Agevolazione agevolazione = Negozio.NuovaAgevolazione(tipologiaAgevolazione); >> TO DO: Da implementare in Negozio
            //if (NoleggioServices.Modifica(agevolazione, true))
            //{
                //Negozio.InserisciNuovaAgevolazione(tipo_agevolazione); >> TO DO: implementare in Negozio
            //}

        }

        private void RimuoviAgevolazione(object sender, EventArgs e)
        {
            // TO DO:
            // - controllare non sia conivolta in noleggi
            // - creare il metodo in Negozio

            //if (_target.DataGridView.CurrentRow != null) {
            //    TipoElemento daDisattivare = (TipoElemento)_target.DataGridView.CurrentRow.DataBoundItem;        
            //    if (MessageBox.Show(
            //        "Procedere con la disattivazione del tipo selezionato? Verranno disattivati anche tutti gli elementi di tale tipo.", 
            //        "Conferma eliminazione", 
            //        MessageBoxButtons.YesNo, 
            //        MessageBoxIcon.Exclamation) == DialogResult.Yes)
            //    {
            //        try
            //        {
            //            Negozio.DisattivaTipoElemento(daDisattivare);
            //        } catch (Exception ex)
            //        {
            //            MessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //    }
            //}
        }

        public void ModificaAgevolazione(object sender, EventArgs e)
        {
            //  Selezionare l'entità che si vuole modificare (SelezionaDa)
            //  Invocare il metodo Modifica
            //  In caso di successo, Invocare il metodo Modifica di Negozio

            // TO DO:
            // - controllare non sia conivolta in noleggi
            // - creare il metodo in Negozio

            if ( _target.DataGridView.CurrentRow != null )
            {
                Agevolazione daModificare = (Agevolazione)_target.DataGridView.CurrentRow.DataBoundItem;

                //foreach (Elemento ele in Negozio.GetInstance().Elementi.Where(el => el.Tipo == daModificare))
                //    if (ele.Stato == FactoryStatiElemento.GetStato("Noleggiabile") && !ele.IsLibero)
                //    {
                //        MessageBox.Show("Impossibile modificare mentre ci sono elementi di questo tipo con un noleggio in corso",
                //                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        return;
                //    }
                if (NoleggioServices.Modifica(daModificare, false))
                {
                    //Negozio.ModificaAgevolazione(daModificare);
                }
            }

        }

        private void initFilters(ListForm target)
        {
            //Popolo il filtro
            //_filterBox.Items.AddRange(Negozio.TipologieAgevolazioni.ToArray()); TO DO >> implementare in Negozio
            _filterBox.SelectedItem = _filterBox.Items.OfType<object>().First();

            //Chiama l'aggiornamento
            RefreshDataGrid(this, ChangedEventArgs.AggiornaFiltro());
        }
    }
}
