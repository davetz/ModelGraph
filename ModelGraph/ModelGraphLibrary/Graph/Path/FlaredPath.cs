using System;
using System.Linq;

namespace ModelGraph.Internals
{/*
 */
    public class FlaredPath : Path
    {
        internal Path[] Paths;

        #region Constructor  ==================================================
        internal FlaredPath(Graph owner, Path[] paths)
        {
            Owner = owner;
            Trait = Trait.FlaredPath;
            IsRadial = true;

            Paths = paths;

            owner.Add(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal override int Count => Paths.Length;
        internal override Item[] Items => Paths.ToArray();

        internal override Query Query { get { return Paths[0].Query; } }
        internal override Item Head { get { return IsReversed ? Paths[0].Tail : Paths[0].Head; } }
        internal override Item Tail { get { return IsReversed ? Paths[0].Head : Paths[0].Tail; } }

        internal override double Width { get { return GetWidth(); } }
        internal override double Height { get { return GetHeight(); } }

        private double GetWidth()
        {
            return 2;
        }
        private double GetHeight()
        {
            return 2;
        }
        #endregion
    }
}
