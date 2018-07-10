using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ModelGraphSTD;


namespace ModelGraphUWP
{
    public sealed partial class ModelGraphControl : UserControl, IModelControl
    {
        private Chef _chef;
        private Graph _graph;
        private RootModel _model;

        public ModelGraphControl(RootModel model)
        {
            _chef = model.Chef;
            _model = model;
            _model.ModelControl = this;
            _graph = model.Graph;
            _selector = new Selector(_graph);

            InitializeComponent();
            Loaded += ModelGraphControl_Loaded;
        }

        private void ModelGraphControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Loaded -= ModelGraphControl_Loaded;
            Focus(Windows.UI.Xaml.FocusState.Programmatic);

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

        public (int Width, int Height) PreferredMinSize => (400, 320);

        public void SetSize(double width, double height)
        {
            if (DrawCanvas == null) return;

            RootGrid.Width = RootCanvas.Width = DrawCanvas.Width = this.Width = width;
            RootGrid.Height = RootCanvas.Height = DrawCanvas.Height = this.Height = height;
        }

        public void Close()
        {
            if (DrawCanvas == null) return;

            DrawCanvas.RemoveFromVisualTree();
            DrawCanvas = null;
        }



        public void Refresh()
        {
            if (DrawCanvas == null) return;

            DrawCanvas.Invalidate();
        }

        // needed because win2D.uwp canvaseControl is implemented in c++ (prevent memory leaks)
        private void ModelGraphControl_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (DrawCanvas == null) return;

            DrawCanvas.RemoveFromVisualTree();
            DrawCanvas = null;
        }

        //===========================================================================================Debug
        int? currentLevel, wantedLevel;

        // This implements requirement #1.
        Task levelLoadTask;


        public void LoadNewLevel(int newLevel)
        {
            Debug.Assert(levelLoadTask == null);
            wantedLevel = newLevel;
//            levelLoadTask = LoadResourcesForLevelAsync(DrawCanvas, newLevel);
        }

        //async Task LoadResourcesForLevelAsync(DrawCanvas resourceCreator, int level)
        //{
        //    //levelBackground = await CanvasBitmap.LoadAsync(resourceCreator, ...);
        //    //levelThingie = await CanvasBitmap.LoadAsync(resourceCreator, ...);
        //    // etc.
        //}

        private void DrawCanvas_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
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

    }
}

