using System;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal abstract partial class Shape
    {
        private const int HeaderPointCountIndex = 15;
        protected byte ST;      // shapte type code
        protected byte A = 0xFF; // color(A, R, G, B)
        protected byte R = 0xFF; // color(A, R, G, B)
        protected byte G = 0xFF; // color(A, R, G, B)
        protected byte B = 0xF0; // color(A, R, G, B)
        protected byte SW = 1;  // stroke width
        protected byte SC = 2;  // startCap
        protected byte EC = 2;  // endCap
        protected byte DC = 2;  // dashCap
        protected byte LJ = 3;  // line join
        protected byte DS;      // dash style
        protected byte FS;      // fill stroke
        protected byte R1;      // minor axis (inner, horzontal)
        protected byte R2;      // major axis (outer, vertical)
        protected byte R3;      // auxiliary axis (for PolyGear)
        protected byte PD;      // polyline dimension specific to the shape type
        protected byte VM;      // variation modifier specific to the shape type
        protected byte A0;      // rotation index for 22.5 degree delta
        protected byte A1;      // rotation index for 30.0 degree delta
        protected List<(float dx, float dy)> DXY;  // zero or more defined points

        #region ShapeType  ====================================================
        internal enum ShapeType : byte
        {
            Circle = 0,
            Ellipse = 1,
            PolySide = 2,
            PolyStar = 3,
            PolyGear = 4,
            Rectangle = 5,
            PolySpline = 6,
            RoundedRectangle = 8,
        }
        #endregion

        #region Deserialize  ==================================================
        static public void Deserialize (byte[] data, List<Shape> shapes)
        {
            shapes.Clear();
            var M = data.Length;

            var I = 0;
            while(I + HeaderPointCountIndex < M)
            {
                var st = data[I];
                if (st <= (byte)ShapeType.RoundedRectangle)
                {
                    var pc = data[I + HeaderPointCountIndex];

                    switch ((ShapeType)st)
                    {
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

                        case ShapeType.Rectangle:
                            shapes.Add(new Rectangle(I, data));
                            break;

                        case ShapeType.PolySpline:
                            shapes.Add(new PolySpline(I, data));
                            break;

                        case ShapeType.RoundedRectangle:
                            shapes.Add(new RoundedRectangle(I, data));
                            break;
                    }
                    I += pc + HeaderPointCountIndex;
                }
                else return; // stop and disregard invalid shape data
            }
        }
        #endregion

        #region ReadData  =====================================================
        void ReadData(int I, byte[] data)
        {
            ST = data[I++];
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
            R3 = data[I++];
            PD = data[I++];
            VM = data[I++];
            A0 = data[I++];
            A1 = data[I++];
            var pc = data[I++];
            if (pc > 0)
            {
                DXY = new List<(float dx, float dy)>(pc);
                for (int i = 0; i < pc; i++)
                {
                    DXY.Add(((sbyte)data[I++], (sbyte)data[I++]));
                }
            }
        }
        #endregion

        #region CopyData  =====================================================
        protected void CopyData(Shape s)
        {
            ST = s.ST; ;
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
            R3 = s.R3;
            PD = s.PD;
            VM = s.VM;
            A0 = s.A0;
            A1 = s.A1;
            DXY = new List<(float dx, float dy)>(s.DXY);
        }
        #endregion
    }
}
