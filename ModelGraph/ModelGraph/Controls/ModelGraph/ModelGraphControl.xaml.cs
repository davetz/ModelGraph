using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using ModelGraphSTD;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ModelGraph.Services;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace ModelGraph.Controls
{
    public sealed partial class ModelGraphControl : Page, IPageControl, IModelPageControl
    {
        private Chef _chef;
        private Graph _graph;
        private readonly Size _desiredSize = new Size { Height = 800, Width = 800 };
        public RootModel RootModel { get; }

        public ModelGraphControl(RootModel model)
        {
            _chef = model.Chef;
            RootModel = model;
            _graph = model.Graph;

            _selector = new Selector(_graph);

            InitializeComponent();
            InitializeControlPanel();
        }
        private void EditorCanvas_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Loaded -= EditorCanvas_Loaded;

            //ApplicationView.GetForCurrentView().TryResizeView(_desiredSize); //problem: also resizes the shellPage window ???

            Focus(Windows.UI.Xaml.FocusState.Programmatic);

            CheckGraphSymbols();

            SetIdleOnVoid();
        }


        [Flags]
        private enum Modifier
        {
            None = 0,
            Menu = 1,
            Ctrl = 2,
            Shift = 4,
        }

        #region IPageControl  =================================================
        public async void Dispatch(UIRequest rq)
        {
            await ModelPageService.Current.Dispatch(rq, this);
        }
        #endregion

        #region IModelControl  ================================================
        public void Save() { }
        public void Release()
        {
            if (EditorCanvas == null) return;

            ReleaseControlPanel();

            EditorCanvas.RemoveFromVisualTree();
            EditorCanvas = null;
            RootModel?.Release();
        }
        public void Reload() { }
        public void Refresh()
        {
            if (EditorCanvas == null) return;

            CheckGraphSymbols();
            EditorCanvas.Invalidate();
        }
        private void CheckGraphSymbols()
        {
            var N = _graph.SymbolCount;
            if (N > 0)
            {

                bool anyChange = (N == _symbol_version.Count);
                if (!anyChange)
                {
                    for (int i = 0; i < N; i++)
                    {
                        var sym = _graph.Symbols[i];
                        if (_symbol_version.TryGetValue(sym, out (byte indx, byte vers) val) && val.indx == i && val.vers == sym.Version) continue;
                        anyChange = true;
                        break;
                    }
                }

                if (anyChange)
                {
                    _symbol_version.Clear();
                    _symbolShapes.Clear();
                    for (int i = 0; i < N; i++)
                    {
                        var sym = _graph.Symbols[i];
                        _symbol_version[sym] = ((byte)i, sym.Version);
                        var shapes = new List<Shape>(8);
                        Shape.Deserialize(sym.Data, shapes);
                        _symbolShapes.Add(shapes);
                    }
                }
            }
            else
            {
                _symbolShapes.Clear();
                _symbol_version.Clear();
            }
        }
        private Dictionary<SymbolX, (byte indx, byte vers)> _symbol_version = new Dictionary<SymbolX, (byte, byte)>();


        // needed because win2D.uwp canvaseControl is implemented in c++ (prevent memory leaks)
        private void ModelGraphControl_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (EditorCanvas == null) return;

            EditorCanvas.RemoveFromVisualTree();
            EditorCanvas = null;
        }
        public (int Width, int Height) PreferredSize => (400, 320);
        public void SetSize(double width, double height)
        {
            if (EditorCanvas == null) return;

            CanvasGrid.Width = RootCanvas.Width = EditorCanvas.Width = this.Width = width;
            CanvasGrid.Height = RootCanvas.Height = EditorCanvas.Height = this.Height = height;
        }
        #endregion
    }
}

