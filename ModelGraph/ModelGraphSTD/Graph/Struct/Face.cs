namespace ModelGraphSTD
{
    public struct Face
    {
        internal (short X, short Y) Delta1; //surface point1
        internal (short X, short Y) Delta2; //surface point2
        internal (short X, short Y) Delta3; //terminal point
        internal Facet Facet;

        internal Face(Facet facet)
        {
            Facet = facet;
            Delta1 = Delta2 = Delta3 = (0, 0);
        }
        internal Face(Face face, Facet facet)
        {
            Facet = ((facet & Facet.Forced) != 0 || face.Facet == Facet.None) ? facet : face.Facet;
            Delta1 = face.Delta1;
            Delta2 = face.Delta2;
            Delta3 = face.Delta3;
        }
        internal Face(Facet facet, (short, short) delta1)
        {
            Facet = facet;
            Delta1 = Delta2 = Delta3 = delta1;
        }
        internal Face(Facet facet, (short, short) delta1, (short, short) delta3)
        {
            Facet = facet;
            Delta1 = Delta2 = delta1;
            Delta3 = delta3;
        }
        internal Face(Facet facet, (short, short) delta1, (short, short) delta2, (short, short) delta3 )
        {
            Facet = facet;
            Delta1 = delta1;
            Delta2 = delta2;
            Delta3 = delta3;
        }
    }
}
