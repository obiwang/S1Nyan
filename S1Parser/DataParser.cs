
namespace S1Parser
{
    public abstract class DataParser<T> 
        where T : new()
    {
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
