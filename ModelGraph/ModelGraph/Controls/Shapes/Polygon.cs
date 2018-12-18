using Microsoft.Graphics.Canvas;
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
    internal class Polygon : PolyLine
    {
        internal Polygon(Vector2 delta) : base(delta, Profile.Polygon) { }
 
        internal override Func<Vector2, Shape> CreateShapeFunction => (delta) => new Polygon(delta);
    }
}
