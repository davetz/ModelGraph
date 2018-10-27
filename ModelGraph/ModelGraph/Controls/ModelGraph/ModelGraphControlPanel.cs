using ModelGraph.Helpers;
using ModelGraphSTD;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ModelGraph.Controls
{
    public sealed partial class ModelGraphControl
    {
        GraphControlPanel _graphControlPanel;
        bool _isActionPinned;
        FlipRotate _flipRotate;

        private void InitializeControlPanel()
        {
            _graphControlPanel = new GraphControlPanel();

            _graphControlPanel.UndoAction = TryUndo;
            _graphControlPanel.RedoAction = TryRedo;

            _controlPannel.Children.Add(_graphControlPanel);
        }

        internal void SetFlipRotate(FlipRotate value)
        {
            _flipRotate = value;
        }

        private void ReleaseControlPanel()
        {
        }

        private void UpdateUndoRedoControls()
        {
            var (canUndo, canRedo, undoCount, redoCount) = _graph.UndoRedoParms;
            _graphControlPanel.UpdateUndoRedo(canUndo, canRedo, undoCount, redoCount);
        }
    }
}
