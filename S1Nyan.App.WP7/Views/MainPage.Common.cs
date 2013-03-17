using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ObiWang.Controls;
using S1Nyan.Model;
using S1Nyan.ViewModel;
using S1Parser;

namespace S1Nyan.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        bool isDataLoaded = false;
        System.Threading.Timer timer;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            BuildLocalizedApplicationBar();
            if (PhoneApplicationService.Current.StartupMode == StartupMode.Launch)
                Vm.DataLoaded += DataLoaded;
            else
                DataLoaded();

            SettingView.UpdateOrientation(this);
            Loaded += (o, e) => SettingView.UpdateOrientation(this);
            timer = new System.Threading.Timer(OnTimeOut, this, 5000, UInt32.MaxValue);
        }

        private static void OnTimeOut(object state)
        {
            MainPage main = state as MainPage;
            main.DataLoaded();
            main.timer.Dispose();
        }

        private void DataLoaded()
        {
            if (isDataLoaded) return;
            isDataLoaded = true;
            Dispatcher.BeginInvoke(() =>
            {
                Popup.Visibility = Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
                UserViewModel.Current.InitLogin();
            });
        }

        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public MainViewModel Vm
        {
            get
            {
                return (MainViewModel)DataContext;
            }
        }

        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var longListSelector = sender as LongListSelector;
            if (longListSelector != null)
            {
                DoNavigation(longListSelector.SelectedItem);
                longListSelector.SelectedItem = null;
            }
            else
            {
                var selector = sender as ExpandableItem;
                if (selector == null) return;
                DoNavigation(selector.SelectedItem);
                selector.SelectedItem = null;
            }
        }

        private void DoNavigation(object o)
        {
            S1ListItem item = o as S1ListItem; 
            if (item != null)
                NavigationService.Navigate(new Uri("/Views/ThreadList.xaml?ID=" + item.Id + "&Title=" + item.Title, UriKind.Relative));
        }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Buttons.Add(SettingView.GetSettingAppBarButton());
            ApplicationBar.IsVisible = false;
        }

#if DEBUG
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                GC.Collect();
            }
        }
#endif
    }
}