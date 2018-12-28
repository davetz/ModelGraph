using System;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class Line : PolyLine
    {
        internal Line(Vector2 delta) : base(delta, Profile.Line) { }
 
        internal override Func<Vector2, Shape> CreateShape => (delta) => new Line(delta);
    }
}
