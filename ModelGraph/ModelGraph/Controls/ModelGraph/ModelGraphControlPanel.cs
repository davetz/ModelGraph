using ModelGraph.Helpers;
using ModelGraphSTD;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ModelGraph.Controls
{
    public sealed partial class ModelGraphControl
    {

        private void InitializeControlPanel()
        {
        }

        private void ReleaseControlPanel()
        {
        }


        #region ActionName  ===================================================
        const string _pin = "\ue718";
        const string _pinned = "\ue840";
        bool _isActionPinned;


        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateActionPinned(!_isActionPinned);
            SetIsPinned?.Invoke(_isActionPinned);
        }
        internal Action<bool> SetIsPinned;

        internal void ResetActionPinned()
        {
            UpdateActionPinned(false);
        }
        private void UpdateActionPinned(bool value)
        {
            if (value)
            {
                _isActionPinned = true;
                PinButton.Content = _pinned;
            }
            else
            {
                _isActionPinned = false;
                PinButton.Content = _pin;
                ActionName.Text = "";
            }
        }
        #endregion

        #region Flipping  =====================================================
        FlipRotate _flipRotate;

        private void CycleFlip()
        {
            if (_flipRotate == FlipRotate.FlipVertical)
            {
                _flipRotate = FlipRotate.FlipHorizontal;
            }
            else if (_flipRotate == FlipRotate.FlipHorizontal)
            {
                _flipRotate = FlipRotate.FlipBothWays;
            }
            else
            {
                _flipRotate = FlipRotate.FlipVertical;
            }
        }

        private void FlipVertical_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
           _flipRotate = FlipRotate.FlipVertical;
        }

        private void FlipHorizontal_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            _flipRotate = FlipRotate.FlipHorizontal;
        }

        private void FlipBothWays_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            _flipRotate = FlipRotate.FlipBothWays;
        }

        #endregion

        #region Rotate  =======================================================
        private void CycleRotate()
        {
            if (_flipRotate == FlipRotate.RotateClockWise)
            {
                _flipRotate = FlipRotate.FlipBothWays;
            }
            else if (_flipRotate == FlipRotate.FlipBothWays)
            {
                _flipRotate = FlipRotate.RotateFlipVertical;
            }
            else
            {
                _flipRotate = FlipRotate.RotateClockWise;
            }
        }
        private void RotateClockwise_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            _flipRotate = FlipRotate.RotateClockWise;
        }

        private void RotateFlipVertical_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            _flipRotate = FlipRotate.RotateFlipVertical;
        }

        private void RotateFlipHorizontal_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            _flipRotate = FlipRotate.RotateFlipHorizontal;
        }

        private void RotateFlipBothWays_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            _flipRotate = FlipRotate.RotateFlipBothWays;
        }
        void SetActionName(Object obj)
        {
            var item = obj as MenuFlyoutItem;
            ActionName.Text = item.Name;
        }

        #endregion
        #region UndoRedo  =====================================================
        private void UndoButton_Click(object sender, RoutedEventArgs e) => TryUndo();
        private void RedoButton_Click(object sender, RoutedEventArgs e) => TryRedo();

        private void UpdateUndoRedoControls()
        {
            var (canUndo, canRedo, undoCount, redoCount) = _graph.UndoRedoParms;

            UndoButton.IsEnabled = canUndo;
            RedoButton.IsEnabled = canRedo;
            UndoCount.Text = undoCount.ToString();
            RedoCount.Text = redoCount.ToString();
        }
        #endregion

    }
}
