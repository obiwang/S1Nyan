using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using S1Nyan.Utils;
using S1Nyan.Model;
using S1Parser;

namespace S1Nyan.App
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
#if DEBUG
            SimpleIoc.Default.Register<IResourceService, ApplicationResourceService>();
#else
            SimpleIoc.Default.Register<IResourceService, NetResourceService>();
#endif
            IParserFactory factory = new S1Parser.PaserFactory.SimpleParserFactory();
            factory.ResourceService = ServiceLocator.Current.GetInstance<IResourceService>();
            ServiceLocator.Current.GetInstance<IDataService>().ParserFactory = factory;
            SimpleIoc.Default.Register<IIndicator, Indicator>();

            SettingView.InitTheme();
        }

    }
}
