using System.Threading.Tasks;

namespace S1Nyan
{
    public interface IUserService
    {
        void InitLogin();
        Task<string> DoSendPost(string replyLink, string replyText, string verify);
    }
}
