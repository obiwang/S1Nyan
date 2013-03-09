using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;

namespace S1Nyan.App
{
    public partial class SettingView : PhoneApplicationPage
    {
        const string signatureFormatString = "\r\n    [url=http://126.am/S1Nyan]—— from S1 Nyan ({0} {1})[/url]";

        internal static string VerifyString { get; set; }

        public static string GetSignature()
        {
            return string.Format(signatureFormatString, DeviceStatus.DeviceManufacturer, DeviceStatus.DeviceName);
        }
    }
}