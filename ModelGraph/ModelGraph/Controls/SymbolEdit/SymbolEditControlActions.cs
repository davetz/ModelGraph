using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Windows.System;

namespace ModelGraph.Controls
{
    public sealed partial class SymbolEditControl
    {/*
        The action flow is controled by the curent state and a fixed
        set of action vectors (function pointers).
        When entering a new state the action vectors are reprogramed.
        This makes the programing problem staight forward and piecemeal.
        I am in this particular state, what are the permited event actions?
        A side effect is a simpler event handle,
        e.g. TryInvokeEventAction(eventEnum);
     */
        #region EventAction  ==================================================
        private enum PointerEvent
        {
            None = 0,
            End = 1,            // pointer goes up
            Drag = 2,           // pointer move with button down
            Hover = 3,          // pointer move with button up
            Wheel = 4,          // mouse wheel changed
            Begin = 5,          // pointer goes down
            Execute = 6,        // pointer double tap
        }

        private void ClearEventActions() => _eventAction.Clear();

        private void SetEventAction(PointerEvent e, Action a) => _eventAction[EventKey(e)] = a;
        private void ClearEventAction(PointerEvent e) => _eventAction.Remove(EventKey(e));
        private void TryInvokeEventAction(PointerEvent e)
        {
            if (_eventAction.TryGetValue(EventKey(e), out Action action))
            {
                action.Invoke();

                EditorCanvas.Invalidate();
            }
        }

        private void SetEventAction(VirtualKey k, VirtualKeyModifiers m, Action a) => _eventAction[EventKey(k, m)] = a;
        private void ClearEventAction(VirtualKey k, VirtualKeyModifiers m) => _eventAction.Remove(EventKey(k, m));
        private void TryInvokeEventAction(VirtualKey k, VirtualKeyModifiers m)
        {
            if (_eventAction.TryGetValue(EventKey(k, m), out Action action))
            {
                action.Invoke();

                EditorCanvas.Invalidate();
            }
        }

        #region Hidden  =======================================================
        private int EventKey(PointerEvent e) => 0xFFF00 | (int)e;
        private int EventKey(VirtualKey k, VirtualKeyModifiers m) => (int)k << 4 | (int)m & 0xF;
        private Dictionary<int, Action> _eventAction = new Dictionary<int, Action>();
        #endregion
        #endregion

        #region EditorState  ==================================================
        internal enum EditorState
        {
            IdleOnVoid,         // default, sitting idle with pointer over empty space

            IdleOnLinePoint,    // hovering over a point of a line
            IdleOnCenterPoint,  // hovering over a shapes center point

            MovingLinePoint,    // dragging line point with the pointer button down
            MovingCenterPoint,  // dragging whole shape with the pointer button down

            NewShapePlacement,  // a shape is available from the picker, waiting for pointer down
            DragNewShape,       // new shape has been place and now the user is dragging it somewhere

            ShapesAreSelected,  // one or more shapes have been selected

            DragSelectorShape,  // changing the drawing order of the symbol shapes
        }
        
        private EditorState _editorState;   // used to log event action state transitions

        private bool SetEditorState(EditorState editorState)
        {
            if (_editorState == editorState) return false;
            _editorState = editorState;
            ClearEventActions();

            Debug.WriteLine($"{editorState}");
            return true;
        }
        #endregion

        #region TryHitTest  ===================================================
        private bool TryHitTest(Vector2 rawPoint, out int index)
        {
            var N = _targetPoints.Count;
            for (int i = 0; i < N; i++)
            {
                var dp = rawPoint - _targetPoints[i];
                if (dp.LengthSquared() > 50) continue;
                index = i;
                return true;
            }
            index = -1;
            return false;
        }
        #endregion

        #region SetPickerShape  ===============================================
        private void SetPicker(Shape pickerShape)
        {
            PickerShape = pickerShape;
            SetNewShapePlacement();
        }
        #endregion

        #region SetIdleOnVoid  ================================================
        private void SetIdleOnVoid()
        {
            if (SetEditorState(EditorState.IdleOnVoid))
            {
                PickerShape = null;
                SelectedShapes.Clear();

                PickerCanvas.Invalidate();
                EditorCanvas.Invalidate();
            }
        }
        #endregion

        #region SetNewShapePlacement  =========================================
        private void SetNewShapePlacement()
        {
            if (SetEditorState(EditorState.NewShapePlacement))
            {
                SetEventAction(PointerEvent.Begin, AddNewShape);
                SetEventAction(PointerEvent.Drag, BeginDragNewShape);
            }
        }

        private void AddNewShape()
        {
            if (PickerShape != null)
            {
                var newShape = PickerShape.Clone(ShapePoint1);
                DragShapes[0] = newShape;
                SymbolShapes.Add(newShape);

                SetProperty(newShape, ProertyId.All);

                EditorCanvas.Invalidate();
            }
        }
        private void BeginDragNewShape()
        {
            PickerShape = null;
            PickerCanvas.Invalidate();

            SetDragNewShape();
        }
        #endregion

        #region SetDragNewShape  ==============================================
        Shape[] DragShapes = new Shape[1];
        private void SetDragNewShape()
        {
            if (SetEditorState(EditorState.DragNewShape))
            {
                SetEventAction(PointerEvent.Drag, DragNewShape);
                SetEventAction(PointerEvent.End, EndDragNewShape);

                EditorCanvas.Invalidate();
            }
        }
        private void DragNewShape()
        {
            Shape.MoveCenter(DragShapes, ShapeDelta);
            RawPoint1 = RawPoint2;
            ShapePoint1 = ShapePoint(RawPoint2);

            EditorCanvas.Invalidate();
        }
        private void EndDragNewShape()
        {
            SetEditorState(EditorState.IdleOnVoid);
        }
        #endregion

        #region SetShapesAreSelected  =========================================
        private void SetShapesAreSelected()
        {
            if (SetEditorState(EditorState.ShapesAreSelected)) //enable hit test
            {
                SetSizeSliders();
                SetEventAction(PointerEvent.Begin, TargetPointerDown);

                PickerShape = null;
                PickerCanvas.Invalidate();
                EditorCanvas.Invalidate();
            }
        }
        private void TargetPointerDown()
        {
            if (TryHitTest(RawPoint1, out int index))
            {
                if (index < 0) SetIdleOnVoid();

                SetEventAction(PointerEvent.End, EndTargetDrag);

                if (index == 0)
                    SetEventAction(PointerEvent.Drag, DragCenterPoint);
                else
                {
                    _linePointIndex = index - 1;
                    SetEventAction(PointerEvent.Drag, DragLinePoint);
                }
            }
            else
                SetIdleOnVoid();
        }
        int _linePointIndex;
        private void DragCenterPoint()
        {
            Shape.MoveCenter(SelectedShapes, ShapeDelta);
            EditorCanvas.Invalidate();
        }
        private void DragLinePoint()
        {
            _polylineTarget.MovePoint(_linePointIndex, ShapeDelta);
            EditorCanvas.Invalidate();
        }
        private void EndTargetDrag()
        {
            ClearEventAction(PointerEvent.Drag);
        }

        private void ShapeSelectorHit(int index)
        {
            var shape = SymbolShapes[index];

            if (IsSelectOneOrMoreShapeMode)
            {
                if (SelectedShapes.Contains(shape))
                    SelectedShapes.Remove(shape);
                else
                    SelectedShapes.Add(shape);
            }
            else
            {
                SelectedShapes.Clear();
                SelectedShapes.Add(shape);
            }

            GrtProperty(shape);
            PickerShape = null;

            if (SelectedShapes.Count > 0)
                SetShapesAreSelected();
            else
                SetIdleOnVoid();

            PickerShape = null;
            PickerCanvas.Invalidate();
            EditorCanvas.Invalidate();
        }
        private void ShapeSelectorMiss()
        {
            SelectedShapes.Clear();
            SelectorCanvas.Invalidate();
            SetIdleOnVoid();
        }
        #endregion
    }
}
