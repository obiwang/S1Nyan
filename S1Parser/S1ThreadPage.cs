using System.Collections.Generic;

namespace S1Parser
{
    public class S1ThreadPage
    {
        public string Title { get; set; }

        public string FullLink { get; internal set; }

        public List<S1ThreadItem> Items { get; set; }

        public int TotalPage { get; set; }

        public int CurrentPage { get; set; }
    }

    public class S1ThreadItem
    {

        public string Author { get; set; }

        public string Date { get; set; }

        public HtmlElement Content { get; set; }
    }

}
