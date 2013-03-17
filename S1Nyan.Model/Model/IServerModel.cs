using System.Collections.Generic;
using System.Threading.Tasks;

namespace S1Nyan.Model
{
    public interface IServerModel
    {
        List<IServerItem> List { get; }
        string Msg { get; set; }
        void UpdateServerAddr(string url = null);
        Task UpdateListFromRemote();
    }
}
