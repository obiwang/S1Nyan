using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using S1Nyan.Resources;
using S1Nyan.Utils;
using S1Nyan.ViewModels;

namespace S1Nyan.Views
{
    public partial class SettingView : PhoneApplicationPage
    {
        bool isAccountInited = false;
        private void InitAccount()
        {
            if (UserViewModel.Current.Uid != null)
                stackPanel.SetValue(Canvas.TopProperty, -400.0);
            UpdateControls(!UserViewModel.Current.IsBusy, false, false);

            if (isAccountInited) return;
            isAccountInited = true;
            InitLogIn();
        }

        public static void InitAccountData()
        {
            CurrentUsername = SavedUserName;
            CurrentPassword = IsRememberPass ? SavedPassword : "";
        }

        private void OnLoginStatusChanged(object msg)
        {
            //TODO:
            //if (msg.Notification != Messages.LoginStatusChangedMessageString) return;
            //UpdateControls(!UserViewModel.Current.IsBusy, msg.Content, true);
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

            RegisterLink.NavigateUri = new System.Uri(S1Parser.S1Resource.SiteBase + "member.php?mod=register");
        }

        private void OnChangeAccount(object sender, RoutedEventArgs e)
        {
            LogoutTransition.Begin();
        }

        private async void OnLogin(object sender, RoutedEventArgs e)
        {
            UpdateControls(false, false, false);
            var pass = PasswordText.Password;
            if (pass == FakePassword)
                pass = CurrentPassword;
            var msg = await UserViewModel.Current.DoLogin(UsernameText.Text, pass);
            UpdateErrorMsg(msg);
            if (msg == null)
            {
                SavedUserName = UsernameText.Text;
                SavedPassword = pass;
            }
            UpdateControls(true, msg == null, true);
        }

        private void UpdateControls(bool isFinished, bool isSuccess, bool useTransition)
        {
            LoginButton.IsEnabled = isFinished;
            PasswordText.IsEnabled = isFinished;
            UsernameText.IsEnabled = isFinished;

            if (isFinished)
            {
                LoginButton.Content = AppResources.AccountPageLogin;
                if (useTransition && isSuccess)
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
        private static readonly SettingProperty<bool> IsFirstLoginSetting = new SettingProperty<bool>(IsFirstLoginKeyName, true);

        public static bool IsFirstLogin
        {
            get { return IsFirstLoginSetting.Value; }
            set { IsFirstLoginSetting.Value = value; }
        }

        private const string IsRememberPassKeyName = "IsRememberPass";
        private static readonly SettingProperty<bool> IsRememberPassSetting = new SettingProperty<bool>(IsRememberPassKeyName, true);
        public static bool IsRememberPass
        {
            get { return IsRememberPassSetting.Value; }
            set { IsRememberPassSetting.Value = value; }
        }

        private const string SavedUserNameKeyName = "SavedUserName";
        private static readonly SettingProperty<string> SavedUserNameSetting = new SettingProperty<string>(SavedUserNameKeyName, "");
        private static string SavedUserName
        {
            get { return SavedUserNameSetting.Value; }
            set { SavedUserNameSetting.Value = CurrentUsername = value; }
        }

        private const string FakePassword = "F…A…K…E";
        private const string SavedPasswordKeyName = "SavedPassword";
        private static readonly SettingProperty<string> SavedPasswordSetting = new SettingProperty<string>(SavedPasswordKeyName, "");
        private static string SavedPassword
        {
            get { return SavedPasswordSetting.Value; }
            set { SavedPasswordSetting.Value = CurrentPassword = value; }

        }

        internal static string VerifyString { get; set; }

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