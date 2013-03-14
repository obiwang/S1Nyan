using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ObiWang.Controls;

namespace MyControls
{
    [TemplatePart(Name = BeginButton, Type = typeof(Button))]
    [TemplatePart(Name = EndButton, Type = typeof(Button))]
    [TemplatePart(Name = InnerSlider, Type = typeof(MySlider))]
    [TemplatePart(Name = BeginButton, Type = typeof(Button))]
    public class PopupNavigator : MySlider
    {
        private const string BeginButton = "Begin";
        private const string EndButton = "End";
        private const string InnerSlider = "InnerSlider";


        public PopupNavigator()
        {
            DefaultStyleKey = typeof(PopupNavigator);
        }

        public override void OnApplyTemplate()
        {
            _slider = GetTemplateChild(InnerSlider) as MySlider;
            if (_slider != null)
            {

            }

            _beginBtn = GetTemplateChild(BeginButton) as Button;
            if (_beginBtn != null)
                _beginBtn.Click += (o, e) => Value = Minimum;

            _endBtn = GetTemplateChild(EndButton) as Button;
            if (_endBtn != null)
                _endBtn.Click += (o, e) => Value = Maximum;

            base.OnApplyTemplate();
        }

#region dependency properties

        public string BeginText
        {
            get { return (string)GetValue(BeginTextProperty); }
            set { SetValue(BeginTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BeginText. 
        public static readonly DependencyProperty BeginTextProperty =
            DependencyProperty.Register("BeginText", typeof(string), typeof(PopupNavigator), new PropertyMetadata(""));

        public string EndText
        {
            get { return (string)GetValue(EndTextProperty); }
            set { SetValue(EndTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndText. 
        public static readonly DependencyProperty EndTextProperty =
            DependencyProperty.Register("EndText", typeof(string), typeof(PopupNavigator), new PropertyMetadata(""));

        private MySlider _slider;
        private Button _beginBtn;
        private Button _endBtn;

#endregion

        private void OnOrientationChanged()
        {
            throw new NotImplementedException();
        }

    }
}
