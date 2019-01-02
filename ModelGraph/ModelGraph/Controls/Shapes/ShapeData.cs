using System;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal abstract partial class Shape
    {
        private const int HeaderPointCountIndex = 14;
        protected byte ST = 0;  // shapte type code
        protected byte A = 255;
        protected byte R = 255;
        protected byte G = 255;
        protected byte B = 255;
        protected byte SW = 1;  // stroke width
        protected byte SC = 0;  // startCap
        protected byte EC = 0;  // endCap
        protected byte DC = 0;  // dashCap
        protected byte LJ = 3;  // line join
        protected byte DS = 0;  // dash style
        protected byte FS = 0;  // fill stroke
        protected byte PS = 3;  // number of polygon sides
        protected byte RF = 0;  // rotate flip state code
        protected List<(sbyte dx, sbyte dy)> DXY;  // zero or more defined points
        protected List<(float dx, float dy)> Points;  // zero or more output points

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
                        case ShapeType.Arc:
                            break;
                        case ShapeType.Line:
                            break;
                        case ShapeType.Spline:
                            break;
                        case ShapeType.Circle:
                            shapes.Add(new Circle(I, data));
                            break;
                        case ShapeType.Ellipse:
                            break;
                        case ShapeType.Polygon:
                            break;
                        case ShapeType.Polyline:
                            break;
                        case ShapeType.ClosedPolyline:
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
            PS = data[I++];
            RF = data[I++];
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

        #region CopyData  ====================================================
        protected void CopyData(Shape s)
        {
            ST = s.ST; ;
            A = s.A;
            R = s.B;
            G = s.G;
            B = s.B;
            SW = s.SW;
            SC = s.SC;
            EC = s.EC;
            DC = s.DC;
            LJ = s.LJ;
            DS = s.DS;
            FS = s.FS;
            PS = s.PS;
            RF = s.RF;
            DXY = new List<(sbyte dx, sbyte dy)>(s.DXY);
        }
        #endregion
    }
}
