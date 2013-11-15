#if DEBUG
//#define UseFakeData
#endif
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Phone.Net.NetworkInformation;
using S1Nyan.Model;
using S1Nyan.Utils;
using S1Nyan.Views;
using S1Parser;

namespace S1Nyan
{
    public class AppConfig
    {
        static AppConfig()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
        }

        public static void Setup()
        {
            SimpleIoc.Default.Register<IIndicator, Indicator>();
            SimpleIoc.Default.Register<IErrorMsg, ErrorMsg>();

#if UseFakeData
            SimpleIoc.Default.Register<IResourceService, ApplicationResourceService>();
            IParserFactory factory = new S1Parser.PaserFactory.FakeSimpleParserFactory();
#else
            SimpleIoc.Default.Register<IResourceService, NetResourceService>();
            IParserFactory factory = new S1Parser.PaserFactory.DZParserFactory(null);
#endif
            S1Resource.ParserFactory = factory;

            factory.ResourceService = SimpleIoc.Default.GetInstance<IResourceService>();
            SimpleIoc.Default.GetInstance<IDataService>().ParserFactory = factory;
            SimpleIoc.Default.GetInstance<IDataService>().StorageHelper = SimpleIoc.Default.GetInstance<IStorageHelper>();

            SettingView.InitTheme();

            ImageTools.IO.Decoders.AddDecoder<ImageTools.IO.Gif.GifDecoder>();

            DeviceNetworkInformation.NetworkAvailabilityChanged += DeviceNetworkInformation_NetworkAvailabilityChanged;
        }

        static void DeviceNetworkInformation_NetworkAvailabilityChanged(object sender, NetworkNotificationEventArgs e)
        {
            if (e.NotificationType == NetworkNotificationType.InterfaceConnected)
            {
                S1Nyan.ViewModels.UserViewModel.Current.ReLogin();
            }
        }

    }
}
