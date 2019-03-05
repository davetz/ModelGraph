using Microsoft.Graphics.Canvas.Geometry;
using ModelGraph.Services;
using ModelGraphSTD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.ViewManagement;
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
        Contacts,
        AutoFlip,
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
        private Size _desiredSize = new Size { Height = 752, Width = 1054 };

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
                InitAutoFlipCheckBoxes(_symbol.AutoFlip);
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
            ApplicationView.GetForCurrentView().TryResizeView(_desiredSize);
            InitContactControls();
            _contactSizeChangeEnabled = true;
            EditorCanvas.Invalidate();
        }

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

        public bool AutoFlipVert { get { return _autoFlipVert; } set { Set(ref _autoFlipVert, value); } }
        public bool _autoFlipVert;
        public bool AutoFlipHorz { get { return _autoFlipHorz; } set { Set(ref _autoFlipHorz, value); } }
        public bool _autoFlipHorz;
        public bool AutoFlipVertHorz { get { return _autoFlipVertHorz; } set { Set(ref _autoFlipVertHorz, value); } }
        public bool _autoFlipVertHorz;
        public bool AutoRotateLeft { get { return _autoRotateLeft; } set { Set(ref _autoRotateLeft, value); } }
        public bool _autoRotateLeft;
        public bool AutoRotateLeftFlip { get { return _autoRotateLeftFlip; } set { Set(ref _autoRotateLeftFlip, value); } }
        public bool _autoRotateLeftFlip;
        public bool AutoRotateRight { get { return _autoRotateRight; } set { Set(ref _autoRotateRight, value); } }
        public bool _autoRotateRight;
        public bool AutoRotateRightFlip { get { return _autoRotateRightFlip; } set { Set(ref _autoRotateRightFlip, value); } }
        public bool _autoRotateRightFlip;
        #endregion

    }
}
