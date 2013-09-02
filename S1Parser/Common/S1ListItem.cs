using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace S1Parser
{
    public class S1ListItem : IEnumerable<S1ListItem>
    {
        public string Title { get; set; }

        public string Subtle { get; set; }

        public string Link { get; internal set; }

        public List<S1ListItem> Children { get; set; }

        public string Id { get; internal set; }

        public string Author { get; set; }

        public DateTime AuthorDate { get; set; }

        public string LastPoster { get; set; }

        public DateTime LastPostDate { get; set; }

        public S1ListItem()
        {
            Children = new List<S1ListItem>();
        }

        public IEnumerator<S1ListItem> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal static S1ListItem GetItem(HtmlElement e)
        {
            S1ListItem item = null;
            var a = e.Element("a");
            if (a != null && a.InnerHtml != null && a.InnerHtml.Length > 0)
            {
                item = new S1ListItem { Title = a.InnerHtml, Link = a.Attributes["href"] };
                item.Id = GetItemId(item.Link);
            }
            return item;
        }

        static Regex _linkPattern = new Regex(@"simple/\?[ft](?<ID>\d+).html");
        internal static string GetItemId(string link)
        {
            if (link == null) return null;

            var match = _linkPattern.Match(link);
            return match.Groups["ID"].Value;
        }

    }

    public static class S1ListItemExtension
    {
        public static S1ListItem FindItemById(this List<S1ListItem> list, string Id)
        {
            S1ListItem find = null;
            if (list != null)
                foreach (var item in list)
                {
                    if (item.Id == Id) return item;
                    if (null != (find = FindItemById(item.Children, Id)))
                        break;
                }
            return find;
        }
    }

}
