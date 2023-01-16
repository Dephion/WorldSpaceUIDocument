using UnityEngine;
using UnityEngine.UIElements;

namespace Dephion.Ui.Core.Extensions
{
    public static class VisualElementExtensions
    {
        public static bool TryQ(this VisualElement element, string name, out VisualElement result)
        {
            result = element.Q(name);
            return result != null;
        }

        public static void SetLayout(this VisualElement visualElement, Rect layout, Position position = Position.Absolute)
        {
            var style = visualElement.style;
            style.position = position;
            style.left = layout.x;
            style.top = layout.y;
            style.right = float.NaN;
            style.bottom = float.NaN;
            style.width = layout.width;
            style.height = layout.height;
        }

        public static void BorderRadius(this VisualElement visualElement, float radius)
        {
            visualElement.style.borderTopLeftRadius = new StyleLength(radius);
            visualElement.style.borderTopRightRadius = new StyleLength(radius);
            visualElement.style.borderBottomLeftRadius = new StyleLength(radius);
            visualElement.style.borderBottomRightRadius = new StyleLength(radius);
        }

        public static void Margin(this VisualElement visualElement, float margin = 0)
        {
            visualElement.style.marginLeft = new StyleLength(margin);
            visualElement.style.marginBottom = new StyleLength(margin);
            visualElement.style.marginRight = new StyleLength(margin);
            visualElement.style.marginTop = new StyleLength(margin);
        }
    }
}