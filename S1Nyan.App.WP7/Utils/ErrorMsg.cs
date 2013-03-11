using System;
using System.Net;
using S1Nyan.App.Resources;
using S1Parser.Action;

namespace S1Nyan.Utils
{
    public static class ErrorMsg 
    {
        public static string GetExceptionMessage(Exception e)
        {
            if (e is WebException)
            {
                return AppResources.ErrorMsgNetWorkFailed;
            }
            else if (e is LoginException)
            {
                return e.Message;
            }
            else if (e is NullReferenceException)
            {
                return AppResources.ErrorMsgGetDataFailed;
            }
            else
                return AppResources.ErrorMsgUnknown + e.Message;
        }
    }
}
