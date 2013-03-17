using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace S1Parser.SimpleParser
{
    public class SimpleThreadParser : DataParser<S1ThreadPage>
    {
        public SimpleThreadParser() { }
        public SimpleThreadParser(Stream s) : base(s) { }
        public SimpleThreadParser(string s) : base(s) { }

        protected override void ParseImpl()
        {
            try { 
                var body = HtmlPage.FindFirst("body");
                var a = body.FindFirst("a");
                theData.Title = a.InnerHtml;
                theData.FullLink = a.Attributes["href"];

                GetReplyLink(body.Element("table"));

                GetPageCount(body.FindElements("center").ElementAt(1));

                theData.Items = new List<S1ThreadItem>();
                int i = 0;
                foreach (var item in body.Descendants("table"))
                {
                    var threadItem = ParseThreadItem(item);
                    if (threadItem != null)
                    {
                        threadItem.No = (theData.CurrentPage - 1) * S1Resource.ItemsPerThreadSimple + i++;
                        theData.Items.Add(threadItem);
                    }
                }
            }
            catch (System.Exception) { }
            finally
            {
                if (theData.Items.Count == 0)
                {
                    S1Parser.User.ErrorParser.Parse(HtmlPage);
                    throw new InvalidDataException();
                }
            }
        }

        private void GetReplyLink(HtmlElement htmlElement)
        {
            theData.ReplyLink = "";
            var replylink = htmlElement.FindFirst("a", 
                (a) => a.Attributes["href"].ToLower().StartsWith("post.php?action=reply"));
            if (replylink != null) 
                theData.ReplyLink = replylink.Attributes["href"];
        }

        protected virtual S1ThreadItem ParseThreadItem(HtmlElement item)
        {
            if (null == (item = item.FindFirst("table"))) return null;

            var trs = item.Descendants("tr");
            if (trs.Count() < 2) return null;
            var head = trs.First();
            if (head.Attributes["class"] != "head") return null;

            var threadItem = new S1ThreadItem();
            threadItem.Author = head.Element().PlainText();
            threadItem.Date = head.Descendants().Last().PlainText();

            var content = trs.ElementAt(1).Element("td");

            if (content != null)
                threadItem.Content = ReGroupContent(content.Descendants());
            return threadItem;
        }

        private IEnumerable<HtmlElement> ReGroupContent(IEnumerable<HtmlElement> elements)
        {
            bool hasContent = false;
            HtmlElement lastGroup = null;
            bool lastIsBr = false;
            foreach (var item in elements)
            {
                if (!hasContent && item.Name == "h1")
                {   //ignore title
                    hasContent = true;
                    continue; 
                }

                if (lastGroup == null)
                    lastGroup = new HtmlElement("Paragraph", children: new List<HtmlElement>());

                if (lastIsBr)
                {
                    if (item.Name != "br")
                    {
                        lastIsBr = false;
                        if (lastGroup.Children.Count != 0)
                        {
                            yield return lastGroup;
                            lastGroup = new HtmlElement("Paragraph", children: new List<HtmlElement>());
                        }
                    }
                    lastGroup.Children.Add(item);
                }
                else if (item.Name == "br")
                {
                    lastIsBr = true;
                    yield return lastGroup;
                    lastGroup = null;
                }
                else
                {
                    lastIsBr = false;
                    lastGroup.Children.Add(item);
                }
            }
            yield return lastGroup;
        }

        static Regex _totalPagePattern = new Regex(@"Pages: \( (?<total>\d+) total \)");
        private void GetPageCount(HtmlElement page)
        {
            theData.CurrentPage = 1;
            theData.TotalPage = 1;

            if (page.Children.Count == 0) return;
            int value = 0;
            if (int.TryParse(page.Element("b").InnerHtml, out value))
                    theData.CurrentPage = value;
            
            var match = _totalPagePattern.Match(page.Descendants().Last().InnerHtml);
            if (match.Success)
            {
                if (int.TryParse(match.Groups["total"].Value, out value))
                    theData.TotalPage = value;
            }
        }

    }
}
