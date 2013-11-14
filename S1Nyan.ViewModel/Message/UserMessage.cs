namespace S1Nyan.ViewModels.Message
{
    public enum Messages
    {
        ReLogin,
        NotifyServer,
        Refresh,
        LoginStatusChanged,
        PostSuccess,
    }

    public class UserMessage
    {
        public static UserMessage PostSuccessMessage = new UserMessage(Messages.PostSuccess);

        public UserMessage(Messages t, object o = null)
        {
            Type = t;
            Content = o;
        }

        public Messages Type { get; set; }

        public object Content { get; set; }
    }
}
