using Caliburn.Micro;
using S1Nyan.ViewModels;
using S1Nyan.Views;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace S1Nyan
{
    public sealed partial class App
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif
        private WinRTContainer container;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void Configure()
        {
            MessageBinder.SpecialValues.Add("$clickeditem", c => ((ItemClickEventArgs)c.EventArgs).ClickedItem);

            container = new WinRTContainer();

            container.RegisterWinRTServices();

            container.PerRequest<MainViewModel>();
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            container.RegisterNavigationService(rootFrame);
            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
    // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif
            }
        }
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            DisplayRootView<MainView>();
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
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
//        protected override void OnLaunched(LaunchActivatedEventArgs e)
//        {
//#if DEBUG
//            if (System.Diagnostics.Debugger.IsAttached)
//            {
//                this.DebugSettings.EnableFrameRateCounter = true;
//            }
//#endif

//            Frame rootFrame = Window.Current.Content as Frame;

//            // Do not repeat app initialization when the Window already has content,
//            // just ensure that the window is active
//            if (rootFrame == null)
//            {
//                // Create a Frame to act as the navigation context and navigate to the first page
//                rootFrame = new Frame();

//                // TODO: change this value to a cache size that is appropriate for your application
//                rootFrame.CacheSize = 1;

//                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
//                {
//                    // TODO: Load state from previously suspended application
//                }

//                // Place the frame in the current Window
//                Window.Current.Content = rootFrame;
//            }

//            if (rootFrame.Content == null)
//            {
//#if WINDOWS_PHONE_APP
//                // Removes the turnstile navigation for startup.
//                if (rootFrame.ContentTransitions != null)
//                {
//                    this.transitions = new TransitionCollection();
//                    foreach (var c in rootFrame.ContentTransitions)
//                    {
//                        this.transitions.Add(c);
//                    }
//                }

//                rootFrame.ContentTransitions = null;
//                rootFrame.Navigated += this.RootFrame_FirstNavigated;
//#endif

//                // When the navigation stack isn't restored navigate to the first page,
//                // configuring the new page by passing required information as a navigation
//                // parameter
//                if (!rootFrame.Navigate(typeof(MainView), e.Arguments))
//                {
//                    throw new Exception("Failed to create initial page");
//                }
//            }

//            // Ensure the current window is active
//            Window.Current.Activate();
//        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        protected override void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}