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
    internal class Line : PolyLine
    {
        internal Line(Vector2 delta) : base(delta, Profile.Line) { }
 
        internal override Func<Vector2, Shape> CreateShapeFunction => (delta) => new Line(delta);
    }
}
