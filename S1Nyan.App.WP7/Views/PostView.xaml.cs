using System.Collections;
using System.Windows;
using Microsoft.Phone.Controls;

namespace S1Nyan.Views
{
    public partial class PostView
    {
        private void PageBottom_Click(object sender, RoutedEventArgs e)
        {
            //VertSlider.Value = VertSlider.Maximum;
            var list = theList.ItemsSource as IList;
            if (list != null && list.Count > 0)
                theList.ScrollToGroup(list[list.Count - 1]);
        }

        private void PageTop_Click(object sender, RoutedEventArgs e)
        {
            var list = theList.ItemsSource as IList;
            if (list.Count > 0)
                theList.ScrollToGroup(list[0]);
           //VertSlider.Value = VertSlider.Minimum;
        }

        private void VertSliderSelectedValueChanged(object sender, System.EventArgs e)
        {
            int index = (int)(VertSlider.Value + .5) % 50;
            var item = (theList.ItemsSource as IList)[index];
            theList.ScrollToGroup(item);
        }

        private void theList_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            Navigator.Visibility = Visibility.Collapsed;
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
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