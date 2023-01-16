using Dephion.Ui.Core.Abstract;
using UnityEngine.UIElements;

namespace Dephion.UI.Core.Samples
{
    public class Button : ClickableElement
    {
        public new class UxmlFactory : UxmlFactory<Button, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _textAttribute = new() { name = "text", defaultValue = string.Empty };
            private readonly UxmlEnumAttributeDescription<Justify> _alignAttribute = new() { name = "align", defaultValue = Justify.Center };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var button = (Button)ve;
                button.Text = _textAttribute.GetValueFromBag(bag, cc);
                button.Align = _alignAttribute.GetValueFromBag(bag, cc);
            }
        }


        private const string COMPONENT_NAME = "button";
        private Label _label;

        protected override string VisualTreeAssetPath => COMPONENT_NAME;

        public Button()
        {
            AddToClassList(COMPONENT_NAME);
            _label = this.Q<Label>("button__label");
        }

        public string Text
        {
            get => _label.text;
            set => _label.text = value;
        }

        public Justify Align
        {
            get => this.Q("content-container").style.justifyContent.value;
            set => this.Q("content-container").style.justifyContent = value;
        }
    }
}