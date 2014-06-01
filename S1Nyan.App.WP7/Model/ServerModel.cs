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
                url = /*Views.SettingView.CurrentServerAddress ??*/ _serverList[0].Addr;
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
            var msg = root.Element("Message");
            if (msg != null)
            {
                Msg = msg.Value;
            }
        } 
        #endregion

        public string Msg { get; set; }


        public void UpdateServerWithSuccessItem(IServerItem item)
        {
            //Root/ServerList/Server addr=
            //var doc = XDocument.;
            var msg = this.Msg ?? "";

            var newItem = new XElement("Server");
            newItem.SetAttributeValue("addr", item.Addr);
            var doc = new XDocument(
                new XElement("Root",
                    new XElement("ServerList", newItem),
                    new XElement("Message", msg)));

            StorageHelper.WriteToLocalCache(ConfigFileName, doc.ToString());
            UpdateServerAddr(item.Addr);
        }
    }
}
