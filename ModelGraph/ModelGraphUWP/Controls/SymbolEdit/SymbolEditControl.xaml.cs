using System;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using Microsoft.Graphics.Canvas.Geometry;
using System.Linq;
using ModelGraphSTD;

namespace ModelGraphUWP
{
    public sealed partial class SymbolEditControl : UserControl, IModelControl
    {
        private Chef _chef;
        private RootModel _rootModel;
        private SymbolX _symbol;

        #region Constructor  ==================================================
        public SymbolEditControl(RootModel root)
        {
            InitializeComponent();

            _rootModel = root;
            _rootModel.ModelControl = this;
            _chef = root.Chef;
            _symbol = root.Item1 as SymbolX;
            UnpackSymbolData(_symbol.Data);
            UpdataLineStyleUI();
            UpdateSymbolSize();

            Initialize();
        }
        #endregion

        #region LineStyle  ====================================================
        private class LineStyle
        {
            internal LineStyle()
            {
                A = 255;
                R = 255;
                G = 255;
                B = 255;
                W = 1;
                SC = 2;
                EC = 2;
                DC = 2;
                DS = 0;
            }
            internal LineStyle(LineStyle s)
            {
                Set(s);
            }
            internal void Set(LineStyle s)
            {
                A = s.A;
                R = s.R;
                G = s.G;
                B = s.B;
                W = s.W;
                SC = s.SC;
                EC = s.EC;
                DC = s.DC;
                DS = s.DS;
            }
            internal void Set(LineStyle p, LineStyle s)
            {
                if (p.A == A) A = s.A;
                if (p.R == R) R = s.R;
                if (p.G == G) G = s.G;
                if (p.B == B) B = s.B;
                if (p.W == W) W = s.W;
                if (p.SC == SC) SC = s.SC;
                if (p.EC == EC) EC = s.EC;
                if (p.DC == DC) DC = s.DC;
                if (p.DS == DS) DS = s.DS;
            }
            const int max = 24;

            internal byte A;
            internal byte R;
            internal byte B;
            internal byte G;
            internal byte W;
            internal byte SC;
            internal byte EC;
            internal byte DC;
            internal byte DS;
            private byte Invert(byte val) { return (byte)((val > 127) ? 0 : 255); }
            internal Windows.UI.Color Color { get { return Windows.UI.Color.FromArgb(A, R, G, B); } }
            internal Windows.UI.Color InvertedColor { get { return Windows.UI.Color.FromArgb(255, Invert(R), Invert(G), Invert(B)); } }
            internal int Width { get { return W; } set { W = (byte)((value < 1) ? 1 : ((value > max) ? max : value)); } }
            internal void Validate( byte maxCap, byte maxDash)
            {
                if (SC > maxCap) SC = 0;
                if (EC > maxCap) EC = 0;
                if (DC > maxCap) DC = 0;
                if (DS > maxDash) DS = maxDash;
            }

            internal bool Equals(LineStyle s)
            {
                if (A != s.A) return false;
                if (R != s.R) return false;
                if (G != s.G) return false;
                if (B != s.B) return false;
                if (W != s.W) return false;
                if (SC != s.SC) return false;
                if (EC != s.EC) return false;
                if (DC != s.DC) return false;
                if (DS != s.DS) return false;
                return true;
            }

            internal string HexColor { get { return GetHex(); } set { SetHex(value); } }
            private string GetHex()
            {
                var a = string.Format("{0,2:X2}", A);
                var r = string.Format("{0,2:X2}", R);
                var b = string.Format("{0,2:X2}", B);
                var g = string.Format("{0,2:X2}", G);
                return $"#{a}{r}{g}{b}";
            }
            static string valid = "0123456789ABCDEF";
            private void SetHex(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return;

                var ca = s.ToUpper().ToCharArray();
                var n = ca.Length;
                var vals = new byte[8];
                int j = 0;
                for (int i = 0; (i < n && j < 8); i++)
                {
                    var c = ca[i];
                    var v = valid.IndexOf(c);
                    if (v < 0) continue;
                    vals[j++] = (byte)v;
                }
                if (j == 0) return;

                // don't be picky, just do something
                A = (byte)((vals[0] << 4) + vals[1]);
                R = (byte)((vals[2] << 4) + vals[3]);
                G = (byte)((vals[4] << 4) + vals[5]);
                B = (byte)((vals[6] << 4) + vals[7]);
            }
        }
        #endregion

        #region LineData  =====================================================
        private class LineData
        {
            internal LineData(LineStyle s)
            {
                Style.Set(s);
            }
            internal LineData(LineData l)
            {
                foreach (var point in l.Points)
                {
                    Points.Add(point);
                }
                Style.Set(l.Style);
            }
            internal LineData(Extent e, LineStyle s)
            {
                Points.Add(e.Point1);
                Points.Add(e.Point2);
                Style.Set(s);
            }

            internal bool IsSelected;
            internal int HitPointIndex;
            internal int HitSegmentIndex;

            internal int Width { get { return Style.Width; } }
            internal Color Color { get { return Style.Color; } }
            internal List<XYPoint> Points = new List<XYPoint>();
            internal readonly LineStyle Style = new LineStyle();

            private int last { get { return Points.Count - 1; } }
            internal XYPoint LastPoint { get { return Points[last]; } }
            internal XYPoint FirstPoint { get { return Points[0]; } }

            internal bool IsSamePoint(XYPoint p, XYPoint q) { return (p.X == q.X && p.Y == q.Y); }
            internal bool IsClosedPolygon { get { return IsSamePoint(FirstPoint, LastPoint); } }

            internal bool TryExtendLine(Extent e, LineStyle s)
            {
                if (!Style.Equals(s)) return false;

                var p = FirstPoint;
                var q = LastPoint;
                if (IsSamePoint(p, q)) return false;
                if (MidPointMatch(e.Point1)) return false;
                if (MidPointMatch(e.Point2)) return false;

                if (IsSamePoint(p, e.Point1))
                {
                    Points.Insert(0, e.Point2);
                    return true;
                }
                else if (IsSamePoint(q, e.Point1))
                {
                    Points.Add(e.Point2);
                    return true;
                }
                if (IsSamePoint(p, e.Point2))
                {
                    Points.Insert(0, e.Point1);
                    return true;
                }
                else if (IsSamePoint(q, e.Point2))
                {
                    Points.Add(e.Point1);
                    return true;
                }
                return false;
            }

            private bool MidPointMatch(XYPoint p)
            {
                var n = last;
                for (int i = 1; i < n; i++)
                {
                    if (IsSamePoint(p, Points[i])) return true;
                }
                return false;
            }
        }
        #endregion

        #region Fields  =======================================================
        private SolidColorBrush activeColor;
        private SolidColorBrush inactiveColor;

        private Color _divColor = Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF);
        private Color _white = Colors.White;
        private Color _black = Colors.Black;
        
        private const int _maxSize = 24;
        private const float _zoomFactor = 12;
        private const int _offset = 6;
        private const int _indexDelta = 36;

        private Action EndAction;
        private Action DragAction;
        private Action BeginAction;

        private XYPoint _rawRef;
        private Extent _drawRef = new Extent();
        private bool _pointerIsPressed;

        private readonly LineStyle _prevStyle = new LineStyle();
        private readonly LineStyle _lineStyle = new LineStyle();
        private readonly List<LineData> _pasteLines = new List<LineData>();
        private readonly List<LineData> _workingLines = new List<LineData>();
        private readonly List<LineData> _selectLines = new List<LineData>();
        private bool _isAllSelected;

        Windows.ApplicationModel.Resources.ResourceLoader _resourceLoader;

        private static int _ds = GraphParm.HitMargin;
        #endregion

        #region Initialize  ===================================================
        private void Initialize()
        {
            activeColor = Resources["ActiveColor"] as SolidColorBrush;
            inactiveColor = Resources["InactiveColor"] as SolidColorBrush;

            UpdataLineStyleUI();

            hitLineStyle.StartCap = CanvasCapStyle.Round;
            hitLineStyle.EndCap = CanvasCapStyle.Round;
            hitLineStyle.DashCap = CanvasCapStyle.Round;
            hitLineStyle.DashStyle = CanvasDashStyle.Solid;

            _resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
        }
        #endregion

        #region SymbolData  ===================================================
        #region UnpackSymbolData  =============================================
        private const int _CP = _maxSize / 2; // center point of symbol

        private void UnpackSymbolData(byte[] data)
        {
            _workingLines.Clear();

            if (data == null) return;

            int len = data.Length;
            int max = len - 13; // the last valid line data record must begin before this value
            for (int i = 2; i < max;)
            {
                _lineStyle.A = data[i++];  // 0
                _lineStyle.R = data[i++];  // 1
                _lineStyle.G = data[i++];  // 2
                _lineStyle.B = data[i++];  // 3
                _lineStyle.W = data[i++];  // 4
                _lineStyle.SC = data[i++]; // 5
                _lineStyle.EC = data[i++]; // 6
                _lineStyle.DC = data[i++]; // 7
                _lineStyle.DS = data[i++]; // 8
                _lineStyle.Validate((byte)(CapStyles.Count - 1), (byte)(DashStyles.Count - 1));

                var pc = data[i++]; // 9

                if (pc < 2) return;             // abort, point count too small
                if ((i + (2 * pc)) > len) return; // abort, point count too large

                var line = new LineData(_lineStyle);
                _workingLines.Add(line);

                for (int j = 0; j < pc; j++)
                {
                    var dx = (sbyte)data[i++];
                    var dy = (sbyte)data[i++];
                    line.Points.Add(new XYPoint(_CP + dx, _CP + dy));
                }
            }
        }
        #endregion

        #region PackageSymbolData  ============================================
        private const int _SZ = 10; // line data record header size

        private byte[] PackageSymbolData()
        {
            var e = GetExtent();
            var cx = e.CenterX;
            var cy = e.CenterY;

            int len = 0;
            foreach (var line in _workingLines)
            {
                int cnt;
                if (!TryGetCount(line, out cnt)) continue;
                len += (_SZ + (2 * cnt));
            }
            len += 2;
            var data = new byte[len];

            data[0] = (byte)e.Width;
            data[1] = (byte)e.Hieght;

            for (int i = 2; i < len;)
            {
                foreach (var line in _workingLines)
                {
                    int cnt = 0;
                    if (!TryGetCount(line, out cnt)) continue;

                    var style = line.Style;
                    data[i++] = style.A;  // 0
                    data[i++] = style.R;  // 1
                    data[i++] = style.G;  // 2
                    data[i++] = style.B;  // 3
                    data[i++] = style.W;  // 4
                    data[i++] = style.SC; // 5
                    data[i++] = style.EC; // 6
                    data[i++] = style.DC; // 7
                    data[i++] = style.DS; // 8
                    data[i++] = (byte)cnt;// 9

                    foreach (var p in line.Points)
                    {
                        var dx = (sbyte)(p.X - cx);
                        var dy = (sbyte)(p.Y - cy);

                        data[i++] = (byte)dx;
                        data[i++] = (byte)dy;
                    }
                }
            }
            return data;
        }
        private bool TryGetCount(LineData line, out int count)
        {
            count = line.Points.Count;
            if (count > 255) count = 255;
            if (count < 2) return false;
            return true;
        }
        #endregion
        #endregion

        #region CanvasDraw  ===================================================
        #region SelectCanvas_Draw  ===============================================
        private void SelectCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            var n = _workingLines.Count;
            SelectCanvas.Height = (n + 1) * _indexDelta;
            for (int i = 0; i < n; i++)
            {
                DrawActualLine(ds, _workingLines[i], i);
            }
            DrawIsSelected(ds);
        }
        #endregion

        #region DrawCanvas_Draw  ==============================================
        private void DrawCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            var n = _workingLines.Count;
            for (int i = 0; i < n; i++)
            {
                DrawEditorLine(ds, _workingLines[i]);
            }
            DrawGrid(ds);
            DrawSegment(ds);
            DrawHitLine(ds);
            UpdateSymbolSize();
            SelectCanvas.Invalidate();
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
            DrawHitLine(ds, _white, 5);
            DrawHitLine(ds, _black, 3);
        }
        private void DrawHitLine(CanvasDrawingSession ds, Color c, float w)
        {
            foreach (var line in _selectLines)
            {
                var points = line.Points;
                var n = points.Count;
                if (n < 2) return;

                var p1 = points[0];

                var o = _offset;
                float z = _zoomFactor;

                float x1, y1, x2, y2;
                for (int i = 1; i < n; i++)
                {
                    var p2 = points[i];

                    x1 = o + p1.X * z;
                    y1 = o + p1.Y * z;
                    x2 = o + p2.X * z;
                    y2 = o + p2.Y * z;

                    p1 = p2;

                    ds.DrawLine(x1, y1, x2, y2, c, w, hitLineStyle);
                }
                for (int i = 0; i < n; i++)
                {
                    var p = points[i];

                    x1 = (o + p.X * z);
                    y1 = (o + p.Y * z);
                    x2 = x1;
                    y2 = y1;

                    ds.DrawLine(x1, y1, x2, y2, c, 2 * w, hitLineStyle);
                }
            }
        }
        #endregion

        #region DrawEditorLine  ===============================================
        private void DrawEditorLine(CanvasDrawingSession ds, LineData line)
        {
            strokeStyle.EndCap = CapStyles[line.Style.EC];
            strokeStyle.DashCap = CapStyles[line.Style.DC];
            strokeStyle.StartCap = CapStyles[line.Style.SC];
            strokeStyle.DashStyle = DashStyles[line.Style.DS];

            var points = line.Points;
            var n = points.Count;
            if (n < 2) return;

            var p1 = points[0];

            var c = line.Color;
            var w = line.Width;

            var o = _offset;
            float z = _zoomFactor;
            w = (int)(w * z);


            float x1, y1, x2, y2;
            for (int i = 1; i < n; i++)
            {
                var p2 = points[i];

                x1 = o + p1.X * z;
                y1 = o + p1.Y * z;
                x2 = o + p2.X * z;
                y2 = o + p2.Y * z;

                p1 = p2;

                ds.DrawLine(x1, y1, x2, y2, c, w, strokeStyle);
            }
        }
        #endregion

        #region DrawActualLine  ===============================================
        private void DrawActualLine(CanvasDrawingSession ds, LineData line, int index)
        {
            strokeStyle.EndCap = CapStyles[line.Style.EC];
            strokeStyle.DashCap = CapStyles[line.Style.DC];
            strokeStyle.StartCap = CapStyles[line.Style.SC];
            strokeStyle.DashStyle = DashStyles[line.Style.DS];

            var points = line.Points;
            var n = points.Count;
            if (n < 2) return;

            var c = line.Color;
            var w = line.Width;

            var o = _offset;
            var h = _indexDelta * (index + 1);

            var p1 = points[0];
            float x1, y1, x2, y2;
            for (int i = 1; i < n; i++)
            {
                var p2 = points[i];

                x1 = o + p1.X;
                y1 = o + p1.Y;
                x2 = o + p2.X;
                y2 = o + p2.Y;

                p1 = p2;

                ds.DrawLine(x1, y1, x2, y2, c, w, strokeStyle);
            }

            x1 = 2;
            x2 = _indexDelta - 3;
            ds.DrawLine(x1, h, x2, h, _divColor);

            p1 = points[0];
            for (int i = 1; i < n; i++)
            {
                var p2 = points[i];

                x1 = o + p1.X;
                y1 = o + h + p1.Y;
                x2 = o + p2.X;
                y2 = o + h + p2.Y;

                p1 = p2;

                ds.DrawLine(x1, y1, x2, y2, c, w, strokeStyle);
            }

            x1 = 2;
            x2 = _indexDelta - 3;
            h += _indexDelta;
            ds.DrawLine(x1, h, x2, h, _divColor);
        }
        #endregion

        #region DrawIsSelected  ===============================================
        private void DrawIsSelected(CanvasDrawingSession ds)
        {
            int m = 2;
            var w = _indexDelta - (2 * m);
            var o = _indexDelta / 2;
            var x1 = m;
            var x2 = x1 + w;

            if (_isAllSelected)
            {
                ds.DrawLine(x1, o, x2, o, _divColor, w);
            }

            for (int i = 0; i < _workingLines.Count; i++)
            {
                if (_workingLines[i].IsSelected)
                {
                    var y = ((i + 1) * _indexDelta) + o;
                    ds.DrawLine(x1, y, x2, y, _divColor, w);
                }
            }
        }
        #endregion

        #region DrawSegment  ==================================================
        private void DrawSegment(CanvasDrawingSession ds)
        {
            if (DragAction == CreateLineDrag)
            {
                var c = _lineStyle.Color;
                var w = _lineStyle.Width * _zoomFactor;

                strokeStyle.EndCap = CapStyles[_lineStyle.EC];
                strokeStyle.DashCap = CapStyles[_lineStyle.DC];
                strokeStyle.StartCap = CapStyles[_lineStyle.SC];
                strokeStyle.DashStyle = DashStyles[_lineStyle.DS];


                float x1 = _offset + _drawRef.Point1.X * _zoomFactor;
                float y1 = _offset + _drawRef.Point1.Y * _zoomFactor;
                float x2 = _offset + _drawRef.Point2.X * _zoomFactor;
                float y2 = _offset + _drawRef.Point2.Y * _zoomFactor;

                ds.DrawLine(x1, y1, x2, y2, c, w, strokeStyle);
            }
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

        #region Button_Click  =================================================
        private void PenButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _isAllSelected = false;
            foreach (var line in _workingLines) { line.IsSelected = false; }
            CutButton.IsEnabled = false;
            CopyButton.IsEnabled = false;
            PasteButton.IsEnabled = false;
            VFlipButton.IsEnabled = false;
            HFlipButton.IsEnabled = false;
            RotateButton.IsEnabled = false;

            PenButton.Background = activeColor;
            _drawRef = new Extent();

            SetCreateLineAction();
            DrawCanvas.Invalidate();
        }
        private void CutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _pasteLines.Clear();
            foreach (var line in _selectLines)
            {
                _pasteLines.Add(new LineData(line));
                _workingLines.Remove(line);
            }
            TryGetSelected();
            DrawCanvas.Invalidate();
        }

        private void CopyButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach (var line in _selectLines)
            {
                var newLine = new LineData(line.Style);
                newLine.IsSelected = true;
                line.IsSelected = false;

                foreach (var p in line.Points)
                {
                    newLine.Points.Add(new XYPoint(p.X + 1, p.Y + 1));
                }
                var i = _workingLines.IndexOf(line);
                _workingLines.Insert(i + 1, newLine);
            }
            TryGetSelected();
            DrawCanvas.Invalidate();
        }

        private void PasteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_pasteLines.Count == 0) return;

            foreach (var line in _pasteLines)
            {
                _workingLines.Add(new LineData(line));
            }
            DrawCanvas.Invalidate();
        }

        private XYPoint GetCenter()
        {
            return new XYPoint(12, 12);
        }
        private void VFlipButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var center = GetCenter();
            foreach (var line in _selectLines)
            {
                for (int i = 0; i < line.Points.Count; i++)
                {
                    line.Points[i] = line.Points[i].VerticalFlip(center.Y);
                }
            }
            DrawCanvas.Invalidate();
        }

        private void HFlipButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var center = GetCenter();
            foreach (var line in _selectLines)
            {
                for (int i = 0; i < line.Points.Count; i++)
                {
                    line.Points[i] = line.Points[i].HorizontalFlip(center.X);
                }
            }
            DrawCanvas.Invalidate();
        }

        private void RotateButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var center = GetCenter();
            foreach (var line in _selectLines)
            {
                for (int i = 0; i < line.Points.Count; i++)
                {
                    line.Points[i] = line.Points[i].Rotate(center.X, center.Y);
                }
            }
            DrawCanvas.Invalidate();
        }

        internal void Save()
        {
            _symbol.Data = PackageSymbolData();
            DrawCanvas.Invalidate();
        }

        internal void Reload()
        {
            UnpackSymbolData(_symbol.Data);
            UpdateSymbolSize();
            UpdataLineStyleUI();
            _pasteLines.Clear();
            _selectLines.Clear();
            DrawCanvas.Invalidate();
        }
        #endregion

        #region EventActions  =================================================
        private void SetCreateLineAction()
        {
            BeginAction = BeginLine;
            DragAction = null;
            EndAction = EndLine;
        }
        private void BeginLine()
        {
            DragAction = CreateLineDrag;
        }
        private void EndLine()
        {
            CreateNewLineSegment();
            DragAction = null;
        }
        private void CreateLineDrag()
        {
            DrawCanvas.Invalidate();
        }
        private void SetHitTestAction()
        {
            BeginAction = HitTest;
            DragAction = null;
            EndAction = null;
        }
        private void SetMoveSelectedPointAction()
        {
            BeginAction = HitTest;
            DragAction = MoveSelectedPoints;
            EndAction = () => { DragAction = null; };
        }
        private void SetMoveSelectedLineAction()
        {
            BeginAction = HitTest;
            DragAction = MoveSelectedLines;
            EndAction = () => { DragAction = null; };
        }
        private void SetAddPointAction()
        {
            BeginAction = null;
            DragAction = null;
            EndAction = null;
        }

        private void MoveSelectedPoints()
        { 
            XYPoint delta;
            if (_drawRef.TryGetDelta(out delta) && _selectLines.Count > 0)
            {
                foreach (var line in _selectLines)
                {
                    var i = line.HitPointIndex;
                    if (i < 0) continue;
                    line.Points[i] = line.Points[i].Move(delta);
                }
                Consolidate();
            }
        }
        private void MoveSelectedLines()
        {
            XYPoint delta;
            if (_drawRef.TryGetDelta(out delta) && _selectLines.Count > 0)
            {
                foreach (var line in _selectLines)
                {
                    var n = line.Points.Count;
                    for (int i = 0; i < n; i++)
                    {
                        line.Points[i] = line.Points[i].Move(delta);
                    }
                }
                Consolidate();
            }
        }
        private void CreateNewLineSegment()
        {
            bool found = false;
            foreach (var line in _workingLines)
            {
                if (line.TryExtendLine(_drawRef, _lineStyle))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                _workingLines.Add(new LineData(_drawRef, _lineStyle));
            }
            Consolidate();
            DrawCanvas.Invalidate();
        }

        private XYPoint _hitPoint;
        private void HitTest()
        {
            var p = _hitPoint = _drawRef.Point1;

            var gotHit = false;
            foreach (var line in _workingLines)
            {
                if (!line.IsSelected) continue;
                line.HitPointIndex = -1;
                var points = line.Points;
                var n = points.Count;
                for (int i = 0; i < n; i++)
                {
                    if (p.Equals(points[i]))
                    {
                        line.HitPointIndex = i;
                        gotHit = true;
                    }
                }
            }
            if (gotHit)
            {
                SetMoveSelectedPointAction();
                return;
            }

            var E = new Extent(p, _ds);
            var e = new Extent();
            foreach (var line in _workingLines)
            {
                if (!line.IsSelected) continue;
                var points = line.Points;
                var n = points.Count;
                e.SetPoint1(points[0], _zoomFactor);
                for (int i = 1; i < n; i++)
                {
                    e.Record(points[i], _zoomFactor);
                    if (e.HitTest(ref p, ref E))
                    {
                        line.HitSegmentIndex = i;
                        gotHit = true;
                    }
                }
            }
            if (gotHit)
                SetAddPointAction();
            SetMoveSelectedLineAction();
        }
        #endregion

        #region Consolidate  ==================================================
        private void Consolidate()
        {
            var n = _workingLines.Count;
            bool anyChange = true;
            while (anyChange)
            {
                anyChange = false;
                for (int i = 0; i < n; i++)
                {
                    var Li = _workingLines[i];
                    var Pi = Li.Points;
                    var Ni = Pi.Count - 1;
                    if (Ni < 0) continue;
                    if (Ni > 1 && Pi[0].Equals(Pi[Ni])) continue;

                    for (int j = i + 1; j < n; j++)
                    {
                        var Lj = _workingLines[j];
                        var Pj = Lj.Points;
                        var Nj = Pj.Count - 1;
                        if (Nj < 0) continue;
                        if (!Li.Style.Equals(Lj.Style)) continue;
                        if (Nj > 1 && Pj[0].Equals(Pj[Nj])) continue;
                        if (HasSameInteriorPoint(Pi, Pj)) continue;
                        if (Pi[0].Equals(Pj[0]) && !Pi[Ni].Equals(Pj[Nj]))
                        {
                            for (int k = 1; k < Nj; k++)
                            {
                                Pi.Insert(0, Pj[k]);
                            }
                            Pj.Clear();
                            anyChange = true;
                            break;
                        }
                        else if (!Pi[0].Equals(Pj[0]) && Pi[Ni].Equals(Pj[Nj]))
                        {
                            for (int k = (Nj - 1); k >= 0; k--)
                            {
                                Pi.Add(Pj[k]);
                            }
                            Pj.Clear();
                            anyChange = true;
                            break;
                        }
                        else if (Pi[0].Equals(Pj[Nj]) && !Pi[Ni].Equals(Pj[0]))
                        {
                            for (int k = (Nj - 1); k >= 0; k--)
                            {
                                Pi.Insert(0, Pj[k]);
                            }
                            Pj.Clear();
                            anyChange = true;
                            break;
                        }
                        else if (!Pi[0].Equals(Pj[Nj]) && Pi[Ni].Equals(Pj[0]))
                        {
                            for (int k = 1; k <= Nj; k++)
                            {
                                Pi.Add(Pj[k]);
                            }
                            Pj.Clear();
                            anyChange = true;
                            break;
                        }
                    }
                }
            }
            for (int i = (n - 1); i >=0 ; i--)
            {
                if (_workingLines[i].Points.Count > 0) continue;
                _workingLines.RemoveAt(i);
            }
            TryGetSelected();
            UpdateSymbolSize();
            DrawCanvas.Invalidate();
        }
        private bool HasSameInteriorPoint(List<XYPoint> p1, List<XYPoint> p2)
        {
            int i1 = 1;
            int i2 = p1.Count - 1;
            int j1 = 1;
            int j2 = p2.Count - 1;
            for (int i = i1; i < i2; i++)
            {
                for (int j = j1; j < j2; j++)
                {
                    if (p1[i].Equals(p2[j])) return true;
                }
            }
            return false;
        }
        #endregion

        #region UpdateSymbolSize  =============================================
        private Extent GetExtent()
        {
            var x = new Extent(0, 0);
            var firstPass = true;
            foreach (var line in _workingLines)
            {
                var style = line.Style;
                var w = style.Width / 2;
                var d1 = (style.SC == 0) ? 0 : w; // allow for a startCap
                var d2 = (style.EC == 0) ? 0 : w; // allow for a endCap

                var points = line.Points;
                var n = points.Count;
                if (n < 2) continue;

                var p1 =points[0];
                if (firstPass) { firstPass = false; x = new Extent(p1, p1); }

                for (int i = 1; i < n; i++)
                {
                    var p2 = points[i];
                    if (w > 0)
                    {
                        if (p2.X == p1.X) // is vertical
                        {
                            if (p1.Y < p2.Y)
                            {
                                x.Expand(p1.X - w, p1.Y);
                                x.Expand(p2.X + w, p2.Y);
                                x.Expand(p1.X, p1.Y - d1);
                                x.Expand(p2.X, p2.Y + d2);
                            }
                            else
                            {
                                x.Expand(p2.X - w, p2.Y);
                                x.Expand(p1.X + w, p1.Y);
                                x.Expand(p2.X, p2.Y - d2);
                                x.Expand(p1.X, p1.Y + d1);
                            }
                        }
                        else if (p2.Y == p1.Y) // is horizontal
                        {
                            if (p1.X < p2.X)
                            {
                                x.Expand(p1.X, p1.Y - w);
                                x.Expand(p2.X, p2.Y + w);
                                x.Expand(p1.X - d1, p1.Y);
                                x.Expand(p2.X + d2, p2.Y);
                            }
                            else
                            {
                                x.Expand(p1.X, p1.Y - w);
                                x.Expand(p2.X, p2.Y + w);
                                x.Expand(p1.X + d1, p1.Y);
                                x.Expand(p2.X - d2, p2.Y);
                            }
                        }
                        else
                        {
                            // similar triangles all over the place
                            int dx, dy, dx1, dy1, dx2, dy2;
                            double DX, DY, HP, r;
                            DX = (double)((p2.X > p1.X) ? (p2.X - p1.X) : (p1.X - p2.X));
                            DY = (double)((p2.Y > p1.Y) ? (p2.Y - p1.Y) : (p1.Y - p2.Y));
                            HP = Math.Sqrt((DX * DX) + (DY * DY));
                            r = w / HP;
                            dx = Round(DY * r);
                            dy = Round(DX * r);
                            r = ((d1 + HP) / HP) - 1;
                            dy1 = Round(DY * r);
                            dx1 = Round(DX * r);
                            r = ((d2 + HP) / HP) - 1;
                            dy2 = Round(DY * r);
                            dx2 = Round(DX * r);

                            x.Expand(p1.X + dx, p1.Y - dy);
                            x.Expand(p2.X + dx, p2.Y - dy);

                            x.Expand(p1.X - dx, p1.Y + dy);
                            x.Expand(p2.X - dx, p2.Y + dy);

                            if (p1.X < p2.X && p1.Y < p2.Y)
                            {
                                x.Expand(p1.X - dx1, p1.Y - dy1);
                                x.Expand(p2.X + dx2, p2.Y + dy2);
                            }
                            else if (p1.X < p2.X && p1.Y > p2.Y)
                            {
                                x.Expand(p1.X + dx1, p1.Y - dy1);
                                x.Expand(p2.X - dx2, p2.Y + dy2);
                            }
                            else if (p1.X > p2.X && p1.Y > p2.Y)
                            {
                                x.Expand(p2.X - dx2, p2.Y - dy2);
                                x.Expand(p1.X + dx1, p1.Y + dy1);
                            }
                            else
                            {
                                x.Expand(p2.X + dx2, p2.Y - dy2);
                                x.Expand(p1.X - dx1, p1.Y + dy1);
                            }
                        }
                    }
                    else
                    {
                        x.Expand(p1);
                        x.Expand(p2);
                    }
                    p1 = p2;
                }
            }

            // force even width and height values
            if (x.Width == 0)
            {
                x.X1 = x.X1 - 1;
                x.X2 = x.X2 + 1;
            }
            else if (x.Width % 2 == 1)
            {
                if (x.X1 % 2 == 1 && x.X2 % 2 == 0)
                    x.X2 = (x.X2 + 1);
                else
                    x.X1 = (x.X1 + 1);
            }
            if (x.Hieght == 0)
            {
                x.Y1 = x.Y1 - 1;
                x.Y2 = x.Y2 + 1;
            }
            else if (x.Hieght % 2 == 1)
            {
                if (x.Y1 % 2 == 1 && x.Y2 % 2 == 0)
                    x.Y2 = (x.Y2 + 1);
                else
                    x.Y1 = (x.Y1 + 1);
            }
            return x;
        }
        private void UpdateSymbolSize()
        {
            var x = GetExtent();
            SymbolSize.Text = $"({x.Width.ToString()} x {x.Hieght.ToString()})";
        }
        #endregion

        #region GetSelected  ==================================================
        // maintain the order in which the lines are selected, 
        // the first selected line will be at the start of the list
        // the most recently selected line will be at the end of the list
        private bool TryGetSelected()
        {
            // remove lines that are no longer selected
            var last = _selectLines.Count - 1;
            for (int i = last; i >= 0; i--)
            {
                var line = _selectLines[i];
                if (_workingLines.Contains(line) && line.IsSelected) continue;
                _selectLines.RemoveAt(i);
            }
            // add new lines that are selected
            foreach (var line in _workingLines)
            {
                if (_selectLines.Contains(line)) continue;

                if (_isAllSelected || line.IsSelected) _selectLines.Add(line);
            }
            return _selectLines.Count > 0;
        }
        #endregion

        #region ViewEvents  ===================================================
        public (int Width, int Height) PreferredMinSize => (504, 360);


        public void SetSize(double width, double hieght)
        {
        }

        public void Refresh()
        {
        }

        public void Close()
        {
            if (DrawCanvas != null)
            {
                DrawCanvas.RemoveFromVisualTree();
                DrawCanvas = null;
            }
            if (SelectCanvas != null)
            {
                SelectCanvas.RemoveFromVisualTree();
                SelectCanvas = null;
            }
        }

        private void UserControl_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (DrawCanvas != null)
            {
                DrawCanvas.RemoveFromVisualTree();
                DrawCanvas = null;
            }
            if (SelectCanvas != null)
            {
                SelectCanvas.RemoveFromVisualTree();
                SelectCanvas = null;
            }
        }

        private void DrawCanvas_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            InitializeColorCanvas();

            DrawCanvas.Invalidate();
        }

        #endregion

        #region LineStyleUI  ==================================================
        private static byte[] _cW = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
        private static byte[] _cR = new byte[] { 0xFF, 0xFF, 0x00, 0x00 };
        private static byte[] _cY = new byte[] { 0xFF, 0xFF, 0xFF, 0x00 };
        private static byte[] _cG = new byte[] { 0xFF, 0x00, 0xFF, 0x00 };
        private static byte[] _cC = new byte[] { 0xFF, 0x00, 0xFF, 0xFF };
        private static byte[] _cB = new byte[] { 0xFF, 0x00, 0x00, 0xFF };
        private static byte[] _cM = new byte[] { 0xFF, 0xFF, 0x00, 0xFF };
        private static byte[] _cWd = new byte[] { 0x00, 0x17, 0x17, 0x17 };
        private static byte[] _cRd = new byte[] { 0x00, 0x17, 0x00, 0x00 };
        private static byte[] _cYd = new byte[] { 0x00, 0x17, 0x17, 0x00 };
        private static byte[] _cGd = new byte[] { 0x00, 0x00, 0x17, 0x00 };
        private static byte[] _cCd = new byte[] { 0x00, 0x00, 0x17, 0x17 };
        private static byte[] _cBd = new byte[] { 0x00, 0x00, 0x00, 0x17 };
        private static byte[] _cMd = new byte[] { 0x00, 0x17, 0x00, 0x17 };
        private static byte[][] _cBase = new byte[][] { _cW, _cR, _cY, _cG, _cC, _cB, _cM };
        private static byte[][] _cDelta = new byte[][] { _cWd, _cRd, _cYd, _cGd, _cCd, _cBd, _cMd };

        private void InitializeColorCanvas()
        {
            var w = ColorCanvas.Width;
            var nx = 7;
            var dx = w / nx;

            var h = ColorCanvas.Height;
            var ny = 9;
            var dy = h / ny;

            var gBT = new Windows.UI.Xaml.Thickness(1, 1, 1, 1);
            var gBB = new SolidColorBrush(Color.FromArgb(0x80, 0, 0, 0));

            var argb = new byte[] { 0xFF, 0, 0, 0 };
            AddGrid(0, 0, dx, dy, argb, gBT, gBB);
            var bg = Colors.DarkSlateGray;
            argb[0] = bg.A;
            argb[1] = bg.R;
            argb[2] = bg.G;
            argb[3] = bg.B;
            AddGrid(dx, 0, dx, dy, argb, gBT, gBB);


            double x = 0;
            for (int i = 0; i < nx; i++, x += dx)
            {
                double y = dy;
                argb = _cBase[i];
                var delta = _cDelta[i];
                for (int j = 1; j < ny; j++, y += dy)
                {
                    AddGrid(x, y, dx, dy, argb, gBT, gBB);
                    argb = new byte[] { argb[0], (byte)(argb[1] - delta[1]), (byte)(argb[2] - delta[2]), (byte)(argb[3] - delta[3]) };
                }
            }
        }
        private void AddGrid(double x, double y, double dx, double dy, byte[] argb, Windows.UI.Xaml.Thickness gBT, SolidColorBrush gBB)
        {
            var g = new Grid();
            g.BorderThickness = gBT;
            g.BorderBrush = gBB;
            g.Width = dx;
            g.Height = dy;
            g.Tag = argb;
            g.Background = new SolidColorBrush(Color.FromArgb(argb[0], argb[1], argb[2], argb[3]));
            g.PointerPressed += ColorGrid_PointerPressed;
            ColorCanvas.Children.Add(g);
            Canvas.SetTop(g, y);
            Canvas.SetLeft(g, x);
        }

        private void ColorGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var temp = new LineStyle(_lineStyle);
            var g = sender as Grid;
            var argb = g.Tag as byte[];
            _lineStyle.A = argb[0];
            _lineStyle.R = argb[1];
            _lineStyle.G = argb[2];
            _lineStyle.B = argb[3];
            UpdataLineStyleUI();
            _prevStyle.Set(temp);
            UpdateSelectedLineStyles();
        }
        private void ColorHexValue_LostFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var temp = new LineStyle(_lineStyle);
            var obj = sender as TextBox;
            _lineStyle.HexColor = obj.Text;
            UpdataLineStyleUI();
            _prevStyle.Set(temp);
            UpdateSelectedLineStyles();
        }

        private void WidthSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _prevStyle.Set(_lineStyle);
            var s = sender as Slider;
            var v = s.Value;
            _lineStyle.Width = (byte)v;
            UpdateSelectedLineStyles();
        }

        private void SliderColorA_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _prevStyle.Set(_lineStyle);
            var s = sender as Slider;
            var v = s.Value;
            _lineStyle.A = (byte)v;
            UpdateSelectedLineStyles();
        }

        private void SliderColorR_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _prevStyle.Set(_lineStyle);
            var s = sender as Slider;
            var v = s.Value;
            _lineStyle.R = (byte)v;
            UpdateSelectedLineStyles();
        }

        private void SliderColorG_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _prevStyle.Set(_lineStyle);
            var s = sender as Slider;
            var v = s.Value;
            _lineStyle.G = (byte)v;
            UpdateSelectedLineStyles();
        }

        private void SliderColorB_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _prevStyle.Set(_lineStyle);
            var s = sender as Slider;
            var v = s.Value;
            _lineStyle.B = (byte)v;
            UpdateSelectedLineStyles();
        }

        private void EndCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _prevStyle.Set(_lineStyle);
            _lineStyle.EC = (byte)CapStyles.IndexOf(EndCap);
            UpdateSelectedLineStyles();
        }
        private void DashCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _prevStyle.Set(_lineStyle);
            _lineStyle.DC = (byte)CapStyles.IndexOf(DashCap);
            UpdateSelectedLineStyles();
        }
        private void StartCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _prevStyle.Set(_lineStyle);
            _lineStyle.SC = (byte)CapStyles.IndexOf(StartCap);
            UpdateSelectedLineStyles();
        }
        private void DashStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _prevStyle.Set(_lineStyle);
            _lineStyle.DS = (byte)DashStyles.IndexOf(DashStyle);
            UpdateSelectedLineStyles();
        }

        private bool _isValidUI = false;
        private void UpdataLineStyleUI()
        {
            if (_isValidUI == false)
            {
                if (EndCapType == null) return;
                if (StartCapType == null) return;
                if (WidthSlider == null) return;
                if (SliderColorA == null) return;
                if (SliderColorR == null) return;
                if (SliderColorG == null) return;
                if (SliderColorB == null) return;
                if (ColorHexValue == null) return;
                if (ColorSample == null) return;
                if (DrawCanvas == null) return;
                if (SelectCanvas == null) return;
                if (ColorCanvas == null) return;
                _isValidUI = true;
            }

            strokeStyle.EndCap = EndCap = CapStyles[_lineStyle.EC];
            strokeStyle.DashCap = DashCap = CapStyles[_lineStyle.DC];
            strokeStyle.StartCap = StartCap = CapStyles[_lineStyle.SC];
            strokeStyle.DashStyle = DashStyle = DashStyles[_lineStyle.DS];

            if (EndCapType.SelectedItem != (object)EndCap) EndCapType.SelectedItem = EndCap;
            if (StartCapType.SelectedItem != (object)StartCap) StartCapType.SelectedItem = StartCap;

            if (WidthSlider.Value != _lineStyle.Width) WidthSlider.Value = _lineStyle.Width;
 
            if (SliderColorA.Value != _lineStyle.A) SliderColorA.Value = _lineStyle.A;
            if (SliderColorR.Value != _lineStyle.R) SliderColorR.Value = _lineStyle.R;
            if (SliderColorG.Value != _lineStyle.G) SliderColorG.Value = _lineStyle.G;
            if (SliderColorB.Value != _lineStyle.B) SliderColorB.Value = _lineStyle.B;

            var s1 = ColorHexValue.Text;
            if (string.IsNullOrWhiteSpace(s1)) s1 = "";
            var s2 = _lineStyle.HexColor;
            if (s1 != s2)
            {
                ColorHexValue.Text = s2;
                ColorSample.Background = new SolidColorBrush(_lineStyle.Color);
            }

            UpdateSymbolSize();
        }
        private void UpdateSelectedLineStyles()
        {
            foreach (var line in _selectLines)
            {
                line.Style.Set(_prevStyle, _lineStyle);
            }
            if (_isValidUI) DrawCanvas.Invalidate();
        }
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
            if (_pointerIsPressed && DragAction != null) DragAction();
        }

        private void DrawCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _pointerIsPressed = false;
            _drawRef.Point2 = DrawPoint(e);

            EndAction?.Invoke();
        }

        private XYPoint RawPoint(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(DrawCanvas).Position;
            var x = (p.X - _offset);
            var y = (p.Y - _offset);
            return new XYPoint(x, y);
        }

        private XYPoint DrawPoint(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(DrawCanvas).Position;
            var x = Round((p.X - _offset) / _zoomFactor);
            var y = Round((p.Y - _offset) / _zoomFactor);
            return new XYPoint(x, y);
        }
        private int Round(double v)
        {
            var n = (int)v;
            var m = n + 1;
            return ((m - v) < (v - n)) ? m : n;
        }
        #endregion

        #region SelectCanvas_PointerEvents  ===================================
        private void SelectCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(SelectCanvas).Position;
            var index = (int)(p.Y / _indexDelta) - 1;


            if (index < 0)
            {
                _isAllSelected = !_isAllSelected;
                foreach (var line in _workingLines) { line.IsSelected = false; }
            }
            else if (index < _workingLines.Count)
            {
                _isAllSelected = false;
                _workingLines[index].IsSelected = !_workingLines[index].IsSelected;
            }
            else
            {
                _isAllSelected = false;
                foreach (var line in _workingLines) { line.IsSelected = false; }
            }

            var isAny = TryGetSelected();
            CutButton.IsEnabled = isAny;
            CopyButton.IsEnabled = isAny;
            PasteButton.IsEnabled = isAny;
            VFlipButton.IsEnabled = isAny;
            HFlipButton.IsEnabled = isAny;
            RotateButton.IsEnabled = isAny;

            if (TryGetSelected())
            {
                var last = _selectLines.Count - 1;
                _lineStyle.Set(_selectLines[last].Style);
                UpdataLineStyleUI();
            }

            PenButton.Background = inactiveColor;
            Consolidate();

            SetHitTestAction();
            DrawCanvas.Invalidate();
        }
        #endregion
    }
}
