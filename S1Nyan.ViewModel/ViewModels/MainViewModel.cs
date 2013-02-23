using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
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

        private RelayCommand _loadedCommand;
        /// <summary>
        /// Gets the LoadedCommand.
        /// </summary>
        public RelayCommand LoadedCommand
        {
            get
            {
                return _loadedCommand
                    ?? (_loadedCommand = new RelayCommand(ExecuteLoadedCommand));
            }
        }

        private bool isInited;
        private async void ExecuteLoadedCommand()
        {
            if (isInited) return;
            isInited = true;
            Util.Indicator.SetLoading();
            try
            {
                MainListData = await _dataService.GetMainListAsync();
                Util.Indicator.SetBusy(false);
            }
            catch (Exception e)
            {
                Util.Indicator.SetError(e.Message);
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}