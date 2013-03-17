using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UnderDev.ViewModel;

namespace UnderDev.View
{
    public partial class Page4 : PhoneApplicationPage
    {
        public Page4()
        {
            InitializeComponent();
            DataContext = new ServerListViewModel();
        }
    }
}