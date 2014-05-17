using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using S1Parser;

namespace S1Nyan.Model
{
    public class DataService : IDataService
    {
        private const double MainListCacheDays = 0.5;
        private const string MainListCacheName = "main.json";

        private IList<S1ListItem> _mainList;
        public IParserFactory ParserFactory { get; set; }
        public IStorageHelper StorageHelper { get; set; }

        public DataService(IStorageHelper storageHelper, IParserFactory parserFactory)
        {
            ParserFactory = parserFactory;
            StorageHelper = storageHelper;
        }

        public async Task<IList<S1ListItem>> UpdateMainListAsync()
        {
            Stream s = await StorageHelper.ReadFromLocalCache(MainListCacheName, MainListCacheDays);

            try
            {
                UpdateMainListFromStream(s);
                Debug.WriteLine("Using cached main list");
                return _mainList;
            }
            catch (Exception)
            {
            }

            string mainListRaw;
            using (s = await ParserFactory.GetMainListStream())
            {
                using (var reader = new StreamReader(s))
                {
                    mainListRaw = reader.ReadToEnd();
                }
            }
            var data = ParserFactory.ParseMainListData(mainListRaw);
            Debug.WriteLine("Using updated main list");
            await StorageHelper.WriteToLocalCache(MainListCacheName, mainListRaw);
            Debug.WriteLine("Save updated main list");

            return _mainList = data;
        }

        public async Task<IEnumerable<S1ListItem>> GetMainListCache()
        {
            if (_mainList == null)
            {
                try
                {
                    UpdateMainListFromStream(
                        await StorageHelper.ReadFromLocalCache(MainListCacheName, -1)); // force read from cache
                }
                catch (Exception) { }

                if (_mainList == null)
                {
                    Debug.WriteLine("Fall back to the data in app resource");
                    UpdateMainListFromStream(
                        await StorageHelper.ReadFromAppResource(MainListCacheName));
                }
            }

            return _mainList;
        }

        private void UpdateMainListFromStream(Stream s)
        {
            using (s)
            {
                _mainList = ParserFactory.ParseMainListData(s);
            }
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