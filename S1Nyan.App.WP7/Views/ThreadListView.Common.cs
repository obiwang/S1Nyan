using System;
using System.Windows.Data;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace S1Nyan.Views
{
    public class ThreadPopularityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double t = 0;
            // value may not be number
            Double.TryParse(value.ToString(), out t);

            return t > 150 ? 1 : (0.25 + t / 200.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Description for ThreadList.
    /// </summary>
    public partial class ThreadListView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the ThreadList class.
        /// </summary>
        public ThreadListView()
        {
            InitializeComponent();
        }

#if DEBUG
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
#endif

    }
}