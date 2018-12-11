using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ModelGraph.Services;
using ModelGraphSTD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Color = Windows.UI.Color;

namespace ModelGraph.Controls
{
    public sealed partial class SymbolEditControl : Page, IPageControl, IModelPageControl
    {
        private RootModel _rootModel;
        private List<Shape> _shapeList = new List<Shape>();
        private Shape _selectedShape;
        private Func<Vector2, Shape> CreateShapeFunction;
        private int _shapeIndex = -1;
        private Vector2 Point1;
        private Vector2 Point2;
        private const float WorkSize = 400;
        private const float WorkMargin = 24;
        private const float WorkCenter = WorkMargin + WorkSize / 2;

        public SymbolEditControl()
        {
            this.InitializeComponent();
        }

        public SymbolEditControl(RootModel model)
        {
            _rootModel = model;
            this.InitializeComponent();
        }

        #region IPageControl  =================================================
        public async void Dispatch(UIRequest rq)
        {
            await ModelPageService.Current.Dispatch(rq, this);
        }
        #endregion

        #region IModelControl  ================================================
        public RootModel RootModel => _rootModel;
        public void Save()
        {
            //            _symbol.Data = PackageSymbolData();
            DrawCanvas.Invalidate();
        }

        public void Release()
        {
            if (DrawCanvas != null)
            {
                DrawCanvas.RemoveFromVisualTree();
                DrawCanvas = null;
            }
        }
        public void Reload()
        {
            DrawCanvas.Invalidate();
        }

        public void Refresh()
        {
        }
        public (int Width, int Height) PreferredSize => (504, 360);
        public void SetSize(double width, double hieght)
        {
            this.Width = RootGrid.Width = width;
            this.Height = RootGrid.Height = hieght;
        }
        #endregion

        #region ViewEvents  ===================================================

        private void UserControl_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (DrawCanvas != null)
            {
                DrawCanvas.RemoveFromVisualTree();
                DrawCanvas = null;
            }
        }

        private void DrawCanvas_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DrawCanvas.Invalidate();
        }

        #endregion

        #region DrawCanvas  ===================================================

        #region SelectCanvas_Draw  ============================================
        private void SelectCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
        }
        #endregion

        #region ShapeCanvas_Draw  =============================================
        private List<Shape> _shapes = new List<Shape> { new PolyLine(Vector2.Zero), new Circle(Vector2.Zero) };
        private void ShapeCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            var W = (float)ShapeCanvas.Width;
            var HW = W / 2;
            var scale = (W - 4) / Shape.FullSize;

            var n = _shapes.Count;
            for (int i = 0; i < n; i++)
            {
                var center = new Vector2(HW, (i * W) + HW);
                var shape = _shapes[i];
                var color = shape.Color;
                shape.Draw(ShapeCanvas, ds, scale, center, _shapeIndex);
            }
        }
        #endregion

        #region DrawCanvas_Draw  ==============================================
        private void DrawCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            DrawGrid(ds);
            foreach (var shape in _shapeList)
            {
                shape.Draw(DrawCanvas, ds, WorkSize / Shape.FullSize, new Vector2(WorkCenter));
            }
        }
        #endregion

        #region DrawingStyles  ================================================
        private Color traceHeavyColor1 = Colors.LightGray;//Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF);
        private Color traceLightColor2 = Colors.DimGray;//Color.FromArgb(0x20, 0xFF, 0xFF, 0xFF); 

        private Color regionColor = Colors.LightGray; //Color.FromArgb(0x80, 0x00, 0x00, 0x00);
        private Color nodeColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00);
        private Color edgeColor = Color.FromArgb(0xFF, 0xFF, 0x7F, 0xFF);

        public static List<T> GetEnumAsList<T>() { return Enum.GetValues(typeof(T)).Cast<T>().ToList(); }
        public List<CanvasDashStyle> DashStyles { get { return GetEnumAsList<CanvasDashStyle>(); } }
        public List<CanvasCapStyle> CapStyles { get { return GetEnumAsList<CanvasCapStyle>(); } }
        public List<CanvasLineJoin> LineJoins { get { return GetEnumAsList<CanvasLineJoin>(); } }
        public CanvasCapStyle StartCap { get; set; }
        public CanvasCapStyle EndCap { get; set; }
        public CanvasDashStyle DashStyle { get; set; }
        public CanvasCapStyle DashCap { get; set; }
        public CanvasLineJoin LineJoin { get; set; }

        CanvasStrokeStyle strokeStyle = new CanvasStrokeStyle();
        CanvasStrokeStyle hitLineStyle = new CanvasStrokeStyle();
        #endregion

        #region DrawHitLine  ==================================================
        private void DrawHitLine(CanvasDrawingSession ds)
        {
        }
        private void DrawHitLine(CanvasDrawingSession ds, Color c, float w)
        {
        }
        #endregion



        #region DrawSegment  ==================================================
        private void DrawSelectedRectangle(CanvasDrawingSession ds)
        {
            
        }
        #endregion

        #region DrawGrid  =====================================================
        private const int _workAxis = (int)(WorkSize / 4);
        private const int _workGrid = (int)(WorkSize / 16);
        private const int _workTick = (int)(_workGrid / 2);
        private void DrawGrid(CanvasDrawingSession ds)
        {
            var color1 = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
            var color2 = Color.FromArgb(0xff, 0xff, 0xff, 0x80);
            var color3 = Color.FromArgb(0x80, 0xff, 0xff, 0x00);
            var color4 = Color.FromArgb(0x40, 0xff, 0xff, 0xff);

            var a = WorkMargin;
            var b = a + WorkSize;
            var c = WorkCenter;
            var r = WorkSize / 2;
            var m = WorkSize + 2 * WorkMargin;

            var d = r * Math.Sin(Math.PI / 8);
            var e = (float)(c - d);
            var f = (float)(c + d);

            for (int i = 0; i <= WorkSize; i += _workGrid)
            {
                var z = a + i;
                ds.DrawLine(z, a, z, b, color3);
                ds.DrawLine(a, z, b, z, color3);
            }
            ds.DrawLine(a, a, b, b, color1);
            ds.DrawLine(a, b, b, a, color1);

            ds.DrawLine(a, e, b, f, color4);
            ds.DrawLine(e, a, f, b, color4);

            ds.DrawLine(a, f, b, e, color4);
            ds.DrawLine(f, a, e, b, color4);

            ds.DrawCircle(c, c, r, color2);
            ds.DrawCircle(c, c, r / 2, color4);

            for (int i = 0; i <= WorkSize; i += _workAxis)
            {
                var z = a + i;
                ds.DrawLine(z, a, z, b, color1);
                ds.DrawLine(a, z, b, z, color1);
            }
            var xC = c - 6;
            var yN = -2;
            var yS = b - 3;
            ds.DrawText("N", xC, yN, color1);
            ds.DrawText("S", xC, yS, color1);

            var xE = b + 3;
            var xW = 2;
            var yC = c - 14;
            ds.DrawText("E", xE, yC, color1);
            ds.DrawText("W", xW, yC, color1);
        }
        #endregion

        #endregion


        #region Fields  =======================================================
        private SolidColorBrush activeColor;
        private SolidColorBrush inactiveColor;


        private const int _maxSize = 32;
        private const float _zoomFactor = 12;
        private const int _offset = 16;
        private const int _indexDelta = 36;

        private Action EndAction;
        private Action DragAction;
        private Action BeginAction;

        private (float X, float Y) _rawRef;
        private Extent _drawRef = new Extent();
        private bool _pointerIsPressed;
        private bool _isAllSelected;

        Windows.ApplicationModel.Resources.ResourceLoader _resourceLoader;

        private static int _ds = GraphDefault.HitMargin;
        #endregion

        #region SelectCanvas_PointerEvents  ===================================
        //                     BeginAction,   DragAction,   EndAction
        private void SelectCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
        }

        private void SelectCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
        }

        private void SelectCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
        }
        #endregion

        #region ShapeCanvas_PointerEvents  ====================================
        //                     BeginAction,   DragAction,   EndAction
        private void ShapeCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var index = ShapeIndex(e);
            _shapeIndex = (index < 0 || index > _shapes.Count) ? -1 : index;
            ShapeCanvas.Invalidate();
        }

        private void ShapeCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
        }

        private void ShapeCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var index = ShapeIndex(e);
            if (index == _shapeIndex)
            {
                CreateShapeFunction = _shapes[index].CreateShapeFunction;

                BeginAction = TryAddNewShape;
                DragAction = DragShape;
                EndAction = EndDragShape;
            }
        }
        private int ShapeIndex(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(ShapeCanvas).Position;
            return (int)(p.Y / ShapeCanvas.Width);
        }
        #endregion

        #region DrawCanvas_PointerEvents  =====================================
        //                     BeginAction,   DragAction,   EndAction
        private void DrawCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _pointerIsPressed = true;
            Point1 = DrawPoint(e);

            BeginAction?.Invoke();
        }

        private void DrawCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Point2 = DrawPoint(e);

            if (_pointerIsPressed && DragAction != null)
            {
                DragAction();
            }
        }

        private void DrawCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _pointerIsPressed = false;

            EndAction?.Invoke();
        }


        private Vector2 DrawPoint(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(DrawCanvas).Position;
            return new Vector2((float)p.X - WorkMargin, (float)p.Y - WorkMargin);
        }
        private int Round(double v)
        {
            var n = (int)v;
            var m = n + 1;
            return ((m - v) < (v - n)) ? m : n;
        }
        #endregion

        #region DrawCanvas_Actions  ===========================================
        private void TryAddNewShape()
        {
            if (CreateShapeFunction is null) return;

            var delta = (Point1 - new Vector2(WorkSize / 2)) * (Shape.FullSize / WorkSize);
            _shapeWasDraged = false;
            _selectedShape = CreateShapeFunction(delta);
            
            _shapeList.Add(_selectedShape);

            ShapeCanvas.Invalidate();
            DrawCanvas.Invalidate();
        }
        private bool _shapeWasDraged;
        private void DragShape()
        {
            if (_selectedShape is null) return;

            var delta = (Point2 - Point1) * (Shape.FullSize / WorkSize);
            if (delta.LengthSquared() < 1) return;

            BeginAction = null;
            Point1 = Point2;

            _selectedShape.Move(delta);
            _shapeWasDraged = true;
            DrawCanvas.Invalidate();
        }
        private void EndDragShape()
        {
            if (_shapeWasDraged)
            {
                BeginAction = null;
                DragAction = null;
                EndAction = null;
                _selectedShape = null;
                _shapeIndex = -1;
                ShapeCanvas.Invalidate();
            }
        }
        #endregion
    }
}
