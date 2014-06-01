using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Phone.Net.NetworkInformation;
using S1Nyan.Resources;
using S1Nyan.Model;
using S1Nyan.ViewModels.Message;
using S1Nyan.Views;
using S1Parser;
using S1Parser.User;

namespace S1Nyan.ViewModels
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class UserViewModel : Screen, IUserService, IHandle<UserMessage>
    {
        public static UserViewModel Current
        {
            get
            {
                return IoC.Get<IUserService>() as UserViewModel;
            }
        }

        private readonly IEventAggregator _eventAggregator;

        private readonly Timer _notifyTimer;

        private string _formHash;

        public void UpdateFormHash(string formHash)
        {
            _formHash = formHash;
        }

        /// <summary>
        /// Initializes a new instance of the UserViewModel class.
        /// </summary>
        public UserViewModel(IEventAggregator eventAggregator)
        {
            _notifyTimer = new Timer(OnTimeUp, this, Timeout.Infinite, Timeout.Infinite);
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            DeviceNetworkInformation.NetworkAvailabilityChanged += DeviceNetworkInformation_NetworkAvailabilityChanged;

            S1Resource.FormHashUpdater = this;
        }

        void DeviceNetworkInformation_NetworkAvailabilityChanged(object sender, NetworkNotificationEventArgs e)
        {
            if (e.NotificationType == NetworkNotificationType.InterfaceConnected)
            {
                ReLogin();
            }
        }

        public async void Handle(UserMessage msg)
        {
            if (msg.Type != Messages.ReLogin) return;

            Uid = null;
            if (SettingView.IsRememberPass && SettingView.CurrentUsername.Length > 0)
                await BackgroundLogin(SettingView.CurrentUsername, SettingView.CurrentPassword);

            if (Uid != null)
                _eventAggregator.Publish(new UserMessage(Messages.Refresh, msg.Content));
        }

        private string _loginStatus = AppResources.AccountPageGuest;

        /// <summary>
        /// Sets and gets the LoginStatus property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string LoginStatus
        {
            get { return _loginStatus; }

            set
            {
                if (_loginStatus == value) return;

                _loginStatus = value;
                NotifyOfPropertyChange(() => LoginStatus);
            }
        }

        private string _uid = null;

        /// <summary>
        /// Sets and gets the Uid property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Uid
        {
            get { return _uid; }

            set
            {
                if (_uid == value) return;

                _uid = value;
                NotifyOfPropertyChange(() => Uid);
            }
        }

        public bool IsBusy;

        public async Task<string> DoLogin(string name, string pass)
        {
            if (name.Length == 0 || pass.Length == 0) return AppResources.ErrorMsgNamePassEmpty;

            string uid = null;
            bool isSuccess = false;
            try
            {
                IsBusy = true;
                if (!String.IsNullOrEmpty(Uid) && !string.IsNullOrEmpty(_formHash))
                    await new S1WebClient().Logout(_formHash);
                S1WebClient.ResetCookie();
                var user = await new S1WebClient().Login(name, pass);
                uid = user.Member_uid;
                if (uid != null)
                {
                    Uid = uid;
                    LoginStatus = name;
                    isSuccess = true;
                    return null;
                }
                return AppResources.ErrorMsgUnknown;
            }
            catch (Exception ex)
            {
                var mainpage = IoC.Get<MainPageViewModel>();
                var userEx = (ex as S1UserException);
                if ((ex is System.Net.WebException && DeviceNetworkInformation.IsNetworkAvailable)
                    || (userEx != null && userEx.ErrorType == UserErrorTypes.InvalidData))
                {
                    ServerViewModel.Current.CheckServerStatus(mainpage);
                }
                return S1Nyan.Utils.Util.ErrorMsg.GetExceptionMessage(ex);
            }
            finally
            {
                IsBusy = false;
                _eventAggregator.Publish(new UserMessage(Messages.LoginStatusChanged, isSuccess));
            }
        }

        public async Task BackgroundLogin(string name, string pass)
        {
            var error = await DoLogin(name, pass);
            if (error == null)
            {
                previousText = null;
            }
            else
            {
                SetNotifyMsg(error);
            }
        }

        private void SetNotifyMsg(string msg)
        {
            if (previousText == null)
                previousText = LoginStatus;
            LoginStatus = msg;
            _notifyTimer.Change(5000, Timeout.Infinite);
        }

        string previousText = null;

        private static void OnTimeUp(object state)
        {
            UserViewModel user = state as UserViewModel;
            GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher.BeginInvoke(() =>
            {
                if (user.previousText == null) return;
                else
                {
                    user.LoginStatus = user.previousText;
                    user.previousText = null;
                }
            });
        }

        public void ShowAccount()
        {
            SettingView.GotoSetting(SettingView.PivotAccount);
        }

        public async void InitLogin()
        {
            if (Uid != null) return;

            var mainpage = IoC.Get<MainPageViewModel>();
            try
            {
                SettingView.InitAccountData();
                if (SettingView.IsRememberPass && SettingView.CurrentUsername.Length > 0)
                    await BackgroundLogin(SettingView.CurrentUsername, SettingView.CurrentPassword);
                else if (SettingView.IsFirstLogin)
                {
                    ServerViewModel.Current.CheckServerStatus(mainpage);
                    SetNotifyMsg(AppResources.ErrorMsgClickToLogin);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        internal void ReLogin()
        {
            Uid = null;
            InitLogin();
        }

        public async Task DoAddToFavorite(string tid)
        {
            await new S1WebClient().AddToFavorite(_formHash, tid);
        }

        public async Task<string> DoSendPost(string replyLink, string replyText)
        {
            UserErrorTypes result = UserErrorTypes.Unknown;
            int retryTimes = 0;
            try
            {
                while (result != UserErrorTypes.Success)
                {
                    if (retryTimes > 2)
                        throw new S1UserException("MaxRetry", UserErrorTypes.MaxRetryTime);
                    if (result == UserErrorTypes.InvalidVerify)
                    {
                        Uid = null;
                        var error = await DoLogin(SettingView.CurrentUsername, SettingView.CurrentPassword);
                        if (error != null)
                            throw new S1UserException(error, UserErrorTypes.LoginFailed);
                    }

                    result = await new S1WebClient().Reply(_formHash,
                        reletivePostUrl: replyLink,
                        content: replyText,
                        signature: S1Nyan.Views.SettingView.GetSignature());

                    retryTimes++;
                }
                return null;
            }
            catch (Exception ex)
            {
                return S1Nyan.Utils.Util.ErrorMsg.GetExceptionMessage(ex);
            }
        }
    }
}