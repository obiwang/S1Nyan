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

        public S1ThreadList GetData(string fid, int page)
        {
            var data = DZMyGroup.ThreadListFromJson(raw, fid);
            var list = new S1ThreadList();
            list.CurrentPage = data.CurrentPage != 0 ? data.CurrentPage : page;
            list.TotalPage = data.TotalPage;
            foreach (var thread in data.ThreadList)
            {
                var item = new S1ListItem
                    {
                        Id = thread.Id,
                        Title = S1Resource.HttpUtility.HtmlDecode(thread.Title),
                        Subtle = thread.Subtle,
                        Author = thread.Author,
                        AuthorDate = thread.AuthorDate,
                        LastPoster = thread.LastPoster,
                        LastPostDate = thread.LastPostDate
                    };

                list.Add(item);
            }
            return list;
        }

    }
}
