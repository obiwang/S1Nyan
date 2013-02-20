using System;
namespace S1Nyan.Model
{
    public interface IResourceService
    {
        void GetResourceStream(Uri uri, Action<System.IO.Stream, Exception> callback);
    }
}
