using ModelGraph.Controls;
using ModelGraph.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ModelGraph.Views
{
    public sealed partial class ModelPage : Page, INotifyPropertyChanged
    {
        public ModelPageControl PageControl { get; private set; }

        #region Constructor  ==================================================
        public ModelPage()
        {
            InitializeComponent();
            SizeChanged += ModelPage_SizeChanged;
        }

        private void ModelPage_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            PageControl?.ModelControl?.SetSize(ActualWidth, ActualHeight - ButtonGrid.ActualHeight);
        }
        #endregion


        #region OnNavigatedTo  ================================================
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is ModelPageControl pageControl)
            {
                InitializeModel (pageControl);
            }
            else if (e.Parameter is ViewLifetimeControl viewControl && !(viewControl.PageControl is null))
            {
                InitializeModel(viewControl.PageControl);
            }
        }
        private void InitializeModel(ModelPageControl pageControl)
        {
            PageControl = pageControl;
            PageControl.Dispatcher = Dispatcher;

            if (pageControl.ModelControl is IModelControl ctrl)
            {
                ctrl.Clear();
            }
            switch (PageControl.RootModel.ControlType)
            {
                case ModelGraphSTD.ControlType.PrimaryTree:
                case ModelGraphSTD.ControlType.PartialTree:
                    pageControl.ModelControl = new ModelTreeControl(pageControl.RootModel);
                    break;

                case ModelGraphSTD.ControlType.SymbolEditor:
                    pageControl.ModelControl = new SymbolEditControl(pageControl.RootModel);
                    break;

                case ModelGraphSTD.ControlType.GraphDisplay:
                    pageControl.ModelControl = new ModelGraphControl(pageControl.RootModel);
                    break;
            }
            pageControl.RootModel.Chef.SetLocalizer(Helpers.ResourceExtensions.GetLocalizer());
            ControlGrid.Children.Add(pageControl.ModelControl as UserControl);
        }
        #endregion


        #region Button_Click  =================================================
        private void CloseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ModelPageService.RemoveModelPage(PageControl);
        }
        private void AppButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ModelPageService.RemoveModelPage(PageControl);
        }
        #endregion


        #region PropertyChanged  ==============================================
        public event PropertyChangedEventHandler PropertyChanged;
        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
