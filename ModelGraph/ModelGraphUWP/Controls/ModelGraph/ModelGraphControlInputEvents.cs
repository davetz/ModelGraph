﻿using ModelGraphSTD;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.System;
using System.Diagnostics;

namespace ModelGraphUWP
{
    public sealed partial class ModelGraphControl
    {

        #region PointerEvents  ================================================
        private bool _isPointerPressed;
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            e.Handled = true;

            _isPointerPressed = true;
            _rootRef = _rootDelta = new Extent(GridPoint(e));
            _drawRef = _drawDelta = _dragDelta = new Extent(DrawPoint(e));
            
            //HitTest(_drawRef.Point1);

            var cp = e.GetCurrentPoint(RootGrid);

            if (cp.Properties.IsLeftButtonPressed && Begin1Action != null) Begin1Action();
            else if (cp.Properties.IsRightButtonPressed && Begin3Action != null) Begin3Action();

            // somewhere, up the visual tree, there is a rogue scrollView that gets focus
            var obj = FocusManager.GetFocusedElement();
            if (obj is ScrollViewer)
            {
                var scv = obj as ScrollViewer;
                scv.IsTabStop = false;

                var ok = FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
                obj = FocusManager.GetFocusedElement();
            }
        }

        // whenever the canvas is panned we get a bougus mouse move event
        private bool _ignorePointerMoved;
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (_ignorePointerMoved) { _ignorePointerMoved = false; return; }

            _rootRef.Point2 = _rootDelta.Point2 = GridPoint(e);
            _drawRef.Point2 = _drawDelta.Point2 = DrawPoint(e);


            if (_isPointerPressed && DragAction != null)
            {
                DragAction?.Invoke();
                if (_enableHitTest) _selector.HitTest(_drawRef.Point2);
                DrawCanvas.Invalidate();
            }
            else if (HoverAction != null)
            {
                _selector.HitTest(_drawRef.Point2);
                if (_selector.IsChanged)
                {
                    Debug.WriteLine($"- - - - - - - - - Hit { _selector.HitLocation}");
                    HoverAction?.Invoke();
                }
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            e.Handled = true;

            _isPointerPressed = false;
            _rootRef.Point2 = _rootDelta.Point2 = GridPoint(e);
            _drawRef.Point2 = _drawDelta.Point2 = DrawPoint(e);

            if (EndAction != null)
            {
                _selector.HitTest(_drawRef.Point2);
                EndAction?.Invoke();
            }
        }

        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            e.Handled = true;

            var cp = e.GetCurrentPoint(RootGrid);

            _wheelDelta = cp.Properties.MouseWheelDelta;
            WheelAction?.Invoke();
        }
        private XYPoint GridPoint(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(RootGrid).Position;
            return new XYPoint(p.X, p.Y);
        }
        private XYPoint DrawPoint(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(DrawCanvas).Position;
            var x = _viewExtent.X1 + (p.X / _zoomFactor);
            var y = _viewExtent.Y1 + (p.Y / _zoomFactor);
            return new XYPoint(x, y);
        }
        #endregion

        #region KeyboardEvents  ===============================================
        private void RootButton_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ignorePointerMoved = true;
            e.Handled = true;

            switch (e.Key)
            {
                case VirtualKey.LeftWindows:
                case VirtualKey.RightWindows:
                case VirtualKey.LeftMenu:
                case VirtualKey.LeftShift:
                case VirtualKey.LeftControl:
                case VirtualKey.RightMenu:
                case VirtualKey.RightShift:
                case VirtualKey.RightControl: break;
                case VirtualKey.Menu: _modifier |= Modifier.Menu; break;
                case VirtualKey.Shift: _modifier |= Modifier.Shift; break;
                case VirtualKey.Control: _modifier |= Modifier.Ctrl; break;
                case VirtualKey.Escape: CancelAction?.Invoke(); break;
                case VirtualKey.Up: _arrowDelta = new XYPoint(0, -1); ArrowAction?.Invoke(); break;
                case VirtualKey.Down: _arrowDelta = new XYPoint(0, 1); ArrowAction?.Invoke(); break;
                case VirtualKey.Left: _arrowDelta = new XYPoint(-1, 0); ArrowAction?.Invoke(); break;
                case VirtualKey.Right: _arrowDelta = new XYPoint(1, 0); ArrowAction?.Invoke(); break;
                case VirtualKey.Home: ZoomToExtent(_graph.Extent); break;
                case VirtualKey.Z: if (_modifier == Modifier.Ctrl) { _graph.TryUndo(); _rootModel.PostModelRefresh();} break;
                case VirtualKey.Y: if (_modifier == Modifier.Ctrl) { _graph.TryRedo(); _rootModel.PostModelRefresh(); } break;
                default: _keyName = e.Key.ToString(); ShortCutAction?.Invoke(); break;
            }
        }

        private void RootCanvas_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ExecuteAction?.Invoke();
        }

        private void RootButton_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            _modifier = Modifier.None;
        }
        #endregion

    }
}
