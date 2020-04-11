using ModelGraph.Controls;
using ModelGraph.Views;
using ModelGraphSTD;
using RepositoryUWP;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

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
        public async Task<bool> Dispatch(UIRequest rq, IModelPageControl ctrl)
        {
            switch (rq.RequestType)
            {
                case RequestType.Save:
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ctrl.Save(); });
                    return true;

                case RequestType.Reload:
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ctrl.Reload(); });
                    return true;

                case RequestType.Refresh:
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ctrl.Refresh(); });
                    return true;

                case RequestType.Close:                   
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { RemoveModelPage(ctrl.RootModel); WindowManagerService.Current.CloseRelatedModels(ctrl.RootModel); ctrl.Release(); });                    
                    return true;

                case RequestType.CreateView:
                    await ctrl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => InsertModelPage(new RootModel(rq)));
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
                var modelControl = ctrl.RootModel?.PageControl as IModelPageControl;
                modelControl?.Release();
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
                InsertModelPage(rootModel);
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
            openPicker.FileTypeFilter.Add(".mgd");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var rootModel = new RootModel(new RepositoryStorageFile(file))
                    {
                        ControlType = ControlType.PrimaryTree
                    };
                    InsertModelPage(rootModel);
                });
            }
            return true;
        }
        #endregion

        public static Action<RootModel> InsertModelPage; //coordination with ShellPage NavigationView
        public static Action<RootModel> RemoveModelPage; //coordination with ShellPage NavigationView
    }
}
