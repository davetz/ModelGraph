using ModelGraph.Internals;
using Windows.Foundation;

namespace ModelGraph
{
    public interface IModelControl
    {
        Size PreferredMinSize { get; }
        void SetSize(double width, double height);
        void Refresh();
        void Close();
    }
}
