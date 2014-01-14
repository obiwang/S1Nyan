namespace S1Parser
{
    public class DummyHttpUtility : IHttpUtility
    {
        public string HtmlDecode(string s)
        {
            return s;
        }
    }
}