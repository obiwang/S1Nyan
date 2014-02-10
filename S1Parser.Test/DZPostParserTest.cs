using Microsoft.VisualStudio.TestTools.UnitTesting;
using S1Parser.DZParser;
using System.IO;
using System.Linq;

namespace S1Parser.Test
{
    [TestClass]
    public class DZPostParserTest
    {
        [TestMethod]
        public void TestReGroupContent()
        {
            FileStream file = new FileStream("Data/post.json", FileMode.Open);
            var parser = new DZPostParser(file);
            var data = parser.GetData();
            var content = data.Items[0];
            Assert.AreEqual(47, content.Count());
            Assert.AreEqual(1, content.First().Descendants().Count());
            Assert.AreEqual(0, content.ElementAt(1).Descendants().Count());
            Assert.AreEqual(0, content.ElementAt(3).Descendants().Count());

            var blockquote = data.Items[4].First().Element();
            Assert.AreEqual("blockquote", blockquote.Name);
            Assert.AreEqual(2, blockquote.Children.Count);
        }

        [TestMethod]
        public void TestParseThread()
        {
            FileStream file = new FileStream("Data/post.json", FileMode.Open);
            var parser = new DZPostParser(file);
            var data = parser.GetData();
            var content = data.Items[0];

        }

        [TestMethod]
        public void TestParseAttachment()
        {
            FileStream file = new FileStream("Data/post2.json", FileMode.Open);
            var parser = new DZPostParser(file);
            var data = parser.GetData();
            var p = data.Items[10][2];
            Assert.AreEqual("img", p.Descendants().Last().Name);
        }
        
        [TestMethod]
        public void TestNullPost()
        {
            FileStream file = new FileStream("Data/post2.json", FileMode.Open);
            var parser = new DZPostParser(file);
            var data = parser.GetData();
            var p = data.Items[3][0];
            Assert.IsNull(p);
        }
    }
}
