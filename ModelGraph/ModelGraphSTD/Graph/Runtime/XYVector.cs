using System;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraphSTD
{
    public static class XYVector
    {
        #region Matrix3x2  ====================================================
        static internal Matrix3x2 Rotation45cw(Vector2 focus) => Matrix3x2.CreateRotation(cw45, focus);
        static internal Matrix3x2 Rotation90cw(Vector2 focus) => Matrix3x2.CreateRotation(cw90, focus);
        static internal Matrix3x2 Rotation135cw(Vector2 focus) => Matrix3x2.CreateRotation(cw135, focus);
        static internal Matrix3x2 Rotation180(Vector2 focus) => Matrix3x2.CreateRotation(cc180, focus);
        static internal Matrix3x2 Rotation135cc(Vector2 focus) => Matrix3x2.CreateRotation(cc135, focus);
        static internal Matrix3x2 Rotation90cc(Vector2 focus) => Matrix3x2.CreateRotation(cc90, focus);
        static internal Matrix3x2 Rotation45cc(Vector2 focus) => Matrix3x2.CreateRotation(cc45, focus);
        static internal Matrix3x2 FlipVertical(Vector2 focus) => Matrix3x2.CreateScale(1, -1, focus);
        static internal Matrix3x2 FlipHorizontal(Vector2 focus) => Matrix3x2.CreateScale(-1, 1, focus);
        static internal Matrix3x2 FlipBothWays(Vector2 focus) => Matrix3x2.CreateScale(-1, -1, focus);

        static readonly float cc45 = (float)(Math.PI / 4.0);
        static readonly float cc90 = (float)(Math.PI / 2.0);
        static readonly float cc135 = (float)(Math.PI * 3.0 / 4.0);
        static readonly float cc180 = (float)(Math.PI);
        static readonly float cw135 = -cc135;
        static readonly float cw90 = -cc90;
        static readonly float cw45 = -cc45;
        #endregion

        #region QuadSliceSlope  ===============================================
        static readonly double slope1 = Math.Tan(Math.PI / 8.0);         // 22.5 degrees
        static readonly double slope2 = 1.0f;                            // 45.0 degrees
        static readonly double slope3 = Math.Tan(Math.PI * 3.0 / 8.0);   // 67.5 degrees
        public static (Quad quad, Slice slice, float slope) QuadSliceSlope((float x, float y) p1, (float x, float y) p2)
        {
            var dx = p2.x - p1.x;
            var dy = p2.y - p1.y;
            bool isVert = dx * dx < .2;
            bool isHorz = dy * dy < .2;

            if (isVert)
            {
                if (isHorz)
                    return (Quad.Any, Slice.S0, 0);
                else
                    return (dy > 0) ? (Quad.Q1, Slice.S2, 1023) : (Quad.Q4, Slice.S7, -1024);
            }
            else
            {
                if (isHorz)
                    return (dx > 0) ? (Quad.Q1, Slice.S1, 0) : (Quad.Q2, Slice.S4, 0);
                else
                {
                    var slope = (dy / dx);
                    if (dx < 0)
                    {
                        if (dy < 0)
                        {//======================================== Quad_3
                            if (slope > slope3)
                                return (Quad.Q3, Slice.S11, slope);
                            else if (slope > slope2)
                                return (Quad.Q3, Slice.S10, slope);
                            else if (slope > slope1)
                                return (Quad.Q3, Slice.S9, slope);
                            else
                                return (Quad.Q3, Slice.S8, slope);
                        }
                        else
                        {//======================================== Quad_2
                            if (slope < -slope3)
                                return (Quad.Q2, Slice.S4, slope);
                            else if (slope < -slope2)
                                return (Quad.Q2, Slice.S5, slope);
                            else if (slope < -slope1)
                                return (Quad.Q2, Slice.S6, slope);
                            else
                                return (Quad.Q2, Slice.S7, slope);
                        }
                    }
                    else
                    {
                        if (dy < 0)
                        {//======================================== Quad_4
                            if (slope < -slope3)
                                return (Quad.Q4, Slice.S12, slope);
                            else if (slope < -slope2)
                                return (Quad.Q4, Slice.S13, slope);
                            else if (slope < -slope1)
                                return (Quad.Q4, Slice.S14, slope);
                            else
                                return (Quad.Q4, Slice.S15, slope);
                        }
                        else
                        {//======================================== Quad_1
                            if (slope > slope3)
                                return (Quad.Q1, Slice.S3, slope);
                            else if (slope > slope2)
                                return (Quad.Q1, Slice.S2, slope);
                            else if (slope > slope1)
                                return (Quad.Q1, Slice.S1, slope);
                            else
                                return (Quad.Q1, Slice.S0, slope);
                        }
                    }
                }
            }
        }
        #endregion

        #region AfterFlipRotate  ==============================================
        static internal FlipRotate AfterRotateCW(FlipRotate flip) => _afterRotateCW[flip & FlipRotate.FlipRotate315];
        static internal FlipRotate AfterRotateCCW(FlipRotate flip) => _afterRotateCCW[flip & FlipRotate.FlipRotate315];
        static internal FlipRotate AfterFlipVertical(FlipRotate flip) => _afterVerticalFlip[flip & FlipRotate.FlipRotate315];
        static internal FlipRotate AfterFlipHorizontal(FlipRotate flip) => _afterHorizontalFlip[flip & FlipRotate.FlipRotate315];

        static readonly Dictionary<FlipRotate, FlipRotate> _afterRotateCW = new Dictionary<FlipRotate, FlipRotate>()
        {
            [FlipRotate.None] = FlipRotate.Rotate45,
            [FlipRotate.Rotate45] = FlipRotate.Rotate90,
            [FlipRotate.Rotate90] = FlipRotate.Rotate135,
            [FlipRotate.Rotate135] = FlipRotate.Rotate180,
            [FlipRotate.Rotate180] = FlipRotate.Rotate225,
            [FlipRotate.Rotate225] = FlipRotate.Rotate270,
            [FlipRotate.Rotate270] = FlipRotate.Rotate315,
            [FlipRotate.Rotate315] = FlipRotate.None,
            [FlipRotate.FlipVertical] = FlipRotate.FlipRotate45,
            [FlipRotate.FlipRotate45] = FlipRotate.FlipRotate90,
            [FlipRotate.FlipRotate90] = FlipRotate.FlipRotate135,
            [FlipRotate.FlipRotate135] = FlipRotate.FlipRotate180,
            [FlipRotate.FlipRotate180] = FlipRotate.FlipRotate225,
            [FlipRotate.FlipRotate225] = FlipRotate.FlipRotate270,
            [FlipRotate.FlipRotate270] = FlipRotate.FlipRotate315,
            [FlipRotate.FlipRotate315] = FlipRotate.FlipVertical,
        };
        static readonly Dictionary<FlipRotate, FlipRotate> _afterRotateCCW = new Dictionary<FlipRotate, FlipRotate>()
        {
            [FlipRotate.None] = FlipRotate.Rotate315,
            [FlipRotate.Rotate45] = FlipRotate.None,
            [FlipRotate.Rotate90] = FlipRotate.Rotate45,
            [FlipRotate.Rotate135] = FlipRotate.Rotate90,
            [FlipRotate.Rotate180] = FlipRotate.Rotate135,
            [FlipRotate.Rotate225] = FlipRotate.Rotate180,
            [FlipRotate.Rotate270] = FlipRotate.Rotate225,
            [FlipRotate.Rotate315] = FlipRotate.Rotate270,
            [FlipRotate.FlipVertical] = FlipRotate.FlipRotate315,
            [FlipRotate.FlipRotate45] = FlipRotate.FlipRotate270,
            [FlipRotate.FlipRotate90] = FlipRotate.FlipRotate225,
            [FlipRotate.FlipRotate135] = FlipRotate.FlipRotate180,
            [FlipRotate.FlipRotate180] = FlipRotate.FlipRotate135,
            [FlipRotate.FlipRotate225] = FlipRotate.FlipRotate90,
            [FlipRotate.FlipRotate270] = FlipRotate.FlipRotate45,
            [FlipRotate.FlipRotate315] = FlipRotate.FlipVertical,
        };
        static readonly Dictionary<FlipRotate, FlipRotate> _afterVerticalFlip = new Dictionary<FlipRotate, FlipRotate>()
        {
            [FlipRotate.None] = FlipRotate.FlipVertical,
            [FlipRotate.Rotate45] = FlipRotate.FlipRotate315,
            [FlipRotate.Rotate90] = FlipRotate.FlipRotate270,
            [FlipRotate.Rotate135] = FlipRotate.FlipRotate225,
            [FlipRotate.Rotate180] = FlipRotate.FlipRotate180,
            [FlipRotate.Rotate225] = FlipRotate.FlipRotate135,
            [FlipRotate.Rotate270] = FlipRotate.FlipRotate90,
            [FlipRotate.Rotate315] = FlipRotate.FlipRotate45,
            [FlipRotate.FlipVertical] = FlipRotate.None,
            [FlipRotate.FlipRotate45] = FlipRotate.Rotate315,
            [FlipRotate.FlipRotate90] = FlipRotate.Rotate270,
            [FlipRotate.FlipRotate135] = FlipRotate.Rotate225,
            [FlipRotate.FlipRotate180] = FlipRotate.Rotate180,
            [FlipRotate.FlipRotate225] = FlipRotate.Rotate135,
            [FlipRotate.FlipRotate270] = FlipRotate.Rotate90,
            [FlipRotate.FlipRotate315] = FlipRotate.Rotate45,
        };
        static readonly Dictionary<FlipRotate, FlipRotate> _afterHorizontalFlip = new Dictionary<FlipRotate, FlipRotate>()
        {
            [FlipRotate.None] = FlipRotate.FlipRotate180,
            [FlipRotate.Rotate45] = FlipRotate.FlipRotate135,
            [FlipRotate.Rotate90] = FlipRotate.FlipRotate90,
            [FlipRotate.Rotate135] = FlipRotate.FlipRotate45,
            [FlipRotate.Rotate180] = FlipRotate.FlipVertical,
            [FlipRotate.Rotate225] = FlipRotate.FlipRotate315,
            [FlipRotate.Rotate270] = FlipRotate.FlipRotate270,
            [FlipRotate.Rotate315] = FlipRotate.FlipRotate225,
            [FlipRotate.FlipVertical] = FlipRotate.Rotate180,
            [FlipRotate.FlipRotate45] = FlipRotate.Rotate135,
            [FlipRotate.FlipRotate90] = FlipRotate.Rotate90,
            [FlipRotate.FlipRotate135] = FlipRotate.Rotate45,
            [FlipRotate.FlipRotate180] = FlipRotate.None,
            [FlipRotate.FlipRotate225] = FlipRotate.Rotate315,
            [FlipRotate.FlipRotate270] = FlipRotate.Rotate270,
            [FlipRotate.FlipRotate315] = FlipRotate.Rotate225,
        };
        #endregion
    }
}
