namespace S1Nyan.Utils
{
    public interface IErrorMsg
    {
        string GetExceptionMessage(System.Exception e);
        bool IsNetworkAvailable();
    }
}
