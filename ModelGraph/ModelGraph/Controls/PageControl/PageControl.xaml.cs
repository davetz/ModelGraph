using ModelGraph.Internals;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ModelGraph
{
    public sealed partial class PageControl : Page, IPageControl
    {
        WindowControl _windowControl;
        ModelRoot Model => _windowControl.RootModel; // primary model
        ModelRoot AuxModel; // auxiliary model

        public PageControl()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is WindowControl windowControl)
                _windowControl = windowControl;
            else
                _windowControl = ((App)Application.Current).RootWindowControl;

            Model.PageControl = this;

            // When this view is finally release, clean up state
            _windowControl.Released += PageControl_Released;
        }
        private void PageControl_Released(Object sender, EventArgs e)
        {
            ((WindowControl)sender).Released -= PageControl_Released;
            Window.Current.Close();
        }


        private void AppButton_Click(object sender, RoutedEventArgs e)
        {
        }


        #region CloseModel  ===================================================
        public void CloseModel(ModelRoot model)
        {
            var rootChef = Model.Chef;
            var childChef = model.Chef;
            if (rootChef.Owner == null && childChef.Owner == rootChef)
            {
                rootChef.Remove(childChef);
                rootChef.PostModelRefresh(Model);
                _windowControl.CloseModelPages(childChef);                
            }
        }
        #endregion

        #region ReloadModel  ==================================================
        public void ReloadModel(ModelRoot model)
        {
            var rootChef = Model.Chef;
            var childChef = model.Chef;
            if (rootChef.Owner == null && childChef.Owner == rootChef)
            {
                rootChef.Remove(childChef);
                rootChef.PostModelRefresh(Model);
                _windowControl.CloseModelPages(childChef);
            }
        }
        #endregion

        #region CreateView  ===================================================
        void CreateView(UIRequest rq)
        {
            if (rq.DoCreateNewPage)
            {
                var model = new ModelRoot(rq);
                _windowControl.CreateModelPage(model);
            }
        }
        #endregion


        IViewControl CreateControl(ModelRoot root, ControlType type)
        {
            switch (type)
            {
                case ControlType.PrimaryTree:
                case ControlType.PartialTree:
                case ControlType.AppRootChef:
                    return new ModelTreeControl(root, type);

                case ControlType.SymbolEditor:
                    return new SymbolEditControl(root);

                case ControlType.GraphDisplay:
                    return new ModelGraphControl(root);

                default:
                    throw new ArgumentException("Unknown ControlType");
            }
        }


        #region IPageControl  =================================================
        public async void Dispatch(UIRequest rq)
        {
            if (rq.DoRefresh)
            {
                var ctrl = rq.RootModel.ViewControl as IModelControl;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    if (!(ctrl is ModelTreeControl) || rq.RootModel.Chef.ValidateModelTree(rq.RootModel))
                        ctrl.Refresh();
                });
            }
            else if (rq.DoSaveModel)
            {
                if (rq.RootModel.ViewControl.ControlType == ControlType.SymbolEditor)
                {
                    var editor = rq.RootModel.ViewControl as SymbolEditControl;
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => { editor.Save(); });
                }
            }
            else if (rq.DoCloseModel)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => { CloseModel(rq.RootModel); });
            }
            else if (rq.DoReloadModel)
            {
                if (rq.RootModel.ViewControl.ControlType == ControlType.SymbolEditor)
                {
                    var editor = rq.RootModel.ViewControl as SymbolEditControl;
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => { editor.Save(); });
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => { ReloadModel(rq.RootModel); });
                }
            }
            else if (rq.DoCreateNewView)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => { CreateView(rq); });
            }
        }
        #endregion
    }
}
