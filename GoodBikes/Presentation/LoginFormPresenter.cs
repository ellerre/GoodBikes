using System;
using System.Windows.Forms;

using View;
using Model;
using Model.Persone;

namespace GoodBikes.Presentation
{
    class LoginFormPresenter
    {
        private readonly LoginForm _target;

        public LoginFormPresenter(LoginForm target)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            _target = target;
            _target.Esci.Click += Esci_Click;
            _target.Login.Click += Login_Click;
        }

        #region Proprietà
        public LoginForm Target
        {
            get { return _target; }
        }

        private static Negozio Negozio
        {
            get { return Negozio.GetInstance(); }
        }
        #endregion

        #region Gestori metodi
        private void MainFormLogin()
        {
            //using () //METODO PULITO PER CHIUDERE LOGINFORM FINALIZZANDOLO E CEDENDO IL CONTROLLO A MAINFORM?
            MainForm mainForm = new MainForm();
            new MainFormPresenter(mainForm); //NoleggioServices.RegisterCommands(); gestito ora dal presenter
            mainForm.FormClosed += Esci_Click;//mainForm_FormClosed;
            mainForm.Show();
            Target.Hide();
        }

        private void Login_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Target.UserName.Text) && !string.IsNullOrEmpty(Target.Password.Text))
            {
                IDipendente logging = Negozio.DipendenteDaCredenziali(Target.UserName.Text, Target.Password.Text);
                if (logging != null)
                {
                    Logged.User = logging;
                    MainFormLogin();
                }
                else
                    MessageBox.Show("Utente sconosciuto!", "Operazione fallita", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
                MessageBox.Show("Compilare i campi!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Esci_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion
    }
}
