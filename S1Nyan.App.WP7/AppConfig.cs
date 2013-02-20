using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using S1Nyan.Utils;
using S1Nyan.Model;

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
            ServiceLocator.Current.GetInstance<IDataService>().ResourceService = new ApplicationResourceService();
#else
            ServiceLocator.Current.GetInstance<IDataService>().ResourceService = new NetResourceService();
#endif

            SimpleIoc.Default.Register<IIndicator, Indicator>();

        }
    }
}
