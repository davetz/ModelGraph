namespace ModelGraphLibrary
{/*
 */
    public class ParmCopy
    {
        internal NodeCopy[] NodeCopies;
        internal EdgeCopy[] EdgeCopies;

        internal ParmCopy(NodeCopy[] nodeCopies, EdgeCopy[] edgeCopies)
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
            var nodeCopies = new NodeCopy[NodeCopies.Length];
            var edgeCopies = new EdgeCopy[EdgeCopies.Length];
            for (int i = 0; i < NodeCopies.Length; i++)
            {
                nodeCopies[i] = new NodeCopy(NodeCopies[i].Node);
            }
            for (int i = 0; i < EdgeCopies.Length; i++)
            {
                edgeCopies[i] = new EdgeCopy(EdgeCopies[i].Edge);
            }
            return new ParmCopy(nodeCopies, edgeCopies);
        }
    }
}
