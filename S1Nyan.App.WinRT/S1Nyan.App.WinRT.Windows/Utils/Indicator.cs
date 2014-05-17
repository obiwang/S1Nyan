using System;
using System.Threading;
using Windows.UI.ViewManagement;

namespace S1Nyan.Utils
{
    public class Indicator : IIndicator
    {
        readonly Timer _timer;

        public Indicator()
        {
            _timer = new Timer(TimeUp, this, TimeSpan.MaxValue, TimeSpan.MaxValue);
        }

        private static void TimeUp(object state)
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.RunAsync(() => (state as Indicator).SetBusy(false));
        }

        public void SetBusy(bool isBusy)
        {
        }

        public void SetText(string text)
        {
        }

        public void SetError(string text)
        {
            //_progressIndicator.Text = text;
            _timer.Change(TimeSpan.FromSeconds(5), TimeSpan.MaxValue);
        }

        public void SetLoading()
        {
            SetBusy(true);
            //SetText(AppResources.AppLoading);
            //TODO
            SetText("-Loading-");
        }
    }
}
