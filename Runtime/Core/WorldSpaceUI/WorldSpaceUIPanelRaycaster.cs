using System.Collections.Generic;
using System.Linq;
using Dephion.Ui.Core.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Dephion.Ui.Core.WorldSpaceUI
{
    public class WorldSpaceUIPanelRaycaster : PanelRaycaster
    {
        private PanelSettings _settings;
        private List<VisualElement> _picked;

        protected override void Awake()
        {
            base.Awake();
            _picked = new List<VisualElement>();
        }

        public void SetPanelSettings(PanelSettings settings)
        {
            _settings = settings;
        }

        public void CopyFrom(PanelRaycaster panelRaycaster)
        {
            panel = panelRaycaster.panel;
        }

        public List<VisualElement> PickElementsByPosition(PointerEventData eventData, Vector2 panelResolution, out Vector2 position)
        {
            position = Vector2.zero;
            if (panel == null)
                return _picked;

            position = eventData.position;
            float h = panelResolution.y;
            position.y = h - position.y;
            _picked = panel.PickElements(position);
            return _picked;
        }

        /// <summary>
        /// Pick the top element of Type T
        /// </summary>
        /// <param name="eventData">event data object</param>
        /// <param name="position">corrector position for UIToolkit</param>
        /// <typeparam name="T">Type to filter by</typeparam>
        /// <returns>the top visual element of type T</returns>
        public T PickTopElementOfType<T>(PointerEventData eventData, Vector2 panelResolution, out Vector2 position)
        {
            var picked = PickElementsByPosition(eventData, panelResolution, out position);
            return (T)(object)picked.FirstOrDefault(p => p is T);
        }
    }
}