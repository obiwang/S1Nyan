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
    public partial class FlipButton : UserControl
    {
        public FlipButton()
        {
            InitializeComponent();

            button.Click += button_Click;

            VisualStateManager.GoToState(this, "FlipBack", false);
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Flip", true);
        }

    }
}
