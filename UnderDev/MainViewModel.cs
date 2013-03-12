using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using S1Parser.Emotion;

namespace UnderDev
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MvvmViewModel1 class.
        /// </summary>
        public MainViewModel()
        {
            Task.Factory.StartNew(() => EmotionParser.Init(Application.GetResourceStream(new Uri("Resources/Emotion.htm", UriKind.Relative)).Stream));
        }

        private bool tester = false;

        private RelayCommand _b1;
        /// <summary>
        /// Gets the B1.
        /// </summary>
        public RelayCommand B1
        {
            get
            {
                return _b1
                    ?? (_b1 = new RelayCommand(
                                          () => NewMethod(), () => tester));
            }
        }

        private bool NewMethod()
        {
            tester = !tester;
            if (tester)
            {
                ThemeManager.SetCustomTheme(new System.Uri("/UnderDev;component/CustomThemeResources.xaml", System.UriKind.Relative), Theme.Light);
                ThemeManager.ToLightTheme();
            }
            else
            {
                ThemeManager.SetCustomTheme(new System.Uri("/UnderDev;component/LightThemeResources.xaml", System.UriKind.Relative), Theme.Light);
                ThemeManager.ToLightTheme();
            }

            B1.RaiseCanExecuteChanged();
            B2.RaiseCanExecuteChanged();
            return tester;
        }
        private RelayCommand _b2;
        /// <summary>
        /// Gets the B1.
        /// </summary>
        public RelayCommand B2
        {
            get
            {
                return _b2
                    ?? (_b2 = new RelayCommand(
                                          () => NewMethod(), () => !tester));
            }
        }

    }
}