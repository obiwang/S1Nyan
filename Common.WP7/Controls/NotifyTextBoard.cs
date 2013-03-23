using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ObiWang.Controls
{
    [TemplatePart(Name = ContentContainer, Type = typeof(ContentControl))]
    [TemplatePart(Name = ShowNotify, Type = typeof(Storyboard))]
    public class NotifyTextBoard : ContentControl
    {
        private const string ShowNotify = "ShowNotify";
        private const string ContentContainer = "ContentContainer";
        private Storyboard _storyboard;

        public NotifyTextBoard()
        {
            DefaultStyleKey = typeof(NotifyTextBoard);
        }

        public double Duration
        {
            get { return (double)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(double), typeof(NotifyTextBoard), new PropertyMetadata(5.0));

        // Summary:
        //     Builds the visual tree for the System.Windows.Controls.Slider control when
        //     a new template is applied.
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _storyboard = GetTemplateChild(ShowNotify) as Storyboard;
            if (_storyboard != null)
            {
                if (Duration > 0)
                    _storyboard.SpeedRatio = 10.0 / Duration;
                _storyboard.Completed += (o, e) => Content = null;
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (_storyboard != null && Content != null)
                _storyboard.Begin();
        }
    }
}
