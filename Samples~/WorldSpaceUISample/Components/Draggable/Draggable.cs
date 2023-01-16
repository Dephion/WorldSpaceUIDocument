using Dephion.Ui.Core.Extensions;
using Dephion.Ui.Core.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dephion.UI.Core.Samples
{
    public class Draggable : Button, IDraggable
    {
        private const string COMPONENT_NAME = "draggable";

        public new class UxmlFactory : UxmlFactory<Draggable, UxmlTraits>
        {
        }

        public bool IsDragging { get; private set; }
        private Vector2 _dragOffset;

        public void StartDrag(Vector2 position)
        {
            _dragOffset = parent.WorldToLocal(new Vector2(worldBound.x, worldBound.y)) - parent.WorldToLocal(position);
            IsDragging = true;
        }

        public virtual void StopDrag(Vector2? position = null)
        {
            IsDragging = false;
            if (position.HasValue)
            {
                var local = parent.WorldToLocal(position.Value + _dragOffset);
                this.SetLayout(new Rect(local, this.layout.size));
            }

            _dragOffset = Vector2.zero;
        }

        public virtual void Drag(Vector2 position)
        {
            var local = parent.WorldToLocal(position + _dragOffset);
            this.SetLayout(new Rect(local, this.layout.size));
        }
    }
}