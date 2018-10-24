using ModelGraph.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ModelGraph.Controls
{
    public sealed partial class ModelGraphControl
    {
        Button _undoButton;
        Button _redoButton;

        TextBlock _undoCount;
        TextBlock _redoCount;

        const string _redo = "\ue7a6";
        const string _undo = "\ue7a7";

        string _undoTip = "004S".GetLocalized();
        string _redoTip = "005S".GetLocalized();


        private void InitializeControlPanel()
        {
            var undoRedoButtonStyle = Resources["UndoRedoButtonStyle"] as Style;
            var undoRedoCountStyle = Resources["UndoRedoCountStyle"] as Style;

            _undoButton = new Button();
            _undoButton.Content = _undo;
            _undoButton.Style = undoRedoButtonStyle;
            _undoButton.Background = _controlPannel.Background;
            _undoButton.Click += UndoButton_Click;
            _controlPannel.Children.Add(_undoButton);

            _undoCount = new TextBlock();
            _undoCount.Style = undoRedoCountStyle;
            _controlPannel.Children.Add(_undoCount);

            _redoButton = new Button();
            _redoButton.Content = _redo;
            _redoButton.Style = undoRedoButtonStyle;
            _redoButton.Background = _controlPannel.Background;
            _redoButton.Click += RedoButton_Click;
            _controlPannel.Children.Add(_redoButton);

            _redoCount = new TextBlock();
            _redoCount.Style = undoRedoCountStyle;
            _controlPannel.Children.Add(_redoCount);

            ToolTipService.SetToolTip(_undoButton, _undoTip);
            ToolTipService.SetToolTip(_redoButton, _redoTip);
        }

        private void UndoButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TryUndo();
        }
        private void RedoButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TryRedo();
        }

        private void UpdateUndoRedoControls()
        {
            var (canUndo, canRedo, undoCount, redoCount) = _graph.UndoRedoParms;

            _undoButton.IsEnabled = canUndo;
            _redoButton.IsEnabled = canRedo;

            _undoCount.Text = undoCount.ToString();
            _redoCount.Text = redoCount.ToString();
        }
    }
}
