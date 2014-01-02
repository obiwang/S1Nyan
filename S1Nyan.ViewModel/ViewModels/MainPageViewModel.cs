using System;
using System.Collections.Generic;
using Caliburn.Micro;
using S1Nyan.Model;
using S1Nyan.Utils;
using S1Parser;

namespace S1Nyan.ViewModels
{

    public class MainPageViewModel : S1NyanViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainPageViewModel(IDataService dataService, 
            IEventAggregator eventAggregator, 
            INavigationService navigationService, 
            IUserService userService,
            IServerModel serverModel) //TODO: move serverModel init somewhere else
            : base(eventAggregator)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            _userService = userService;
        }

        private IEnumerable<S1ListItem> _data = null;
        /// <summary>
        /// Sets and gets the ListData property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IEnumerable<S1ListItem> MainListData
        {
            get { return _data ?? (_data = _dataService.GetMainListCache()); }
            set
            {
                if (_data == value) return;

                _data = value;
                NotifyOfPropertyChange(() => MainListData);
            }
        }

        public override async void RefreshData()
        {
            Util.Indicator.SetLoading();
            try
            {
                MainListData = await _dataService.UpdateMainListAsync();
                Util.Indicator.SetBusy(false);
                _dataService.GetMainListDone();
            }
            catch (Exception e)
            {
                _dataService.GetMainListDone(false);
                if (!HandleUserException(e))
                    Util.Indicator.SetError(e);
            }
        }

        public void GoToAccount()
        {
            _navigationService.Navigate(new Uri("/Views/SettingView.xaml?Pivot=Account", UriKind.Relative));
        }

        public void DoNavigation(S1ListItem item)
        {
            if (item != null)
                _navigationService
                    .UriFor<ThreadListViewModel>()
                    .WithParam(vm => vm.Fid, item.Id)
                    .WithParam(vm => vm.Title, item.Title)
                    .Navigate();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _userService.InitLogin();
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}