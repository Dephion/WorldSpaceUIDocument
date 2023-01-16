namespace Dephion.Ui.Core.Interfaces
{
    public interface ISelectable
    {
        public bool IsSelected { get; }
        public void Select();
        public void Deselect();
    }
}