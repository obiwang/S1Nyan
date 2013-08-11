using System.Collections.Generic;

namespace S1Parser
{
    public class S1ThreadPage
    {
        public string Title { get; set; }

        public string FullLink { get; internal set; }

        public string ReplyLink { get; internal set; }

        public List<S1ThreadItem> Items { get; set; }

        public int TotalPage { get; set; }

        public int CurrentPage { get; set; }

        public string Hash { get; set; }
}

    public class S1ThreadItem : IEnumerable<HtmlElement>
    {
        public int No { get; set; }

        public string Author { get; set; }

        public string Date { get; set; }

        public IEnumerable<HtmlElement> Content { get; set; }

        public IEnumerator<HtmlElement> GetEnumerator()
        {
            return Content.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
