using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace ObiWang.Controls
{
    public class MyPhoneTextBox : PhoneTextBox
    {
        private TextBlock measureTextBlock;
        public MyPhoneTextBox()
        {
            DefaultStyleKey = typeof(MyPhoneTextBox);
        }

        public string MeasureText
        {
            get { return (string)GetValue(MeasureTextProperty); }
            set { SetValue(MeasureTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MeasureText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MeasureTextProperty =
            DependencyProperty.Register("MeasureText", typeof(string), typeof(MyPhoneTextBox), new PropertyMetadata(""));

        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            measureTextBlock = GetTemplateChild("MeasurementTextBlock") as TextBlock;
        }

        //protected override void OnTextChanged(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    MeasureText = Text + " XXXXXXXX";
        //    base.OnTextChanged(sender, e);
        //}

        //private double preHeight = -1;
        //protected override void ResizeTextBox()
        //{
        //    if (ActionIcon == null || TextWrapping != System.Windows.TextWrapping.Wrap) { return; }

        //    measureTextBlock.Width = ActualWidth - 36;

        //    var currentHeight = measureTextBlock.ActualHeight;
        //    if (preHeight != currentHeight)
        //    {
        //        if (preHeight != -1) 
        //        {
        //            var deleta = currentHeight - preHeight;
        //            Height = ActualHeight + deleta;
        //        }
        //        preHeight = currentHeight;
        //    }
        //}
    }
}
