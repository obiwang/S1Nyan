using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using S1Parser;

namespace S1Nyan.Model
{
    public class DataService : IDataService
    {
        private const int MainListCacheDays = -1;
        private const string MainListCacheName = "simple.htm";

        private IList<S1ListItem> mainListData;
        public IParserFactory ParserFactory { get; set; }
        public IStorageHelper StorageHelper { get; set; }

        public DataService(IStorageHelper storageHelper, IParserFactory parserFactory)
        {
            ParserFactory = parserFactory;
            StorageHelper = storageHelper;
        }

        string mainListHtml;
        public async Task<IList<S1ListItem>> UpdateMainListAsync()
        {
            Stream s = StorageHelper.ReadFromLocalCache(MainListCacheName, MainListCacheDays);
            if (s == null)
            {
                s = await ParserFactory.GetMainListStream();
                using (s) {
                    using (var reader = new StreamReader(s))
                    {
                        mainListHtml = reader.ReadToEnd();
                    }
                }
                Debug.WriteLine("Using updated main list");
                return mainListData = ParserFactory.ParseMainListData(mainListHtml);
            }
            if (mainListData == null)
                mainListData = ParserFactory.ParseMainListData(s);
            s.Dispose();
            Debug.WriteLine("Using cached main list");
            return mainListData;
        }

        public IEnumerable<S1ListItem> GetMainListCache()
        {
            if (mainListData == null)
            {
                Stream s = StorageHelper.ReadFromLocalCache(MainListCacheName, -1);
                if (s == null)
                {
                    Debug.WriteLine("Using resource main list");
                    s = StorageHelper.ReadFromAppResource(MainListCacheName);
                }
                mainListData = ParserFactory.ParseMainListData(s);
            }
            return mainListData;
        }

        public void GetMainListDone(bool success = true)
        {
            if (success && mainListHtml != null)
            {
                Debug.WriteLine("save updated main list");

                StorageHelper.WriteToLocalCache(MainListCacheName, mainListHtml);
            }
            mainListHtml = null;
        }

        public async Task<S1ThreadList> GetThreadListAsync(string fid, int page)
        {
            return await ParserFactory.GetThreadListData(fid, page);
        }

        public async Task<S1Post> GetThreadDataAsync(string tid, int page)
        {
            return await ParserFactory.GetPostData(tid, page);
        }
    }
}