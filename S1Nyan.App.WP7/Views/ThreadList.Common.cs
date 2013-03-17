using System;
using System.Net;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using S1Nyan.App.Resources;
using S1Nyan.ViewModel;
using S1Parser;

namespace S1Nyan.Views
{
    public class ThreadPopularityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var t = System.Convert.ToDouble(value);

            return t > 150 ? 1 : (0.25 + t / 200.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Description for ThreadList.
    /// </summary>
    public partial class ThreadList : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the ThreadList class.
        /// </summary>
        public ThreadList()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            SettingView.UpdateOrientation(this);
            Loaded += (o, e) => SettingView.UpdateOrientation(this);
        }

        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public ThreadListViewModel Vm
        {
            get
            {
                return (ThreadListViewModel)DataContext;
            }
        }

        string idParam = null;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ThreadView.GetInfoStack().Clear();
            if (e.NavigationMode == NavigationMode.Back)
            {
#if DEBUG
                GC.Collect();
#endif
                if (idParam != null)
                    return;
            }
            string titleParam = idParam = null;
            if (NavigationContext.QueryString.TryGetValue("ID", out idParam))
            {
                NavigationContext.QueryString.TryGetValue("Title", out titleParam);
                Vm.OnChangeFID(idParam, HttpUtility.HtmlDecode(titleParam));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                Vm.Cleanup();
            }
        }

        private void ThreadListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = sender as LongListSelector;
            var item = selector.SelectedItem as S1ListItem;
            if (item != null)
            {
                var uri = "/Views/ThreadView.xaml?ID=" + item.Id + "&Title=" + item.Title;
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
            selector.SelectedItem = null;
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.sync.rest.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarButtonRefresh;
            appBarButton.Click += (o, e) => Vm.RefreshData(); 
            ApplicationBar.Buttons.Add(appBarButton);

            ApplicationBarIconButton preBarButton  = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.back.rest.png", UriKind.Relative));
            preBarButton.Text = AppResources.AppBarButtonPrePage;
            preBarButton.Click += (o, e) =>  Vm.CurrentPage--; 
            ApplicationBar.Buttons.Add(preBarButton);

            ApplicationBarIconButton nextBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.next.rest.png", UriKind.Relative));
            nextBarButton.Text = AppResources.AppBarButtonNextPage;
            nextBarButton.Click += (o, e) =>  Vm.CurrentPage++; 
            ApplicationBar.Buttons.Add(nextBarButton);

            ApplicationBar.MenuItems.Add(SettingView.GetSettingMenuItem());

            preBarButton.IsEnabled = false;
            nextBarButton.IsEnabled = false;

            Vm.PageChanged = (current, total) =>
            {
                if (current > 1 && total > 1) preBarButton.IsEnabled = true;
                else preBarButton.IsEnabled = false;
                if (current < total && total > 1) nextBarButton.IsEnabled = true;
                else nextBarButton.IsEnabled = false;
            };
        }

    }
}