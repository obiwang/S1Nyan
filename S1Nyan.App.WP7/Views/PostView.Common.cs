using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Navigation;

namespace S1Nyan.Views
{
    public partial class PostView : PhoneApplicationPage, IFocusable
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
        #region Tomb stone support
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

                PhoneApplicationService.Current.State[PostViewReplyTextKey] = ReplyText.Text;
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

        #endregion

        public ImageResourceManager ImageResourceManager = new ImageResourceManager();

        private void CleanUp()
        {
            ImageResourceManager.Reset();
        }

        private void OnReplyButton(object sender, EventArgs e)
        {
            //ToggleReplyPanelVisible();
            if (savedReply != null)
            {
                ReplyText.Text = savedReply;
                savedReply = null;
            }
        }

        #region SIP margin walkaround

        void replyText_LostFocus(object sender, RoutedEventArgs e)
        {
            ApplicationBar.IsVisible = true;
        }

        void replyText_GotFocus(object sender, RoutedEventArgs e)
        {
            Focus();
        }

        public void Focus()
        {
            ApplicationBar.IsVisible = false;
            ReplyText.Focus();
        }
        #endregion
    }
}