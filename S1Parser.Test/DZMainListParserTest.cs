using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using S1Parser.SimpleParser;
using System.Linq;
using S1Parser.DZParser;

namespace S1Parser.Test
{
    [TestClass]
    public class DZMainListParserTest
    {
        [TestMethod]
        public void TestMainList()
        {
            FileStream file = new FileStream("Data/main.json", FileMode.Open);
            var parser = new DZListParser(file);
            var list = parser.GetData();
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual("子论坛", list.Last().Title);
            Assert.AreEqual("吃货", list[1][8][0].Title);
            Assert.AreEqual("134", list[0][0].Id);
            Assert.AreEqual("将军", list.FindItemById("122").Title);
        }
    }
}
