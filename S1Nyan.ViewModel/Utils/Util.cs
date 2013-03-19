using GalaSoft.MvvmLight.Ioc;

namespace S1Nyan.Utils
{
    public static class Util
    {
        private static IIndicator _indicator;
        public static IIndicator Indicator
        {
            get
            {
                return _indicator ?? (_indicator = SimpleIoc.Default.GetInstance<IIndicator>());
            }
        }

        private static IErrorMsg _errorMsg;
        public static IErrorMsg ErrorMsg
        {
            get
            {
                return _errorMsg ?? (_errorMsg = SimpleIoc.Default.GetInstance<IErrorMsg>());
            }
        }

        public static void SetError(this IIndicator indicator, System.Exception e)
        {
            indicator.SetError(ErrorMsg.GetExceptionMessage(e));
        }

        public static void SetText(this IIndicator indicator, System.Exception e)
        {
            indicator.SetText(ErrorMsg.GetExceptionMessage(e));
        }
    }
}
