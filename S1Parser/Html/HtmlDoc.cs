using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace S1Parser
{
    internal static class MatchTagExtension
    {
        static Regex attrib = new Regex("(?<key>[\\w-_]+)=(?<quote>['\"])(?<value>.*?)\\k<quote>");
        public static string TagName(this Match tag)
        {
            return tag != null ? tag.Groups["tag"].Value.ToLower() : null;
        }

        public static Dictionary<string, string> Attributes(this Match tag)
        {
            var result = new Dictionary<string, string>();
            foreach(Capture a in tag.Groups["attrib"].Captures)
            {
                var match = attrib.Match(a.Value);
                result[match.Groups["key"].Value.ToLower()] = match.Groups["value"].Value;
            }
            return result;
        }
    }

    public class HtmlDoc
    {
        class TempElement
        {
            internal Match Match { get; set; }
            internal List<HtmlElement> Children { get; set; }
            internal string Name { get { return Match == null ? null : Match.TagName(); } }
            internal Dictionary<string, string> Attributes { get { return Match == null ? null : Match.Attributes(); } }

            public override string ToString()
            {
                return string.Format("<{0}>: {1} children", Name, Children.Count);
            }
        }

        string rawhtml = null;

        public HtmlDoc(Stream data)
        {
            if (data == null) return;
            using (data) {
                using (var reader = new StreamReader(data)) {
                    rawhtml = reader.ReadToEnd();
                }
            }
        }

        public HtmlDoc(String rawtext)
        {
            rawhtml = rawtext;
        }
        
        public override string ToString()
        {
            return rawhtml;
        }

        HtmlElement rootElement;
        public HtmlElement RootElement
        {
            get
            {
                return rootElement ?? (rootElement = HtmlDoc.Parse(rawhtml));
            }
        }

        static Regex _tag_pattern = new Regex("</?(?<tag>\\w+)(?<attrib>\\s+(?<key>[\\w-_]+)=(?<quote>['\"])(?<value>.*?)\\k<quote>)*\\s*/?>");
        static Regex _space_pattern = new Regex(@"\A\s*\z");
        static public HtmlElement Parse(string html)
        {
            if (html == null) return null;

            int offset = 0;

            var matchStack = new Stack<TempElement>();
            matchStack.Push(new TempElement { Children =  new List<HtmlElement>() });

            while (true)
            {
                var match = _tag_pattern.Match(html, offset);

                if (!match.Success)
                {   //no tags any more, add remain text and return
                    if (html.Length - 1 > offset)
                    {
                        var text = html.Substring(offset, html.Length - offset);
                        var matchSpace = _space_pattern.Match(text);
                        if (!matchSpace.Success)
                            matchStack.Peek().Children.Add(
                                new HtmlElement(type: HtmlElementType.Text,
                                           innerHtml: text));
                    }
                    break;
                }

                if (match.Index != offset)
                {   //text before tags, add as previous's child
                    var text = html.Substring(offset, match.Index - offset);
                    var matchSpace = _space_pattern.Match(text);
                    if (!matchSpace.Success)    //ignore spaces
                    {
                        matchStack.Peek().Children.Add(
                            new HtmlElement(type: HtmlElementType.Text, innerHtml: text));
                    }
                }
                if (match.Value.StartsWith("</"))
                {   //tag ending found, seach for corresponding tag beginning
                    if (ContainsMatch(matchStack, match))
                    {   //mismatch </tag> just ignored
                        while (matchStack.Count > 1) 
                        {
                            var top = matchStack.Pop();
                            var e = new HtmlElement(top.Name,
                                attributes: top.Attributes);
                            matchStack.Peek().Children.Add(e);

                            if (top.Name == match.TagName())
                            {
                                e.Children = top.Children;
                                break; // break inner while
                            }                           
                            else 
                            {   //mismatch <tag> ==> <tag />
                                e.Type = HtmlElementType.SelfClosed;
                                matchStack.Peek().Children.AddRange(top.Children);
                            }
                        }
                    }
                }
                else if (match.Value.EndsWith("/>"))
                {   // self close tag
                    var e = new HtmlElement(
                        match.TagName(),
                        attributes:match.Attributes(),
                        type:HtmlElementType.SelfClosed);

                    matchStack.Peek().Children.Add(e);
                }
                else
                {
                    matchStack.Push(new TempElement { Match = match, Children = new List<HtmlElement>() });
                }
                offset = match.Index + match.Length;
            }

            //final task
            HtmlElement tempElement = null;
            while (matchStack.Count > 1)
            {
                var top = matchStack.Pop();
                if (tempElement != null)
                    top.Children.Add(tempElement);

                tempElement = new HtmlElement(
                    top.Name,
                    top.Attributes,
                    top.Children);
            }
            var result = new HtmlElement(children: matchStack.Pop().Children);
            if (tempElement != null)
                result.Children.Add(tempElement);
            if (result.Children.Count == 1)
                result = result.Children[0];

            return result;
        }

        private static bool ContainsMatch(Stack<TempElement> matchStack, Match match)
        {
            foreach (var item in matchStack)
                if (item.Name == match.TagName())
                    return true;
            return false;
        }
    }
}
