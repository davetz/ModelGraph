using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ModelGraph.Views
{
    public sealed partial class ModelPage : Page
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
            Window.Current.Close();
        }


        private void AppButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
