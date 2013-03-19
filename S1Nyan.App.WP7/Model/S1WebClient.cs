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
        private const string UserAgent = "Mozilla/5.0 (compatible; S1Nyan 1.x; Windows Phone OS 7.5; )";

        public CookieCollection Cookies { get; private set; }
        private static CookieContainer CookieContainer = new CookieContainer();

        Dictionary<string, object> dataCollection = null;

        public S1WebClient()
        {
            dataCollection = new Dictionary<string, object>();
        }

        public Task<string> PostDataTaskAsync(Uri address)
        {
            Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

            string postData = BuildPostData();
            return this.UploadStringTaskAsync(address, postData);
        }


        protected override WebRequest GetWebRequest(Uri address)
        {
            bool httpResult = HttpWebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);

            var webRequest =(HttpWebRequest)base.GetWebRequest(address);

            webRequest.CookieContainer = CookieContainer;
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
            if (dataCollection == null)
                dataCollection = new Dictionary<string, object>();
            dataCollection[key] = value;
        }

        private string BuildPostData()
        {
            StringBuilder result = new StringBuilder();
            foreach (var pair in dataCollection)
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
            foreach (var pair in dataCollection)
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
