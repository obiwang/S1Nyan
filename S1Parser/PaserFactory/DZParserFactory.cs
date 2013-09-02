using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using S1Parser.DZParser;
using S1Parser.SimpleParser;

namespace S1Parser.PaserFactory
{
    public class DZParserFactory : IParserFactory
    {
        internal const int ThreadsPerPage = 50;
        internal const int PostsPerPage = 30;

        public IResourceService ResourceService { get; set; }
        internal const string DZMobilePath = "api/mobile/";

        public string Path
        {
            get { return DZMobilePath; }
        }

        public async Task<IList<S1ListItem>> GetMainListData()
        {
            Stream s = await GetMainListStream();
            return new SimpleListParser(s).GetData();
        }

        public async Task<Stream> GetMainListStream()
        {
            Stream s = await ResourceService.GetResourceStream(GetMainUri());
            return s;
        }

        public IList<S1ListItem> ParseMainListData(Stream s)
        {
            return new SimpleListParser(s).GetData();
        }

        public IList<S1ListItem> ParseMainListData(string s)
        {
            return new SimpleListParser(s).GetData();
        }

        public async Task<S1ThreadList> GetThreadListData(string fid, int page)
        {
            Stream s = await ResourceService.GetResourceStream(GetThreadListUri(fid, page));
            return new DZThreadListParser(s).GetData();
        }

        public async Task<S1ThreadPage> GetThreadData(string tid, int page)
        {
            Stream s = await ResourceService.GetResourceStream(GetThreadUri(tid, page));
            return new DZThreadParser(s).GetData();
        }

        protected virtual Uri GetMainUri()
        {
            return new Uri(S1Resource.ForumBase);
        }

        protected virtual Uri GetThreadListUri(string fid, int page)
        {
#if UseLocalhost
            return new Uri(S1Resource.ForumBase + string.Format("?module=forumdisplay&fid=2"));
#else
            return new Uri(S1Resource.ForumBase + String.Format("?module=forumdisplay&fid={0}&page={1}&tpp={2}", fid, page, ThreadsPerPage));
#endif
        }

        protected virtual Uri GetThreadUri(string tid, int page)
        {
#if UseLocalhost
            return new Uri(S1Resource.ForumBase + string.Format("?module=viewthread&tid=1&ppp=50"));
#else
            return new Uri(S1Resource.ForumBase + string.Format("?module=viewthread&tid={0}&page={1}&ppp={2}", tid, page, PostsPerPage));
#endif
        }

        public static DateTime DateTimeSince1970Interval(int interval)
        {
            var the1970 = new DateTime(1970,1,1);
            return the1970.AddSeconds(interval).ToLocalTime();
        }
    }
}
