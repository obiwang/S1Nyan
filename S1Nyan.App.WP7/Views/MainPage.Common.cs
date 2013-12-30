using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using S1Nyan.Resources;
using S1Nyan.ViewModels;
using System;
using System.Windows;

namespace S1Nyan.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        bool isDataLoaded = false;

        // Constructor
        public MainPage()
        {
            SettingView.InitTheme();

            InitializeComponent();

            BuildLocalizedApplicationBar();

            Loaded += PageLoaded; 
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            DataLoaded();
        }

        private void DataLoaded()
        {
            if (isDataLoaded) return;
            isDataLoaded = true;
            Popup.Visibility = Visibility.Collapsed;
            ApplicationBar.IsVisible = true;
            UserViewModel.Current.InitLogin();
        }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Buttons.Add(S1NyanThreadAppBarButton);
            ApplicationBar.Buttons.Add(SettingView.GetSettingAppBarButton());
            ApplicationBar.Buttons.Add(SettingView.GetAboutAppBarButton());
            ApplicationBar.IsVisible = false;
        }

        private ApplicationBarIconButton _s1NyanThreadAppBarButton;

        ApplicationBarIconButton S1NyanThreadAppBarButton
        {
            get
            {
                if (_s1NyanThreadAppBarButton == null)
                {
                    _s1NyanThreadAppBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.column.one.png", UriKind.Relative));
                    _s1NyanThreadAppBarButton.Text = AppResources.S1NyanThread;
                    _s1NyanThreadAppBarButton.Click += (o, e) => NavigationService.Navigate(new Uri("/Views/PostView.xaml?ID=899264", UriKind.Relative));
                }

                return _s1NyanThreadAppBarButton;
            }
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
#if DEBUG
                GC.Collect();
                GC.WaitForPendingFinalizers();
#endif
            }
        }
    }
}