
using System.Collections.Generic;

namespace ModelGraphLibrary
{/*
 */
    public class Level : Item
    {
        internal List<Path> Paths = new List<Path>();

        #region Constructor  ==================================================
        internal Level(Graph owner)
        {
            Owner = owner;
            Trait = Trait.Level;

            owner.Add(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal int Count => Paths.Count;
        internal void Add(Path path) { Paths.Add(path); }
        internal Graph Graph { get { return Owner as Graph; } }
        internal string Name => Graph.Levels.IndexOf(this).ToString();
        #endregion
    }
}
