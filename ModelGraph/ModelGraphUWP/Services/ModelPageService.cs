﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;

using ModelGraphSTD;

namespace ModelGraphUWP.Services
{
    internal class ModelPageService
    {
        PageControl _rootPage;
        RootModel _rootModel;
        RootModel _compareModel;

        public static ModelPageService Current => _current ?? (_current = new ModelPageService());
        static ModelPageService _current;

        #region Constructor  ==================================================
        private ModelPageService()
        {
            _modelPages = new ConcurrentDictionary<RootModel, PageControl>();

            ApplicationView.GetForCurrentView().Consolidated += ViewConsolidated;
        }
        private void ViewConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            var app = (App)Application.Current;
            app.Exit(); // all ModelPage will close
        }
        #endregion

        readonly ConcurrentDictionary<RootModel, PageControl> _modelPages;

        #region AddModelPage  =================================================
        internal async void AddModelPage(RootModel model, PageControl page)
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
        #endregion

        #region RemoveModelPage  ==============================================
        internal async void RemoveModelPage(RootModel root, PageControl page)
        {
            if (root == null) return;

            if (page == _rootPage)
            {
                var hitList = new List<(RootModel model, PageControl page)>();

                foreach (var e in _modelPages)
                {
                    if (root.Chef == e.Key.Chef)
                    {
                        hitList.Add((e.Key, e.Value));
                    }
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
                _modelPages.TryRemove(root, out PageControl p);
            }

            if (page == _rootPage)
            {
                await _rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { UpdateNavigationPane(_rootModel); });
            }
        }
        #endregion


        #region RegesterNavigaionPaneUpdateMethod  ============================
        internal void RegesterNavigaionPaneUpdateMethod(Action<RootModel, RootModel, List<RootModel>, RootModel> updater)
        {
            _updateNavigationPane = updater;
            UpdateNavigationPane(_rootModel);
        }
        Action<RootModel, RootModel, List<RootModel>, RootModel> _updateNavigationPane;
        #endregion

        #region UpdateNavigationPane  =========================================
        void UpdateNavigationPane(RootModel selectedModed)
        {
            if (_rootModel == null || _updateNavigationPane == null)
            {
                return;
            }

            var modelList = new List<RootModel>(8);

            foreach (var e in _modelPages)
            {
                if (_rootPage == e.Value && e.Key != _rootModel)
                {
                    modelList.Add((e.Key));
                }
            }
            _updateNavigationPane(_rootModel, _compareModel, modelList, selectedModed);
        }
        #endregion

        internal void ShowModelControl(RootModel model) => _rootPage?.ShowModelControl(model);

        #region CreateNewPage  ================================================
        internal async void CreateNewPage(RootModel model)
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

        #endregion
    }
}
