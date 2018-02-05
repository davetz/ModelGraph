using System;
using System.Collections.Generic;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Concurrent;
using ModelGraphSTD;
using System.Threading.Tasks;

namespace ModelGraphUWP.Services
{
    internal class ModelPageService
    {
        PageControl _rootPage;
        ModelRoot _rootModel;
        ModelRoot _compareModel;
        readonly ConcurrentDictionary<ModelRoot, PageControl> _modelPages;
        static ModelPageService _current;

        //=====================================================================

        Action<ModelRoot, ModelRoot, List<ModelRoot>, ModelRoot> _updateNavigationPane;
        internal void RegesterNavigaionPaneUpdateMethod(Action<ModelRoot, ModelRoot, List<ModelRoot>, ModelRoot> updater)
        {
            _updateNavigationPane = updater;
            UpdateNavigationPane(_rootModel);
        }

        //=====================================================================
        public static ModelPageService Current => _current ?? (_current = new ModelPageService());

        private ModelPageService()
        {
            _modelPages = new ConcurrentDictionary<ModelRoot, PageControl>();

            ApplicationView.GetForCurrentView().Consolidated += ViewConsolidated;
        }
        private void ViewConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            var app = (App)Application.Current;
            app.Exit(); // all ModelPage will close
        }

        //=====================================================================

        internal void ShowModelPage(ModelRoot model)
        {
            if (_rootPage != null)
            {
                _rootPage.ShowModelPage(model);
            }
        }

        //=====================================================================

        internal void RefreshModelPage(ModelRoot model)
        {
            if (_modelPages.TryGetValue(model, out PageControl p))
            {
                p.ModelRefresh();
            }
        }

        //=====================================================================

        internal async void AddModelPage(ModelRoot model, PageControl page)
        {
            _modelPages.TryAdd(model, page);
            if (_rootPage == null)
            {
                _rootPage = page;
                _rootModel = model;
            }
            if (page == _rootPage)
            {
                await _rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { UpdateNavigationPane(model); });
            }
        }
        internal async void RemoveModelPage(ModelRoot model, PageControl page)
        {
            if (page == _rootPage)
            {
                var hitList = new List<(ModelRoot model, PageControl page)>();

                foreach (var e in _modelPages)
                {
                    if (model.Chef == e.Key.Chef) hitList.Add((e.Key, e.Value));
                }

                foreach (var e in hitList)
                {
                    if (_modelPages.TryRemove(e.model, out PageControl p))
                    {
                        if (e.page != _rootPage)
                        {
                            await e.page.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                Window.Current.Close();
                            });
                        }
                    }
                }
            }
            else
            {
                _modelPages.TryRemove(model, out PageControl p);
            }

            if (page == _rootPage)
            {
                await _rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { UpdateNavigationPane(_rootModel); });
            }
        }

        void UpdateNavigationPane(ModelRoot selectedModed)
        {
            if (_rootModel == null || _updateNavigationPane == null)
                return;

            var modelList = new List<ModelRoot>(8);

            foreach (var e in _modelPages)
            {
                if (_rootPage == e.Value && e.Key != _rootModel) modelList.Add((e.Key));
            }
            _updateNavigationPane(_rootModel, _compareModel, modelList, selectedModed);
        }

        //=====================================================================

        internal async void AddNewPage(ModelRoot model)
        {
            AddModelPage(model, _rootPage);
            _rootPage.InitializeModel(model);

            ShowModelPage(model);
            await Task.CompletedTask;
        }

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
