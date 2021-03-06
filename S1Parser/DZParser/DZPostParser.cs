﻿using S1Parser.PaserFactory;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

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
            var data = raw.Parse<ThreadVariables>();
            if (S1Resource.FormHashUpdater != null)
                S1Resource.FormHashUpdater.UpdateFormHash(data.Formhash);

            var thread = new S1Post();
            thread.Title = S1Resource.HttpUtility.HtmlDecode(data.Thread.Subject);
            thread.TotalPage = (data.Thread.Replies + DZParserFactory.PostsPerPage)/DZParserFactory.PostsPerPage;
            thread.Items = new List<S1PostItem>();
            thread.ReplyLink = string.Format("?module=sendreply&replysubmit=yes&fid={0}&tid={1}", data.Thread.Fid, data.Thread.Tid);

            foreach (var post in data.Postlist)
            {
                var item = new S1PostItem();
                if (thread.CurrentPage == 0)
                    thread.CurrentPage = post.Number/DZParserFactory.PostsPerPage + 1;
                item.No = post.Number;
                item.Author = S1Resource.HttpUtility.HtmlDecode(post.Author);
                item.Date = S1Resource.HttpUtility.HtmlDecode(post.Dateline);

                BuildContent(post, item);
                thread.Items.Add(item);
            }
            return thread;
        }

        private void BuildContent(PostItem post, S1PostItem item)
        {
            post.Message = post.Message ?? "";

            //work around
            post.Message = post.Message.Replace("<imgwidth=", "<img width=").Replace("\n", "");

            FillAttachment(post);

            var content =
                new HtmlDoc(string.Format("<div>{0}</div>", S1Resource.HttpUtility.HtmlDecode(post.Message)))
                    .RootElement;

            if (content != null)
                item.AddRange(SimpleParser.SimpleThreadParser.ReGroupContent(content));
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
