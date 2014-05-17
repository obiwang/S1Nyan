using System.Net;

namespace S1Parser
{
    public class DummyHttpUtility : IHttpUtility
    {
        public string HtmlDecode(string s)
        {
#if S1NyanRT
            return WebUtility.HtmlDecode(s);
#else
            return s;
#endif
        }
    }
}