using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace UnderDev
{
    public partial class Page2 : PhoneApplicationPage
    {
        public Page2()
        {
            InitializeComponent();
            var vm = new Page2ViewModel();
            vm.OnUpdateView = OnUpdateWebView;
            DataContext = vm;
        }
        Random r = new Random((int)DateTime.Now.Ticks);

        private void OnUpdateWebView(string html)
        {
            if (html != null)
                browser.NavigateToString(html);
            flipButton.Content = r.Next();
        }
    }
}