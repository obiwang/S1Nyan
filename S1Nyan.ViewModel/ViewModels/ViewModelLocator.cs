using Caliburn.Micro;

namespace S1Nyan.ViewModels
{
    public class ViewModelLocator
    {
        public ISendPostService User
        {
            get { return IoC.Get<ISendPostService>(); }
        }
    }
}
