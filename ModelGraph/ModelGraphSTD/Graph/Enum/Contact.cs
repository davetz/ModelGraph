namespace ModelGraphSTD
{
    /// <summary>
    /// Allowed connections on the side (top,left,right,bottom) of a symbol
    /// </summary>
    public enum Contact : byte
    {
        Any = 0, // allow any number of contacts
        One = 1, // allow exactly one contact
        None = 2, // absolutely no connections allowed 
    }
}
