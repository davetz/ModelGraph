using System;

namespace ModelGraphSTD
{/*
 */
    public class SeriesPath : Path
    {
        internal Path[] Paths;

        #region Constructor  ==================================================
        internal SeriesPath(Graph owner, Path[] paths, bool isRadial = false)
        {
            Owner = owner;
            Trait = Trait.SeriesPath;

            Paths = paths;
            IsRadial = isRadial;

            owner.Add(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal override Item Head => (IsReversed ? Paths[Paths.Length - 1].Tail : Paths[0].Head);
        internal override Item Tail => (IsReversed ? Paths[0].Head : Paths[Paths.Length - 1].Tail);
        internal override Query Query => Paths[0].Query;
        internal override double Width => 4; 
        internal override double Height => 4;

        internal override int Count => Paths.Length;
        internal override Item[] Items => Paths;
        #endregion
    }
}
