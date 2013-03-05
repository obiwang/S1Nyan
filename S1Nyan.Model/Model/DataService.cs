using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using S1Parser;

namespace S1Nyan.Model
{
    public class DataService : IDataService
    {
        IList<S1ListItem> mainListData;
        public IParserFactory ParserFactory { get; set; }

        public async Task<IList<S1ListItem>> GetMainListAsync()
        {
            if (mainListData == null)
                mainListData = await ParserFactory.GetMainListData();
            return mainListData;
        }

        public async Task<S1ThreadList> GetThreadListAsync(string fid, int page)
        {
            return await ParserFactory.GetThreadListData(fid, page);
        }

        public async Task<S1ThreadPage> GetThreadDataAsync(string tid, int page)
        {
            return await ParserFactory.GetThreadData(tid, page);
        }
    }
}