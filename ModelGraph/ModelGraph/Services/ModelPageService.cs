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
    public delegate void PageClosedHandler(ViewLifetimeControl viewControl, EventArgs e);

    public class ModelPageService
    {
        public static ModelPageService Current => _current ?? (_current = new ModelPageService());
        private static ModelPageService _current;

        private RootModel _rootModel;

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

        public static Action<ModelPageControl> InsertModelPage;
        public static Action<ModelPageControl> RemoveModelPage;
    }
}
