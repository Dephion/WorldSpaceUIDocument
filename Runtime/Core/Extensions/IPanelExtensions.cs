using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dephion.Ui.Core.Extensions
{
    public static class IPanelExtensions
    {
        /// <summary>
        /// Pick all VisualElements in a given panel
        /// </summary>
        /// <param name="panel">the target panel</param>
        /// <param name="position">position</param>
        /// <returns>list of all visual elements that are contain the point</returns>
        public static List<VisualElement> PickElements(this IPanel panel, Vector2 position)
        {
            var picked = new List<VisualElement>();
            PerformPick(panel.visualTree, position, picked);
            return picked;
        }

        /// <summary>
        /// Pick the top element of Type T
        /// </summary>
        /// <param name="panel">the panel</param>
        /// <param name="position">position</param>
        /// <typeparam name="T">Type to filter by</typeparam>
        /// <returns>the top visual element of type T</returns>
        public static T PickTopElementOfType<T>(this IPanel panel, Vector2 position)
        {
            var picked = PickElements(panel, position);
            return (T)(object)picked.FirstOrDefault(p => p is T);
        }

        /// <summary>
        /// recursive picking function
        /// </summary>
        /// <param name="root">root visual element</param>
        /// <param name="point">position</param>
        /// <param name="picked">list of picked elements</param>
        /// <returns>new root element</returns>
        private static VisualElement PerformPick(
            VisualElement root,
            Vector2 point,
            List<VisualElement> picked = null)
        {
            if (root.resolvedStyle.display == DisplayStyle.None || root.pickingMode == PickingMode.Ignore && root.hierarchy.childCount == 0)
                return (VisualElement)null;
            Vector2 local = root.WorldToLocal(point);
            bool flag = root.ContainsPoint(local);

            VisualElement visualElement1 = (VisualElement)null;
            for (int key = root.hierarchy.childCount - 1; key >= 0; --key)
            {
                VisualElement visualElement2 = PerformPick(root.hierarchy[key], point, picked);
                if (visualElement1 == null && visualElement2 != null)
                {
                    if (picked == null)
                        return visualElement2;
                    visualElement1 = visualElement2;
                }
            }

            if (((!root.visible ? 0 : (root.pickingMode == PickingMode.Position ? 1 : 0)) & (flag ? 1 : 0)) != 0)
            {
                picked?.Add(root);
                if (visualElement1 == null)
                    visualElement1 = root;
            }

            return visualElement1;
        }
    }
}