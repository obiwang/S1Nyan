using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Practices.ServiceLocation;
using ObiWang.Controls;
using S1Nyan.Model;
using S1Nyan.Resources;
using S1Nyan.ViewModels;
using S1Parser;

namespace S1Nyan.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        bool isDataLoaded = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            BuildLocalizedApplicationBar();

            SettingView.UpdateOrientation(this);
            Loaded += PageLoaded; 
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            SettingView.UpdateOrientation(this);
            Debug.Assert(IoC.Get<MainPageViewModel>() != null);

            //if (Vm == null)
            //{
            //    DataContext = ServiceLocator.Current.GetInstance<MainViewModel>();
            //    DataLoaded();
            //}

            DataLoaded();
        }

        private void DataLoaded()
        {
            if (isDataLoaded) return;
            isDataLoaded = true;
            Popup.Visibility = Visibility.Collapsed;
            ApplicationBar.IsVisible = true;
            UserViewModel.Current.InitLogin();
        }

        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public MainPageViewModel Vm
        {
            get
            {
                return (MainPageViewModel)DataContext;
            }
        }

        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var longListSelector = sender as LongListSelector;
            if (longListSelector != null)
            {
                DoNavigation(longListSelector.SelectedItem);
                longListSelector.SelectedItem = null;
            }
            else
            {
                var selector = sender as ExpandableItem;
                if (selector == null) return;
                DoNavigation(selector.SelectedItem);
                selector.SelectedItem = null;
            }
        }

        private void DoNavigation(object o)
        {
            S1ListItem item = o as S1ListItem; 
            if (item != null)
                NavigationService.Navigate(new Uri("/Views/ThreadList.xaml?ID=" + item.Id + "&Title=" + item.Title, UriKind.Relative));
        }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Buttons.Add(S1NyanThreadAppBarButton);
            ApplicationBar.Buttons.Add(SettingView.GetSettingAppBarButton());
            ApplicationBar.Buttons.Add(SettingView.GetAboutAppBarButton());
            ApplicationBar.IsVisible = false;
        }

        private ApplicationBarIconButton _s1NyanThreadAppBarButton;

        ApplicationBarIconButton S1NyanThreadAppBarButton
        {
            get
            {
                if (_s1NyanThreadAppBarButton == null)
                {
                    _s1NyanThreadAppBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.column.one.png", UriKind.Relative));
                    _s1NyanThreadAppBarButton.Text = AppResources.S1NyanThread;
                    _s1NyanThreadAppBarButton.Click += (o, e) => NavigationService.Navigate(new Uri("/Views/ThreadView.xaml?ID=899264", UriKind.Relative));
                }

                return _s1NyanThreadAppBarButton;
            }
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
#if DEBUG
                GC.Collect();
                GC.WaitForPendingFinalizers();
#endif
            }
        }
    }
}