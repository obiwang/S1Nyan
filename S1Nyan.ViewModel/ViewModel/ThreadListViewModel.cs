using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using S1Nyan.Model;
using S1Nyan.Utils;
using S1Parser;

namespace S1Nyan.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ThreadListViewModel : S1NyanViewModelBase
    {
        private IDataService _dataService;
        
        /// <summary>
        /// Initializes a new instance of the ThreadListViewModel class.
        /// </summary>
        public ThreadListViewModel(IDataService dataService) : base()
        {
            _dataService = dataService;

            isUnregisterMessageDuringCleanUp = false;

            //if (IsInDesignMode) _dataService.GetThreadListData(null, 0, (item, error) => { ThreadListData = item; });

            //Buttons = new ObservableCollection<ButtonViewModel>();

            //Buttons.Add(new ButtonViewModel {
            //    IconUri = new Uri("/Assets/AppBar/appbar.sync.rest.png", UriKind.Relative),
            //    Text = "refresh",
            //    Command = new RelayCommand(() => RefreshThreadList())
            //});
            //Buttons.Add(new ButtonViewModel
            //{
            //    IconUri = new Uri("/Assets/AppBar/appbar.back.rest.png", UriKind.Relative),
            //    Text = "pre",
            //    Command = preCommand
            //});
            //Buttons.Add(new ButtonViewModel
            //{
            //    IconUri = new Uri("/Assets/AppBar/appbar.next.rest.png", UriKind.Relative),
            //    Text = "pre",
            //    Command = nextCommand
            //});
        }

        //RelayCommand preCommand { get { return new RelayCommand(() => CurrentPage--, () => CurrentPage > 1 && TotalPage > 1); } }

        //RelayCommand nextCommand { get { return new RelayCommand(() => CurrentPage++, () => CurrentPage < TotalPage && TotalPage > 1); } }

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
                RaisePropertyChanged(() => Title);
            }
        }

        private S1ThreadList _threadListData = null;

        /// <summary>
        /// Sets and gets the ThreadListData property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public S1ThreadList ThreadListData
        {
            get { return _threadListData; }
            set
            {
                if (_threadListData == value) return;

                _threadListData = value;
                RaisePropertyChanged(() => ThreadListData);
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
                    if (PageChanged != null)
                    {
                        PageChanged(CurrentPage, TotalPage);
                    }
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
        public void OnChangeFID(string fid, string title)
        {
            Title = title;
            if (fid != null)
            {
                _fid = fid;
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

        public Action<int, int> PageChanged { get; set; }

        public override void Cleanup()
        {
            base.Cleanup();
            ThreadListData = null;
            PageChanged = null;
        }
    }
}