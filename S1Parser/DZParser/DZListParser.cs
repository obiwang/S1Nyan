using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace S1Parser.DZParser
{
    public class DZListParser
    {
        private string raw;

        public DZListParser()
        {
        }

        public DZListParser(Stream s)
        {
            using (var reader = new StreamReader(s))
            {
                raw = reader.ReadToEnd();
            }
        }

        public DZListParser(string s)
        {
            raw = s;
        }

        public List<S1ListItem> GetData()
        {
            var data = raw.Parse<ForumList>();

            var groups = from g in data.Forums
                where g.Type == ForumTypes.Group && !string.IsNullOrEmpty(g.Name)
                select BuildGroup(g, data.Forums);

            var list = new List<S1ListItem>(groups);
            list.Insert(0, DZMyGroup.MyGroup);
            return list;
        }

        private S1ListItem BuildGroup(ForumItem g, ForumItem[] forumItem)
        {
            var group = new S1ListItem {Title = S1Resource.HttpUtility.HtmlDecode(g.Name)};
            var forums =
                from f in forumItem
                where f.Fup == g.Fid && !string.IsNullOrEmpty(f.Name)
                select new S1ListItem(S1Resource.HttpUtility.HtmlDecode(f.Name), f.Fid,
                    (from sub in forumItem
                        where sub.Fup == f.Fid && !string.IsNullOrEmpty(sub.Name)
                        select new S1ListItem
                        {
                            Title = S1Resource.HttpUtility.HtmlDecode(sub.Name),
                            Id = sub.Fid
                        }));

            group.AddRange(forums);
            return group;
        }

    }
}
