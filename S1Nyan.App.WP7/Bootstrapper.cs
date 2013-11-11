using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using S1Nyan.Resources;
using S1Nyan.ViewModel;
using S1Nyan.Utils;

namespace S1Nyan
{
    public class Bootstrapper : PhoneBootstrapper
    {
        public PhoneContainer container;

        public static Bootstrapper Current
        {
            get
            {
                return Application.Current.Resources["bootstrapper"] as Bootstrapper;
            }
        }

        protected override PhoneApplicationFrame CreatePhoneApplicationFrame()
        {
            // for page transitions from phone.toolkit
            return new TransitionFrame();
        }

        protected override void Configure()
        {
            container = new PhoneContainer();

            // Language display initialization
            InitializeLanguage();
            if (!Execute.InDesignMode)
                container.RegisterPhoneServices(RootFrame);
            container.Singleton<MainViewModel>();

            GalaSoft.MvvmLight.Threading.DispatcherHelper.Initialize();

            AddCustomConventions();
        }

        static void AddCustomConventions()
        {
            //ellided 
        }

        //protected override void OnStartup(object sender, StartupEventArgs e)
        //{
        //    var config = new TypeMappingConfiguration
        //    {
        //        DefaultSubNamespaceForViewModels = "OF.MPL.ViewModels",
        //        DefaultSubNamespaceForViews = "OF.MPL.Views"
        //    };

        //    ViewLocator.ConfigureTypeMappings(config);
        //    ViewModelLocator.ConfigureTypeMappings(config);

        //    base.OnStartup(sender, e);
        //}

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = new List<Assembly>();
            var refAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            assemblies.AddRange(refAssemblies);
            assemblies.Add(Assembly.Load("S1Nyan.ViewModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));

            return assemblies;
        }

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.

                //set display language to zh-CN always
                RootFrame.Language = XmlLanguage.GetLanguage("zh-CN"/* AppResources.ResourceLanguage*/);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection, true);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

    }
}
