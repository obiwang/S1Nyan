using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Browser;
using System.Text;
using System.Threading.Tasks;
using Coding4Fun.Toolkit.Net;
using S1Parser;

namespace S1Nyan.Model
{
    public class S1WebClient : GzipWebClient, IS1Client
    {
        private static string UserAgent = string.Format("Mozilla/5.0 (compatible; S1Nyan 1.x; Windows Phone {0}; )", Environment.OSVersion.Version);

        public CookieCollection Cookies { get; private set; }
        private static CookieContainer _cookieContainer;

        Dictionary<string, object> _dataCollection = null;

        static S1WebClient ()
        {
            ResetCookie();
        }

        public static void ResetCookie()
        {
            _cookieContainer = new CookieContainer();
        }

        public S1WebClient()
        {
            _dataCollection = new Dictionary<string, object>();
        }

        public Task<string> PostDataTaskAsync(Uri address)
        {
            Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            Headers[HttpRequestHeader.Referer] = address.Host; //Attention Discuz server will verify this
            string postData = BuildPostData();
            return this.UploadStringTaskAsync(address, postData);
        }


        protected override WebRequest GetWebRequest(Uri address)
        {
            bool httpResult = HttpWebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);

            var webRequest =(HttpWebRequest)base.GetWebRequest(address);

            webRequest.CookieContainer = _cookieContainer;
            webRequest.Headers[HttpRequestHeader.UserAgent] = UserAgent;
            return webRequest;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            var webResponse = (HttpWebResponse)base.GetWebResponse(request, result);
            if (webResponse != null)
                Cookies = webResponse.Cookies;
            return webResponse;
        }

        public void AddPostParam(string key, object value)
        {
            if (_dataCollection == null)
                _dataCollection = new Dictionary<string, object>();
            _dataCollection[key] = value;
        }

        private string BuildPostData()
        {
            StringBuilder result = new StringBuilder();
            foreach (var pair in _dataCollection)
            {
                result.Append(string.Format("{0}={1}&", Uri.EscapeDataString(pair.Key), Uri.EscapeDataString(pair.Value.ToString())));
            }
            return result.ToString();
        }

        #region multipart form-data Post, not used for now

        private const string formBoundary = "--S1NyanFormBoundary";
        private const string realBoundary = "----S1NyanFormBoundary";

        public Task<string> PostMultipartTaskAsync(Uri address)
        {
            Headers[HttpRequestHeader.ContentType] = "multipart/form-data; boundary=" + formBoundary;

            string postData = BuildMultipartData();
            return this.UploadStringTaskAsync(address, postData);
        }

        private string BuildMultipartData()
        {
            StringBuilder result = new StringBuilder();
            foreach (var pair in _dataCollection)
            {
                result.Append(realBoundary);
                result.Append(string.Format("\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n", pair.Key));
                result.Append(pair.Value.ToString());
                result.Append("\r\n");
            }
            result.Append(realBoundary);
            return result.ToString();
        }
        #endregion

    }

}
