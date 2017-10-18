using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using View;
using Model.Elementi;
using System.Reflection;
using System2.Reflection;
using Model;

namespace GoodBikes.Presentation
{
    //  Permette di modificare i valori delle proprietà di un qualsiasi tipo di oggetto.
    //  Le proprietà prese in considerazione sono esclusivamente quelle marcate con l'attributo Editable.
    public class EditingControlPresenter
    {
        private readonly EditingControl _target;
        private EditingDocument _editingDocument;
        private bool _firstEdit;

        public EditingControlPresenter(EditingControl target, bool firstEdit)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            _target = target;
            _firstEdit = firstEdit;
        }

        public EditingControl Target
        {
            get { return _target; }
        }

        private EditingDocument EditingDocument
        {
            get { return _editingDocument; }
            set { _editingDocument = value; }
        }

        // Restituisce true se ci sono errori di validazione.
        public bool HasError
        {
            get
            {
                return EditingDocument.HasError;
            }
        }

        //  Associa all'EditingControlPresenter il nuovo oggetto da modificare.
        //  Può essere invocato più volte
        public void SetEditableObject(object editingObject)
        {
            EditingDocument = new EditingDocument(editingObject);
            InitializeTarget();
        }

        //  Reimposta i valori originali di tutte le proprietà non readonly dell'editingObject.
        public void ResetEditingObject()
        {
            EditingDocument.ResetEditingObject();
            RefreshControls(null);
        }

        //  Inizializza l’EditingControl aggiungendo una coppia di controlli (Label, TextBox) per ogni proprietà 'editable' di EditingDocument
        //  Viene invocato ogni volta che cambia l'editingObject (e quindi l'EditingDocument).
        private void InitializeTarget()
        {
            //	Eliminare da Target.TableLayoutPanel eventuali controlli inseriti in precedenza.
            //	Per ogni proprietà Editable:
            //  invocare il metodo AddRow passando come argomento l'EditingProperty.
            //	Invocare il metodo RefreshTextBoxes in modo che vengano aggiornati i valori di tutte le TextBox.
            Target.SuspendLayout();
            Target.TableLayoutPanel.Controls.Clear();
            foreach (EditingProperty p in EditingDocument.EditingProperties)
                AddRow(p);
            RefreshControls(null);
            Target.ResumeLayout(false);
        }

        //  Crea, inizializza e aggiunge al Target.TableLayoutPanel nell’ordine: una Label e una TextBox.
        private void AddRow(EditingProperty editingProperty)
        {
            //  Per inizializzare la Label:
            // 	  assegnare alla proprietà Text il valore della proprietà Label di editableAttribute; 
            // 	  assegnare alla proprietà AutoSize il valore true, in modo che il testo venga visualizzato correttamente.
            //  Per inizializzare la TextBox:
            // 	  dimensionare il controllo in modo che
            //      la larghezza sia pari al valore della proprietà Width di editableAttribute e
            //      l’altezza sia pari al valore della proprietà PreferredHeight del controllo stesso;
            // 	  se la editingProperty è read-only,
            //      disabilitare la TextBox (utilizzare la proprietà Enabled);
            // 	  assegnare alla proprietà Tag il valore di editingProperty;
            // 	  infine, collegare all’evento Validating della TextBox il gestore ValidatingHandler.

            Label label = new Label();
            label.Text = editingProperty.Label;
            label.AutoSize = true;
            Target.TableLayoutPanel.Controls.Add(label);

            Control tb = null;
            if (editingProperty.Mode == "Combo")
            {
                tb = new ComboBox();
                if (editingProperty.Label.StartsWith("Tipo"))
                {
                    Type categoria = typeof(Elemento).GetSubtypes().Where(t => t.Name.ToUpper() == editingProperty.Label.Substring(4).ToUpper()).First();
                    ((ComboBox)tb).Items.AddRange(Negozio.GetInstance().GetTipiPerCategoria(categoria).ToArray());
                    ((ComboBox)tb).SelectedItem = ((ComboBox)tb).Items.OfType<TipoElemento>().First();
                }
                else if (editingProperty.Label.StartsWith("Stato"))
                {
                    ((ComboBox)tb).Items.AddRange(Negozio.GetInstance().StatiElemento.ToArray());
                    ((ComboBox)tb).SelectedItem = ((ComboBox)tb).Items.OfType<StatoElemento>().First();
                }
            }
            else
            {
                tb = new TextBox();
                tb.Height = ((TextBox)tb).PreferredHeight;
            }

            tb.Width = editingProperty.Width;
            tb.Enabled = checkIfEnabled(editingProperty);
            tb.Tag = editingProperty;
            tb.Validating += ValidatingHandler;
            Target.TableLayoutPanel.Controls.Add(tb);
        }

        private bool checkIfEnabled(EditingProperty editingProperty)
        {
            return editingProperty.CanWrite && (!editingProperty.NotEditableAfterFirstTime || (editingProperty.NotEditableAfterFirstTime && _firstEdit));
        }

        
        //  Inserisce nelle TextBox i valori delle corrispondenti proprietà dell'editingObject.
        private void RefreshControls(Control excludedBox)
        {
            //  Per ogni textBox contenuta in Target.TableLayoutPanel, ad esclusione di excludedTextBox:
            //	  recuperare la editingProperty precedentemente salvata nella proprietà Tag di textBox;
            //    assegnare alla proprietà Text di textBox il valore corrente dell'editingProperty come stringa di caratteri;
            //	  infine, se la editingProperty è writable, invocare il metodo Validate passando come argomento textBox.

            foreach (Control tb in Target.TableLayoutPanel.Controls)
            {
                if ((tb.GetType().Equals(typeof(TextBox)) || tb.GetType().Equals(typeof(ComboBox))) && !Equals(excludedBox, tb))
                {
                    EditingProperty ep = (EditingProperty)tb.Tag;
                    tb.Text = ep.ConvertToString();
                    tb.Select(); //trick per avere red al load senza usare il metodo Validate ed ottenere ciò che volevamo
                    //ho tolto il blink da View.EditingControl perchè uccide
                }
            }
            Target.TableLayoutPanel.Select(); //celo il trucco
        }

        //  Viene invocato automaticamente quando una TextBox perde il focus
        //  Coordina la validazione del dato inserito nella TextBox(il sender).
        private void ValidatingHandler(object sender, CancelEventArgs args)
        {
            //  Invocare il metodo Validate passando come argomento il sender.
            //  Se non ci sono errori di validazione, invocare il metodo RefreshTextBoxes passando come argomento il sender.
            //  Si noti che l’invocazione finale del metodo RefreshTextBoxes permette di visualizzare correttamente
            //  i valori di eventuali proprietà calcolabili (cioè che si basano sui valori di altre proprietà).

            Validate((Control)sender);
            if (!HasError)
                RefreshControls((Control)sender);
        }

        //  Esegue la validazione vera e propria del dato contenuto nella textBox passata come argomento.
        private void Validate(Control box)
        {
            // 	Recuperare la editingProperty dalla textBox.
            // 	Invocare in modo opportuno i metodi TryConvertFromString e TrySetValue dell'editingProperty.
            //  Infine, aggiornare l'ErrorProvider per segnalare all'utente che la textBox è con o senza errori.

            object value;
            EditingProperty ep = (EditingProperty)box.Tag;
            if (box.GetType() == typeof(TextBox) && ep.TryConvertFromString(box.Text, out value))
                ep.TrySetValue(value);
            else if (box.GetType() == typeof(ComboBox) && ep.TryConvertFromComboItem(((ComboBox)box).SelectedItem, out value))
                ep.TrySetValue(value);

            Target.ErrorProvider.SetError(box, ep.Message);
        }
    }
}