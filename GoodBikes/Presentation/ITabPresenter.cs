using System;
using Model;

namespace GoodBikes.Presentation
{
    interface ITabPresenter
    {
        void RefreshView(object sender, ChangedEventArgs e);
    }
}
