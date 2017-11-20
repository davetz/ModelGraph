namespace ModelGraph.Internals
{/*
 */
    public interface IModelControl
    {
        ControlType ControlType { get; }
        void SetSize(double width, double height);
        void Refresh(); 
        void Close();
    }
}
