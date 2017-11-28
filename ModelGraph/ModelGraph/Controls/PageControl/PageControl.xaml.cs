using ModelGraph.Internals;
using ModelGraph.Services;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ModelGraph
{
    public sealed partial class PageControl : Page, IPageControl
    {
        ModelPageService _pageService;
        ModelRoot _model;  // primary model
        ModelRoot _auxModel; // auxiliary model

        internal ModelRoot Model => _model;

        public PageControl()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is ModelRoot model)
                _model = model;
            else
                _model = new ModelRoot();

            _model.PageControl = this;

            _pageService = ((App)Application.Current).PageService;
            _pageService.AddModelPage(this);

            CreateControl(_model);

            Loaded += PageControl_Loaded;

            ApplicationView.GetForCurrentView().Consolidated += ViewConsolidated;
        }

        private void PageControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= PageControl_Loaded;
            _model.PostModelRefresh();
        }

        private void ViewConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            sender.Consolidated -= ViewConsolidated;
            _pageService.RemoveModelPage(this);
        }


        #region AppButton_Click  ==============================================
        private async void AppButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var cmd = btn.Tag as ModelCommand;
            if (cmd.IsStorageFileParameter1)
            {
                if (cmd.IsSaveAsCommand)
                {
                    FileSavePicker savePicker = new FileSavePicker();
                    savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    savePicker.SuggestedFileName = string.Empty;
                    savePicker.FileTypeChoices.Add("DataFile", new List<string>() { ".mgdf" });
                    StorageFile file = await savePicker.PickSaveFileAsync();
                    if (file != null)
                    {
                        cmd.Parameter1 = file;
                        cmd.Execute();
                        //ReloadModelView();
                    }
                }
                else
                {
                    FileOpenPicker openPicker = new FileOpenPicker();
                    openPicker.ViewMode = PickerViewMode.List;
                    openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    openPicker.FileTypeFilter.Add(".mgdf");
                    StorageFile file = await openPicker.PickSingleFileAsync();
                    if (file != null)
                    {
                        cmd.Parameter1 = file;
                        cmd.Execute();
                    }
                }
            }
            else
            {
                cmd.Execute();
            }
        }
        #endregion


        void CreateControl(ModelRoot model)
        {
            switch (model.ControlType)
            {
                case ControlType.PrimaryTree:
                case ControlType.PartialTree:
                case ControlType.AppRootChef:
                    ControlGrid.Children.Add(new ModelTreeControl(model));
                    break;

                case ControlType.SymbolEditor:
                    ControlGrid.Children.Add(new SymbolEditControl(model));
                    break;

                case ControlType.GraphDisplay:
                    ControlGrid.Children.Add(new ModelGraphControl(model));
                    break;

                default:
                    throw new ArgumentException("Unknown ControlType");
            }
            AddAppButtons();
        }

        void AddAppButtons()
        {
            _model.GetAppCommands();
            var N = _model.AppButtonCommands.Count;
            var M = ButtonPanel.Children.Count;
            for (int i = 0; i < M; i++)
            {
                var btn = ButtonPanel.Children[i] as Button;
                if (i < N)
                {
                    var cmd = _model.AppButtonCommands[i];
                    btn.Tag = cmd;
                    btn.Content = cmd.Name;
                    btn.Visibility = Visibility.Visible;
                    ToolTipService.SetToolTip(btn, cmd.Summary);
                }
                else
                {
                    btn.Visibility = Visibility.Collapsed;
                }
            }
            ModelTitle.Text = _model.TitleName;
        }

        #region IPageControl  =================================================
        public async void Dispatch(UIRequest rq)
        {
            if (rq.DoRefresh)
            {
                var ctrl = rq.RootModel.ModelControl;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    if (!(ctrl is ModelTreeControl) || rq.RootModel.Chef.ValidateModelTree(rq.RootModel))
                        ctrl.Refresh();
                });
            }
            else if (rq.DoSaveModel)
            {
                if (rq.RootModel.ControlType == ControlType.SymbolEditor)
                {
                    var editor = rq.RootModel.ModelControl as SymbolEditControl;
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => { editor.Save(); });
                }
            }
            else if (rq.DoCloseModel)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => { CloseModel(rq.RootModel); });
            }
            else if (rq.DoReloadModel)
            {
                if (rq.RootModel.ControlType == ControlType.SymbolEditor)
                {
                    var editor = rq.RootModel.ModelControl as SymbolEditControl;
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => { editor.Save(); });
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => { ReloadModel(rq.RootModel); });
                }
            }
            else if (rq.DoCreateNewView)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => _pageService.CreatePage(new ModelRoot(rq)));
            }
        }

        private void ReloadModel(ModelRoot rootModel)
        {
            throw new NotImplementedException();
        }

        private void CloseModel(ModelRoot rootModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
