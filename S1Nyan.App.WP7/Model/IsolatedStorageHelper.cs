using System;
using System.IO;
using System.IO.IsolatedStorage;
using GalaSoft.MvvmLight.Ioc;

namespace S1Nyan.Model
{
    public class IsolatedStorageHelper : IStorageHelper
    {
        public static IStorageHelper Current
        {
            get { return SimpleIoc.Default.GetInstance<IStorageHelper>(); }
        }

        const string tempDir = "temp\\";
        private IsolatedStorageFile local = IsolatedStorageFile.GetUserStoreForApplication();

        private string GetFilePath(string path)
        {
            path = tempDir + path.Replace('/', '\\');
            CreateDirIfNecessary(local, path);
            return path;
        }

        public Stream ReadFromLocalCache(string relativePath, double expireDays)
        {
            var path = GetFilePath(relativePath);

            if (local.FileExists(path))
            {
                if (expireDays < 0 || (DateTime.Now - local.GetLastWriteTime(path) < TimeSpan.FromDays(expireDays)))
                {
                    return local.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
            }

            return null;
        }

        public void WriteToLocalCache(string relativePath, string raw)
        {
            var path = GetFilePath(relativePath);

            using (var fileStream = local.OpenFile(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                using (var isoFileWriter = new StreamWriter(fileStream))
                {
                    isoFileWriter.Write(raw);
                    isoFileWriter.Flush();
                }
            }
        }

        public void WriteBinaryToLocalCache(string relativePath, Stream s)
        {
            var path = GetFilePath(relativePath);

            using (var fileStream = local.OpenFile(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                using (var isoFileWriter = new BinaryWriter(fileStream))
                {
                    var reader = new BinaryReader(s);
                    isoFileWriter.Write(reader.ReadBytes((int)s.Length));
                    isoFileWriter.Flush();
                }
            }
        }

        private void CreateDirIfNecessary(IsolatedStorageFile local, string path)
        {
            int pos = 0;
            string dir = path;

            while ((pos = dir.IndexOf('\\', pos)) != -1)
            {
                var dirname = dir.Substring(0, pos);
                if (!local.DirectoryExists(dirname))
                    local.CreateDirectory(dirname);
                pos++;
            }
        }

        public Stream ReadFromAppResource(string relativePath)
        {
            var res = System.Windows.Application.GetResourceStream(new Uri("Resources/" + relativePath, UriKind.Relative));
            return (res != null) ? res.Stream : null;
        }
    }
}
