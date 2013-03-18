using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using S1Nyan.Resources;
using S1Nyan.ViewModel;

namespace S1Nyan.Views
{
    public partial class SettingView : PhoneApplicationPage
    {
        bool isAccountInited = false;
        private void InitAccount()
        {
            if (UserViewModel.Current.Uid != null)
                stackPanel.SetValue(Canvas.TopProperty, -400.0);

            if (isAccountInited) return;
            isAccountInited = true;
            InitLogIn();
        }

        public static void InitAccountData()
        {
            CurrentUsername = SavedUserName;
            CurrentPassword = IsRememberPass ? SavedPassword : "";
        }

        #region LogIn

        public static string CurrentUsername { get; private set; } /// INIT NEEDED
        public static string CurrentPassword { get; private set; } /// INIT NEEDED

        private void InitLogIn()
        {
            RememberPass.IsChecked = IsRememberPass;
            RememberPass.Click += (o, e) => IsRememberPass = RememberPass.IsChecked ?? false;

            LoginButton.Click += OnLogin;
            ChangeAccount.Click += OnChangeAccount;

            UsernameText.Text = CurrentUsername;
            UsernameText.ActionIconTapped += (o, e) =>
            {
                UsernameText.Text = "";
                UsernameText.Focus();
            };

            PasswordText.Password = CurrentPassword == "" ? "" : FakePassword;
            CheckPasswordWatermark();

            RegisterLink.NavigateUri = new System.Uri(S1Parser.S1Resource.SiteBase + "register.php");
        }

        private void OnChangeAccount(object sender, RoutedEventArgs e)
        {
            LogoutTransition.Begin();
        }

        private async void OnLogin(object sender, RoutedEventArgs e)
        {
            UpdateControls(false);
            var pass = PasswordText.Password;
            if (pass == FakePassword)
                pass = CurrentPassword;
            var msg = await UserViewModel.Current.DoLogin(UsernameText.Text, pass);
            VerifyString = await UserViewModel.Current.GetVerifyString();
            UpdateControls(true, msg == null);
            UpdateErrorMsg(msg);
            if (msg == null)
            {
                if (isFirstLoginCache)
                    IsFirstLogin = false;
                SavedUserName = UsernameText.Text;
                SavedPassword = pass;
            }
        }

        private void UpdateControls(bool isFinished, bool useTransition = false)
        {
            LoginButton.IsEnabled = isFinished;

            if (isFinished)
            {
                LoginButton.Content = AppResources.AccountPageLogin;
                if (useTransition)
                    LoginTransition.Begin();
            }
            else
            {
                LoginButton.Content = AppResources.AccountPageLoginLoading;
            }
        }

        private void UpdateErrorMsg(string msg)
        {
            ShowError.Stop();
            ErrorMsg.Text = msg;
            ShowError.Begin();
        }

        private const string IsFirstLoginKeyName = "IsFirstLogin";
        private const bool IsFirstLoginDefault = true;
        private static bool isFirstLoginCache = true;
        public static bool IsFirstLogin
        {
            get
            {
                return isFirstLoginCache = GetValueOrDefault<bool>(IsFirstLoginKeyName, IsFirstLoginDefault);
            }
            set
            {
                if (AddOrUpdateValue(IsFirstLoginKeyName, value))
                {
                    Save();
                }
            }
        }

        private const string IsRememberPassKeyName = "IsRememberPass";
        private const bool IsRememberPassDefault = true;
        private static bool? _isRememberPass;
        public static bool IsRememberPass
        {
            get
            {
                return (bool)(_isRememberPass ?? (_isRememberPass = GetValueOrDefault<bool>(IsRememberPassKeyName, IsRememberPassDefault)));
            }
            set
            {
                if (AddOrUpdateValue(IsRememberPassKeyName, value))
                {
                    Save();
                    _isRememberPass = value;
                }
            }
        }

        private const string SavedUserNameKeyName = "SavedUserName";
        private static string _savedUserName;
        private static string SavedUserName
        {
            get
            {
                return _savedUserName ?? (_savedUserName = GetValueOrDefault<string>(SavedUserNameKeyName, ""));
            }
            set
            {
                if (AddOrUpdateValue<string>(SavedUserNameKeyName, value))
                {
                    Save();
                    CurrentUsername = _savedUserName = value;
                }
            }
        }

        private const string FakePassword = "F…A…K…E";
        private const string SavedPasswordKeyName = "SavedPassword";
        private static string _savedPassword;
        private static string SavedPassword
        {
            get
            {
                return _savedPassword ?? (_savedPassword = GetValueOrDefault<string>(SavedPasswordKeyName, ""));
            }
            set
            {
                if (AddOrUpdateValue<string>(SavedPasswordKeyName, value))
                {
                    Save();
                    CurrentPassword = _savedPassword = value;
                }
            }
        }

        private const string VerifyStringKeyName = "VerifyString";
        private static string _verifyString;
        internal static string VerifyString
        {
            get
            {
                if (IsRememberPass)
                    return _verifyString ?? (_verifyString = GetValueOrDefault<string>(VerifyStringKeyName, ""));
                else
                    return _verifyString;
            }
            set
            {
                if (IsRememberPass)
                {
                    if (AddOrUpdateValue<string>(VerifyStringKeyName, value))
                    {
                        Save();
                        _verifyString = value;
                    }
                }
                else
                    _verifyString = value;
            }
        }

        private void PasswordLostFocus(object sender, RoutedEventArgs e)
        {
            CheckPasswordWatermark();
        }

        public void CheckPasswordWatermark()
        {
            var passwordEmpty = string.IsNullOrEmpty(PasswordText.Password);
            PasswordWatermark.Opacity = passwordEmpty ? 100 : 0;
            PasswordText.Opacity = passwordEmpty ? 0 : 100;
        }

        private void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            PasswordWatermark.Opacity = 0;
            PasswordText.Opacity = 100;

            PasswordText.SelectAll();
        }

        #endregion

    }
}