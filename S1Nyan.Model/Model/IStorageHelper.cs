namespace S1Nyan.Model
{
    public interface IStorageHelper
    {
        System.IO.Stream ReadFromLocalCache(string relativePath, double expireDays);
        System.IO.Stream ReadFromAppResource(string relativePath);
        void WriteBinaryToLocalCache(string relativePath, System.IO.Stream s);
        void WriteToLocalCache(string relativePath, string raw);
    }
}
