using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas;
using ModelGraphSTD;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using System.Numerics;
using Windows.Foundation;
using System.Diagnostics;

namespace ModelGraphUWP
{
    public sealed partial class ModelGraphControl
    {
        private List<Color> _groupColor = new List<Color>() { Color.FromArgb(255, 255, 255, 127) };
        private float _zoomFactor;
        private Vector2 _offset;
        private Extent _viewExtent = new Extent();

        #region DrawingStyles  ================================================
        public static List<T> GetEnumAsList<T>() { return Enum.GetValues(typeof(T)).Cast<T>().ToList(); }
        public List<CanvasDashStyle> DashStyles { get { return GetEnumAsList<CanvasDashStyle>(); } }
        public List<CanvasCapStyle> CapStyles { get { return GetEnumAsList<CanvasCapStyle>(); } }
        public List<CanvasLineJoin> LineJoins { get { return GetEnumAsList<CanvasLineJoin>(); } }
        #endregion

        #region DrawCanvas_Draw  ==============================================
        private void DrawCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            #region RefreshGroupColors  =======================================
            if (_graph.GroupColor.Count > 0)
            {
                _groupColor.Clear();
                foreach (var (a, r, g, b) in _graph.GroupColor)
                {
                    _groupColor.Add(Color.FromArgb(a, r, g, b));
                }
            }
            #endregion

            #region InitializeZoomFactor  =====================================
            if (_zoomFactor == 0)
            {
                Initialize(_graph.Extent);
                _viewExtent = _graph.Extent;
                
                RootGrid.Width = RootCanvas.Width = DrawCanvas.Width = ActualWidth;
                RootGrid.Height = RootCanvas.Height = DrawCanvas.Height = ActualHeight;
            }
            #endregion


            #region Initialize  ===============================================
            var x = _viewExtent.Xmin;
            var y = _viewExtent.Ymin;
            var z = _zoomFactor;
            var pen = new DrawingPen(_zoomFactor, _offset, args.DrawingSession);
            #endregion

            #region DrawEdges  ================================================
            pen.Width = 2;
            pen.Color = Colors.Magenta;
            for (int i = 0; i < _graph.EdgeCount; i++)
            {
                var points = _graph.Edges[i].Points;
                var len = (points == null) ? 0 : points.Length;
                if (len > 1)
                {
                    var ix1 = _graph.Edges[i].Node1.Core.Color;
                    var ix2 = _graph.Edges[i].Node2.Core.Color;
                    var ix = (ix1 > ix2) ? ix1 : ix2;
                    pen.Color = _groupColor[ix];
                    pen.Initialize();
                    for (int j = 0; j < len; j++)
                    {
                        pen.DrawLine(points[j]);
                    }
                }
            }
            #endregion

            #region DrawNodes  ================================================
            for (int i = 0; i < _graph.NodeCount; i++)
            {
                var node = _graph.Nodes[i];
                pen.Color = Colors.Magenta;

                var k = node.Core.Symbol - 2;
                if (k < 0 || k >= _graph.SymbolCount || _graph.Symbols[k].Data == null)
                {
                    pen.Color = _groupColor[node.Core.Color];
                    pen.Initialize();
                    if (node.Core.IsPointNode)
                    {
                        pen.DrawPoint(node.Core);
                    }
                    else
                    {
                        pen.DrawBusBar(node.Core.X, node.Core.Y, node.Core.DX, node.Core.DY);
                    }
                }
                else
                {
                    var sym = _graph.Symbols[k];
                    _drawSymbol[(int)node.Core.FlipRotate & 7](node, sym, pen);
                }
            }
            #endregion

            #region RegionTrace  ==============================================
            pen.Width = 2;
            pen.Style.DashStyle = CanvasDashStyle.Dot;

            if (_traceRegion != null)
            {
                pen.Color = (_traceRegion.IsPolygon) ? Colors.DimGray : Colors.LightGray;
                pen.DrawRectangle(_traceRegion.Normal, x, y, z);

                var points = _traceRegion.Points;
                pen.Color = (_traceRegion.IsPolygon) ? Colors.LightGray : Colors.DimGray;
                pen.Initialize();
                for (int i = 0; i < points.Count; i++)
                {
                    pen.DrawLine(points[i]);
                }
            }
            #endregion

            #region DrawRegions  ==============================================
            if (_selector.Regions.Count > 0)
            {
                foreach (var region in _selector.Regions)
                {
                    var points = region.Points;
                    pen.Color = Colors.LightGray;
                    if (region.IsPolygon)
                    {
                        pen.Initialize();
                        for (int i = 0; i < points.Count; i++)
                        {
                            pen.DrawLine(points[i]);
                        }
                        pen.DrawLine(points[0]);
                    }
                    else
                    {
                        pen.DrawRectangle(region.Normal, x, y, z);
                    }
                }
            }
            #endregion
        }

        #region DrawingPen  ===================================================
        /// <summary>
        /// Helper class used for draw lines and symbols.
        /// </summary>
        private class DrawingPen
        {
            internal float Width;
            internal Color Color;
            internal CanvasStrokeStyle Style;
            private CanvasDrawingSession _session;

            private float _zoom;
            private Vector2 _p1;
            private Vector2 _offset;
            private bool _firstItteration;

            internal DrawingPen(float zoom, Vector2 offset, CanvasDrawingSession session)
            {
                _zoom = zoom;
                _offset = offset;
                _session = session;
                Style = new CanvasStrokeStyle
                {
                    EndCap = CanvasCapStyle.Round,
                    DashCap = CanvasCapStyle.Round,
                    StartCap = CanvasCapStyle.Round,
                    DashStyle = CanvasDashStyle.Solid
                };
            }

            internal void Initialize()
            {
                Style.EndCap = CanvasCapStyle.Round;
                Style.DashCap = CanvasCapStyle.Round;
                Style.StartCap = CanvasCapStyle.Round;
                Style.DashStyle = CanvasDashStyle.Solid;
                _firstItteration = true;
            }

            internal int Initialize(byte[] data, int index)
            {
                var d = index;
                var A = data[d++];  // 0
                var R = data[d++];  // 1
                var G = data[d++];  // 2
                var B = data[d++];  // 3
                var W = data[d++];  // 4
                var SC = data[d++]; // 5
                var EC = data[d++]; // 6
                var DC = data[d++]; // 7
                var DS = data[d++]; // 8

                Width = W * _zoom;
                Color = Color.FromArgb(A, R, G, B);
                Style.EndCap = (CanvasCapStyle)EC;
                Style.StartCap = (CanvasCapStyle)SC;
                Style.DashCap = (CanvasCapStyle)DC;
                Style.DashStyle = (CanvasDashStyle)DS;

                _firstItteration = true;
                return index + 9;
            }
            internal void DrawLine(XYPoint pxy) => DrawLine(new Vector2(pxy.X, pxy.Y));
            internal void DrawLine(Vector2 point)
            {
                Vector2 p2 = (point + _offset) * _zoom;

                if (_firstItteration)
                {
                    _firstItteration = false;
                }
                else
                {
                    _session.DrawLine(_p1, p2, Color, Width, Style);
                }

                _p1 = p2;
                Debug.WriteLine($"{point} => {p2},   O: {_offset} Z: {_zoom}");
            }
            internal void DrawRectangle(Extent e) => DrawRectangle(new Rect(e.Xmin, e.Ymin, e.Width, e.Hieght));
            internal void DrawRectangle(Extent e, int x, int y, float z)
            {
                var r = new Rect(e.Xmin, e.Ymin, e.Width, e.Hieght);

                DrawRectangle(new Rect(z * (r.X - x), z * (r.Y - y), z * r.Width, z * r.Height));
            }
            internal void DrawRectangle(Rect rect)
            {
                _session.DrawRectangle(rect, Color, Width);
            }

            internal void DrawPoint(NodeX core)
            {
                var center = new Vector2(core.X, core.Y);
                var radius = core.Radius;
                Vector2 p = (center + _offset) * _zoom;
                _session.DrawLine(p, p, Color, (radius * 2 * _zoom), Style);
            }

            internal void DrawBusBar(float x, float y, float dx, float dy)
            {
                if (dx > dy)
                {
                    Vector2 p1 = (new Vector2(x - dx, y) + _offset) * _zoom;
                    Vector2 p2 = (new Vector2(x + dx, y) + _offset) * _zoom;
                    _session.DrawLine(p1, p2, Color, (dy * 2 * _zoom));
                }
                else
                {
                    Vector2 p1 = (new Vector2(x, y - dy) + _offset) * _zoom;
                    Vector2 p2 = (new Vector2(x, y + dy) + _offset) * _zoom;
                    _session.DrawLine(p1, p2, Color, (dx * 2 * _zoom));
                }
            }
        }
        #endregion

        #region DrawSymbol  ===================================================
        private static Action<Node, SymbolX, DrawingPen>[] _drawSymbol = new Action<Node, SymbolX, DrawingPen>[]
        {
            SymbolFlipRotateNone,
            SymbolFlipVertical,
            SymbolFlipHorizontal,
            SymbolFlipBothWays,
            SymbolRotateClockWise,
            SymbolRotateFlipVertical,
            SymbolRotateFlipHorizontal,
            SymbolRotateFlipBothWays,
        };

        #region SymbolFlipRotateNone  =========================================
        private static void SymbolFlipRotateNone(Node node, SymbolX sym, DrawingPen slave)
        {
            var data = sym.Data;
            int len = data.Length;
            int max = len - 13; // the last valid line data record must begin before this value
            for (int d = 2; d < max;)
            {
                d = slave.Initialize(data, d);

                var pc = data[d++];
                if (pc < 2)
                {
                    continue;               // abort, point count too small
                }

                if ((d + (2 * pc)) > len)
                {
                    continue; // abort, point count too large
                }

                for (int j = 0; j < pc; j++)
                {
                    var dx = (sbyte)data[d++];
                    var dy = (sbyte)data[d++];
                    slave.DrawLine(new Vector2(node.Core.X + dx, node.Core.Y + dy));
                }
            }
        }
        #endregion

        #region SymbolFlipVertical  ===========================================
        private static void SymbolFlipVertical(Node node, SymbolX sym, DrawingPen slave)
        {
            var data = sym.Data;
            int len = data.Length;
            int max = len - 13; // the last valid line data record must begin before this value
            for (int d = 2; d < max;)
            {
                d = slave.Initialize(data, d);

                var pc = data[d++];
                if (pc < 2)
                {
                    continue;               // abort, point count too small
                }

                if ((d + (2 * pc)) > len)
                {
                    continue; // abort, point count too large
                }

                for (int j = 0; j < pc; j++)
                {
                    var dx = (sbyte)data[d++];
                    var dy = (sbyte)data[d++];
                    slave.DrawLine(new Vector2(node.Core.X + dx, node.Core.Y - dy));
                }
            }
        }
        #endregion

        #region SymbolFlipHorizontal  =========================================
        private static void SymbolFlipHorizontal(Node node, SymbolX sym, DrawingPen slave)
        {
            var data = sym.Data;
            int len = data.Length;
            int max = len - 13; // the last valid line data record must begin before this value
            for (int d = 2; d < max;)
            {
                d = slave.Initialize(data, d);

                var pc = data[d++];
                if (pc < 2)
                {
                    continue;               // abort, point count too small
                }

                if ((d + (2 * pc)) > len)
                {
                    continue; // abort, point count too large
                }

                for (int j = 0; j < pc; j++)
                {
                    var dx = (sbyte)data[d++];
                    var dy = (sbyte)data[d++];
                    slave.DrawLine(new Vector2(node.Core.X - dx, node.Core.Y + dy));
                }
            }
        }
        #endregion

        #region SymbolFlipBothWays  ===========================================
        private static void SymbolFlipBothWays(Node node, SymbolX sym, DrawingPen slave)
        {
            var data = sym.Data;
            int len = data.Length;
            int max = len - 13; // the last valid line data record must begin before this value
            for (int d = 2; d < max;)
            {
                d = slave.Initialize(data, d);

                var pc = data[d++];
                if (pc < 2)
                {
                    continue;               // abort, point count too small
                }

                if ((d + (2 * pc)) > len)
                {
                    continue; // abort, point count too large
                }

                for (int j = 0; j < pc; j++)
                {
                    var dx = (sbyte)data[d++];
                    var dy = (sbyte)data[d++];
                    slave.DrawLine(new Vector2(node.Core.X - dx, node.Core.Y - dy));
                }
            }
        }
        #endregion

        #region SymbolRotateClockWise  ========================================
        private static void SymbolRotateClockWise(Node node, SymbolX sym, DrawingPen slave)
        {
            var data = sym.Data;
            int len = data.Length;
            int max = len - 13; // the last valid line data record must begin before this value
            for (int d = 2; d < max;)
            {
                d = slave.Initialize(data, d);

                var pc = data[d++];
                if (pc < 2)
                {
                    continue;               // abort, point count too small
                }

                if ((d + (2 * pc)) > len)
                {
                    continue; // abort, point count too large
                }

                for (int j = 0; j < pc; j++)
                {
                    var dx = (sbyte)data[d++];
                    var dy = (sbyte)data[d++];
                    slave.DrawLine(new Vector2(node.Core.X - dy, node.Core.Y + dx));
                }
            }
        }
        #endregion

        #region SymbolRotateFlipVertical  =====================================
        private static void SymbolRotateFlipVertical(Node node, SymbolX sym, DrawingPen slave)
        {
            var data = sym.Data;
            int len = data.Length;
            int max = len - 13; // the last valid line data record must begin before this value
            for (int d = 2; d < max;)
            {
                d = slave.Initialize(data, d);

                var pc = data[d++];
                if (pc < 2)
                {
                    continue;               // abort, point count too small
                }

                if ((d + (2 * pc)) > len)
                {
                    continue; // abort, point count too large
                }

                for (int j = 0; j < pc; j++)
                {
                    var dx = (sbyte)data[d++];
                    var dy = (sbyte)data[d++];
                    slave.DrawLine(new Vector2(node.Core.X - dy, node.Core.Y - dx));
                }
            }
        }
        #endregion

        #region SymbolRotateFlipHorizontal  ===================================
        private static void SymbolRotateFlipHorizontal(Node node, SymbolX sym, DrawingPen slave)
        {
            var data = sym.Data;
            int len = data.Length;
            int max = len - 13; // the last valid line data record must begin before this value
            for (int d = 2; d < max;)
            {
                d = slave.Initialize(data, d);

                var pc = data[d++];
                if (pc < 2)
                {
                    continue;               // abort, point count too small
                }

                if ((d + (2 * pc)) > len)
                {
                    continue; // abort, point count too large
                }

                for (int j = 0; j < pc; j++)
                {
                    var dx = (sbyte)data[d++];
                    var dy = (sbyte)data[d++];
                    slave.DrawLine(new Vector2(node.Core.X + dy, node.Core.Y + dx));
                }
            }
        }
        #endregion

        #region SymbolRotateFlipBothWays  =====================================
        private static void SymbolRotateFlipBothWays(Node node, SymbolX sym, DrawingPen slave)
        {
            var data = sym.Data;
            int len = data.Length;
            int max = len - 13; // the last valid line data record must begin before this value
            for (int d = 2; d < max;)
            {
                d = slave.Initialize(data, d);

                var pc = data[d++];
                if (pc < 2)
                {
                    continue;               // abort, point count too small
                }

                if ((d + (2 * pc)) > len)
                {
                    continue; // abort, point count too large
                }

                for (int j = 0; j < pc; j++)
                {
                    var dx = (sbyte)data[d++];
                    var dy = (sbyte)data[d++];
                    slave.DrawLine(new Vector2(node.Core.X + dy, node.Core.Y - dx));
                }
            }
        }
        #endregion

        #endregion
        #endregion

        #region PanZoom  ======================================================
        const float maxZoomFactor = 2;
        const float minZoomDiagonal = 8000;

        internal void ZoomIn() { Zoom(_zoomFactor * 1.1f); }
        internal void ZoomOut() { Zoom(_zoomFactor / 1.1f); }
        internal void ZoomReset() { Zoom(1); }

        #region Secondary  ====================================================
        void Zoom(float zoom, bool toClosestNode = false)
        {
            var z = zoom;
            var p = _graph.Extent.Center;
            if (toClosestNode && _graph.NodeCount > 0)
            {
                p = _drawRef.Point2;
                var e = new Extent(p);
                if (_graph.NodeCount > 0)
                {
                    for (int i = 0; i < _graph.NodeCount; i++)
                    {
                        _graph.Nodes[i].Core.Minimize(e);
                    }
                }
                p = e.Point2;
            }
            ZoomToPoint(zoom, p);
        }

        internal void PanToPoint(XYPoint p)
        {
            ZoomToPoint(_zoomFactor, p);
        }
        #endregion

        #region Primary  ======================================================
        void ZoomToPoint(float zoom, XYPoint p)
        {
            var z = (zoom < maxZoomFactor) ? zoom : maxZoomFactor;
            if (_graph.Extent.Diagonal * z < minZoomDiagonal)
            {
                z = minZoomDiagonal / _graph.Extent.Diagonal;
            }

            _zoomFactor = z;

            var dx = (int)(ActualWidth / z);
            _viewExtent.X1 = p.X - (dx / 2);
            _viewExtent.X2 = _viewExtent.X1 + dx;

            var dy = (int)(ActualHeight / z);
            _viewExtent.Y1 = p.Y - (dy / 2);
            _viewExtent.Y2 = _viewExtent.Y1 + dy;

            DrawCanvas.Invalidate();
        }

        private void ZoomToExtent(Extent extent)
        {
            Initialize(extent);
            DrawCanvas.Invalidate();
        }
        void Initialize(Extent extent)
        {
            var aw = (float)ActualWidth;
            var ah = (float)ActualHeight;
            var ew = (float)extent.Width;
            var eh = (float)extent.Hieght;

            if (aw < 1) aw = 1;
            if (ah < 1) ah = 1;
            if (ew < 1) ew = 1;
            if (eh < 1) eh = 1;

            var zw = aw / ew;
            var zh = ah / eh;
            var z = (zw < zh) ? zw : zh;

            if (z > maxZoomFactor) z = maxZoomFactor;

            _zoomFactor = z;
            _offset = new Vector2((aw - ew * z) / 2, (ah - (eh * z)) / 2);
            _viewExtent = extent;
        }

        private void ScrollVerticalDelta(double dy)
        {
            var x = _offset.X;
            var y = (float)(_offset.Y + (dy / _zoomFactor));
            _offset = new Vector2(x, y);

            DrawCanvas.Invalidate(); 
        }

        private void ScrollHorizontalDelta(double dx)
        {
            var x = (float)(_offset.X + (dx / _zoomFactor)); 
            var y = _offset.Y;
            _offset = new Vector2(x, y);

            DrawCanvas.Invalidate();
        }
        #endregion
        #endregion
    }
}
