using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using S1Parser;

namespace S1Nyan.Model
{
    public class NetResourceService : IResourceService
    {
        private static IStorageHelper _helper;

        public static IStorageHelper StorageHelper
        {
            get { return _helper ?? (_helper = IoC.Get<IStorageHelper>()); }
        }

        public static async Task<Stream> GetResourceStreamStatic(Uri uri, string path = null, int expireDays = 3, bool isS1 = true)
        {
            Stream s = null;

            if (path != null)
            {
                s = StorageHelper.ReadFromLocalCache(path, expireDays);
                if (s != null) return s;
            }

            if (isS1)
                s = await new S1WebClient().OpenReadTaskAsync(uri);
            else
                s = await new WebClient().OpenReadTaskAsync(uri);

            if (path != null && s != null)
            {
                StorageHelper.WriteBinaryToLocalCache(path, s);
                s.Seek(0, SeekOrigin.Begin);
            }
            return s;
        }
        
        public Task<Stream> GetResourceStream(Uri uri, string path = null)
        {
            return GetResourceStreamStatic(uri, path);
        }
    }

    public class ApplicationResourceService : IResourceService
    {
        public static Task<Stream> GetResourceStreamStatic(Uri uri, string path = null)
        {
            return Task<Stream>.Factory.StartNew(() => Application.GetResourceStream(uri).Stream);
        }

        public Task<Stream> GetResourceStream(Uri uri, string path = null)
        {
            return GetResourceStreamStatic(uri, path);
        }
    }

}
