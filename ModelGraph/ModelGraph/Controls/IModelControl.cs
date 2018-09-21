
namespace ModelGraph.Controls
{
    public interface IModelControl
    {
        (int Width, int Height) PreferredMinSize { get; }
        void SetSize(double width, double height);
        void Refresh();
        void Close();
    }
}
