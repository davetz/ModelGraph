using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public partial class Graph
    {
        private Stack<Snapshot> _undoStack = new Stack<Snapshot>();
        private Stack<Snapshot> _redoStack = new Stack<Snapshot>();

        internal void TakeSnapshot(Selector selector)
        {
            _undoStack.Push(new Snapshot(selector));
        }
        private void ClearUndoRedo()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        #region UndoRedo  =====================================================
        private bool CanUndo { get { return _undoStack.Count > 0; } }
        private bool CanRedo { get { return _redoStack.Count > 0; } }

        private void Undo()
        {
            if (CanUndo)
            {
                var snap = _undoStack.Pop();
                _redoStack.Push(new Snapshot(snap));
                snap.Restore();
            }
        }

        private void Redo()
        {
            if (CanRedo)
            {
                var snap = _redoStack.Pop();
                _undoStack.Push(new Snapshot(snap));
                snap.Restore();
            }
        }
        #endregion
    }
}
