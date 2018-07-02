using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
    Relation is the abstract base class for RelationOf<T1, T2>   
 */
    public abstract class Relation : Item // used by undo/redo changes and StoreOf<Relation> _relationStore
    {
        internal Guid Guid;
        internal Pairing Pairing;

        internal abstract bool HasChildLink(Item key);
        internal abstract bool HasParentLink(Item key);
        internal bool HasNoParent(Item key)
        {
            return !HasParentLink(key);
        }
        internal bool HasNoChildren(Item key)
        {
            return !HasChildLink(key);
        }

        #region RequiredMethods  ==============================================
        internal abstract bool IsValidParentChild(Item parentItem, Item childItem);
        internal abstract int ChildCount(Item key);
        internal abstract int ParentCount(Item key);
        internal abstract bool RelationExists(Item key, Item childItem);
        internal abstract void InsertLink(Item parentItem, Item childItem, int parentIndex, int childIndex);
        internal abstract (int ParentIndex, int ChildIndex) AppendLink(Item parentItem, Item childItem);
        internal abstract (int ParentIndex, int ChildIndex) GetIndex(Item parentItem, Item childItem);
        internal abstract void RemoveLink(Item parentItem, Item childItem);
        internal abstract void MoveChild(Item key, Item item, int index);
        internal abstract void MoveParent(Item key, Item item, int index);
        internal abstract (int Index1, int Index2) GetChildrenIndex(Item key, Item item1, Item item2);
        internal abstract (int Index1, int Index2) GetParentsIndex(Item key, Item item1, Item item2);
        internal abstract int GetLinks(out List<Item> parents, out List<Item> children);
        internal abstract int GetLinksCount();
        internal abstract void SetLink(Item key, Item val, int capacity = 0); // used by storage file load
        internal abstract bool TryGetParents(Item key, out List<Item> parents);
        internal abstract bool TryGetChildren(Item key, out List<Item> children);
        internal abstract bool HasKey1(Item key);
        internal abstract bool HasKey2(Item key);
        internal abstract int KeyCount { get; }
        #endregion
    }
}
