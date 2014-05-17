using Caliburn.Micro;
using S1Nyan.Model;
using S1Nyan.Utils;
using S1Parser;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S1Nyan.ViewModels
{

    public class MainPageViewModel : S1NyanViewModelBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainPageViewModel(
            IDataService dataService, 
            IEventAggregator eventAggregator, 
            INavigationService navigationService, 
            IUserService userService)
            : base(dataService, eventAggregator, navigationService)
        {
            _userService = userService;
        }

        private IEnumerable<S1ListItem> _data = null;
        /// <summary>
        /// Sets and gets the ListData property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IEnumerable<S1ListItem> MainListData
        {
            get { return _data; }
            set
            {
                if (_data == value) return;

                _data = value;
                NotifyOfPropertyChange(() => MainListData);
            }
        }

        public override async Task RefreshData()
        {
            Util.Indicator.SetLoading();
            try
            {
                MainListData = await _dataService.UpdateMainListAsync();
            }
            catch (Exception) { }
            Util.Indicator.SetBusy(false);
        }

        public void GoToAccount()
        {
            //TODO
            //_navigationService.Navigate(new Uri("/Views/SettingView.xaml?Pivot=Account", UriKind.Relative));
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

        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (_userService != null)
                _userService.InitLogin();
            MainListData = await _dataService.GetMainListCache();
            await RefreshData();
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}