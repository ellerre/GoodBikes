using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using Model.Persone;
using System.Reflection;

namespace GoodBikes.Presentation
{
    using Services;
    using Services.Attributes;
    public partial class MainFormPresenter
    {
        public class MainFormMenuPresenter
        {
            private readonly MenuStrip _target;

            public MainFormMenuPresenter(MenuStrip target)
            {
                if (target == null)
                    throw new ArgumentNullException("target");
                _target = target;
                Generate();
            }

            private void Generate()
            {
                GenerateFor<DipendenteAttribute>();
                if (Logged.User is Amministratore) GenerateFor<AdminAttribute>();
            }

            private void GenerateFor<T>() where T : Attribute
            {
                IEnumerable<MethodInfo> en = typeof(NoleggioServices).GetMethods().Where(m => m.GetCustomAttribute<T>()!=null);
                //per Dipendente od Admin
                en = en.Intersect(typeof(NoleggioServices).GetMethods().Where(m => m.GetCustomAttribute<MenuItemAttribute>()!=null));
                //i metodi con attribute MenuItem
                foreach (MethodInfo mi in en)
                    AddItem(mi.GetCustomAttribute<MenuItemAttribute>(),
                        //Ciao -> _ciao
                        string.Format("_{0}{1}", mi.Name[0].ToString().ToLower(), mi.Name.Substring(1)), mi.Name);
            }

            public ToolStripMenuItem AddItem(MenuItemAttribute attribute, string nome, string tag)
            {
                IList<string> tmp = new List<string>(attribute.NomeLivelli);
                tmp.Add(nome);
                return AddItem(attribute.Testo, tag, tmp.ToArray());
            }
            public ToolStripMenuItem AddItem(string testo, string tag, params string[] path)
            {
                SearchAndCreate(path.ToList(), _target.Items);
                ToolStripMenuItem ret = _target.Items.Find(path[path.Length - 1], true)[0] as ToolStripMenuItem;
                ret.Text = testo;
                ret.Tag = tag;
                return ret;
            }

            private void SearchAndCreate(IList<string> path, ToolStripItemCollection items)
            {
                string first = path.ElementAt(0);
                ToolStripMenuItem item;
                ToolStripItem[] subItems;
                if ((subItems = items.Find(first, false)).Length == 0)
                {
                    item = new ToolStripMenuItem();
                    item.Name = item.Text = first; //non bellissimo, ma semplificativo
                    items.Add(item);
                }
                else
                {
                    item = subItems[0] as ToolStripMenuItem;
                    if (item == null)
                        Console.WriteLine("Problema di conversione menuItem");
                }
                path.RemoveAt(0);
                if (path.Count > 0)
                    SearchAndCreate(path, item.DropDownItems);
            }
        }
    }
}
