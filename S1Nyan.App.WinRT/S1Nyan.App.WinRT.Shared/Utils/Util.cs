using Caliburn.Micro;

namespace S1Nyan.Utils
{
    public static class Util
    {
        private static IIndicator _indicator;
        public static IIndicator Indicator
        {
            set { _indicator = value; }
            get
            {
                return _indicator ?? (_indicator = IoC.Get<IIndicator>());
            }
        }

        private static IErrorMsg _errorMsg;
        public static IErrorMsg ErrorMsg
        {
            set { _errorMsg = value; }
            get
            {
                return _errorMsg ?? (_errorMsg = IoC.Get<IErrorMsg>());
            }
        }

        public static void SetError(this IIndicator indicator, System.Exception e)
        {
            indicator.SetError(
                ErrorMsg != null 
                ? ErrorMsg.GetExceptionMessage(e) 
                : e.Message);
        }

        public static void SetText(this IIndicator indicator, System.Exception e)
        {
            indicator.SetText(ErrorMsg.GetExceptionMessage(e));
        }
    }
}
