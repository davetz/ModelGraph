namespace ModelGraphLibrary
{/*
    Control interface between ModelGraphLibrary --> ModelGraph App

    Isolate the ModelGraphLibray from the UI plumbing.
 */
    public interface IPageControl
    {
        void Dispatch(UIRequest request);
    }
}
