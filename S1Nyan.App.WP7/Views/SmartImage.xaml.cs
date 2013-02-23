using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageTools;
using ImageTools.Controls;
#if S1Nyan
using Microsoft.Practices.ServiceLocation;
using S1Nyan.Model;
using S1Parser;
#endif

namespace S1Nyan.App.Views
{
    public partial class SmartImage : UserControl
    {
        public SmartImage()
        {
            InitializeComponent();
            SizeChanged += (o, e) => SmartImageSizeChanged(e.NewSize);
            ImageHolder.DataContext = this;
        }

        BitmapImage image;
        ExtendedImage gifImage;
        Stream imageStream;

        public int Percent
        {
            get { return (int)GetValue(PercentProperty); }
            set { SetValue(PercentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Percent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentProperty =
            DependencyProperty.Register("Percent", typeof(int), typeof(SmartImage), new PropertyMetadata(0));

        public bool IsGif
        {
            get { return (bool)GetValue(IsGifProperty); }
            set { SetValue(IsGifProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsGif.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsGifProperty =
            DependencyProperty.Register("IsGif", typeof(bool), typeof(SmartImage), new PropertyMetadata(false));

        public string UriSource
        {
            get { return (string)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UriSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(string), typeof(SmartImage), new PropertyMetadata(null, OnSourceChanged));


        public bool IsAutoDownload
        {
            get { return (bool)GetValue(IsAutoDownloadProperty); }
            set { SetValue(IsAutoDownloadProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAutoDownload.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAutoDownloadProperty =
            DependencyProperty.Register("IsAutoDownload", typeof(bool), typeof(SmartImage), new PropertyMetadata(false, OnIsAutoDownloadChanged));

        private static void OnIsAutoDownloadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SmartImage).OnIsAutoDownloadChanged();
        }

        private void OnIsAutoDownloadChanged()
        {
            if (ImageHolder.Visibility == Visibility.Visible && IsAutoDownload)
                ShowImage();
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SmartImage).OnSourceChanged();
        }

        protected async void OnSourceChanged()
        {
            if (UriSource == null) return;

            imageStream = null;
            if (UriSource.ToLower().EndsWith(".gif"))
            {
                IsGif = true;
            }
            else
                IsGif = false;

#if S1Nyan
            string path = null;
            if (null != (path = S1Resource.GetEmotionPath(UriSource)))
            {
                var res = Application.GetResourceStream(new Uri("Resources/" + path, UriKind.Relative));
                if (res != null)
                    imageStream = res.Stream;
                else
                    imageStream = await NetResourceService.GetResourceStreamStatic(new Uri(UriSource), path, -1);
            }
#endif
            Percent = 0;
            ShowImage();
        }

        protected void ShowImage()
        {
            if (UriSource == null) return;
            if (!IsAutoDownload) return;

            try
            {
                if (!IsGif)
                {   //try decode jpg/png
                    if (imageStream != null)
                    {
                        image = new BitmapImage();
                        image.SetSource(imageStream);
                        ImageDownloadComplete();
                    }
                    else
                    {
                        image = new BitmapImage(new Uri(UriSource));
                        image.DownloadProgress += (o, e) => ImageDownloadProgress(e.Progress);
                        image.ImageOpened += (o, e) => ImageDownloadComplete();
                    }
                    RealImage.Source = image;
                    RealImage.Visibility = Visibility.Visible;
                }
                else
                {
                    if (imageStream != null)
                    {
                        gifImage = new ExtendedImage();
                        gifImage.SetSource(imageStream);
                        ImageDownloadComplete();
                    }
                    else
                    {
                        gifImage = (ExtendedImage)(new ImageConverter().Convert(new Uri(UriSource), typeof(Uri), null, null));
                        gifImage.DownloadProgress += (o, e) => ImageDownloadProgress(e.ProgressPercentage);
                        gifImage.DownloadCompleted += (o, e) => ImageDownloadComplete();
                    }
                    RealImageGif.Source = gifImage;
                    RealImageGif.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex) {
                ImageDecodeFailed();
            }
        }

        private void ImageDecodeFailed()
        {
            if (!IsGif && image != null)
#if S1Nyan
                if (S1Resource.IsEmotion(UriSource))  //apply to emotion pics only
#endif
                {   //decode jpg/png failed, try gif
                    IsGif = true;
                    if (imageStream != null)
                        imageStream.Seek(0, SeekOrigin.Begin);
                    ShowImage();
                }
        }

        private void ImageDownloadComplete()
        {
            ImageHolder.Visibility = Visibility.Collapsed;
        }

        private void ImageDownloadProgress(int percent)
        {
            Percent = percent;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            SmartImageSizeChanged(availableSize);
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            SmartImageSizeChanged(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        void SmartImageSizeChanged(Size newSize)
        {
            if (IsGif && gifImage != null && gifImage.PixelWidth > 0)
            {
                if (gifImage.PixelWidth < newSize.Width)
                {
                    RealImageGif.Width = gifImage.PixelWidth;
                    RealImageGif.Stretch = Stretch.None;
                }
                else
                {
                    RealImageGif.Width = newSize.Width;
                    RealImageGif.Stretch = Stretch.Uniform;
                }
            }
            else if (image != null && image.PixelWidth > 0)
            {
                if (image.PixelWidth < newSize.Width)
                {
                    RealImage.Width = image.PixelWidth;
                    RealImage.Stretch = Stretch.None;
                }
                else
                {
                    RealImage.Width = newSize.Width;
                    RealImage.Stretch = Stretch.Uniform;
                }
            }
        }

        private void ImageHolder_Click(object sender, RoutedEventArgs e)
        {
            IsAutoDownload = true;
        }

    }
}
