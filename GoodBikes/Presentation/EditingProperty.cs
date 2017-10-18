using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Model;
using Model.Elementi;

namespace GoodBikes.Presentation
{
    //  Modella una singola proprietà 'editable' di un editingObject
    public class EditingProperty
    {
        //  Descrittore della proprietà
        private readonly PropertyInfo _propertyInfo;
        //  Attributo associato alla proprietà
        private readonly EditableAttribute _editableAttribute;
        //  Oggetto da editare
        private readonly object _editingObject;
        //  Valore originale (iniziale) della proprietà nell'editingObject
        private readonly object _originalValue;

        public EditingProperty(PropertyInfo propertyInfo, EditableAttribute editableAttribute, object editingObject)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            if (editableAttribute == null)
                throw new ArgumentNullException("editableAttribute");
            if (editingObject == null)
                throw new ArgumentNullException("editingObject");
            _propertyInfo = propertyInfo;
            _editableAttribute = editableAttribute;
            _editingObject = editingObject;
            _originalValue = GetValue();
        }

        public PropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
        }

        public bool CanWrite
        {
            get { return PropertyInfo.CanWrite; }
        }

        public string Label
        {
            get { return _editableAttribute.Label; }
        }

        public int Width
        {
            get { return _editableAttribute.Width; }
        }

        public bool NotEditableAfterFirstTime
        {
            get { return _editableAttribute.NotEditableAfterFirstTime; }
        }

        public string Mode
        {
            get { return _editableAttribute.Mode; }
        }

        private object EditingObject
        {
            get { return _editingObject; }
        }

        private object OriginalValue
        {
            get { return _originalValue; }
        }

        public bool HasError
        {
            get { return LastException != null; }
        }

        public string Message
        {
            get
            {
                if (LastException != null)
                {
                    Exception exception = LastException;
                    while (exception.InnerException != null)
                        exception = exception.InnerException;
                    return exception.Message;
                }
                else
                {
                    return null;
                }
            }
        }

        //  Contiene l'eventuale ultima eccezione sollevata, oppure vale null.
        private Exception LastException { get; set; }

        //  Restituisce il valore corrente della proprietà nell'editingObject
        public object GetValue()
        {
            return _propertyInfo.GetValue(_editingObject, null);
        }

        //  Cerca di assegnare un nuovo valore alla proprietà dell'editingObject
        public void TrySetValue(object value)
        {
            //  Verificare che la proprietà non sia readonly.
            // 	Cercare di assegnare il nuovo valore alla proprietà dell'editingObject e,
            //  in caso di valore non valido, intercettare l'eccezione sollevata.
            //  Modificare in modo opportuno LastException.

            if (PropertyInfo.CanWrite)
            {
                try
                {
                    PropertyInfo.SetValue(EditingObject, value, null);
                    LastException = null;
                }
                catch (Exception e) { LastException = e; }
            }
            else throw new ArgumentException("Can't write");
        }

        //  Restituisce il valore corrente della proprietà nell'editingObject come stringa di caratteri.
        //  Se il valore corrente è null, restituisce una stringa vuota.
        public string ConvertToString()
        {
            object value = GetValue();
            return value == null ? string.Empty : value.ToString();
        }

        //  Cerca di convertire il valore passato come stringa (textValue) nel tipo della proprietà (ad es. un int o un double).
        //  In uscita, value contiene il valore convertito (se la conversione è avvenuta correttamente), oppure null.
        //  Restituisce true se la conversione è avvenuta correttamente.
        public bool TryConvertFromString(string textValue, out object value)
        {
            //  Cercare di convertire textValue nel tipo della proprietà.
            //  Per effettuare tutte le possibili conversioni in modo semplice, utilizzare il metodo Convert.ChangeType.
            //  In caso di errore di conversione, intercettare e memorizzare l'eccezione sollevata (utilizzare la stessa struttura di TrySetValue).
            value = null;
            try
            {
                value = Convert.ChangeType(textValue, PropertyInfo.PropertyType);
                return true;
            }
            catch (Exception e)
            {
                LastException = e;
                return false;
            }
        }

        public bool TryConvertFromComboItem(object item, out object value)
        {
            value = null;
            try
            {
                value = Convert.ChangeType(item, item.GetType());
                return true;
            }
            catch (Exception e)
            {
                LastException = e;
                return false;
            }
        }

        //  Reimposta il valore originale della proprietà
        public void ResetValue()
        {
            try
            {
                if (OriginalValue != null)
                    PropertyInfo.SetValue(_editingObject, OriginalValue, null);
            }
            catch (Exception e)
            {
                LastException = e;
                return;
            }
        }

        //  Restituisce il nome della proprietà, il contenuto dell'EditableAttribute e il valore originale.
        //  Ad esempio: "Titolo  {Label = Titolo, Width = 100} OriginalValue = TitoloLibro_1".
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.PropertyInfo.Name + " {");
            sb.Append("Label = " + this._editableAttribute.Label + ", ");
            sb.Append("Width = " + this._editableAttribute.Width + "} ");
            sb.Append("OriginalValue = " + this._originalValue);
            return sb.ToString();
        }
    }
}
