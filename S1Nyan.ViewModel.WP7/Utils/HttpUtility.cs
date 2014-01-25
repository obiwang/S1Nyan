namespace S1Nyan.Utils
{
    public class HttpUtility : S1Parser.IHttpUtility
    {
        public string HtmlDecode(string s)
        {
            return System.Net.HttpUtility.HtmlDecode(s);
        }
    }
}