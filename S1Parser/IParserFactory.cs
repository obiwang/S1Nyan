using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace S1Parser
{
    public interface IParserFactory
    {
        IResourceService ResourceService { get; set; }
        Task<IList<S1ListItem>> GetMainListData();
        Task<S1ThreadList> GetThreadListData(string fid, int page);
        Task<S1ThreadPage> GetThreadData(string tid, int page);
    }
}
