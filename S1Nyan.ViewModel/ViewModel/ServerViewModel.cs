using System;
using System.Collections.Generic;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using S1Nyan.Model;
using S1Nyan.Utils;
using S1Parser.User;

namespace S1Nyan.ViewModel
{
    public class ServerViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IServerModel _serverModel;

        public static ServerViewModel Current
        {
            get { return SimpleIoc.Default.GetInstance<ServerViewModel>(); }
        }

        private bool isCheckingStatus = false;
        /// <summary>
        /// Initializes a new instance of the ServerViewModel class.
        /// </summary>
        public ServerViewModel(IDataService dataService, IServerModel serverModel)
        {
            _dataService = dataService;
            _serverModel = serverModel;
        }

        private int serversToCheck = 0;

        private int ServersToCheck
        {
            get { return serversToCheck; }
            set
            {
                lock (this)
                {
                    serversToCheck = value;
                }

                if (serversToCheck == 0)
                {
                    NotifyError();
                    isCheckingStatus = false;
                    lastViewModel = null;
                }
                if (serversToCheck < 0)
                {
                    Util.Indicator.SetError(S1UserException.ServerUpdateSuccess);
                    isCheckingStatus = false;
                    lastViewModel = null;
                }
            }
        }

        private void NotifyError()
        {
            string msg = "";
            if (closedServer != null)
            {
                Util.Indicator.SetError(S1UserException.SiteClosed);
                msg = Util.ErrorMsg.GetExceptionMessage(S1UserException.SiteClosed);
                _serverModel.UpdateServerAddr(closedServer.Addr);
                MessengerInstance.Send(new NotificationMessage<S1NyanViewModelBase>(lastViewModel, Messages.ReLoginMessageString));
            }
            else
            {
                Util.Indicator.SetError(S1UserException.NoServerAvailable);

                if (lastException != null && lastException.Message != null)
                    msg = lastException.Message;
            }
            if (_serverModel.Msg != null && _serverModel.Msg.Length>0)
                msg = msg + "\r\n" + _serverModel.Msg;
            MessengerInstance.Send(new NotificationMessage<string>(msg, Messages.NotifyServerMessageString));
        }

        private void OnNotifySuccess(IServerItem item)
        {
            Debug.WriteLine("OnNotifySuccess ");
            //MainListData = data;
            foreach (var server in serverList)
            {
                server.NotifyComplete = null;
                server.NotifySuccess = null;
                server.Cancel();
            }
            _serverModel.UpdateServerAddr(item.Addr);
            MessengerInstance.Send(new NotificationMessage<S1NyanViewModelBase>(lastViewModel, Messages.RefreshMessageString));
            serverList = null;
            ServersToCheck = -1;
        }

        private void OnNotifyComplete(IServerItem item)
        {
            item.NotifyComplete = null;
            item.NotifySuccess = null;
            ServersToCheck--;
            Debug.WriteLine("OnNotifyComplete: " + item.Addr);
            if (item.UserException != null && closedServer == null)
            {
                lastException = item.UserException;
                if (item.UserException.ErrorType == UserErrorTypes.SiteClosed)
                    closedServer = item;
            }
        }

        IServerItem closedServer = null; 
        Exception lastException = null;
        S1NyanViewModelBase lastViewModel;
        private List<IServerItem> serverList = null;
        internal async void CheckServerStatus(S1NyanViewModelBase viewModel)
        {
            if (isCheckingStatus) return;
            lastViewModel = viewModel;
            lastException = null;
            closedServer = null;
            Util.Indicator.SetText(S1UserException.CheckServerStatus);
            isCheckingStatus = true;

            await _serverModel.UpdateListFromRemote();
            serverList = _serverModel.List;
            ServersToCheck = serverList.Count;
            foreach (var server in serverList)
            {
                server.NotifySuccess = OnNotifySuccess;
                server.NotifyComplete = OnNotifyComplete;
                server.ParserFactory = _dataService.ParserFactory;
                server.Check();
            }
        }
    }
}
