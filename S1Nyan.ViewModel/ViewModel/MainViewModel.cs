using System;
using System.Collections.Generic;
using S1Nyan.Model;
using S1Nyan.Utils;
using S1Parser;

namespace S1Nyan.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : S1NyanViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService) : base()
        {
            _dataService = dataService;
            MainListData = _dataService.GetMainListCache();
            RefreshData();
            //if (IsInDesignMode) _dataService.GetMainListData((item, error) => { MainListData = item; });
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
                RaisePropertyChanged(() => MainListData);
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
        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}