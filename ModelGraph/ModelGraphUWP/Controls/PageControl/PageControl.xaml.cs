using ModelGraphSTD;
using ModelGraphUWP.Services;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ModelGraphUWP.Helpers;
using RepositoryUWP;

namespace ModelGraphUWP
{
    public sealed partial class PageControl : Page, IPageControl
    {
        readonly ModelPageService _pageService;
        RootModel _activeModel; // curently active model

        #region Constructor/OnNavigatedTo  ====================================
        public PageControl()
        {
            InitializeComponent();
            _pageService = ModelPageService.Current;

            SizeChanged += PageControl_SizeChanged;
            ApplicationView.GetForCurrentView().Consolidated += ViewConsolidated;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!(e.Parameter is RootModel model))
            {
                model = new RootModel();
            }

            InitializeModel(model);
        }
        #endregion

        #region PageEventHandlers  ============================================
        private void ViewConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            sender.Consolidated -= ViewConsolidated;
            _pageService.RemoveModelPage(_activeModel, this);
        }

        private void PageControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _activeModel?.ModelControl?.SetSize(ActualWidth, ActualHeight);
        }
        #endregion

        #region InitializeModel/ShowModelControl  =============================
        internal void InitializeModel(RootModel model)
        {
            model.PageControl = this;
            model.Chef.SetLocalizer(ResourceExtensions.GetLocalizer());

            _pageService.AddModelPage(model, this);

            ShowModelControl(model);
        }
        internal void ShowModelControl(RootModel model)
        {
            _activeModel = model;
            ControlGrid.Children.Clear();
            if (model == null) return;

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
            (model.ModelControl as UserControl).Loaded += ModelControl_Loaded;

            model.GetAppCommands();
            var N = model.AppButtonCommands.Count;
            var M = ButtonPanel.Children.Count;
            for (int i = 0; i < M; i++)
            {
                var btn = ButtonPanel.Children[i] as Button;
                if (i < N)
                {
                    var cmd = model.AppButtonCommands[i];
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
            ModelTitle.Text = model.TitleName;
        }

        private void ModelControl_Loaded(object sender, RoutedEventArgs e)
        {
            var ctl = sender as UserControl;
            ctl.Loaded -= ModelControl_Loaded;

            _activeModel?.ModelControl?.SetSize(ActualWidth, ActualHeight);
            ModelRefresh();
        }
        #endregion

        #region AppButton_Click  ==============================================
        private async void AppButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var cmd = btn.Tag as ModelCommand;
            if (cmd.IsStorageFileParameter1)
            {
                if (cmd.IsSaveAsCommand)
                {
                    var savePicker = new FileSavePicker
                    {
                        SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                        SuggestedFileName = string.Empty
                    };
                    savePicker.FileTypeChoices.Add("DataFile", new List<string>() { ".mgdf" });
                    StorageFile file = await savePicker.PickSaveFileAsync();
                    if (file != null)
                    {
                        cmd.Parameter1 = new RepositoryStorageFile(file);
                        cmd.Execute();
                        //ReloadModelView();
                    }
                }
                else
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
                        cmd.Parameter1 = new RepositoryStorageFile(file);
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

        #region IPageControl  =================================================
        public async void Dispatch(UIRequest rq)
        {
            if (rq.DoSaveModel)
            {
                if (rq.RootModel.ControlType == ControlType.SymbolEditor)
                {
                    var editor = rq.RootModel.ModelControl as SymbolEditControl;
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { editor.Save(); });
                }
            }
            else if (rq.DoCloseModel)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { CloseModel(rq.RootModel); });
            }
            else if (rq.DoReloadModel)
            {
                if (rq.RootModel.ControlType == ControlType.SymbolEditor)
                {
                    var editor = rq.RootModel.ModelControl as SymbolEditControl;
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { editor.Save(); });
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ReloadModel(rq.RootModel); });
                }
            }
            else if (rq.DoCreateNewView)
            {
                if (rq.DoCreateNewPage)
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => _pageService.CreateNewPage(new RootModel(rq)));
                else
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => InitializeModel(new RootModel(rq)));
            }

            ModelRefresh();
        }
        internal async void ModelRefresh()
        {
            if (_activeModel != null && _activeModel.ModelControl != null)
            {
                var ctrl = _activeModel.ModelControl;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (!(ctrl is ModelTreeControl) || _activeModel.Chef.ValidateModelTree(_activeModel))
                        ctrl.Refresh();
                });
            }
        }

        private void ReloadModel(RootModel model)
        {
            throw new NotImplementedException();
        }

        private void CloseModel(RootModel model)
        {
            var chef = model.Chef;
            chef.Close();
            _pageService.RemoveModelPage(model, this);
        }
        #endregion
    }
}
