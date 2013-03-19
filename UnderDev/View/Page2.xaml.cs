using System;
using Microsoft.Phone.Controls;
using UnderDev.ViewModel;

namespace UnderDev
{
    public partial class Page2 : PhoneApplicationPage
    {
        public Page2()
        {
            InitializeComponent();
            var vm = new UserTestViewModel();
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