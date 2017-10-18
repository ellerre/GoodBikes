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
        private static void Item_Click<T>(object sender, EventArgs e)
            where T : Control
        {
            #region ----- DESCRIZIONE (dal lab n. 3) -----
            //  Lanciare un'eccezione se il Tag del sender non è una stringa
            //  Eseguire il comando memorizzato nel Tag del sender
            #endregion

            T item = (T)sender;
            if (item.Tag as string == null)
                throw new ApplicationException("item.Tag NON è una stringa.");
            DoCommand((string)item.Tag);
        }
        private static void Item_Click_ToolStripItem<T>(object sender, EventArgs e)
            where T : ToolStripItem
        {
            #region ----- DESCRIZIONE (dal lab n. 3) -----
            //  Lanciare un'eccezione se il Tag del sender non è una stringa
            //  Eseguire il comando memorizzato nel Tag del sender
            #endregion

            T item = (T)sender;
            if (item.Tag as string == null)
                throw new ApplicationException("item.Tag NON è una stringa.");
            DoCommand((string)item.Tag);
        }
    }
}
