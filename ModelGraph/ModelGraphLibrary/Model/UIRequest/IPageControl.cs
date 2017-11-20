namespace ModelGraph.Internals
{/*
    Control interface between ModelGraph.Internals --> ModelGraph App

    Isolate the ModelGraphLibray from the UI plumbing.
 */
    public interface IPageControl
    {
        void Dispatch(UIRequest request);
    }
}
