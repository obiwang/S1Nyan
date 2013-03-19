using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using S1Nyan.Resources;
using S1Nyan.Model;
using S1Nyan.Views;
using S1Parser.User;

namespace S1Nyan.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class UserViewModel : ViewModelBase, ISendPostService
    {
        public static UserViewModel Current
        {
            get
            {
                return SimpleIoc.Default.GetInstance<ISendPostService>() as UserViewModel;
            }
        }

        private Timer notifyTimer;

        /// <summary>
        /// Initializes a new instance of the UserViewModel class.
        /// </summary>
        public UserViewModel()
        {
            notifyTimer = new Timer(OnTimeUp, this, Timeout.Infinite, Timeout.Infinite);
            MessengerInstance.Register<NotificationMessage<S1NyanViewModelBase>>(this, OnReLoginMsg);
        }

        private async void OnReLoginMsg(NotificationMessage<S1NyanViewModelBase> msg)
        {
            if (msg.Notification != "ReLoginMsg") return;

            Uid = null;
            if (SettingView.IsRememberPass && SettingView.CurrentUsername.Length > 0)
                await BackgroundLogin(SettingView.CurrentUsername, SettingView.CurrentPassword);

            if (Uid != null)
                MessengerInstance.Send(new NotificationMessage<S1NyanViewModelBase>(msg.Content, "RefreshMessage"));
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
                RaisePropertyChanged(() => LoginStatus);
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
                RaisePropertyChanged(() => Uid);
            }
        }
        public async Task<string> DoLogin(string name, string pass)
        {
            if (name.Length == 0 || pass.Length == 0) return AppResources.ErrorMsgNamePassEmpty;

            string uid = null;
            try
            {
                uid = await new S1WebClient().Login(name, pass);
                if (uid != null)
                {
                    Uid = uid;
                    LoginStatus = name;
                    return null;
                }
                return AppResources.ErrorMsgUnknown;
            }
            catch (Exception ex)
            {
                return S1Nyan.Utils.Util.ErrorMsg.GetExceptionMessage(ex);
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
            notifyTimer.Change(5000, Timeout.Infinite);
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

        private RelayCommand _showAccount;

        /// <summary>
        /// Gets the ShowAccount.
        /// </summary>
        public RelayCommand ShowAccount
        {
            get
            {
                return _showAccount
                    ?? (_showAccount = new RelayCommand(
                            () =>
                            {
                                SettingView.GotoSetting(SettingView.PivotAccount);
                            }));
            }
        }

        internal async void InitLogin()
        {
            if (Uid != null) return;

            try
            {
                SettingView.InitAccountData();
                if (SettingView.IsRememberPass && SettingView.CurrentUsername.Length > 0)
                    await BackgroundLogin(SettingView.CurrentUsername, SettingView.CurrentPassword);
                else if (SettingView.IsFirstLogin)
                    SetNotifyMsg(AppResources.ErrorMsgClickToLogin);
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

                    await CheckVerify();
                    result = await new S1WebClient().Reply(SettingView.VerifyString,
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

        private async Task CheckVerify()
        {
            if (SettingView.VerifyString == null || SettingView.VerifyString.Length == 0)
            {
                SettingView.VerifyString = await new S1WebClient().GetVerifyString();
            }
        }

        internal async Task<string> GetVerifyString()
        {
            try
            {
                return await new S1WebClient().GetVerifyString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}