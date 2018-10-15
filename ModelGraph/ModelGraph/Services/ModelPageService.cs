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
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ctrl.ModelControl?.Save(); });
                    return true;

                case RequestType.Reload:
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ctrl.ModelControl?.Reload(); });
                    return true;

                case RequestType.Refresh:
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ctrl.ModelControl?.Refresh(); });
                    return true;

                case RequestType.Close:                   
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { RemoveModelPage(ctrl); WindowManagerService.Current.CloseRelatedModels(ctrl.RootModel); ctrl.Release(); });                    
                    return true;

                case RequestType.CreateView:
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => InsertModelPage(new ModelPageControl(new RootModel(rq))));
                    return true;

                case RequestType.CreatePage:
                    var rootModel = new RootModel(rq);
                    var viewLifetimeControl = await WindowManagerService.Current.TryShowAsStandaloneAsync(rootModel.TitleName, typeof(ModelPage), rootModel);
                    viewLifetimeControl.Released += ViewLifetimeControl_Released;
                    return true;
            }
            GC.Collect();
            return false;
        }

        private void ViewLifetimeControl_Released(object sender, EventArgs e)
        {
            if (sender is ViewLifetimeControl ctrl)
            {
                ctrl.Released -= ViewLifetimeControl_Released;
                ctrl.PageControl?.Release();
                ctrl.PageControl = null;
                ctrl.RootModel = null;
            }
        }
        #endregion

        #region CreateNewModel  ===============================================
        public async Task<bool> CreateNewModelAsync(CoreDispatcher dispatcher)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var rootModel = new RootModel()
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
                    var rootModel = new RootModel(new RepositoryStorageFile(file))
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
