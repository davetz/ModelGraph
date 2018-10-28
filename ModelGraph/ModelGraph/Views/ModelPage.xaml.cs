using ModelGraph.Controls;
using ModelGraph.Helpers;
using ModelGraph.Services;
using ModelGraphSTD;
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
        public IModelControl ModelControl { get; private set; }

        #region Constructor  ==================================================
        public ModelPage()
        {
            InitializeComponent();
            SizeChanged += ModelPage_SizeChanged;
        }

        private void ModelPage_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            ModelControl?.SetSize(ActualWidth, ActualHeight);
        }
        #endregion

        #region NavigatedTo/From  =============================================
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {             
            NavigatedTo(e.Parameter);
        }

        internal async void NavigatedTo(object parm)
        {
            if (parm is RootModel model)
            {
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
                        var sc = new SymbolEditControl(model);
                        ModelControl = sc;
                        ControlGrid.Children.Add(sc);
                        break;

                    case ControlType.GraphDisplay:
                        var gc = new ModelGraphControl(model);
                        ModelControl = gc;
                        ControlGrid.Children.Add(gc);
                        break;

                    default:
                        throw new ArgumentException("Unknown ControlType");
                }
            }
            //else if (parm is ViewLifetimeControl viewControl && viewControl.PageControl is null && !(viewControl.RootModel is null))
            //{
            //    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { viewControl.PageControl = new IModelControl(viewControl.RootModel); ModelControl = viewControl.PageControl;  ControlGrid.Children.Add(viewControl.PageControl); });
            //}
        }
        internal void NavigatedFrom()
        {
            ControlGrid.Children.Clear();
            ModelControl = null;
        }
        #endregion
    }
}
