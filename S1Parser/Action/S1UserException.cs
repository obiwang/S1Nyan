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
        public UserErrorTypes ErrorType { get; protected set; }

        public S1UserException(string msg, UserErrorTypes type = UserErrorTypes.Unknown)
            : base(msg)
        {
            ErrorType = type;
        }
    }
}
