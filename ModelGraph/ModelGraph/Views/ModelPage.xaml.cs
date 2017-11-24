using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ModelGraph.Views
{
    public sealed partial class ModelPage : Page, INotifyPropertyChanged
    {
        ModelPageControl _pageControl;

        public ModelPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _pageControl = (ModelPageControl)e.Parameter;

            // When this view is finally release, clean up state
            _pageControl.Released += ModelPageControl_Released;
        }
        private void ModelPageControl_Released(Object sender, EventArgs e)
        {
            ((ModelPageControl)sender).Released -= ModelPageControl_Released;
            // The ViewLifetimeControl object is bound to UI elements on the main thread
            // So, the object must be removed from that thread
            //await mainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    ((App)App.Current).SecondaryViews.Remove(thisViewControl);
            //});

            // The released event is fired on the thread of the window
            // it pertains to.
            //
            // It's important to make sure no work is scheduled on this thread
            // after it starts to close (no data binding changes, no changes to
            // XAML, creating new objects in destructors, etc.) since
            // that will throw exceptions
            Window.Current.Close();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
