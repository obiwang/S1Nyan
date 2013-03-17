using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using S1Nyan.App.Resources;
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
    public class UserViewModel : ViewModelBase
    {
        public static UserViewModel Current
        {
            get
            {
                return SimpleIoc.Default.GetInstance<UserViewModel>();
            }
        }

        private readonly IDataService _dataService;
        private Timer notifyTimer;

        /// <summary>
        /// Initializes a new instance of the UserViewModel class.
        /// </summary>
        public UserViewModel(IDataService dataService)
        {
            _dataService = dataService;
            notifyTimer = new Timer(OnTimeUp, this, Timeout.Infinite, Timeout.Infinite);
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

                SettingView.VerifyString = await new S1WebClient().GetVerifyString();
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
    }
}