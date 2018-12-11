using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ModelGraph.Controls
{
    internal abstract class Shape
    {
        public const float FullSize = 200;
        public const float HalfSize = 100;
        public static readonly Vector2 Center = new Vector2(HalfSize);
        protected CanvasStrokeStyle _strokeStyle = new CanvasStrokeStyle();
        protected byte _a = 255;
        protected byte _r = 255;
        protected byte _g = 255;
        protected byte _b = 255;
        protected byte _w = 1;
        protected byte _sc = 0;
        protected byte _ec = 0;
        protected byte _ds = 0;

        public CanvasStrokeStyle StrokeStyle()
        {
            var ss = new CanvasStrokeStyle();
            var ds = DashStyle;
            ss.DashStyle = DashStyle;
            ss.StartCap = StartCap;
            ss.EndCap = EndCap;

            ss.DashCap = CanvasCapStyle.Round;
            ss.LineJoin = CanvasLineJoin.Round;
            return ss;
        }
        public CanvasDashStyle DashStyle { get { return (CanvasDashStyle)_ds; } set { _ds = (byte)value; } }
        public CanvasCapStyle StartCap { get { return (CanvasCapStyle)_sc; } set { _sc = (byte)value; } }
        public CanvasCapStyle EndCap { get { return (CanvasCapStyle)_ec; } set { _ec = (byte)value; } }

        public float Width { get { return _w; } set { _w = (byte)((value < 1) ? 1 : (value > 20) ? 20 : value); } }
        public Color Color => Color.FromArgb(_a, _r, _g, _b);
        public string ColorCode { get { return $"#{_a}{_r}{_g}{_b}"; } set { SetColor(value); } }

        #region SetColor  =====================================================
        private void SetColor(string code)
        {
            var (a, r, g, b) = GetARGB(code);
            _a = a;
            _r = r;
            _g = g;
            _b = b;
        }

        private static (byte, byte, byte, byte) GetARGB(string argbStr)
        {
            if (IsInvalid(argbStr)) return _invalidColor;
            var argb = _invalidColor; // default color when there is a bad color string

            var ca = argbStr.ToLower().ToCharArray();
            if (ca[0] != '#') return _invalidColor;

            var N = _argbLength;
            int[] va = new int[N];
            for (int j = 1; j < N; j++)
            {
                va[j] = _hexValues.IndexOf(ca[j]);
                if (va[j] < 0) return _invalidColor;
            }
            return ((byte)((va[1] << 4) | va[2]), (byte)((va[3] << 4) | va[4]), (byte)((va[5] << 4) | va[6]), (byte)((va[7] << 4) | va[8]));
        }
        static bool IsValid(string argbStr) => !IsInvalid(argbStr);
        static bool IsInvalid(string argbStr) => (string.IsNullOrWhiteSpace(argbStr) || argbStr.Length != _argbLength);
        static readonly (byte, byte, byte, byte) _invalidColor = (0x88, 0x87, 0x86, 0x85);
        static readonly string _hexValues = "0123456789abcdef";
        const int _argbLength = 9;
        #endregion

        internal abstract CanvasGeometry GetGeometry(ICanvasResourceCreator resourceCreator, float scale, Vector2 center);
        internal abstract Func<Vector2, Shape> CreateShapeFunction { get; }
        internal abstract void  Move(Vector2 delta);
        internal abstract Vector2 ValideDelta(Vector2 delta);

        internal void Draw(CanvasControl ctl, CanvasDrawingSession ds, float scale, Vector2 center, int index = -1)
        {
            if (index != -1)
            {
                var w = (float)ctl.Width;
                var hw = w / 2;
                var y1 = index * w;
                var y2 = y1 + w;
                ds.DrawLine(hw,y1,hw,y2, Color.FromArgb(0x80, 0xff, 0x8f, 0xff), w);
            }
            var geo = GetGeometry(ctl, scale, center);
            ds.DrawGeometry(geo, Color, Width, StrokeStyle());
        }

    }
}
