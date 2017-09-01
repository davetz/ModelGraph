using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ModelGraphLibrary;

namespace ModelGraph
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    sealed partial class PageControl : Page, IPageControl
    {
        private List<RootModel> _modelList = new List<RootModel>();
        private RootModel _currentModel;
        private UIRequest _uiRequest;
        public static int _newCount = 0;
        public int ModelCount => _modelList.Count;
        public RootModel[] GetModels() => (_modelList == null) ? null : _modelList.ToArray();

        public int ViewId { get; set; }

        #region Constructor  ==================================================
        public PageControl()
        {
            InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(320, 320);
            //ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 320));
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;


            Loaded += PageControl_Loaded;
            Window.Current.SizeChanged += Current_SizeChanged;

            if (((App)Application.Current).RootModel == null)
            {
                var root = new RootModel(this);
                root.ViewControl = new ModelTreeControl(root, ControlType.PrimaryTree);
                _modelList.Add(root);

                ((App)Application.Current).RootModel = root;
            }
            ButtonGrid.Tag = this;
        }
        #endregion

        #region CloseModel  ===================================================
        public void CloseModel(RootModel model)
        {
            if (model == null) return;
            if (model.IsInvalid) return;

            var page = model.PageControl;
            var chef = model.Chef;

            var app = ((App)Application.Current);
            var pageList = app.PageList.ToArray();
            var rootModel = app.RootModel;
            var rootChef = rootModel.Chef;
            foreach (var pg in pageList)
            {
                var list = pg.GetModels();
                if (list != null)
                {
                    foreach (var mod in list)
                    {
                        if (mod.Chef != chef) continue;
                        CloseModelView(mod);
                    }
                }
            }
            rootChef.Remove(chef);
            rootChef.PostModelRefresh(rootModel);
        }
        #endregion

        #region ReloadModel  ==================================================
        public void ReloadModel(RootModel model)
        {
            if (model == null) return;
            if (model.IsInvalid) return;

            var page = model.PageControl;
            var chef = model.Chef;
            var file = chef.ModelingFile;
            if (file == null) return;

            CloseModel(model);
            var root = ((App)Application.Current).RootModel;
            var rootChef = root.Chef;
            rootChef.AppRootOpenModel(root, file);
        }
        #endregion

        #region CloseModelView  ===============================================
        public async void CloseModelView(RootModel model)
        {
            var page = model.PageControl as PageControl;
            if (model == _currentModel)
            {
                var prev = _currentModel.ViewControl as UIElement;
                ControlGrid.Children.Remove(prev);
                _currentModel = null;
                var inx = _modelList.IndexOf(model);
                _modelList.Remove(model);
                model.Close();

                if (inx < _modelList.Count)
                {
                    LoadModelView(_modelList[inx]);
                }
                else if (_modelList.Count > 0)
                {
                    inx = _modelList.Count - 1;
                    LoadModelView(_modelList[inx]);
                }
                else
                {
                    TabPanel.Refresh(null, maxTabPanelWidth);
                }
            }
            else
            {
                _modelList.Remove(model);
                TabPanel.Refresh(_currentModel, maxTabPanelWidth);
            }
            model.Close();

            if (_modelList.Count == 0 && !CoreApplication.GetCurrentView().IsMain)
            {
                var pageList = ((App)Application.Current).PageList;
                pageList.Remove(page);
                await ApplicationViewSwitcher.SwitchAsync(pageList[0].ViewId, ViewId, ApplicationViewSwitchingOptions.ConsolidateViews);
            }
        }

        #endregion

        #region LoadModelView  ================================================
        public void LoadModelView(RootModel model)
        {
            if (model == null) return;
            if (model.IsInvalid) return;
            if (model == _currentModel) return;

            var ctrl = model.ViewControl as UIElement;
            if (ctrl == null) return;

            if (!_modelList.Contains(model))
                _modelList.Add(model);

            if (_currentModel != null)
            {
                var prev = _currentModel.ViewControl as UIElement;
                ControlGrid.Children.Remove(prev);
            }

            _currentModel = model;
            ControlGrid.Children.Add(ctrl);

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

            var height = ActualHeight - TabGrid.ActualHeight - ButtonGrid.ActualHeight;
            (model.ViewControl as IModelControl).SetSize(ActualWidth, height);
            model.MajorDelta = -1;
            Dispatch(UIRequest.Refresh(model));
            TabPanel.Refresh(_currentModel, maxTabPanelWidth);
        }
        #endregion

        #region ReloadModelView  ==============================================
        public void ReloadModelView()
        {
            var model = _currentModel;
            if (model == null) return;
            if (model.IsInvalid) return;

            var ctrl = model.ViewControl as UIElement;
            if (ctrl == null) return;

            if (!_modelList.Contains(model))
                _modelList.Add(model);

            if (_currentModel != null)
            {
                var prev = _currentModel.ViewControl as UIElement;
                ControlGrid.Children.Remove(prev);
            }

            _currentModel = model;
            ControlGrid.Children.Add(ctrl);

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

            var height = ActualHeight - TabGrid.ActualHeight - ButtonGrid.ActualHeight;
            (model.ViewControl as IModelControl).SetSize(ActualWidth, height);
            model.MajorDelta = -1;
            Dispatch(UIRequest.Refresh(model));
            TabPanel.Refresh(_currentModel, maxTabPanelWidth);
        }
        #endregion

        #region ViewControl  ==================================================
        async void CreateView(UIRequest rq)
        {
            if (rq.DoCreateNewPage)
            {
                var thisViewId = ApplicationView.GetForCurrentView().Id;
                CoreApplicationView newView = CoreApplication.CreateNewView();
                int newViewId = 0;
                await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var frame = new Frame();
                    frame.Navigate(typeof(PageControl), rq);
                    Window.Current.Content = frame;
                    Window.Current.Activate();
                    newViewId = ApplicationView.GetForCurrentView().Id;
                });
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId, ViewSizePreference.UseMinimum, thisViewId, ViewSizePreference.UseMinimum);
            }
            else
            {
                var root = new RootModel(this, rq);
                root.ViewControl = CreateControl(root, rq.Type);
                LoadModelView(root);
            }
        }

        IViewControl CreateControl(RootModel root, ControlType type)
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ((App)Application.Current).PageList.Add(this);

            var view = ApplicationView.GetForCurrentView();
            view.TryResizeView(new Size(320, 320));

            if (!CoreApplication.GetCurrentView().IsMain && (e.Parameter is UIRequest uiRequest))
            {
                if (_uiRequest == null)
                {
                    _uiRequest = uiRequest;
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ((App)Application.Current).PageList.Remove(this);
        }
        #endregion

        #region CustomTitleBar  ===============================================
        private double defaultControlsWidth = 0.0;
        private double maxTabPanelWidth = 0.0;
        private CoreApplicationViewTitleBar mainCoreTitleBar;

        private void PageControl_Loaded(object sender, RoutedEventArgs e)
        {
            mainCoreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            mainCoreTitleBar.ExtendViewIntoTitleBar = true;
            mainCoreTitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;

            var view = ApplicationView.GetForCurrentView();
            view.Consolidated += ApplicationView_Consolidated;
            ViewId = view.Id;

            if (CoreApplication.GetCurrentView().IsMain)
            {
                var root = ((App)Application.Current).RootModel;
                LoadModelView(root);
            }
            else if (_uiRequest != null)
            {
                var rq = _uiRequest;
                _uiRequest = null;
                var model = new RootModel(this, rq);
                LoadModelView(model);
                model.PostModelRefresh();
            }

            TabPanel.Initialize(this, _modelList);
        }

        private void ApplicationView_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            if (_modelList != null)
            {
                var hitList = _modelList.ToArray();
                foreach (var model in hitList)
                {
                    model.Close();
                }
                _modelList = null;
            }
        }

        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            //CustomTitleBar.Margin = new Thickness() { Left = mainCoreTitleBar.SystemOverlayLeftInset, Right = mainCoreTitleBar.SystemOverlayRightInset };
            //CustomTitleBar.Margin = new Thickness() { Left = mainCoreTitleBar.SystemOverlayRightInset, Right = mainCoreTitleBar.SystemOverlayLeftInset };
            defaultControlsWidth = sender.SystemOverlayRightInset;
            maxTabPanelWidth = ActualWidth - defaultControlsWidth;
            TabPanel.Height = GrapPanel.Height = sender.Height;
            Window.Current.SetTitleBar(GrapPanel);
            TabPanel.Refresh(_currentModel, maxTabPanelWidth);

            var height = ActualHeight - TabGrid.ActualHeight - ButtonGrid.ActualHeight;
            (_currentModel.ViewControl as IModelControl).SetSize(ActualWidth, height);
        }

        private async void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (_currentModel == null) return;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                maxTabPanelWidth = ActualWidth - defaultControlsWidth;
                TabPanel.Refresh(_currentModel, maxTabPanelWidth);

                var height = ActualHeight - TabGrid.ActualHeight - ButtonGrid.ActualHeight;
                (_currentModel.ViewControl as IModelControl).SetSize(ActualWidth, height);
            });
        }
        #endregion

        #region ButtonGrid_DragDrop  ==========================================
        private void ButtonGrid_DragOver(object sender, DragEventArgs e)
        {
            var model = ((App)Application.Current).DragSource;
            if (model != null && (model.PageControl as PageControl).ModelCount > 1 && model.PageControl == this)
            {
                e.AcceptedOperation = DataPackageOperation.Move;
                e.DragUIOverride.IsCaptionVisible = true;
                e.DragUIOverride.IsGlyphVisible = true;
                e.DragUIOverride.IsContentVisible = false;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }

        async private void ButtonGrid_Drop(object sender, DragEventArgs e)
        {
            var g = sender as Grid;
            if (g == null) return;
            var local = g.Tag as PageControl;
            if (local == null) return;

            if (((App)Application.Current).TryGetDragSource(out RootModel model))
            {
                model.PageControl.Dispatch(model.BuildViewRequest());
            }
        }
        #endregion

        #region TabItem_DragDrop  =============================================
        public void TabItem_DragOver(RootModel target, DragEventArgs e)
        {
            if (((App)Application.Current).DragSource != null)
            {
                e.AcceptedOperation = DataPackageOperation.Move;
                e.DragUIOverride.IsCaptionVisible = true;
                e.DragUIOverride.IsGlyphVisible = true;
                e.DragUIOverride.IsContentVisible = false;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }
        public async void TabItem_DragDrop(RootModel target, DragEventArgs e)
        {
            RootModel model;
            if (((App)Application.Current).TryGetDragSource(out model))
            {
                var rq = model.BuildViewRequest();
                var page = model.PageControl as PageControl;
                await page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { page.CloseModelView(model); });
                LoadModelView(new RootModel(this, rq));
            }
        }
        public void TabItem_DragStarting(RootModel model)
        {
            ((App)Application.Current).DragSource = model;
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
                    FileSavePicker savePicker = new FileSavePicker();
                    savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    savePicker.SuggestedFileName = string.Empty;
                    savePicker.FileTypeChoices.Add("DataFile", new List<string>() { ".mgdf" });
                    StorageFile file = await savePicker.PickSaveFileAsync();
                    if (file != null)
                    {
                        cmd.Parameter1 = file;
                        cmd.Execute();
                        ReloadModelView();
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

        #region IPageControl  =================================================
        public async void Dispatch(UIRequest rq)
        {
            if (rq.DoRefresh)
            {
                var ctrl = rq.RootModel.ViewControl as IModelControl;
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
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
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { editor.Save(); });
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
