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


        private bool CanUndo { get { return _undoStack.Count > 0; } }
        private bool CanRedo { get { return _redoStack.Count > 0; } }


        public (bool canUndo, bool canRedo, int undoCount, int redoCount) UndoRedoParms => (CanUndo, CanRedo, _undoStack.Count, _redoStack.Count);

        public void TryUndo()
        {
            if (CanUndo)
            {
                 var prev = _undoStack.Pop();

                _redoStack.Push(new Snapshot(prev));
                prev.Restore();
            }
        }

        public void TryRedo()
        {
            if (CanRedo)
            {
                var prev = _redoStack.Pop();

                _undoStack.Push(new Snapshot(prev));
                prev.Restore();
            }
        }
    }
}
