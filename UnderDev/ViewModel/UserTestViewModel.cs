using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Info;
using S1Nyan.Model;
using S1Parser.User;

namespace UnderDev.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class UserTestViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the Page1ViewModel class.
        /// </summary>
        public UserTestViewModel()
        {

        }

        private RelayCommand _loginCommand2;

        /// <summary>
        /// Gets the LoginCommand.
        /// </summary>
        public RelayCommand LoginCommand2
        {
            get
            {
                return _loginCommand2
                    ?? (_loginCommand2 = new RelayCommand(
                                          async () =>
                                          {
                                              await DoTestLogin2();
                                          }));
            }
        }

        private RelayCommand _loginCommand;

        /// <summary>
        /// Gets the LoginCommand.
        /// </summary>
        public RelayCommand LoginCommand
        {
            get
            {
                return _loginCommand
                    ?? (_loginCommand = new RelayCommand(
                                          async () =>
                                          {
                                              await DoTestLogin();
                                          }));
            }
        }

        const string testUser = "test2", testPass = "testtest";
        const string userKey = "pwuser";
        const string passKey = "pwpwd";
        const string stepKey = "step";    //hidden input
        const string cktimeKey = "cktime";//remember pass ...
        const int cktime = 31536000;      //for 1 year (in sec)
        const string loginTypeKey = "lgt";//0 - name ; 1 - uid; 2 - email
        const int loginType = 0;

        //const string loginUrl = "http://bbs.saraba1st.com/2b/login.php?";
        //const string userUrl = "http://bbs.saraba1st.com/2b/u.php";
        const string loginUrl = "http://192.168.0.113:8080/phpwind/login.php?";
        const string userUrl = "http://192.168.0.113:8080/phpwind/u.php";
        const string postFormatString = "http://192.168.0.113:8080/phpwind/post.php?fid=";
        const string signatureFormatString = "\r\n    [url=http://126.am/S1Nyan]—— from S1 Nyan ({0} {1})[/url]";

        public Action<string> OnUpdateView;

        S1WebClient client = null;
        private async void DoLogin()
        {
            client = new S1WebClient();

            client.AddPostParam(stepKey, 2);
            client.AddPostParam(loginTypeKey, loginType);
            client.AddPostParam(userKey, testUser);
            client.AddPostParam(passKey, testPass);
            client.AddPostParam(cktimeKey, cktime);
            Result = await client.PostDataTaskAsync(new Uri(loginUrl));
            foreach (Cookie c in client.Cookies)
                if (c.Name.Contains("uid")) Uid = c.Value;

            if (OnUpdateView != null) OnUpdateView(Result);
        }

        private async Task DoTestLogin()
        {
            try
            {
                Result = await UserLogin();
                //verify = await new S1WebClient().GetVerifyString();
            }
            catch (Exception ex)
            {
                Result = ex.Message;
            }
            finally
            {
                if (OnUpdateView != null) OnUpdateView(Result);
            }
        }

        private async Task<string> UserLogin()
        {
            return await new S1WebClient().Login(testUser, testPass);
        }

        private async Task DoTestLogin2()
        {
            try
            {
                Result = await new S1WebClient().Login("test1", testPass);
                //verify = await new S1WebClient().GetVerifyString();
            }
            catch (Exception ex)
            {
                Result = ex.Message;
            }
            finally
            {
                if (OnUpdateView != null) OnUpdateView(Result);
            }
        }

        private RelayCommand _testCommand;

        /// <summary>
        /// Gets the LogoutCommand.
        /// </summary>
        public RelayCommand TestCommand
        {
            get
            {
                return _testCommand
                    ?? (_testCommand = new RelayCommand(
                                          () =>
                                          {
                                              TestReply();
                                          }));
            }
        }


        //to get verify, in page "profile.php?action=privacy" look for <input type="hidden" name="verify" value="9e5242f7">

        private string verify = null;

        Regex resultPattern = new Regex(@"CDATA\[(?<data>.+)\]");
    
        static string deviceInfo = string.Format(signatureFormatString, DeviceStatus.DeviceManufacturer, DeviceStatus.DeviceName);

        private bool isSending = false;
        private bool IsSending
        {
            get { return isSending; }
            set
            {
                isSending = value; SendCommand.RaiseCanExecuteChanged();
            }
        }
        string replyLink = "post.php?action=reply&fid=2&tid=1";
        private async Task DoPost()
        {
            UserErrorTypes result = UserErrorTypes.Unknown;
            int retryTimes = 0;
            try
            {
                IsSending = true;
                while (result != UserErrorTypes.Success)
                {
                    if (retryTimes > 2)
                        throw new S1UserException("MaxRetry", UserErrorTypes.MaxRetryTime);
                    if (result == UserErrorTypes.InvalidVerify)
                    {
                        await UserLogin();
                    }

                    await CheckVerify();
                    result = await new S1WebClient().Reply(verify,
                        reletivePostUrl: replyLink,
                        content: ReplyText + "\r\n" + DateTime.Now.ToShortTimeString(),
                        signature: deviceInfo);

                    if (result == UserErrorTypes.Success)
                        Result = "Success";
                    retryTimes++;
                }
            }
            catch (Exception ex)
            {
                Result = ex.Message;
            }
            finally
            {
                IsSending = false;
            }
        }

        private async Task CheckVerify()
        {
            if (verify == null || verify.Length == 0)
            {
                verify = await new S1WebClient().GetVerifyString();
            }
        }

        private async void TestReply()
        {
            await DoPost();
        }

        private async void TestWrongVerify()
        {
            verify = "12345";
            await DoPost();
        }

        private async void TestWrongID()
        {
            replyLink = "post.php?action=reply&fid=2&tid=2";
            await DoPost();
        }

        private async void DoTest()
        {
            client = new S1WebClient();
            int fid = 2;
            //client.Headers[HttpRequestHeader.Referer] = "http://192.168.0.113:8080/phpwind/read.php?tid=1";
            //client.AddPostParam("replytouser", "asdf");
            //client.AddPostParam("cyid", "1");
            //client.AddPostParam("iscontinue", "0");
            //client.AddPostParam("atc_desc1", "");
            //client.AddPostParam("_hexie", "36c309f2");
            client.AddPostParam("atc_title", "12345");
            client.AddPostParam("verify", verify);
            client.AddPostParam("atc_usesign", "1");
            client.AddPostParam("atc_convert", "1");
            client.AddPostParam("atc_autourl", "1");
            client.AddPostParam("stylepath", "wind");

            client.AddPostParam(stepKey, 2);
            client.AddPostParam("action", "reply");
            client.AddPostParam("fid", fid);
            client.AddPostParam("tid", "1");
            client.AddPostParam("ajax", "1");

            client.AddPostParam("atc_content", "[s:13] test from client @" + DateTime.Now.ToShortTimeString() + deviceInfo);

            Result = await client.PostDataTaskAsync(new Uri(postFormatString + fid));
            //Result = await client.PostMultipartTaskAsync(new Uri(postUrl));
            string error = "";
            var match =resultPattern.Match(Result);
            if (match.Success)
                error = match.Groups["data"].Value;
            if (error.Length < 200)
                Debug.WriteLine(error);

            if (OnUpdateView != null) OnUpdateView(Result + ConvertExtendedASCII(error));
        }

        public static string ConvertExtendedASCII(string HTML)
        {
            StringBuilder str = new StringBuilder();
            char c;
            for (int i = 0; i < HTML.Length; i++)
            {
                c = HTML[i];
                if (Convert.ToInt32(c) > 127)
                {
                    str.Append("&#" + Convert.ToInt32(c) + ";");
                }
                else
                {
                    str.Append(c);
                }
            }
            return str.ToString();
        }

        private string _uid = "";
        /// <summary>
        /// Sets and gets the Uid property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Uid
        {
            get { return _uid; }

            set
            {
                if (_uid == value) return;

                _uid = value;
                RaisePropertyChanged(() => Uid);
            }
        }

        private string _result = "";

        /// <summary>
        /// Sets and gets the Result property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Result
        {
            get { return _result; }

            set
            {
                if (_result == value) return;

                _result = value;
                RaisePropertyChanged(() => Result);
            }
        }

        private string _replyText = "";

        /// <summary>
        /// Sets and gets the ReplyText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ReplyText
        {
            get { return _replyText; }

            set
            {
                if (_replyText == value) return;

                _replyText = value;
                RaisePropertyChanged(() => ReplyText);
                SendCommand.RaiseCanExecuteChanged();
            }
        }

        private RelayCommand _sendCommand;

        /// <summary>
        /// Gets the SendCommand.
        /// </summary>
        public RelayCommand SendCommand
        {
            get
            {
                return _sendCommand
                    ?? (_sendCommand = new RelayCommand(
                        () => TestReply(), 
                        () => ReplyText.Length > 0 && !IsSending));
            }
        }
    }
}