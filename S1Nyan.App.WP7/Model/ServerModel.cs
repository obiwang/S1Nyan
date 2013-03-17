using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;

namespace S1Nyan.Model
{
    public class ServerModel : IServerModel
    {
        public ServerModel()
        {
            Init();
        }

        private void Init()
        {
            GetLocalList();

            UpdateServerAddr();
        }

        public void UpdateServerAddr(string url = null)
        {
            if (url == null)
            {
                url = S1Nyan.Views.SettingView.CurrentServerAddr;
                if (url == null)
                    url = List[0].Addr;
            }
            else
            {
                S1Nyan.Views.SettingView.CurrentServerAddr = url;
            }

            List<string> list = new List<string>();
            foreach (var server in List)
            {
                list.Add(server.Addr);
            }
            S1Parser.S1Resource.HostList = list;
            S1Parser.S1Resource.SiteBase = url;
        }

        private List<IServerItem> _serverList = new List<IServerItem>();

        public List<IServerItem> List
        {
            get {
                return _serverList;
            }
        }

        private void GetLocalList()
        {
            var root = XDocument.Load(Application.GetResourceStream(new Uri("Resources/server_list.xml", UriKind.Relative)).Stream).Root;
            var temp = root.Element("ServerList");
            foreach (var server in temp.Descendants())
            {
                _serverList.Add(new ServerItem { Addr = server.Attribute("addr").Value });
            }

            Msg = root.Element("Message").Value;
        }

        public string Msg { get; set; }
    }
}
