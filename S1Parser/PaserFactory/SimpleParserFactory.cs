using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using S1Parser.SimpleParser;

namespace S1Parser.PaserFactory
{
    public class SimpleParserFactory : IParserFactory
    {
        public IResourceService ResourceService { get; set; }
        internal const short ItemsPerThreadSimple = 50;
        internal const short ItemsPerThread = 30;
        internal const string SimplePath = "simple/";

        public string Path
        {
            get { return SimplePath; }
        }

        public string GetThreadOriginalUrl(string tid)
        {
            throw new NotImplementedException();
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
            return new SimpleThreadListParser(s).GetData();
        }

        public async Task<S1Post> GetPostData(string tid, int page)
        {
            Stream s = await ResourceService.GetResourceStream(GetThreadUri(tid, page));
            return new SimpleThreadParser(s).GetData();
        }

        protected virtual Uri GetMainUri()
        {
            return new Uri(S1Resource.ForumBase);
        }

        protected virtual Uri GetThreadListUri(string fid, int page)
        {
            return new Uri(S1Resource.ForumBase + String.Format("?f{0}{1}.html", fid, GetPageParam(page)));
        }

        private static string GetPageParam(int page)
        {
            return page > 1 ? String.Format("_{0}", page) : "";
        }

        protected virtual Uri GetThreadUri(string tid, int page)
        {
            return new Uri(S1Resource.ForumBase + String.Format("?t{0}{1}.html", tid, GetPageParam(page)));
        }
    }
}
