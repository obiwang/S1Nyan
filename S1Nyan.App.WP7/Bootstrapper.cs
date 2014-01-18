﻿using Caliburn.Micro;
using Caliburn.Micro.BindableAppBar;
using Microsoft.Phone.Controls;
using S1Nyan.Model;
using S1Nyan.Resources;
using S1Nyan.Utils;
using S1Nyan.ViewModels;
using S1Parser;
using S1Parser.PaserFactory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

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

            container.Singleton<MainPageViewModel>();
            container.Singleton<ServerViewModel>();
            container.PerRequest<ThreadListViewModel>();
            container.PerRequest<PostViewModel>();
            container.Singleton<IUserService, UserViewModel>();

            container.Singleton<IIndicator, Indicator>();
            container.Singleton<IErrorMsg, ErrorMsg>();
            container.Singleton<IResourceService, NetResourceService>();
            container.Singleton<IDataService, DataService>();
            container.Singleton<IStorageHelper, IsolatedStorageHelper>();
            container.Singleton<IParserFactory, DZParserFactory>();
            container.Singleton<IOrientationHelper, OrientationHelper>();

            GalaSoft.MvvmLight.Threading.DispatcherHelper.Initialize();
            ImageTools.IO.Decoders.AddDecoder<ImageTools.IO.Gif.GifDecoder>();

            AddCustomConventions();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            S1Resource.HttpUtility = new HttpUtility();
            container.Instance<IServerModel>(new ServerModel());
            Views.SettingView.InitTheme();
        }

        static void AddCustomConventions()
        {
            // App Bar Conventions
            ConventionManager.AddElementConvention<BindableAppBarButton>(
                Control.IsEnabledProperty, "DataContext", "Click");
            ConventionManager.AddElementConvention<BindableAppBarMenuItem>(
                Control.IsEnabledProperty, "DataContext", "Click");
        }

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
