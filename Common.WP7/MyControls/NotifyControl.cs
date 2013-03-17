using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ObiWang.Controls
{
    [TemplatePart(Name = ContentContainer, Type = typeof(ContentControl))]
    [TemplatePart(Name = ShowNotify, Type = typeof(Storyboard))]
    public class NotifyText : ContentControl
    {
        private const string ShowNotify = "ShowNotify";
        private const string ContentContainer = "ContentContainer";
        private Storyboard _storyboard;

        public NotifyText()
        {
            DefaultStyleKey = typeof(NotifyText);
        }

        // Summary:
        //     Builds the visual tree for the System.Windows.Controls.Slider control when
        //     a new template is applied.
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _storyboard = GetTemplateChild(ShowNotify) as Storyboard;
            if (_storyboard != null)
                _storyboard.Completed += (o, e) => Content = null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (_storyboard != null && Content != null)
                _storyboard.Begin();
        }
    }
}
