using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ImageTools.Controls;

namespace S1Nyan.Views
{
    public partial class SmartImage : UserControl
    {
        public SmartImage()
        {
            InitializeComponent();
            SizeChanged += (o, e) => SmartImageSizeChanged(e.NewSize);
            ImageHolder.DataContext = this;
            Unloaded += OnUnload;
        }

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            if (RealImageGif != null)
                RealImageGif.Stop();
        }

        private AnimatedImage RealImageGif;
        private Image RealImage;

        private ImageSourceProxy proxy = null;
        public ImageSourceProxy Proxy
        {
            get { return proxy; }
            set
            {
                if (proxy != null)
                {
                    proxy.DownloadProgressChanged -= LoadingProgress;
                    proxy.LoadingCompleted -= LoadingComplete;
                    imageArea.Content = null;
                }
                if (value != null)
                {
                    value.DownloadProgressChanged += LoadingProgress;
                    value.LoadingCompleted += LoadingComplete;
                    IsLoadingFailed = value.IsLoadingFailed;
                    if (!value.IsEmotion)
                        ImageHolder.Visibility = Visibility.Visible;
                }
                proxy = value;
            }
        }

        public bool IsLoadingFailed
        {
            get { return (bool)GetValue(IsLoadingFailedProperty); }
            set { SetValue(IsLoadingFailedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoadingFailed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingFailedProperty =
            DependencyProperty.Register("IsLoadingFailed", typeof(bool), typeof(SmartImage), new PropertyMetadata(false));

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

        private void LoadingComplete(object sender, EventArgs e)
        {
            if (Proxy == null) return;
            if (IsGif = Proxy.IsGif)
            {
                if (RealImageGif == null)
                    RealImageGif = new AnimatedImage { Stretch = Stretch.Uniform };
                imageArea.Content = RealImageGif;
                RealImageGif.Source = Proxy.GifImage;
            }
            else
            {
                if (RealImage == null)
                    RealImage = new Image { Stretch = Stretch.Uniform };
                imageArea.Content = RealImage;
                RealImage.Source = Proxy.Image;
            }
            ImageHolder.Visibility = Visibility.Collapsed;
            FailedFlag.Visibility = Visibility.Collapsed;
        }

        private void LoadingProgress(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            Percent = e.ProgressPercentage;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //SmartImageSizeChanged(availableSize);
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //SmartImageSizeChanged(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        void SmartImageSizeChanged(Size newSize)
        {
            if (Proxy == null) return;
#if S1Nyan
            double zoom = Proxy.IsEmotion ? (SettingView.ContentFontSize / 20.0) : 1.0;
#else 
            double zoom = Proxy.IsEmotion ? 1.5 : 1.0;
#endif
            double width = Proxy.PixelWidth * zoom;

            if (width == 0) return;

            if (Proxy.IsGif)
            {
                if (width < newSize.Width)
                    RealImageGif.Width = width;
                else
                    RealImageGif.Width = newSize.Width;
            }
            else 
            {
                if (width < newSize.Width)
                    RealImage.Width = width;
                else
                    RealImage.Width = newSize.Width;
            }
        }

        private void ImageHolder_Click(object sender, RoutedEventArgs e)
        {
            if (Proxy != null) Proxy.IsForceShow = true;
        }

    }
}
