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



        #region MenuAction  ===================================================
        const string _pin = "\ue718";
        const string _pinned = "\ue840";
        bool _isActionPinned;
        private Action _menuAction;

        private void SetMenuAction(Button btn, MenuFlyoutItem itm, Action act)
        {
            ActionHelp.Visibility = Visibility.Visible;
            ActionName.Text = itm.Text;
            ToolTipService.SetToolTip(ActionName, ToolTipService.GetToolTip(btn));

            _menuAction = act;
        }


        internal void TryEXecuteMenuAction()
        {
            if (_menuAction is null) return;
            _menuAction.Invoke();
            if (_isActionPinned) return;
            ClearMenuAction();
        }
        private void ClearMenuAction()
        {
            _menuAction = null;
            _isActionPinned = false;
            PinButton.Content = _pin;
            ActionName.Text = "";
            ActionHelp.Visibility = Visibility.Collapsed;
        }

        private void UpdateActionPinned(bool value)
        {
            if (value)
            {
                _isActionPinned = true;
                PinButton.Content = _pinned;
            }
            else
                ClearMenuAction();
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
            SetMenuAction(FlipButton, FlipVertItem, FlipVert);
        }

        private void FlipHorzItem_Click(object sender, RoutedEventArgs e)
        {
            SetMenuAction(FlipButton, FlipHorzItem, FlipHorz);
        }
        #endregion

        #region Rotate  =======================================================
        private void RotateLeft45Item_Click(object sender, RoutedEventArgs e)
        {
            SetMenuAction(RotateButton, RotateLeft45Item, RotateLeft45);
        }

        private void RotateRight45Item_Click(object sender, RoutedEventArgs e)
        {
            SetMenuAction(RotateButton, RotateRight45Item, RotateRight45);
        }
        private void RotateLeft90Item_Click(object sender, RoutedEventArgs e)
        {
            SetMenuAction(RotateButton, RotateLeft90Item, RotateLeft90);
        }

        private void RotateRight90Item_Click(object sender, RoutedEventArgs e)
        {
            SetMenuAction(RotateButton, RotateRight90Item, RotateRight90);
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
