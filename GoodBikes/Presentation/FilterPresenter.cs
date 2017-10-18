using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using View;

namespace GoodBikes.Presentation
{
    internal class FilterPresenter
    {
        private ElementoUpperPanel _filterPanel;
        private DataGridView _dataGridView;

        private static readonly string ALL_STATES = "Tutti gli stati";
        private static readonly string ALL_CATEGORIES = "Tutte le categorie";
        private static readonly string ALL_TYPES = "Tutti i tipi";

        #region Constructor
        public FilterPresenter(DataGridView dataGridView)
        {
            if (dataGridView == null)
                throw new ArgumentNullException("data grid view must be not null");
            _filterPanel = new ElementoUpperPanel();
            _dataGridView = dataGridView;
        }
        #endregion
        #region Properties
        public ElementoUpperPanel UpperPanel { get { return _filterPanel; } }
        public DataGridView DataGridView { get { return _dataGridView; } }
        private Negozio Negozio { get { return Negozio.GetInstance(); } }
        #endregion
        #region Methods
        public ElementoUpperPanel InitializeFilters()
        {
            // Popolo la grid
            DataGridView.DataSource = Negozio.Elementi.ToList();

            //Creo il pannello
            _filterPanel = new ElementoUpperPanel();

            _filterPanel.ComboCategoriaElemento.Items.Add(ALL_CATEGORIES);
            _filterPanel.ComboCategoriaElemento.Items.AddRange(Negozio.Categorie.ToArray());
            _filterPanel.ComboCategoriaElemento.SelectedItem = _filterPanel.ComboCategoriaElemento.Items.OfType<string>().First();

            _filterPanel.ComboStatoElemento.Items.Add(ALL_STATES);
            _filterPanel.ComboStatoElemento.Items.AddRange(Negozio.StatiElemento.ToArray());
            _filterPanel.ComboStatoElemento.SelectedItem = _filterPanel.ComboStatoElemento.Items.OfType<string>().First();

            _filterPanel.ComboTipoElemento.Items.Clear();
            _filterPanel.ComboTipoElemento.Items.Add(ALL_TYPES);
            _filterPanel.ComboTipoElemento.SelectedItem = _filterPanel.ComboTipoElemento.Items.OfType<string>().First();
            _filterPanel.ComboTipoElemento.Enabled = false;

            _filterPanel.ComboStatoElemento.SelectionChangeCommitted += RefreshStati;
            _filterPanel.ComboCategoriaElemento.SelectionChangeCommitted += RefreshCategoria;
            _filterPanel.ComboTipoElemento.SelectionChangeCommitted += RefreshTipo;

            return _filterPanel;
        }

        private void RefreshStati(object sender, EventArgs e)
        {
            RefreshDataGrid(this, ChangedEventArgs.AggiornaFiltro());
        }
        private void RefreshTipo(object sender, EventArgs e)
        {
            RefreshDataGrid(this, ChangedEventArgs.AggiornaFiltro());
        }
        private void RefreshCategoria(object sender, EventArgs e)
        {
            //In base a quanto scelto in categoria, si determina come popolare la combo
            //dei tipi

            if (_filterPanel.ComboCategoriaElemento.SelectedItem.ToString() == ALL_CATEGORIES)
            {
                _filterPanel.ComboTipoElemento.SelectedItem = ALL_TYPES;
                _filterPanel.ComboTipoElemento.Enabled = false;
            }
            else
            {
                _filterPanel.ComboTipoElemento.Items.Clear();
                _filterPanel.ComboTipoElemento.Items.Add(ALL_TYPES);
                _filterPanel.ComboTipoElemento.SelectedItem = _filterPanel.ComboTipoElemento.Items.OfType<string>().First();
                _filterPanel.ComboTipoElemento.Items.AddRange(Negozio.GetTipiPerCategoria(((ComboBox)sender).SelectedItem.ToString()).ToArray());
                _filterPanel.ComboTipoElemento.Enabled = true;

            }

            RefreshDataGrid(this, ChangedEventArgs.AggiornaFiltro());
        }

        public void RefreshDataGrid(object sender, ChangedEventArgs e)
        {
            if (e.TipoEvento == TipoEvento.InserimentoNuovoElemento || e.TipoEvento == TipoEvento.RimozioneElemento || e.TipoEvento == TipoEvento.ModificaElemento || e.TipoEvento == TipoEvento.CambiamentoFiltro)
            {
                DataGridView.DataSource = typeof(BindingList<>);
                // Un elemento viene incluso se , per ogni criterio (risp. stato, categoria, tipo di quella categoria):
                //  > o, per quel criterio, è stato scelto di visualizzare tutti ("ALL_<criterio>") gli elementi,
                //  > oppure, in caso contrario, se soddisfano il criterio specificato.
               DataGridView.DataSource = Negozio.Elementi.Where(el =>
                                           (_filterPanel.ComboStatoElemento.SelectedItem.ToString() == ALL_STATES || _filterPanel.ComboStatoElemento.SelectedItem.ToString() == el.Stato.GetType().Name) &&
                                           (_filterPanel.ComboCategoriaElemento.SelectedItem.ToString() == ALL_CATEGORIES || _filterPanel.ComboCategoriaElemento.SelectedItem.ToString() == el.Categoria) &&
                                           (_filterPanel.ComboTipoElemento.SelectedItem.ToString() == ALL_TYPES || _filterPanel.ComboTipoElemento.SelectedItem.ToString() == el.Tipo.Nome)).ToList();
            }

        }
        #endregion
    }
}
