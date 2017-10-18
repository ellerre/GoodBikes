using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;

namespace GoodBikes.Services
{
    static partial class CommandManager
    {
        private static readonly Dictionary<string, Action> _actions;

        static CommandManager()
        {
            _actions = new Dictionary<string, Action>();
        }

        //  Permette di registrare automaticamente tutti i comandi contenuti in voci dei menù e bottoni delle toolstrip di una form.
        public static void RegisterTarget(Form target)
        {
            InitializeCommandHandlers(target.Controls);
        }

        //  Permette di associare a un comando un'azione da eseguire
        public static void RegisterCommand(string command, Action action)
        {
            #region ----- DESCRIZIONE (dal lab n. 3) -----
            /* Verificare:
           che il comando non sia una stringa nulla o vuota e che non contenga spazi bianchi
           che l'azione non sia nulla
           Se il dizionario contiene già il comando, 'aggiungere' la nuova azione al comando
           altrimenti, inserire il nuovo comando e la corrispondente azione
           Visualizzare nella finestra di output del debugger il comando e il metodo da invocare
           in particolare, il nome della classe che contiene il metodo e il nome del metodo
           ad esempio: "RegisterCommand InserisciNuovoElemento -> DocumentServices.InserisciNuovoElemento"
        */
            #endregion

            if (String.IsNullOrEmpty(command))
                throw new ArgumentException("String.IsNullOrEmpty(command)");
            if (command.Contains(' '))
                throw new ArgumentException("command.Contains(' ')");
            if (action == null)
                throw new ArgumentNullException("action");
            if (_actions.ContainsKey(command))
                _actions[command] += action;
            else
                _actions.Add(command, action);

            Console.WriteLine("Command {0} -> {1}.{2}", command, action.Method.DeclaringType.Name, action.Method.Name);
        }

        //  Permette di eseguire le azioni correntemente associate al comando
        public static void DoCommand(string command)
        {
            #region ----- DESCRIZIONE (dal lab n. 3) -----

            //  Visualizzare nella finestra di output del debugger il comando da eseguire
            //    ad esempio: "DoCommand InserisciNuovoDipendente"
            //  Se il comando esiste, eseguire il comando
            #endregion

            Console.WriteLine("DoCommand {0}", command);
            if (_actions.ContainsKey(command))
            {
                Console.WriteLine("LO FACCIO: {0}", command);
                _actions[command]();
            }
            else
                MessageBox.Show("Utente non autorizzato!", "Operazione fallita", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
