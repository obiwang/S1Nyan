using System.Threading.Tasks;
using S1Parser;

namespace S1Nyan
{
    public interface IUserService : IFormHashUpdater
    {
        void InitLogin();
        Task<string> DoSendPost(string replyLink, string replyText);
        Task DoAddToFavorite(string tid);
    }
}
