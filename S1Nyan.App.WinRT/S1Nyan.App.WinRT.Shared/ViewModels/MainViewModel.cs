using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Popups;

namespace S1Nyan.ViewModels
{
    public class MainViewModel : Screen
    {
        public MainViewModel()
        {
            Welcome = "Hello, world";
        }

        public string Welcome { get; set; }

    }
}
