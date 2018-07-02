using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public class ParallelPath : Path
    {
        #region Constructor  ==================================================
        internal ParallelPath(Graph owner, List<Path> paths, bool isRadial = false)
            : base(owner, Trait.ParallelPath, paths)
        {
            IsRadial = isRadial;
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal override Query Query { get { return Paths[0].Query; } }
        internal override Item Head { get { return IsReversed ? Paths[Paths.Count - 1].Tail : Paths[0].Head; } }
        internal override Item Tail { get { return IsReversed ? Paths[0].Head : Paths[Paths.Count - 1].Tail; } }

        internal override double Height => 2;
        internal override double Width => 2;
        #endregion
    }
}
