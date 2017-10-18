using GoodBikes.Services;
using Model;
using Model.Agevolazioni;
using Model.Elementi;
using Model.Noleggi;
using Model.Persone;
using System;
using System2;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using View;
using System.Reflection;
using System.Collections;

namespace GoodBikes.Presentation
{
    using Adapters;
    class GestioneNoleggioPresenter
    {
        private readonly NoleggioFormCreate _target;
        private EditingControlPresenter _editingControlPresenter;
        private Noleggio _noleggio;
        private Cliente _cliente;
        private IDipendente _dipendenteInizio;

        public GestioneNoleggioPresenter(NoleggioFormCreate target)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            _target = target;
            _target.Text = "Creazione nuovo noleggio";
            _target.AggiungiElementoAlNoleggioButton.Click += AggiungiElementoAlNoleggio;
            _target.RimuoviElementoDalNoleggioButton.Click += RimuoviElementoDalNoleggio;
            _target.ComboTipologieCliente.SelectionChangeCommitted += RefreshEditingControl;
            _target.CercaClienteButton.Click += CercaCliente;
            _target.ResetClienteButton.Click += ResetButtonEditingControl;

            //Prototipo! Non gestiamo prenotazioni!
            _target.DatePicker.Enabled = false;

            _target.PrezzoSingoloElementoStatus.Text = "Nessun elemento selezionato";
            _target.ElementiNoleggioDataGridView.SelectionChanged += RefreshPrezzoStatusStrip;
            _target.DatePicker.ValueChanged += RefreshPrezzoStatusStrip;
            _target.TimePicker.ValueChanged += RefreshPrezzoStatusStrip;

            _target.ElementiNoleggioDataGridView.DataSourceChanged += RefreshPrezzoTotale;
            _target.DatePicker.ValueChanged += RefreshPrezzoTotale;
            _target.TimePicker.ValueChanged += RefreshPrezzoTotale;

            PopolaComboClienti(_target.ComboTipologieCliente);
            if (_editingControlPresenter == null)
                _editingControlPresenter = new EditingControlPresenter(Target.EditingControl, true);

            _noleggio = Negozio.NuovoNoleggio();
            _dipendenteInizio = Logged.User;

            //Serve per verificare i dati; il valore effettivo viene aggiunto al click sul bottone avvia
            _noleggio.DataOraInizio = DateTime.Now;
        }

        public NoleggioFormCreate Target
        {
            get { return _target; }
        }
        private static Negozio Negozio
        {
            get { return Negozio.GetInstance(); }
        }
        private EditingControlPresenter EditingControlPresenter
        {
            get { return _editingControlPresenter; }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message,
                           "Creazione noleggio impossibile",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error);
        }

        private bool isValidTime()
        {
            return (Target.DatePicker.Value.Date + Target.TimePicker.Value.TimeOfDay).CompareTo(DateTime.Now) >= 0;
        }

        public void AvviaNoleggio()
        {
            _noleggio.Cliente = _cliente;
            try
            {
                _noleggio.ElementiNoleggio = Adapter<ElementoNoleggio, ElementoNoleggioAdapted>
                    .Revert(Target.ElementiNoleggioDataGridView.DataSource as IList<ElementoNoleggioAdapted>) as IList<ElementoNoleggio>;
                //Se qualche elemento viene usato per la prima volta, questo deve essere registrato
                foreach (ElementoNoleggio eln in _noleggio.ElementiNoleggio)
                    if (!eln.Corrente.HasBeenUsed())
                        eln.Corrente.setUsed(true);
            }
            catch
            {
                //Comunque non può succedere, perché il bottone "avvia" sarebbe disabilitato.
                //Lasciamo per evitare in futuro che, cambiando, ci siano malfunzionamenti
                ShowError("Selezionare almeno un elemento per il noleggio");
                return;
            }

            _noleggio.DataOraInizio = DateTime.Now;
            try
            {
                _noleggio.DataOraFineStimata = Target.DatePicker.Value.Date + Target.TimePicker.Value.TimeOfDay;
            }
            catch
            {
                ShowError("Ora di fine stimata del noleggio precedente all'orario di inizio");
                return;
            }
            //tutto a buon fine
            _noleggio.DipendenteInizio = _dipendenteInizio;
            Negozio.InserisciNuovoNoleggio(_noleggio);
            Negozio.InserisciCliente(_cliente);
        }

        private void AggiungiElementoAlNoleggio(object sender, EventArgs e)
        {
            using (ElementiNoleggioChooseForm chooseForm = new ElementiNoleggioChooseForm())
            {
                ElementoNoleggioUpperPanel upperPanel = new ElementoNoleggioUpperPanel();
                chooseForm.UpperPanel = upperPanel;

                #region Gestione filtro
                string ALL_CATEGORIES = "Tutte le categorie";
                string ALL_TYPES = "Tutti i tipi";

                chooseForm.Text = "Scelta elemento da inserire";
                chooseForm.UpperPanel.StatoElemento.Items.Add("Libero");
                chooseForm.UpperPanel.StatoElemento.SelectedItem = "Libero";
                upperPanel.StatoElemento.Enabled = false;

                chooseForm.UpperPanel.CategoriaElemento.Items.Add("Tutte le categorie");
                upperPanel.CategoriaElemento.Items.AddRange(Negozio.Categorie.ToArray());
                upperPanel.CategoriaElemento.SelectedItem = upperPanel.CategoriaElemento.Items.OfType<string>().First();
                upperPanel.CategoriaElemento.Enabled = true;

                chooseForm.UpperPanel.TipoElemento.Items.Add("Tutti i tipi");
                upperPanel.TipoElemento.SelectedItem = upperPanel.TipoElemento.Items.OfType<string>().First();
                upperPanel.TipoElemento.Enabled = false;

                //Gestore filtro categorie
                EventHandler handlerCategorie = (object s, EventArgs ev) =>
                {
                    if (chooseForm.UpperPanel.CategoriaElemento.SelectedItem.ToString() == ALL_CATEGORIES)
                    {
                        upperPanel.TipoElemento.SelectedItem = ALL_TYPES;
                        upperPanel.TipoElemento.Enabled = false;
                    }
                    else
                    {
                        upperPanel.TipoElemento.Items.Clear();
                        upperPanel.TipoElemento.Items.Add(ALL_TYPES);
                        upperPanel.TipoElemento.SelectedItem = upperPanel.TipoElemento.Items.OfType<string>().First();
                        upperPanel.TipoElemento.Items.AddRange(Negozio.GetTipiPerCategoria(upperPanel.CategoriaElemento.SelectedItem.ToString()).ToArray());
                        upperPanel.TipoElemento.Enabled = true;

                    }
                   chooseForm.DataGridView.DataSource = typeof(BindingList<>);
                   chooseForm.DataGridView.DataSource = Adapter<Elemento, ElementoAdapted>.Convert(Negozio.Elementi.Where(el =>
                       el.IsLibero && !ContenutoNelNoleggio(el) && 
                        (upperPanel.CategoriaElemento.SelectedItem.ToString() == ALL_CATEGORIES || el.GetType().Name == upperPanel.CategoriaElemento.SelectedItem.ToString())
                        ).ToList());
                };
                //Gestore filtro tipi
                EventHandler handlerTipi = (object s, EventArgs ev) =>
                {
                    chooseForm.DataGridView.DataSource = typeof(BindingList<>);
                    chooseForm.DataGridView.DataSource = Adapter<Elemento, ElementoAdapted>.Convert(Negozio.Elementi.Where(el =>
                        el.IsLibero && !ContenutoNelNoleggio(el) &&
                         ((upperPanel.CategoriaElemento.SelectedItem.ToString() == ALL_CATEGORIES || el.GetType().Name == upperPanel.CategoriaElemento.SelectedItem.ToString()) &&
                         (upperPanel.TipoElemento.SelectedItem.ToString() == ALL_TYPES || el.Tipo.Nome == upperPanel.TipoElemento.SelectedItem.ToString()))
                         ).ToList());
                };

                //Aggancio i gestori
                upperPanel.CategoriaElemento.SelectionChangeCommitted += handlerCategorie;
                upperPanel.TipoElemento.SelectionChangeCommitted += handlerTipi;

                // Popolo la grid (stato iniziale)
                handlerCategorie.Invoke(this, EventArgs.Empty);

                #endregion

                //Combo agevolazioni
                foreach (IAgevolazioneNormale a in Negozio.AgevolazioniNormali.Where(ag => ag.IsValidaOggi))
                {
                    chooseForm.UpperPanel.AddAgevolazioneNormale(a);
                    if ((a as Agevolazione).Nome == "Nessuna agevolazione")
                        chooseForm.UpperPanel.AgevolazioneBox.SelectedItem = a;
                }

                //Mostro la grid
                chooseForm.ShowDialog();

                if (chooseForm.DialogResult == DialogResult.OK)
                {
                    if (chooseForm.DataGridView.CurrentRow != null)
                    {
                        ElementoNoleggio daAggiungere = new ElementoNoleggioConcreto(
                                                        Adapter<Elemento, ElementoAdapted>.Revert((ElementoAdapted)chooseForm.DataGridView.CurrentRow.DataBoundItem),
                                                        (IAgevolazioneNormale)chooseForm.UpperPanel.AgevolazioneNormale);
                        _noleggio.ElementiNoleggio.Add(daAggiungere);
                        _target.ElementiNoleggioDataGridView.DataSource = typeof(BindingList<>);
                        _target.ElementiNoleggioDataGridView.DataSource = Adapter<ElementoNoleggio, ElementoNoleggioAdapted>.Convert(_noleggio.ElementiNoleggio);
                    }
                }
                // In ogni caso, rimuovo i gestori
                upperPanel.CategoriaElemento.SelectionChangeCommitted -= handlerCategorie;
                upperPanel.TipoElemento.SelectionChangeCommitted -= handlerTipi;
            }
        }

        private void RimuoviElementoDalNoleggio(object sender, EventArgs e)
        {
            ElementoNoleggio elemento = null;
            try
            {
                elemento = Adapter<ElementoNoleggio, ElementoNoleggioAdapted>.Revert((ElementoNoleggioAdapted)_target.ElementiNoleggioDataGridView.CurrentRow.DataBoundItem);
            }
            catch (Exception)
            {
                MessageBox.Show("Nessun elemento selezionato!");
            }

            _noleggio.ElementiNoleggio.Remove(elemento);
            _target.ElementiNoleggioDataGridView.DataSource = typeof(BindingList<>);
            _target.ElementiNoleggioDataGridView.DataSource = Adapter<ElementoNoleggio, ElementoNoleggioAdapted>.Convert(_noleggio.ElementiNoleggio);
        }

        private bool ContenutoNelNoleggio(Elemento e)
        {
            foreach (ElementoNoleggio eln in _noleggio.ElementiNoleggio)
            {
                if (eln.Corrente.Equals(e))
                    return true;
            }
            return false;
        }

        private void RefreshEditingControl(object sender, EventArgs args)
        {
            //Creo istanza di cliente. La creo qui, perchè se cambio il tipo cliente
            //selezionato, cambierò anche la classe da istanziare
            if (((ComboBox)sender).Items.Count > 0)
            {
                _cliente = Negozio.NuovoCliente(((ComboBox)sender).SelectedItem.ToString());
            }
            if (_editingControlPresenter == null)
                _editingControlPresenter = new EditingControlPresenter(Target.EditingControl, true);
            EditingControlPresenter.SetEditableObject(_cliente);
            Application.Idle += Application_Idle;
        }

        private void ResetButtonEditingControl(object sender, EventArgs args)
        {
            RefreshEditingControl(Target.ComboTipologieCliente, EventArgs.Empty);
        }

        private void PopolaComboClienti(ComboBox comboClienti)
        {
            comboClienti.Items.AddRange(Negozio.GetNomiTipiClienti().ToArray());
            comboClienti.SelectedItem = comboClienti.Items.OfType<string>().First();
            RefreshEditingControl(comboClienti, EventArgs.Empty);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            Target.AvviaNoleggioButton.Enabled = !EditingControlPresenter.HasError && isValidTime() && Target.ElementiNoleggioDataGridView.CurrentRow != null;

        }

        private void RefreshPrezzoStatusStrip(object sender, EventArgs e)
        {
            if (!isValidTime())
            {
                Target.PrezzoSingoloElementoStatus.Text = "Durata del noleggio non valida";
                return;
            }
            else
            {
                if (Target.ElementiNoleggioDataGridView.CurrentRow == null)
                    Target.PrezzoSingoloElementoStatus.Text = "Nessun elemento selezionato";
                else
                {
                    ElementoNoleggio current = Adapter<ElementoNoleggio, ElementoNoleggioAdapted>.Revert((ElementoNoleggioAdapted)Target.ElementiNoleggioDataGridView.CurrentRow.DataBoundItem);
                    Target.PrezzoSingoloElementoStatus.Text = "Prezzo elemento " + current.Corrente.Id + ": " +
                                                                                current.CalcolaPrezzo((Target.DatePicker.Value.Date + Target.TimePicker.Value.TimeOfDay) - _noleggio.DataOraInizio, Negozio.MINUTI_TOLLERANZA).ToEuroString();
                }
            }
        }

        private void RefreshPrezzoTotale(object sender, EventArgs e)
        {
            //Calcolabile solo se la durata indicata è positiva
            if (!isValidTime())
            {
                Target.PrezzoTotaleElementoTextBox.Text = "Durata non valida";
                return;
            }
            else
            {
                if (Target.ElementiNoleggioDataGridView.CurrentRow == null)
                    Target.PrezzoTotaleElementoTextBox.Text = "0 €";
                else
                {
                    _noleggio.DataOraFineStimata = Target.DatePicker.Value.Date + Target.TimePicker.Value.TimeOfDay;
                    _noleggio.ElementiNoleggio = Adapter<ElementoNoleggio, ElementoNoleggioAdapted>
                        .Revert(Target.ElementiNoleggioDataGridView.DataSource as IList<ElementoNoleggioAdapted>) as IList<ElementoNoleggio>;
                    Target.PrezzoTotaleElementoTextBox.Text = _noleggio.CalcolaPrezzo((Target.DatePicker.Value.Date + Target.TimePicker.Value.TimeOfDay) - _noleggio.DataOraInizio, Negozio.MINUTI_TOLLERANZA).ToEuroString();
                }
            }
        }

        private void CercaCliente(object sender, EventArgs args)
        {
            //Scelgo il cliente
            Cliente selezionato = null;
            using (ListForm listForm = new ListForm())
            {
                listForm.Text = "Scegli cliente";
                listForm.EditButton.Enabled = false;
                listForm.AddButton.Enabled = false;
                listForm.DeleteButton.Enabled = false;
                //Trovo il tipo del cliente specifico
                Type tipoCliente = Negozio.GetTipoCliente((string)Target.ComboTipologieCliente.SelectedItem);
                //Costruisco una lista di quel tipo specifico, così ne vedrò mostrate le proprietà
                Type listType = typeof(List<>).MakeGenericType(new[] { tipoCliente });
                IList list = (IList)Activator.CreateInstance(listType);
                //Aggiungo gli elementi alla lista
                foreach (Cliente o in Negozio.Clienti.Where(cl => cl.GetType() == tipoCliente))
                    list.Add(Convert.ChangeType(o, tipoCliente));
                //Aggiungo la lista alla gridView
                listForm.DataSource = list;
                Label label = new Label();
                label.Text = "Scegliere un cliente dall'elenco";
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Anchor = AnchorStyles.Top;
                label.AutoSize = true;
                listForm.SetFilter(label);
                listForm.Size = new Size(568, 355);
                listForm.OkButton.DialogResult = DialogResult.OK;
                try
                {
                    if (listForm.ShowDialog() == DialogResult.OK && listForm.DataGridView.CurrentRow != null)
                    {
                        selezionato = (Cliente)listForm.DataGridView.CurrentRow.DataBoundItem;
                    }
                    else return;
                }
                catch { selezionato = (Cliente)listForm.DataGridView.CurrentRow.DataBoundItem; }
            }

            //Imposto le sue info nel noleggio
            _editingControlPresenter.SetEditableObject(selezionato);
            //Imposto tale cliente come quello associato correntemente al noleggio
            _cliente = selezionato;
        }
    }
}
