using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using ModelGraph.Services;
using ModelGraphSTD;
using Windows.UI.ViewManagement;
using Windows.Foundation;

namespace ModelGraph
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        private ActivationService ActivationService => _activationService.Value;

        private Lazy<ModelPageService> _modelPageService;
        internal ModelPageService ModelPageService => _modelPageService.Value;



        public App()
        {
            InitializeComponent();

            _activationService = new Lazy<ActivationService>(CreateActivationService);

            _modelPageService = new Lazy<ModelPageService>(CreateModelPageService);
        }



        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(PageControl));//, new Views.ShellPage());
        }

        private ModelPageService CreateModelPageService()
        {
            return new ModelPageService(this);
        }

        internal ModelPageService PageService => ModelPageService;

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }
    }
}
