using ModelGraph.Helpers;
using ModelGraphSTD;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ModelGraph.Controls
{
    public sealed partial class GraphControlPanel : UserControl
    {
        public GraphControlPanel()
        {
            this.InitializeComponent();
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

        #region FlipRotate  ===================================================
        internal Action<FlipRotate> SetFlipRotatate;

        private void FlipVertical_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            SetFlipRotatate?.Invoke(FlipRotate.FlipVertical);
        }

        private void FlipHorizontal_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            SetFlipRotatate?.Invoke(FlipRotate.FlipHorizontal);
        }

        private void FlipBothWays_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            SetFlipRotatate?.Invoke(FlipRotate.FlipBothWays);
        }

        private void RotateClockwise_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            SetFlipRotatate?.Invoke(FlipRotate.RotateClockWise);
        }

        private void RotateFlipVertical_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            SetFlipRotatate?.Invoke(FlipRotate.RotateFlipVertical);
        }

        private void RotateFlipHorizontal_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            SetFlipRotatate?.Invoke(FlipRotate.RotateFlipHorizontal);
        }

        private void RotateFlipBothWays_Click(object sender, RoutedEventArgs e)
        {
            SetActionName(sender);
            SetFlipRotatate?.Invoke(FlipRotate.RotateFlipBothWays);
        }
        void SetActionName(Object obj)
        {
            var item = obj as MenuFlyoutItem;
            ActionName.Text = item.Name;
        }
        #endregion

        #region UndoRedo  =====================================================
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            UndoAction?.Invoke();
        }
        internal Action UndoAction;
        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            RedoAction?.Invoke();
        }
        internal Action RedoAction;

        internal void UpdateUndoRedo(bool enableUndo, bool enableRedo, int undoCount, int redoCount)
        {
            UndoButton.IsEnabled = enableUndo;
            RedoButton.IsEnabled = enableRedo;
            UndoCount.Text = undoCount.ToString();
            RedoCount.Text = redoCount.ToString();
        }

        #endregion
    }
}
