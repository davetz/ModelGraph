namespace ModelGraphSTD
{
    /// <summary>
    /// Allowed connections on the side (top,left,right,bottom) of a symbol
    /// </summary>
    public enum Contact : byte
    {
        Any = 0, // allow any number of contacts on this symbol face
        One = 1, // allow exactly one contact on this symbol face
        None = 3, // absolutely no connections are allowed on this symbol face
    }
}
