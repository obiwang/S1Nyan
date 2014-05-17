using System;
using System.Threading;
using Windows.UI.ViewManagement;

namespace S1Nyan.Utils
{
    public class Indicator : IIndicator
    {
        private StatusBarProgressIndicator _progressIndicator
        {
            get { return StatusBar.GetForCurrentView().ProgressIndicator; }
        }

        readonly Timer _timer;

        public Indicator()
        {
            _timer = new Timer(TimeUp, this, -1, -1);
        }

        private static void TimeUp(object state)
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.RunAsync(() => (state as Indicator).SetBusy(false));
        }

        public void SetBusy(bool isBusy)
        {
            if (isBusy)
            {
                _progressIndicator.ShowAsync();
                _progressIndicator.ProgressValue = null;
            }
            else
            {
                _progressIndicator.HideAsync();
                _progressIndicator.ProgressValue = 0;
            }
        }

        public void SetText(string text)
        {
            _progressIndicator.Text = text;
        }

        public void SetError(string text)
        {
            _progressIndicator.Text = text;
            _progressIndicator.ProgressValue = 0;
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
