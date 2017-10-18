using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace GoodBikes.Presentation
{
    //  Modella l'insieme di tutte le proprietà 'editable' di un editingObject
    public class EditingDocument
    {
        //  Oggetto da modificare.
        private readonly object _editingObject;
        //  Elenco delle proprietà editabili
        private readonly List<EditingProperty> _editingProperties = new List<EditingProperty>();

        public EditingDocument(object editingObject)
        {
            if (editingObject == null)
                throw new ArgumentNullException("editingObject");
            _editingObject = editingObject;
            InitializeEditingProperties();
        }

        public object EditingObject
        {
            get { return _editingObject; }
        }

        public IEnumerable<EditingProperty> EditingProperties
        {
            get { return _editingProperties; }
        }

        //  Restituisce true se c'è almeno un errore di validazione.
        public bool HasError
        {
            get
            {
                return EditingProperties.Any(ep => ep.HasError);
            }
        }

        //  Reimposta i valori originali di tutte le proprietà non readonly dell'editingObject.
        public void ResetEditingObject()
        {
            foreach (EditingProperty ep in EditingProperties.Where(ep => ep.CanWrite))
                ep.ResetValue();

        }

        //  Memorizza in _editingProperties tutte le proprietà editable
        private void InitializeEditingProperties()
        {
            //	Per ogni proprietà pubblica di EditingObject alla quale è stato associato l’attributo Editable:
            //  se la proprietà è write-only, sollevare un’eccezione;
            //  creare una nuova EditingProperty sulla proprietà e aggiungerla alla collezione _editingProperties;
            //  visualizzare nella finestra di output del debugger la descrizione completa dell'EditingProperty
            //  ad esempio: "Add EditingProperty Titolo  {Label = Titolo, Width = 100} OriginalValue = TitoloLibro_1"

            Type t = EditingObject.GetType();
            foreach (PropertyInfo p in t.GetProperties())
            {
                if((p.GetCustomAttributes(typeof(Model.EditableAttribute), false).Length > 0)) 
                {
                    if (p.CanWrite && !p.CanRead)
                        throw new ArgumentException("public editable property can not be write only");
                    EditingProperty ep = new EditingProperty(p, (Model.EditableAttribute) p.GetCustomAttributes(typeof(Model.EditableAttribute), false)[0], EditingObject);
                    _editingProperties.Add(ep);
                    Console.WriteLine(ep.ToString());
                }
            }
          
        }
    }
}
