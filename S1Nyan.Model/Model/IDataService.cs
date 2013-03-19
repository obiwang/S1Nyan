using System.Collections.Generic;
using System.Threading.Tasks;
using S1Parser;

namespace S1Nyan.Model
{
    public interface IDataService
    {
        Task<IList<S1ListItem>> UpdateMainListAsync();
        void GetMainListDone(bool success = true);
        Task<S1ThreadList> GetThreadListAsync(string fid, int page);
        Task<S1ThreadPage> GetThreadDataAsync(string tid, int page);
        IParserFactory ParserFactory { get; set; }
        IEnumerable<S1ListItem> GetMainListCache();
        IStorageHelper StorageHelper { get; set; }
    }
}
