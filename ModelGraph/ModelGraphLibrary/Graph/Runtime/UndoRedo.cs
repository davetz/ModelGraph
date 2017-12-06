using System.Collections.Generic;

namespace ModelGraphLibrary
{/*
 */
    public partial class Graph
    {
        private Stack<ParmCopy> _undoStack = new Stack<ParmCopy>();
        private Stack<ParmCopy> _redoStack = new Stack<ParmCopy>();
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
                var parms = _undoStack.Pop();
                var redo = parms.GetCurrent();// we need to copy the current values before we restore the saved ones
                _redoStack.Push(redo);
                parms.Restore();
            }
        }

        private void Redo()
        {
            if (CanRedo)
            {
                var parms = _redoStack.Pop();
                var undo = parms.GetCurrent();// we need to copy the current values before we restore the saved ones
                _undoStack.Push(undo);
                parms.Restore();
            }
        }
        #endregion

        #region PushSnapShot  =================================================
        internal void PushSnapShot(ParmCopy copy)
        {
            _undoStack.Push(copy);
        }
        #endregion
    }
}
