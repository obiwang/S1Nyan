
namespace S1Parser
{
    public static class S1Resource
    {
        public const string SiteBase = "http://bbs.saraba1st.com/2b/";

        public const string SimpleBase = "http://bbs.saraba1st.com/2b/simple/";

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
    }
}
