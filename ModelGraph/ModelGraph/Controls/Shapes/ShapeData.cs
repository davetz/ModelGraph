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
        protected byte SC;      // startCap
        protected byte EC;      // endCap
        protected byte DC = 2;  // dashCap
        protected byte LJ = 3;  // line join
        protected byte DS;      // dash style
        protected byte FS;      // fill stroke
        protected byte R1;      // minor axis (inner, horzontal)
        protected byte R2;      // major axis (outer, vertical)
        protected byte R3;      // auxiliary axis (circomference)
        protected byte PD;      // polygon dimension
        protected List<(sbyte dx, sbyte dy)> DXY;  // zero or more defined points

        #region Deserialize  ==================================================
        static public void Deserialize (byte[] data, List<Shape> shapes)
        {
            shapes.Clear();
            var M = data.Length;

            var I = 0;
            while(I + HeaderPointCountIndex < M)
            {
                var st = data[I];
                if (st < (byte)ShapeType.InvalidShapeType)
                {
                    var pc = data[I + HeaderPointCountIndex];

                    switch ((ShapeType)st)
                    {
                        case ShapeType.Line:
                            break;
                        case ShapeType.Circle:
                            shapes.Add(new Circle(I, data));
                            break;
                        case ShapeType.Ellipse:
                            break;
                        case ShapeType.Rectangle:
                            break;
                        case ShapeType.RoundedRectangle:
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
            var pc = data[I++];
            if (pc > 0)
            {
                DXY = new List<(sbyte dx, sbyte dy)>(pc);
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
            DXY = new List<(sbyte dx, sbyte dy)>(s.DXY);
        }
        #endregion
    }
}
