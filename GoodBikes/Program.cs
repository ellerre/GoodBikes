using View;
using System;
using System.Linq;
using System2;
using System.Windows.Forms;
using System.Collections.Generic;
using Model.Agevolazioni;
using GoodBikes.Presentation;

namespace GoodBikes
{
    #region references
    //PRESENTER
    //E' REFERENZIATO:      -
    //REFERENZIA:           MODEL
    //                      VIEW
    #endregion
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //VEDERE Model.StatoNegozio
            //LoadMainForAdmin();
            //LoadMainForDipendente();
            LoadLoginForm();
        }

        static void LoadMainFor(int who)
        {
            Logged.User = Model.Negozio.GetInstance().Dipendenti.ElementAt(who);
            MainForm mainForm = new MainForm();
            new MainFormPresenter(mainForm);
            Application.Run(mainForm);
        }
        static void LoadMainForAdmin()
        {
            LoadMainFor(0);
        }
        static void LoadMainForDipendente()
        {
            LoadMainFor(5);
        }
        static void LoadLoginForm()
        {
            LoginForm loginForm = new LoginForm();
            new LoginFormPresenter(loginForm);
            Application.Run(loginForm);
        }
    }
}
