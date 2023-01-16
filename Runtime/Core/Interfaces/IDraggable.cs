using UnityEngine;

namespace Dephion.Ui.Core.Interfaces
{
    public interface IDraggable : ISelectable
    {
        public bool IsDragging { get; }
        public void StartDrag(Vector2 position);
        public void StopDrag(Vector2? position = null);
        public void Drag(Vector2 position);
    }
}