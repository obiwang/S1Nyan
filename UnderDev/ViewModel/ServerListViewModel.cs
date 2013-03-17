using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using S1Nyan.Model;
using S1Parser;

namespace UnderDev.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ServerListViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ServerListViewModel class.
        /// </summary>
        public ServerListViewModel()
        {
            var root = XDocument.Load(Application.GetResourceStream(new Uri("Resources/server_list.xml", UriKind.Relative)).Stream).Root;
            var temp = root.Element("ServerList");
            foreach (var server in temp.Descendants())
            {
                _serverList.Add(new ServerViewModel { Addr = server.Attribute("addr").Value });
            }
            temp = root.Element("Titles");
            foreach (var item in temp.Descendants())
            {
                _specialTitles[item.Name.LocalName] = item.Value;
            }
            Msg = root.Element("Message").Value;
        }
        private List<ServerViewModel> _serverList = new List<ServerViewModel>();
        private static Dictionary<string, string> _specialTitles = new Dictionary<string, string>();

        public static string ServerDownTitle
        {
            get
            {
                if (_specialTitles.ContainsKey("ServerDown"))
                {
                    return _specialTitles["ServerDown"];
                }
                return null;
            }
        }

        /// <summary>
        /// Sets and gets the ServerList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public List<ServerViewModel> ServerList
        {
            get { return _serverList; }

            set
            {
                if (_serverList == value) return;

                _serverList = value;
                RaisePropertyChanged(() => ServerList);
            }
        }

        private RelayCommand _refreshCommand;

        /// <summary>
        /// Gets the RefreshCommand.
        /// </summary>
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand
                    ?? (_refreshCommand = new RelayCommand(
                                          () =>
                                          {
                                              foreach (var item in ServerList)
                                              {
                                                  if (IsBreakWhenAny)
                                                      item.NotifySuccess = OnSuccess;
                                                  item.TestServer();
                                              }
                                          }));
            }
        }

        private void OnSuccess()
        {
            foreach (var item in ServerList)
                item.Cancle();
        }

        private bool _isBreakOnAny = false;

        /// <summary>
        /// Sets and gets the IsBreakWhenAny property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsBreakWhenAny
        {
            get { return _isBreakOnAny; }

            set
            {
                if (_isBreakOnAny == value) return;

                _isBreakOnAny = value;
                RaisePropertyChanged(() => IsBreakWhenAny);
            }
        }

        private string _msg = "";

        /// <summary>
        /// Sets and gets the Msg property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Msg
        {
            get { return _msg; }

            set
            {
                if (_msg == value) return;

                _msg = value;
                RaisePropertyChanged(() => Msg);
            }
        }
    }

    public class ServerViewModel: ViewModelBase
    {
        private string _addr = "";

        /// <summary>
        /// Sets and gets the Addr property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Addr
        {
            get { return _addr; }

            set
            {
                if (_addr == value) return;

                _addr = value;
                RaisePropertyChanged(() => Addr);
            }
        }

        private string _status = " ";

        /// <summary>
        /// Sets and gets the Status property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Status
        {
            get { return _status; }

            set
            {
                if (_status == value) return;

                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        private const string path = "simple/";
        S1WebClient client = null;
        public async void TestServer()
        {
            Status = "Connecting";
            try{
                client = new S1WebClient();
                var result = await client.DownloadStringTaskAsync(Addr + path); 
                Status = "Wrong Data";
                if (result.Length > 0)
                {
                    var root = new HtmlDoc(result).RootElement;
                    var serverDownTitle = ServerListViewModel.ServerDownTitle;
                    if (serverDownTitle != null &&
                        root.FindFirst("title").InnerHtml.Contains(serverDownTitle))
                    {
                        Status = "Server Down";
                        if (NotifySuccess != null) NotifySuccess();
                    }
                    else
                    {
                        if (NotifySuccess != null) NotifySuccess();
                        Status = "Success";
                    }
                }
            }
            catch (TaskCanceledException)
            {
                Status = "Cancled";
            }
            catch(Exception)
            {
                Status = "Failed";
            }
        }

        public Action NotifySuccess;

        public void Cancle()
        {
            if (client != null && client.IsBusy)
            {
                client.CancelAsync();
            }
        }
    }
}