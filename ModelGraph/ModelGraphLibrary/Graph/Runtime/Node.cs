
namespace ModelGraphSTD
{
    public class Node : Item
    {
        public Item Item;
        public NodeX Core = new NodeX();
        public int OpenPathIndex = -1;

        #region Constructor  ==================================================
        internal Node(int cp = 0)
        {
            Owner = null;
            Trait = Trait.Node;
            Core.X = Core.Y = cp;
            Core.DX = Core.DY = (byte)GraphParm.MinNodeSize;
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal Graph Graph { get { return Owner as Graph; } }
        internal GraphX GraphX { get { return (Owner == null) ? null : Owner.Owner as GraphX; } }

        internal bool HasOpenPaths => (OpenPathIndex >= 0);
        internal int OpenPathCount => (Graph == null) ? 0 : Graph.OpenPathCount(OpenPathIndex);

        //internal Rect Rect { get { return Core.Rect; } }
        //internal Rect GetRect(int x, int y, float z)
        //{
        //    var r = Core.Rect;
        //    return new Rect(z * (r.X - x), z * (r.Y - y), z * r.Width, z * r.Height);
        //}

        internal void Move(XYPoint delta)
        {
            Core.X = Core.X + delta.X;
            Core.Y = Core.Y + delta.Y;
        }
        #endregion
    }
}
