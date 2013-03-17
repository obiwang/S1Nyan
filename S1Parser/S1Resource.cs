﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace S1Parser
{
    public static class S1Resource
    {
        private const string EmotionPath = "images/post/smile/";
        internal const string SimplePath = "simple/";

        private static string siteBase;
        public static string SiteBase
        {
            get
            {
                return siteBase;
            }
            set
            {
                if (value == null || value.Length == 0) return;
                siteBase = value;
                simpleBase = null;
                emotionBase = null;
            }
        }

        public static List<string> HostList { get; set; }

        private static string simpleBase;
        internal static string SimpleBase
        {
            get
            {
                return simpleBase ?? (simpleBase = SiteBase + SimplePath);
            }
        }
        private static string emotionBase;
        internal static string EmotionBase { get { return emotionBase ?? (emotionBase = SiteBase + EmotionPath); } } 

        public const short ItemsPerThreadSimple = 50;
        public const short ItemsPerThread = 30;

        public static string GetRelativePath(string url)
        {
            var temp = url.ToLower();
            foreach (var host in HostList)
            {
                if (temp.StartsWith(host))
                {
                    return url.Substring(host.Length);
                }
            }
            return null;
        }

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
            var temp = GetRelativePath(url);
            if (temp != null && temp.StartsWith(EmotionPath))
                return true;
            return false;
        }

        public static string GetEmotionPath(string url)
        {
            var temp = GetRelativePath(url);
            if (temp != null && temp.StartsWith(EmotionPath))
            {
                return temp.Substring(EmotionPath.Length);
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
            url = GetRelativePath(url);
            if (url == null) return null;
            string ID = null, Page = null;
            if (url.StartsWith(SimplePath))
            {
                var match = p3.Match(url);
                if (match.Success)
                {
                    ID = match.Groups["ID"].Value;
                    Page = match.Groups["Page"].Value;
                    return GetThreadParams(ID, Page);
                }
            }
            else
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
