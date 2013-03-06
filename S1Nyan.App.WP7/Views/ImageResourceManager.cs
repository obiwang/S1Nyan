using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace S1Nyan.App.Views
{
    public class ImageResourceManager
    {
        protected static Dictionary<string, ImageSourceProxy> proxyList = new Dictionary<string, ImageSourceProxy>();

        /// <summary>
        /// Gets the value of the Uri to use for providing the contents of the Image's Source property.
        /// </summary>
        /// <param name="obj">Image needing its Source property set.</param>
        /// <returns>Uri to use for providing the contents of the Source property.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "UriSource is applicable only to Image elements.")]
        public static string GetUriSource(SmartImage obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            return (string)obj.GetValue(UriSourceProperty);
        }

        /// <summary>
        /// Sets the value of the Uri to use for providing the contents of the Image's Source property.
        /// </summary>
        /// <param name="obj">Image needing its Source property set.</param>
        /// <param name="value">Uri to use for providing the contents of the Source property.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "UriSource is applicable only to Image elements.")]
        public static void SetUriSource(SmartImage obj, string value)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            obj.SetValue(UriSourceProperty, value);
        }

        /// <summary>
        /// Identifies the UriSource attached DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.RegisterAttached(
            "UriSource", typeof(string), typeof(ImageResourceManager), new PropertyMetadata(OnUriSourceChanged));

        private static void OnUriSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var image = d as SmartImage;
            if (image == null) throw new ArgumentNullException("image");
            var url = e.NewValue as string;
            ImageSourceProxy proxy = null;
            if (proxyList.ContainsKey(url))
                proxy = proxyList[url];
            else
            {
                proxy = new ImageSourceProxy();
                proxy.SourceUrl = url;
                proxyList[url] = proxy;
            }
            proxy.AddSmartImage(image);
        }

        public static void Reset()
        {
            if (proxyList.Count != 0)
            {
                foreach (var item in proxyList.Values)
                {
                    item.Dispose();
                }
                proxyList.Clear();
            }
        }
    }
}
