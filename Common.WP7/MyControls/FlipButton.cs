using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ObiWang.Controls
{
    [TemplatePart(Name = ContentContainer, Type = typeof(ContentControl))]
    [TemplateVisualState(Name = Flip, GroupName = FlipStates)]
    [TemplateVisualState(Name = FlipBack, GroupName = FlipStates)]
    public class FlipButton : Button
    {
        private const string FlipStates = "FlipStates";
        private const string Flip = "Flip";
        private const string FlipBack = "FlipBack";
        private const string ContentContainer = "ContentContainer";

        private ContentControl _contentControl;
        private VisualState _flip;
        public FlipButton()
        {
            DefaultStyleKey = typeof(FlipButton);
        }

        // Summary:
        //     Builds the visual tree for the System.Windows.Controls.Slider control when
        //     a new template is applied.
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _contentControl = GetTemplateChild(ContentContainer) as ContentControl;
            if (_contentControl != null)
                _contentControl.Content = Content;

            _flip = GetTemplateChild(Flip) as VisualState;
            if (_flip != null)
                _flip.Storyboard.Completed += OnFlipComplete;
            
            VisualStateManager.GoToState(this, FlipBack, false);
        }

        private void OnFlipComplete(object sender, EventArgs e)
        {
            _contentControl.Content = Content;
            VisualStateManager.GoToState(this, FlipBack, true);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (_contentControl != null && _flip != null)
            {
                VisualStateManager.GoToState(this, Flip, true);
            }
        }
    }
}
