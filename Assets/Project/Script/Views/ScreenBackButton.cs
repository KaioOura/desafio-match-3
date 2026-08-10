using Gazeus.DesafioMatch3.Controllers;

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
