namespace S1Parser
{
    public interface IDataParser<T>
     where T : new()
    {
        T GetData();
        HtmlElement HtmlPage { get; set; }
    }
}
