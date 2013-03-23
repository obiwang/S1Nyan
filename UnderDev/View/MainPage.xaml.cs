using System.Collections.Generic;
using Microsoft.Phone.Controls;
using UnderDev.ViewModel;

namespace UnderDev
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ImageTools.IO.Decoders.AddDecoder<ImageTools.IO.Gif.GifDecoder>();

            DataContext = new MainViewModel();
        }
    }
}
