using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;

namespace ModelGraph.Controls
{
    internal abstract partial class Shape
    {
        private const int PointCountIndex = 19;
        private byte A = 0xFF; // of color(A, R, G, B)
        private byte R = 0xFF; // of color(A, R, G, B)
        private byte G = 0xFF; // of color(A, R, G, B)
        private byte B = 0xF0; // of color(A, R, G, B)
        private byte SW = 1;  // stroke width
        private byte SC = 2;  // startCap
        private byte EC = 2;  // endCap
        private byte DC = 2;  // dashCap
        private byte LJ = 3;  // line join
        private byte DS;      // dash style
        private byte FS;      // fill stroke
        private byte R1 = 1;  // minor axis (inner, horzontal) (1 to 128)
        private byte R2 = 1;  // major axis (outer, vertical) (1 to 128)
        private byte F1;      // auxiliary factor (for PolyGear and PolyPulse) (0 to 100 %)
        private byte PD = 1;  // polyline dimension specific to the shape type
        private byte PL;      // the polyline control parameters are locked
        private byte A0;      // rotation index for 22.5 degree delta
        private byte A1;      // rotation index for 30.0 degree delta
        protected List<(float dx, float dy)> DXY;  // one or more defined points

        #region Properties  ===================================================

        #region Color  ========================================================
        internal enum Coloring { Gray, Light, Normal };

        internal Color Color => Color.FromArgb(A, R, G, B);
        internal string ColorCode { get { return $"#{A:X}{R:X}{G:X}{B:X}"; } set { SetColor(value); } }
        internal Color GetColor(Coloring c) => (c == Coloring.Normal) ? Color : (c == Coloring.Light) ? Color.FromArgb(0x60, R, G, B) : Color.FromArgb(0x60, 0X80, 0X80, 0X80);

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

        #region StrokeStyle  ==================================================
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

        internal CanvasDashStyle DashStyle { get { return (CanvasDashStyle)DS; } set { DS = (byte)value; } }
        internal CanvasCapStyle StartCap { get { return (CanvasCapStyle)SC; } set { SC = (byte)value; } }
        internal CanvasCapStyle EndCap { get { return (CanvasCapStyle)EC; } set { EC = (byte)value; } }
        internal CanvasLineJoin LineJoin { get { return (CanvasLineJoin)LJ; } set { LJ = (byte)value; } }
        internal CanvasCapStyle DashCap { get { return (CanvasCapStyle)DC; } set { DC = (byte)value; } }

        internal Fill_Stroke FillStroke { get { return (Fill_Stroke)FS; } set { FS = (byte)value; } }

        internal float StrokeWidth { get { return SW; } set { SW = (byte)((value < 1) ? 1 : (value > 20) ? 20 : value); } }
        #endregion

        #region Radius  =======================================================
        public static byte ToByte(float v, float L = 0)
        {
            if (v < L) v = L;
            var b = v * 255;
            if (b > 255) b = 255;
            return (byte)b;
        }
        public static sbyte ToSByte(float v)
        {
            if (v < -1) v = -1;
            if (v >  1) v =  1;

            return (sbyte)(v * 127);
        }
        public static float ToFloat(byte b) => b / 255f;
        public static float ToFloat(sbyte s) => s / 127f;

        public static (float, float) ToFloat((sbyte dx, sbyte dy) p) => (ToFloat(p.dx), ToFloat(p.dy));
        public static Vector2 ToVector((sbyte dx, sbyte dy) p) => new Vector2(ToFloat(p.dx), ToFloat(p.dy));
        public static (sbyte dx, sbyte dy) ToSByte(Vector2 p) => (ToSByte(p.X), ToSByte(p.Y));

        protected float Radius1 { get { return ToFloat(R1); } set { R1 = ToByte(value, 0.004f); } }
        protected float Radius2 { get { return ToFloat(R2); } set { R2 = ToByte(value, 0.004f); } }
        protected float AuxFactor { get { return ToFloat(F1); } set { F1 = ToByte(value); } }

        protected Vector2 Radius => new Vector2(Radius1, Radius2);
        protected (float r1, float r2, float f1) GetRadius(float scale) => (Radius1 * scale, Radius2 * scale, AuxFactor * scale);
        protected (float r1, float r2, float f1) GetRadius() => (Radius1, Radius2, AuxFactor);
        #endregion

        #region Dimension  ====================================================
        protected int Dimension
        {
            get { return PD; }

            set
            {
                var (min, max) = MinMaxDimension;
                PD = (byte)((value < min) ? min : (value > max) ? max : value);
            }
        }
        #endregion

        #region Radians  ======================================================
        protected static float FullRadians = (float)(2 * Math.PI);
        protected static float DeltaRadians0 = (float)(Math.PI / 8);
        protected static float DeltaRadians1 = (float)(Math.PI / 6);
        protected float RotateLeftRadians0 => -DeltaRadians0;
        protected float RotateRightRadians0 => DeltaRadians0;
        protected float RotateLeftRadians1 => -DeltaRadians1;
        protected float RotateRightRadians1 => DeltaRadians1;
        protected float RadiansStart => (A0 % 16) * DeltaRadians0 + (A1 % 12) * DeltaRadians1;

        protected void RotateStartLeft0() { A0 = (byte)((A0 - 1) % 16); }
        protected void RotateStartLeft1() { A1 = (byte)((A1 - 1) % 12); }
        protected void RotateStartRight0() { A0 = (byte)((A0 + 1) % 16); }
        protected void RotateStartRight1() { A1 = (byte)((A1 + 1) % 12); }
        #endregion

        #region Sliders  ======================================================
        internal bool IsLocked { get { return PL != 0; } set { PL = (byte)(value ? 1 : 0); } }
        internal double AuxSlider { get { return 100 * AuxFactor; } set { AuxFactor = (float)value / 100; CreatePoints(); } }
        internal double MajorSlider { get { return 100 * Radius2; } set { Radius2 = (float)value / 100; CreatePoints(); } }
        internal double MinorSlider { get { return 100 * Radius2; } set { Radius1 = (float)value / 100; CreatePoints(); } }
        #endregion

        #endregion

        #region Serialize  ====================================================
        public static byte[] Serialize(IEnumerable<Shape> shapes)
        {
            var data = new List<byte>(shapes.Count() * 30);

            foreach (var shape in shapes)
            {
                data.Add(shape.TypeCode);

                data.Add(shape.A);
                data.Add(shape.R);
                data.Add(shape.G);
                data.Add(shape.B);
                data.Add(shape.SW);
                data.Add(shape.SC);
                data.Add(shape.EC);
                data.Add(shape.DC);
                data.Add(shape.LJ);
                data.Add(shape.DS);
                data.Add(shape.FS);
                data.Add(shape.R1);
                data.Add(shape.R2);
                data.Add(shape.F1);
                data.Add(shape.PD);
                data.Add(shape.PL);
                data.Add(shape.A0);
                data.Add(shape.A1);

                var points = shape.DXY;
                var count = points.Count;
                if (count > byte.MaxValue) count = byte.MaxValue;

                for (int i = 0; i < count; i++)
                {
                    var (dx, dy) = points[i];
                    data.Add((byte)ToSByte(dx));
                    data.Add((byte)ToSByte(dy));
                }
            }

            return data.ToArray();
        }
        #endregion

        #region ShapeType  ====================================================
        internal enum ShapeType : byte
        {
            Line = 1,
            Circle = 2,
            Ellipse = 3,
            PolySide = 4,
            PolyStar = 5,
            PolyGear = 6,
            PolyWave = 7,
            Rectangle = 8,
            PolySpike = 9,
            PolyPulse = 10,
            PolySpring = 11,
            RoundedRectangle = 12,
        }
        #endregion

        #region Deserialize  ==================================================
        static public void Deserialize (byte[] data, List<Shape> shapes)
        {
            shapes.Clear();
            var M = data.Length;

            var I = 0;
            while (I + PointCountIndex < M)
            {
                var st = (ShapeType)data[I];
                var pc = data[I + PointCountIndex];

                switch (st)
                {
                    case ShapeType.Line:
                        shapes.Add(new Line(I, data));
                        break;

                    case ShapeType.Circle:
                        shapes.Add(new Circle(I, data));
                        break;

                    case ShapeType.Ellipse:
                        shapes.Add(new Ellipes(I, data));
                        break;

                    case ShapeType.PolySide:
                        shapes.Add(new PolySide(I, data));
                        break;

                    case ShapeType.PolyStar:
                        shapes.Add(new PolyStar(I, data));
                        break;

                    case ShapeType.PolyGear:
                        shapes.Add(new PolyGear(I, data));
                        break;

                    case ShapeType.PolyWave:
                        shapes.Add(new PolyWave(I, data));
                        break;

                    case ShapeType.Rectangle:
                        shapes.Add(new Rectangle(I, data));
                        break;

                    case ShapeType.PolySpike:
                        shapes.Add(new PolySpike(I, data));
                        break;

                    case ShapeType.PolyPulse:
                        shapes.Add(new PolyPulse(I, data));
                        break;

                    case ShapeType.PolySpring:
                        shapes.Add(new PolySpring(I, data));
                        break;

                    case ShapeType.RoundedRectangle:
                        shapes.Add(new RoundedRectangle(I, data));
                        break;
                    default:
                        return; // stop and disregard invalid shape data
                }
                I += pc + PointCountIndex;
            }
        }
        #endregion

        #region ReadData  =====================================================
        void ReadData(int I, byte[] data)
        {
            A = data[I++];
            R = data[I++];
            G = data[I++];
            B = data[I++];
            SW = data[I++];
            SC = data[I++];
            EC = data[I++];
            DC = data[I++];
            LJ = data[I++];
            DS = data[I++];
            FS = data[I++];
            R1 = data[I++];
            R2 = data[I++];
            F1 = data[I++];
            PD = data[I++];
            PL = data[I++];
            A0 = data[I++];
            A1 = data[I++];
            var pc = data[I++];
            if (pc > 0)
            {
                DXY = new List<(float dx, float dy)>(pc);
                for (int i = 0; i < pc; i++)
                {
                    var dx = ((sbyte)data[I++]) / 127.0f; //use as much of the sbyte
                    var dy = ((sbyte)data[I++]) / 127.0f; //precision as posible
                    DXY.Add((dx, dy));
                }
            }
        }
        #endregion

        #region CopyData  =====================================================
        protected void CopyData(Shape s)
        {
            A = s.A;
            R = s.R;
            G = s.G;
            B = s.B;
            SW = s.SW;
            SC = s.SC;
            EC = s.EC;
            DC = s.DC;
            LJ = s.LJ;
            DS = s.DS;
            FS = s.FS;
            R1 = s.R1;
            R2 = s.R2;
            F1 = s.F1;
            PD = s.PD;
            PL = s.PL;
            A0 = s.A0;
            A1 = s.A1;
            DXY = new List<(float dx, float dy)>(s.DXY);
        }
        #endregion
    }
}
