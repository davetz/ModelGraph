
namespace ModelGraphSTD
{/*
    interface between ModelGraphSTD --> ModelGraph

    Isolate the ModelGraphSTD from the UI plumbing.
 */
    public interface IPageControl
    {
        void Dispatch(UIRequest request);
    }
}
