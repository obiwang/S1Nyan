using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using S1Parser.ThreadParser;

namespace S1Parser.Test
{
    [TestClass]
    public class SimpleThreadParserTest
    {
        [TestMethod]
        public void TestGetThread()
        {
            FileStream file = new FileStream("Data/simple_read.htm", FileMode.Open);
            var parser = new SimpleThreadParser { HtmlPage = new HtmlDoc(file).RootElement };
            var thread = parser.GetData();
            Assert.AreEqual<string>("我是卖切糕的！高富帅们请进~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~", thread.Title);
            Assert.AreEqual<string>("read-htm-tid-880700.html", thread.FullLink);
            Assert.AreEqual<int>(4, thread.TotalPage);
            Assert.AreEqual<int>(1, thread.CurrentPage);
            Assert.AreEqual<int>(50, thread.Items.Count);
            Assert.AreEqual<string>("睡醒的鱼", thread.Items[1].Author);
            Assert.AreEqual<string>("2013-01-06 19:20", thread.Items[1].Date);
        }
    }
}
