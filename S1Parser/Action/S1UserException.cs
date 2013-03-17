using System;

namespace S1Parser.User
{

    public class LoginException : S1UserException
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public LoginException(string msg, string account, string pass)
            : base(msg)
        {
            Username = account;
            Password = pass;
            ErrorType = UserErrorTypes.LoginFailed;
        }
    }

    public class S1UserException : Exception
    {
        public static S1UserException NoServerAvailable = new S1UserException(UserErrorTypes.NoServerAvailable);
        public static S1UserException ServerUpdateSuccess = new S1UserException(UserErrorTypes.ServerUpdateSuccess);
        public static S1UserException InvalidVerify = new S1UserException(UserErrorTypes.InvalidVerify);
        public static S1UserException CheckServerStatus = new S1UserException(UserErrorTypes.CheckServerStatus);

        public UserErrorTypes ErrorType { get; protected set; }

        public S1UserException(UserErrorTypes type)
            : base(type.ToString())
        {
            ErrorType = type;
        }

        public S1UserException(string msg, UserErrorTypes type = UserErrorTypes.Unknown)
            : base(msg)
        {
            ErrorType = type;
        }
    }
}
