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

        protected override void ParseImpl()
        {
            var body = HtmlPage.FindFirst("body");
            var a = body.FindFirst("a");
            theData.Title = a.InnerHtml;
            theData.FullLink = a.Attributes["href"];

            GetPageCount(body.FindElements("center").ElementAt(1));
            if (theData.CurrentPage == 0) theData.CurrentPage = 1;

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
            threadItem.Content = content;
            return threadItem;
        }

        static Regex _totalPagePattern = new Regex(@"Pages: \( (?<total>\d+) total \)");
        private void GetPageCount(HtmlElement page)
        {
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
