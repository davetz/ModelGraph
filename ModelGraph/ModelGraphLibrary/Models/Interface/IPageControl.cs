using Windows.UI.Core;
using Windows.UI.Xaml;

namespace ModelGraphLibrary
{/*
    Control Interface between ModelGraphLibrary --> ModelGraph App

    Issolate the ModelGraphLibray from the actual UI plumbing.
 */
    /// <summary>
    ///  Control Interface between ModelGraphLibrary --> ModelGraph App
    /// </summary>
    public interface IPageControl
    {
        int ViewId { get; set; }
        int ModelCount { get; }
        RootModel[] GetModels();
        void TabItem_DragStarting(RootModel model);
        void TabItem_DragOver(RootModel model, DragEventArgs e);
        void TabItem_DragDrop(RootModel model, DragEventArgs e);
        void CreateView(ViewRequest viewRequest);
        void CloseModel(RootModel root);
        void ReloadModel(RootModel root);
        void CloseModelView(RootModel model);
        void LoadModelView(RootModel model);
        CoreDispatcher Dispatcher { get; }
    }
}
