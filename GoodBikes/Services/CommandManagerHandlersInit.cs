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
        private static void InitializeCommandHandlers(Control.ControlCollection controls)
        {
            #region ----- DESCRIZIONE (dal lab n. 3) -----
            //  Per ogni control contenuto in controls
            //    se control è un ToolStrip
            //      invocare InitializeCommandHandlers passando come argomento la collezione degli item contenuti nel ToolStrip
            //    altrimenti
            //      invocare InitializeCommandHandlers passando come argomento la collezione dei controlli contenuti in control
            #endregion

            foreach (Control control in controls)
            {
                if (control is ToolStrip)
                    InitializeCommandHandlers((control as ToolStrip).Items);
                else if (control is Button)
                    InitializeCommandHandlers((control as Button));
                else
                    InitializeCommandHandlers(control.Controls);
            }
        }

        private static void InitializeCommandHandlers(ToolStripItemCollection items)
        {
            #region ----- DESCRIZIONE (dal lab n. 3) -----
            //  Per ogni ToolStripItem in items
            //    se il Tag è una stringa (è un comando)
            //      collegare all'evento Click dell'item il gestore Item_Click
            //      visualizzare nella finestra di output del debugger il comando da eseguire
            //        ad esempio: "InitializeCommandHandler for InserisciNuovoLibro"
            //    se è un ToolStripMenuItem
            //      andare in ricorsione passando le sotto voci del menu (proprietà DropDownItems)
            #endregion

            foreach (ToolStripItem item in items)
            {
                if (!string.IsNullOrEmpty(item.Tag as string))
                {
                    item.Click += Item_Click_ToolStripItem<ToolStripMenuItem>;
                    Console.WriteLine("ToolStripMenuItem {0} -> Command {1}", item.Name, item.Tag);
                }
                if (item is ToolStripMenuItem)
                    InitializeCommandHandlers((item as ToolStripMenuItem).DropDownItems);
            }
        }

        private static void InitializeCommandHandlers(Button button)
        {
            if (!string.IsNullOrEmpty(button.Tag as string))
            {
                button.Click += Item_Click<Button>;
                Console.WriteLine("Button {0} -> Command {1}", button.Name, button.Tag);
            }
        }
    }
}
