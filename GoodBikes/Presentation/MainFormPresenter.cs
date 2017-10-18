using System;
using System.Linq;
using View;
using GoodBikes.Services;
using Model;
using Model.Noleggi;
using Model.Elementi;

using System2.Reflection;
using GoodBikes.Adapters;
using System.Windows.Forms;
using Model.Persone;
using System.Collections;
using System.Collections.Generic;

namespace GoodBikes.Presentation
{
    public partial class MainFormPresenter
    {
        private readonly MainForm _target;
        private readonly TabPresenter<Noleggio, NoleggioAdapted> _noleggiTabPresenter;
        private MainFormMenuPresenter _mainFormMenuPresenter;

        public MainFormPresenter(MainForm target)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            _target = target;
            _target.AutoSize = true;

            TabGrid tg = new TabGrid(typeof(Noleggio).Name);
            _target.ElenchiTab.TabPages.Add(tg);
            _noleggiTabPresenter = new TabPresenter<Noleggio, NoleggioAdapted>(tg.DataGridView,
                Negozio.Noleggi, el => el is Noleggio && !el.IsChiuso//el is Noleggio al momento non è indispensabile
                );
            Negozio.Changed += _noleggiTabPresenter.RefreshView;
            
            // Creazione delle tab con gli elementi
            foreach (Type t in typeof(Elemento).GetSubtypes())
            {
                TabGrid tbg = new TabGrid(t.Name);
                _target.ElenchiTabControl.TabPages.Add(tbg);
                Negozio.Changed += new TabPresenter<Elemento, ElementoAdapted>(tbg.DataGridView,
                    Negozio.Elementi, el => el.GetType() == t && el.IsLibero).RefreshView;
            }
            _target.RestituisciElementiButton.Click += ConsegnaElementi;
            _target.SostituisciElementiButton.Click += SostituisciElementi;

            _mainFormMenuPresenter = new MainFormMenuPresenter(Target.MenuStrip);
            CommandManager.RegisterTarget(Target);
            NoleggioServices.RegisterCommands();
        }

        

        #region Proprietà
        public MainForm Target
        {
            get { return _target; }
        }

        private static Negozio Negozio
        {
            get { return Negozio.GetInstance(); }
        }
        #endregion

        #region Gestori metodi

        private void SostituisciElementi(object sender, EventArgs e)
        {
            if (_noleggiTabPresenter.DataGridView.CurrentRow != null)
            {
                Noleggio noleggio = ((NoleggioAdapted)_noleggiTabPresenter.DataGridView.CurrentRow.DataBoundItem).GetWrapped();
                Console.WriteLine(noleggio.DataOraInizio);

                Label message = new Label();
                message.Width = 1000;
                message.Text = "Scegli l'elemento del noleggio da sostituire.";

                ChooseForm chooseForm= new ChooseForm();
                chooseForm.DataGridView.MultiSelect = false;
                chooseForm.Text = "Scelta elemento da sostituire";
                chooseForm.AddFilter(message);
                chooseForm.DataGridView.DataSource = Adapter<ElementoNoleggio, ElementoNoleggioAdapted>.Convert(noleggio.ElementiNoleggio);
                chooseForm.ShowDialog();

                IEnumerator rowEnum = chooseForm.DataGridView.SelectedRows.GetEnumerator();
                rowEnum.MoveNext();
                if (chooseForm.DialogResult == DialogResult.OK &&  rowEnum.Current != null)
                {
                    ElementoNoleggio elementoNoleggio = ((ElementoNoleggioAdapted)((DataGridViewRow)rowEnum.Current).DataBoundItem).GetWrapped();
                    
                    Elemento daSostituire = elementoNoleggio.Originario;
                    Console.WriteLine("Tipo da cercare: " + daSostituire.GetType());
                    List<Elemento> validi = new List<Elemento>();
                    foreach(Elemento elemento in Negozio.Elementi)
                    {
                        if(elemento.IsLibero && elemento.GetType().Equals(daSostituire.GetType()))
                        {
                            validi.Add(elemento);
                        }
                    }
                    if (validi.Count == 0)
                    {
                        MessageBox.Show("Nel negozio non sono presenti elementi per la sostituzione", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    message.Text = "Scegli l'elemento con cui sostituire " + daSostituire.Id + ".";
                    chooseForm.Text = "Scelta nuovo elemento";
                    chooseForm.DataGridView.DataSource = Adapter<Elemento, ElementoAdapted>.Convert(validi);
                    chooseForm.FormClosing += delegate(object inSender, FormClosingEventArgs inE) 
                    {
                        if (MessageBox.Show("Confermare la sostituzione di " + daSostituire.Id + " ?", "Conferma sostituzione", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                            inE.Cancel = true;                            
                    };
                    chooseForm.ShowDialog();

                    Elemento sostituto;
                    rowEnum = chooseForm.DataGridView.SelectedRows.GetEnumerator();
                    rowEnum.MoveNext();
                    if (chooseForm.DialogResult == DialogResult.OK && rowEnum.Current != null)
                    {
                        sostituto = ((ElementoAdapted)((DataGridViewRow)rowEnum.Current).DataBoundItem).GetWrapped();
                        Console.WriteLine("Sostituisco " + daSostituire.Id + " con " + sostituto.Id);
                        elementoNoleggio.SostituisciCon(sostituto, DateTime.Now, Logged.User);
                    }
                }
            }
        }

        private void ConsegnaElementi(object sender, EventArgs e)
        {
            if (_noleggiTabPresenter.DataGridView.CurrentRow != null)
            {
                Noleggio daTerminare = ((NoleggioAdapted)_noleggiTabPresenter.DataGridView.CurrentRow.DataBoundItem).GetWrapped();
                if (daTerminare.IsChiuso)
                {
                    MessageBox.Show("Noleggio già concluso!", "Operazione non permessa", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                NoleggioFormEditOrView form = new NoleggioFormEditOrView();
                ChiusuraNoleggioPresenter presenter = new ChiusuraNoleggioPresenter(form, daTerminare);
                if (form.ShowDialog() == DialogResult.OK)
                    Negozio.ConcludiNoleggio(daTerminare, Logged.User);
            }
            
        }
        #endregion
    }
}
