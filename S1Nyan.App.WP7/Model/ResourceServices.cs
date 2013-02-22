using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Coding4Fun.Toolkit.Net;
using S1Parser;

namespace S1Nyan.Model
{
    public class NetResourceService : IResourceService
    {
        public Task<Stream> GetResourceStream(Uri uri)
        {
            return new GzipWebClient().OpenReadTaskAsync(uri);
        }
    }

    public class ApplicationResourceService : IResourceService
    {
        public Task<Stream> GetResourceStream(Uri uri)
        {
            return Task<Stream>.Factory.StartNew(() => Application.GetResourceStream(uri).Stream);
        }
    }

}
