using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ImageTools;
using S1Nyan.Model;
using S1Parser;

namespace S1Nyan.Views
{
    public class ImageSourceProxy : IDisposable
    {
        public ImageSourceProxy()
        {
            client = new WebClient();
        }
        #region filed
        private WebClient client = null;
        private List<SmartImage> smartImageList = new List<SmartImage>();
        #endregion

        #region Properties
        public bool IsGif { get; set; }

        public bool IsEmotion { get; set; }

        private bool isLoadingFailed = false;
        public bool IsLoadingFailed
        {
            get { return isLoadingFailed; }
            set
            {
                isLoadingFailed = value;
                foreach (var image in smartImageList)
                {
                    image.IsLoadingFailed = value;
                }
            }
        }

        public BitmapImage Image { private set; get; }

        public ExtendedImage GifImage { private set; get; }

        private string sourceUrl;

        public string SourceUrl
        {
            get { return sourceUrl; }
            set
            {
                if (value == null) return;
                sourceUrl = value;
                if (sourceUrl.ToLower().EndsWith(".gif"))
                    IsGif = true;
                else
                    IsGif = false;

                PrepareImage();
            }
        }

        private bool isForceShow = false;
        public bool IsForceShow
        {
            get { return isForceShow; }
            set
            {
                isForceShow = value;
                if (!client.IsBusy)
                    PrepareImage();
            }
        }

        public int PixelWidth
        {
            get
            {
                if (Image != null) return Image.PixelWidth;
                else if (GifImage != null) return GifImage.PixelHeight;
                else return 0;
            }
        }

        public int PixelHeight
        {
            get
            {
                if (Image != null) return Image.PixelHeight;
                else if (GifImage != null) return GifImage.PixelHeight;
                else return 0;
            }
        }

        public Stream SourceStream { get; set; }
        
        #endregion

        #region Events
        // Summary:
        //     Occurs when the download progress changed.
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        // Summary:
        //     Occurs when the download is completed.
        public event EventHandler LoadingCompleted;

        #endregion

        private async void PrepareImage()
        {
            string path = null;
            if (null != (path = S1Resource.GetEmotionPath(SourceUrl)))
            {
                IsEmotion = true;

                var res = Application.GetResourceStream(new Uri("Resources/" + path, UriKind.Relative));

                try
                {
                    if (res != null)
                        SourceStream = res.Stream;
                    else
                        SourceStream = await NetResourceService.GetResourceStreamStatic(new Uri(SourceUrl), path, -1, false);
                }
                catch { }
                finally
                {
                    if (SourceStream == null) OnImageOpenFailed();
                }

                UpdateImage();
            }
            else if (IsShowImage())
            {
                await GetRemoteStream();

                UpdateImage();
            }
        }

        private void UpdateImage()
        {
            if (SourceStream == null) return;
            SourceStream.Seek(0, SeekOrigin.Begin);

            try
            {
                if (IsGif)
                {
                    GifImage = new ExtendedImage();
                    GifImage.SetSource(SourceStream);
                }
                else
                {
                    Image = new BitmapImage();
                    Image.SetSource(SourceStream);
                }
                OnImageOpened();
            }
            catch
            {
                Image = null;
                GifImage = null;
                if (!IsGif && IsEmotion)
                {
                    IsGif = !IsGif;
                    UpdateImage();
                }
                else
                    OnImageOpenFailed();
            }
        }

        private bool IsShowImage()
        {
#if S1Nyan
            return ((IsForceShow || SettingView.IsShowPic) && !IsGif) || IsEmotion;
#else
            return true;
#endif
        }

        private async Task GetRemoteStream()
        {
            client.DownloadProgressChanged += OnDownloadProgressChanged;
            try
            {
                SourceStream = await client.OpenReadTaskAsync(SourceUrl);
            }
            catch { }
            finally
            {
                if (SourceStream == null) OnImageOpenFailed();
            }
        }

        void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
                DownloadProgressChanged(this, e);
        }

        void OnImageOpenFailed()
        {
            IsLoadingFailed = true;
        }

        bool isImageReady = false;
        void OnImageOpened()
        {
            if (LoadingCompleted != null)
                LoadingCompleted(this, null);

            isImageReady = true;
            IsLoadingFailed = false;
        }

        internal void AddSmartImage(SmartImage image)
        {
            smartImageList.Add(image);
            image.Proxy = this;
            if (isImageReady)
            {
                LoadingCompleted(this, null);
            }
        }

        public void Dispose()
        {
            try
            {
                if (client.IsBusy) client.CancelAsync();
                client.DownloadProgressChanged -= OnDownloadProgressChanged;

                if (SourceStream != null)
                    SourceStream.Dispose();

                Image = null;
                GifImage = null;

                foreach (var image in smartImageList)
                {
                    image.Proxy = null;
                }
                smartImageList.Clear();
            }
            catch { }
        }

    }

}
