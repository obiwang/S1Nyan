using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace S1Parser
{
    public enum HtmlElementType
    {
        Tag = 0,
        Text,
        SelfClosed,
    }

    public class HtmlElement
    {
        public string Name { get; private set;}
        public Dictionary<string, string> Attributes { get; internal set; }
        internal List<HtmlElement> Children { get; set; }
        public HtmlElementType Type { get; internal set; }

        private string _innerHtml;
        public string InnerHtml
        {
            get { return _innerHtml ?? GetInnerHtml(); }
            private set { _innerHtml = value; }
        }

        public HtmlElement(string name = null, Dictionary<string, string> attributes = null, List<HtmlElement> children = null, string innerHtml = null, HtmlElementType type = 0)
        {
            Name = name;
            try
            {
                InnerHtml = WebUtility.HtmlDecode(innerHtml);
            }
            catch (Exception)
            {
                InnerHtml = innerHtml;
            }
            
            Attributes = attributes;
            Children = children ?? new List<HtmlElement>();
            Type = type;
        }

        /// <summary>
        /// Get first child element named tagName
        /// </summary>
        /// <param name="tagName">Html tag name</param>
        /// <returns></returns>
        public HtmlElement Element(string tagName = null)
        {
            if (tagName == null)
                return Children.Count > 0 ? Children[0] : null;
 
            foreach (var e in Children)
            {
                if (e.Name == tagName.ToLower())
                    return e;
            }
            return null;
        }

        /// <summary>
        /// Get all child elements
        /// </summary>
        /// <param name="tagName">Html tag name</param>
        /// <returns></returns>
        public IEnumerable<HtmlElement> Descendants(string tagName = null)
        {
            foreach (var e in Children)
            {
                if (tagName == null || e.Name == tagName.ToLower())
                    yield return e;
            }
        }

        /// <summary>
        /// Find Element recursively
        /// </summary>
        /// <param name="tagName">Html tag name</param>
        /// <returns></returns>
        public IEnumerable<HtmlElement> FindElements(string tagName = null)
        {
            foreach (var e in Children)
            {
                if (tagName == null || e.Name == tagName.ToLower())
                    yield return e;
                foreach(var ee in e.FindElements(tagName))
                    yield return ee;
            }
        }

        public HtmlElement FindFirst(string tagName = null)
        {
            foreach (var e in Children)
            {
                if (tagName == null || e.Name == tagName.ToLower())
                    return e;
                var ee = e.FindFirst(tagName);
                if (ee != null)
                    return ee;
            }
            return null;
        }

        #region string functions
        private string GetInnerHtml()
        {
            StringBuilder result = new StringBuilder();
            foreach (var e in Children)
                result.Append(e.FormatString(withFormat:false));

            return result.ToString();
        }

        private void AppendTag(StringBuilder result, int level, bool withFormat, bool IsEnd = false)
        {
            if (withFormat)
                for (int i = 0; i < level; i++) result.Append("    ");
            if (Type != HtmlElementType.Text) result.Append('<');
            if (IsEnd) result.Append('/');
            if (Type != HtmlElementType.Text) result.Append(Name);
            if (Attributes != null)
                foreach (var a in Attributes)
                    result.Append(string.Format(" {0}=\"{1}\"", a.Key, a.Value));
            if (Type == HtmlElementType.Text) result.Append(InnerHtml);
            if (Type == HtmlElementType.SelfClosed) result.Append(" /");
            if (Type != HtmlElementType.Text) result.Append('>');
            if (withFormat) result.Append("\r\n");
        }

        string FormatString(int level = 0, bool withFormat = true, StringBuilder pre = null)
        {
            StringBuilder result = pre ?? new StringBuilder();
      
            if (Type != HtmlElementType.Tag)
                AppendTag(result, level, withFormat);
            else
            {
                if (Name != null)
                {
                    if (Children.Count > 1 || (Children.Count == 1 && Children[0].Type != HtmlElementType.Text))
                    {
                        AppendTag(result, level, withFormat);
                        foreach (var e in Children)
                            e.FormatString(level + 1, withFormat, result);
                        AppendTag(result, level, withFormat, true);
                    }
                    else
                    {
                        if (withFormat)
                            for (int i = 0; i < level; i++) result.Append("    ");
                        AppendTag(result, level, false);
                        result.Append(InnerHtml);
                        AppendTag(result, level, false, true);
                        if (withFormat)
                            result.Append("\r\n");
                    }
                }
                else
                    foreach (var e in Children)
                        e.FormatString(level, withFormat, result);
            }

            //return only at root level
            return level == 0 ? result.ToString() : null;
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool withFormat)
        {
            return FormatString(withFormat: withFormat);
        }
        #endregion

        public string PlainText(StringBuilder result = null)
        {
            if(result == null) 
                result = new StringBuilder();

            foreach (var e in Children)
            {
                if (e.Type == HtmlElementType.Text)
                    result.Append(RemoveLineReturns(e.InnerHtml));
                else if (e.Name == "br")
                    result.Append("\r\n");
                e.PlainText(result);
            }
            return result.ToString();
        }

        static Regex lr = new Regex(@"[\r\n]");
        public static string RemoveLineReturns(string s)
        {
            return lr.Replace(s, "");
        }
    }
}
