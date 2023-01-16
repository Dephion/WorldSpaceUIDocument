using Dephion.Ui.Core.WorldSpaceUI;

namespace Dephion.UI.Core.Samples
{
    public class SampleUIDocument : WorldSpaceUIDocument
    {
        protected override void Start()
        {
            base.Start();
            UiDocument.rootVisualElement.Add(new SampleRoot());
        }
    }
}