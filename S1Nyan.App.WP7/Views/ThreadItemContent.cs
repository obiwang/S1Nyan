using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Phone.Tasks;
using S1Parser;

namespace S1Nyan.Views
{
    public class FontsizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int count = value.ToString().Length;
            double size = 28;
            switch (count)
            {
                case 1:
                case 2:
                    break;
                case 3:
                    size = 24;
                    break;
                case 4:
                    size = 18;
                    break;
                default:
                    size = 15;
                    break;
            }
            return size;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ContentConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is HtmlElement)
            {
                return ThreadItemContent.BuildParagraph((HtmlElement)value);
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ThreadItemContent : ContentControl
    {
        public ThreadItemContent()
        {
            FontSize = SettingView.ContentFontSize;
            Loaded += ViewLoaded;
        }

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {
            FontSize = SettingView.ContentFontSize;
        }

        public static FrameworkElement BuildParagraph(HtmlElement paragraph)
        {
            if (paragraph.Descendants().Count() == 1)
            {
                var e = paragraph.Element();

                if (e.Type == HtmlElementType.Text)
                    return new TextBlock { Text = HttpUtility.HtmlDecode(paragraph.InnerHtml.Trim()), TextWrapping = TextWrapping.Wrap };
                switch (e.Name)
                {
                    case "br":
                        return new TextBlock();
                    case "img":
                        return BuildImgControl(e);
                    case "blockquote":
                        if (e.Element("div") != null)
                            return BuildQuote(e);
                        break;
                    default:
                        break;
                }

            }
            RichTextBox r = new RichTextBox { FontSize = SettingView.ContentFontSize, Margin = new Thickness(-12, 0, -12, 0) };
            r.Blocks.Add(BuildRichText(paragraph));
            return r;
        }

        private static Paragraph BuildRichText(HtmlElement content)
        {
            var p = new Paragraph();
            if (content == null) return p;

            foreach (var item in content.Descendants())
            {
                p.Inlines.Add(BuildInlines(item));
            }

            return p;
        }

        private static Inline BuildInlines(HtmlElement item)
        {
            Span span = new Span();
            if (item.Type == HtmlElementType.Text)
            {
                var text = item.InnerHtml;//.Trim();
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
                default:
                    break;
            }
            foreach (var e in item.Descendants())
            {
                span.Inlines.Add(BuildInlines(e));
            }

            return span;
        }

        private static FrameworkElement BuildQuote(HtmlElement item)
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(26, 12, 0, 12);

            var rect = new System.Windows.Shapes.Rectangle();
            rect.StrokeDashArray = new DoubleCollection { 4, 6 };
            rect.Stroke = new SolidColorBrush(Colors.Gray);
            rect.Stretch = Stretch.Fill;
            grid.Children.Add(rect);

            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(12);
            grid.Children.Add(panel);

            SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
            foreach (var div in item.Descendants())
            {
                if ("quote" == div.Attributes["class"])
                {
                    var text = new TextBlock { Text = div.InnerHtml };
                    text.Foreground = gray;
                    text.FontStyle = FontStyles.Italic;
                    panel.Children.Add(text);
                }
                else if ("text" == div.Attributes["class"])
                {
                    //foreach (var e in div.Descendants())
                    {
                        FrameworkElement p = BuildParagraph(div);
                        if (p is TextBlock) (p as TextBlock).Foreground = gray;
                        if (p is RichTextBox) (p as RichTextBox).Foreground = gray;
                        panel.Children.Add(p);
                    }
                }
            }
            return grid;
        }

        private static Inline BuildLink(HtmlElement item)
        {
            Hyperlink link = new Hyperlink();
            var url = item.Attributes["href"];
            var aText = item.PlainText();
            if (url == null || aText.Length == 0) return link;

            if (url.ToLower().StartsWith("mailto:"))
            {
                link.Click += (o, e) =>
                {
                    EmailComposeTask emailComposeTask = new EmailComposeTask();

                    emailComposeTask.To = url.Substring(url.IndexOf(':') + 1);

                    emailComposeTask.Show();
                };
            }

            S1Resource.GetAbsoluteUrl(ref url);
            var viewParam = S1Resource.GetThreadParamFromUrl(url);
            if (viewParam != null)
            {
                Run header = new Run();
                header.Text = "<S1: ";
                header.FontStyle = FontStyles.Italic;
                link.Inlines.Add(header);
                link.Inlines.Add(aText);
                link.Inlines.Add(" >");
                link.NavigateUri = new Uri("/Views/ThreadView.xaml" + viewParam, UriKind.Relative);
            }
            else
            {
                try
                {
                    link.NavigateUri = new Uri(url);
                    link.TargetName = "_blank";
                }
                catch (Exception) { };
                link.Inlines.Add(aText);
            }
            link.FontSize = SettingView.ContentFontSize * .85;
            link.Foreground = (Brush)Application.Current.Resources["PhoneAccentBrush"];

            return link;
        }

        private static InlineUIContainer BuildImg(HtmlElement item)
        {
            InlineUIContainer container = new InlineUIContainer();

            var img = BuildImgControl(item);
            if (img != null)
                container.Child = img;
            return container;
        }

        private static SmartImage BuildImgControl(HtmlElement item)
        {
            var url = Uri.UnescapeDataString(item.Attributes["src"]);
            if (url == null) return null;

            S1Resource.GetAbsoluteUrl(ref url);
            bool isEmotion = S1Resource.IsEmotion(url);

            var image = new SmartImage ();
            ImageResourceManager.SetUriSource(image, url);
            if (isEmotion)
                image.Margin = new Thickness(0, 0, 0, -6);
            else
                image.Margin = new Thickness(6);
            return image;
        }
    }
}
