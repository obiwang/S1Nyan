using S1Nyan.ViewModels;
using S1Parser.DZParser;
using System.Reflection;

namespace S1Nyan.DesignData
{
    public class DesignViewModelLocator
    {
        private MainPageViewModel _mainPage;
        public MainPageViewModel MainPage
        {
            get
            {
                if (_mainPage == null)
                {
                    _mainPage = new MainPageViewModel(null, null, null, null);

                    var stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream("S1Nyan.main.json");
                    var parser = new DZListParser(stream);
                    _mainPage.MainListData = parser.GetData();
                }
                return _mainPage;
            }
        }

        private ThreadListViewModel _threadList;

        public ThreadListViewModel ThreadList
        {
            get
            {
                if (_threadList == null)
                {
                    _threadList = new ThreadListViewModel(null, null, null);

                    var stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream("S1Nyan.threadlist.json");
                    var parser = new DZThreadListParser(stream);
                    _threadList.ThreadListData = parser.GetData("11", 1);
                    _threadList.Title = "PC数码";
                }
                return _threadList;
            }
        }

        public object Test
        {
            get
            {
                return "It works!";
            }
        }

    }
}
