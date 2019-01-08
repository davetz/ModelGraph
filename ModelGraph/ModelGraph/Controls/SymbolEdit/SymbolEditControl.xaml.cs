using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ModelGraph.Helpers;
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
        _8 = 8,
    }
    #endregion

    public sealed partial class SymbolEditControl : Page, IPageControl, IModelPageControl, INotifyPropertyChanged
    {
        private RootModel _rootModel;
        private List<Shape> SymbolShapes = new List<Shape>();
        private List<Shape> PickerShapes = new List<Shape> { new Circle(), new Ellipes(), new RoundedRectangle(), new Rectangle(), new PolySide(), new PolyStar(), new PolyGear() };
        private HashSet<Shape> SelectedShapes = new HashSet<Shape>();
        private static HashSet<Shape> CutCopyShapes = new HashSet<Shape>(); //cut/copy/clone shapes between two SymbolEditControls

        private Shape NewShape; //the just-cloned picker shape
        private Shape PickerShape; //current selected picker shape

        private float EditZoom => EditSize / ShapeSize;
        private const float ShapeSize = 256; //max width, height of shape
        private const float EditSize = 512;  //width, height of shape in the editor

        private const float EditMargin = 24; //size of empty space arround the shape editor 
        private const float EditLimit = (EditSize + EditMargin) / 2; // delta to center of the margin area
        private const float EDITCenter = EditMargin + EditSize / 2; //center of editor canvas
        private static Vector2 Center = new Vector2(EDITCenter);
        private static Vector2 Limit = new Vector2(EditLimit);

        private Vector2 ShapeDelta => ShapePoint2 - ShapePoint1;
        private Vector2 ShapePoint1; // pointer down, transformed to shape coordinates
        private Vector2 ShapePoint2; // pointer up or pointer moved
        private Vector2 RawPoint1; // edit canvas pointer down
        private Vector2 RawPoint2; // edit canvas pointer up or pointer moved

        #region Constructor  ==================================================
        public SymbolEditControl()
        {
            this.InitializeComponent();
            Initialize();
        }

        public SymbolEditControl(RootModel model)
        {
            _rootModel = model;
            this.InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            _fillStroke = Fill_Stroke.Stroke;
            _strokeWidth = 1;
            _shapeColor = Colors.White;
            ToggleOneManyButton();
        }
        #endregion

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
            if (SymbolCanvas != null)
            {
                SymbolCanvas.RemoveFromVisualTree();
                SymbolCanvas = null;
            }
            if (SelectorCanvas != null)
            {
                SelectorCanvas.RemoveFromVisualTree();
                SelectorCanvas = null;
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

        #region DrawingStyles  ================================================
        public static List<T> GetEnumAsList<T>() { return Enum.GetValues(typeof(T)).Cast<T>().ToList(); }
        public List<CanvasDashStyle> DashStyleList { get { return GetEnumAsList<CanvasDashStyle>(); } }
        public List<CanvasCapStyle> CapStyleList { get { return GetEnumAsList<CanvasCapStyle>(); } }
        public List<CanvasLineJoin> LineJoinList { get { return GetEnumAsList<CanvasLineJoin>(); } }
        public List<Fill_Stroke> FillStrokeList { get { return GetEnumAsList<Fill_Stroke>(); } }
        public List<PolygonSides> PolygonSideList { get { return GetEnumAsList<PolygonSides>(); } }
        #endregion


        #region PickerCanvas_Draw  ============================================
        private void PickerCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            var W = (float)sender.Width;
            var HW = W / 2;
            var scale = (W - 2) / ShapeSize;

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

        #region SymbolCanvas_Draw  ============================================
        private void SymbolCanvas_Draw(CanvasControl canvas, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            var W = (float)canvas.Width;
            var HW = W / 2;
            var scale = (W - 2) / ShapeSize;
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

        #region SelectorCanvas_Draw  ==========================================
        private void SelectorCanvas_Draw(CanvasControl canvas, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            var W = (float)canvas.Width;
            var HW = W / 2;
            var scale = (W - 2) / ShapeSize;
            var n = SymbolShapes.Count;

            var m = SelectorCanvas.MinHeight;
            var v = (n + 1) * W;
            SelectorCanvas.Height = (v > m) ? v : m;

            var center_0 = new Vector2(HW, HW);
            for (int i = 0; i < n; i++)
            {
                var center = new Vector2(HW, (i * W) + HW);
                var shape = SymbolShapes[i];
                var strokeWidth = shape.StrokeWidth;
                if (SelectedShapes.Contains(shape)) Shape.HighLight(ds, W, i);

                shape.Draw(canvas, ds, scale, center, strokeWidth);
            }
        }
        #endregion

        #region EditorCanvas_Draw  ============================================
        private void EditorCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            var scale = EditSize / ShapeSize;

            DrawEditorBackgroundGrid(ds);
            foreach (var shape in SymbolShapes)
            {
                var strokeWidth = shape.StrokeWidth * scale * 5;
                shape.Draw(EditorCanvas, ds, scale, Center, strokeWidth);
            }
            DrawSelectTargets();

            SymbolCanvas.Invalidate();
            SelectorCanvas.Invalidate();

            void DrawSelectTargets()
            {
                if (SelectedShapes.Count > 0)
                {
                    _polylineTarget = SelectedShapes.First();
                    _hasPolylineTarget = (SelectedShapes.Count == 1 && !(_polylineTarget is Central));
                    if (!_hasPolylineTarget) _polylineTarget = null;

                    var record = !_editorCanvasPressed; // don't refresh targets durring drag operations

                    var (cent, vert, horz) = Shape.DrawTargets(SelectedShapes, record, _hasPolylineTarget, _targetPoints, ds, scale, Center);

                    if (_initializeCentralSlider) { _initializeCentralSlider = false; ShapeCentralSize = cent; }
                    if (_initializeVerticalSlider) { _initializeVerticalSlider = false; ShapeVerticalSize = cent; }
                    if (_initializeHorizontalSlider) { _initializeHorizontalSlider = false; ShapeHorizontalSize = cent; }
                }
            }
        }
        private bool _initializeCentralSlider;
        private bool _initializeVerticalSlider;
        private bool _initializeHorizontalSlider;
        private Shape _polylineTarget;
        private bool _hasPolylineTarget;
        private List<Vector2> _targetPoints = new List<Vector2>();

        #endregion

        #region DrawEditorBackgroundGrid  =====================================
        private const int _workAxis = (int)(EditSize / 4);
        private const int _workGrid = (int)(EditSize / 16);
        private void DrawEditorBackgroundGrid(CanvasDrawingSession ds)
        {
            var color1 = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
            var color2 = Color.FromArgb(0xff, 0xff, 0xff, 0x80);
            var color3 = Color.FromArgb(0x80, 0xff, 0xff, 0x00);
            var color4 = Color.FromArgb(0x40, 0xff, 0xff, 0xff);

            var a = EditMargin;
            var b = a + EditSize;
            var c = EDITCenter;
            var r = EditSize / 2;

            var d = r * Math.Sin(Math.PI / 8);
            var e = (float)(c - d);
            var f = (float)(c + d);

            for (int i = 0; i <= EditSize; i += _workGrid)
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

            for (int i = 0; i <= EditSize; i += _workAxis)
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


        #region PickerCanvas_PointerEvents  ===================================
        private int _pickerIndex = -1;
        private void PickerCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PickerShape = null;

            _pickerIndex = GetPickerShapeIndex(e);

            PickerCanvas.Invalidate();
        }

        private void PickerCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var index = GetPickerShapeIndex(e);
            if (index < 0 || (index != _pickerIndex)) return;

            SetPicker(PickerShapes[index]);

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

        #region SymbolCanvas_PointerEvents  ===================================
        private bool _symbolCanvasPressed;
        private void SymbolCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _symbolCanvasPressed = true;
        }

        private void SymbolCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_symbolCanvasPressed)
            {
                _symbolCanvasPressed = false;

                if (SelectedShapes.Count > 0)
                {
                    SelectedShapes.Clear();
                    SetIdle();
                }
                else
                {
                    foreach (var shape in SymbolShapes) { SelectedShapes.Add(shape); }
                    EnableHitTest();
                }


                PickerShape = null;
                PickerCanvas.Invalidate();
            }
        }
        #endregion

        #region SelectorCanvas_PointerEvents  =================================
        private int _selectorIndex = -1;
        private bool _selectorCanvasPressed;
        private bool _ignoreSelectorCanvaseReleased;
        private void SelectorCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _selectorCanvasPressed = true;
            _selectorIndex = GetSelectorIndex(e);
        }

        private void SelectorCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_selectorCanvasPressed && _selectorIndex >= 0 && SelectedShapes.Count == 0)
            {
                var index  = GetSelectorIndex(e);
                if (index >= 0 && index != _selectorIndex)
                {
                    var shape = SymbolShapes[_selectorIndex];
                    SymbolShapes[_selectorIndex] = SymbolShapes[index];
                    SymbolShapes[index] = shape;
                    _selectorIndex = index;
                    _ignoreSelectorCanvaseReleased = true;
                    EditorCanvas.Invalidate();
                }
            }
        }

        private void SelectorCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_selectorCanvasPressed && !_ignoreSelectorCanvaseReleased)
            {
                var index = GetSelectorIndex(e);
                if (index < 0 || _selectorIndex < 0)
                    SelectedShapes.Clear();
                else if (index == _selectorIndex)
                {
                    var shape = SymbolShapes[index];

                    if (IsSelectOneOrMoreShapeMode)
                    {
                        if (SelectedShapes.Contains(shape))
                            SelectedShapes.Remove(shape);
                        else
                            SelectedShapes.Add(shape);
                    }
                    else
                    {
                        SelectedShapes.Clear();
                        SelectedShapes.Add(shape);
                    }

                    GrtProperty(shape);
                }
                PickerShape = null;

                if (SelectedShapes.Count > 0)
                    EnableHitTest();
                else
                    SetIdle();

                PickerCanvas.Invalidate();
            }
            _selectorCanvasPressed = false;
            _ignoreSelectorCanvaseReleased = false;
        }
        private int GetSelectorIndex(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(SelectorCanvas).Position;
            int index = (int)(p.Y / SelectorCanvas.Width);
            return (index < 0 || index >= SymbolShapes.Count) ? -1 : index;
        }
        #endregion

        #region EditorCanvas_PointerEvents  ===================================
        private bool _editorCanvasPressed;

        private void EditorCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _editorCanvasPressed = true;
            RawPoint1 = GetRawPoint(e);
            ShapePoint1 = ShapePoint(RawPoint1);

            BeginAction?.Invoke();
        }

        private void EditorCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            RawPoint2 = GetRawPoint(e);
            ShapePoint2 = ShapePoint(RawPoint2);

            if (_editorCanvasPressed && ShapeDelta.LengthSquared() > 1) DragAction?.Invoke();
        }

        private void EditorCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _editorCanvasPressed = false;
            RawPoint2 = GetRawPoint(e);
            ShapePoint2 = ShapePoint(RawPoint2);

            EndAction?.Invoke();
        }
        private Vector2 ShapePoint(Vector2 rawPoint) => (rawPoint - Center) / EditZoom;

        private Vector2 GetRawPoint(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(EditorCanvas).Position;
            return new Vector2((float)p.X, (float)p.Y);
        }
        #endregion


        #region EditorAction  =================================================
        private Action EndAction;
        private Action DragAction;
        private Action BeginAction;

        private void SetIdle()
        {
            BeginAction = DragAction = EndAction = null;
            NewShape = null;
            PickerShape = null;
            SelectedShapes.Clear();

            EditorCanvas.Invalidate();
        }

        private void SetPicker(Shape pickerShape)
        {
            PickerShape = pickerShape;
            BeginAction = AddNewShape;
            DragAction = EndAction = null;
        }
        private void AddNewShape()
        {
            if (PickerShape is null) return;

            NewShape = PickerShape.Clone(ShapePoint1);

            SymbolShapes.Add(NewShape);
            GrtProperty(NewShape);
            DragAction = BeginDragNewShape;

            EditorCanvas.Invalidate();
        }
        private void BeginDragNewShape()
        {
            if (NewShape is null) return;

            SelectedShapes.Add(NewShape);
            NewShape = null;
            PickerShape = null;

            PickerCanvas.Invalidate();

            DragAction = DragingShapes;
            DragingShapes();
        }
        private void DragingShapes()
        {
            if (SelectedShapes.Count > 0)
            {
                Shape.MoveCenter(SelectedShapes, ShapeDelta);
                RawPoint1 = RawPoint2;
                ShapePoint1 = ShapePoint(RawPoint2);

                EditorCanvas.Invalidate();
            }
        }

        private void EnableHitTest()
        {
            BeginAction = CheckHitTest;
            DragAction = EndAction = null;

            _initializeCentralSlider = _initializeVerticalSlider = _initializeHorizontalSlider = true;
            _ignoreCentralSliderChange = _ignoreVerticalSliderChange = _ignoreHorizontalSliderChange = true;
            _ignoreColorChange = true;

            EditorCanvas.Invalidate();
        }

        private void CheckHitTest()
        {
            var index = HitTest(RawPoint1);
            if (index < 0) SetIdle();

            if (index == 0)
                DragAction = DragingShapes;
        }

        private int HitTest(Vector2 rawPoint)
        {
            var N = _targetPoints.Count;
            for (int i = 0; i < N; i++)
            {
                var dp = rawPoint - _targetPoints[i];
                if (dp.LengthSquared() < 50) return i;
            }
            return -1;
        }
        #endregion


        #region PropertyChanged  ==============================================
        private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (_ignoreColorChange)
                _ignoreColorChange = false;
            else
            {
                _shapeColor = ColorPicker.Color;
                SetProperty(ProertyId.ShapeColor);
            }
        }
        private bool _ignoreColorChange;
        private void StrokeWidthSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _strokeWidth = StrokeWidthSlider.Value;
            SetProperty(ProertyId.StrokeWidth);
        }
        private void FillStroke_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetProperty(ProertyId.FillStroke);
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
        private void CentralSizeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (_ignoreCentralSliderChange)
                _ignoreCentralSliderChange = false;
            else
            {
                _ignoreVerticalSliderChange = _ignoreHorizontalSliderChange = true;
                _initializeVerticalSlider = _initializeHorizontalSlider = true;

                Shape.ResizeCentral(SelectedShapes, (float)CentralSizeSlider.Value);
                EditorCanvas.Invalidate();
            }
        }
        private void VerticalSizeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (_ignoreVerticalSliderChange)
                _ignoreVerticalSliderChange = false;
            else
            {
                _ignoreCentralSliderChange =_initializeCentralSlider = true;

                Shape.ResizeRadius2(SelectedShapes, (float)VerticalSizeSlider.Value);
                EditorCanvas.Invalidate();
            }
        }
        private void HorizontalSizeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (_ignoreHorizontalSliderChange)
                _ignoreHorizontalSliderChange = false;
            else
            {
                _ignoreCentralSliderChange = _initializeCentralSlider = true;

                Shape.ResizeRadius1(SelectedShapes, (float)HorizontalSizeSlider.Value);
                EditorCanvas.Invalidate();
            }
        }
        private bool _ignoreCentralSliderChange;
        private bool _ignoreVerticalSliderChange;
        private bool _ignoreHorizontalSliderChange;
        #endregion

        #region SetGetProperty  ===============================================
        [Flags]
        private enum ProertyId
        {
            All = 0xFFF,
            EndCap = 0x01,
            DashCap = 0x02,
            StartCap = 0x04,
            LineJoin = 0x08,
            DashStyle = 0x10,
            FillStroke = 0x20,
            ShapeColor = 0x40,
            StrokeWidth = 0x80,
            PolygonSide = 0x100,
        }
        void SetProperty(ProertyId pid)
        {
            foreach (var shape in SelectedShapes) { SetProperty(shape, pid); }

            EditorCanvas.Invalidate();
        }
        void SetProperty(Shape shape, ProertyId pid)
        {
            if ((pid & ProertyId.EndCap) != 0) shape.EndCap = ShapeEndCap;
            if ((pid & ProertyId.DashCap) != 0) shape.DashCap = ShapeDashCap;
            if ((pid & ProertyId.StartCap) != 0) shape.StartCap = ShapeStartCap;
            if ((pid & ProertyId.LineJoin) != 0) shape.LineJoin = ShapeLineJoin;
            if ((pid & ProertyId.DashStyle) != 0) shape.DashStyle = ShapeDashStyle;
            if ((pid & ProertyId.FillStroke) != 0) shape.FillStroke = ShapeFillStroke;
            if ((pid & ProertyId.ShapeColor) != 0) shape.ColorCode = ShapeColor.ToString();
            if ((pid & ProertyId.StrokeWidth) != 0) shape.StrokeWidth = (float)ShapeStrokeWidth;
            if ((pid & ProertyId.PolygonSide) != 0) shape.Dimension = ShapeDimension;
        }

        void GrtProperty(Shape shape)
        {
            ShapeStartCap = shape.StartCap;
            ShapeEndCap = shape.EndCap;
            ShapeLineJoin = shape.LineJoin;
            ShapeDashCap = shape.DashCap;
            ShapeDashStyle = shape.DashStyle;
            ShapeFillStroke = shape.FillStroke;
            ShapeDimension = shape.Dimension;
            ShapeColor = shape.Color;
            ShapeStrokeWidth = shape.StrokeWidth;
        }
        #endregion


        #region LeftButtonClick  ==============================================
        private void OneManyButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToggleOneManyButton();
        }
        private void ToggleOneManyButton()
        {
            if (IsSelectOneOrMoreShapeMode)
            {
                OneManyButton.Content = "\uE8C5";
                ToolTipService.SetToolTip(OneManyButton, "_001A".GetLocalized() );
                IsSelectOneOrMoreShapeMode = false;
            }
            else
            {
                OneManyButton.Content = "\uE8C4";
                ToolTipService.SetToolTip(OneManyButton, "_001B".GetLocalized());
                IsSelectOneOrMoreShapeMode = true;
            }
        }
        private bool IsSelectOneOrMoreShapeMode = true;

        private void CutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (SelectedShapes.Count > 0)
            {
                CutCopyShapes.Clear();
                foreach (var shape in SelectedShapes)
                {
                    CutCopyShapes.Add(shape);
                    SymbolShapes.Remove(shape);
                }
                SelectedShapes.Clear();

                EditorCanvas.Invalidate();
            }
        }

        private void CopyButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (SelectedShapes.Count > 0)
            {
                CutCopyShapes.Clear();
                foreach (var shape in SelectedShapes)
                {
                    CutCopyShapes.Add(shape.Clone());
                }
                SelectedShapes.Clear();

                EditorCanvas.Invalidate();
            }
        }

        private void PasteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (CutCopyShapes.Count > 0)
            {
                SelectedShapes.Clear();
                foreach (var template in CutCopyShapes)
                {
                    var shape = template.Clone();
                    SymbolShapes.Add(shape);
                    SelectedShapes.Add(shape);
                }
                EnableHitTest();
                EditorCanvas.Invalidate();
            }
        }

        private void RecenterButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (SelectedShapes.Count > 0)
            {
                Shape.SetCenter(SelectedShapes, Vector2.Zero);
                EditorCanvas.Invalidate();
            }
        }
        private void RotateLeftButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => RotateLeft();
        private void RotateRightButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => RotateRight();
        private void FlipVerticalButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => FlipVertical();
        private void FlipHorizontalButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => FlipHorizontal();
        #endregion

        #region LeftButtonHelperMethods  ======================================
        private List<(float dx, float dy)> _getList = new List<(float dx, float dy)>();
        private List<(float dx, float dy)> _setList = new List<(float dx, float dy)>();
        private void RotateLeft()
        {
            if (SelectedShapes.Count > 0)
            {
                Shape.RotateLeft(SelectedShapes);
                EditorCanvas.Invalidate();
            }
        }
        private void RotateRight()
        {
            if (SelectedShapes.Count > 0)
            {
                Shape.RotateRight(SelectedShapes);
                EditorCanvas.Invalidate();
            }
        }
        private void FlipVertical()
        {
            if (SelectedShapes.Count > 0)
            {
                Shape.VerticalFlip(SelectedShapes);
                EditorCanvas.Invalidate();
            }
        }
        private void FlipHorizontal()
        {
            if (SelectedShapes.Count > 0)
            {
                Shape.HorizontalFlip(SelectedShapes);
                EditorCanvas.Invalidate();
            }
        }
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

        public ShapeDimension ShapeDimension { get { return _shapeDimension; } set { Set(ref _shapeDimension, value); } }
        private ShapeDimension _shapeDimension;

        public Color ShapeColor { get { return _shapeColor; } set { Set(ref _shapeColor, value); } }
        private Color _shapeColor;

        public double ShapeStrokeWidth { get { return _strokeWidth; } set { Set(ref _strokeWidth, value); } }
        public double _strokeWidth;

        public double ShapeCentralSize { get { return _centralSize; } set { Set(ref _centralSize, value); } }
        public double _centralSize;

        public double ShapeVerticalSize { get { return _verticalSize; } set { Set(ref _verticalSize, value); } }
        public double _verticalSize;

        public double ShapeHorizontalSize { get { return _horizontalSize; } set { Set(ref _horizontalSize, value); } }
        public double _horizontalSize;

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
