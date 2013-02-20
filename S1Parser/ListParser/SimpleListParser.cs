using System.Collections.Generic;
using System.Linq;

namespace S1Parser.ListParser
{
    public class SimpleListParser : DataParser<List<S1ListItem>>
    {
        protected override void ParseImpl()
        {
            var root = HtmlPage.FindElements("table").Last().FindFirst("td");
            S1ListItem lastitem = null;
            foreach (var e in root.Descendants())
            {
                if (e.Name == "li")
                {
                    var header = e.Element();
                    if (header.Name == "h2")
                    {
                        lastitem = new S1ListItem { Title = header.InnerHtml };
                        theData.Add(lastitem);
                    }
                    else
                    {   //missing ul
                        AddChildItem(lastitem, e);
                    }
                }
                else AddChildren(lastitem, e);
            }
        }

        protected static void AddChildren(S1ListItem item, HtmlElement e)
        {
            var lastitem = item;
            foreach (var ee in e.Descendants())
            {
                if (ee.Name == "li")
                    lastitem = AddChildItem(item, ee);
                if (ee.Name == "ul")
                    AddChildren(lastitem, ee);
            }
        }

        protected static S1ListItem AddChildItem(S1ListItem item, HtmlElement e)
        {
            var result = item;
            if (item != null && (result = S1ListItem.GetItem(e)) != null)
            {
                if (item.Children == null)
                    item.Children = new List<S1ListItem>();
                item.Children.Add(result);
            }
            return result;
        }

    }
}
