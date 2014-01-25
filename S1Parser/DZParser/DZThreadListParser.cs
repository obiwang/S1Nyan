using System.IO;
using System.Linq;

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
            var list = new S1ThreadList
            {
                CurrentPage = data.CurrentPage != 0 ? data.CurrentPage : page,
                TotalPage = data.TotalPage
            };
            if (data.ThreadList != null)
                FillList(data, list);

            return list;
        }

        private static void FillList(IThreadList data, S1ThreadList list)
        {
            list.AddRange(
                data.ThreadList.Select(thread => new S1ListItem
                {
                    Id = thread.Id,
                    Title = S1Resource.HttpUtility.HtmlDecode(thread.Title),
                    Subtle = S1Resource.HttpUtility.HtmlDecode(thread.Subtle),
                    Author = S1Resource.HttpUtility.HtmlDecode(thread.Author),
                    AuthorDate = thread.AuthorDate,
                    LastPoster = S1Resource.HttpUtility.HtmlDecode(thread.LastPoster),
                    LastPostDate = thread.LastPostDate
                }));
        }
    }
}
