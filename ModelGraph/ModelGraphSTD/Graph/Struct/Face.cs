namespace ModelGraphSTD
{
    public struct Face
    {
        internal Side Side; 
        internal byte Index;
        internal byte Count;
        internal Facet Facet;
        internal Attach Attach;

        internal Face(PathEnd pathEnd)
        {
            Side = Side.Any;
            Index = 1;
            Count = 1;
            Facet = pathEnd.Facet;
            Attach = pathEnd.Attach;
        }
        internal Face(Face face, PathEnd pathEnd)
        {
            Side = face.Side;
            Index = face.Index;
            Count = face.Count;
            Facet = pathEnd.Facet;
            Attach = pathEnd.Attach;
        }
        //=====================================================================
        // The index is 0 based, the totalCount is 1's based 
        // for examle:    0,  1,  2,  3,  4 with the totalCount = 5
        //    _offset:   -4, -2,  0,  2,  4  
        //              . . . . . | . . . . . 
        //  or examle:  0,  1,  2,  3,  4,  5 with the totalCount = 6
        //    _offset: -5, -3, -1,  1,  3,  5
        //
        // The diference between succesive offset values is always 2
        internal int Offset { get { return 2 * Index - (Count - 1); } }
    }
}
