using Dephion.Ui.Core.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dephion.Ui.Core.Abstract
{
    public abstract class AbstractElement : VisualElement
    {
        protected abstract string VisualTreeAssetPath { get; }

        protected AbstractElement()
        {
            CreateVisualTree();
        }

        private void CreateVisualTree()
        {
            if (string.IsNullOrWhiteSpace(VisualTreeAssetPath)) return;

            if (!TryGetVisualTreeAsset(out var template))
            {
                Debug.LogWarning($"Could not load template at [{VisualTreeAssetPath}]");
                return;
            }

            template.CloneTree(this);
        }

        private bool TryGetVisualTreeAsset(out VisualTreeAsset visualTreeAsset)
        {
            visualTreeAsset = Resources.Load<VisualTreeAsset>(VisualTreeAssetPath);
            return visualTreeAsset;
        }
    }

    /// <summary>
    /// base class for selectable elements (by touch/mouse)
    /// </summary>
    public abstract class SelectableElement : AbstractElement, ISelectable
    {
        public bool IsSelected { get; private set; }

        public virtual void Select()
        {
            IsSelected = true;
            AddToClassList("selected");
        }

        public virtual void Deselect()
        {
            IsSelected = false;
            AddToClassList("selected");
        }
    }
}