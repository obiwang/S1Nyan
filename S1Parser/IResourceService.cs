using System;
using System.IO;
using System.Threading.Tasks;

namespace S1Parser
{
    public interface IResourceService
    {
        Task<Stream> GetResourceStream(Uri uri);
    }
}
