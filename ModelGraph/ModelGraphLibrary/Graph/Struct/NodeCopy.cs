namespace ModelGraph.Internals
{
    // facilitate undo redo by recording the state of node core before and after
    // modifications to position and orientation
    internal struct NodeCopy
    {
        internal Node Node;
        internal NodeX Core;

        internal NodeCopy(Node node)
        {
            Node = node;
            Core = node.Core;
        }
        internal void Restore()
        {
            Node.Core = Core;
        }
    }
}
