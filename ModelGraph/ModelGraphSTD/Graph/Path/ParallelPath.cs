using System.Collections.Generic;

namespace ModelGraphSTD
{
    public class ParallelPath : Path
    {
        readonly Path[] _paths;
        internal override Path[] Paths => _paths;

        #region Constructor  ==================================================
        internal ParallelPath(Graph owner, Path[] paths, bool isRadial = false)
        {
            Owner = owner;
            Trait = Trait.ParallelPath;
            IsRadial = isRadial;
            _paths = paths;

            owner.Add(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal override Query Query { get { return Paths[0].Query; } }
        internal override Item Head { get { return IsReversed ? Paths[Last].Tail : Paths[0].Head; } }
        internal override Item Tail { get { return IsReversed ? Paths[0].Head : Paths[Last].Tail; } }

        internal override double Height => 2;
        internal override double Width => 2;
        #endregion
    }
}
