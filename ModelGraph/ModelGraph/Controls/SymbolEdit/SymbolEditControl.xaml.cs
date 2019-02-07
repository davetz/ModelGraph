using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
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
using Color = Windows.UI.Color;

namespace ModelGraph.Controls
{
    #region Enums  ============================================================
    public enum Fill_Stroke
    {
        Stroke = 0,
        Filled = 1
    }
    public enum Edit_Contact
    {
        Edit,
        Contacts
    }
    #endregion

    public sealed partial class SymbolEditControl : Page, IPageControl, IModelPageControl, INotifyPropertyChanged
    {
        private bool _isScratchPad;
        private RootModel _rootModel;
        private SymbolX _symbol;

        private List<Shape> SymbolShapes = new List<Shape>();
        private List<Shape> PickerShapes = new List<Shape> { new Circle(), new Ellipes(), new RoundedRectangle(), new Rectangle(), new PolySide(), new PolyStar(), new PolyGear(), new Line(), new PolySpike(), new PolyPulse(), new PolyWave(), new PolySpring() };
        private HashSet<Shape> SelectedShapes = new HashSet<Shape>();
        private static HashSet<Shape> CutCopyShapes = new HashSet<Shape>(); //cut/copy/clone shapes between two SymbolEditControls

        private Shape PickerShape; //current selected picker shape

        private float EditScale => EditSize / 2;
        private const float EditSize = 512;  //width, height of shape in the editor

        private const float EditMargin = 32; //size of empty space arround the shape editor 
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
            _isScratchPad = true;
            this.InitializeComponent();
        }

        public SymbolEditControl(RootModel model)
        {
            if (model is null || model.Item is null || !(model.Item is SymbolX))
            {
                _isScratchPad = true;
            }
            else
            {
                _rootModel = model;
                _symbol = model.Item as SymbolX;
                _symbol.GetTargetContacts(Target_Contacts);
            }

            this.InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            ToggleOneManyButton();
            UnlockPolyline();
            SetSizeSliders();
            if (_isScratchPad)
            {
                EditContactComboBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                Shape.Deserialize(_symbol.Data, SymbolShapes);
            }
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
            EditorCanvas.Invalidate();
        }

        public void Release()
        {
            if (EditorCanvas != null)
            {
                EditorCanvas.RemoveFromVisualTree();
                EditorCanvas = null;
            }
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
            if (PickerCanvas != null)
            {
                PickerCanvas.RemoveFromVisualTree();
                PickerCanvas = null;
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
            InitContactControls();
            _contactSizeChangeEnabled = true;
            EditorCanvas.Invalidate();
        }

        #endregion

        #region DrawingStyles  ================================================
        public static List<T> GetEnumAsList<T>() { return Enum.GetValues(typeof(T)).Cast<T>().ToList(); }
        public List<CanvasDashStyle> DashStyleList { get { return GetEnumAsList<CanvasDashStyle>(); } }
        public List<CanvasCapStyle> CapStyleList { get { return GetEnumAsList<CanvasCapStyle>(); } }
        public List<CanvasLineJoin> LineJoinList { get { return GetEnumAsList<CanvasLineJoin>(); } }
        public List<Fill_Stroke> FillStrokeList { get { return GetEnumAsList<Fill_Stroke>(); } }
        public List<Edit_Contact> EditContactList { get { return GetEnumAsList<Edit_Contact>(); } }
        public List<Contact> ContactList { get { return GetEnumAsList<Contact>(); } }
        #endregion


        #region PickerCanvas_Draw  ============================================
        private void PickerCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            var W = (float)sender.Width;
            var HW = W / 2;

            var strokeWidth = 3;

            var n = PickerShapes.Count;
            for (int i = 0; i < n; i++)
            {
                var a = i * W;
                var b = (i + 1) * W;
                var center = new Vector2(HW, a + HW);
                var shape = PickerShapes[i];

                if (shape == PickerShape) Shape.HighLight(ds, W, i);
                shape.Draw(sender, ds, HW, center, strokeWidth);
                if (i == 3 || i == 6)
                    ds.DrawLine(0, b, b, b, Colors.LightGray, 1);
            }
        }
        #endregion

        #region SymbolCanvas_Draw  ============================================
        private void SymbolCanvas_Draw(CanvasControl canvas, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            var W = (float)canvas.Width;
            var HW = W / 2;
            var n = SymbolShapes.Count;

            var center = new Vector2(HW, HW);
            for (int i = 0; i < n; i++)
            {
                var shape = SymbolShapes[i];
                var strokeWidth = shape.StrokeWidth;

                shape.Draw(canvas, ds, HW, center, strokeWidth);
            }
        }
        #endregion

        #region SelectorCanvas_Draw  ==========================================
        private void SelectorCanvas_Draw(CanvasControl canvas, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            var W = (float)canvas.Width;
            var HW = W / 2;
            var n = SymbolShapes.Count;

            var m = SelectorCanvas.MinHeight;
            var v = (n + 1) * W;
            SelectorCanvas.Height = (v > m) ? v : m;

            var center_0 = new Vector2(HW, HW);
            for (int i = 0; i < n; i++)
            {
                var a = i * W;
                var b = (i + 1) * W;
                var center = new Vector2(HW, a + HW);
                var shape = SymbolShapes[i];
                var strokeWidth = shape.StrokeWidth;
                if (SelectedShapes.Contains(shape)) Shape.HighLight(ds, W, i);

                shape.Draw(canvas, ds, HW, center, strokeWidth);
                ds.DrawLine(0, b, b, b, Colors.LightGray, 1);
            }
        }
        #endregion

        #region EditorCanvas_Draw  ============================================
        private void EditorCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            var scale = EditSize / 2;
            DrawEditorBackgroundGrid(ds);

            if (EditContact == Edit_Contact.Contacts)
            {
                foreach (var shape in SymbolShapes)
                {
                    var strokeWidth = shape.StrokeWidth * 5;
                    shape.Draw(EditorCanvas, ds, scale, Center, strokeWidth, Shape.Coloring.Light);
                }

                DrawTargetContacts(ds);            }
            else
            {
                if (SelectedShapes.Count > 0)
                {
                    foreach (var shape in SymbolShapes)
                    {
                        var coloring = SelectedShapes.Contains(shape) ? Shape.Coloring.Light : Shape.Coloring.Gray;
                        var strokeWidth = shape.StrokeWidth * 5;
                        shape.Draw(EditorCanvas, ds, scale, Center, strokeWidth, coloring);
                    }

                    _polylineTarget = SelectedShapes.First() as Polyline;
                    _targetPoints.Clear();
                    Shape.DrawTargets(SelectedShapes, _targetPoints, ds, scale, Center);
                }
                else
                {
                    foreach (var shape in SymbolShapes)
                    {
                        var strokeWidth = shape.StrokeWidth * 5;
                        shape.Draw(EditorCanvas, ds, scale, Center, strokeWidth, Shape.Coloring.Normal);
                    }
                }
                SymbolCanvas.Invalidate();
                SelectorCanvas.Invalidate();
            }
        }
        private Polyline _polylineTarget;
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

            var a = EditMargin;   //north or west axis line
            var b = a + EditSize; //south or east axis line
            var c = EDITCenter;   //center axis line
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
            ds.DrawLine(2, 2, 10, 14, color1, 3);
            ds.DrawLine(2, 2, 14, 10, color1, 3);
            ds.DrawLine(0, 0, b, b, color1);
            ds.DrawLine(0, 0, b, b, color1);
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
            var xe = b - _workAxis - 10;
            var xw = a + _workAxis - 10;
            var xec = b - 10;
            var xwc = a - 16;
            ds.DrawText("nec", xec, yN, color3);
            ds.DrawText("ne", xe, yN, color3);
            ds.DrawText("N", xC, yN, color1);
            ds.DrawText("nw", xw, yN, color3);
            ds.DrawText("nwc", xwc, yN, color3);

            ds.DrawText("sec", xec, yS, color3);
            ds.DrawText("se", xe, yS, color3);
            ds.DrawText("S", xC, yS, color1);
            ds.DrawText("sw", xw, yS, color3);
            ds.DrawText("swc", xwc, yS, color3);


            var xE = b + 3;
            var xW = 8;
            var yC = c - 14;
            var yn = a + _workAxis - 14;
            var ys = b - _workAxis - 14;

            ds.DrawText("en", xE, yn, color3);
            ds.DrawText("E", xE, yC, color1);
            ds.DrawText("es", xE, ys, color3);

            ds.DrawText("wn", xW - 4, yn, color3);
            ds.DrawText("W", xW, yC, color1);
            ds.DrawText("ws", xW - 4, ys, color3);
        }
        #endregion

        #region DrawTargetContacts  ===========================================
        private void DrawTargetContacts(CanvasDrawingSession ds)
        {
            var cp = Center;

            CheckContacts();
            var N = _contactTargets.Count;

            _targetPoints.Clear();
            for (int i = 0; i < N; i++)
            {
                var (cont, targ, point, size) = _contactTargets[i];

                var p = point * EditScale;
                var c = Center + p;
                _targetPoints.Add(c);
                ds.DrawCircle(c, 8, Colors.Red, 5);
                if (cont == Contact.Any && size > 0)
                {
                    var (p1, p2) = XYPair.GetScaledNormal(targ, point, size, Center, EditScale);
                    ds.DrawLine(p1, p2, Color.FromArgb(0x80, 0xFF, 0, 0), 20);
                }
            }
        }
        #endregion

        #region Target_Contacts  ==============================================
        private Dictionary<Target, (Contact contact, (sbyte dx, sbyte dy) point, byte size)> Target_Contacts = new Dictionary<Target, (Contact contact, (sbyte dx, sbyte dy) point, byte size)>(5);
        private List<(Contact cont, Target targ, Vector2 point, float size)> _contactTargets = new List<(Contact cont, Target targ, Vector2 point, float)>();

        #region InitContactControls  ==========================================
        private void InitContactControls()
        {
            foreach (var e in Target_Contacts)
            {
                switch (e.Key)
                {
                    case Target.N:
                        Contact_N = e.Value.contact;
                        SetContactHighlight(Contact_N, ContactComboBox_N, ContactSizeSlider_N, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.S:
                        Contact_S = e.Value.contact;
                        SetContactHighlight(Contact_S, ContactComboBox_S, ContactSizeSlider_S, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.E:
                        Contact_E = e.Value.contact;
                        SetContactHighlight(Contact_E, ContactComboBox_E, ContactSizeSlider_E, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.W:
                        Contact_W = e.Value.contact;
                        SetContactHighlight(Contact_W, ContactComboBox_W, ContactSizeSlider_W, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.NE:
                        Contact_NE = e.Value.contact;
                        SetContactHighlight(Contact_NE, ContactComboBox_NE, ContactSizeSlider_NE, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.NW:
                        Contact_NW = e.Value.contact;
                        SetContactHighlight(Contact_NW, ContactComboBox_NW, ContactSizeSlider_NW, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.SE:
                        Contact_SE = e.Value.contact;
                        SetContactHighlight(Contact_SE, ContactComboBox_SE, ContactSizeSlider_SE, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.SW:
                        Contact_SW = e.Value.contact;
                        SetContactHighlight(Contact_SW, ContactComboBox_SW, ContactSizeSlider_SW, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.EN:
                        Contact_EN = e.Value.contact;
                        SetContactHighlight(Contact_EN, ContactComboBox_EN, ContactSizeSlider_EN, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.ES:
                        Contact_ES = e.Value.contact;
                        SetContactHighlight(Contact_ES, ContactComboBox_ES, ContactSizeSlider_ES, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.WN:
                        Contact_WN = e.Value.contact;
                        SetContactHighlight(Contact_WN, ContactComboBox_WN, ContactSizeSlider_WN, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.WS:
                        Contact_WS = e.Value.contact;
                        SetContactHighlight(Contact_WS, ContactComboBox_WS, ContactSizeSlider_WS, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.NEC:
                        Contact_NEC = e.Value.contact;
                        SetContactHighlight(Contact_NEC, ContactComboBox_NEC, ContactSizeSlider_NEC, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.NWC:
                        Contact_NWC = e.Value.contact;
                        SetContactHighlight(Contact_NWC, ContactComboBox_NWC, ContactSizeSlider_NWC, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.SEC:
                        Contact_SEC = e.Value.contact;
                        SetContactHighlight(Contact_SEC, ContactComboBox_SEC, ContactSizeSlider_SEC, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.SWC:
                        Contact_SWC = e.Value.contact;
                        SetContactHighlight(Contact_SWC, ContactComboBox_SWC, ContactSizeSlider_SWC, Shape.ToFloat(e.Value.size));
                        break;
                }
            }
        }
        #endregion

        #region CheckContacts  ================================================
        private void CheckContacts()
        {
            _contactTargets.Clear();
            
            CheckContact(Contact_NWC, Target.NWC, new Vector2(-1, -1), 0);
            CheckContact(Contact_NW, Target.NW, new Vector2(-.5f, -1), 0);
            CheckContact(Contact_N, Target.N, new Vector2(0, -1), 0);
            CheckContact(Contact_NE, Target.NE, new Vector2(.5f, -1), 0);
            CheckContact(Contact_NEC, Target.NEC, new Vector2(1, -1), 0);

            CheckContact(Contact_EN, Target.EN, new Vector2(1, -.5f), 0);
            CheckContact(Contact_E, Target.E, new Vector2(1, 0), 0);
            CheckContact(Contact_ES, Target.ES, new Vector2(1, .5f), 0);

            CheckContact(Contact_WN, Target.WN, new Vector2(-1, -.5f), 0);
            CheckContact(Contact_W, Target.W, new Vector2(-1, 0), 0);
            CheckContact(Contact_WS, Target.WS, new Vector2(-1, .5f), 0);

            CheckContact(Contact_SWC, Target.SWC, new Vector2(-1, 1), 0);
            CheckContact(Contact_SW, Target.SW, new Vector2(-.5f, 1), 0);
            CheckContact(Contact_S, Target.S, new Vector2(0, 1), 0);
            CheckContact(Contact_SE, Target.SE, new Vector2(.5f, 1), 0);
            CheckContact(Contact_SEC, Target.SEC, new Vector2(1, 1), 0);

            void CheckContact(Contact cont, Target targ, Vector2 point, float size)
            {
                if (cont == Contact.None)
                {
                    Target_Contacts.Remove(targ);
                }
                else
                {
                    if (Target_Contacts.TryGetValue(targ, value: out (Contact c, (sbyte, sbyte) p, byte s) e))
                    {
                        Target_Contacts[targ] = (cont, e.p, e.s);

                        point = Shape.ToVector(e.p);
                        size = Shape.ToFloat(e.s);
                    }
                    else
                    {
                        Target_Contacts.Add(targ, (cont, Shape.ToSByte(point), Shape.ToByte(size)));
                    }
                    _contactTargets.Add((cont, targ, point, size));
                }
            }
        }
        #endregion
        #endregion


        #region SetGetProperty  ===============================================
        [Flags]
        private enum ProertyId
        {
            All = 0x0FF,
            EndCap = 0x01,
            DashCap = 0x02,
            StartCap = 0x04,
            LineJoin = 0x08,
            DashStyle = 0x10,
            FillStroke = 0x20,
            ShapeColor = 0x40,
            StrokeWidth = 0x80,
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
        }

        void GrtProperty(Shape shape)
        {
            _changesEnabled = false;

            ShapeColor = shape.Color;
            ShapeStartCap = shape.StartCap;
            ShapeEndCap = shape.EndCap;
            ShapeLineJoin = shape.LineJoin;
            ShapeDashCap = shape.DashCap;
            ShapeDashStyle = shape.DashStyle;
            ShapeFillStroke = shape.FillStroke;
            ShapeStrokeWidth = shape.StrokeWidth;

            _changesEnabled = true;
        }
        #endregion


        #region LeftButtonHelperMethods  ======================================
        private List<(float dx, float dy)> _getList = new List<(float dx, float dy)>();
        private List<(float dx, float dy)> _setList = new List<(float dx, float dy)>();
        private bool _use30degreeDelta;
        private void RotateLeft()
        {
            if (SelectedShapes.Count > 0)
            {
                Shape.RotateLeft(SelectedShapes, _use30degreeDelta);
                EditorCanvas.Invalidate();
            }
        }
        private void RotateRight()
        {
            if (SelectedShapes.Count > 0)
            {
                Shape.RotateRight(SelectedShapes, _use30degreeDelta);
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
        private CanvasCapStyle _startCap = CanvasCapStyle.Round;

        public CanvasCapStyle ShapeEndCap { get { return _endCap; } set { Set(ref _endCap, value); } }
        private CanvasCapStyle _endCap = CanvasCapStyle.Round;

        public CanvasDashStyle ShapeDashStyle { get { return _dashStyle; } set { Set(ref _dashStyle, value); } }
        private CanvasDashStyle _dashStyle;

        public CanvasCapStyle ShapeDashCap { get { return _dashCap; } set { Set(ref _dashCap, value); } }
        private CanvasCapStyle _dashCap = CanvasCapStyle.Round;

        public CanvasLineJoin ShapeLineJoin { get { return _lineJoin; } set { Set(ref _lineJoin, value); } }
        private CanvasLineJoin _lineJoin = CanvasLineJoin.Round;

        public Fill_Stroke ShapeFillStroke { get { return _fillStroke; } set { Set(ref _fillStroke, value); } }
        private Fill_Stroke _fillStroke = Fill_Stroke.Stroke;

        public Edit_Contact EditContact{ get { return _editContact; } set { Set(ref _editContact, value); } }
        private Edit_Contact _editContact = Edit_Contact.Edit;

        public Color ShapeColor { get { return _shapeColor; } set { Set(ref _shapeColor, value); } }
        private Color _shapeColor = Color.FromArgb(0xff, 0xcd, 0xdf, 0xff);

        public double ShapeStrokeWidth { get { return _strokeWidth; } set { Set(ref _strokeWidth, value); } }
        public double _strokeWidth = 1;


        public Contact Contact_N { get { return _contact_N; } set { Set(ref _contact_N, value); } }
        private Contact _contact_N = Contact.None;
        public Contact Contact_NE { get { return _contact_NE; } set { Set(ref _contact_NE, value); } }
        private Contact _contact_NE = Contact.None;
        public Contact Contact_NW { get { return _contact_NW; } set { Set(ref _contact_NW, value); } }
        private Contact _contact_NW = Contact.None;
        public Contact Contact_NEC { get { return _contact_NEC; } set { Set(ref _contact_NEC, value); } }
        private Contact _contact_NEC = Contact.None;
        public Contact Contact_NWC { get { return _contact_NWC; } set { Set(ref _contact_NWC, value); } }
        private Contact _contact_NWC = Contact.None;

        public Contact Contact_E { get { return _contact_E; } set { Set(ref _contact_E, value); } }
        private Contact _contact_E = Contact.None;
        public Contact Contact_EN { get { return _contact_EN; } set { Set(ref _contact_EN, value); } }
        private Contact _contact_EN = Contact.None;
        public Contact Contact_ES { get { return _contact_ES; } set { Set(ref _contact_ES, value); } }
        private Contact _contact_ES = Contact.None;

        public Contact Contact_W { get { return _contact_W; } set { Set(ref _contact_W, value); } }
        private Contact _contact_W = Contact.None;
        public Contact Contact_WN { get { return _contact_WN; } set { Set(ref _contact_WN, value); } }
        private Contact _contact_WN = Contact.None;
        public Contact Contact_WS { get { return _contact_WS; } set { Set(ref _contact_WS, value); } }
        private Contact _contact_WS = Contact.None;

        public Contact Contact_S { get { return _contact_S; } set { Set(ref _contact_S, value); } }
        private Contact _contact_S = Contact.None;
        public Contact Contact_SE { get { return _contact_SE; } set { Set(ref _contact_SE, value); } }
        private Contact _contact_SE = Contact.None;
        public Contact Contact_SW { get { return _contact_SW; } set { Set(ref _contact_SW, value); } }
        private Contact _contact_SW = Contact.None;
        public Contact Contact_SEC { get { return _contact_SEC; } set { Set(ref _contact_SEC, value); } }
        private Contact _contact_SEC = Contact.None;
        public Contact Contact_SWC { get { return _contact_SWC; } set { Set(ref _contact_SWC, value); } }
        private Contact _contact_SWC = Contact.None;

        public double ContactSize_N { get { return _contactSize_N; } set { Set(ref _contactSize_N, value); } }
        public double _contactSize_N;
        public double ContactSize_NE { get { return _contactSize_NE; } set { Set(ref _contactSize_NE, value); } }
        public double _contactSize_NE;
        public double ContactSize_NW { get { return _contactSize_NW; } set { Set(ref _contactSize_NW, value); } }
        public double _contactSize_NW;
        public double ContactSize_NEC { get { return _contactSize_NEC; } set { Set(ref _contactSize_NEC, value); } }
        public double _contactSize_NEC;
        public double ContactSize_NWC { get { return _contactSize_NWC; } set { Set(ref _contactSize_NWC, value); } }
        public double _contactSize_NWC;

        public double ContactSize_E { get { return _contactSize_E; } set { Set(ref _contactSize_E, value); } }
        public double _contactSize_E;
        public double ContactSize_EN { get { return _contactSize_EN; } set { Set(ref _contactSize_EN, value); } }
        public double _contactSize_EN;
        public double ContactSize_ES { get { return _contactSize_ES; } set { Set(ref _contactSize_ES, value); } }
        public double _contactSize_ES;

        public double ContactSize_W { get { return _contactSize_W; } set { Set(ref _contactSize_E, value); } }
        public double _contactSize_W;
        public double ContactSize_WN { get { return _contactSize_WN; } set { Set(ref _contactSize_WN, value); } }
        public double _contactSize_WN;
        public double ContactSize_WS { get { return _contactSize_WS; } set { Set(ref _contactSize_WS, value); } }
        public double _contactSize_WS;

        public double ContactSize_S { get { return _contactSize_S; } set { Set(ref _contactSize_S, value); } }
        public double _contactSize_S;
        public double ContactSize_SE { get { return _contactSize_SE; } set { Set(ref _contactSize_SE, value); } }
        public double _contactSize_SE;
        public double ContactSize_SW { get { return _contactSize_SW; } set { Set(ref _contactSize_SW, value); } }
        public double _contactSize_SW;
        public double ContactSize_SEC { get { return _contactSize_SEC; } set { Set(ref _contactSize_SEC, value); } }
        public double _contactSize_SEC;
        public double ContactSize_SWC { get { return _contactSize_SWC; } set { Set(ref _contactSize_SWC, value); } }
        public double _contactSize_SWC;
        #endregion
    }
}
