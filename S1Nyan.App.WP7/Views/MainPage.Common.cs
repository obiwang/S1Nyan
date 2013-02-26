using System;
using System.Windows.Controls;
using ImageTools.IO.Gif;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyControls;
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
            Loaded += (o, e) => this.SupportedOrientations = SettingView.IsAutoRotateSetting ? SupportedPageOrientation.PortraitOrLandscape : SupportedPageOrientation.Portrait; 
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
        }
    }
}