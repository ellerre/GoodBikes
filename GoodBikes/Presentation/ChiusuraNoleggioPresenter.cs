using Model.Noleggi;
using Model.Persone;
using System;
using System2;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using View;
using Model;
using Model.Agevolazioni;
using Model.Elementi;
using System.Reflection;

namespace GoodBikes.Presentation
{
    using Adapters;
    class ChiusuraNoleggioPresenter
    {
        private NoleggioFormEditOrView _target;
        private Noleggio _noleggio;
        private Cliente _cliente;

        private DateTime _fineEffettiva;
        private TimeSpan _durataEffettiva;

        public ChiusuraNoleggioPresenter(NoleggioFormEditOrView target, Noleggio noleggio)
        {
            _noleggio = noleggio;
            _cliente = _noleggio.Cliente;
            _target = target;
            _target.Text = "Chiusura noleggio";
            //Eventi
            _target.AggiungiAgevolazioneEccezionale.Click += AggiungiAgevolazioneEccezionale;
            _target.ElementiNoleggioDataGridView.SelectionChanged += ShowPrezzoSingoloElemento;

            //Popolamento tabella elementi noleggiati
            _target.ElementiNoleggioDataGridView.DataSource = Adapter<ElementoNoleggio, ElementoNoleggioAdapted>.Convert(_noleggio.ElementiNoleggio);

            //Comunicazione della durata e del noleggio
            _fineEffettiva = DateTime.Now;
            _durataEffettiva = _fineEffettiva - _noleggio.DataOraInizio;
            _target.DurataLabel.Text = string.Format("Durata noleggio: {0} giorni, {1} ore, {2} minuti",
                _durataEffettiva.Days, _durataEffettiva.Hours, _durataEffettiva.Minutes);

            //Comunicazione del prezzo del noleggio
            _target.PrezzoTotaleLabel.Text = string.Format("Prezzo totale: {0}", _noleggio.CalcolaPrezzo(_durataEffettiva, Negozio.MINUTI_TOLLERANZA).ToEuroString());

            //Popolamento dati cliente
            TableLayoutPanel clientePanel = _target.DatiClientePanel;
            foreach (PropertyInfo info in _cliente.GetType().GetProperties())
            {
                Label nomeProprietà = new Label();
                nomeProprietà.Text = (info.GetCustomAttributes(typeof(EditableAttribute), false)[0] as EditableAttribute).Label;
                Label valoreProprietà = new Label();
                valoreProprietà.Text = info.GetValue(_cliente).ToString();
                clientePanel.Controls.Add(nomeProprietà);
                clientePanel.Controls.Add(valoreProprietà);
            }

            _target.FormClosing += CheckFascia;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message,
                           "Chiusura noleggio impossibile",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error);
        }

        //Controlla che esista una fascia oraria da applicare al noleggio
        private void CheckFascia(object sender, FormClosingEventArgs e)
        {
            if ((sender as Form).DialogResult == DialogResult.OK)
                try
                {
                    FactoryFasceOrarie.Ricava(_fineEffettiva - _noleggio.DataOraInizio, Negozio.MINUTI_TOLLERANZA);
                    if (((NoleggioFormEditOrView)sender).DialogResult == DialogResult.OK && MessageBox.Show("Confermare il pagamento e la chiusura del noleggio?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        e.Cancel = true;
                }
                catch
                {
                    ShowError("Non esiste una fasica oraria per la durata del noleggio");
                    e.Cancel = true;
                }
            if ((sender as Form).DialogResult == DialogResult.Cancel)
            {
                foreach (ElementoNoleggio el in _noleggio.ElementiNoleggio)
                    el.AgevolazioneEccezionale = null;
            }
        }

        private NoleggioFormEditOrView Target { get { return _target; } }

        private void AggiungiAgevolazioneEccezionale(object sender, EventArgs args)
        {
            ElementoNoleggio elementoNoleggio = (ElementoNoleggio)Adapter<ElementoNoleggio, ElementoNoleggioAdapted>
                .Revert((ElementoNoleggioAdapted)_target.ElementiNoleggioDataGridView.CurrentRow.DataBoundItem);

            using (ChooseForm form = new ChooseForm())
            {
                //creazione filtro (che in realtà è solo un messaggio)
                Label messaggio = new Label();
                messaggio.Text = "Seleziona l'agevolazione eccezionale";
                messaggio.Width = 300;
                Panel p = new Panel();
                p.Controls.Add(messaggio);
                form.AddFilter(p);

                //form
                form.DataGridView.MultiSelect = false;

                //popolamento tabella
                form.DataGridView.DataSource = Negozio.GetInstance().AgevolazioniEccezionali.Cast<Agevolazione>().ToList();

                if (form.ShowDialog() == DialogResult.OK)
                {
                    elementoNoleggio.AgevolazioneEccezionale = (IAgevolazioneEccezionale)form.DataGridView.CurrentRow.DataBoundItem;
                    Console.WriteLine((elementoNoleggio.AgevolazioneEccezionale as Agevolazione).Nome);
                    _target.ElementiNoleggioDataGridView.Refresh();
                    /*La nuova agevolazione eccezionale non è visualizzata nella tabella perchè è un'interfaccia, 
                     * anche se da come si vede dalla stampa su console l'oggetto viene aggiornato*/
                    _target.PrezzoTotaleLabel.Text = string.Format("Prezzo totale: {0}",
                        _noleggio.CalcolaPrezzo(_durataEffettiva, Negozio.MINUTI_TOLLERANZA).ToEuroString());
                }
                ShowPrezzoSingoloElemento(this, EventArgs.Empty);
            }
        }

        private void ShowPrezzoSingoloElemento(object sender, EventArgs args)
        {
            if (Target.ElementiNoleggioDataGridView.CurrentRow == null)
                Target.PrezzoSingoloElementoStatus.Text = "Nessun elemento selezionato";
            else
            {
                ElementoNoleggio current = (ElementoNoleggio)Adapter<ElementoNoleggio, ElementoNoleggioAdapted>
                    .Revert((ElementoNoleggioAdapted)Target.ElementiNoleggioDataGridView.CurrentRow.DataBoundItem);

                Target.PrezzoSingoloElementoStatus.Text = string.Format("Prezzo elemento {0}: {1}", current.Corrente.Id,
                    current.CalcolaPrezzo(_fineEffettiva - _noleggio.DataOraInizio, Negozio.MINUTI_TOLLERANZA).ToEuroString());
            }
        }
    }
}
