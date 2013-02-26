using System;
using System.Net;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using S1Nyan.App.Resources;
using S1Nyan.ViewModel;

namespace S1Nyan.App.Views
{
    public partial class ThreadPage : PhoneApplicationPage
    {
        public ThreadPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            Loaded += (o, e) => this.SupportedOrientations = SettingView.IsAutoRotateSetting ? SupportedPageOrientation.PortraitOrLandscape : SupportedPageOrientation.Portrait;
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

            string idParam, titleParam = null, pageParam = null;
            int page = 1;
            if (NavigationContext.QueryString.TryGetValue("ID", out idParam))
            {
                if (NavigationContext.QueryString.TryGetValue("Title", out titleParam))
                    HttpUtility.HtmlDecode(titleParam);
                if (NavigationContext.QueryString.TryGetValue("Page", out pageParam))
                {
                    int.TryParse(pageParam, out page);
                }
                Vm.OnChangeTID(idParam, titleParam, page);
            }
        }

        ApplicationBarIconButton navBarButton;
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.sync.rest.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarButtonRefresh;
            appBarButton.Click += (o, e) => Vm.RefreshThread();
            ApplicationBar.Buttons.Add(appBarButton);

            navBarButton = new ApplicationBarIconButton(navIcon);
            navBarButton.Text = AppResources.AppBarButtonNavigator;
            navBarButton.Click += ToggleNavigator;
            ApplicationBar.Buttons.Add(navBarButton);

            ApplicationBarIconButton nextBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.next.rest.png", UriKind.Relative));
            nextBarButton.Text = AppResources.AppBarButtonNextPage;
            nextBarButton.Click += (o, e) => Vm.CurrentPage++;
            ApplicationBar.Buttons.Add(nextBarButton);

            ApplicationBar.MenuItems.Add(SettingView.GetSettingMenuItem());

            nextBarButton.IsEnabled = false;

            Vm.PageChanged = (current, total) =>
            {
                if (current > 1 && total > 1) 
                    FirstPage.IsEnabled = true;
                else 
                    FirstPage.IsEnabled = false;
                if (current < total && total > 1)
                {
                    nextBarButton.IsEnabled = true;
                    LastPage.IsEnabled = true;
                }
                else
                {
                    nextBarButton.IsEnabled = false;
                    LastPage.IsEnabled = false;
                }
            };
        }

        Uri navIcon = new Uri("/Assets/AppBar/appbar.stairs.up.horizontal.png", UriKind.Relative);
        Uri navIconRevert = new Uri("/Assets/AppBar/appbar.stairs.up.revert.horizontal.png", UriKind.Relative);
        bool IsNavigatorVisible { get { return Navigator.Visibility == Visibility.Visible; } }
        private void ToggleNavigator(object sender, EventArgs e)
        {
            if (navBarButton == null) return;
            if (IsNavigatorVisible)
            {
                navBarButton.IconUri = navIcon;
                HideNavi.Begin();
                HideNavi.Completed += (o, ee) => Navigator.Visibility = Visibility.Collapsed;
            }
            else
            {
                navBarButton.IconUri = navIconRevert;
                Navigator.Visibility = Visibility.Visible;
                ShowNavi.Begin();
            }
        }

    }
}