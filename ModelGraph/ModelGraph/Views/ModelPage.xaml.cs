using ModelGraph.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

            Button1.Content = "Close";
            Button1.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }


        private void CloseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ModelPageService.RemoveModelPage(PageControl);
        }
        private void AppButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ModelPageService.RemoveModelPage(PageControl);
        }



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
    }
}
