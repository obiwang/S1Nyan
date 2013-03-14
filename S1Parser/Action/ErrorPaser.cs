using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S1Parser.User
{
    public enum UserErrorTypes
    {
        Unknown = -1,
        Success = 0,
        LoginFailed = 1,
        NotLogin,
        InvalidVerify,
        MaxRetryTime,
    }    

    public static class ErrorPaser
    {
        public static UserErrorTypes Parse(HtmlElement root)
        {
            var content = root.FindFirst("div", (e) => e.Attributes["class"] == "tip-content");
            
            if (content == null)
                throw new NullReferenceException();
            else
            {
                int i = 0;
                string msg = null;
                foreach (var item in content.Descendants())
                {
                    if (i++ > 2) break;
                    msg = msg + item.PlainText();
                    if (i == 1) msg = msg + "\r\n";
                }
                return UserErrorTypes.NotLogin;

                //throw new UserException(msg, UserErrorTypes.NotLogin);
            }
            return UserErrorTypes.Unknown;
        }

    }
}
