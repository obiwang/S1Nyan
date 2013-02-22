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

        public async Task<IList<S1ListItem>> GetMainListData()
        {
            Stream s = await ResourceService.GetResourceStream(GetMainUri());
            return new SimpleListParser(s).GetData();
        }

        public async Task<S1ThreadList> GetThreadListData(string fid, int page)
        {
            Stream s = await ResourceService.GetResourceStream(GetThreadListUri(fid, page));
            return new SimpleThreadListParser(s).GetData();
        }

        public async Task<S1ThreadPage> GetThreadData(string tid, int page)
        {
            Stream s = await ResourceService.GetResourceStream(GetThreadUri(tid, page));
            return new SimpleThreadParser(s).GetData();
        }

        private Uri GetMainUri()
        {
#if !DEBUG
            return new Uri(S1Resource.SimpleBase);
#else
            return new Uri("FakeData/simple.htm", UriKind.Relative);
#endif
        }

        private Uri GetThreadListUri(string fid, int page)
        {
#if !DEBUG
            var uri = new Uri(S1Resource.SimpleBase + string.Format("?f{0}{1}.html", fid, GetPageParam(page)));
#else
            var uri = new Uri("FakeData/simple_thread.htm", UriKind.Relative);
#endif
            return uri;
        }

        private static string GetPageParam(int page)
        {
            return page > 1 ? string.Format("_{0}", page) : "";
        }


        private Uri GetThreadUri(string tid, int page)
        {
#if !DEBUG
            var uri = new Uri(S1Resource.SimpleBase + string.Format("?t{0}{1}.html", tid, GetPageParam(page)));
#else
            var uri = new Uri("FakeData/simple_read.htm", UriKind.Relative);
#endif
            return uri;
        }
    }
}
