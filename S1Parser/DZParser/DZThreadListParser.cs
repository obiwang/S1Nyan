using System;
using System.IO;
using System.Net;
using S1Parser.PaserFactory;

namespace S1Parser.DZParser
{
    public class DZThreadListParser
    {
        private string raw;

        public DZThreadListParser() { }
        public DZThreadListParser(Stream s)
        {
            using (var reader = new StreamReader(s))
            {
                raw = reader.ReadToEnd();
            }
        }

        public DZThreadListParser(string s)
        {
            raw = s;
        }

        public S1ThreadList GetData()
        {
            var data = DZForum.FromJson(raw).Variables;
            var list = new S1ThreadList();
            list.CurrentPage = data.Page;
            list.TotalPage = data.Forum.Threads/DZParserFactory.ThreadsPerPage;
            foreach (var thread in data.Forum_threadlist)
            {
                var item = new S1ListItem
                    {
                        Id = thread.Tid,
                        Title = WebUtility.HtmlDecode(thread.Subject),
                        Subtle = thread.Replies,
                        Author = thread.Author,
                        AuthorDate = DZParserFactory.DateTimeSince1970Interval(thread.Dbdateline),
                        LastPoster = thread.Lastposter,
                        LastPostDate = DZParserFactory.DateTimeSince1970Interval(thread.Dblastpost)
                    };

                list.Children.Add(item);
            }
            return list;
        }

    }
}
