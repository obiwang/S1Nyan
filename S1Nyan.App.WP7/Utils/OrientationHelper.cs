using Microsoft.Phone.Controls;

namespace S1Nyan.Utils
{
    public class OrientationHelper : IOrientationHelper
    {
        public void UpdateOrientation(PhoneApplicationPage page)
        {
            if (page == null) return;

            //BUG: Auto Rotate >> Landscape >> Disable Auto Rotate >> Still LandScape
            page.SupportedOrientations = Views.SettingView.IsAutoRotate ? SupportedPageOrientation.PortraitOrLandscape : SupportedPageOrientation.Portrait;
        }
    }
}