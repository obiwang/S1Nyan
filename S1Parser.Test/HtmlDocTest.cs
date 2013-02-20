using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace S1Parser.Test
{
    [TestClass]
    public class HtmlDocTest
    {
        MemoryStream stream = null;

        string test0 = "lask dfl lk ask\r\ndf <hello name=\"test\"> this is\r\n <H2>a test</h2> \r\n</hello>l; aks ;fdl\r\n";

        string test1 = 
@"</ul><li><h2>热门新区</h2></li>
<ul><li><a href='simple/?f122.html'>将军</a></li>
<li><a href='simple/?f125.html'>热血海贼王</a></li>
<li><a href='simple/?f118.html'>真碉堡山 Diablo3</a></li>
<ul><li><a href='simple/?f121.html'>装备区</a></li>
</ul><li><a href='simple/?f56.html'>梦幻之星 Phantasy Star</a></li>
<li><a href='simple/?f111.html'>英雄联盟(LOL)</a></li>
<li><a href='simple/?f15.html'>本垒</a></li>
</ul>";

        [TestInitialize()]
        public void Setup()
        {
            stream = new MemoryStream(1024);
            var writer = new StreamWriter(stream);
            writer.Write(test0);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }

        [TestMethod]
        public void TestElement()
        {
            var doc = new HtmlDoc(stream).RootElement;
            HtmlElement node;
            node = doc.Element("h2");
            Assert.IsNull(node);

            node = doc.Element("hello");
            Assert.AreEqual<String>("hello", node.Name);

            node = node.Element("h2");
            Assert.AreEqual<String>("h2", node.Name);
        }

        [TestMethod]
        public void TestDescendants()
        {
            var e = HtmlDoc.Parse(test1);
            Assert.IsTrue(e.Descendants().Count() > 1);
            Assert.AreEqual<int>(6, e.Element("ul").Descendants("li").Count());
        }

        string test2 = "asdf<div><h2>f<br /></h2><b>asdf</b>daa</div>jkl";
        string test2Formated =
@"asdf
<div>
    <h2>
        f
        <br />
    </h2>
    <b>asdf</b>
    daa
</div>
jkl
";
        [TestMethod]
        public void TestToString()
        {
            var e = HtmlDoc.Parse(test2);
            Assert.AreEqual<string>(test2Formated, e.ToString());
            Assert.AreEqual<string>(test2, e.ToString(false));
        }

        [TestMethod]
        public void TestLink()
        {
            var e = HtmlDoc.Parse("<a href='./' target='_blank'>STAGE1</a>");
            Assert.AreEqual<string>("STAGE1", e.InnerHtml);
        }

        string test3 = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />";
        [TestMethod]
        public void TestSelfClosed()
        {
            var e = HtmlDoc.Parse(test3);
            Assert.AreEqual<string>("meta", e.Name);
        }

        [TestMethod]
        public void TestAttributes()
        {
            var e = HtmlDoc.Parse(test3);
            Assert.AreEqual<string>("Content-Type", e.Attributes["http-equiv"]);
            Assert.AreEqual<string>("text/html; charset=utf-8", e.Attributes["content"]);
        }

        string test4 = @"
<head>
    <meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
    <base href='http://bbs.saraba1st.com/2b/'>
    <title>STAGE1 - Powered by phpwind</title>
    <meta name='generator' content='phpwind 8.3' />
    <meta name='description' content='STAGE1' />
    <meta name='keywords' content='STAGE1' />
</head>";
        string test44 = @"
<table>
    <td>
        <ul>
            <li><a href='simple/?f27.html'>内野</a></li> <br>
            <ul>
                <li><a href='simple/?f9.html'>游戏精华备份区</a></li>
    </td>
</table>
";

        [TestMethod]
        public void TestMismatch()
        {
            var e = HtmlDoc.Parse(test4);
            Assert.AreEqual<int>(6, e.Descendants().Count());
            e = HtmlDoc.Parse(test44).Element().Element();
            Assert.AreEqual<string>("ul", e.Name);
            //Assert.AreEqual<int>(3, e.Descendants().Count()); ///TODO: look up html tags
            //Assert.AreEqual<string>("ul", e.Descendants().Last().Name);
        }

        [TestMethod]
        public void TestFindElement()
        {
            FileStream file = new FileStream("Data/simple.htm", FileMode.Open);
            var doc = new HtmlDoc(file).RootElement;
            var tables = from table in doc.FindElements("table")
                        where table.Attributes["width"] == "98%"
                        where table.Attributes["cellpadding"] == "7"
                        select table;

            Assert.AreEqual<int>(1, tables.Count());

            tables = from table in doc.FindElements()
                    where table.Name == "table"
                    select table;
            Assert.AreEqual<int>(3, tables.Count());
        }

        //[TestMethod]
        public void MyTestMethod()
        {
            FileStream file = new FileStream("Data/simple.htm", FileMode.Open);
            var e = new HtmlDoc(file).RootElement;
            var s = e.ToString();
            Assert.AreEqual<string>("", s);
        }
    }
}
