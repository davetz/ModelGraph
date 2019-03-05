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
            Loaded += ModelGraphControl_Loaded;
        }

        private void ModelGraphControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Loaded -= ModelGraphControl_Loaded;
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

        #region IModelControl  ================================================
        public void Save() { }
        public void Release()
        {
            if (EditorCanvas == null) return;

            ReleaseControlPanel();

            EditorCanvas.RemoveFromVisualTree();
            EditorCanvas = null;
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


        //===========================================================================================Debug
        int? currentLevel, wantedLevel;

        // This implements requirement #1.
        Task levelLoadTask;


        public void LoadNewLevel(int newLevel)
        {
            Debug.Assert(levelLoadTask == null);
            wantedLevel = newLevel;
//            levelLoadTask = LoadResourcesForLevelAsync(EditorCanvas, newLevel);
        }

        //async Task LoadResourcesForLevelAsync(EditorCanvas resourceCreator, int level)
        //{
        //    //levelBackground = await CanvasBitmap.LoadAsync(resourceCreator, ...);
        //    //levelThingie = await CanvasBitmap.LoadAsync(resourceCreator, ...);
        //    // etc.
        //}

        private void EditorCanvas_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            {
                // Synchronous resource creation, for globally-required resources goes here:
                //x = new CanvasRenderTarget(sender, ...);
                //y = new CanvasRadialGradientBrush(sender, ...);
                // etc.

                args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
            }
        }
        async Task CreateResourcesAsync(CanvasControl sender)
        {
            // If there is a previous load in progress, stop it, and
            // swallow any stale errors. This implements requirement #3.
            if (levelLoadTask != null)
            {
                levelLoadTask.AsAsyncAction().Cancel();
                try { await levelLoadTask; } catch { }
                levelLoadTask = null;
            }

            // Unload resources used by the previous level here.

            // Asynchronous resource loading, for globally-required resources goes here:
           // baz = await CanvasBitmap.LoadAsync(sender, ...);
            //qux = await CanvasBitmap.LoadAsync(sender, ...);
            // etc.

            // If we are already in a level, reload its per-level resources.
            // This implements requirement #4.
            if (wantedLevel.HasValue)
            {
                LoadNewLevel(wantedLevel.Value);
            }
        }

        // Because of how this is designed to throw an exception, this must only 
        // ever be called from a Win2D event handler.
        bool IsLoadInProgress()
        {
            // No loading task?
            if (levelLoadTask == null)
            {
                return false;
            }

            // Loading task is still running?
            if (!levelLoadTask.IsCompleted)
            {
                return true;
            }

            // Query the load task results and re-throw any exceptions
            // so Win2D can see them. This implements requirement #2.
            try
            {
                levelLoadTask.Wait();
            }
            catch (AggregateException aggregateException)
            {
                // .NET async tasks wrap all errors in an AggregateException.
                // We unpack this so Win2D can directly see any lost device errors.
                aggregateException.Handle(exception => { throw exception; });
            }
            finally
            {
                levelLoadTask = null;
            }

            currentLevel = wantedLevel;
            return false;
        }

        public async void Dispatch(UIRequest rq)
        {
            await ModelPageService.Current.Dispatch(rq, this);
        }
    }
}

