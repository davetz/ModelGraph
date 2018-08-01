using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public struct ParmCopy
    {
        internal List<NodeParm> NodeParms;
        internal List<EdgeParm> EdgeParms;

        internal ParmCopy(List<NodeParm> nodeParms, List<EdgeParm> edgeParms)
        {
            NodeParms = nodeParms;
            EdgeParms = edgeParms;
        }

        internal void Restore()
        {
            foreach (var copy in NodeParms) { copy.Restore(); }
            foreach (var copy in EdgeParms) { copy.Restore(); }
        }

        // we need to copy the current values before we restore the saved ones
        internal ParmCopy GetCurrent() => new ParmCopy(new List<NodeParm>(NodeParms), new List<EdgeParm>(EdgeParms));
    }
}
