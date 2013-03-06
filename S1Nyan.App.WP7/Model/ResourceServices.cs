using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows;
using Coding4Fun.Toolkit.Net;
using S1Parser;

namespace S1Nyan.Model
{
    public class NetResourceService : IResourceService
    {
        const string tempDir = "temp\\";

        // TODO fix cache issue with <img src="images/post/smile/goose/13.gif" />
        public static async Task<Stream> GetResourceStreamStatic(Uri uri, string path = null, int expireDays = 3)
        {
            Stream s = null;
            IsolatedStorageFile local = null;

            if (path != null)
            {
                local = IsolatedStorageFile.GetUserStoreForApplication();
                path = tempDir + path.Replace('/', '\\');
                CreateDirIfNecessary(local, path);

                if (local.FileExists(path))
                {
                    if (expireDays < 0 || (DateTime.Now - local.GetLastWriteTime(path) < TimeSpan.FromDays(expireDays)))
                    {
                        return local.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                }
            }
            s = await new GzipWebClient().OpenReadTaskAsync(uri);
            if (path != null && s != null)
            {
                using (var fileStream = local.OpenFile(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (var isoFileWriter = new BinaryWriter(fileStream))
                    {
                        var reader = new BinaryReader(s);
                        isoFileWriter.Write(reader.ReadBytes((int)s.Length));
                        isoFileWriter.Flush();
                    }
                }
                s.Seek(0, SeekOrigin.Begin);
            }
            return s;
        }

        private static void CreateDirIfNecessary(IsolatedStorageFile local, string path)
        {
            int pos = 0;
            string dir = path;

            while ((pos = dir.IndexOf('\\', pos)) != -1)
            {
                var dirname = dir.Substring(0, pos);
                if (!local.DirectoryExists(dirname))
                    local.CreateDirectory(dirname);
                pos ++;
            }
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
