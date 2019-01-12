using ModelGraph.Controls;
using ModelGraph.Helpers;
using ModelGraph.Services;
using ModelGraphSTD;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ModelGraph.Views
{
    public sealed partial class ModelPage : Page
    {
        IModelPageControl PageControl;

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

        internal void NavigatedTo(object parm)
        {
            if (parm is RootModel m1)
            {
                GetModelControl(m1);
                NavigationService.ActiveModelPage = this;
            }
            else if (parm is ViewLifetimeControl viewControl && viewControl.RootModel is RootModel m2)
            {
                GetModelControl(m2);
                //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { viewControl.PageControl = new IModelControl(viewControl.RootModel); ModelControl = viewControl.PageControl;  ControlGrid.Children.Add(viewControl.PageControl); });
            }
            ControlGrid.Children.Add(PageControl as UIElement);


            void GetModelControl(RootModel m)
            {
                if (m.PageControl is null)
                {
                    m.Chef.SetLocalizer(ResourceExtensions.GetLocalizer());

                    switch (m.ControlType)
                    {
                        case ControlType.PrimaryTree:
                        case ControlType.PartialTree: m.PageControl = new ModelTreeControl(m); break;

                        case ControlType.SymbolEditor: m.PageControl = new SymbolEditControl(m); break;

                        case ControlType.GraphDisplay: m.PageControl = new ModelGraphControl(m); break;

                        default:
                            throw new ArgumentException("Unknown ControlType");
                    }
                }
                PageControl = m.PageControl as IModelPageControl;
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
