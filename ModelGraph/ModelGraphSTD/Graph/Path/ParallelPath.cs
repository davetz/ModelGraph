using System.Linq;

namespace ModelGraphSTD
{/*
 */
    public class ParallelPath : Path
    {
        internal Path[] Paths;

        #region Constructor  ==================================================
        internal ParallelPath(Graph owner, Path[] paths, bool isRadial = false)
        {
            Owner = owner;
            Trait = Trait.ParallelPath;
            IsRadial = isRadial;

            Paths = paths;

            owner.Add(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal override int Count => 2;
        internal override Item[] Items => Paths.ToArray();

        internal override Query Query { get { return Paths[0].Query; } }
        internal override Item Head { get { return IsReversed ? Paths[Paths.Length - 1].Tail : Paths[0].Head; } }
        internal override Item Tail { get { return IsReversed ? Paths[0].Head : Paths[Paths.Length - 1].Tail; } }

        internal override double Height => 2;
        internal override double Width => 2;
        #endregion
    }
}
