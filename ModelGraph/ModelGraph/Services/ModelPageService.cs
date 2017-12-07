using System;
using System.Collections.Generic;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using ModelGraph.Views;
using System.Collections.Concurrent;
using ModelGraphSTD;

namespace ModelGraph.Services
{
    internal class ModelPageService
    {
        private readonly App _app;
        private readonly ConcurrentDictionary<PageControl, Chef> _modelPages;

        //=====================================================================

        internal ModelPageService(App app)
        {
            _app = app;
            _modelPages = new ConcurrentDictionary<PageControl, Chef>();

            ApplicationView.GetForCurrentView().Consolidated += ViewConsolidated;
        }
        private void ViewConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            _app.Exit(); // all ModelPage will close
        }

        //=====================================================================

        internal void AddModelPage(PageControl page)
        {
            _modelPages.TryAdd(page, page.Model.Chef);
        }
        internal void RemoveModelPage(PageControl page)
        {
            _modelPages.TryRemove(page, out Chef owner);
        }

        //=====================================================================

        internal async void CloseModelPages(Chef key)
        {
            var hitList = new List<PageControl>();

            foreach (var e in _modelPages)
            {
                if (key == null || e.Value == key) hitList.Add(e.Key);
            }
            foreach (var page in hitList)
            {
                RemoveModelPage(page);

                await page.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Window.Current.Close();
                });
            }
        }

        //=====================================================================

        internal async void CreatePage(ModelRoot model)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                frame.Navigate(typeof(PageControl), model);
                Window.Current.Content = frame;
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId, ViewSizePreference.UseMinimum);
        }

    }
}
