using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using S1Nyan.App.Utils;
using S1Parser;

namespace S1Nyan.App.Views
{
    public partial class ThreadPage
    {
        private void LastPage_Click(object sender, RoutedEventArgs e)
        {
            HoriSlider.Value = HoriSlider.Maximum;
        }

        private void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            HoriSlider.Value = HoriSlider.Minimum;
        }

        private void PageBottom_Click(object sender, RoutedEventArgs e)
        {
            VertSlider.Value = VertSlider.Maximum;
        }

        private void PageTop_Click(object sender, RoutedEventArgs e)
        {
            VertSlider.Value = VertSlider.Minimum;
        }

        private void VertSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int index = (int)(VertSlider.Value + .5) % 50;
            //theList.ScrollTo(theList.)
        }

        private void theList_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            if (IsNavigatorVisible)
                ToggleNavigator(null, null);
        }

        protected override void OnOrientationChanged(Microsoft.Phone.Controls.OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);
            if (e.Orientation == PageOrientation.LandscapeRight)
            {
                VertBorder.HorizontalAlignment = HorizontalAlignment.Left;
                VertSlider.HandSide = HorizontalAlignment.Left;
            }
            else
            {
                VertBorder.HorizontalAlignment = HorizontalAlignment.Right;
                VertSlider.HandSide = HorizontalAlignment.Right;
            }
        }
    }
}