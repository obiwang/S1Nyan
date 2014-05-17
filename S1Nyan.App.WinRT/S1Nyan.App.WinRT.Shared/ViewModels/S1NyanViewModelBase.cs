using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using S1Nyan.Model;
using S1Nyan.Utils;
using S1Nyan.ViewModels.Message;
using S1Parser;
using S1Parser.User;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace S1Nyan.ViewModels
{
    public abstract class S1NyanViewModelBase : Screen, IHandle<UserMessage>
    {
        private static TimeSpan checkInterval = TimeSpan.FromMinutes(10);
        private static DateTime lastCheck = DateTime.Now - checkInterval;

        protected readonly IEventAggregator _eventAggregator;
        protected readonly INavigationService _navigationService;
        protected readonly IDataService _dataService;
        protected Page HostedView { get; set; }

        public S1NyanViewModelBase(IDataService dataService, IEventAggregator eventAggregator, INavigationService navigationService)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            if (_eventAggregator != null)
                _eventAggregator.Subscribe(this);
            if (_navigationService != null)
                _navigationService.Navigating += NavigationServiceOnNavigating;
        }

        private void NavigationServiceOnNavigating(object sender, NavigatingCancelEventArgs navigatingCancelEventArgs)
        {
            //clear error msg or loading state when navigate away
            Util.Indicator.SetBusy(false);

            if (navigatingCancelEventArgs.NavigationMode == NavigationMode.Back)
            {
                _eventAggregator.Unsubscribe(this);
                if (_navigationService != null)
                    _navigationService.Navigating -= NavigationServiceOnNavigating;
            }
        }

#if DEBUG
        ~S1NyanViewModelBase()
        {
            System.Diagnostics.Debug.WriteLine("Finalizing " + this.GetType().FullName);
        }
#endif

        public virtual void Handle(UserMessage msg)
        {
            switch (msg.Type)
            {
                case Messages.NotifyServer:
                    NotifyMessage = msg.Content.ToString();
                    break;
                case Messages.Refresh:
                    if (msg.Content != null && msg.Content.GetType() == this.GetType())
                    {
                        RefreshData();
                    }
                    break;
            }
        }

        abstract public Task RefreshData();

        protected bool HandleUserException(Exception e)
        {
            if ((e is S1UserException && (e as S1UserException).ErrorType == UserErrorTypes.NotAuthorized))
            {
                NotifyMessage = e.Message;
            }

            if ((e is S1UserException && (e as S1UserException).ErrorType == UserErrorTypes.SiteClosed) ||
                (e is InvalidDataException) ||
                (e is System.Net.WebException && Util.ErrorMsg.IsNetworkAvailable()))
            {
                if (DateTime.Now - lastCheck < checkInterval) return false;
                lastCheck = DateTime.Now;

                //TODO:
                //ServerViewModel.Current.CheckServerStatus(this);
                //return true;
            }
            return false;
        }

        private string _notifyMessge = null;

        /// <summary>
        /// Sets and gets the NotifyMessage property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string NotifyMessage
        {
            get { return _notifyMessge; }

            set
            {
                _notifyMessge = value;
                NotifyOfPropertyChange(() => NotifyMessage);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            UpdateOrientation();
        }

        private void UpdateOrientation()
        {
            IoC.Get<IOrientationHelper>().UpdateOrientation(HostedView);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            HostedView = view as Page;
            UpdateOrientation();
            if (HostedView is IViewLoaded)
                (HostedView as IViewLoaded).ViewLoaded(this);
        }

    }
}
