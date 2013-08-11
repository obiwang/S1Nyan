using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using S1Parser.SimpleParser;
using System.Linq;
using S1Parser.DZParser;

namespace S1Parser.Test
{
    [TestClass]
    public class DZThreadParserTest
    {
        [TestMethod]
        public void TestReGroupContent()
        {
            FileStream file = new FileStream("Data/thread.json", FileMode.Open);
            var parser = new DZThreadParser(file);
            var data = parser.GetData();
            var content = data.Items[0].Content;
            Assert.AreEqual(47, content.Count());
            Assert.AreEqual(1, content.First().Descendants().Count());
            Assert.AreEqual(0, content.ElementAt(1).Descendants().Count());
            Assert.AreEqual(0, content.ElementAt(3).Descendants().Count());

            var blockquote = data.Items[4].Content.First().Element();
            Assert.AreEqual("blockquote", blockquote.Name);
            Assert.AreEqual(2, blockquote.Children.Count);
        }
    }
}
