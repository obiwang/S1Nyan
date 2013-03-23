using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using S1Nyan.Resources;

namespace S1Nyan.Views
{
    public enum SettingThemes
    {
        S1 = 0,
        System = 1
    }

    public enum SettingShowPicsWhen
    {
        None = 0,
        OnlyWifi = 1,
        Always = 2
    }

    public enum SettingFontSizes
    {
        FontSizeUnknow = -1,
        FontSizeSmall = 0,
        FontSizeMiddle = 1,
        FontSizeLarge = 2
    }

    public partial class SettingView : PhoneApplicationPage
    {
        static SettingView()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        private static List<string> themeSource = new List<string> { AppResources.ThemeS1, AppResources.ThemeSystem };
        private static List<string> showPicSource = new List<string> { AppResources.ShowPicNone, AppResources.ShowPicOnlyWifi, AppResources.ShowPicAlways };
        private static List<string> fontSizeSource = new List<string> { AppResources.FontSizeSmall, AppResources.FontSizeMiddle, AppResources.FontSizeLarge };
        private static IsolatedStorageSettings settings;

        const string IsAutoRotateSettingKeyName = "IsEnableAutoRotate";
        const string AppThemeKeyName = "AppTheme";
        const string ShowPicWhenKeyName = "ShowPicWhen";
        const string ContentFontSizeKeyName = "ContentFontSize";

        const bool IsAutoRotateSettingDefault = false;
        const SettingThemes AppThemeDefault = SettingThemes.S1;
        const SettingShowPicsWhen ShowPicsDefault = SettingShowPicsWhen.OnlyWifi;
        const SettingFontSizes ContentFontSizeDefault = SettingFontSizes.FontSizeMiddle;

        private static Theme SystemTheme;

        #region Initializations

        public SettingView()
        {
            InitializeComponent();
            Loaded += SettingView_Loaded;
            Unloaded += SettingView_Unloaded;
            SettingPivot.Loaded += SettingLoaded;
            AboutPivot.Loaded += AboutLoaded;
            AccountPivot.Loaded += AccountLoaded;
            DataContext = this;
        }

        void SettingView_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister(this);
        }

        void SettingView_Loaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Register<NotificationMessage<bool>>(this, OnLoginStatusChanged);
        }

        private void AccountLoaded(object sender, RoutedEventArgs e)
        {
            InitAccount();
        }

        bool isAboutInit = false;
        private void AboutLoaded(object sender, RoutedEventArgs e)
        {
            if(isAboutInit) return;
            InitAbout();
            isAboutInit = true;
        }

        bool isSettingInit = false;
        private void SettingLoaded(object sender, RoutedEventArgs e)
        {
            if (isSettingInit) return;

            InitSetTheme();

            InitSetShowPic();

            InitSetFontSize();

            InitSetAutoRotate();

            InitSignature();

            isSettingInit = true;
        }

        private void InitSetAutoRotate()
        {
            setAutoRotate.IsChecked = IsAutoRotateSetting;
        }

        private void InitSetFontSize()
        {
            setFontSize.ItemsSource = fontSizeSource;
            setFontSize.SelectedItem = fontSizeSource[(int)SettingFontSize];
            setFontSize.SelectionChanged += (o, e) =>
            {
                SettingFontSize = (SettingFontSizes)setFontSize.SelectedIndex;
            };
        }

        private void InitSetShowPic()
        {
            setShowPic.ItemsSource = showPicSource;
            setShowPic.SelectedItem = showPicSource[(int)ShowPicWhen];
            setShowPic.SelectionChanged += (o, e) =>
            {
                ShowPicWhen = (SettingShowPicsWhen)setShowPic.SelectedIndex;
            };
        }

        private void InitSetTheme()
        {
            setTheme.ItemsSource = themeSource;
            setTheme.SelectedItem = themeSource[(int)AppTheme];
            setTheme.SelectionChanged += (o, e) =>
            {
                AppTheme = (SettingThemes)setTheme.SelectedIndex;
            };
        }

        private static Uri lightTheme = new Uri("/S1Nyan.App.WP7;component/Resources/Themes/LightThemeResources.xaml", System.UriKind.Relative);
        private static Uri darkTheme = new Uri("/S1Nyan.App.WP7;component/Resources/Themes/DarkThemeResources.xaml", System.UriKind.Relative);
        private static Uri s1Theme = new Uri("/S1Nyan.App.WP7;component/Resources/Themes/CustomThemeResources.xaml", System.UriKind.Relative);

        private static void ApplyTheme(bool isInit = false)
        {
            if (AppTheme == SettingThemes.System)
            {   //apply system theme, not at init
                if (isInit) return;
                if (SystemTheme == Theme.Light)
                {
                    ThemeManager.SetCustomTheme(lightTheme, Theme.Light);
                    ThemeManager.ToLightTheme();
                }
                else
                {
                    ThemeManager.SetCustomTheme(darkTheme, Theme.Dark);
                    ThemeManager.ToDarkTheme();
                }
            }
            else
            {
                ThemeManager.SetCustomTheme(s1Theme, Theme.Light);
                ThemeManager.ToLightTheme();
            }
        }

        internal static void InitTheme()
        {
            SystemTheme = ((Visibility)Application.Current.Resources["PhoneLightThemeVisibility"] == Visibility.Visible) ? Theme.Light : Theme.Dark;
            ApplyTheme(true);
        }
        #endregion

        #region wrappers

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool AddOrUpdateValue<T>(string Key, T value)
            where T: IEquatable<T>
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (!((T)settings[Key]).Equals(value))
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public static void Save()
        {
            settings.Save();
        }

        #endregion

        #region Setting Properties

        private static bool? isAutoRotate;
        /// <summary>
        /// Property to get and set a CheckBox Setting Key.
        /// </summary>
        public static bool IsAutoRotateSetting
        {
            get
            {
                return (bool)(isAutoRotate ?? (isAutoRotate = GetValueOrDefault<bool>(IsAutoRotateSettingKeyName, IsAutoRotateSettingDefault)));
            }
            set
            {
                if (AddOrUpdateValue(IsAutoRotateSettingKeyName, value))
                {
                    Save();
                    isAutoRotate = value;
                }
            }
        }

        public static void UpdateOrientation(PhoneApplicationPage page)
        {
            page.SupportedOrientations = IsAutoRotateSetting ? SupportedPageOrientation.PortraitOrLandscape : SupportedPageOrientation.Portrait;
        }

        /// <summary>
        /// Property to get and set a ListBox Setting Key.
        /// </summary>
        public static SettingThemes AppTheme
        {
            get
            {
                return GetValueOrDefault<SettingThemes>(AppThemeKeyName, AppThemeDefault);
            }
            set
            {
                if (AddOrUpdateValue<int>(AppThemeKeyName, (int)value))
                {
                    ApplyTheme();

                    Save();
                }
            }
        }

        private static SettingShowPicsWhen? showPicWhen;
        /// <summary>
        /// Property to get and set a ListBox Setting Key.
        /// </summary>
        public static SettingShowPicsWhen ShowPicWhen
        {
            get
            {
                return (SettingShowPicsWhen)(showPicWhen ?? (showPicWhen = GetValueOrDefault<SettingShowPicsWhen>(ShowPicWhenKeyName, ShowPicsDefault)));
            }
            set
            {
                if (AddOrUpdateValue<int>(ShowPicWhenKeyName, (int)value))
                {
                    Save();
                    showPicWhen = value;
                }
            }
        }

        public static bool IsShowPic
        {
            get
            {
                switch (ShowPicWhen)
                {
                    case SettingShowPicsWhen.Always:
                        return true;
                    case SettingShowPicsWhen.None:
                        return false;
                    default:
                        return DeviceNetworkInformation.IsWiFiEnabled;
                }
            }
        }

        /// <summary>
        /// Property to get and set a ListBox Setting Key.
        /// </summary>
        public static SettingFontSizes SettingFontSize
        {
            get
            {
                return GetValueOrDefault<SettingFontSizes>(ContentFontSizeKeyName, ContentFontSizeDefault);
            }
            set
            {
                if (AddOrUpdateValue<int>(ContentFontSizeKeyName, (int)value))
                {
                    Save();
                    contentFontSize = GetFontSize(value);
                }
            }
        }

        private static double contentFontSize = 0;
        public static double ContentFontSize {
            get
            {
                if (contentFontSize == 0) 
                    contentFontSize = GetFontSize();
                return contentFontSize;
            }
        }

        private static double GetFontSize(SettingFontSizes size = SettingFontSizes.FontSizeUnknow)
        {
            if (size == SettingFontSizes.FontSizeUnknow) size = SettingFontSize;
            switch (size)
            {
                case SettingFontSizes.FontSizeLarge:
                    return 28.667;
                case SettingFontSizes.FontSizeSmall:
                    return 22.667;
                default:
                    return 25.333;
            }
        }

        #endregion


        #region Signature
        private const string ShowSignatureKeyName = "ShowSignature";
        private static SignatureTypes ShowSignatureDefault = SignatureTypes.ShowAppNameAndModel;

        private const string AppDisplaySignature = "from S1 Nyan";
        private const string AppPostSignature = "\r\n    —— from [url=http://126.am/S1Nyan]S1 Nyan [/url]";
        private static string ModelName = string.Format(" ({0})", S1Nyan.Utils.PhoneDeviceModel.GetFriendlyName());

        private enum SignatureTypes
        {
            None = 0,
            ShowAppName,
            ShowAppNameAndModel
        }

        private void InitSignature()
        {
            setSignature.ItemsSource = signatureSource;
            setSignature.SelectedIndex = (int)SignatureType;
            setSignature.SelectionChanged += (o, e) =>
            {
                SignatureType = (SignatureTypes)setSignature.SelectedIndex;
            };
        }

        private static List<string> signatureSource = new List<string> { AppResources.ShowSignatureNone, AppDisplaySignature, AppDisplaySignature + ModelName };
        private static List<string> postSignatureSource = new List<string> { "", AppPostSignature, AppPostSignature + ModelName };

        public static string GetSignature()
        {
            return postSignatureSource[(int)SignatureType];
        }

        private static SignatureTypes? signatureType;
        private static SignatureTypes SignatureType
        {
            get
            {
                return (SignatureTypes)(signatureType ?? (signatureType = GetValueOrDefault<SignatureTypes>(ShowSignatureKeyName, ShowSignatureDefault)));
            }
            set
            {
                if (AddOrUpdateValue<int>(ShowSignatureKeyName, (int)value))
                {
                    Save();
                    signatureType = value;
                }
            }
        }

        #endregion

        #region About section

        private void InitAbout()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length > 0)
                CopyRightText.Text = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;

            var version = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version;
            VersionText.Text = string.Format("v {0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }

        private void OnFeedBack(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = AppResources.FeedBackSubject;
            emailComposeTask.To = AppResources.FeedBackEmail;

            emailComposeTask.Show();
        }

        private void OnRate(object sender, RoutedEventArgs e)
        {
            new MarketplaceReviewTask().Show();
        }

        #endregion

        #region Server Settings
        private const string CurrentServerAddrKeyName = "CurrentServerAddr";
        private static string _currentServerAddr;
        internal static string CurrentServerAddr
        {
            get
            {
                return _currentServerAddr ?? (_currentServerAddr = GetValueOrDefault<string>(CurrentServerAddrKeyName, null));
            }
            set
            {
                if (AddOrUpdateValue<string>(CurrentServerAddrKeyName, value))
                {
                    Save();
                    _currentServerAddr = value;
                }
            }
        }
        #endregion

        #region Setting Button & Menu
        internal static ApplicationBarMenuItem GetSettingMenuItem()
        {
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.SettingPage);
            appBarMenuItem.Click += (o, e) => GotoSetting();
            return appBarMenuItem;
        }

        internal static ApplicationBarIconButton GetSettingAppBarButton()
        {
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.feature.settings.rest.png", UriKind.Relative));
            appBarButton.Text = AppResources.SettingPage;
            appBarButton.Click += (o, e) => GotoSetting();
            return appBarButton;
        }

        internal static ApplicationBarIconButton GetAboutAppBarButton()
        {
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.information.png", UriKind.Relative));
            appBarButton.Text = AppResources.AboutPage;
            appBarButton.Click += (o, e) => GotoSetting(PivotAbout);
            return appBarButton;
        }

        public static void GotoSetting(string pivotName = null)
        {
            string param = "";
            if (pivotName != null)
                param = string.Format("?Pivot={0}", pivotName);

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Views/SettingView.xaml" + param, UriKind.Relative));
        }

        internal const string PivotAccount = "Account";
        internal const string PivotAbout = "About";

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back) return;
            string pivotName = null;
            if (NavigationContext.QueryString.TryGetValue("Pivot", out pivotName))
            {
                int index = 0;
                switch(pivotName)
                {
                    case PivotAccount:
                        index = 1;
                        break;
                    case PivotAbout:
                        index = 2;
                        break;
                }
                Pivot.SelectedIndex = index;
            }
        }
        #endregion

    }
}