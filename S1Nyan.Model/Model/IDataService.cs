using System.Collections.Generic;
using System.Threading.Tasks;
using S1Parser;

namespace S1Nyan.Model
{
    public interface IDataService
    {
        Task<IList<S1ListItem>> GetMainListAsync();
        Task<S1ThreadList> GetThreadListAsync(string fid, int page);
        Task<S1ThreadPage> GetThreadDataAsync(string tid, int page);
        IParserFactory ParserFactory { get; set; }
    }
}
