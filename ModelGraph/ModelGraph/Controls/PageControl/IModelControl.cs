using ModelGraph.Internals;

namespace ModelGraph
{
    public interface IModelControl
    {
        ControlType ControlType { get; }
        void SetSize(double width, double height);
        void Refresh();
        void Close();
    }
}
