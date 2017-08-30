using ModelGraphLibrary;
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

namespace ModelGraph
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    sealed partial class MainPage : Page, IPageControl
    {
        private List<RootModel> _modelList = new List<RootModel>();
        private RootModel _currentModel;
        private ViewRequest _viewRequest;
        public static int _newCount = 0;
        public int ModelCount => _modelList.Count;
        public RootModel[] GetModels() => (_modelList == null) ? null : _modelList.ToArray();

        public int ViewId { get; set; }
        CoreDispatcher IPageControl.Dispatcher => Dispatcher;

        #region Constructor  ==================================================
        public MainPage()
        {
            InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(320, 320);
            //ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 320));
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;


            Loaded += MainPage_Loaded;
            Window.Current.SizeChanged += Current_SizeChanged;

            if (((App)Application.Current).RootModel == null)
            {
                var page = this;
                var root = new RootModel(page);
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

            var page = model.Page;
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

            var page = model.Page;
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
            var page = model.Page;
            if (model == _currentModel)
            {
                var prev = _currentModel.ModelControl as UIElement;
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

            var ctrl = model.ModelControl as UIElement;
            if (ctrl == null) return;

            if (!_modelList.Contains(model))
                _modelList.Add(model);

            if (_currentModel != null)
            {
                var prev = _currentModel.ModelControl as UIElement;
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
            model.ModelControl.SetSize(ActualWidth, height);
            model.MajorDelta = -1;
            model.PageRefresh();
            TabPanel.Refresh(_currentModel, maxTabPanelWidth);
        }
        #endregion

        #region ReloadModelView  ==============================================
        public void ReloadModelView()
        {
            var model = _currentModel;
            if (model == null) return;
            if (model.IsInvalid) return;

            var ctrl = model.ModelControl as UIElement;
            if (ctrl == null) return;

            if (!_modelList.Contains(model))
                _modelList.Add(model);

            if (_currentModel != null)
            {
                var prev = _currentModel.ModelControl as UIElement;
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
            model.ModelControl.SetSize(ActualWidth, height);
            model.MajorDelta = -1;
            model.PageRefresh();
            TabPanel.Refresh(_currentModel, maxTabPanelWidth);
        }
        #endregion

        #region ViewControl  ==================================================
        async void IPageControl.CreateView(ViewRequest viewRequest)
        {
            if (viewRequest.OpenInNewPage)
            {
                var thisViewId = ApplicationView.GetForCurrentView().Id;
                CoreApplicationView newView = CoreApplication.CreateNewView();
                int newViewId = 0;
                await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var frame = new Frame();
                    frame.Navigate(typeof(MainPage), viewRequest);
                    Window.Current.Content = frame;
                    Window.Current.Activate();
                    newViewId = ApplicationView.GetForCurrentView().Id;
                });
                bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId, ViewSizePreference.UseMinimum, thisViewId, ViewSizePreference.UseMinimum);
            }
            else
            {
                var root = new RootModel(this, viewRequest);
                LoadModelView(root);
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ((App)Application.Current).PageList.Add(this);

            var view = ApplicationView.GetForCurrentView();
            view.TryResizeView(new Size(320, 320));

            if (!CoreApplication.GetCurrentView().IsMain && (e.Parameter is ViewRequest))
            {
                if (_viewRequest == null)
                {
                    _viewRequest = e.Parameter as ViewRequest;
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

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
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
            else if (_viewRequest != null)
            {
                var rq = _viewRequest;
                _viewRequest = null;
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
            _currentModel.ModelControl?.SetSize(ActualWidth, height);
        }

        private async void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (_currentModel == null) return;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                maxTabPanelWidth = ActualWidth - defaultControlsWidth;
                TabPanel.Refresh(_currentModel, maxTabPanelWidth);

                var height = ActualHeight - TabGrid.ActualHeight - ButtonGrid.ActualHeight;
                _currentModel.ModelControl?.SetSize(ActualWidth, height);
            });
        }
        #endregion

        #region ButtonGrid_DragDrop  ==========================================
        private void ButtonGrid_DragOver(object sender, DragEventArgs e)
        {
            var model = ((App)Application.Current).DragSource;
            if (model != null && model.Page.ModelCount > 1 && model.Page == this)
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
            var local = g.Tag as IPageControl;
            if (local == null) return;

            if (((App)Application.Current).TryGetDragSource(out RootModel model))
            {
                var rq = model.BuildViewRequest();
                rq.OpenInNewPage = true;
                var page = model.Page;
                await page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { page.CloseModelView(model); });
                local.CreateView(rq);
            }
        }
        #endregion

        #region TabItem_DragDrop  =============================================
        void IPageControl.TabItem_DragOver(RootModel target, DragEventArgs e)
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
        async void IPageControl.TabItem_DragDrop(RootModel target, DragEventArgs e)
        {
            RootModel model;
            if (((App)Application.Current).TryGetDragSource(out model))
            {
                var rq = model.BuildViewRequest();
                var page = model.Page;
                await page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { page.CloseModelView(model); });
                LoadModelView(new RootModel(this, rq));
            }
        }
        void IPageControl.TabItem_DragStarting(RootModel model)
        {
            ((App)Application.Current).DragSource = model;
        }

        #endregion

        #region AppButton_Click  =================================================
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
    }
}
