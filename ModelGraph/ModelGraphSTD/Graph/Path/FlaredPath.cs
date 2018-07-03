using System.Collections.Generic;

namespace ModelGraphSTD
{
    public class FlaredPath : Path
    {
        readonly Path[] _paths;
        internal override Path[] Paths => _paths; 

        #region Constructor  ==================================================
        internal FlaredPath(Graph owner, Path[] paths) 
        {
            Owner = owner;
            Trait = Trait.FlaredPath;
            IsRadial = true;
            _paths = paths;

            owner.Add(this);
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
