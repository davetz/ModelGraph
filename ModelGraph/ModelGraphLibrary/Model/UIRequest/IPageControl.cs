using System;

namespace ModelGraphLibrary
{/*
    interface between ModelGraphLibrary --> ModelGraph

    Isolate the ModelGraphLibrary from the UI plumbing.
 */
    public interface IPageControl
    {
        void Dispatch(UIRequest request);
    }
}
