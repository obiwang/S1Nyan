using System;
using System.Collections.Generic;
using S1Nyan.Model;
using S1Parser;

namespace S1Nyan.App.Design
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
            Content = new HtmlElement(innerHtml:@"峯岸南作为AKB48的一期生，出道已经5年之久，在AKB48四年总选举中均在15名左右，虽然不像已经毕业的前田敦子那样人气爆棚，但也有非常稳定的粉丝圈。作为一期生，当年十五六岁的少女们也早已经步入了正常的恋爱年龄。说实话有绯闻也是很正常的事情。而AKB48的管理模式，在此次事件之后，真的超出了我的常识范围。旗下偶像削发谢罪，让我感觉这已经不是一个偶像团体而是“集中营”了。在我个人看来，这件事对于AKB48的影响的确非常大，但是并非是因为峯岸南的绯闻，而是因为AKB48对旗下偶像毫无关爱可言。就连如今身为AKB48领头人的高桥南都公开对本次事件的处理表达了不满，AKB48也许真的需要一些思考了。")
        };

        static S1ThreadItem threadItem2 = new S1ThreadItem
        {
            Author = "Author",
            Date = "2013-01-31 23:11",
            Content = new HtmlElement(innerHtml: @"峯岸南作为AKB48的一期生，出道已经5年之久，在AKB48四年总选举中均在15名左右，虽然不像已经毕业的前田敦子那样人气爆棚，但也有非常稳定的粉丝圈。作为一期生，当年十五六岁的少女们也早已经步入了正常的恋爱年龄。说实话有绯闻也是很正常的事情。而AKB48的管理模式，在此次事件之后，真的超出了我的常识范围。旗下偶像削发谢罪，让我感觉这已经不是一个偶像团体而是“集中营”了。在我个人看来，这件事对于AKB48的影响的确非常大，但是并非是因为峯岸南的绯闻，而是因为AKB48对旗下偶像毫无关爱可言。就连如今身为AKB48领头人的高桥南都公开对本次事件的处理表达了不满，AKB48也许真的需要一些思考了。")
        };

        static S1ThreadPage thread = new S1ThreadPage
        {
            Title = "这是标题，很长的标题，this is title, a title very long 这是标题，很长的标题，this is title, a title very long",
            TotalPage = 3,
            CurrentPage = 2,
            Items = new List<S1ThreadItem> { threadItem, threadItem2, threadItem2 }
        };

        public void GetMainListData(Action<List<S1ListItem>, Exception> callback)
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
    }
}