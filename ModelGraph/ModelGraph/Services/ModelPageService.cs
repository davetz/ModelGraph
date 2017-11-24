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
        private readonly RootModel _rootModel;
        private readonly ConcurrentDictionary<ModelPageControl, Chef> _modelPages;



        internal ModelPageService(App app)
        {
            _app = app;
            _rootModel = new RootModel();
            _modelPages = new ConcurrentDictionary<ModelPageControl, Chef>();

            ApplicationView.GetForCurrentView().Consolidated += ViewConsolidated;

            CreateModelPage(_rootModel);
        }
        private void ViewConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            _app.Exit(); //(don't save anything) just kill off all ModelPage instances
        }




        internal void CloseModelPages(object key)
        {
            var hitList = new List<ModelPageControl>();

            foreach (var e in _modelPages)
            {
                if (key == null || e.Value == key) hitList.Add(e.Key);
            }
            foreach (var ctrl in hitList)
            {
                RemoveModelPage(ctrl);
            }
        }

        internal void RemoveModelPage(ModelPageControl ctrl)
        {
            _modelPages.TryRemove(ctrl, out Chef owner);
        }


        internal async void CreateModelPage(RootModel model)
        {
            // Set up the secondary view, but don't show it yet
            ModelPageControl pageControl = null;
            await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                pageControl = ModelPageControl.CreateForCurrentView(this, model);
                model.PageControl = pageControl;
                _modelPages.TryAdd(pageControl, null);

                // Increment the ref count because we just created the view and we have a reference to it                
                pageControl.StartViewInUse();

                var frame = new Frame();
                frame.Navigate(typeof(ModelPage), pageControl);
                Window.Current.Content = frame;
                Window.Current.Activate();
            });

            try
            {
                // Prevent the view from closing while switching to it
                pageControl.StartViewInUse();

                var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
                    pageControl.Id, ViewSizePreference.UseMinimum);

                // Signal that switching has completed and let the view close
                pageControl.StopViewInUse();
            }
            catch (InvalidOperationException)
            {
                // The view could be in the process of closing, and
                // this thread just hasn't updated. As part of being closed,
            }
        }
    }
}
