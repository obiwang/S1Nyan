using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace S1Parser.SimpleParser
{
    public class SimpleThreadListParser : DataParser<S1ThreadList>
    {
        public SimpleThreadListParser() { }
        public SimpleThreadListParser(Stream s) : base(s) { }

        static Regex reply_pattern = new Regex(@"\d+");
        protected override void ParseImpl()
        {
            var root = HtmlPage.FindElements("table").Last()
                .FindFirst("td").FindFirst("ul");
            GetPageCount(HtmlPage.FindFirst("body").FindElements("center").ElementAt(1));

            theData.Children = new List<S1ListItem>();
            S1ListItem item = null;

            foreach (var e in root.Descendants("li"))
            {
                item = S1ListItem.GetItem(e);
                var replys = reply_pattern.Match(e.Element("span").InnerHtml);
                if (replys.Success)
                    item.Subtle = replys.Value;
                theData.Children.Add(item);
            }
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
