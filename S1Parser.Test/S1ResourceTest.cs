using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace S1Parser.Test
{
    [TestClass]
    public class S1ResourceTest
    {
        [TestMethod]
        public void TestGetViewParamFromUrl()
        {
            S1Resource.HostList = new List<string> { "http://220.196.42.167/2b/", "http://bbs.saraba1st.com/2b/"};
            string s0 = "http://220.196.42.167/2b/read-htm-tid-785763-page-54.html";
            string s1 = "http://bbs.saraba1st.com/2b/read-htm-tid-785763-page-54.html";
            string s2 = "http://bbs.saraba1st.com/2b/read.php?tid=785763&page=54#20636585";
            string s3 = "http://bbs.saraba1st.com/2b/simple/?t785763_54.html";
            string s4 = "http://bbs.saraba1st.com/2b/read-htm-tid-785763.html";
            string s5 = "http://bbs.saraba1st.com/2b/read.php?tid=785763";
            string s6 = "http://bbs.saraba1st.com/2b/simple/?t785763.html";
            string expect = "?ID=785763&Page=32";
            string expect2 = "?ID=785763";
            Assert.AreEqual<string>(expect, S1Resource.GetThreadParamFromUrl(s0));
            Assert.AreEqual<string>(expect, S1Resource.GetThreadParamFromUrl(s1));
            Assert.AreEqual<string>(expect2, S1Resource.GetThreadParamFromUrl(s4));
            Assert.AreEqual<string>(expect, S1Resource.GetThreadParamFromUrl(s2));
            Assert.AreEqual<string>(expect2, S1Resource.GetThreadParamFromUrl(s5));
            Assert.AreEqual<string>(expect, S1Resource.GetThreadParamFromUrl(s3));
            Assert.AreEqual<string>(expect2, S1Resource.GetThreadParamFromUrl(s6));
        }
    }
}
