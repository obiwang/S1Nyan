using AdvancedREI.Net.Http.Compression;
using S1Nyan.Model;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace S1Parser.Net
{
    public class NetResourceService : IResourceService
    {
        private static readonly CookieContainer CookieContainer = new CookieContainer();
        private readonly IStorageHelper _storageHelper;

        public NetResourceService(IStorageHelper storageHelper)
        {
            _storageHelper = storageHelper;
        }

        public async Task<Stream> GetResourceStream(Uri uri, string path = null)
        {
            Stream s = null;

            if (path != null)
            {
                s = await _storageHelper.ReadFromLocalCache(path, 3);
                if (s != null) return s;
            }

            s = await GetResource(uri);

            if (path != null && s != null)
            {
                await _storageHelper.WriteBinaryToLocalCache(path, s);
                s.Seek(0, SeekOrigin.Begin);
            }
            return s;
        }

        public async Task<Stream> PostFormContent(Uri url, string content)
        {
            var postContent = new StringContent(content);
            postContent.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            postContent.Headers.Add("Referer", url.Host);

            return await GetResource(url, postContent);
        }

        public async Task<Stream> GetResource(Uri url, HttpContent postContent = null)
        {
            var handler = new CompressedHttpClientHandler();
            handler.CookieContainer = CookieContainer;
            var client = new HttpClient(handler);

            HttpResponseMessage response;
            if (postContent != null)
                response = await client.PostAsync(url, postContent);
            else
                response = await client.GetAsync(url);

            if (response.Content != null) 
                return await response.Content.ReadAsStreamAsync();
            return null;
        }

    }
}
