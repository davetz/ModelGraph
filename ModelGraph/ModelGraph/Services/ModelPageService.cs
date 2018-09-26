using ModelGraph.Controls;
using ModelGraph.Views;
using ModelGraphSTD;
using RepositoryUWP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ModelGraph.Services
{
    public class ModelPageService
    {
        public static ModelPageService Current => _current ?? (_current = new ModelPageService());
        private static ModelPageService _current;

        internal RootModel AppRootModel => _appRootModel ?? (_appRootModel = new RootModel());
        private static RootModel _appRootModel;

        #region Constructor  ==================================================
        private ModelPageService()
        {
            ApplicationView.GetForCurrentView().Consolidated += ModelPageService_Consolidated;
        }

        private void ModelPageService_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            var app = (App)Application.Current;
            app.Exit(); // all ModelPage will close
        }
        #endregion

        #region Dispatch  =====================================================
        public async Task<bool> Dispatch(UIRequest rq, ModelPageControl ctrl)
        {
            switch (rq.RequestType)
            {
                case RequestType.Save:
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { SaveModel(rq.Root); });
                    return true;

                case RequestType.Close:
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { CloseModel(rq.Root); });
                    return true;

                case RequestType.Reload:
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ReloadModel(rq.Root); });
                    return true;

                case RequestType.Refresh:
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ctrl.ModelControl?.Refresh(); });
                    return true;

                case RequestType.CreatePage:
                    //await WindowManagerService.Current.TryShowAsStandaloneAsync("blabber-blabber", typeof(ModelPage), new ModelPageControl(new RootModel(rq)));
                    //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => _pageService.CreateNewPage(new RootModel(rq)));
                    return true;
            }
            return false;
        }

        private void SaveModel(RootModel model)
        {
        }
        private void ReloadModel(RootModel model)
        {
        }

        private void CloseModel(RootModel model)
        {
        }
        #endregion

        #region CreateNewModel  ===============================================
        public async Task<bool> CreateNewModelAsync(CoreDispatcher dispatcher)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var rootModel = new RootModel(AppRootModel)
                {
                    ControlType = ControlType.PrimaryTree
                };
                var pageControl = new ModelPageControl(rootModel);
                InsertModelPage(pageControl);
            });

            return true;
        }
        #endregion

        #region OpenModelDataFile  ============================================\
        public async Task<bool> OpenModelDataFileAsync(CoreDispatcher dispatcher)
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            openPicker.FileTypeFilter.Add(".mgdf");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var rootModel = new RootModel(AppRootModel, new RepositoryStorageFile(file))
                    {
                        ControlType = ControlType.PrimaryTree
                    };
                    var pageControl = new ModelPageControl(rootModel);
                    InsertModelPage(pageControl);
                });
            }
            return true;
        }
        #endregion

        public static Action<ModelPageControl> InsertModelPage;
        public static Action<ModelPageControl> RemoveModelPage;
    }
}
