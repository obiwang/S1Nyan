using System;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Net.NetworkInformation;

namespace UnderDev
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class Page1ViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the Page1ViewModel class.
        /// </summary>
        public Page1ViewModel()
        {
            timer = new Timer(UpdateStatus, null, 2000, 5000);
            DispatcherHelper.Initialize();
            DeviceNetworkInformation.NetworkAvailabilityChanged += DeviceNetworkInformation_NetworkAvailabilityChanged;
        }

        private void UpdateStatus(object state)
        {
            CellularMobileOperator = DeviceNetworkInformation.CellularMobileOperator;
            IsCellularDataEnabled = DeviceNetworkInformation.IsCellularDataEnabled;
            IsNetworkAvailable = DeviceNetworkInformation.IsNetworkAvailable;
            IsWiFiEnabled = DeviceNetworkInformation.IsWiFiEnabled;
            DispatcherHelper.RunAsync(() =>
            {
                RaisePropertyChanged(() => IsCellularDataEnabled);
                RaisePropertyChanged(() => IsNetworkAvailable);
                RaisePropertyChanged(() => IsWiFiEnabled);
                RaisePropertyChanged(() => CellularMobileOperator);
            });
        }

        void DeviceNetworkInformation_NetworkAvailabilityChanged(object sender, NetworkNotificationEventArgs e)
        {
            NetworkAvailabilityChanged = e.NotificationType.ToString();
            DispatcherHelper.RunAsync(() =>
                RaisePropertyChanged(() => NetworkAvailabilityChanged));
        }

        public Timer timer { get; set; }

        public bool IsCellularDataEnabled { get; set; }

        public bool IsNetworkAvailable { get; set; }

        public string CellularMobileOperator { get; set; }

        public bool IsWiFiEnabled { get; set; }

        public string NetworkAvailabilityChanged { get; set; }

    }
}