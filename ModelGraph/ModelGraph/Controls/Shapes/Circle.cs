﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ModelGraph.Controls
{
    internal class Circle : Shape
    {
        private float _dx;
        private float _dy;
        private float _radius = 30;

        internal Circle(Vector2 delta)
        {
            Width = 3;
            Move(delta);
        }

        #region OverideAbstract  ==============================================
        internal override Func<Vector2, Shape> CreateShapeFunction => (delta) => new Circle(delta);
        internal override void Move(Vector2 delta)
        {
            var d = ValideDelta(delta);
            _dx += d.X;
            _dy += d.Y;
        }

        internal override CanvasGeometry GetGeometry(ICanvasResourceCreator resourceCreator, float scale, Vector2 center)
        {
            return CanvasGeometry.CreateCircle(resourceCreator, (new Vector2(_dx, _dy) * scale + center), (_radius * scale));
        }
        #endregion

        #region ValidateDelta  ================================================
        internal override Vector2 ValideDelta(Vector2 delta)
        {
            var dxmin = HalfSize;
            var dymin = HalfSize;

            var dxmax = -HalfSize;
            var dymax = -HalfSize;

            var dx = _radius + _dx;
            var dy = _radius + _dy;

            if (dx > dxmax) dxmax = dx;
            if (dx < dxmin) dxmin = dx;
            if (dy > dymax) dymax = dy;
            if (dy < dymin) dymin = dy;

            dxmin = -(HalfSize + dxmin);
            dymin = -(HalfSize + dymin);
            dxmax = (HalfSize - dxmax);
            dymax = (HalfSize - dymax);
            dx = delta.X;
            dy = delta.Y;
            if (dx < dxmin) dx = dxmin;
            if (dx > dxmax) dx = dxmax;
            if (dy < dymin) dy = dymin;
            if (dy > dymax) dy = dymax;
            return new Vector2(dx, dy);
        }
        #endregion
    }
}
