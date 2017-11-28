using ModelGraph.Internals;

namespace ModelGraph
{
    public interface IModelControl
    {
        void SetSize(double width, double height);
        void Refresh();
        void Close();
    }
}
