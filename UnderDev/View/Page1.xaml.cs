using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using S1Nyan.Views;
using UnderDev.ViewModel;

namespace UnderDev
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
            DataContext = new Page1ViewModel();
            Loaded += Page1_Loaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                ImageResourceManager.Reset();
            }
        }
        void Page1_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                var a = new SmartImage();
                //ImageResourceManager.SetUriSource(a, S1Parser.S1Resource.EmotionBase + "face/fake.jpg");
                ImageResourceManager.SetUriSource(a, "http://ww2.sinaimg.cn/bmiddle/9941abc6gw1e239z973nwj.jpg");
                //ImageResourceManager.SetUriSource(a, "http://rack.0.mshcdn.com/media/ZgkyMDEzLzAxLzMwLzIzL0xpZ2h0UmVmcmFjLmRhZjE3LmdpZg/674a0906/aea/Light-Refraction.gif");
                //ImageResourceManager.SetUriSource(a, "http://ww3.sinaimg.cn/bmiddle/64112046jw1e2cv7zkejbg.gif");
                //ImageResourceManager.SetUriSource(a, "http://ww1.sinaimg.cn/bmiddle/64112046jw1e24rf0j1ehg.gif");
                Panel.Children.Add(a);
            }
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);
        }
    }
}
