namespace ModelGraph.Internals
{/*
    interface between ModelGraph.Internals --> ModelGraph

    Isolate the ModelGraph.Internals from the UI plumbing.
 */
    public interface IPageControl
    {
        void Dispatch(UIRequest request);
    }
}
