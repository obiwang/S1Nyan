using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using S1Parser.PaserFactory;
using S1Parser.User;

namespace S1Parser.DZParser
{
    public class DZPostParser
    {
        private string raw;

        public DZPostParser() { }
        public DZPostParser(Stream s)
        {
            using (var reader = new StreamReader(s))
            {
                raw = reader.ReadToEnd();
            }
        }

        public DZPostParser(string s)
        {
            raw = s;
        }

        public S1Post GetData()
        {
            var json = DZPost.FromJson(raw);
            var data = json.Variables;
            if (data.Postlist.Length == 0 && json.Message != null)
                throw new S1UserException(json.Message.Messagestr);

            var thread = new S1Post();
            thread.Title = S1Resource.HttpUtility.HtmlDecode(data.Thread.Subject);
            thread.TotalPage = (data.Thread.Replies + DZParserFactory.PostsPerPage)/DZParserFactory.PostsPerPage;
            thread.Items = new List<S1PostItem>();
            thread.Hash = data.Formhash;
            thread.FullLink = S1Resource.SiteBase + string.Format("thread-{1}-{0}-1.html", data.Thread.Fid, data.Thread.Tid);
            thread.ReplyLink = string.Format("?module=sendreply&replysubmit=yes&fid={0}&tid={1}", data.Thread.Fid, data.Thread.Tid);

            foreach (var post in data.Postlist)
            {
                var item = new S1PostItem();
                if (thread.CurrentPage == 0)
                    thread.CurrentPage = post.Number/DZParserFactory.PostsPerPage + 1;
                item.No = post.Number;
                item.Author = S1Resource.HttpUtility.HtmlDecode(post.Author);
                item.Date = S1Resource.HttpUtility.HtmlDecode(post.Dateline);

                //work around
                post.Message = post.Message.Replace("<imgwidth=", "<img width=");
                post.Message = post.Message.Replace("\n", "");
                FillAttachment(post);

                var content = new HtmlDoc(string.Format("<div>{0}</div>", S1Resource.HttpUtility.HtmlDecode(post.Message))).RootElement;

                if (content != null)
                    item.AddRange(SimpleParser.SimpleThreadParser.ReGroupContent(content)); 
                thread.Items.Add(item);
            }
            return thread;
        }

        private static readonly Regex AttachPattern = new Regex(@"\[attach](?<id>\d+)\[/attach]", RegexOptions.IgnoreCase);
        private void FillAttachment(PostItem post)
        {
            post.Message = AttachPattern.Replace(post.Message, match =>
            {
                var attachNo = match.Groups["id"].Value;
                if (!string.IsNullOrEmpty(attachNo) && 
                    post.Attachments != null && 
                    post.Attachments.ContainsKey(attachNo))
                {
                    var a = post.Attachments[attachNo];
                    return string.Format("<img src=\"{0}\"></img>", a.url + a.attachment);
                }
                return "";
            });
        }

    }
}
