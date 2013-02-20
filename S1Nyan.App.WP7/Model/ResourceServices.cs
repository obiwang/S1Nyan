using System;
using System.IO;
using System.Windows;
using Coding4Fun.Toolkit.Net;

namespace S1Nyan.Model
{
    public class NetResourceService : IResourceService
    {
        public void GetResourceStream(Uri uri, Action<System.IO.Stream, Exception> callback)
        {
            GzipWebClient client = new GzipWebClient();
            client.OpenReadAsync(uri);
            client.OpenReadCompleted += (o, e) =>
            {
                try
                {
                    callback(e.Result, null);
                }
                catch (Exception ex)
                {
                    callback(null, ex);
                }
            };
        }
    }

    public class ApplicationResourceService : IResourceService
    {
        public void GetResourceStream(Uri uri, Action<System.IO.Stream, Exception> callback)
        {
            try
            {
                callback(Application.GetResourceStream(uri).Stream, null);
            }
            catch (Exception ex)
            {
                callback(null, ex);
            }
        }
    }

}
