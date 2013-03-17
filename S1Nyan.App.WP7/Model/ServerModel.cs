using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace S1Nyan.Model
{
    public class ServerModel : IServerModel
    {
        private const string ServerListCacheName = "server_list.xml";
        private const double ServerListCacheDays = 3.0/24.0;

#if UseLocalhost
        private const string RemoteServerPath = "http://192.168.0.60/" + ServerListCacheName;
#else
        private const string RemoteServerPath = "https://raw.github.com/obiwang/S1Nyan/master/S1Nyan.App.WP7/Resources/" + ServerListCacheName;
#endif
        public ServerModel()
        {
            Init();
        }

        private void Init()
        {
            GetLocalList();

            UpdateServerAddr();

            UpdateHostList();
        }

        public IStorageHelper StorageHelper
        {
            get { return IsolatedStorageHelper.Current; }
        }

        public void UpdateServerAddr(string url = null)
        {
            if (url == null)
            {
                url = S1Nyan.Views.SettingView.CurrentServerAddr;
                if (url == null)
                    url = _serverList[0].Addr;
            }
            else
            {
                S1Nyan.Views.SettingView.CurrentServerAddr = url;
            }
            S1Parser.S1Resource.SiteBase = url;
        }

        private void UpdateHostList()
        {
            List<string> list = new List<string>();
            foreach (var server in _serverList)
            {
                list.Add(server.Addr);
            }
            foreach (var addr in _obsoleteServerList)
            {
                list.Add(addr);
            }
            S1Parser.S1Resource.HostList = list;
        }

        private List<IServerItem> _serverList = new List<IServerItem>();
        private List<string> _obsoleteServerList = new List<string>();

        public List<IServerItem> List
        {
            get {
                return _serverList;
            }
        }

        public async Task UpdateListFromRemote()
        {
            Stream s = StorageHelper.ReadFromLocalCache(ServerListCacheName, ServerListCacheDays);
            if (s != null)
            {
                s.Dispose();
                return;
            }

            WebClient client = new WebClient();
            try
            {
                s = await client.OpenReadTaskAsync(RemoteServerPath);
                Debug.WriteLine("Update server list from remote");
                UpdateList(s);

                UpdateHostList();

                using (s)
                {
                    using (var reader = new StreamReader(s))
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        StorageHelper.WriteToLocalCache(ServerListCacheName, reader.ReadToEnd());
                    }
                }
            }
            catch (Exception) { }
        }

        private void GetLocalList()
        {
            Stream s = StorageHelper.ReadFromLocalCache(ServerListCacheName, -1);
            if (s == null)
            {
                Debug.WriteLine("Using resource server list");
                s = StorageHelper.ReadFromAppResource(ServerListCacheName);
            }

            UpdateList(s);
        }

        private void UpdateList(Stream s)
        {
            var root = XDocument.Load(s).Root;
            var temp = root.Element("ServerList");
            
            _serverList.Clear();
            foreach (var server in temp.Descendants())
            {
                _serverList.Add(new ServerItem { Addr = server.Attribute("addr").Value });
            }

            _obsoleteServerList.Clear();
            temp = root.Element("ObsoleteServerList");
            foreach (var server in temp.Descendants())
            {
                _obsoleteServerList.Add(server.Attribute("addr").Value);
            }

            Msg = root.Element("Message").Value;
        }

        public string Msg { get; set; }
    }
}
