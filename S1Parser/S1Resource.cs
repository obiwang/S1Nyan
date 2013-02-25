
using System.Text.RegularExpressions;
namespace S1Parser
{
    public static class S1Resource
    {
        public const string SiteBase = "http://bbs.saraba1st.com/2b/";

        public const string SimpleBase = SiteBase + "simple/";

        public const string EmotionBase = SiteBase + "images/post/smile/";

        public const short ItemsPerThreadSimple = 50;

        public const short ItemsPerThread = 30;

        /// <summary>
        /// Get the absolute url if not
        /// </summary>
        /// <param name="url"></param>
        /// <returns>true if original url is absolute</returns>
        public static bool GetAbsoluteUrl(ref string url)
        {
            if (!url.ToLower().StartsWith("http"))
            {
                url = SiteBase + url;
                return false;
            }
            return true;
        }

        public static bool IsEmotion(string url)
        {
            if (url.ToLower().StartsWith(EmotionBase))
                return true;
            return false;
        }

        public static string GetEmotionPath(string url)
        {
            if (IsEmotion(url))
            {
                return url.Substring(EmotionBase.Length);
            }
            return null;
        }

        //1 http://bbs.saraba1st.com/2b/read-htm-tid-785763-page-54.html  
        //2 http://bbs.saraba1st.com/2b/read.php?tid=785763&page=54#20636585
        static Regex p1 = new Regex(@"tid[-=](?<ID>\d+)");
        static Regex p2 = new Regex(@"page[-=](?<Page>\d+)");
        //3 http://bbs.saraba1st.com/2b/simple/?t785763_54.html
        static Regex p3 = new Regex(@"\?t(?<ID>\d+)(_(?<Page>\d+))?");

        //=> ?ID=785763&Page=54
        public static string GetThreadParamFromUrl(string url)
        {
            url = url.ToLower();
            string ID = null, Page = null;
            if (url.StartsWith(SimpleBase))
            {
                var match = p3.Match(url);
                if (match.Success)
                {
                    ID = match.Groups["ID"].Value;
                    Page = match.Groups["Page"].Value;
                    return GetThreadParams(ID, Page);
                }
            }
            else if (url.StartsWith(SiteBase))
            {
                var match = p1.Match(url);
                if (match.Success)
                    ID = match.Groups["ID"].Value;

                match = p2.Match(url);
                Page = match.Groups["Page"].Value;

                return GetThreadParams(ID, Page);
            }
            return null;
        }

        private static string GetThreadParams(string ID, string Page)
        {
            if (ID == null) return null;
            if (Page == "" || Page == null)
                return string.Format("?ID={0}", ID);
            else
            {
                int p = 1;
                int.TryParse(Page, out p);
                p = p * ItemsPerThread / ItemsPerThreadSimple;
                if (p < 1) p = 1;
                return string.Format("?ID={0}&Page={1}", ID, p.ToString());
            }
        }
    }
}
