using System.Collections.Generic;

namespace S1Parser
{
    public class S1Post
    {
        public string Title { get; set; }

        public string FullLink { get; internal set; }

        public string ReplyLink { get; internal set; }

        public List<S1PostItem> Items { get; set; }

        public int TotalPage { get; set; }

        public int CurrentPage { get; set; }
}

    public class S1PostItem : List<HtmlElement>
    {
        public int No { get; set; }

        public string Author { get; set; }

        public string Date { get; set; }
    }

}
