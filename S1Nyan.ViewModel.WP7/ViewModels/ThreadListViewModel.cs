using System;
using Caliburn.Micro;
using S1Nyan.Model;
using S1Nyan.Utils;
using S1Parser;

namespace S1Nyan.ViewModels
{

    public class ThreadListViewModel : S1NyanViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the ThreadListViewViewModel class.
        /// </summary>
        public ThreadListViewModel(
            IDataService dataService, 
            IEventAggregator eventAggregator,
            INavigationService navigationService)
            : base(dataService, eventAggregator, navigationService)
        {
        }

        private string _title = "";

        /// <summary>
        /// Sets and gets the Title property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Title
        {
            get { return _title; }

            set
            {
                if (_title == value) return;

                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        private S1ThreadList _threadListData = null;

        /// <summary>
        /// Sets and gets the ThreadListViewData property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public S1ThreadList ThreadListData
        {
            get { return _threadListData; }
            set
            {
                if (_threadListData == value) return;

                _threadListData = value;
                NotifyOfPropertyChange(() => ThreadListData);
            }
        }

        public override async void RefreshData()
        {
            ThreadListData = null;
            if (null == _fid) return;
            Util.Indicator.SetLoading();
            try
            {
                var temp = await _dataService.GetThreadListAsync(_fid, CurrentPage);
                if (temp.CurrentPage == CurrentPage)
                {
                    ThreadListData = temp;
                    TotalPage = ThreadListData.TotalPage;
                    NotifyOfPropertyChange(() => CanNextPage);
                    NotifyOfPropertyChange(() => CanPrePage);
                    Util.Indicator.SetBusy(false);
                }
            }
            catch (Exception e)
            {
                if (!HandleUserException(e))
                    Util.Indicator.SetError(e);
            }
        }

        private string _fid = null;

        public string Fid
        {
            get { return _fid; }
            set
            {
                if (value == null || value == _fid) return;
                _fid = value;
                TotalPage = 0;
                CurrentPage = 1;
            }
        }

        public int TotalPage { get; set; }

        //may change during async actions, disable optimization
        volatile int currentPage;
        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (value > 0 )
                {
                    currentPage = value;
                    RefreshData();
                }
            }
        }

        //public override void Cleanup()
        //{
        //    base.Cleanup();
        //    ThreadListViewData = null;
        //    PageChanged = null;
        //}


        #region Bind ApplicaionBar Actions

        public void RefreshThreadList()
        {
            RefreshData();
        }

        public void PrePage()
        {
            CurrentPage--;
        }

        public bool CanPrePage
        {
            get { return CurrentPage > 1 && TotalPage > 1; }
        }

        public void NextPage()
        {
            CurrentPage++;
        }

        public bool CanNextPage
        {
            get { return CurrentPage < TotalPage && TotalPage > 1; }
        }

        public void GoToSetting()
        {
            _navigationService.Navigate(new Uri("/Views/SettingView.xaml", UriKind.Relative));
        }

        #endregion

        public void GoToAccount()
        {
            _navigationService.Navigate(new Uri("/Views/SettingView.xaml?Pivot=Account", UriKind.Relative));
        }

        public void DoNavigation(S1ListItem item)
        {
            if (item == null) return;

            _navigationService.UriFor<PostViewModel>()
                .WithParam(vm => vm.Title, item.Title)
                .WithParam(vm => vm.Tid, item.Id)
                .Navigate();
        }

    }
}