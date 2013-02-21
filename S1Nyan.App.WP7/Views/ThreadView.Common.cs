using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ImageTools;
using ImageTools.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using S1Nyan.App.Resources;
using S1Nyan.ViewModel;
using S1Parser;

namespace S1Nyan.App.Views
{
    public partial class ThreadPage : PhoneApplicationPage
    {
        public ThreadPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public ThreadViewModel Vm
        {
            get
            {
                return (ThreadViewModel)DataContext;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back) return;

            string idParam, titleParam = null;
            if (NavigationContext.QueryString.TryGetValue("ID", out idParam))
            {
                NavigationContext.QueryString.TryGetValue("Title", out titleParam);
                Vm.OnChangeTID(idParam, HttpUtility.HtmlDecode(titleParam));
            }
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.sync.rest.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarButtonRefresh;
            appBarButton.Click += (o, e) => Vm.RefreshThread();
            ApplicationBar.Buttons.Add(appBarButton);

            ApplicationBarIconButton preBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.back.rest.png", UriKind.Relative));
            preBarButton.Text = AppResources.AppBarButtonPrePage;
            preBarButton.Click += (o, e) => Vm.CurrentPage--;
            ApplicationBar.Buttons.Add(preBarButton);

            ApplicationBarIconButton nextBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.next.rest.png", UriKind.Relative));
            nextBarButton.Text = AppResources.AppBarButtonNextPage;
            nextBarButton.Click += (o, e) => Vm.CurrentPage++;
            ApplicationBar.Buttons.Add(nextBarButton);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            ApplicationBar.MenuItems.Add(appBarMenuItem);

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