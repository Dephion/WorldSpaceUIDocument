using System;

namespace Dephion.Ui.Core.Interfaces
{
    public interface IClickable : ISelectable
    {
        public event Action<IClickable> Clicked;
        public void Click();
    }
}