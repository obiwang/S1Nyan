namespace S1Nyan.Utils
{
    public interface IIndicator
    {
        void SetBusy(bool isBusy);
        void SetError(string text);
        void SetError(System.Exception e);
        void SetLoading();
        void SetText(string text);
    }
}
