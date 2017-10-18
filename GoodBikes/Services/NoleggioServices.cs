using GoodBikes.Presentation;
using Model;
using Model.Elementi;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using View;
using System2;
using System;
using System.Reflection;

namespace GoodBikes.Services
{
    using Attributes;
    static partial class NoleggioServices
    {
        public static void RegisterCommands()
        {
            // Comandi della MainForm
            RegisterFor<DipendenteAttribute>();
            if (Logged.User is Model.Persone.Amministratore) RegisterFor<AdminAttribute>();
        }

        private static void RegisterFor<T>() where T : Attribute
        {
            //per ogni metodo pubblico di questo foglio (o dei parziali), lo registro se espone l'attributo di tipo T
            foreach (MethodInfo mi in typeof(NoleggioServices).GetMethods()
                .Where(m => m.GetCustomAttribute<T>()!=null))
                CommandManager.RegisterCommand(mi.Name, mi.CreateDelegate(typeof(Action)) as Action);
        }

        public static bool Modifica<T>(T item, bool firstInsert)
         where T : class
        {
            //  Se item non è null
            //    creare una EditingDialog
            //    creare un corrispondente EditingDialogPresenter
            //    invocare il metodo SetEditableObject dell'EditingDialogPresenter
            //    visualizzare la EditingDialog e, in caso di successo, restituire true

            if (item != null)
            {
                using (EditingDialog editingDialog = new EditingDialog())
                {
                    EditingDialogPresenter editingDialogPresenter = new EditingDialogPresenter(editingDialog, firstInsert);
                    editingDialogPresenter.SetEditableObject(item);
                    if (editingDialog.ShowDialog() == DialogResult.OK)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static Negozio Negozio
        {
            get { return Negozio.GetInstance(); }
        }
    }
}