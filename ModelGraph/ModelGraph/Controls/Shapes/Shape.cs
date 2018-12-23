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
        internal const float FULLSIZE = 200;    // full size of the symbol's graphic definition
        internal const float HALFSIZE = 100;    // half size (radius) of the symbol's graphic definition
        internal static Vector2 Center = new Vector2(HALFSIZE); // geometric center of symbol's definition

        #region CommonProperties  =============================================
        protected byte A = 255;
        protected byte R = 255;
        protected byte G = 255;
        protected byte B = 255;
        protected byte SW = 1;  //stroke width
        protected byte SC = 0;  //startCap
        protected byte EC = 0;  //endCap
        protected byte DC = 0;  //dashCap
        protected byte LJ = 3;  //line join
        protected byte DS = 0;  //dash style
        protected byte FS = 0;  //fill stroke
        protected byte PS = 3;  //polygon sides

        internal bool IsSelected;

        public CanvasStrokeStyle StrokeStyle()
        {            
            var ss = _strokeStyle;
            ss.DashStyle = DashStyle;
            ss.StartCap = StartCap;
            ss.EndCap = EndCap;
            ss.DashCap = DashCap;
            ss.LineJoin = LineJoin; ;
            return ss;
        }
        private CanvasStrokeStyle _strokeStyle = new CanvasStrokeStyle();

        public CanvasDashStyle DashStyle { get { return (CanvasDashStyle)DS; } set { DS = (byte)value; } }
        public CanvasCapStyle StartCap { get { return (CanvasCapStyle)SC; } set { SC = (byte)value; } }
        public CanvasCapStyle EndCap { get { return (CanvasCapStyle)EC; } set { EC = (byte)value; } }
        public CanvasLineJoin LineJoin { get { return (CanvasLineJoin)LJ; } set { LJ = (byte)value; } }
        public CanvasCapStyle DashCap { get { return (CanvasCapStyle)DC; } set { DC = (byte)value; } }

        public  Fill_Stroke FillStroke { get { return (Fill_Stroke)FS; } set { FS = (byte)value; } }
        public PolygonSides PolygonSide { get { return (PolygonSides)PS; } set { PS = (byte)value; } }

        public float StrokeWidth { get { return SW; } set { SW = (byte)((value < 1) ? 1 : (value > 20) ? 20 : value); } }
        public Color Color => Color.FromArgb(A, R, G, B);
        public string ColorCode { get { return $"#{A}{R}{G}{B}"; } set { SetColor(value); } }
        #endregion

        #region SetColor  =====================================================
        private void SetColor(string code)
        {
            var (a, r, g, b) = GetARGB(code);
            A = a;
            R = r;
            G = g;
            B = b;
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

        #region CopyToClone  ==================================================
        protected Shape CopyToClone(Shape clone)
        {
            clone.A = A;
            clone.R = R;
            clone.G = G;
            clone.B = B;
            clone.SW = SW;
            clone.SC = SC;
            clone.EC = EC;
            clone.DC = DC;
            clone.LJ = LJ;
            clone.DS = DS;
            return clone;
        }
        #endregion

        #region HighLight  ====================================================
        internal static void HighLight(CanvasDrawingSession ds, float width, int index)
        {
            var hw = width / 2;
            var y1 = index * width;
            var y2 = y1 + width;
            ds.DrawLine(hw, y1, hw, y2, Colors.SlateGray, width);
        }
        #endregion

        #region GetCenter  ====================================================
        internal static (float dx, float dy) GetCenter(List<(float dx, float dy)> points)
        {
            var N = points.Count;
            if (N == 0) return (0, 0);

            float dxmin, dxmax, dymin, dymax;

            dxmin = dxmax = points[0].dx;
            dymin = dymax = points[0].dy;
            for (int i = 1; i < N; i++)
            {
                var (dx, dy) = points[i];
                if (dx < dxmin) dxmin = dx;
                if (dy < dymin) dymin = dy;

                if (dx > dxmax) dxmax = dx;
                if (dy > dymax) dymax = dy;
            }
            return ((dxmax + dxmin) / 2, (dymax + dymin) / 2);
        }
        internal void ReCenter()
        {

        }
        #endregion

        #region RequiredMethods  ==============================================
        internal abstract Shape Clone();
        internal abstract Func<Vector2, Shape> CreateShape { get; }

        internal abstract void Move(Vector2 delta);
        internal abstract Vector2 ValideDelta(Vector2 delta);

        internal abstract void Draw(CanvasControl ctl, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth);

        internal abstract void GetPoints(List<(float dx, float dy)> points);
        internal abstract void SetPoints(List<(float dx, float dy)> points);
        #endregion
    }
}
