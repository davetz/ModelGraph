
using Windows.UI.Xaml.Controls;

namespace ModelGraph.Controls
{
    public interface IModelControl
    {
        (int Width, int Height) PreferredSize { get; }
        void Save();
        void Reload();
        void Release();
        void Refresh();
        void SetSize(double width, double height);
    }
}
