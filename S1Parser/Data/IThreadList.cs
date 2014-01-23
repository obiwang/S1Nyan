using System;
namespace S1Parser
{
    public interface IThreadList
    {
        int TotalPage { get; }
        int CurrentPage { get; }
        int ItemsPerPage { get; }
        IThreadListItem[] ThreadList { get; }
    }

    public interface IThreadListItem
    {
        string Title { get; }
        string Subtle { get; }
        string Id { get; }
        string Author { get; }
        DateTime AuthorDate { get; }
        string LastPoster { get; }
        DateTime LastPostDate { get; }
    }
}