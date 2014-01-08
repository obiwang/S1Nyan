using Microsoft.Phone.Controls;
using System.Net;
using System.Windows;
using System.Windows.Navigation;

namespace S1Nyan.Views
{
    public partial class PostView : PhoneApplicationPage, IFocusable, IViewLoaded
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

        private IPostViewModel _postViewModel;
        public void ViewLoaded(object vm)
        {
            _postViewModel = vm as IPostViewModel;
            if (_postViewModel != null && _id != null)
                _postViewModel.InitContent(_id, _page, _title);
            _id = null;
        }

        private string _id, _title;
        private int _page;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string pageParam = _id = _title = null;
            _page = 1;
            if (NavigationContext.QueryString.TryGetValue("ID", out _id))
            {
                if (NavigationContext.QueryString.TryGetValue("Title", out _title))
                    _title = HttpUtility.HtmlDecode(_title);
                if (NavigationContext.QueryString.TryGetValue("Page", out pageParam))
                {
                    int.TryParse(pageParam, out _page);
                }
            }
        }

        public ImageResourceManager ImageResourceManager = new ImageResourceManager();

        private void CleanUp()
        {
            ImageResourceManager.Reset();
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