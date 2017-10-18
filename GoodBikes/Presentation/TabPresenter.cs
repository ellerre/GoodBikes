using Model;
using Model.Elementi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System2.Collections.Generic;
using GoodBikes.Adapters;
using View;

namespace GoodBikes.Presentation
{
    abstract class ATabPresenter<T> : ITabPresenter
    {
        protected AutoDataGridView _grid;
        protected IEnumerable<T> _elementi;
        protected Func<T, bool> _filter; //-ricorda decorator-

        public AutoDataGridView DataGridView { get { return _grid; } }

        protected ATabPresenter(AutoDataGridView grid, IEnumerable<T> auto, Func<T, bool> filter)
        {
            this._grid = grid;
            this._elementi = auto;
            this._filter = filter;
        }

        protected IEnumerable<T> GetFiltered()
        {
            return _filter != null ? _elementi.Where(_filter).ToList() : _elementi;
        }

        public abstract void RefreshView(object sender, ChangedEventArgs e);
    }

    sealed class TabPresenter<T> : ATabPresenter<T>
    {
        public TabPresenter(AutoDataGridView grid, IEnumerable<T> auto, Func<T, bool> filter)
            : base(grid, auto, filter)
        {
            RefreshView(this, null);
        }

        public override void RefreshView(object sender, ChangedEventArgs e)
        {
            _grid.DataSource = GetFiltered();
        }
    }

    sealed class TabPresenter<T1, T2> : ATabPresenter<T1> where T2 : AAdapted<T1>, new()
    {
        public TabPresenter(AutoDataGridView grid, IEnumerable<T1> auto, Func<T1, bool> filter)
            : base(grid, auto, filter)
        {
            RefreshView(this, null);
        }

        public override void RefreshView(object sender, ChangedEventArgs e)
        {
            _grid.DataSource = Adapter<T1, T2>.Convert(GetFiltered());
        }
    }
}
