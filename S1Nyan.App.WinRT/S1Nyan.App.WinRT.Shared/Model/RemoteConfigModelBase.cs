using Caliburn.Micro;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace S1Nyan.Model
{
    public abstract class RemoteConfigModelBase
    {
        public const string RemoteResourcePath = "https://raw.github.com/obiwang/S1Nyan/master/Config/";
        protected abstract string ConfigFileName { get; }
        protected abstract double CacheDays { get; }
        protected abstract string RemoteFilePath { get; }

        private static IStorageHelper _helper;

        public static IStorageHelper StorageHelper
        {
            get { return _helper ?? (_helper = IoC.Get<IStorageHelper>()); }
        }

        protected abstract void RefreshList();

        public async Task UpdateListFromRemote()
        {
            var s = await StorageHelper.ReadFromLocalCache(ConfigFileName, CacheDays);
            if (s != null)
            {
                s.Dispose();
                return;
            }

            var client = new HttpClient();
            try
            {
                s = await client.GetStreamAsync(RemoteFilePath);
                Debug.WriteLine("Update server list from remote");
                UpdateList(s);

                RefreshList();

                using (s)
                {
                    using (var reader = new StreamReader(s))
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        await StorageHelper.WriteToLocalCache(ConfigFileName, reader.ReadToEnd());
                    }
                }
            }
            catch (Exception) { }
        }

        protected async void GetLocalList()
        {
            var s = await StorageHelper.ReadFromLocalCache(ConfigFileName, -1);
            if (s == null)
            {
                Debug.WriteLine("Using resource server list");
                s = await StorageHelper.ReadFromAppResource(ConfigFileName);
            }

            UpdateList(s);
        }

        protected abstract void UpdateList(Stream s);
    }
}