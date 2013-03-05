using System;
using System.Windows;
using System.Windows.Controls;
using ImageTools.IO.Gif;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ObiWang.Controls;
using S1Nyan.ViewModel;
using S1Parser;

namespace S1Nyan.App.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ImageTools.IO.Decoders.AddDecoder<GifDecoder>();

            BuildLocalizedApplicationBar();
            if (PhoneApplicationService.Current.StartupMode == StartupMode.Launch)
                Vm.DataLoaded += DataLoaded;
            else
                DataLoaded();

            Loaded += (o, e) => this.SupportedOrientations = SettingView.IsAutoRotateSetting ? SupportedPageOrientation.PortraitOrLandscape : SupportedPageOrientation.Portrait; 
        }

        private void DataLoaded()
        {
            Dispatcher.BeginInvoke(() =>
            {
                SystemTray.IsVisible = true;
                Popup.Visibility = Visibility.Collapsed;
            });
            ApplicationBar.IsVisible = true;
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