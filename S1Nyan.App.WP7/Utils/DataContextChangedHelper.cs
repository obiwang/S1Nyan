using System;
using System.Windows;
using System.Windows.Data;

namespace S1Nyan.App.Utils
{
    public interface IDataContextChangedHandler<T> where T : FrameworkElement
    {
        void OnDataContextChanged(T sender, DependencyPropertyChangedEventArgs e);
    }

    public static class DataContextChangedHelper<T> where T : FrameworkElement, IDataContextChangedHandler<T>
    {
        public static readonly DependencyProperty InternalDataContextProperty =
            DependencyProperty.Register("InternalDataContext",
            typeof(Object), typeof(T), new PropertyMetadata(OnDataContextChanged));

        public static void Bind(T control)
        {
            control.SetBinding(InternalDataContextProperty, new Binding());
        }

        private static void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            T control = (T)sender;
            control.OnDataContextChanged(control, e);
        }
    }
}
