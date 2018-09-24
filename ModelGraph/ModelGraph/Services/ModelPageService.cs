using ModelGraphSTD;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel.Core;
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
        public async Task<bool> Dispatch(UIRequest rq, CoreDispatcher dispatcher)
        {
            switch (rq.RequestType)
            {
                case RequestType.Save:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { SaveModel(rq.Root); });
                    break;

                case RequestType.Close:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { CloseModel(rq.Root); });
                    break;

                case RequestType.Reload:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ReloadModel(rq.Root); });
                    break;

                case RequestType.Refresh:
                    // await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { _activeModel?.ModelControl?.Refresh(); });
                    break;

                case RequestType.CreateView:
                    //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => InitializeModel(new RootModel(rq)));
                    break;

                case RequestType.CreatePage:
                    //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => _pageService.CreateNewPage(new RootModel(rq)));
                    break;
            }
            return true;
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
                var rootModel = new RootModel(AppRootModel);
                var pageControl = new ModelPageControl(rootModel);
                InsertModelPage(pageControl);
            });

            return true;
        }
        #endregion

        public static Action<ModelPageControl> InsertModelPage;
        public static Action<ModelPageControl> RemoveModelPage;
    }
}
