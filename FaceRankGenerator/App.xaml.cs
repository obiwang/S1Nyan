using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace FaceRankGenerator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
