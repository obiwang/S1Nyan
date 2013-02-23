using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using S1Parser;

namespace S1Nyan.App.Views
{
    public partial class ThreadItemView : UserControl
    {
        public ThreadItemView()
        {
            InitializeComponent();
            Loaded += ViewLoaded;
        }

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {
            S1ThreadItem data = DataContext as S1ThreadItem;
            if (content.Blocks.Count == 0 && data != null)
            {
                content.Blocks.Add(BuildRichText(data.Content));
            }
        }

        private static Paragraph BuildRichText(HtmlElement content)
        {
            var p = new Paragraph();
            if (content == null) return p;

            foreach (var item in content.Descendants())
            {
                if (item.Name == "h1" && p.Inlines.Count == 0) continue; //ignore headline

                p.Inlines.Add(BuildInlines(item));
            }

            return p;
        }

        private static Inline BuildInlines(HtmlElement item)
        {
            Span span = new Span();
            if (item.Type == HtmlElementType.Text)
            {
                var text = item.InnerHtml.Trim();
                return new Run { Text = HttpUtility.HtmlDecode(text) };
            }
            switch (item.Name)
            {
                case "br":
                    return new LineBreak();
                case "img":
                    return BuildImg(item);
                case "a":
                    return BuildLink(item);
                case "b":
                    span.FontWeight = FontWeights.ExtraBold;
                    break;
                case "i":
                    span.FontStyle = FontStyles.Italic;
                    break;
                case "blockquote":
                    if (item.Element("div") != null)
                        return BuildQuote(item);
                    break;
                default:
                    break;
            }
            foreach (var e in item.Descendants())
            {
                span.Inlines.Add(BuildInlines(e));
            }

            return span;
        }

        private static Inline BuildQuote(HtmlElement item)
        {
            InlineUIContainer container = new InlineUIContainer();
            RichTextBox r = new RichTextBox { FontSize = 25, TextWrapping = TextWrapping.Wrap };
            r.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid grid = new Grid();
            grid.Margin = new Thickness(26, 12, 0, 12);
            var rect = new System.Windows.Shapes.Rectangle();
            rect.StrokeDashArray = new DoubleCollection { 4, 6 };
            rect.Stroke = new SolidColorBrush(Colors.Gray);
            rect.Stretch = Stretch.Fill;
            grid.Children.Add(rect);
            grid.Children.Add(r);
            container.Child = grid;

            foreach (var div in item.Descendants())
            {
                if ("quote" == div.Attributes["class"])
                {
                    var p = new Paragraph();
                    Italic italic = new Italic();
                    italic.Inlines.Add(div.InnerHtml);
                    italic.FontSize = 23;
                    p.Inlines.Add(italic);
                    r.Blocks.Add(p);
                }
                else if ("text" == div.Attributes["class"])
                {
                    var p = BuildRichText(div);
                    p.Foreground = new SolidColorBrush(Colors.Gray);
                    r.Blocks.Add(p);
                }
            }
            return container;
        }

        private static Inline BuildLink(HtmlElement item)
        {
            Hyperlink link = new Hyperlink();
            link.Inlines.Add(item.PlainText());
            var url = item.Attributes["href"];
            if (url != null)
            {
                S1Resource.GetAbsoluteUrl(ref url);
                link.NavigateUri = new Uri(url);
            }
            link.TargetName = "_blank";
            link.Foreground = (Brush)Application.Current.Resources["PhoneAccentBrush"];

            return link;
        }

        private static InlineUIContainer BuildImg(HtmlElement item)
        {
            InlineUIContainer container = new InlineUIContainer();
            var url = item.Attributes["src"];
            if (url == null) return container;
            bool isEmotion = false;
            if (!S1Resource.GetAbsoluteUrl(ref url))
            {   //image is on S1
                isEmotion = S1Resource.IsEmotion(url);
            }
            //order matters! set IsAutoDownload first;
            var image = new SmartImage { IsAutoDownload = isEmotion, UriSource = url };
            if (isEmotion) image.Margin = new Thickness(0, 0, 0, -4);
            else image.Margin = new Thickness(12);
            container.Child = image;
            return container;
        }
    }
}
