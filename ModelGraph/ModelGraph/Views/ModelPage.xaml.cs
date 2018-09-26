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
    public sealed partial class ModelPage : Page
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
            PageControl?.SetSize(ActualWidth, ActualHeight);
        }
        #endregion

        #region NavigatedTo/From  =============================================
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.ActiveModelPage = this; //enables call to NavigationFrom()

            if (e.Parameter is ModelPageControl pageControl)
            {
                ControlGrid.Children.Add(pageControl);
            }
            else if (e.Parameter is ViewLifetimeControl viewControl && !(viewControl.PageControl is null))
            {
                ControlGrid.Children.Add(viewControl.PageControl);
            }
        }
        internal void NavigatedFrom()
        {
            ControlGrid.Children.Clear();
        }
        #endregion
    }
}
