using System;

namespace GoodBikes.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MenuItemAttribute : Attribute
    {
        private readonly string[] _nomeLivelli;
        private readonly string _testo;

        public MenuItemAttribute(string testo, params string[] nomeLivelli)
        {
            if (string.IsNullOrEmpty(testo))
                throw new ArgumentNullException("testo");
            if (nomeLivelli == null)
                throw new ArgumentNullException("nomeLivelli");
            _testo = testo;
            _nomeLivelli = nomeLivelli;
        }

        public string[] NomeLivelli { get { return _nomeLivelli; } }
        public string Testo { get { return _testo; } }
    }
}
