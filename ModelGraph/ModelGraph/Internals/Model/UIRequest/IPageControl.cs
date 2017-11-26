namespace ModelGraph.Internals
{/*
    Control interface between ModelGraph.Internals --> ModelGraph App

    Isolate the ModelGraph.Internals from the UI plumbing.
 */
    public interface IPageControl
    {
        void Dispatch(UIRequest request);
    }
}
