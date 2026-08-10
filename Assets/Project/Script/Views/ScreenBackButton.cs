using Gazeus.DesafioMatch3.Core;

namespace Gazeus.DesafioMatch3.Views
{
    public class ScreenBackButton : ButtonView
    {
        protected override void OnClick()
        {
            ScreenManager.Instance.Back();
        }
    }
}
