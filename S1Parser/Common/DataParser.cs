
using System.IO;
namespace S1Parser
{
    public abstract class DataParser<T> : IDataParser<T> 
        where T : new()
    {
        public DataParser() { }
        public DataParser(Stream s) { HtmlPage = new HtmlDoc(s).RootElement; }
        public DataParser(string s) { HtmlPage = new HtmlDoc(s).RootElement; }

        private HtmlElement htmlPage;
        public HtmlElement HtmlPage
        {
            get { return htmlPage; }
            set
            {
                htmlPage = value;
                theData = default(T);
            }
        }

        protected T theData;

        public T GetData()
        {
            if (theData != null)
                return theData;
            else
                theData = new T();

            if (HtmlPage != null)
            {
                ParseImpl();
            }
            return theData;
        }

        abstract protected void ParseImpl();
    }
}
