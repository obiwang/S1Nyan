namespace S1Parser
{
    public interface IS1Client
    {
        void AddPostParam(string key, object value);
        global::System.Threading.Tasks.Task<string> PostDataTaskAsync(System.Uri address);
        System.Net.CookieCollection Cookies { get; }
    }
}
