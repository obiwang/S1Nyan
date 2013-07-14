using System.IO;
using System.Xml.Linq;
using System.Linq;
using S1Nyan.Model;
using Microsoft.Phone.Info;

namespace S1Nyan.Utils
{
    public class PhoneDeviceModel : RemoteConfigModelBase
    {
        private static string _friendlyName;

        public static string FriendlyName
        {
            get
            {
                if (_friendlyName == null)
                {
                    new PhoneDeviceModel().UpdateDeviceFriendlyName();
                }
                return _friendlyName;
            }
        }

        private static string GetFriendlyDeviceName(Stream s, string manufacturer, string deviceName)
        {
            var root = XDocument.Load(s).Root;

            try
            {
                var mft = root.Element(manufacturer.ToUpper());
                if (mft == null) return deviceName; 
                
                var models = from model in mft.Descendants()
                             where deviceName.ToUpper().Contains(model.Attribute("DeviceName").Value)
                             select model;
                if (models.Any())
                    return models.First().Attribute("FriendlyName").Value;
            }
            catch (System.Exception) { }
            return deviceName;
        }

        private const string ModelListFileName = "model_name.xml";
        private const string RemoteModelListPath = RemoteResourcePath + ModelListFileName;

        public void UpdateDeviceFriendlyName()
        {
            GetLocalList();
            UpdateListFromRemote();
        }

        #region RemoteConfigModelBase member

        protected override string ConfigFileName
        {
            get { return ModelListFileName; }
        }

        protected override double CacheDays
        {
            get { return 10; }
        }

        protected override string RemoteFilePath
        {
            get { return RemoteModelListPath; }
        }

        protected override void RefreshList()
        {
            
        }

        protected override void UpdateList(Stream s)
        {
            UpdateFriendlyName(s);
        }

        private static void UpdateFriendlyName(Stream s)
        {
            var manufacturer = DeviceStatus.DeviceManufacturer;
            var deviceName = DeviceStatus.DeviceName;

            _friendlyName = string.Format("{0} {1}", manufacturer, GetFriendlyDeviceName(s, manufacturer, deviceName));
        }

        #endregion
    }
}
