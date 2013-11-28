using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using S1Nyan.Resources;
using S1Nyan.Utils;

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
        private static List<string> themeSource = new List<string> { AppResources.ThemeS1, AppResources.ThemeSystem };
        private static List<string> showPicSource = new List<string> { AppResources.ShowPicNone, AppResources.ShowPicOnlyWifi, AppResources.ShowPicAlways };
        private static List<string> fontSizeSource = new List<string> { AppResources.FontSizeSmall, AppResources.FontSizeMiddle, AppResources.FontSizeLarge };

        const string IsAutoRotateSettingKeyName = "IsEnableAutoRotate";
        const string AppThemeKeyName = "AppTheme";
        const string ShowPicWhenKeyName = "ShowPicWhen";
        const string ContentFontSizeKeyName = "ContentFontSize";

        private static Theme SystemTheme;

        #region Initializations

        public SettingView()
        {
            InitializeComponent();
            //Loaded += SettingView_Loaded;
            //Unloaded += SettingView_Unloaded;
            SettingPivot.Loaded += SettingLoaded;
            AboutPivot.Loaded += AboutLoaded;
            AccountPivot.Loaded += AccountLoaded;
            DataContext = this;
        }

        //void SettingView_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    Messenger.Default.Unregister(this);
        //}

        //void SettingView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    Messenger.Default.Register<NotificationMessage<bool>>(this, OnLoginStatusChanged);
        //}

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
            setAutoRotate.IsChecked = IsAutoRotate;
            setAutoRotate.Click += (sender, args) =>
            {
                IsAutoRotate = (bool)setAutoRotate.IsChecked;
            };
        }

        private void InitSetFontSize()
        {
            setFontSize.ItemsSource = fontSizeSource;
            setFontSize.SelectedItem = fontSizeSource[(int)PostFontSize];
            setFontSize.SelectionChanged += (o, e) =>
            {
                PostFontSize = (SettingFontSizes)setFontSize.SelectedIndex;
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

        private static Uri lightTheme = new Uri("/S1Nyan;component/Resources/Themes/LightThemeResources.xaml", UriKind.Relative);
        private static Uri darkTheme = new Uri("/S1Nyan;component/Resources/Themes/DarkThemeResources.xaml", UriKind.Relative);
        private static Uri s1Theme = new Uri("/S1Nyan;component/Resources/Themes/CustomThemeResources.xaml", UriKind.Relative);

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

        #region Setting Properties

        private static readonly SettingProperty<bool> IsAutoRotateSetting = new SettingProperty<bool>(IsAutoRotateSettingKeyName);
        private static readonly SettingProperty<int> AppThemeSetting = new SettingProperty<int>(AppThemeKeyName, (int)SettingThemes.S1);
        private static readonly SettingProperty<int> ShowPicWhenSetting = new SettingProperty<int>(ShowPicWhenKeyName, (int)SettingShowPicsWhen.OnlyWifi);
        private static readonly SettingProperty<int> PostFontSizeSetting = new SettingProperty<int>(ContentFontSizeKeyName, (int)SettingFontSizes.FontSizeMiddle);

        /// <summary>
        /// Property to get and set a CheckBox Setting Key.
        /// </summary>
        public static bool IsAutoRotate
        {
            get { return IsAutoRotateSetting.Value; }
            set { IsAutoRotateSetting.Value = value; }
        }

        public static void UpdateOrientation(PhoneApplicationPage page)
        {
            page.SupportedOrientations = IsAutoRotate ? SupportedPageOrientation.PortraitOrLandscape : SupportedPageOrientation.Portrait;
        }

        /// <summary>
        /// Property to get and set a ListBox Setting Key.
        /// </summary>
        public static SettingThemes AppTheme
        {
            get { return (SettingThemes) AppThemeSetting.Value; }
            set
            {
                AppThemeSetting.Value = (int) value;
                ApplyTheme();
            }
        }

        /// <summary>
        /// Property to get and set a ListBox Setting Key.
        /// </summary>
        public static SettingShowPicsWhen ShowPicWhen
        {
            get { return (SettingShowPicsWhen)ShowPicWhenSetting.Value; }
            set { ShowPicWhenSetting.Value = (int)value; }
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
        public static SettingFontSizes PostFontSize
        {
            get { return (SettingFontSizes)PostFontSizeSetting.Value; }
            set
            {
                PostFontSizeSetting.Value = (int)value;
                _contentFontSize = GetFontSize(value);
            }

        }

        private static double _contentFontSize = 0;
        public static double ContentFontSize 
        {
            get
            {
                if (Math.Abs(_contentFontSize) < .01) 
                    _contentFontSize = GetFontSize();
                return _contentFontSize;
            }
        }

        private static double GetFontSize(SettingFontSizes size = SettingFontSizes.FontSizeUnknow)
        {
            if (size == SettingFontSizes.FontSizeUnknow) size = PostFontSize;
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
        private const string AppPostSignature = "\r\n    —— from [url=http://126.am/S1Nyan]S1 Nyan[/url]";
        private static string ModelName = string.Format(" ({0})", S1Nyan.Utils.PhoneDeviceModel.FriendlyName);

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

        private static readonly SettingProperty<int> SignatureTypeSetting = new SettingProperty<int>(ShowSignatureKeyName, (int)SignatureTypes.ShowAppNameAndModel);

        private static SignatureTypes SignatureType
        {
            get { return (SignatureTypes)SignatureTypeSetting.Value; }
            set { SignatureTypeSetting.Value = (int)value; }
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
            emailComposeTask.Body = "\r\n\r\n" + signatureSource[2];

            emailComposeTask.Show();
        }

        private void OnRate(object sender, RoutedEventArgs e)
        {
            new MarketplaceReviewTask().Show();
        }

        #endregion

        #region Server Settings
        private const string CurrentServerAddressKeyName = "CurrentServerAddress";
        private static readonly SettingProperty<string> CurrentServerAddressSetting = new SettingProperty<string>(CurrentServerAddressKeyName, null);
        internal static string CurrentServerAddress
        {
            get { return CurrentServerAddressSetting.Value; }
            set { CurrentServerAddressSetting.Value = value; }
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