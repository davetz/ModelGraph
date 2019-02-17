using ModelGraphSTD;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ModelGraph.Controls
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

            var cp = e.GetCurrentPoint(CanvasGrid);

            if (cp.Properties.IsLeftButtonPressed && _eventAction.TryGetValue(EventId.Begin1, out Action begin1))
                begin1?.Invoke();
            else if (cp.Properties.IsRightButtonPressed && _eventAction.TryGetValue(EventId.Begin3, out Action begin3))
                begin3.Invoke();

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


            if (_isPointerPressed && _eventAction.TryGetValue(EventId.Drag, out Action drag))
            {
                drag?.Invoke();
                if (_enableHitTest)
                {
                    HitTest(_drawRef.Point2);
                }

                EditorCanvas.Invalidate();
            }
            else if (_eventAction.TryGetValue(EventId.Hover, out Action hover))
            {
                HitTest(_drawRef.Point2);
                if (_selector.IsChanged)
                {
                    Debug.WriteLine($"- - - - - - - - - Hit { _selector.HitLocation}");
                    hover?.Invoke();
                }
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            e.Handled = true;

            _isPointerPressed = false;
            _rootRef.Point2 = _rootDelta.Point2 = GridPoint(e);
            _drawRef.Point2 = _drawDelta.Point2 = DrawPoint(e);

            if (_eventAction.TryGetValue(EventId.End, out Action end))
            {
                HitTest(_drawRef.Point2);
                end?.Invoke();
            }
        }

        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            e.Handled = true;

            var cp = e.GetCurrentPoint(CanvasGrid);

            _wheelDelta = cp.Properties.MouseWheelDelta;
            if(_eventAction.TryGetValue(EventId.Wheel, out Action wheel)) wheel?.Invoke();
        }
        private (float X, float Y) GridPoint(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(CanvasGrid).Position;
            return ((float)p.X, (float)p.Y);
        }
        private (float X, float Y) DrawPoint(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(EditorCanvas).Position;
            var x = (p.X - _offset.X) / _zoomFactor;
            var y = (p.Y - _offset.Y) / _zoomFactor;
            return ((float)x, (float)y);
        }
        #endregion

        #region KeyboardEvents  ===============================================

        private void RootButton_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            //_ignorePointerMoved = true;
            //e.Handled = true;
            //switch (e.Key)
            //{
            //    case VirtualKey.LeftWindows:
            //    case VirtualKey.RightWindows:
            //    case VirtualKey.LeftMenu:
            //    case VirtualKey.LeftShift:
            //    case VirtualKey.LeftControl:
            //    case VirtualKey.RightMenu:
            //    case VirtualKey.RightShift:
            //    case VirtualKey.RightControl: break;
            //    case VirtualKey.Menu: _modifier |= Modifier.Menu; break;
            //    case VirtualKey.Shift: _modifier |= Modifier.Shift; break;
            //    case VirtualKey.Control: _modifier |= Modifier.Ctrl; break;
            //    case VirtualKey.Enter: ExecuteAction?.Invoke(); break;
            //    case VirtualKey.Escape: CancelAction?.Invoke(); break;
            //    case VirtualKey.Home: ZoomToExtent(_graph.Extent); break;
            //    case VirtualKey.Z: if (_modifier == Modifier.Ctrl) { TryUndo(); } break;
            //    case VirtualKey.Y: if (_modifier == Modifier.Ctrl) { TryRedo(); } break;
            //    default: _keyName = e.Key.ToString(); ShortCutAction?.Invoke(); break;
            //}
        }
        private char _prevKey;

        private void RootCanvas_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
        }
        private void KeyboardAccelerator_A_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            _prevKey = 'A';
        }
        private void KeyboardAccelerator_F_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            _prevKey = 'F';
        }
        private void KeyboardAccelerator_R_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            if (_prevKey == 'R') EnableRotateRight();
            _prevKey = 'R';
        }
        private void KeyboardAccelerator_G_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }
        private void KeyboardAccelerator_M_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }
        private void KeyboardAccelerator_L_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            if (_prevKey == 'R') EnableRotateLeft();
            _prevKey = 'L';
        }
        private void KeyboardAccelerator_U_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }
        private void KeyboardAccelerator_C_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }
        private void KeyboardAccelerator_X_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }
        private void KeyboardAccelerator_P_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }
        private void KeyboardAccelerator_D_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }
        private void KeyboardAccelerator_V_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            if (_prevKey == 'A') EnableAlignVert();
            else if (_prevKey == 'F') EnableFlipVert();
            _prevKey = 'V';
        }
        private void KeyboardAccelerator_H_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            if (_prevKey == 'A') EnableAlignHorz();
            else if (_prevKey == 'F') EnableFlipHorz();
            _prevKey = 'H';
        }
        private void KeyboardAccelerator_Up_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            _arrowDelta = (0, -1);
            if (_eventAction.TryGetValue(EventId.Arrow, out Action arrow)) arrow?.Invoke();
        }
        private void KeyboardAccelerator_Left_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            _arrowDelta = (-1, 0);
            if (_eventAction.TryGetValue(EventId.Arrow, out Action arrow)) arrow?.Invoke();
        }
        private void KeyboardAccelerator_Down_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            _arrowDelta = (0, 1);
            if (_eventAction.TryGetValue(EventId.Arrow, out Action arrow)) arrow?.Invoke();
        }
        private void KeyboardAccelerator_Right_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            _arrowDelta = (1, 0);
            if (_eventAction.TryGetValue(EventId.Arrow, out Action arrow)) arrow?.Invoke();
        }
        private void KeyboardAccelerator_Enter_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }
        private void KeyboardAccelerator_Home_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }
        private void KeyboardAccelerator_Escape_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            _prevKey = ' ';
            UpdateActionPinned(false);
            if (_eventAction.TryGetValue(EventId.Cancel, out Action cancel)) cancel?.Invoke();
        }
        private void KeyboardAccelerator_Delete_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }
        private void KeyboardAccelerator_Cut_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }
        private void KeyboardAccelerator_Copy_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }
        private void KeyboardAccelerator_Paste_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
        }

        private async void TryUndo()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { _graph.TryUndo(); _graph.AdjustGraph(); });
            PostRefresh();
        }
        private async void TryRedo()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { _graph.TryRedo(); _graph.AdjustGraph(); });
            PostRefresh();
        }
        private void RootButton_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            _modifier = Modifier.None;
        }
        #endregion
    }
}
