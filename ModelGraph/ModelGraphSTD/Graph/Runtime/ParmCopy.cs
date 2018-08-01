using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public class ParmCopy
    {
        internal List<NodeCopy> NodeCopies;
        internal List<EdgeCopy> EdgeCopies;

        internal ParmCopy(List<NodeCopy> nodeCopies, List<EdgeCopy> edgeCopies)
        {
            NodeCopies = nodeCopies;
            EdgeCopies = edgeCopies;
        }

        internal void Restore()
        {
            foreach (var copy in NodeCopies) { copy.Restore(); }
            foreach (var copy in EdgeCopies) { copy.Restore(); }
        }

        // we need to copy the current values before we restore the saved ones
        internal ParmCopy GetCurrent()
        {
            var nodeCopies = new List<NodeCopy>(NodeCopies.Count);
            var edgeCopies = new List<EdgeCopy>(EdgeCopies.Count);
            foreach (var copy in NodeCopies) { nodeCopies.Add(copy); }
            foreach (var copy in EdgeCopies) { edgeCopies.Add(copy); }
            return new ParmCopy(nodeCopies, edgeCopies);
        }
    }
}
