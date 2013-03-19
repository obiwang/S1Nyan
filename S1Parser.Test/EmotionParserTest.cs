using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using S1Parser.Emotion;

namespace S1Parser.Test
{
    [TestClass]
    public class EmotionParserTest
    {

        const string test = @"
<div class='face_main'>
    <ul id='face_main' class='cc'>
        <li><a onclick='PwFace.addsmile(27);return false;'>
            <img alt=' ' src='face/00.gif'></a></li>
        <li><a onclick='PwFace.addsmile(147);return false;'>
            <img alt=' ' src='face/153.gif'></a></li>
        <li><a onclick='PwFace.addsmile(149);return false;'>
            <img alt=' ' src='face/119.gif'></a></li>
        <li><a onclick='PwFace.addsmile(132);return false;'>
            <img alt=' ' src='face/90.gif'></a></li>
        <li><a onclick='PwFace.addsmile(133);return false;'>
            <img alt=' ' src='face/109.gif'></a></li>
        <li><a onclick='PwFace.addsmile(145);return false;'>
            <img alt=' ' src='face/59.gif'></a></li>
    </ul>
    <div class='face_pages cc' id='face_page'></div>
</div>";
        [TestMethod]
        public void TestInit()
        {
            EmotionParser.Init(test);
            var list = EmotionParser.EmotionList;
            Assert.AreEqual<string>("face/59.gif", list["145"].Path);
            Assert.AreEqual<string>("face/00.gif", list["27"].Path);
        }
    }
}
