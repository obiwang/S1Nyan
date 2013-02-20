using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ImageTools;
using ImageTools.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using S1Nyan.App.Resources;
using S1Nyan.ViewModel;
using S1Parser;

namespace S1Nyan.App.Views
{
    public partial class ThreadPage : PhoneApplicationPage
    {
        public ThreadPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public ThreadViewModel Vm
        {
            get
            {
                return (ThreadViewModel)DataContext;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back) return;

            string idParam, titleParam = null;
            if (NavigationContext.QueryString.TryGetValue("ID", out idParam))
            {
                NavigationContext.QueryString.TryGetValue("Title", out titleParam);
                Vm.OnChangeTID(idParam, HttpUtility.HtmlDecode(titleParam));
            }
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.sync.rest.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarButtonRefresh;
            appBarButton.Click += (o, e) => Vm.RefreshThread();
            ApplicationBar.Buttons.Add(appBarButton);

            ApplicationBarIconButton preBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.back.rest.png", UriKind.Relative));
            preBarButton.Text = AppResources.AppBarButtonPrePage;
            preBarButton.Click += (o, e) => Vm.CurrentPage--;
            ApplicationBar.Buttons.Add(preBarButton);

            ApplicationBarIconButton nextBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.next.rest.png", UriKind.Relative));
            nextBarButton.Text = AppResources.AppBarButtonNextPage;
            nextBarButton.Click += (o, e) => Vm.CurrentPage++;
            ApplicationBar.Buttons.Add(nextBarButton);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            ApplicationBar.MenuItems.Add(appBarMenuItem);

            preBarButton.IsEnabled = false;
            nextBarButton.IsEnabled = false;

            Vm.PageChanged = (current, total) =>
            {
                if (current > 1 && total > 1) preBarButton.IsEnabled = true;
                else preBarButton.IsEnabled = false;
                if (current < total && total > 1) nextBarButton.IsEnabled = true;
                else nextBarButton.IsEnabled = false;
            };
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
                return new Run { Text = HttpUtility.HtmlDecode(text)};
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
                    span.FontWeight = FontWeights.Bold;
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
            RichTextBox r = new RichTextBox { FontSize = 25, TextWrapping = TextWrapping.Wrap};
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
                GetAbsoluteUrl(ref url);
                link.NavigateUri = new Uri(url);
            }
            link.TargetName = "_blank";
            link.Foreground = (Brush) Application.Current.Resources["PhoneAccentBrush"];

            return link;
        }

        /// <summary>
        /// Get the absolute url if not
        /// </summary>
        /// <param name="url"></param>
        /// <returns>true if original url is absolute</returns>
        private static bool GetAbsoluteUrl(ref string url)
        {
            if (!url.ToLower().StartsWith("http://"))
            {
                url = "http://bbs.saraba1st.com/2b/" + url;
                return false;
            }
            return true;
        }

        private static InlineUIContainer BuildImg(HtmlElement item)
        {
            InlineUIContainer container = new InlineUIContainer();
            var url = item.Attributes["src"];
            if (url == null) return container;
            var image = new Image();
            if (!GetAbsoluteUrl(ref url))
            {
                if (url.ToLower().EndsWith("gif"))
                {
                    AnimatedImage ani = new AnimatedImage();
                    ani.Stretch = Stretch.None;
                    ani.AnimationMode = AnimationMode.Repeat;
                    ani.Source = (ExtendedImage)(new ImageConverter().Convert(new Uri(url), typeof(Uri), null, null));
                    container.Child = ani;
                    return container;
                }
            }
            BitmapImage bitmap = new BitmapImage(new Uri(url));
            image.Source = bitmap;
            image.MaxWidth = 480;
            bitmap.ImageOpened += (o, e) =>
            {
                if (bitmap.PixelWidth < 480)
                    image.Stretch = Stretch.None;
                else
                {   //8.0 only
                    //bitmap.DecodePixelWidth = 480;
                    image.Stretch = Stretch.UniformToFill;
                }
            };
            container.Child = image;
            return container;
        }

    }
}