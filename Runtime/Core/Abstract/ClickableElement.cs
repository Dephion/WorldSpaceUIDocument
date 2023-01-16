using System;
using Dephion.Ui.Core.Interfaces;

namespace Dephion.Ui.Core.Abstract
{
    public abstract class ClickableElement : SelectableElement, IClickable
    {
        public event Action<IClickable> Clicked;

        public virtual void Click()
        {
            Clicked?.Invoke(this);
        }
    }
}