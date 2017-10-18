using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Persone;

namespace GoodBikes
{
    static class Logged
    {
        private static IDipendente _user;
        public static IDipendente User
        {
            get
            {
                if (_user == null) throw new ApplicationException("LoggedUser è nullo?!");
                return _user;
            }
            set
            {
                if (_user != null) throw new ApplicationException("LoggedUser già settato.");
                if (value == null) throw new ApplicationException("LoggedUser non può essere nullo.");
                _user = value;
            }
        }
    }
}
