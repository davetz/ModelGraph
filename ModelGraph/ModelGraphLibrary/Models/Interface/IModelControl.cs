namespace ModelGraphLibrary
{/*
    Control Interface between ModelGraphLibrary --> ModelGraph Control

    Issolate the ModelGraphLibray from the actual UI control plumbing.
 */
    /// <summary>
    /// Issolate the UI control pumbing from ModelGraphLibrary 
    /// </summary>
    public interface IModelControl
    {
        void SetSize(double width, double height); // only used between UI controls
        void Refresh();
        void Close();
    }
}
