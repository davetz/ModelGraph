using ModelGraphSTD;
using ModelGraph.Services;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ModelGraph.Helpers;
using RepositoryUWP;

namespace ModelGraph.Controls
{
    public sealed partial class ModelPageControl : Page, IPageControl
    {
        public RootModel RootModel { get; private set; }
        public IModelControl ModelControl { get; set; }

        #region Constructor  ==================================================
        public ModelPageControl(RootModel rootModel)
        {
            InitializeComponent();
            InitializeModel(rootModel);
        }
        #endregion

        internal void Release()
        {
            ModelControl?.Release();
            ModelControl = null;

            RootModel?.Release();
            RootModel = null;
        }

        #region InitializeModelControl  =======================================
        internal void InitializeModel(RootModel model)
        {
            RootModel = model;
            model.PageControl = this;
            model.Chef.SetLocalizer(ResourceExtensions.GetLocalizer());

            switch (model.ControlType)
            {
                case ControlType.AppRootChef:
                    break;

                case ControlType.PrimaryTree:
                case ControlType.PartialTree:
                    var tc = new ModelTreeControl(model);
                    ModelControl = tc;
                    ControlGrid.Children.Add(tc);
                    break;

                case ControlType.SymbolEditor:
                    var sc =new SymbolEditControl(model);
                    ModelControl = sc;
                    ControlGrid.Children.Add(sc);
                    break;

                case ControlType.GraphDisplay:
                    var gc = new ModelGraphControl(model, ControlPanel);
                    ModelControl = gc;
                    ControlGrid.Children.Add(gc);
                    break;

                default:
                    throw new ArgumentException("Unknown ControlType");
            }

            var buttonCommands = new List<ModelCommand>();
            model.PageButtonComands(buttonCommands);

            var N = buttonCommands.Count;
            var M = PageButtonPanel.Children.Count;
            for (int i = 0; i < M; i++)
            {
                if (PageButtonPanel.Children[i] is Button btn)
                {
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
        public void SetSize(double width, double height)
        {
            ModelControl?.SetSize(width, height - ButtonGrid.ActualHeight);
        }


        public async void Dispatch(UIRequest rq)
        {
            await ModelPageService.Current.Dispatch(rq, this);
        }
        #endregion
    }
}
