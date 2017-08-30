using ModelGraphLibrary;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ModelGraph
{
    public sealed partial class ModelTabControl : UserControl
    {
        private IPageControl _page;
        private List<Grid> _tabList;
        private List<RootModel> _modelList;
        private RootModel _selectModel;

        public ModelTabControl()
        {
            InitializeComponent();

            _iconStyle = Resources["IconStyle"] as Style;
            _nameStyle = Resources["NameStyle"] as Style;

            _closeBrush = Resources["CloseBrush"] as Brush;
            _hoverBrush = Resources["HoverBrush"] as Brush;
            _selectBrush = Resources["SelectBrush"] as Brush;

        }

        #region Properties  ===================================================
        private double _maxWidth = 120;
        private Style _iconStyle;
        private Style _nameStyle;

        private Brush _closeBrush;
        private Brush _hoverBrush;
        private Brush _selectBrush;

        private const string _masterIcon = "\uE24A";//
        private const string _windowIcon = "\uE78B";//
        private const string _vacantIcon = "\uE738";//
        private const string _symbolIcon = "\uECA7";//
        private const string _splitIcon = "\u1E7E";//
        private const string _closeIcon = "\uE10A";//
        private const string _graphIcon = "\uEBD2";//
        private const string _treeIcon = "\u7E1D";//

        private bool IsInvalid => (_page == null || Width == Double.NaN || Width == 0);
        #endregion

        #region Initialize  ===================================================
        internal void Initialize(IPageControl page, List<RootModel> modelList)
        {
            _page = page;
            _modelList = modelList;
            _tabList = new List<Grid>();
            CloseBlock.Text = _closeIcon;
        }
        #endregion

        #region Refresh  ======================================================
        internal void Refresh(RootModel selectModel, double maxWidth)
        {
            if (IsInvalid) return;
            _selectModel = selectModel;

            ValidateTabList();

            var height = TabCanvas.Height = CloseGrid.Height = Height;

            var N = _modelList.Count;

            var tabWidth = maxWidth / N;
            if (tabWidth > _maxWidth) tabWidth = _maxWidth;
            TabCanvas.Width = Width = tabWidth * N;

            var textWidth = tabWidth - 20;

            for (int i = 0; i < N; i++)
            {
                var isSelect = false;
                if (selectModel == _modelList[i])
                {
                    isSelect = true;
                }

                var g = _tabList[i];
                g.Tag = _modelList[i];
                var k = g.Children[0] as TextBlock;
                var n = g.Children[1] as TextBlock;
                var m = _modelList[i];
                n.Text = m.TabName;


                g.Background = (isSelect) ? _selectBrush : null;

                g.Height = height;
                g.Width = tabWidth;
                n.Width = textWidth;

                Canvas.SetLeft(g, tabWidth * i);

                switch(m.ControlType)
                {
                    case ControlType.AppRootChef:
                        g.CanDrag = false;
                        k.Text = _masterIcon;
                        break;
                    case ControlType.PrimaryTree:
                        k.Text = _treeIcon;
                        break;
                    case ControlType.PartialTree:
                        k.Text = _treeIcon;
                        break;
                    case ControlType.GraphDisplay:
                        k.Text = _graphIcon;
                        break;
                    case ControlType.SymbolEditor:
                        k.Text = _symbolIcon;
                        break;
                }
            }
        }
        #endregion

        #region ValidateTabList  ==============================================
        private void ValidateTabList()
        {
            var E = _modelList.Count - _tabList.Count;
            if (E > 0)
            {
                for (int i = 0; i < E; i++)
                {
                    var g = new Grid();
                    var k = new TextBlock();
                    var n = new TextBlock();
                    k.Style = _iconStyle;
                    n.Style = _nameStyle;
                    g.Children.Add(k);
                    g.Children.Add(n);
                    TabCanvas.Children.Add(g);
                    _tabList.Add(g);
                    
                    g.CanDrag = true;
                    g.AllowDrop = true;
                    g.DragStarting += TabItem_DragStarting;
                    g.DragOver += TabItem_DragOver;
                    g.Drop += TabItem_DragDrop;
                }
            }
            else if ( E < 0)
            {
                var j = _tabList.Count - 1;
                var P = -E;
                for (int i = 0; i < P; i++, j--)
                {
                    var g = _tabList[j];
                    _tabList.Remove(g);
                    TabCanvas.Children.Remove(g);
                }
            }
        }
        #endregion

        #region DragDrop  =====================================================
        private void TabItem_DragOver(object sender, DragEventArgs e)
        {
            RootModel m;
            if (TryGetModel(sender, out m))
                m.Page.TabItem_DragOver(m, e);
        }

        private void TabItem_DragDrop(object sender, DragEventArgs e)
        {
            RootModel m;
            if (TryGetModel(sender, out m))
                m.Page.TabItem_DragDrop(m, e);
        }

        private void TabItem_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            args.AllowedOperations = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            RootModel m;
            if (TryGetModel(sender, out m))
                m.Page.TabItem_DragStarting(m);
        }
        static private bool TryGetModel(object sender, out RootModel model)
        {
            model = null;

            var g = sender as Grid;
            if (g == null) return false;

            var m = g.Tag as RootModel;
            if (m == null) return false;

            model = m;
            return true;
        }
        #endregion

        #region HitTest  ======================================================
        RootModel _hoverModel;
        private void HitTest(double x)
        {
            if (IsInvalid) return;

            _hoverModel = null;

            var closeVisibility = Visibility.Collapsed;
            var N = _modelList.Count;
            if (_tabList.Count != N) return;

            for (int i = 0; i < N; i++)
            {
                var g = _tabList[i];
                var x1 = Canvas.GetLeft(g);
                var x2 = x1 + g.Width;

                if (x < x1 || x > x2)
                {
                    if (g.Background == null) continue;
                    if (g.Background == _selectBrush) continue;
                    g.Background = null;
                }
                else
                {
                    _hoverModel = _modelList[i];
                    if(_hoverModel.ControlType != ControlType.AppRootChef)
                    {
                        var xl = x2 - CloseBlock.Width;
                        Canvas.SetLeft(CloseGrid, xl);
                        CloseGrid.Tag = _hoverModel;
                        closeVisibility = Visibility.Visible;
                    }
                    if (g.Background == _selectBrush) continue;
                    g.Background = _hoverBrush;
                }
            }
            CloseGrid.Visibility = closeVisibility;
        }
        #endregion

        #region PointerEvent  =================================================
        private void TabCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var cp = e.GetCurrentPoint(TabCanvas);
            HitTest(cp.Position.X);
        }

        private void TabCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            HitTest(-99);
        }

        private void TabCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _page.LoadModelView(_hoverModel);
        }
        private void CloseGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var g = sender as Grid;
            if (g == null) return;
            var m = g.Tag as RootModel;
            if (m == null) return;

            e.Handled = true;

            CloseGrid.Background = _hoverBrush;
            CloseGrid.Visibility = Visibility.Collapsed;

           _page.CloseModelView(m);
        }
        private void CloseGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CloseGrid.Background = _closeBrush;
        }

        private void CloseGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            CloseGrid.Background = _hoverBrush;
        }
        #endregion

    }
}
