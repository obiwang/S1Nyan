using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Windows.Navigation;

namespace S1Nyan.Views
{
    public partial class PostView : PhoneApplicationPage
    {
        public PostView()
        {
            InitializeComponent();
        }

#if DEBUG
        ~PostView()
        {
            System.Diagnostics.Debug.WriteLine("Finalizing " + this.GetType().FullName);
        }
#endif

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
                        //Vm.OnChangeTID(item.id, item.title, item.page);
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
                //Vm.OnChangeTID(idParam, titleParam, page);
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
                //item.id = idParam;
                //item.page = Vm.CurrentPage;
                //item.title = Vm.Title;
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
        }


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

        //private void ToggleNavigator(object sender, EventArgs e)
        //{
        //    if (navBarButton == null) return;
        //    if (IsReplyPanelVisible)
        //        ShowHideReplyPanel(true);
        //    ShowHideNavi(IsNavigatorVisible);
        //}

        //private void ShowHideNavi(bool hide)
        //{
        //    if (hide)
        //    {
        //        navBarButton.IconUri = navIcon;
        //        HideNavi.Begin();
        //        HideNavi.Completed += OnHideNaviComplete;
        //    }
        //    else
        //    {
        //        navBarButton.IconUri = navIconInvert;
        //        Navigator.Visibility = Visibility.Visible;
        //        ShowNavi.Begin();
        //    }
        //}

        //private void OnHideNaviComplete(object sender, EventArgs e)
        //{
        //    Navigator.Visibility = Visibility.Collapsed;
        //}

    }
}