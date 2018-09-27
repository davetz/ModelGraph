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
            NavigatedTo(e.Parameter);
        }

        internal async void NavigatedTo(object parm)
        {
            if (parm is ModelPageControl pageControl)
            {
                NavigationService.ActiveModelPage = this; //enable the call to NavigationFrom()
                PageControl = pageControl;
                ControlGrid.Children.Add(PageControl);
            }
            else if (parm is ViewLifetimeControl viewControl && viewControl.PageControl is null && !(viewControl.RootModel is null))
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { viewControl.PageControl = new ModelPageControl(viewControl.RootModel); PageControl = viewControl.PageControl;  ControlGrid.Children.Add(viewControl.PageControl); });
            }
        }
        internal void NavigatedFrom()
        {
            ControlGrid.Children.Clear();
            PageControl = null;
        }
        #endregion
    }
}
