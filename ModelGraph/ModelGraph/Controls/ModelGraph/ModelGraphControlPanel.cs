using ModelGraph.Helpers;
using ModelGraphSTD;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ModelGraph.Controls
{
    public sealed partial class ModelGraphControl
    {

        #region InitializeControlPanel  =======================================
        private void InitializeControlPanel()
        {
        }
        #endregion

        private void ReleaseControlPanel()
        {
        }

        private Action _menuAction;
        private void SetMenuAction(Button btn, MenuFlyoutItem itm, Action act)
        {
                ActionName.Text = itm.Text;
                ToolTipService.SetToolTip(ActionName, ToolTipService.GetToolTip(btn));

               _menuAction = act;
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

        #region Aligning  =====================================================

        private void AlignVertItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuAction(AlignButton, AlignVertItem, AlignVert);
        }
        private void AlignHorzItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuAction(AlignButton, AlignHorzItem, AlignHorz);
        }
        private void AlignWestItem_Click(object sender, RoutedEventArgs e)
        {
            //ActionName.Text = AlignWestItem.Text;
        }
        private void AlignEastItem_Click(object sender, RoutedEventArgs e)
        {
            //ActionName.Text = AlignEastItem.Text;
        }
        private void AlignNorthItem_Click(object sender, RoutedEventArgs e)
        {
            //ActionName.Text = AlignNorthItem.Text;
        }
        private void AlignSouthItem_Click(object sender, RoutedEventArgs e)
        {
            //ActionName.Text = AlignSouthItem.Text;
        }
        #endregion

        #region Flipping  =====================================================

        private void FlipVertItem_Click(object sender, RoutedEventArgs e)
        {
            EnableFlipVert();
        }

        private void FlipHorzItem_Click(object sender, RoutedEventArgs e)
        {
            EnableFlipHorz();
        }
        void EnableFlipVert()
        {
            ActionName.Text = FlipVertItem.Text;
        }
        void EnableFlipHorz()
        {
            ActionName.Text = FlipHorzItem.Text;
        }
        #endregion

        #region Rotate  =======================================================
        private void RotateLeftItem_Click(object sender, RoutedEventArgs e)
        {
            EnableRotateLeft();
        }

        private void RotateRightItem_Click(object sender, RoutedEventArgs e)
        {
            EnableRotateRight();
        }
        private void EnableRotateLeft()
        {
            ActionName.Text = RotateLeftItem.Text;
        }
        private void EnableRotateRight()
        {
            ActionName.Text = RotateRightItem.Text;
        }

        #endregion

        #region Gravity  ======================================================

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
