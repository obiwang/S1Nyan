using System;
using System.Net;
using System.Threading.Tasks;

namespace S1Parser.Action
{
    public static class UserAction
    {
        const string userKey = "pwuser";
        const string passKey = "pwpwd";
        const string stepKey = "step";    //hidden input
        const string stepValue = "2";
        const string loginTypeKey = "lgt";//0 - name ; 1 - uid; 2 - email
        //const string cktimeKey = "cktime";//remember pass ...
        //const int cktime = 31536000;      //for 1 year (in sec)

#if UseLocalhost
        const string SiteBase = "http://192.168.0.113:8080/phpwind/";
        const string loginUrl = SiteBase + "login.php?";
        public const string PrivacyUrl = SiteBase + "profile.php?action=privacy";
#else
        const string SiteBase = S1Resource.SiteBase;
        const string loginUrl = SiteBase + "login.php?";
        public const string PrivacyUrl = SiteBase + "profile.php?action=privacy";
#endif

        public static async Task<string> Login(this IS1Client client, string account, string pass, int loginType = 0)
        {
            string uid = null;
            client.AddPostParam(stepKey, stepValue);
            client.AddPostParam(loginTypeKey, loginType);
            client.AddPostParam(userKey, account);
            client.AddPostParam(passKey, pass);
            //client.AddPostParam(cktimeKey, cktime);
            var result = await client.PostDataTaskAsync(new Uri(loginUrl));

            foreach (Cookie c in client.Cookies)
                if (c.Name.Contains("uid")) uid = c.Value;

            if (uid == null)
            {   // handle error
                
                System.Diagnostics.Debug.WriteLine(result);
            }
            return uid;
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
        public static async Task<string> Reply(this IS1Client client, string verify, string reletivePostUrl, string content, string signature = "", string title = "")
        {
            AddConstParam(client);

            client.AddPostParam("verify", verify);
            client.AddPostParam("atc_content", content + signature);
            client.AddPostParam("atc_title", title);

            var result = await client.PostDataTaskAsync(new Uri(SiteBase + reletivePostUrl));
            if (result != null)
            {   // handle error
                //<?xml version="1.0" encoding="utf-8"?><ajax><![CDATA[success	read.php?tid=1&page=e#a]]></ajax>
                //<?xml version="1.0" encoding="utf-8"?><ajax><![CDATA[非法请求，请返回重试!]]></ajax>
                System.Diagnostics.Debug.WriteLine(result);
            }
            return result;
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
