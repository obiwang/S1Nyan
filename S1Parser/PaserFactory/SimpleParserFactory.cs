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
            Stream s = await ResourceService.GetResourceStream(GetMainUri(), "MainList");
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

        protected virtual Uri GetMainUri()
        {
            return new Uri(S1Resource.SimpleBase);
        }

        protected virtual Uri GetThreadListUri(string fid, int page)
        {
            return new Uri(S1Resource.SimpleBase + string.Format("?f{0}{1}.html", fid, GetPageParam(page)));
        }

        private static string GetPageParam(int page)
        {
            return page > 1 ? string.Format("_{0}", page) : "";
        }

        protected virtual Uri GetThreadUri(string tid, int page)
        {
            return new Uri(S1Resource.SimpleBase + string.Format("?t{0}{1}.html", tid, GetPageParam(page)));
        }
    }
}
