using Microsoft.VisualStudio.TestTools.UnitTesting;
using S1Parser.DZParser;
using System;
using System.IO;
using System.Linq;

namespace S1Parser.Test
{
    [TestClass]
    public class DZMainListParserTest
    {
        [TestMethod]
        public void TestMainList()
        {
            var file = new FileStream("Data/main.json", FileMode.Open);
            var parser = new DZListParser(file);
            var list = parser.GetData();
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual("子论坛", list.Last().Title);
            Assert.AreEqual("吃货", list[2][8][0].Title);
            Assert.AreEqual("134", list[1][0].Id);
            Assert.AreEqual("将军", list.FindItemById("122").Title);
        }

        [TestMethod]
        public void TestMainListWithInvalidData()
        {
            var file = new FileStream("Data/post.json", FileMode.Open);
            try
            {
                var parser = new DZListParser(file);
                parser.GetData();
                Assert.Fail();
            }
            catch (Exception)
            {
            } 
        }

        [TestMethod]
        public void TestMainListWithNullData()
        {
            try
            {
                var parser = new DZListParser((Stream)null);
                parser.GetData();
                Assert.Fail();
            }
            catch (Exception)
            {
            }
        }

    }
}
