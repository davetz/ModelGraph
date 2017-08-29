namespace ModelGraphLibrary
{/*
    IndexPair is used with relations
 */
    internal struct IndexPair
    {
        internal int Parent;
        internal int Child;

        internal IndexPair(int parent, int child)
        {
            Parent = parent;
            Child = child;
        }

        internal bool IsValid => !IsInvalid;
        internal bool IsInvalid => (Parent < 0 || Child < 0);
    }
}
