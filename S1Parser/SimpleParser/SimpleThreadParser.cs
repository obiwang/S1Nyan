using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using S1Parser.PaserFactory;

namespace S1Parser.SimpleParser
{
    public class SimpleThreadParser : DataParser<S1Post>
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

                GetReplyLink(body.Element("table"));

                GetPageCount(body.FindElements("center").ElementAt(1));

                theData.Items = new List<S1PostItem>();
                int i = 0;
                foreach (var item in body.Descendants("table"))
                {
                    var threadItem = ParseThreadItem(item);
                    if (threadItem != null)
                    {
                        threadItem.No = (theData.CurrentPage - 1) * SimpleParserFactory.ItemsPerThreadSimple + i++;
                        theData.Items.Add(threadItem);
                    }
                }
            }
            catch (System.Exception) { }
            finally
            {
                if (theData.Items == null || theData.Items.Count == 0)
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

        protected virtual S1PostItem ParseThreadItem(HtmlElement item)
        {
            if (null == (item = item.FindFirst("table"))) return null;

            var trs = item.Descendants("tr");
            if (trs.Count() < 2) return null;
            var head = trs.First();
            if (head.Attributes["class"] != "head") return null;

            var threadItem = new S1PostItem();
            threadItem.Author = head.Element().PlainText();
            threadItem.Date = head.Descendants().Last().PlainText();

            var content = trs.ElementAt(1).Element("td");

            if (content != null)
                threadItem.AddRange(ReGroupContent(content));
            return threadItem;
        }

        static internal List<HtmlElement> ReGroupContent(HtmlElement element)
        {
            bool hasContent = false;
            HtmlElement lastGroup = null;

            var list = new List<HtmlElement>();
            int BRsBefore = 0;
            foreach (var item in element.Descendants())
            {
                if (!hasContent && item.Name == "h1")
                {
                    //ignore title
                    hasContent = true;
                    continue;
                }

                if (BRsBefore > 0 && item.Name != "br")
                {
                    if (BRsBefore > 1)
                    {
                        list.Add(lastGroup);
                        lastGroup = null;
                    }
                    BRsBefore = 0;
                }

                if (lastGroup == null)
                    lastGroup = new HtmlElement("Paragraph", children: new List<HtmlElement>());

                if (item.Name == "br")
                {
                    if (BRsBefore == 0)
                    {
                        list.Add(lastGroup);
                        lastGroup = null;
                    }
                    BRsBefore++;
                }
                else
                {
                    if (item.Name == "div")
                    {
                        var quote = item.Element();
                        if (quote != null &&
                            quote.Name == "blockquote")
                        {
                            if (lastGroup.Children.Count != 0)
                            {
                                list.Add(lastGroup);
                            }
                            lastGroup = new HtmlElement("Paragraph", children: new List<HtmlElement>());
                            lastGroup.Children.Add(new HtmlElement("blockquote", children: ReGroupContent(quote)));
                            list.Add(lastGroup);
                            lastGroup = null;
                        }
                        else
                        {
                            list.Add(lastGroup);
                            lastGroup = null;
                            list.AddRange(ReGroupContent(item));
                        }
                    }
                    else
                        lastGroup.Children.Add(item);
                }
            }
            list.Add(lastGroup);
            return list;
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
