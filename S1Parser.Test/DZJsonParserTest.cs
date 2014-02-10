using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using S1Parser.DZParser;
using System.IO;
using System.Linq;

namespace S1Parser.Test
{
    [TestClass]
    public class DZJsonParserTest
    {        
        [TestMethod]
        public void TestParseError()
        {
            FileStream file = new FileStream("Data/error.json", FileMode.Open);
            try
            {
                var parser = new DZPostParser(file).GetData();
                Assert.Fail("There should be an exception");
            }
            catch (Exception e)
            {
                Assert.AreEqual("要淡定", e.Message);
            }
        }
    }
}
