/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:S1Nyan.ViewModels"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using S1Nyan.Model;

namespace S1Nyan.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //SimpleIoc.Default.Register<IDataService, DataService>();
            //SimpleIoc.Default.Register<IStorageHelper, IsolatedStorageHelper>(true);
            //SimpleIoc.Default.Register<IServerModel, ServerModel>(true);
            //SimpleIoc.Default.Register<ISendPostService, UserViewModel>();

            //SimpleIoc.Default.Register<MainViewModel>();
            //SimpleIoc.Default.Register<ThreadListViewModel>();
            //SimpleIoc.Default.Register<ServerViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainPageViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainPageViewModel>();
            }
        }

        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public ThreadListViewModel ThreadList
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ThreadListViewModel>();
            }
        }

        public UserViewModel User
        {
            get { return UserViewModel.Current; }
        }

        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public PostViewModel Thread
        {
            get
            {
                return null; //return new ThreadViewModel(ServiceLocator.Current.GetInstance<IDataService>());
            }
        }


        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public ServerViewModel Server
        {
            get { return ServerViewModel.Current; }
            //get
            //{
            //    return new ThreadViewModel(ServiceLocator.Current.GetInstance<IDataService>());
            //}
        }
      /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}