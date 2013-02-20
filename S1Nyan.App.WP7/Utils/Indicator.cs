using System.Threading;
using Microsoft.Phone.Shell;
using S1Nyan.App.Resources;

namespace S1Nyan.Utils
{
    public class Indicator : S1Nyan.Utils.IIndicator
    {
        ProgressIndicator _progressIndicator;
        Timer timer;

        public Indicator()
        {
            timer = new Timer(TimeUp, this, uint.MaxValue, uint.MaxValue);
        }
        private static void TimeUp(object state)
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.RunAsync(() => (state as Indicator).SetBusy(false));
        }

        public void SetBusy(bool isBusy)
        {
            if (null == _progressIndicator)
            {
                _progressIndicator = new ProgressIndicator();
            }
            if (isBusy)
                SystemTray.ProgressIndicator = _progressIndicator;
            _progressIndicator.IsIndeterminate = isBusy;
            _progressIndicator.IsVisible = isBusy;
        }

        public void SetText(string text)
        {
            _progressIndicator.Text = text;
        }

        public void SetError(string text)
        {
            _progressIndicator.Text = text;
            _progressIndicator.IsIndeterminate = false;
            timer.Change(5000, uint.MaxValue);
        }

        public void SetLoading()
        {
            SetBusy(true);
            SetText(AppResources.AppLoading);
        }
    }
}
