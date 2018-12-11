using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;

namespace ModelGraph.Controls
{
    internal static class ShapeUtils
    {
        internal static List<T> GetEnumAsList<T>() => Enum.GetValues(typeof(T)).Cast<T>().ToList();
        private static List<CanvasDashStyle> _dashStyles = GetEnumAsList<CanvasDashStyle>();
        private static List<CanvasCapStyle> _capStyles = GetEnumAsList<CanvasCapStyle>();
        private static List<CanvasLineJoin> _lineJoins = GetEnumAsList<CanvasLineJoin>();
        private static List<CanvasStrokeStyle> _strokeStyles = GetEnumAsList<CanvasStrokeStyle>();

        internal static CanvasCapStyle GetCapStyle(int n) => (n < 0) ? CanvasCapStyle.Flat : (n< _capStyles.Count) ? _capStyles[n] : _capStyles[0];
        internal static CanvasLineJoin GetLineJoin(int n) => (n < 0) ? _lineJoins[0] : (n < _lineJoins.Count) ? _lineJoins[n] : _lineJoins[0];
        static CanvasDashStyle GetDashStyle(int n) => (n < 0) ? _dashStyles[0] : (n < _dashStyles.Count) ? _dashStyles[n] : _dashStyles[0];
        public static CanvasStrokeStyle GetStrokeStyle(int n) => (n < 0) ? _strokeStyles[0] : (n < _strokeStyles.Count) ? _strokeStyles[n] : _strokeStyles[0];

        public static List<CanvasCapStyle> CapStyles => _capStyles;
        public static List<CanvasLineJoin> LineJoins => _lineJoins;
        public static List<CanvasDashStyle> DashStyles => _dashStyles;

        public static float DegreesToRadians(float angle)
        {
            return angle * (float)Math.PI / 180;
        }

        public static Matrix3x2 GetDisplayTransform(Vector2 outputSize, Vector2 sourceSize)
        {
            // Scale the display to fill the control.
            var scale = outputSize / sourceSize;
            var offset = Vector2.Zero;

            // Letterbox or pillarbox to preserve aspect ratio.
            if (scale.X > scale.Y)
            {
                scale.X = scale.Y;
                offset.X = (outputSize.X - sourceSize.X * scale.X) / 2;
            }
            else
            {
                scale.Y = scale.X;
                offset.Y = (outputSize.Y - sourceSize.Y * scale.Y) / 2;
            }

            return Matrix3x2.CreateScale(scale) *
                   Matrix3x2.CreateTranslation(offset);
        }
    }
}
