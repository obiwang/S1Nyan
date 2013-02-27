using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using S1Nyan.App.Utils;
using S1Parser;

namespace S1Nyan.App.Views
{
    public class FontsizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int count = value.ToString().Length;
            double size = 28;
            switch(count)
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

    public partial class ThreadItemView : UserControl, IDataContextChangedHandler<ThreadItemView>
    {
        private bool isLoaded;
        public ThreadItemView()
        {
            InitializeComponent();
            DataContextChangedHelper<ThreadItemView>.Bind(this);
            Loaded += ViewLoaded;
        }

        public void OnDataContextChanged(ThreadItemView sender, DependencyPropertyChangedEventArgs e)
        {
            InitContent();
        }

        private void InitContent()
        {
            if (!isLoaded) return;
            S1ThreadItem data = DataContext as S1ThreadItem;

            content.Blocks.Clear();
            if (data != null)
            {
                content.FontSize = SettingView.ContentFontSize;
                var p = new Paragraph();
                p.Inlines.Add(No.Text);
                //content.Blocks.Add(p);
                //return;
                content.Blocks.Add(BuildRichText(data.Content));
            }
        }

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;
            InitContent();
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
            RichTextBox r = new RichTextBox { FontSize = SettingView.ContentFontSize, TextWrapping = TextWrapping.Wrap };
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
            var url = item.Attributes["href"];
            var aText = item.PlainText();
            if (url == null || aText.Length == 0) return link;

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
                link.NavigateUri = new Uri(url);
                link.TargetName = "_blank";
                link.Inlines.Add(aText);
            }

            link.Foreground = (Brush)Application.Current.Resources["PhoneAccentBrush"];

            return link;
        }

        private static InlineUIContainer BuildImg(HtmlElement item)
        {
            InlineUIContainer container = new InlineUIContainer();
            var url = item.Attributes["src"];
            if (url == null) return container;

            S1Resource.GetAbsoluteUrl(ref url);
            bool isEmotion = S1Resource.IsEmotion(url);

            //order matters! set IsAutoDownload first;
            var image = new SmartImage { IsAutoDownload = isEmotion, UriSource = url };
            if (isEmotion) 
                image.Margin = new Thickness(0, 0, 0, -4);
            else 
                image.Margin = new Thickness(6);
            container.Child = image;
            return container;
        }

    }
}
