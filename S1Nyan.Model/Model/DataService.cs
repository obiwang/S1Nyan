using System;
using System.Collections.Generic;
using System.IO;
using S1Parser;
using S1Parser.ListParser;
using S1Parser.ThreadParser;

namespace S1Nyan.Model
{
    public class EmptyResourceService : IResourceService
    {
        public void GetResourceStream(Uri uri, Action<Stream, Exception> callback)
        {
            callback(null, null);
        }
    }

    public class DataService : IDataService
    {
        List<S1ListItem> mainListData;
        private IResourceService resourceService;
        public IResourceService ResourceService
        {
            get { return resourceService ?? new EmptyResourceService(); }
            set { resourceService = value; }
        }

        public void GetMainListData(Action<List<S1ListItem>, Exception> callback)
        {
            if (mainListData == null)
                RefreshMainListData(callback);
            else
                callback(mainListData, null);
        }

        public void RefreshMainListData(Action<List<S1ListItem>, Exception> callback)
        {
#if !DEBUG
            var uri = new Uri("http://bbs.saraba1st.com/2b/simple/");
#else
            var uri = new Uri("FakeData/simple.htm", UriKind.Relative);
#endif
            ResourceService.GetResourceStream(uri, (s, error) =>
            {
                if (error == null)
                {
                    var parser = new SimpleListParser { HtmlPage = new HtmlDoc(s).RootElement };
                    mainListData = parser.GetData();
                }
                callback(mainListData, error);
            });
        }

        public void GetThreadListData(string fid, int page, Action<S1ThreadList, Exception> callback)
        {
#if !DEBUG
            var uri = new Uri("http://bbs.saraba1st.com/2b/simple/" + string.Format("?f{0}{1}.html", fid, GetPageParam(page)));
#else
            var uri = new Uri("FakeData/simple_thread.htm", UriKind.Relative);
#endif
            ResourceService.GetResourceStream(uri, (s, error) =>
            {
                S1ThreadList data = null;

                if (error == null)
                {
                    var parser = new SimpleThreadListParser { HtmlPage = new HtmlDoc(s).RootElement };
                    data = parser.GetData();
                }
                callback(data, error);
            });
        }

        public void GetThreadData(string tid, int page, Action<S1ThreadPage, Exception> callback)
        {
#if !DEBUG
            var uri = new Uri("http://bbs.saraba1st.com/2b/simple/" + string.Format("?t{0}{1}.html", tid, GetPageParam(page)));
#else
            var uri = new Uri("FakeData/simple_read.htm", UriKind.Relative);
#endif
            ResourceService.GetResourceStream(uri, (s, error) =>
            {
                S1ThreadPage data = null;

                if (error == null)
                {
                    var parser = new SimpleThreadParser { HtmlPage = new HtmlDoc(s).RootElement };
                    data = parser.GetData();
                }
                callback(data, error);
            });
        }

        private static string GetPageParam(int page)
        {
            return page > 1 ? string.Format("_{0}", page) : "";
        }
    }
}