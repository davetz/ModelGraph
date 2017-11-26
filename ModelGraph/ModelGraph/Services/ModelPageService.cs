using System;
using System.Collections.Generic;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using ModelGraph.Views;
using System.Collections.Concurrent;
using ModelGraph.Internals;

namespace ModelGraph.Services
{
    internal class ModelPageService
    {
        private readonly App _app;
        private readonly ModelRoot _rootModel;
        private readonly WindowControl _rootWindowControl;
        private readonly ConcurrentDictionary<WindowControl, Chef> _modelPages;

        internal WindowControl RootWindowControl => _rootWindowControl;

        internal ModelPageService(App app)
        {
            _app = app;
            _rootModel = new ModelRoot();
            _modelPages = new ConcurrentDictionary<WindowControl, Chef>();
            _rootWindowControl = WindowControl.CreateForCurrentView(this, _rootModel);

            ApplicationView.GetForCurrentView().Consolidated += ViewConsolidated;
        }
        private void ViewConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            _app.Exit(); // don't save anything, kill off all ModelPage instances
        }




        internal void CloseModelPages(Chef key)
        {
            var hitList = new List<WindowControl>();

            foreach (var e in _modelPages)
            {
                if (key == null || e.Value == key) hitList.Add(e.Key);
            }
            foreach (var ctrl in hitList)
            {
                ctrl.StopInUse();
                RemoveModelPage(ctrl);
            }
        }

        internal void RemoveModelPage(WindowControl ctrl)
        {
            _modelPages.TryRemove(ctrl, out Chef owner);
        }


        internal async void CreateModelPage(ModelRoot model)
        {
            WindowControl windowControl = null;
            await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                windowControl = WindowControl.CreateForCurrentView(this, model);
                _modelPages.TryAdd(windowControl, null);

                windowControl.StartInUse();

                var frame = new Frame();
                frame.Navigate(typeof(PageControl), windowControl);
                Window.Current.Content = frame;
                Window.Current.Activate();
            });

            try
            {
                windowControl.StartInUse();

                var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
                    windowControl.Id, ViewSizePreference.UseMinimum);

                windowControl.StopInUse();
            }
            catch (InvalidOperationException)
            {
                // The view could be in the process of closing, and
                // this thread just hasn't updated. As part of being closed,
            }
        }
    }
}
