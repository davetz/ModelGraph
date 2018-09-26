using ModelGraph.Services;
using ModelGraphSTD;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ModelGraph.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();
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

        private async void NewButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ModelPageService.Current.CreateNewModelAsync(Dispatcher);
        }
        private async void OpenButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ModelPageService.Current.OpenModelDataFileAsync(Dispatcher);
        }
    }
}
