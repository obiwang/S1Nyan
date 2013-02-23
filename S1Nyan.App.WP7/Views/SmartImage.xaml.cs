using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageTools;
using ImageTools.Controls;
using Microsoft.Practices.ServiceLocation;
using S1Nyan.Model;
using S1Parser;

namespace S1Nyan.App.Views
{
    public partial class SmartImage : UserControl
    {
        public SmartImage()
        {
            InitializeComponent();
            SizeChanged += SmartImage_SizeChanged;
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
            imageStream = null;
            if (UriSource.ToLower().EndsWith(".gif"))
            {
                IsGif = true;
            }
            else
                IsGif = false;

            string path = null;
            if (null != (path = S1Resource.GetEmotionPath(UriSource)))
            {
                var res = Application.GetResourceStream(new Uri("Resources/" + path, UriKind.Relative));
                if (res != null)
                    imageStream = res.Stream;
                else
                    imageStream = await NetResourceService.GetResourceStreamStatic(new Uri(UriSource), path, -1);
            }
            Percent = 0;
            ShowImage();
        }

        protected void ShowImage()
        {
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
                    image.ImageFailed += (o, e) => ImageDecodeFailed();
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
            catch (Exception) { }
        }

        private void ImageDecodeFailed()
        {
            if (!IsGif && image != null && S1Resource.IsEmotion(UriSource))
            {   //decode jpg/png failed, try gif
                IsGif = true;
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

        void SmartImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsGif && gifImage != null && gifImage.PixelWidth > 0)
            {
                if (gifImage.PixelWidth < e.NewSize.Width)
                {
                    RealImageGif.Width = gifImage.PixelWidth;
                    RealImageGif.Stretch = Stretch.None;
                }
                else
                {
                    RealImageGif.Stretch = Stretch.Uniform;
                }
            }
            else if (image != null && image.PixelWidth > 0)
            {
                if (image.PixelWidth < e.NewSize.Width)
                {
                    RealImage.Width = image.PixelWidth;
                    RealImage.Stretch = Stretch.None;
                }
                else
                {
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
