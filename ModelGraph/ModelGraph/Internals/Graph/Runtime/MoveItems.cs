
namespace ModelGraph.Internals
{/*
 */
    public partial class Graph
    {
        #region Undo, Redo  ===================================================
        public void TryUndo()
        {
            if (CanUndo) Undo();
        }
        public void TryRedo()
        {
            if (CanRedo) Redo();
        }
        #endregion

        #region Flip, Align, Rotate  ==========================================
        internal void TryAlignVertical(Selector regions, int x)
        {
            foreach (var node in regions.Nodes)
            {
                node.Core.AlignVertical(x);
            }
        }

        internal void TryAlignHorizontal(Selector regions, int y)
        {
            foreach (var node in regions.Nodes)
            {
                node.Core.AlignHorizontal(y);
            }
        }
        #endregion
    }
}
