using Caliburn.Micro;
using GalaSoft.MvvmLight.Threading;
using S1Nyan.Model;
using S1Nyan.Utils;
using S1Parser;
using System;
using System.Threading.Tasks;

namespace S1Nyan.ViewModels
{

    public class PostViewModel : S1NyanViewModelBase, IPostViewModel
    {
        readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the PostViewModel class.
        /// </summary>
        public PostViewModel(
            IDataService dataService, 
            IUserService userService,
            INavigationService navigationService,
            IEventAggregator eventAggregator)
            : base(dataService, eventAggregator, navigationService)
        {
            _userService = userService;
        }

        private string _title = null;

        /// <summary>
        /// Sets and gets the Title property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value) return;

                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        private string _tid = null;
        public void InitContent(string tid, int page = 1, string title = "")
        {
            Title = title;
            if(tid!=null && tid != _tid)
            {
                _tid = tid;
                TotalPage = 0;
                CurrentPage = page;
            }
        }

        public string Tid
        {
            get { return _tid; }
            set
            {
                if(value ==null || value ==_tid) return;
                _tid = value;
                TotalPage = 0;
                CurrentPage = 1;
                NotifyOfPropertyChange(() => CanOpenInBrowser);
            }
        }

        private S1Post _thePost = null;
        /// <summary>
        /// Sets and gets the TheThread property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public S1Post ThePost
        {
            get { return _thePost; }
            set
            {
                if (_thePost == value) return;

                _thePost = value;
                NotifyOfPropertyChange(() => ThePost);
            }
        }

        public override async Task RefreshData()
        {
            ThePost = null;
            if (null == _tid) return;
            Util.Indicator.SetLoading();
            try
            {
                var temp = await _dataService.GetThreadDataAsync(_tid, CurrentPage);
                if (temp.CurrentPage == CurrentPage)
                {
                    ThePost = temp;
                    TotalPage = ThePost.TotalPage;
                    Title = ThePost.Title;
                    NotifyOfPropertyChange(() => CanFirstPage);
                    NotifyOfPropertyChange(() => CanPrePage);
                    NotifyOfPropertyChange(() => CanNextPage);
                    NotifyOfPropertyChange(() => CanLastPage);
                }
                Util.Indicator.SetBusy(false);
            }
            catch (Exception e)
            {
                if (!HandleUserException(e))
                {
                    Util.Indicator.SetError(e);
                    NotifyMessage = e.Message;
                }
            }
        }

        public bool IsShowPage { get { return TotalPage > 1; } }

        int _totalPage;
        public int TotalPage
        {
            get { return _totalPage; }
            set
            {
                if (_totalPage == value) return;
                _totalPage = value;
                NotifyOfPropertyChange(() => TotalPage);
                NotifyOfPropertyChange(() => IsShowPage);
            }
        }

        //may change during async actions, disable optimization
        volatile int _currentPage;
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (value > 0 && 
                    value != _currentPage &&
                    ((_totalPage > 0 && value <= _totalPage) || _totalPage == 0))
                {
                    _currentPage = value;
                    RefreshData();
                    NotifyOfPropertyChange(() => CurrentPage);
                }
            }
        }

        #region Bind ApplicaionBar

        private bool _isShowNavigator;
        public bool IsShowNavigator
        {
            get { return _isShowNavigator; }
            set
            {
                if (value && IsShowReplyPanel)
                {
                    IsShowReplyPanel = false;
                } 
                _isShowNavigator = value;
                NotifyOfPropertyChange(() => IsShowNavigator);
                NotifyOfPropertyChange(() => NavigatorIcon);
            }
        }

        public Uri NavigatorIcon
        {
            get
            {
                string iconPath = IsShowNavigator
                    ? "/Assets/AppBar/appbar.stairs.up.horizontal.invert.png"
                    : "/Assets/AppBar/appbar.stairs.up.horizontal.png";

                return new Uri(iconPath, UriKind.Relative);
            }
        }

        public void ToggleNavigator()
        {
            IsShowNavigator = !IsShowNavigator;
        }

        private bool _isShowReplyPanel;
        public bool IsShowReplyPanel
        {
            get { return _isShowReplyPanel; }
            set
            {
                if (value && IsShowNavigator)
                {
                    IsShowNavigator = false;
                } 
                _isShowReplyPanel = value;
                if (value && HostedView is IFocusable)
                {
                    (HostedView as IFocusable).Focus();
                }
                NotifyOfPropertyChange(() => IsShowReplyPanel);
                NotifyOfPropertyChange(() => ReplyIcon);
            }
        }

        public Uri ReplyIcon
        {
            get
            {
                string iconPath = IsShowReplyPanel
                    ? "/Assets/AppBar/appbar.reply.email.invert.png"
                    : "/Assets/AppBar/appbar.reply.email.png";

                return new Uri(iconPath, UriKind.Relative);
            }
        }

        public void ToggleReply()
        {
            IsShowReplyPanel = !IsShowReplyPanel;
        }

        public void RefreshPost()
        {
            RefreshData();
        }

        public void FirstPage()
        {
            CurrentPage = 1;
        }

        public bool CanFirstPage
        {
            get { return CanPrePage; }
        }

        public void PrePage()
        {
            CurrentPage--;
        }

        public bool CanPrePage
        {
            get { return CurrentPage > 1 && TotalPage > 1; }
        }

        public void NextPage()
        {
            CurrentPage++;
        }

        public bool CanNextPage
        {
            get { return CurrentPage < TotalPage && TotalPage > 1; }
        }

        public void LastPage()
        {
            CurrentPage = TotalPage;
        }

        public bool CanLastPage
        {
            get { return CanNextPage; }
        }

        public void GoToSetting()
        {
            _navigationService.Navigate(new Uri("/Views/SettingView.xaml", UriKind.Relative));
        }

        public void OpenInBrowser()
        {
            var task = new Microsoft.Phone.Tasks.WebBrowserTask
            {
                Uri = new Uri(S1Resource.GetThreadOriginalUrl(_tid), UriKind.Absolute)
            };
            task.Show();
        }

        public bool CanOpenInBrowser
        {
            get { return !string.IsNullOrEmpty(_tid); }
        }

        public async void AddToFavorite()
        {
            Util.Indicator.SetLoading();
            try
            {
                await _userService.DoAddToFavorite(_tid);
                Util.Indicator.SetBusy(false);
            }
            catch (Exception e)
            {
                if (!HandleUserException(e))
                {
                    Util.Indicator.SetError(e);
                    NotifyMessage = e.Message;
                }
            }

        }

        #endregion

        //public override void Cleanup()
        //{
        //    base.Cleanup();
        //    TheThread = null;
        //    PageChanged = null;
        //}

        #region Reply
        private string _replyText = "";

        /// <summary>
        /// Sets and gets the ReplyText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ReplyText
        {
            get { return _replyText; }

            set
            {
                if (_replyText == value) return;

                _replyText = value;
                NotifyOfPropertyChange(() => ReplyText);
                NotifyOfPropertyChange(() => CanSendReply);
            }
        }

        private bool isSending = false;
        private bool IsSending
        {
            get { return isSending; }
            set
            {
                isSending = value;
                NotifyOfPropertyChange(() => CanSendReply);
            }
        }

        private string _replyResult = "";

        /// <summary>
        /// Sets and gets the Result property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ReplyResult
        {
            get { return _replyResult; }

            set
            {
                if (_replyResult == value) return;

                if (_hasReplySucceed && value == null)
                {
                    _hasReplySucceed = false;
                    DispatcherHelper.RunAsync(() => IsShowReplyPanel = false);
                }
                _replyResult = value;
                NotifyOfPropertyChange(() => ReplyResult);
            }
        }

        //private RelayCommand _sendCommand;
        private bool _hasReplySucceed;

        public bool CanSendReply
        {
            get { return ReplyText.Length > 0 && !IsSending; }
        }

        public async void SendReply()
        {
            var replyLink = ThePost.ReplyLink;

            System.Diagnostics.Debug.WriteLine("Send Reply: " + replyLink + "\r\n" + ReplyText);

            IsSending = true;
            var result = await _userService.DoSendPost(replyLink, ReplyText.Replace("\r","\r\n"));
            IsSending = false;
            if (result == null)
            {
                ReplyText = "";
                _hasReplySucceed = true;
                ReplyResult = Utils.Util.ErrorMsg.GetExceptionMessage(S1Parser.User.S1UserException.ReplySuccess);
            }
            else
            {
                ReplyResult = result;
            }
        }

        #endregion
    }
}