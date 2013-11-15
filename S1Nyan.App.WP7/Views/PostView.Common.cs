using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using S1Nyan.Resources;
using S1Nyan.Model;
using S1Nyan.ViewModels;
using S1Parser.PaserFactory;
using S1Parser.User;
using GalaSoft.MvvmLight.Messaging;

namespace S1Nyan.Views
{
    public partial class PostView : PhoneApplicationPage
    {
        public PostView()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            SettingView.UpdateOrientation(this);
            Loaded += OnPageLoaded;
            Unloaded += OnPageUnloaed;
        }

        private void OnPageUnloaed(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister(this);
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            SettingView.UpdateOrientation(this);
            Messenger.Default.Register<NotificationMessage>(this, OnReplySucceed);
        }

        private void OnReplySucceed(NotificationMessage obj)
        {
            ShowHideReplyPanel(true);
        }

#if DEBUG
        ~PostView()
        {
            System.Diagnostics.Debug.WriteLine("Finalizing " + this.GetType().FullName);
        }
#endif
        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public PostViewModel Vm
        {
            get
            {
                return (PostViewModel)DataContext;
            }
        }

        [DataContract]
        public class PageInfoItem
        {
            [DataMember]
            public string id;
            [DataMember]
            public string title;
            [DataMember]
            public int page;
        }

        [DataContract]
        public class PostViewPageInfo
        {
            [DataMember]
            public List<PageInfoItem> Stack;

            public PostViewPageInfo()
            {
                Stack = new List<PageInfoItem>();
            }
        }

        private const string PostViewPageInfoKey = "PostViewPageInfo";
        private const string PostViewReplyTextKey = "PostViewReplyText";

        public ImageResourceManager ImageResourceManager = new ImageResourceManager();

        private string idParam = null, titleParam = null, savedReply = null;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //ImageResourceManager.Reset();
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (idParam == null)
                {   //tombstone
                    var stack = GetInfoStack();
                    if (stack.Count > 0)
                    {
                        var item = stack[stack.Count - 1];
                        Vm.OnChangeTID(item.id, item.title, item.page);
                    }

                    if (PhoneApplicationService.Current.State.ContainsKey(PostViewReplyTextKey))
                    {
                        savedReply = PhoneApplicationService.Current.State[PostViewReplyTextKey] as string;
                        PhoneApplicationService.Current.State.Remove(PostViewReplyTextKey);
                    }
                }
                return;
            }
            string pageParam = idParam = titleParam = null;
            int page = 1;
            if (NavigationContext.QueryString.TryGetValue("ID", out idParam))
            {
                if (NavigationContext.QueryString.TryGetValue("Title", out titleParam))
                    titleParam = HttpUtility.HtmlDecode(titleParam);
                if (NavigationContext.QueryString.TryGetValue("Page", out pageParam))
                {
                    int.TryParse(pageParam, out page);
                }
                Vm.OnChangeTID(idParam, titleParam, page);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {   //tombstone
            base.OnNavigatedFrom(e);
            //ImageResourceManager.Reset();
            if (e.NavigationMode == NavigationMode.Back)
            {
                var stack = GetInfoStack();
                if (stack.Count > 0) stack.RemoveAt(stack.Count - 1);
                CleanUp();
            }
            else if (e.NavigationMode == NavigationMode.New)
            {
                var stack = GetInfoStack();
                var item = new PageInfoItem();
                item.id = idParam;
                item.page = Vm.CurrentPage;
                item.title = Vm.Title;
                stack.Add(item);

                PhoneApplicationService.Current.State[PostViewReplyTextKey] = replyText.Text;
            }
        }

        public static List<PageInfoItem> GetInfoStack()
        {
            PostViewPageInfo info = null;
            if (PhoneApplicationService.Current.State.ContainsKey(PostViewPageInfoKey))
            {
                info = PhoneApplicationService.Current.State[PostViewPageInfoKey] as PostViewPageInfo;
            }
            if (info == null)
            {
                info = new PostViewPageInfo();
                PhoneApplicationService.Current.State[PostViewPageInfoKey] = info;
            }

            return info.Stack;
        }

        private void CleanUp()
        {
            ImageResourceManager.Reset();
            navBarButton.Click -= ToggleNavigator;
            replyButton.Click -= OnReplyButton;
            refreshBarButton.Click -= OnRefresh;
            nextBarButton.Click -= OnNextPage;
            HideNavi.Completed -= OnHideNaviComplete;
            //Vm.Cleanup();
        }

        ApplicationBarIconButton navBarButton;
        ApplicationBarIconButton replyButton;
        ApplicationBarIconButton refreshBarButton;
        ApplicationBarIconButton nextBarButton;
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            refreshBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.sync.rest.png", UriKind.Relative));
            refreshBarButton.Text = AppResources.AppBarButtonRefresh;
            refreshBarButton.Click += OnRefresh;

            navBarButton = new ApplicationBarIconButton(navIcon);
            navBarButton.Text = AppResources.AppBarButtonNavigator;
            navBarButton.Click += ToggleNavigator;

            replyButton = new ApplicationBarIconButton(replyIcon);
            replyButton.Text = AppResources.AppBarButtonReply;
            replyButton.Click += OnReplyButton;

            nextBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.next.rest.png", UriKind.Relative));
            nextBarButton.Text = AppResources.AppBarButtonNextPage;
            nextBarButton.Click += OnNextPage;

            ApplicationBar.Buttons.Add(refreshBarButton);
            ApplicationBar.Buttons.Add(replyButton);
            ApplicationBar.Buttons.Add(nextBarButton);
            ApplicationBar.Buttons.Add(navBarButton);

            ApplicationBar.MenuItems.Add(OpenThreadInBrowserMenuItem());
            ApplicationBar.MenuItems.Add(SettingView.GetSettingMenuItem());

            nextBarButton.IsEnabled = false;

            //Vm.PageChanged = (current, total) =>
            //{
            //    ImageResourceManager.Reset();

            //    if (current > 1 && total > 1)
            //        FirstPage.IsEnabled = true;
            //    else
            //        FirstPage.IsEnabled = false;
            //    if (current < total && total > 1)
            //    {
            //        nextBarButton.IsEnabled = true;
            //        LastPage.IsEnabled = true;
            //    }
            //    else
            //    {
            //        nextBarButton.IsEnabled = false;
            //        LastPage.IsEnabled = false;
            //    }
            //    VertSlider.Minimum = 0;
            //    VertSlider.Value = VertSlider.Minimum;
            //    VertSlider.Maximum = VertSlider.Minimum + Vm.TheThread.Items.Count - 1;
            //};
        }

        private ApplicationBarMenuItem OpenThreadInBrowserMenuItem()
        {
            var appBarMenuItem = new ApplicationBarMenuItem(AppResources.ApplicationMenuOpenThreadInBrowser);
            appBarMenuItem.Click += (o, e) =>
                {
                    var task = new Microsoft.Phone.Tasks.WebBrowserTask
                        {
                            Uri = new Uri(Vm.TheThread.FullLink, UriKind.Absolute)
                        };
                    task.Show();
                };
            return appBarMenuItem;
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            Vm.RefreshData();
        }

        private void OnNextPage(object sender, EventArgs e)
        {
            Vm.CurrentPage++;
        }

        static Uri replyIcon = new Uri("/Assets/AppBar/appbar.reply.email.png", UriKind.Relative);
        static Uri replyIconInvert = new Uri("/Assets/AppBar/appbar.reply.email.invert.png", UriKind.Relative);
        static Uri replyFullIcon = new Uri("/Assets/AppBar/appbar.quill.png", UriKind.Relative);
        static Uri navIcon = new Uri("/Assets/AppBar/appbar.stairs.up.horizontal.png", UriKind.Relative);
        static Uri navIconInvert = new Uri("/Assets/AppBar/appbar.stairs.up.horizontal.invert.png", UriKind.Relative);
        bool IsNavigatorVisible { get { return Navigator.Visibility == Visibility.Visible; } }
        private void ToggleNavigator(object sender, EventArgs e)
        {
            if (navBarButton == null) return;
            if (IsReplyPanelVisible)
                ShowHideReplyPanel(true);
            ShowHideNavi(IsNavigatorVisible);
        }

        private void ShowHideNavi(bool hide)
        {
            if (hide)
            {
                navBarButton.IconUri = navIcon;
                HideNavi.Begin();
                HideNavi.Completed += OnHideNaviComplete;
            }
            else
            {
                navBarButton.IconUri = navIconInvert;
                Navigator.Visibility = Visibility.Visible;
                ShowNavi.Begin();
            }
        }

        private void OnHideNaviComplete(object sender, EventArgs e)
        {
            Navigator.Visibility = Visibility.Collapsed;
        }

    }
}