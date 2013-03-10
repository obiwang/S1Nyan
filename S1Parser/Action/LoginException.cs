using System;

namespace S1Parser.Action
{
    public class LoginException : Exception
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public LoginException(string msg, string account, string pass): base(msg)
        {
            Username = account;
            Password = pass;
        }

    }
}
