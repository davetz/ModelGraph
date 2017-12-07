namespace ModelGraphSTD
{/*
    ControlType used to communicate which type of control to create.
    ModelGraphSTD --> IPage --> ModelGragph
 */
    /// <summary>
    ///  ControlType used to communicate which type of control to create.
    /// </summary>
    public enum ControlType
    {
        AppRootChef,    // initial UI control of the ModelGraph App
        PrimaryTree,    // full model hierarchy tree control
        PartialTree,    // partial model hierarchy tree control
        SymbolEditor,   // graphical editor for graph node symbols
        GraphDisplay,   // graphical interface for relational graphs
    }
}
