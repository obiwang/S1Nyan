using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace MyControls
{
    [TemplatePart(Name = PartHorizontalThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = PartVerticalThumb, Type = typeof(Thumb))]
    [TemplateVisualState(Name = DragEnterState, GroupName = DragStates)]
    [TemplateVisualState(Name = DragOverState, GroupName = DragStates)]
    public class MySlider : Slider
    {
        public MySlider()
        {
            DefaultStyleKey = typeof(MySlider);
        }

        public double SelectedValue
        {
            get { return (double)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(double), typeof(MySlider), new PropertyMetadata(0.0, OnSelectedValueChanged));

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = d as MySlider;
            if (slider != null)
                slider.Value = slider.SelectedValue;
        }

        public HorizontalAlignment HandSide
        {
            get { return (HorizontalAlignment)GetValue(HandSideProperty); }
            set { SetValue(HandSideProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HandSide.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HandSideProperty =
            DependencyProperty.Register("HandSide", typeof(HorizontalAlignment), typeof(MySlider), new PropertyMetadata(HorizontalAlignment.Right, HandSideChanged));

        private static void HandSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MySlider).HandSideChanged();
        }

        private void HandSideChanged()
        {
            if (_vertBorder != null)
            {
                _vertBorder.SetValue(Canvas.LeftProperty, HandSide == HorizontalAlignment.Right ? -192.0 : 48.0);
            }
        }

        // Summary:
        //     Builds the visual tree for the System.Windows.Controls.Slider control when
        //     a new template is applied.
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _horiThumb = GetTemplateChild(PartHorizontalThumb) as Thumb;
            if (_horiThumb != null)
            {
                _horiThumb.DragStarted += OnDragEnter;
                _horiThumb.DragCompleted += OnDragOver;
            }
            _vertThumb = GetTemplateChild(PartVerticalThumb) as Thumb;
            if (_vertThumb != null)
            {
                _vertThumb.DragStarted += OnDragEnter;
                _vertThumb.DragCompleted += OnDragOver;
            }

            _vertBorder = GetTemplateChild("VerticalBorder") as Border;
            HandSideChanged();
        }

        void OnDragEnter(object sender, DragStartedEventArgs e)
        {
            VisualStateManager.GoToState(this, DragEnterState, true);
        }

        void OnDragOver(object sender, DragCompletedEventArgs e)
        {
            VisualStateManager.GoToState(this, DragOverState, true);
            SelectedValue = Value;
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            if (!(_horiThumb != null && _horiThumb.IsDragging) &&
                !(_vertThumb != null && _vertThumb.IsDragging))
            {
                VisualStateManager.GoToState(this, DragEnterState, false);
                VisualStateManager.GoToState(this, DragOverState, true);
                SelectedValue = Value;
            }
        }

        private const string PartHorizontalThumb = "HorizontalThumb";
        private const string PartVerticalThumb = "VerticalThumb";
        private const string DragStates = "DragStates";
        private const string DragEnterState = "DragEnter";
        private const string DragOverState = "DragOver";
        private Thumb _horiThumb;
        private Thumb _vertThumb;
        private Border _vertBorder;

    }

    public class IntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double temp = 0;
            double.TryParse(value.ToString(), out temp);
            return (int)(temp + 0.5);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
