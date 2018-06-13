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
                case ControlType.AppRootChef:
                    break;

                case ControlType.PrimaryTree:
                case ControlType.PartialTree:
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

            var buttonCommands = new List<ModelCommand>();
            model.ButtonComands(buttonCommands);

            var N = buttonCommands.Count;
            var M = ButtonPanel.Children.Count;
            for (int i = 0; i < M; i++)
            {
                var btn = ButtonPanel.Children[i] as Button;
                if (i < N)
                {
                    var cmd = buttonCommands[i];
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
            switch (rq.RequestType)
            {
                case RequestType.Save:
                    if (rq.Root.ControlType == ControlType.SymbolEditor)
                    {
                        var editor = rq.Root.ModelControl as SymbolEditControl;
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { editor.Save(); });
                    }
                    break;
                case RequestType.Close:
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { CloseModel(rq.Root); });
                    break;
                case RequestType.Reload:
                    if (rq.Root.ControlType == ControlType.SymbolEditor)
                    {
                        var editor = rq.Root.ModelControl as SymbolEditControl;
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { editor.Save(); });
                    }
                    else
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ReloadModel(rq.Root); });
                    }
                    break;
                case RequestType.Refresh:
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { _activeModel?.ModelControl?.Refresh(); });
                    break;
                case RequestType.CreateView:
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => InitializeModel(new RootModel(rq)));
                    break;
                case RequestType.CreatePage:
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => _pageService.CreateNewPage(new RootModel(rq)));
                    break;
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
