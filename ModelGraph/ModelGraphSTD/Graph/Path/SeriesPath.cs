using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public class SeriesPath : Path
    {
        #region Constructor  ==================================================
        internal SeriesPath(Graph owner, List<Path> paths, bool isRadial = false)
            : base(owner, Trait.SeriesPath, paths)
        {
            IsRadial = isRadial;
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal override Item Head => (IsReversed ? Paths[Paths.Count - 1].Tail : Paths[0].Head);
        internal override Item Tail => (IsReversed ? Paths[0].Head : Paths[Paths.Count - 1].Tail);
        internal override Query Query => Paths[0].Query;
        internal override double Width => 4; 
        internal override double Height => 4;
        #endregion
    }
}
