using ModelGraphSTD;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace ModelGraph.Controls
{
    public sealed partial class SymbolEditControl
    {/*
        The action flow is controled by the curent state and a fixed
        set of action vectors (function pointers).
        When entering a new state the action vectors are reprogramed.
        This makes the programing problem staight forward and piecemeal.
     */
        #region EventAction  ==================================================
        private void ClearEventActions()
        {
            _eventAction.Clear();
            ClearKeyboardAccelerator();
        }

        #region PointerEvents  ================================================
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
        #endregion

        #region KeyboardEvents  ===============================================
        private void SetEventAction(VirtualKey k, VirtualKeyModifiers m, Action a)
        {
            TryAddAccelerator(k, m);
            _eventAction[EventKey(k, m)] = a;
        }
        private void ClearEventAction(VirtualKey k, VirtualKeyModifiers m)
        {
            _eventAction.Remove(EventKey(k, m));
        }

        private void KeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var k = args.KeyboardAccelerator.Key;
            var m = args.KeyboardAccelerator.Modifiers;
            if (_eventAction.TryGetValue(EventKey(k, m), out Action action))
            {
                action.Invoke();
                EditorCanvas.Invalidate();
            }
        }

        #region KeyboardAccelerators  =========================================
        private void ClearKeyboardAccelerator()
        {
            foreach (var acc in EditorCanvas.KeyboardAccelerators)
            {
                acc.Invoked -= KeyboardAccelerator_Invoked;
            }
            EditorCanvas.KeyboardAccelerators.Clear();
        }
        private void TryAddAccelerator(VirtualKey k, VirtualKeyModifiers m)
        {
            if (TryFindAccelerator(k, m, out KeyboardAccelerator _)) return;
            var acc = new Windows.UI.Xaml.Input.KeyboardAccelerator
            {
                Key = k,
                Modifiers = m
            };
            acc.Invoked += KeyboardAccelerator_Invoked;
            EditorCanvas.KeyboardAccelerators.Add(acc);
        }
        private void TryRemoveAccelerator(VirtualKey k, VirtualKeyModifiers m)
        {
            if (TryFindAccelerator(k, m, out KeyboardAccelerator acc)) return;
            acc.Invoked -= KeyboardAccelerator_Invoked;
            EditorCanvas.KeyboardAccelerators.Remove(acc);
        }
        private bool TryFindAccelerator(VirtualKey k, VirtualKeyModifiers m, out KeyboardAccelerator acc)
        {
            foreach (var ka in EditorCanvas.KeyboardAccelerators)
            {
                if (ka.Key != k || ka.Modifiers != m) continue;
                acc = ka;
                return true;
            }
            acc = null;
            return false;
        }
        #endregion
        #endregion

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

            ContactsOnVoid, 
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
            index = -1;
            var bestDelta = 500.0f;
            var N = _targetPoints.Count;
            for (int i = 0; i < N; i++)
            {
                var delta = (rawPoint - _targetPoints[i]).LengthSquared();
                if (delta > 200 || delta > bestDelta) continue;
                bestDelta = delta;
                index = i;
            }
            return !(index < 0);
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
                SetSizeSliders();

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
                SetEventAction(PointerEvent.Begin, TargetPointerDown);
                SetEventAction(PointerEvent.Hover, TargetPointerHover);

                PickerShape = null;
                PickerCanvas.Invalidate();
                EditorCanvas.Invalidate();
            }
        }

        #region Hover  ========================================================
        private void TargetPointerHover()
        {
            if (TryHitTest(RawPoint1, out int index))
            {
                _hoverPointIndex = index;
                SetEventAction(VirtualKey.Up, VirtualKeyModifiers.None, AccessKey_Up);
                SetEventAction(VirtualKey.Down, VirtualKeyModifiers.None, AccessKey_Down);
                SetEventAction(VirtualKey.Left, VirtualKeyModifiers.None, AccessKey_Left);
                SetEventAction(VirtualKey.Right, VirtualKeyModifiers.None, AccessKey_Right);
            }
            else
            {
                ClearEventAction(VirtualKey.Up, VirtualKeyModifiers.None);
                ClearEventAction(VirtualKey.Down, VirtualKeyModifiers.None);
                ClearEventAction(VirtualKey.Left, VirtualKeyModifiers.None);
                ClearEventAction(VirtualKey.Right, VirtualKeyModifiers.None);
            }
        }
        int _hoverPointIndex;

        const float KD = 1 / 127f;
        private void AccessKey_Up()
        {
            var ds = new Vector2(0, -KD);
            if (_hoverPointIndex == 0)
                Shape.MoveCenter(SelectedShapes, ds);
            else
                _polylineTarget.MovePoint(_hoverPointIndex - 1, ds);
        }
        private void AccessKey_Down()
        {
            var ds = new Vector2(0, KD);
            if (_hoverPointIndex == 0)
                Shape.MoveCenter(SelectedShapes, ds);
            else
                _polylineTarget.MovePoint(_hoverPointIndex - 1, ds);
        }
        private void AccessKey_Left()
        {
            var ds = new Vector2(-KD, 0);
            if (_hoverPointIndex == 0)
                Shape.MoveCenter(SelectedShapes, ds);
            else
                _polylineTarget.MovePoint(_hoverPointIndex - 1, ds);
        }
        private void AccessKey_Right()
        {
            var ds = new Vector2(KD, 0);
            if (_hoverPointIndex == 0)
                Shape.MoveCenter(SelectedShapes, ds);
            else
                _polylineTarget.MovePoint(_hoverPointIndex - 1, ds);
        }
        #endregion

        #region Drag  =========================================================
        private void TargetPointerDown()
        {
            if (TryHitTest(RawPoint1, out int index))
            {
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
            {
                var ip = NewLineSegmentPointIndex();
                if (ip.index > 0)
                {
                    if (_polylineTarget.TryAddPoint(ip.index - 1, ShapePoint(ip.hitPoint)))
                    {
                        _linePointIndex = ip.index;
                        SetEventAction(PointerEvent.Drag, DragLinePoint);
                    }
                }
                else
                    SetIdleOnVoid();
            }
        }
        int _linePointIndex;

        #region OnLineSegment  ================================================
        private (int index, Vector2 hitPoint) NewLineSegmentPointIndex()
        {
            var p = RawPoint1;
            if (_targetPoints.Count > 2)
            {
                (float X, float Y) p1 = (0, 0); // used for testing line segments
                (float X, float Y) p2 = (0, 0); // used for testing line segments

                var v = p;
                var E = new Extent((p.X, p.Y), 15);

                v = _targetPoints[1];
                p1 = (v.X, v.Y);

                for (int i = 2; i < _targetPoints.Count; i++)
                {
                    v = _targetPoints[i];
                    p2 = (v.X, v.Y);

                    var e = new Extent(p1, p2);
                    if (e.Intersects(E))
                    {
                        if (e.IsHorizontal)
                            return (i - 1, new Vector2(p.X, p2.Y));
                        else if (e.IsVertical)
                            return (i - 1, new Vector2(p1.X, p.Y));
                        else
                        {
                            var dx = (double)(p2.X - p1.X);
                            var dy = (double)(p2.Y - p1.Y);

                            int xi = (int)(p1.X + (dx * (p.Y - p1.Y)) / dy);
                            if (E.ContainsX(xi))
                                return (i - 1, new Vector2(xi, p.Y));

                            xi = (int)(p2.X + (dx * (p.Y - p2.Y)) / dy);
                            if (E.ContainsX(xi))
                                return (i - 1, new Vector2(xi, p.Y));

                            int yi = (int)(p1.Y + (dy * (p.X - p1.X)) / dx);
                            if (E.ContainsY(yi))
                                return (i - 1, new Vector2(p.X, yi));

                            yi = (int)(p2.Y + (dy * (p.X - p2.X)) / dx);
                            if (E.ContainsY(yi))
                                return (i - 1, new Vector2(p.X, yi));
                        }
                    }
                    p1 = p2;
                }
            }
            return (-1, Vector2.Zero);
        }
        #endregion

        private void DragCenterPoint()
        {
            Shape.MoveCenter(SelectedShapes, ShapeDelta);
            Shape.LockSliders(SelectedShapes, true);
            SetSizeSliders();

            EditorCanvas.Invalidate();
        }
        private void DragLinePoint()
        {
            _polylineTarget.MovePoint(_linePointIndex, ShapeDelta);
            Shape.LockSliders(SelectedShapes, true);
            SetSizeSliders();

            if (IsKillZone())
                TrySetNewCursor(CoreCursorType.UniversalNo);
            else
                RestorePointerCursor();

            EditorCanvas.Invalidate();
        }

        #region PointerCursor  ================================================
        private void RestorePointerCursor()
        {
            if (defaultPointerCursor is null) return;
            Window.Current.CoreWindow.PointerCursor = (CoreCursor)defaultPointerCursor;
            defaultPointerCursor = null;
        }
        private void TrySetNewCursor(CoreCursorType type)
        {
            if (Window.Current.CoreWindow.PointerCursor.Type != type)
            {
                if (defaultPointerCursor is null)
                    defaultPointerCursor = Window.Current.CoreWindow.PointerCursor;

                Window.Current.CoreWindow.PointerCursor = new CoreCursor(type, 0);
            }
        }
        private bool IsKillZone() => (RawPoint2.X < KILL_1 || RawPoint2.X > KILL_2 || RawPoint2.Y < KILL_1 || RawPoint2.Y > KILL_2);
        private object defaultPointerCursor;
        static float KILL_1 = EditMargin -4 ;
        static float KILL_2 = EditSize + (2 * EditMargin) - KILL_1;
        #endregion

        private void EndTargetDrag()
        {
            if (IsKillZone())
            {
                RestorePointerCursor();
                if (!_polylineTarget.TryDeletePoint(_linePointIndex))
                {

                    SymbolShapes.Remove(_polylineTarget);
                    _targetPoints.Clear();
                    SetIdleOnVoid();
                }
            }

            ClearEventAction(PointerEvent.Drag);
            EditorCanvas.Invalidate();
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
            SetSizeSliders();
            PickerCanvas.Invalidate();
            EditorCanvas.Invalidate();
        }
        private void ShapeSelectorMiss()
        {
            SelectedShapes.Clear();
            SetSizeSliders();
            SelectorCanvas.Invalidate();
            SetIdleOnVoid();
        }
        #endregion

        #endregion

        #region SetContactsOnVoid  ============================================
        private void SetContactsOnVoid()
        {
            if (SetEditorState(EditorState.ContactsOnVoid))
            {
                SetEventAction(PointerEvent.Begin, BeginContact);
                SetEventAction(PointerEvent.End, EndContact);
            }
        }
        private void EndContact()
        {
            ClearEventAction(PointerEvent.Drag);
        }

        #region BeginContact  =================================================
        private void BeginContact()
        {
            var index = -1;
            var bestDelta = 500.0f;
            var N = _targetPoints.Count;
            for (int i = 0; i < N; i++)
            {
                var delta = (RawPoint1 - _targetPoints[i]).LengthSquared();
                if (delta > 200 || delta > bestDelta) continue;
                bestDelta = delta;
                index = i;
            }
            if (index < 0) return;

            var (cont, targ, point, size) = _contactTargets[index];
            switch (targ)
            {
                case Target.N:
                    SetEventAction(PointerEvent.Drag, DragContact_N);
                    break;
                case Target.S:
                    SetEventAction(PointerEvent.Drag, DragContact_S);
                    break;
                case Target.E:
                    SetEventAction(PointerEvent.Drag, DragContact_E);
                    break;
                case Target.W:
                    SetEventAction(PointerEvent.Drag, DragContact_W);
                    break;
                case Target.NE:
                    SetEventAction(PointerEvent.Drag, DragContact_NE);
                    break;
                case Target.NW:
                    SetEventAction(PointerEvent.Drag, DragContact_NW);
                    break;
                case Target.SE:
                    SetEventAction(PointerEvent.Drag, DragContact_SE);
                    break;
                case Target.SW:
                    SetEventAction(PointerEvent.Drag, DragContact_SW);
                    break;
                case Target.EN:
                    SetEventAction(PointerEvent.Drag, DragContact_EN);
                    break;
                case Target.ES:
                    SetEventAction(PointerEvent.Drag, DragContact_ES);
                    break;
                case Target.WN:
                    SetEventAction(PointerEvent.Drag, DragContact_WN);
                    break;
                case Target.WS:
                    SetEventAction(PointerEvent.Drag, DragContact_WS);
                    break;
                case Target.NEC:
                    SetEventAction(PointerEvent.Drag, DragContact_NEC);
                    break;
                case Target.NWC:
                    SetEventAction(PointerEvent.Drag, DragContact_NWC);
                    break;
                case Target.SEC:
                    SetEventAction(PointerEvent.Drag, DragContact_SEC);
                    break;
                case Target.SWC:
                    SetEventAction(PointerEvent.Drag, DragContact_SWC);
                    break;
                case Target.Any:
                    SetEventAction(PointerEvent.Drag, DragContact_SWC);
                    break;
            }
        }
        #endregion

        #region DragContact_N  ================================================
        private void DragContact_NWC()
        {
            if (Target_Contacts.TryGetValue(Target.NWC, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.NWC);
                Target_Contacts[Target.NWC] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        private void DragContact_NW()
        {
            if (Target_Contacts.TryGetValue(Target.NW, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.NW);
                Target_Contacts[Target.NW] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        private void DragContact_N()
        {
            if (Target_Contacts.TryGetValue(Target.N, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.N);
                Target_Contacts[Target.N] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        private void DragContact_NE()
        {
            if (Target_Contacts.TryGetValue(Target.NE, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.NE);
                Target_Contacts[Target.NE] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        private void DragContact_NEC()
        {
            if (Target_Contacts.TryGetValue(Target.NEC, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var d = ShapePoint2.Y;
                if (d < -1f) d = -1f;
                if (d > -0.1f) d = -0.1f;
                var p = new Vector2(-d, d);

                Target_Contacts[Target.NEC] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        #endregion

        #region DragContact_E  ================================================
        private void DragContact_EN()
        {
            if (Target_Contacts.TryGetValue(Target.EN, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.EN);
                Target_Contacts[Target.EN] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        private void DragContact_E()
        {
            if (Target_Contacts.TryGetValue(Target.E, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.E);
                Target_Contacts[Target.E] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        private void DragContact_ES()
        {
            if (Target_Contacts.TryGetValue(Target.ES, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.ES);
                Target_Contacts[Target.ES] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        #endregion

        #region DragContact_W  ================================================
        private void DragContact_WN()
        {
            if (Target_Contacts.TryGetValue(Target.WN, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.WN);
                Target_Contacts[Target.WN] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        private void DragContact_W()
        {
            if (Target_Contacts.TryGetValue(Target.W, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.W);
                Target_Contacts[Target.W] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        private void DragContact_WS()
        {
            if (Target_Contacts.TryGetValue(Target.WS, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.WS);
                Target_Contacts[Target.WS] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        #endregion

        #region DragContact_S  ================================================
        private void DragContact_SWC()
        {
            if (Target_Contacts.TryGetValue(Target.SWC, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.SWC);
                Target_Contacts[Target.SWC] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        private void DragContact_SW()
        {
            if (Target_Contacts.TryGetValue(Target.SW, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.SW);
                Target_Contacts[Target.SW] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        private void DragContact_S()
        {
            if (Target_Contacts.TryGetValue(Target.S, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.S);
                Target_Contacts[Target.S] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        private void DragContact_SE()
        {
            if (Target_Contacts.TryGetValue(Target.SE, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.SE);
                Target_Contacts[Target.SE] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        private void DragContact_SEC()
        {
            if (Target_Contacts.TryGetValue(Target.SEC, out (Contact con, (sbyte dx, sbyte dy) pnt, byte siz) val))
            {
                var p = ClampPoint(ShapePoint2, TargetIndex.SEC);
                Target_Contacts[Target.SEC] = (val.con, Shape.ToSByte(p), val.siz);
            }
            EditorCanvas.Invalidate();
        }
        #endregion

        #region ClampPoint  ===================================================
        private static Vector2 ClampPoint (Vector2 p, TargetIndex tix)
        {
            var (x, y) = (p.X, p.Y);
            var (x1, x2, y1, y2) = _targetLimits[(int)tix];
            if (x < x1) x = x1;
            if (x > x2) x = x2;
            if (y < y1) y = y1;
            if (y > y2) y = y2;
            return new Vector2(x, y);
        }
        static (float x1, float x2, float y1, float y2)[] _targetLimits =
        {
            (0, 1, -1, 0),      //EN  - [0]
            (0, 1, -.5f, .5f),  //E   - [1]
            (0, 1, 0, 1),       //ES  - [2]

            (0, 1, 0, 1),       //SEC - [3]
            (0, 1, 0, 1),       //SE  - [4]
            (-.5f, .5f, 0, 1),  //S   - [5]
            (-1, 0, 0, 1),      //SW  - [6]
            (-1, 0, 0, 1),      //SWC - [7]

            (-1, 0, 0, 1),      //WS  - [8]
            (-1, 0, -.5f, .5f), //W   - [9]
            (-1, 0, -1, 0),     //WN  - [10]

            (-1, 0, -1, 0),     //NWC - [11]
            (-1, 0, 0, 1),      //NW  - [12]
            (-.5f, .5f, -1, 0), //N   - [13]
            (0, 1, -1, 0),      //NE  - [14]
            (0, 1, -1, 0),      //NEC - [15]
        };
        #endregion

        #endregion
    }
}
