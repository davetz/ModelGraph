using System;
using System.Collections.Generic;

namespace ModelGraphLibrary
{/*
 */
    public class GraphX : StoreOf<Graph>
    {
        internal string Name;
        internal string Summary;
        internal string Description;
        internal byte WideBusSize = 20;
        internal byte ThinBusSize = 8;
        internal byte MinNodeSize = 4;
        internal byte TerminalLength = 8;
        internal byte TerminalSpacing = 8;
        internal byte TerminalSkew = 20;

        #region Constructors  =================================================
        internal GraphX(Store owner)
        {
            Owner = owner;
            Trait = Trait.GraphX;
            Guid = Guid.NewGuid();
            AutoExpandRight = true;

            owner.Add(this);
        }
        internal GraphX(Store owner, Guid guid)
        {
            Owner = owner;
            Trait = Trait.GraphX;
            Guid = guid;

            owner.Add(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal bool TryGetGraph(Item root, out Graph graph)
        {
            if (Count < 0)
            {
                foreach (var item in Items)
                {
                    var g = item as Graph;
                    if (g.RootItem != root) continue;
                    graph = g;
                    return true;
                }
            }
            graph = null;
            return false;
        }
        #endregion
    }
}
