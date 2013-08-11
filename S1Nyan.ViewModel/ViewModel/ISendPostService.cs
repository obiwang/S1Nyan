using System;
namespace S1Nyan.ViewModel
{
    public interface ISendPostService
    {
        System.Threading.Tasks.Task<string> DoSendPost(string replyLink, string replyText, string verify);
    }
}
