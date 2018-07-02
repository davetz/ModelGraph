using System.Collections.Generic;

namespace ModelGraphSTD
{
    public class FlaredPath : Path
    {
        #region Constructor  ==================================================
        internal FlaredPath(Graph owner, List<Path> paths) 
            : base(owner, Trait.FlaredPath, paths)
        {
            IsRadial = true;
        }
        #endregion

        #region Properties/Methods  ===========================================
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
