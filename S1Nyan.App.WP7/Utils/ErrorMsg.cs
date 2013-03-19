using System;
using System.Net;
using Microsoft.Phone.Net.NetworkInformation;
using S1Nyan.Resources;
using S1Parser.User;

namespace S1Nyan.Utils
{
    public class ErrorMsg : IErrorMsg 
    {
        public string GetExceptionMessage(Exception e)
        {
            if (e is WebException)
            {
                if (!IsNetworkAvailable())
                    return AppResources.ErrorMsgNoNetwork;
                return AppResources.ErrorMsgConnectServerFailed;
            }
            else if (e is S1UserException)
            {
                string msg = AppResources.ErrorMsgUnknown;
                switch((e as S1UserException).ErrorType)
                {
                    case UserErrorTypes.SiteClosed:
                        msg = AppResources.ErrorMsgSiteClosed;
                        break;
                    case UserErrorTypes.NoServerAvailable:
                        msg = AppResources.ErrorMsgNoServerAvailable;
                        break;
                    case UserErrorTypes.ServerUpdateSuccess:
                        msg = AppResources.ErrorMsgServerUpdateSuccess;
                        break;
                    case UserErrorTypes.TryCheckingOtherServers:
                        msg = AppResources.ErrorMsgTryCheckingOtherServers;
                        break;
                    case UserErrorTypes.ReplySuccess:
                        msg = AppResources.ErrorMsgReplySuccess;
                        break;
                    default:
                        if (e.Message != null)
                            msg = e.Message;
                        break;
                }
                return msg;
            }
            else if (e is NullReferenceException)
            {
                return AppResources.ErrorMsgGetDataFailed;
            }
            else
                return AppResources.ErrorMsgUnknown + e.Message;
        }

        public bool IsNetworkAvailable()
        {
            return DeviceNetworkInformation.IsNetworkAvailable;
        }
    }
}
