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
        NotAuthorized,
        SiteClosed,
        ErrorWithMsg,
        InvalidVerify,
        NoServerAvailable,
        ServerUpdateSuccess,
        TryCheckingOtherServers,
        MaxRetryTime,
        ReplySuccess,
    }    

    public static class ErrorParser
    {
        public static UserErrorTypes Parse(HtmlElement root)
        {
            var content = root.FindFirst("div", (e) => e.Attributes["class"] == "tip-content");
            
            if (content == null)
                throw new NullReferenceException();
            else
            {
                var divs = content.Descendants("div");
                string msg = null;
                var errType = UserErrorTypes.ErrorWithMsg;
                int count = divs.Count();
                if (count == 1)
                {
                    var span = divs.First().Element();
                    if (span != null)
                        msg = span.InnerHtml;
                }
                else if (count >1)
                {
                    msg = divs.ElementAt(1).PlainText();
                }

                if (msg.Contains("您没有权限"))
                    errType = UserErrorTypes.NotAuthorized;
                else if (msg.Contains("网站已经关闭"))
                    errType = UserErrorTypes.SiteClosed;
                if (msg != null)
                    throw new S1UserException(msg, errType);
            }
            return UserErrorTypes.Unknown;
        }

        public static UserErrorTypes Parse(string html)
        {
            return Parse(new HtmlDoc(html).RootElement);
        }
    }
}
