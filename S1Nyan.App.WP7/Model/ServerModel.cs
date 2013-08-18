using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace S1Nyan.Model
{
    public class ServerModel : RemoteConfigModelBase, IServerModel
    {
        private const string ServerListCacheName = "server_list.xml";
        private const double ServerListCacheDays = 3.0/24.0;

#if UseLocalhost
        private const string RemoteServerPath = "http://192.168.0.60/" + ServerListCacheName;
#else
        private const string RemoteServerPath = RemoteResourcePath + ServerListCacheName;
#endif

        private readonly List<IServerItem> _serverList = new List<IServerItem>();
        private readonly List<string> _obsoleteServerList = new List<string>();

        public List<IServerItem> List
        {
            get
            {
                return _serverList;
            }
        }

        public ServerModel()
        {
            Init();
        }

        private void Init()
        {
            GetLocalList();

            UpdateServerAddr();

            RefreshList();
        }

        public void UpdateServerAddr(string url = null)
        {
            if (url == null)
            {   //TODO:
                url = /*Views.SettingView.CurrentServerAddr ??*/ _serverList[0].Addr;
            }
            else
            {
                Views.SettingView.CurrentServerAddr = url;
            }
            S1Parser.S1Resource.SiteBase = url;
        }

        #region RemoteConfigModelBase member
        protected override string ConfigFileName
        {
            get { return ServerListCacheName; }
        }

        protected override double CacheDays
        {
            get { return ServerListCacheDays; }
        }

        protected override string RemoteFilePath
        {
            get { return RemoteServerPath; }
        }

        protected override void RefreshList()
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

        protected override void UpdateList(Stream s)
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
        #endregion

        public string Msg { get; set; }
    }
}
