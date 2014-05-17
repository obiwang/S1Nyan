using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace S1Nyan.Model
{
    public class StorageHelper : IStorageHelper
    {
        const string TempDir = "temp";
        private readonly StorageFolder _local = ApplicationData.Current.LocalFolder;

        private async Task<StorageFolder> CacheFolder()
        {
            try
            {
                return await _local.GetFolderAsync(TempDir);
            }
            catch (Exception) { }
            return await _local.CreateFolderAsync(TempDir);
        }

        public async Task<Stream> ReadFromLocalCache(string relativePath, double expireDays)
        {
            var folder = await CacheFolder();

            try
            {
                var file = await folder.GetFileAsync(relativePath);
                var lastModified = (await file.GetBasicPropertiesAsync()).DateModified;
                if (expireDays < 0 || (DateTime.Now - lastModified.Date < TimeSpan.FromDays(expireDays)))
                    return await file.OpenStreamForReadAsync();
            }
            catch (Exception) { }

            return null;
        }

        public async Task WriteToLocalCache(string relativePath, string raw)
        {
            var folder = await CacheFolder();

            var file = await folder.CreateFileAsync(relativePath, CreationCollisionOption.ReplaceExisting);

            using (var fileStream = await file.OpenStreamForWriteAsync())
            {
                using (var isoFileWriter = new StreamWriter(fileStream))
                {
                    await isoFileWriter.WriteAsync(raw);
                    await isoFileWriter.FlushAsync();
                }
            }
        }

        public async Task WriteBinaryToLocalCache(string relativePath, Stream s)
        {
            var folder = await CacheFolder();

            //using (var fileStream = _local.OpenFile(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            //{
            //    using (var isoFileWriter = new BinaryWriter(fileStream))
            //    {
            //        var reader = new BinaryReader(s);
            //        isoFileWriter.Write(reader.ReadBytes((int)s.Length));
            //        isoFileWriter.Flush();
            //    }
            //}
        }

        public async Task<Stream> ReadFromAppResource(string relativePath)
        {
            try
            {
                var folder = Package.Current.InstalledLocation;
                var res = await folder.GetFolderAsync("Resources");
                return await res.OpenStreamForReadAsync(relativePath);
            }
            catch (Exception) { }

            return null;
        }
    }
}
