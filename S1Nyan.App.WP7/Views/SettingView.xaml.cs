using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using S1Nyan.App.Resources;
using S1Nyan.App.Utils;

namespace S1Nyan.App
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

        private static List<string> themes = new List<string> { AppResources.ThemeS1, AppResources.ThemeSystem };
        private static List<string> showPics = new List<string> { AppResources.ShowPicNone, AppResources.ShowPicOnlyWifi, AppResources.ShowPicAlways };
        private static List<string> fontSizes = new List<string> { AppResources.FontSizeSmall, AppResources.FontSizeMiddle, AppResources.FontSizeLarge };
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
            InitSetting();
        }

        private void InitSetting()
        {
            
            InitSetTheme();

            InitSetShowPic();

            InitSetFontSize();

            InitSetAutoRotate();
        }

        private void InitSetAutoRotate()
        {
            setAutoRotate.IsChecked = IsAutoRotateSetting;
            setAutoRotate.Click += (o, e) =>
            {
                IsAutoRotateSetting = setAutoRotate.IsChecked ?? false;
                var msg = new NotificationMessage("IsAutoRotateChanged");

            };
        }

        private void InitSetFontSize()
        {
            setFontSize.ItemsSource = fontSizes;
            setFontSize.SelectedItem = fontSizes[(int)ContentFontSize];
            setFontSize.SelectionChanged += (o, e) =>
            {
                ContentFontSize = (SettingFontSizes)setFontSize.SelectedIndex;
                ApplyFontSize();
            };
        }

        private void ApplyFontSize()
        {
            
        }

        private void InitSetShowPic()
        {
            setShowPic.ItemsSource = showPics;
            setShowPic.SelectedItem = showPics[(int)ShowPicWhen];
            setShowPic.SelectionChanged += (o, e) =>
            {
                ShowPicWhen = (SettingShowPicsWhen)setShowPic.SelectedIndex;
            };
        }

        private void InitSetTheme()
        {
            setTheme.ItemsSource = themes;
            setTheme.SelectedItem = themes[(int)AppTheme];
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

        /// <summary>
        /// Property to get and set a CheckBox Setting Key.
        /// </summary>
        public static bool IsAutoRotateSetting
        {
            get
            {
                return GetValueOrDefault<bool>(IsAutoRotateSettingKeyName, IsAutoRotateSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(IsAutoRotateSettingKeyName, value))
                {
                    Save();
                }
            }
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

        /// <summary>
        /// Property to get and set a ListBox Setting Key.
        /// </summary>
        public static SettingShowPicsWhen ShowPicWhen
        {
            get
            {
                return GetValueOrDefault<SettingShowPicsWhen>(ShowPicWhenKeyName, ShowPicsDefault);
            }
            set
            {
                if (AddOrUpdateValue<int>(ShowPicWhenKeyName, (int)value))
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Property to get and set a ListBox Setting Key.
        /// </summary>
        public static SettingFontSizes ContentFontSize
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
                }
            }
        }
#endregion


        internal static ApplicationBarMenuItem GetSettingMenuItem()
        {
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.SettingPage);
            appBarMenuItem.Click += GotoSetting;
            return appBarMenuItem;
        }

        internal static ApplicationBarIconButton GetSettingAppBarButton()
        {
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.feature.settings.rest.png", UriKind.Relative));
            appBarButton.Text = AppResources.SettingPage;
            appBarButton.Click += GotoSetting;
            return appBarButton;
        }

        private static void GotoSetting(object sender, EventArgs e)
        {
            (App.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Views/SettingView.xaml", UriKind.Relative));
        }

    }
}