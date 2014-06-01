using Caliburn.Micro;

namespace S1Nyan.ViewModels
{
    public class ViewModelLocator
    {
        private IUserService _user;
        public IUserService User
        {
            get { return _user ?? (_user = IoC.Get<IUserService>()); }
        }

    }
}
