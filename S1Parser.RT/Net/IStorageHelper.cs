using System.IO;
using System.Threading.Tasks;

namespace S1Nyan.Model
{
    public interface IStorageHelper
    {
        Task<Stream> ReadFromLocalCache(string relativePath, double expireDays);
        Task<Stream> ReadFromAppResource(string relativePath);
        Task WriteBinaryToLocalCache(string relativePath, Stream s);
        Task WriteToLocalCache(string relativePath, string raw);
    }
}
