using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using S1Nyan.Utils;
using S1Parser;
using S1Parser.User;

namespace S1Nyan.ViewModel
{
    public abstract class S1NyanViewModelBase : ViewModelBase
    {
        private static TimeSpan checkInterval = TimeSpan.FromMinutes(10);
        private static DateTime lastCheck = DateTime.Now - checkInterval;

        public S1NyanViewModelBase()
        {
            MessengerInstance.Register<NotificationMessage<S1NyanViewModelBase>>(this, OnNotifyRefresh);
            MessengerInstance.Register<NotificationMessage<string>>(this, OnNotifyServerMsg);
        }

#if DEBUG
        ~S1NyanViewModelBase()
        {
            System.Diagnostics.Debug.WriteLine("Finalizing " + this.GetType().FullName);
        }
#endif

        private void OnNotifyServerMsg(NotificationMessage<string> msg)
        {
            NotifyMessage = msg.Content;
        }

        private void OnNotifyRefresh(NotificationMessage<S1NyanViewModelBase> msg)
        {
            if (msg.Notification != "RefreshMessage") return;
            if (msg.Content!= null && msg.Content.GetType().IsInstanceOfType(this))
            {
                RefreshData();
            }
        }

        abstract public void RefreshData();

        protected bool isUnregisterMessageDuringCleanUp = true;
        public override void Cleanup()
        {
            base.Cleanup();
            NotifyMessage = null;
            if (isUnregisterMessageDuringCleanUp)
                MessengerInstance.Unregister<NotificationMessage>(this);
        }

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

                ServerViewModel.Current.CheckServerStatus(this);

                return true;
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
                RaisePropertyChanged(() => NotifyMessage);
            }
        }
    }
}
