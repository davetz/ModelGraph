namespace ModelGraphSTD
{
    internal class NodeParm
    {
        internal Node Node;
        private readonly (int X, int Y, byte DX, byte DY, byte Color, byte Symbol, Labeling Labeling, Sizing Resizing, BarWidth BarWidth, FlipRotate FlipRotate, Orient Orientation)  _parms;

        internal NodeParm(Node node) { Node = node; _parms = node.Parms; }
        internal void Restore() => Node.Parms = _parms;
    }
}
