using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;

namespace S1Parser.User
{
    public static class UserAction
    {
        const string userKey = "username";
        const string passKey = "password";
        const string stepKey = "loginsubmit";    //hidden input
        const string stepValue = "yes";
        const string loginTypeKey = "lgt";//0 - name ; 1 - uid; 2 - email

        private const string loginSucceed = "login_succeed";
        private const string loginSucceedMobile = "location_login_succeed_mobile";
        private const string logoutSucceed = "logout_succeed";
        private const string logoutSucceedMobile = "location_logout_succeed_mobile";
        //const string cktimeKey = "cktime";//remember pass ...
        //const int cktime = 31536000;      //for 1 year (in sec)

#if UseLocalhost
        const string SiteBase = "http://192.168.0.60/phpwind/";
#else
        static string SiteBase { get { return S1Resource.DZMobileBase; } }
#endif
        static string loginUrl { get { return SiteBase + "?module=login"; } }
        static string logoutUrl { get { return SiteBase + "?module=login&action=logout"; } }
        public static string PrivacyUrl { get { return SiteBase + "profile.php?action=privacy"; } }

        public static async Task<UserVariables> Login(this IS1Client client, string account, string pass, int loginType = 0)
        {
            client.AddPostParam(stepKey, stepValue);
            //client.AddPostParam(loginTypeKey, loginType);
            client.AddPostParam(userKey, account);
            client.AddPostParam(passKey, pass);
            //client.AddPostParam(cktimeKey, cktime);
            var result = await client.PostDataTaskAsync(new Uri(loginUrl));

            var user = DZUser.FromJson(result);

            if (user.Message.Messageval != loginSucceed &&
                user.Message.Messageval != loginSucceedMobile)
            {
                throw new LoginException(user.Message.Messagestr, account, pass);
            }
            return user.Variables;
        }

        public static async Task<bool> Logout(this IS1Client client, string formhash)
        {
            var result = await client.PostDataTaskAsync(new Uri(string.Format("{0}&formhash={1}", logoutUrl, formhash)));
            var user = DZUser.FromJson(result);
            return (user.Message.Messageval == loginSucceed || user.Message.Messageval == loginSucceedMobile);
        }


        /// <summary>
        /// send a post
        /// </summary>
        /// <param name="client">A <see cref="IS1Client"/> WebClient</param>
        /// <param name="verify">verify string retreive from <see cref="UserExtension.GetVerifyString"/></param>
        /// <param name="reletivePostUrl">something like post.php?action=reply&amp;fid=75&amp;tid=900385</param>
        /// <param name="content">post content</param>
        /// <param name="signature"></param>
        /// <param name="title">post title</param>
        /// <returns></returns>
        public static async Task<UserErrorTypes> Reply(this IS1Client client, string verify, string reletivePostUrl, string content, string signature = "", string title = "")
        {
            AddConstParam(client);

            client.AddPostParam("verify", verify);
            client.AddPostParam("atc_content", content + signature);
            client.AddPostParam("atc_title", title);

            var result = await client.PostDataTaskAsync(new Uri(SiteBase + reletivePostUrl));
            try
            {
                var root = XDocument.Parse(result).Root;
                string error = root.Value.ToLower();
                System.Diagnostics.Debug.WriteLine(error);

                if (error.StartsWith("success"))
                    return UserErrorTypes.Success;
                else if (error.Contains("非法请求"))
                    return UserErrorTypes.InvalidVerify;
                else
                    throw new S1UserException(error, UserErrorTypes.Unknown);
            }
            catch (System.Xml.XmlException)
            {
                System.Diagnostics.Debug.WriteLine(result);
                throw new NullReferenceException();
            }
        }

        private static void AddConstParam(IS1Client client)
        {
            client.AddPostParam("atc_usesign", "1");
            client.AddPostParam("atc_convert", "1");
            client.AddPostParam("atc_autourl", "1");
            client.AddPostParam("stylepath", "wind");
            client.AddPostParam("ajax", "1");
            client.AddPostParam("action", "reply");
            client.AddPostParam(stepKey, stepValue);
        }
    }
}
