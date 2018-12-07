using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ModelGraph.Services;
using ModelGraphSTD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Color = Windows.UI.Color;

namespace ModelGraph.Controls
{
    public sealed partial class SymbolEditControl : Page, IPageControl, IModelPageControl
    {
        private RootModel _rootModel;

        public SymbolEditControl()
        {
            this.InitializeComponent();
        }

        public SymbolEditControl(RootModel model)
        {
            _rootModel = model;
            this.InitializeComponent();
        }

        public async void Dispatch(UIRequest rq)
        {
            await ModelPageService.Current.Dispatch(rq, this);
        }
        public RootModel RootModel => _rootModel;
        #region IModelControl  ================================================
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

        #region CanvasDraw  ===================================================
        #region SelectCanvas_Draw  ===============================================
        private void SelectCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
        }
        #endregion

        #region DrawCanvas_Draw  ==============================================
        private void DrawCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            DrawGrid(ds);
            DrawSegment(ds);
            DrawHitLine(ds);
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
        private void DrawSegment(CanvasDrawingSession ds)
        {
        }
        #endregion

        #region DrawGrid  =====================================================
        private void DrawGrid(CanvasDrawingSession ds)
        {
            var gridColor = Color.FromArgb(0x80, 0xff, 0xff, 0xff);
            var axisColor = Color.FromArgb(0xff, 0xff, 0xff, 0xff);

            float d = _offset + _maxSize * _zoomFactor;
            float o = _offset;

            for (int i = 0; i <= _maxSize; i++)
            {
                var y = _offset + i * _zoomFactor;
                var c = (i % 4 == 0) ? axisColor : gridColor;
                ds.DrawLine(o, y, d, y, c);
            }
            for (int i = 0; i <= _maxSize; i++)
            {
                var x = _offset + i * _zoomFactor;
                var c = (i % 4 == 0) ? axisColor : gridColor;
                ds.DrawLine(x, o, x, d, c);
            }

            float p = _offset + 7 * _zoomFactor;
            float q = _offset + 17 * _zoomFactor;
            float r = (_maxSize / 2) * _zoomFactor;
            float s = _offset + r;

            ds.DrawCircle(new System.Numerics.Vector2(s, s), r, axisColor);

            ds.DrawLine(o, p, d, q, gridColor);
            ds.DrawLine(p, o, q, d, gridColor);
            ds.DrawLine(o, q, d, p, gridColor);
            ds.DrawLine(p, d, q, o, gridColor);
            ds.DrawLine(o, o, d, d, gridColor);
            ds.DrawLine(o, d, d, o, gridColor);
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

        #region DrawCanvas_PointerEvents  =====================================
        //                     BeginAction,   DragAction,   EndAction
        private void DrawCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _pointerIsPressed = true;
            _rawRef = RawPoint(e);
            _drawRef.Point1 = DrawPoint(e);

            BeginAction?.Invoke();
        }

        private void DrawCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _drawRef.Point2 = DrawPoint(e);
            if (_pointerIsPressed && DragAction != null)
            {
                DragAction();
            }
        }

        private void DrawCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _pointerIsPressed = false;
            _drawRef.Point2 = DrawPoint(e);

            EndAction?.Invoke();
        }

        private (float X, float Y) RawPoint(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(DrawCanvas).Position;
            var x = (p.X - _offset);
            var y = (p.Y - _offset);
            return ((int)x, (int)y);
        }

        private (float X, float Y) DrawPoint(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(DrawCanvas).Position;
            var x = Round((p.X - _offset) / _zoomFactor);
            var y = Round((p.Y - _offset) / _zoomFactor);
            return (x, y);
        }
        private int Round(double v)
        {
            var n = (int)v;
            var m = n + 1;
            return ((m - v) < (v - n)) ? m : n;
        }
        #endregion

    }
}
