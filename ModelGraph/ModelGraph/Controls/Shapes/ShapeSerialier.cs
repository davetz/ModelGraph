using System;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal abstract partial class Shape
    {
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
            PS = data[I++];
            RF = data[I++];
            var pc = data[I++];
            if (pc > 0)
            {
                DXY = new (sbyte dx, sbyte dy)[pc];
                for (int i = 0; i < pc; i++)
                {
                    DXY[i] = ((sbyte)data[I++], (sbyte)data[I++]);
                }
            }
        }
    }
}
