using System.Collections.Generic;
using System.IO;
using System.Net;
using S1Parser.PaserFactory;

namespace S1Parser.DZParser
{
    public class DZThreadParser
    {
        private string raw;

        public DZThreadParser() { }
        public DZThreadParser(Stream s)
        {
            using (var reader = new StreamReader(s))
            {
                raw = reader.ReadToEnd();
            }
        }

        public DZThreadParser(string s)
        {
            raw = s;
        }

        public S1ThreadPage GetData()
        {
            var data = DZThread.FromJson(raw).Variables;
            var thread = new S1ThreadPage();
            thread.Title = WebUtility.HtmlDecode(data.Thread.Subject);
            thread.TotalPage = (data.Thread.Maxposition + DZParserFactory.PostsPerPage)/DZParserFactory.PostsPerPage;
            thread.Items = new List<S1ThreadItem>();
            foreach (var post in data.Postlist)
            {
                var item = new S1ThreadItem();
                if (thread.CurrentPage == 0)
                    thread.CurrentPage = post.Number/DZParserFactory.PostsPerPage + 1;
                item.No = post.Number;
                item.Author = WebUtility.HtmlDecode(post.Author);
                item.Date = WebUtility.HtmlDecode(post.Dateline);
                //work around
                post.Message = post.Message.Replace("\n", "");
                var content = new HtmlDoc(string.Format("<div>{0}</div>", WebUtility.HtmlDecode(post.Message))).RootElement;

                if (content != null)
                    item.Content = SimpleParser.SimpleThreadParser.ReGroupContent(content); 
                thread.Items.Add(item);
            }
            return thread;
        }

    }
}
