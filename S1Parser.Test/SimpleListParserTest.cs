﻿using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using S1Parser.SimpleParser;
using System.Linq;

namespace S1Parser.Test
{
    [TestClass]
    public class SimpleListParserTest
    {
        [TestMethod]
        public void TestGetList()
        {
            FileStream file = new FileStream("Data/simple.htm", FileMode.Open);
            var parser = new SimpleListParser(file);
            var list = parser.GetData();
            Assert.AreEqual<int>(4, list.Count);
            Assert.AreEqual<string>("内野", list.Last()[0].Title);
            Assert.AreEqual<string>("游戏精华备份区", list.Last()[1].Title);
            Assert.AreEqual<string>("装备区", list[0][2][0].Title);
            Assert.AreEqual<string>("122", list[0][0].Id);
            Assert.AreEqual<string>("将军", list.FindItemById("122").Title);
        }

        [TestMethod]
        public void TestGetThreadList()
        {
            FileStream file = new FileStream("Data/simple_thread.htm", FileMode.Open);
            var parser = new SimpleThreadListParser(file);
            var data = parser.GetData();
            Assert.AreEqual<int>(2773, data.TotalPage);
            Assert.AreEqual<int>(1, data.CurrentPage);
            Assert.AreEqual<int>(100, data.Count);
            Assert.AreEqual<string>("876168", data[0].Id);
            Assert.AreEqual<string>("254", data[0].Subtle);

        }
    }
}
