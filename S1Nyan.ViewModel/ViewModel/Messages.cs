using GalaSoft.MvvmLight.Messaging;

namespace S1Nyan.ViewModel
{
    public static class Messages
    {
        public static NotificationMessage PostSuccessMessage = new NotificationMessage("PostSuccessMsg");

        public const string ReLoginMessageString = "ReLoginMsg";
        public const string NotifyServerMessageString = "NotifyServerMsg";
        public const string RefreshMessageString = "RefreshMessage";
        public const string LoginStatusChangedMessageString = "LoginStatusChangedMsg";
    }
}
