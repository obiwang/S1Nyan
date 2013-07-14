using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace S1Nyan.Model
{
    public abstract class RemoteConfigModelBase
    {
        public const string RemoteResourcePath = "https://raw.github.com/obiwang/S1Nyan/master/S1Nyan.App.WP7/Resources/";
        protected abstract string ConfigFileName { get; }
        protected abstract double CacheDays { get; }
        protected abstract string RemoteFilePath { get; }

        public IStorageHelper StorageHelper
        {
            get { return IsolatedStorageHelper.Current; }
        }

        protected abstract void RefreshList();

        public async Task UpdateListFromRemote()
        {
            var s = StorageHelper.ReadFromLocalCache(ConfigFileName, CacheDays);
            if (s != null)
            {
                s.Dispose();
                return;
            }

            var client = new WebClient();
            try
            {
                s = await client.OpenReadTaskAsync(RemoteFilePath);
                Debug.WriteLine("Update server list from remote");
                UpdateList(s);

                RefreshList();

                using (s)
                {
                    using (var reader = new StreamReader(s))
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        StorageHelper.WriteToLocalCache(ConfigFileName, reader.ReadToEnd());
                    }
                }
            }
            catch (Exception) { }
        }

        protected void GetLocalList()
        {
            var s = StorageHelper.ReadFromLocalCache(ConfigFileName, -1);
            if (s == null)
            {
                Debug.WriteLine("Using resource server list");
                s = StorageHelper.ReadFromAppResource(ConfigFileName);
            }

            UpdateList(s);
        }

        protected abstract void UpdateList(Stream s);
    }
}