using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace S1Parser.Emotion
{
    public class EmotionItemViewModel : INotifyPropertyChanged
    {
        public string Path { get; set; }
        public string Id { get; set; }
        public bool IsValid { get { return Id != null && Id.Length > 0; } }

        public string ImagePath
        {
            get { return S1Resource.EmotionBase + Path; }
        }

        public void NotifyChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ImagePath"));
                PropertyChanged(this, new PropertyChangedEventArgs("Id"));
                PropertyChanged(this, new PropertyChangedEventArgs("IsValid"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EmotionItemViewModel Clone()
        {
            return new EmotionItemViewModel { Id = this.Id, Path = this.Path };
        }
    }

    public static class EmotionParser
    {
        public static Dictionary<string, EmotionItemViewModel> EmotionList { get; private set; }

        private static bool IsInited
        {
            get
            {
                if (EmotionList == null)
                    EmotionList = new Dictionary<string, EmotionItemViewModel>();
                else
                    if (EmotionList.Count > 0) return true;
                return false;
            }
        }

        public static void Init(string data)
        {
            if (IsInited) return;
            ParseEmotion(new HtmlDoc(data).RootElement);
        }

        public static void Init(Stream data)
        {
            if (IsInited) return;
            ParseEmotion(new HtmlDoc(data).RootElement);
        }

        public static void InitFromXml(Stream xml)
        {
            if (IsInited) return;
            ParseEmotionXml(xml);
        }

        private static void ParseEmotionXml(Stream xml)
        {
            var root = XDocument.Load(xml).Root;
            foreach (var item in root.Descendants("img"))
            {
                var id = item.Attribute("Id").Value;
                var path = item.Attribute("Path").Value;
                EmotionList[id] = new EmotionItemViewModel { Id = id, Path = path }; 
            }
        }

/*
<div class="face_main">
    <ul id="face_main" class="cc">
        <li><a onclick="PwFace.addsmile(27);return false;">
            <img alt=" " src="face/00.gif"></a></li>
         * */
        static Regex smilePattern = new Regex(@"addsmile\((?<id>\d+)\)");
        private static void ParseEmotion(HtmlElement root)
        {
            var list = root.Element("ul");
            if (list == null) return;

            foreach (var li in list.Descendants("li"))
            {
                HtmlElement a = li.Element("a");
                HtmlElement img = null;
                if (a != null &&
                    (img = a.Element("img")) != null)
                {
                    var onclick = a.Attributes["onclick"];
                    var id = "";
                    var match = smilePattern.Match(onclick);
                    if (match.Success)
                        id = match.Groups["id"].Value;
                    var path = img.Attributes["src"];
                    if (id.Length > 0 && path.Length > 0)
                        EmotionList[id] = new EmotionItemViewModel { Id = id, Path = path };
                }
            }
        }

    }
}
