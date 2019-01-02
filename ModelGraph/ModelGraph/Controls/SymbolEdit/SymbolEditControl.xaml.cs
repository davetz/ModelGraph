﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ModelGraph.Services;
using ModelGraphSTD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Color = Windows.UI.Color;

namespace ModelGraph.Controls
{
    #region Enums  ============================================================
    public enum Fill_Stroke
    {
        Stroke = 0,
        Filled = 1
    }
    public enum PolygonSides
    {
        _3 = 3,
        _4 = 4,
        _5 = 5,
        _6 = 6,
        _7 = 7,
        _8 = 8,
        _9 = 9,
        _10 = 10
    }
    #endregion

    public sealed partial class SymbolEditControl : Page, IPageControl, IModelPageControl, INotifyPropertyChanged
    {
        private RootModel _rootModel;
        private List<Shape> SymbolShapes = new List<Shape>();
        private List<Shape> PickerShapes = new List<Shape> { new Circle(Vector2.Zero), new Ellipes(Vector2.Zero), new Rectangle(Vector2.Zero), new RoundedRectangle(Vector2.Zero) };
        private Shape PickerShape;

        private Vector2 Point1; // pointer down coordinate
        private Vector2 Point2; // pointer up / move coordinate

        private const float EDITSize = 512;  //max size of shape being edited
        private const float EDITMargin = 24; //size of empty space arround the shape beint edited 
        private const float EDITCenter = EDITMargin + EDITSize / 2; //center of editor canvase
        private static Vector2 Center = new Vector2(EDITCenter);

        public SymbolEditControl()
        {
            this.InitializeComponent();

            _hasSharedShapes = _sharedShapes.Count > 0;
            _fillStroke = Fill_Stroke.Stroke;
            _polygonSide = PolygonSides._3;
        }

        public SymbolEditControl(RootModel model)
        {
            _rootModel = model;
            this.InitializeComponent();

            _hasSharedShapes = _sharedShapes.Count > 0;
            _fillStroke = Fill_Stroke.Stroke;
            _polygonSide = PolygonSides._3;
            _strokeWidth = 1;
            _centralSizeMax = _verticalSizeMax = _horizontalSizeMax = 100;
            _shapeColor = Colors.White;
            ToggleOneManyButton();
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
            EditorCanvas.Invalidate();
        }

        public void Release()
        {
            if (EditorCanvas != null)
            {
                EditorCanvas.RemoveFromVisualTree();
                EditorCanvas = null;
            }
        }
        public void Reload()
        {
            EditorCanvas.Invalidate();
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

        #region SymbolEditControl_Unloaded  ===================================
        private void SymbolEditControl_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (SelectorCanvas0 != null)
            {
                SelectorCanvas0.RemoveFromVisualTree();
                SelectorCanvas0 = null;
            }
            if (SelectorCanvas1 != null)
            {
                SelectorCanvas1.RemoveFromVisualTree();
                SelectorCanvas1 = null;
            }
            if (EditorCanvas != null)
            {
                EditorCanvas.RemoveFromVisualTree();
                EditorCanvas = null;
            }
            if (PickerCanvas != null)
            {
                PickerCanvas.RemoveFromVisualTree();
                PickerCanvas = null;
            }
        }

        private void EditorCanvas_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            EditorCanvas.Invalidate();
        }

        #endregion

        #region SelectorCanvas0_Draw  =========================================
        private void SelectorCanvas0_Draw(CanvasControl canvas, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            var W = (float)canvas.Width;
            var HW = W / 2;
            var scale = (W - 2) / Shape.FULLSIZE;
            var n = SymbolShapes.Count;

            var center = new Vector2(HW, HW);
            for (int i = 0; i < n; i++)
            {
                var shape = SymbolShapes[i];
                var strokeWidth = shape.StrokeWidth;

                shape.Draw(canvas, ds, scale, center, strokeWidth);
            }
        }
        #endregion

        #region SelectorCanvas1_Draw  =========================================
        private void SelectorCanvas1_Draw(CanvasControl canvas, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            var W = (float)canvas.Width;
            var HW = W / 2;
            var scale = (W - 2) / Shape.FULLSIZE;
            var n = SymbolShapes.Count;

            var center_0 = new Vector2(HW, HW);
            for (int i = 0; i < n; i++)
            {
                var center = new Vector2(HW, (i * W) + HW);
                var shape = SymbolShapes[i];
                var strokeWidth = shape.StrokeWidth;
                if (shape.IsSelected) Shape.HighLight(ds, W, i);

                shape.Draw(canvas, ds, scale, center, strokeWidth);
            }
        }
        #endregion

        #region PickerCanvas_Draw  ============================================
        private void PickerCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            var W = (float)sender.Width;
            var HW = W / 2;
            var scale = (W - 2) / Shape.FULLSIZE;

            var strokeWidth = 3;

            var n = PickerShapes.Count;
            for (int i = 0; i < n; i++)
            {
                var center = new Vector2(HW, (i * W) + HW);
                var shape = PickerShapes[i];

                if (shape == PickerShape) Shape.HighLight(ds, W, i);
                shape.Draw(sender, ds, scale, center, strokeWidth);
            }
        }
        #endregion

        #region EditorCanvas_Draw  ============================================
        private void EditorCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            var scale = EDITSize / Shape.FULLSIZE;

            DrawEditorBackgroundGrid(ds);
            foreach (var shape in SymbolShapes)
            {
                var strokeWidth = shape.StrokeWidth * scale * 5;
                shape.Draw(EditorCanvas, ds, scale, Center, strokeWidth);
            }
            DrawSelectTargets();

            SelectorCanvas0.Invalidate();
            SelectorCanvas1.Invalidate();

            void DrawSelectTargets()
            {
                var list = GetSelectedShapes();
                if (list is null) return;

                Shape.DrawTargets(list, ds, scale, Center);
            }
        }
        #endregion

        #region DrawSelectTargets  ============================================
        #endregion

        #region DrawEditorBackgroundGrid  =====================================
        private const int _workAxis = (int)(EDITSize / 4);
        private const int _workGrid = (int)(EDITSize / 16);
        private const int _workTick = (int)(_workGrid / 2);
        private void DrawEditorBackgroundGrid(CanvasDrawingSession ds)
        {
            var color1 = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
            var color2 = Color.FromArgb(0xff, 0xff, 0xff, 0x80);
            var color3 = Color.FromArgb(0x80, 0xff, 0xff, 0x00);
            var color4 = Color.FromArgb(0x40, 0xff, 0xff, 0xff);

            var a = EDITMargin;
            var b = a + EDITSize;
            var c = EDITCenter;
            var r = EDITSize / 2;

            var d = r * Math.Sin(Math.PI / 8);
            var e = (float)(c - d);
            var f = (float)(c + d);

            for (int i = 0; i <= EDITSize; i += _workGrid)
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

            for (int i = 0; i <= EDITSize; i += _workAxis)
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

            ds.DrawText("(keep it simple)", a, yN, Colors.LightPink);
        }
        #endregion


        #region DrawingStyles  ================================================

        public static List<T> GetEnumAsList<T>() { return Enum.GetValues(typeof(T)).Cast<T>().ToList(); }
        public List<CanvasDashStyle> DashStyleList { get { return GetEnumAsList<CanvasDashStyle>(); } }
        public List<CanvasCapStyle> CapStyleList { get { return GetEnumAsList<CanvasCapStyle>(); } }
        public List<CanvasLineJoin> LineJoinList { get { return GetEnumAsList<CanvasLineJoin>(); } }
        public List<Fill_Stroke> FillStrokeList { get { return GetEnumAsList<Fill_Stroke>(); } }
        public List<PolygonSides> PolygonSideList { get { return GetEnumAsList<PolygonSides>(); } }
        #endregion

        #region DrawHitLine  ==================================================
        private void DrawHitLine(CanvasDrawingSession ds)
        {
        }
        private void DrawHitLine(CanvasDrawingSession ds, Color c, float w)
        {
        }
        #endregion



        #region Fields  =======================================================
        private const int _maxSize = 32;
        private const float _zoomFactor = 12;
        private const int _offset = 16;
        private const int _indexDelta = 36;

        private Action EndAction;
        private Action DragAction;
        private Action BeginAction;

        private bool _pointerIsPressed;


        private static int _ds = GraphDefault.HitMargin;
        #endregion


        #region SelectorCanvas0_PointerEvents  ================================
        //                     BeginAction,   DragAction,   EndAction
        private int _selector0Index = -1;
        private void SelectorCanvas0_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _selector0Index = GetSelector0Index(e);
        }

        private void SelectorCanvas0_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var index = GetSelector0Index(e);
            if (index < 0 || index != _selector0Index)
            {
                foreach (var shape in SymbolShapes) { shape.IsSelected = false; }
            }
            else
            {
                var isSelected = false;
                foreach (var shape in SymbolShapes) { isSelected |= shape.IsSelected; }

                foreach (var shape in SymbolShapes) { shape.IsSelected = !isSelected; }
            }
            PickerShape = null;
            PickerCanvas.Invalidate();
            EditorCanvas.Invalidate();
        }
        private int GetSelector0Index(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(SelectorCanvas0).Position;
            int index = (int)(p.Y / SelectorCanvas0.Width);
            return (index < 0 || index > SymbolShapes.Count) ? -1 : index;
        }
        #endregion

        #region SelectorCanvas1_PointerEvents  ================================
        //                     BeginAction,   DragAction,   EndAction
        private int _selector1Index = -1;
        private void SelectorCanvas1_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _selector1Index = GetSelector1Index(e);
        }

        private void SelectorCanvas1_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
        }

        private void SelectorCanvas1_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var index = GetSelector1Index(e);
            if (index < 0 || index != _selector1Index)
                ClearIsSelected();
            else
            {
                var shape = SymbolShapes[index];
                var isSelected = SymbolShapes[index].IsSelected;

                if (!IsSelectOneOrMoreShapeMode)
                    ClearIsSelected();

                shape.IsSelected = !isSelected;

                GrtProperty(shape);
            }
            PickerShape = null;

            PickerCanvas.Invalidate();
            EditorCanvas.Invalidate();

            void ClearIsSelected()
            {
                foreach (var shape in SymbolShapes) { shape.IsSelected = false; }
            }
        }
        private int GetSelector1Index(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(SelectorCanvas1).Position;
            int index = (int)(p.Y / SelectorCanvas1.Width);
            return (index < 0 || index >= SymbolShapes.Count) ? -1 : index;
        }
        #endregion

        #region PickerCanvas_PointerEvents  ===================================
        //                     BeginAction,   DragAction,   EndAction
        private int _pickerIndex = -1;
        private void PickerCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PickerShape = null;

            _pickerIndex = GetPickerShapeIndex(e);

            PickerCanvas.Invalidate();
        }

        private void PickerCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            foreach (var shape in SymbolShapes) { shape.IsSelected = false; }

            var index = GetPickerShapeIndex(e);
            if (index < 0 || (index != _pickerIndex)) return;

            PickerShape = PickerShapes[index];

            BeginAction = TryAddNewShape;

            PickerCanvas.Invalidate();
            EditorCanvas.Invalidate();
        }
        private int GetPickerShapeIndex(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(PickerCanvas).Position;
            int index = (int)(p.Y / PickerCanvas.Width);
            return (index < 0 || index >= PickerShapes.Count) ? -1 : index;
        }
        #endregion


        #region EditorCanvas_PointerEvents  ===================================
        //                     BeginAction,   DragAction,   EndAction
        private void EditorCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _pointerIsPressed = true;
            Point1 = DrawPoint(e);

            BeginAction?.Invoke();
        }

        private void EditorCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Point2 = DrawPoint(e);

            if (_pointerIsPressed && DragAction != null)
            {
                DragAction();
            }
        }

        private void EditorCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _pointerIsPressed = false;

            EndAction?.Invoke();
        }


        private Vector2 DrawPoint(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(EditorCanvas).Position;
            return new Vector2((float)p.X - EDITMargin, (float)p.Y - EDITMargin);
        }
        private int Round(double v)
        {
            var n = (int)v;
            var m = n + 1;
            return ((m - v) < (v - n)) ? m : n;
        }
        #endregion

        #region EditorCanvas_Actions  =========================================
        private void TryAddNewShape()
        {
            if (PickerShape is null) return;

            var delta = (Point1 - new Vector2(EDITSize / 2)) * (Shape.FULLSIZE / EDITSize);

            var newShape = PickerShape.Clone(delta);
            newShape.IsSelected = true;
            foreach (var shape in SymbolShapes) { shape.IsSelected = false; }
            SymbolShapes.Add(newShape);

            DragAction = DragShape;

            PickerCanvas.Invalidate();

            SetProperty(ProertyId.All);
        }

        private void DragShape()
        {
            var delta = (Point2 - Point1) * (Shape.FULLSIZE / EDITSize);
            if (delta.LengthSquared() < 1) return;

            BeginAction = null;
            Point1 = Point2;

            var list = GetSelectedShapes();
            if (list is null) return;

            Shape.MoveCenter(list, delta);

            EditorCanvas.Invalidate();
        }
        #endregion


        #region PropertyChanged  ==============================================
        private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            _shapeColor = ColorPicker.Color;
            SetProperty(ProertyId.ShapeColor);
        }
        private void StrokeWidthSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _strokeWidth = StrokeWidthSlider.Value;
            SetProperty(ProertyId.StrokeWidth);
        }
        private void RadiusSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _centralSize = RadiusSlider.Value;
            SetProperty(ProertyId.CentralSize);
        }

        private void VerticalSizeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _verticalSize = VerticalSizeSlider.Value;
            SetProperty(ProertyId.VerticalSize);
        }

        private void HorizontalSizeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _horizontalSize = HorizontalSizeSlider.Value;
            SetProperty(ProertyId.HorizontalSize);
        }
        private void FillStroke_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetProperty(ProertyId.StartCap);
        }
        private void DashStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetProperty(ProertyId.DashStyle);
        }
        private void PolygonSides_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetProperty(ProertyId.PolygonSide);
        }
        private void LineJoin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetProperty(ProertyId.LineJoin);
        }
        private void StartCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetProperty(ProertyId.StartCap);
        }
        private void EndCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetProperty(ProertyId.EndCap);
        }
        private void DashCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetProperty(ProertyId.DashCap);
        }
        #endregion

        #region SetGetProperty  ===============================================
        [Flags]
        private enum ProertyId
        {
            All = 0xFF,
            EndCap = 0x01,
            DashCap = 0x02,
            StartCap = 0x04,
            LineJoin = 0x08,
            DashStyle = 0x10,
            FillStroke = 0x20,
            ShapeColor = 0x40,
            StrokeWidth = 0x80,
            PolygonSide = 0x100,
            CentralSize = 0x200,
            VerticalSize = 0x400,
            HorizontalSize = 0x800
        }
        void SetProperty(ProertyId pid)
        {
            var list = GetSelectedShapes();
            if (list is null) return;
            foreach (var shape in list)
            {
                if ((pid & ProertyId.EndCap) != 0) shape.EndCap = ShapeEndCap;
                if ((pid & ProertyId.DashCap) != 0) shape.DashCap = ShapeDashCap;
                if ((pid & ProertyId.StartCap) != 0) shape.StartCap = ShapeStartCap;
                if ((pid & ProertyId.LineJoin) != 0) shape.LineJoin = ShapeLineJoin;
                if ((pid & ProertyId.DashStyle) != 0) shape.DashStyle = ShapeDashStyle;
                if ((pid & ProertyId.FillStroke) != 0) shape.FillStroke = ShapeFillStroke;
                if ((pid & ProertyId.ShapeColor) != 0) shape.ColorCode = ShapeColor.ToString();
                if ((pid & ProertyId.StrokeWidth) != 0) shape.StrokeWidth = (float)ShapeStrokeWidth;
                if ((pid & ProertyId.PolygonSide) != 0) shape.PolygonSide = ShapePolygonSide;
            }
            EditorCanvas.Invalidate();
        }

        void GrtProperty(Shape shape)
        {
            ShapeStartCap = shape.StartCap;
            ShapeEndCap = shape.EndCap;
            ShapeLineJoin = shape.LineJoin;
            ShapeDashCap = shape.DashCap;
            ShapeDashStyle = shape.DashStyle;
            ShapeFillStroke = shape.FillStroke;
            ShapePolygonSide = shape.PolygonSide;
            ShapeColor = shape.Color;
            ShapeStrokeWidth = shape.StrokeWidth;
        }
        #endregion

        #region CentralSize  ===================================================
        void SetShapeCentralSize()
        {
            var list = GetSelectedShapes();
            if (list is null) return;
        }
        #endregion

        #region VerticalSize  =================================================
        float _initialVerticalSize;
        #endregion

        #region HorizontalSize  ===============================================
        float _beginningHorizontallSize;
        #endregion


        #region LeftButtonClick  ==============================================
        internal static List<Shape> _sharedShapes = new List<Shape>();
        private void OneManyButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToggleOneManyButton();
        }
        private void ToggleOneManyButton()
        {
            if (IsSelectOneOrMoreShapeMode)
            {
                OneManyButton.Content = "\uE8C5";
                ToolTipService.SetToolTip(OneManyButton, "Select one shape at a time");
                IsSelectOneOrMoreShapeMode = false;
            }
            else
            {
                OneManyButton.Content = "\uE8C4";
                ToolTipService.SetToolTip(OneManyButton, "Select one or more shapes at a time");
                IsSelectOneOrMoreShapeMode = true;
            }
        }
        private bool IsSelectOneOrMoreShapeMode = true;

        private void CutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _sharedShapes.Clear();
            var list = GetSelectedShapes();
            if (list is null) return;

            foreach (var shape in list)
            {
                shape.IsSelected = false;
                _sharedShapes.Add(shape);

                SymbolShapes.Remove(shape);
            }
            HasSharedShapes = _sharedShapes.Count > 0;

            EditorCanvas.Invalidate();
        }

        private void CopyButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _sharedShapes.Clear();
            var list = GetSelectedShapes();
            if (list is null) return;

            foreach (var shape in list)
            {
                shape.IsSelected = false;
                _sharedShapes.Add(shape.Clone());
            }
            HasSharedShapes = _sharedShapes.Count > 0;

            EditorCanvas.Invalidate();
        }

        private void PasteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach (var shape in _sharedShapes)
            {
                SymbolShapes.Add(shape.Clone());
            }
            EditorCanvas.Invalidate();
        }

        private void RecenterButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var list = GetSelectedShapes();
            if (list is null) return;

            Shape.SetCenter(list, Vector2.Zero);
                EditorCanvas.Invalidate();
        }
        private void RotateRightButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => RotateSelectedShapes(Math.PI / 8);

        private void RotateLeftButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => RotateSelectedShapes(Math.PI / -8);

        private void FlipHorizontalButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => ScaleSelectedShapes(new Vector2(-1, 1));

        private void FlipVerticalButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => ScaleSelectedShapes(new Vector2(1, -1));
        #endregion

        #region LeftButtonHelperMethods  ======================================
        private List<(float dx, float dy)> _getList = new List<(float dx, float dy)>();
        private List<(float dx, float dy)> _setList = new List<(float dx, float dy)>();
        private (float dx, float dy) GetCenter(List<Shape> list)
        {
            return (0, 0);
        }
        private void RotateSelectedShapes(double radians)
        {
            var list = GetSelectedShapes();
            if (list is null) return;

        }
        private void ScaleSelectedShapes(Vector2 scale)
        {
        }
        #endregion

        #region GetSelectedShapes  ============================================
        private Shape[] GetSelectedShapes()
        {
            _selectedShapes.Clear();
            foreach (var shape in SymbolShapes)
            {
                if (shape.IsSelected) _selectedShapes.Add(shape);
            }
            return _selectedShapes.Count > 0 ? _selectedShapes.ToArray() : null;
        }
        private List<Shape> _selectedShapes = new List<Shape>();
        #endregion

        #region PropertyChangeHelper  =========================================
        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region INotifyPropertyChanged  =======================================
        public bool HasSharedShapes { get { return _hasSharedShapes; } set { Set(ref _hasSharedShapes, value); } }
        private bool _hasSharedShapes;

        public CanvasCapStyle ShapeStartCap { get { return _startCap; } set { Set(ref _startCap, value); } }
        private CanvasCapStyle _startCap;

        public CanvasCapStyle ShapeEndCap { get { return _endCap; } set { Set(ref _endCap, value); } }
        private CanvasCapStyle _endCap;

        public CanvasDashStyle ShapeDashStyle { get { return _dashStyle; } set { Set(ref _dashStyle, value); } }
        private CanvasDashStyle _dashStyle;

        public CanvasCapStyle ShapeDashCap { get { return _dashCap; } set { Set(ref _dashCap, value); } }
        private CanvasCapStyle _dashCap;

        public CanvasLineJoin ShapeLineJoin { get { return _lineJoin; } set { Set(ref _lineJoin, value); } }
        private CanvasLineJoin _lineJoin;

        public Fill_Stroke ShapeFillStroke { get { return _fillStroke; } set { Set(ref _fillStroke, value); } }
        public Fill_Stroke _fillStroke;

        public PolygonSides ShapePolygonSide { get { return _polygonSide; } set { Set(ref _polygonSide, value); } }
        private PolygonSides _polygonSide;

        public Color ShapeColor { get { return _shapeColor; } set { Set(ref _shapeColor, value); } }
        private Color _shapeColor;

        public double ShapeStrokeWidth { get { return _strokeWidth; } set { Set(ref _strokeWidth, value); } }
        public double _strokeWidth;

        public double ShapeCentralSize { get { return _centralSize; } set { Set(ref _centralSize, value); } }
        public double _centralSize;

        public double ShapeCentralSizeMin { get { return _centralSizeMin; } set { Set(ref _centralSizeMin, value); } }
        public double _centralSizeMin;

        public double ShapeCentralSizeMax { get { return _centralSizeMax; } set { Set(ref _centralSizeMax, value); } }
        public double _centralSizeMax;
        public double ShapeVerticalSize { get { return _verticalSize; } set { Set(ref _verticalSize, value); } }
        public double _verticalSize;

        public double ShapeVerticalSizeMin { get { return _verticalSizeMin; } set { Set(ref _verticalSizeMin, value); } }
        public double _verticalSizeMin;

        public double ShapeVerticalSizeMax { get { return _verticalSizeMax; } set { Set(ref _verticalSizeMax, value); } }
        public double _verticalSizeMax;

        public double ShapeHorizontalSize { get { return _horizontalSize; } set { Set(ref _horizontalSize, value); } }
        public double _horizontalSize;

        public double ShapeHorizontalSizeMin { get { return _horizontalSizeMin; } set { Set(ref _horizontalSizeMin, value); } }
        public double _horizontalSizeMin;

        public double ShapeHorizontalSizeMax { get { return _horizontalSizeMax; } set { Set(ref _horizontalSizeMax, value); } }
        public double _horizontalSizeMax;

        public double EastContactSize { get { return _eastContactSize; } set { Set(ref _eastContactSize, value); } }
        public double _eastContactSize;

        public double WestContactSize { get { return _westContactSize; } set { Set(ref _westContactSize, value); } }
        public double _westContactSize;

        public double NorthContactSize { get { return _northContactSize; } set { Set(ref _northContactSize, value); } }
        public double _northContactSize;

        public double SouthContactSize { get { return _southContactSize; } set { Set(ref _southContactSize, value); } }
        public double _southContactSize;

        public double NorthEastContactSize { get { return _northastContactSize; } set { Set(ref _northastContactSize, value); } }
        public double _northastContactSize;

        public double NorthWestContactSize { get { return _northWestContactSize; } set { Set(ref _northWestContactSize, value); } }
        public double _northWestContactSize;

        public double SouthEastContactSize { get { return _southEastContactSize; } set { Set(ref _southEastContactSize, value); } }
        public double _southEastContactSize;

        public double SouthWestContactSize { get { return _southWestContactSize; } set { Set(ref _southWestContactSize, value); } }
        public double _southWestContactSize;
        #endregion
    }
}
