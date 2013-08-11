using System.Collections;
using System.Windows;
using Microsoft.Phone.Controls;

namespace S1Nyan.Views
{
    public partial class ThreadView
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
            ShowHideNavi(true);
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