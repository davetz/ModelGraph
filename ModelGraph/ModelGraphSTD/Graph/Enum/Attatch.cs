namespace ModelGraphSTD
{
    public enum Attach : byte
    {/*
        The edge connection style.

        An edge may connect a symbol to another symbol,
        to a rectangular bus bar node, or to a point node. 

     */
        Normal = 0, // none of the attachment styles listed below
        Radial = 1, // attach the edge radial to the symbol's center
        RightAngle = 2, // attach at 90 degrees to a point node's center
        SkewedAngle = 3, // attach at a skewed angle to a point node's center
    }
}
