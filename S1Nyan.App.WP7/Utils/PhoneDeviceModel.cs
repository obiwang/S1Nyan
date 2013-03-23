using System.Xml.Linq;
using System.Linq;
using S1Nyan.Model;
using Microsoft.Phone.Info;

namespace S1Nyan.Utils
{
    public class PhoneDeviceModel
    {
        private XElement root;

        private static PhoneDeviceModel current;
        public static PhoneDeviceModel Current
        {
            get
            {
                return current ?? (current = new PhoneDeviceModel());
            }
        }

        public PhoneDeviceModel()
        {
            Init();
        }

        private void Init()
        {
            var s = IsolatedStorageHelper.Current.ReadFromAppResource("model_name.xml");
            if (s == null) return;

            root = XDocument.Load(s).Root;
        }

        public static string GetFriendlyName()
        {
            var manufacturer = DeviceStatus.DeviceManufacturer;
            var deviceName = DeviceStatus.DeviceName;

            return string.Format("{0} {1}", manufacturer, GetFriendlyDeviceName(manufacturer, deviceName));
        }

        private static string GetFriendlyDeviceName(string manufacturer, string deviceName)
        {
            var mft = Current.root.Element(manufacturer.ToUpper());
            if (mft == null) return deviceName;

            try
            {
                var models = from model in mft.Descendants()
                             where deviceName.ToUpper().Contains(model.Attribute("DeviceName").Value)
                             select model;
                if (models.Count() > 0)
                    return models.First().Attribute("FriendlyName").Value;
            }
            catch (System.Exception) { }
            return deviceName;
        }
    }
}
