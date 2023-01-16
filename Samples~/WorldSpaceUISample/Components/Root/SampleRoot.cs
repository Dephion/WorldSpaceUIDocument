using System;
using Dephion.Ui.Core.Abstract;
using Dephion.Ui.Core.Interfaces;
using UnityEngine.UIElements;

namespace Dephion.UI.Core.Samples
{
    public class SampleRoot : AbstractElement, IDisposable
    {
        protected override string VisualTreeAssetPath => "sample-root";

        private Button _button;
        private Label _label;
        private int _clicks;

        public SampleRoot() : base()
        {
            AddToClassList(VisualTreeAssetPath);
            _button = this.Q<Button>("button");
            _button.Clicked += ButtonOnClicked;

            _label = this.Q<Label>("clicked-label");
        }

        private void ButtonOnClicked(IClickable obj)
        {
            _clicks++;
            _label.text = $"You clicked {_clicks} times!";
        }

        public void Dispose()
        {
            if (_button != null)
                _button.Clicked -= ButtonOnClicked;
        }
    }
}