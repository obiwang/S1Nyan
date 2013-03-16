using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using S1Nyan.Model;
using S1Parser;

namespace S1Nyan.Design
{
    public class DesignDataService : IDataService
    {
        static S1ListItem childChildItem = new S1ListItem { Title = "子子测试", Subtle = "简介简介简介", };
        static S1ListItem childItem0 = new S1ListItem { Title = "子测试0", Subtle = "简介简介简介", };
        static S1ListItem childItem1 = new S1ListItem
        {
            Title = "子测试1",
            Subtle = "简介简介简介",
            Children = new List<S1ListItem> { childChildItem }
        };
        static List<S1ListItem> data = new List<S1ListItem> { 
            new S1ListItem { Title = "测试1", Subtle = "65",
                Children = new List<S1ListItem> { childItem1, childItem0, childItem1}
            }, 
            new S1ListItem { Title = "Samle Text 0, Samle Text 1, Samle Text 0, Samle Text 1, Samle Text 0, Samle Text 1," , Subtle = "8865",
                Children = new List<S1ListItem> {childItem0, childItem0 }
            } ,
            new S1ListItem { Title = "测试3" } };

        static S1ThreadItem threadItem = new S1ThreadItem
        {
            Author = "Author",
            Date = "2013-01-31 23:11",
        };

        static S1ThreadItem threadItem2 = new S1ThreadItem
        {
            Author = "Author",
            Date = "2013-01-31 23:11",
        };

        static S1ThreadPage thread = new S1ThreadPage
        {
            Title = "这是标题，很长的标题，this is title, a title very long 这是标题，很长的标题，this is title, a title very long",
            TotalPage = 3,
            CurrentPage = 2,
            Items = new List<S1ThreadItem> { threadItem, threadItem2, threadItem2 }
        };

        public void GetMainListData(Action<IList<S1ListItem>, Exception> callback)
        {
            callback(data, null);
        }

        public void GetThreadListData(string fid, int page, Action<S1ThreadList, Exception> callback)
        {
            callback(new S1ThreadList { Children = data}, null);
        }

        public void GetThreadData(string tid, int page, Action<S1ThreadPage, Exception> callback)
        {
            callback(thread, null);
        }

        public IResourceService ResourceService { get; set; }
        public IParserFactory ParserFactory { get; set; }

        public Task<IList<S1ListItem>> GetMainListAsync()
        {
            return Task<IList<S1ListItem>>.Factory.StartNew(() => data);
        }

        public Task<S1ThreadList> GetThreadListAsync(string fid, int page)
        {
            return Task<S1ThreadList>.Factory.StartNew(() => new S1ThreadList { Children = data });
        }

        public Task<S1ThreadPage> GetThreadDataAsync(string tid, int page)
        {
            return Task<S1ThreadPage>.Factory.StartNew(() => thread);
        }
    }
}