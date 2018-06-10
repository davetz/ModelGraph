using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
    RelationOf

    It maintains two maps (parent -> child) and (child -> parent). 
    This allows for a bidirectional traversal.
 */
    public class RelationOf<T1, T2> : Relation where T1 : Item where T2 : Item
    {
        static ArgumentException _invalidPairingException = new ArgumentException("Invalid Pairing");

        private MapToOne<T1> _parents1;
        private MapToOne<T2> _children1;
        private MapToMany<T1> _parents2;
        private MapToMany<T2> _children2;

        override internal bool IsValidParentChild(Item parentItem, Item childItem) { return (parentItem is T1 && childItem is T2); }

        #region Constructors  =================================================
        internal RelationOf() { } // dummy parameterless constructor

        internal RelationOf(StoreOf<Relation> owner, Trait trait, Guid guid, Pairing pairing = Pairing.OneToMany, int parentCount = 0, int childCount = 0, bool isRequired = false)
        {
            Guid = guid;
            Owner = owner;
            Trait = trait;
            Pairing = pairing;
            IsRequired = isRequired;
            Initialize(parentCount, childCount);

            owner.Add(this);
        }
        #endregion

        #region Initialize  ===================================================
        internal void Initialize(int parentCount, int childCount)
        {
            // create the maps only if we know thier going to be used
            if ((parentCount + childCount) > 0)
            {
                switch (Pairing)
                {
                    case Pairing.OneToOne:
                        _parents1 = new MapToOne<T1>(parentCount);
                        _children1 = new MapToOne<T2>(childCount);
                        break;

                    case Pairing.OneToMany:
                        _parents1 = new MapToOne<T1>(parentCount);
                        _children2 = new MapToMany<T2>(childCount);
                        break;

                    case Pairing.ManyToMany:
                        _parents2 = new MapToMany<T1>(parentCount);
                        _children2 = new MapToMany<T2>(childCount);
                        break;

                    default:
                        throw _invalidPairingException;
                }
            }
        }
        #endregion

        #region TrySetPairing  ================================================
        internal bool TrySetPairing(Pairing pairing)
        {
            if (Pairing == pairing)
            {
                return true;
            }
            if (_children1 == null && _children2 == null)
            {
                Pairing = pairing;
                return true;
            }
            switch (pairing)
            {
                case Pairing.OneToOne:
                    if ((_parents2 != null && !_parents2.CanMapToOne) || (_children2 != null && !_children2.CanMapToOne)) return false;
                    break;
                case Pairing.OneToMany:
                    if (_parents2 != null && !_parents2.CanMapToOne) return false;
                    break;
            }

            int count = 0;
            Item[] parents = null;
            Item[] children = null;
            if (_children1 != null) count = _children1.GetLinks(out parents, out children);
            if (_children2 != null) count = _children2.GetLinks(out parents, out children);

            Pairing = pairing;
            Initialize(count, count);

            if (_parents1 != null)
            {
                for (int i = 0; i < count; i++)
                {
                    _parents1.SetLink(children[i], (T1)parents[i]);
                }
            }
            if (_parents2 != null)
            {
                for (int i = 0; i < count; i++)
                {
                    _parents2.SetLink(children[i], (T1)parents[i]);
                }
            }
            if (_children1 != null)
            {
                for (int i = 0; i < count; i++)
                {
                    _children1.SetLink(parents[i], (T2)children[i]);
                }
            }
            if (_parents2 != null)
            {
                for (int i = 0; i < count; i++)
                {
                    _children2.SetLink(parents[i], (T2)children[i]);
                }
            }
            return true;
        }
        #endregion


        #region <Get,TryGet><Child,Parent,Children,Parents>  ==================
        internal bool TryGetChild(Item key, out T2 child)
        {
            child = GetChild(key);
            return (child != null);
        }
        internal T2 GetChild(Item key)
        {
            T2 child = null;

            if (_children1 != null)
            {
                _children1.TryGetVal(key, out child);
            }
            else if (_children2 != null && _children2.TryGetVals(key, out List<T2> children))
            {
                child = children[0];
            }

            return child;
        }
        internal bool TryGetChildren(Item key, out T2[] children)
        {
            children = GetChildren(key);
            return (children != null);
        }
        override internal bool HasChildLink(Item key)
        {
            return ((_children1 != null && _children1.ContainsKey(key)) || (_children2 != null && _children2.ContainsKey(key)));
        }
        override internal bool HasParentLink(Item key)
        {
            return ((_parents1 != null && _parents1.ContainsKey(key)) || (_parents2 != null && _parents2.ContainsKey(key)));
        }
        internal T2[] GetChildren(Item key)
        {
            List<T2> children = null;

            if (_children1 != null && _children1.TryGetVal(key, out T2 child))
            {
                children = new List<T2>(1);
                children.Add(child);
            }
            else if (_children2 != null)
            {
                _children2.TryGetVals(key, out children);
            }

            return (children == null) ? null : children.ToArray(); // protected against accidental corruption 
        }
        internal bool TryGetParent(Item key, out T1 parent)
        {
            parent = GetParent(key);
            return (parent != null);
        }
        internal T1 GetParent(Item key)
        {
            T1 parent = null;

            if (_parents1 != null)
            {
                _parents1.TryGetVal(key, out parent);
            }
            else if (_parents2 != null && _parents2.TryGetVals(key, out List<T1> parents))
            {
                parent = parents[0];
            }

            return parent;
        }
        internal bool TryGetParents(Item key, out T1[] parents)
        {
            parents = GetParents(key);
            return (parents != null);
        }
        internal T1[] GetParents(Item key)
        {
            List<T1> parents = null;

            if (_parents1 != null && _parents1.TryGetVal(key, out T1 parent))
            {
                parents = new List<T1>(1);
                parents.Add(parent);
            }
            else if (_parents2 != null)
            {
                _parents2.TryGetVals(key, out parents);
            }

            return (parents == null) ? null : parents.ToArray(); // protected against accidental corruption
        }
        internal override bool TryGetParents(Item key, out Item[] parents)
        {
            parents = GetParents(key);
            return (parents != null);
        }
        internal override bool TryGetChildren(Item key, out Item[] children)
        {
            children = GetChildren(key);
            return (children != null);
        }
        #endregion

        #region <Count,Move,Check,Insert,Append,Remove>Link  ==================
        override internal int ChildCount(Item key)
        {
            if (_children1 != null) return _children1.GetValCount(key);
            if (_children2 != null) return _children2.GetValCount(key);

            return 0;
        }
        override internal int ParentCount(Item key)
        {
            if (_parents1 != null) return _parents1.GetValCount(key);
            if (_parents2 != null) return _parents2.GetValCount(key);

            return 0;
        }
        override internal bool RelationExists(Item parentItem, Item childItem)
        {
            var key = parentItem as T1;
            var child = childItem as T2;
            return (key == null || child == null) ? false : RelationExists(key, child);
        }
        internal bool RelationExists(T1 key, T2 child)
        {
            if (_children1 != null) return _children1.ContainsLink(key, child);
            if (_children2 != null) return _children2.ContainsLink(key, child);

            return false;
        }

        internal override void MoveChild(Item keyRef, Item itemRef, int index)
        {
            var key = keyRef as T1;
            var item = itemRef as T2;
            if (key == null || item == null) return;

            MoveChildLink(key, item, index);
        }
        internal void MoveChildLink(T1 key, T2 item, int index)
        {
            if (_children2 != null) _children2.Move(key, item, index);
        }
        internal override void MoveParent(Item keyRef, Item itemRef, int index)
        {
            var key = keyRef as T2;
            var item = itemRef as T1;
            if (key == null || item == null) return;

            MoveParentLink(key, item, index);
        }
        internal void MoveParentLink(T2 key, T1 item, int index)
        {
            if (_parents2 != null) _parents2.Move(key, item, index);
        }
        override internal void InsertLink(Item parentItem, Item childItem, int parentIndex, int childIndex)
        {
            var parent = parentItem as T1;
            var child = childItem as T2;
            if (parent == null || child == null) return;

            InsertLink(parent, child, parentIndex, childIndex);
        }

        internal void InsertLink(T1 parent, T2 child, int parentIndex, int childIndex)
        {
            switch (Pairing)
            {
                case Pairing.OneToOne:
                    if (_parents1 == null)
                    {
                        _parents1 = new MapToOne<T1>();
                        _children1 = new MapToOne<T2>();
                    }
                    _parents1.InsertLink(child, parent, parentIndex);
                    _children1.InsertLink(parent, child, childIndex);
                    break;

                case Pairing.OneToMany:
                    if (_parents1 == null)
                    {
                        _parents1 = new MapToOne<T1>();
                        _children2 = new MapToMany<T2>();
                    }
                    _parents1.InsertLink(child, parent, parentIndex);
                    _children2.InsertLink(parent, child, childIndex);
                    break;

                case Pairing.ManyToMany:
                    if (_parents2 == null)
                    {
                        _parents2 = new MapToMany<T1>();
                        _children2 = new MapToMany<T2>();
                    }
                    _parents2.InsertLink(child, parent, parentIndex);
                    _children2.InsertLink(parent, child, childIndex);
                    break;
            }
        }
        override internal (int ParentIndex, int ChildIndex) AppendLink(Item parentItem, Item childItem)
        {
            var parent = parentItem as T1;
            var child = childItem as T2;
            if (parent == null || child == null) return (-1, -1);

            return AppendLink(parent, child);
        }
        internal (int ParentIndex, int ChildIndex) AppendLink(T1 parent, T2 child)
        {
            switch (Pairing)
            {
                case Pairing.OneToOne:
                    if (_parents1 == null)
                    {
                        _parents1 = new MapToOne<T1>();
                        _children1 = new MapToOne<T2>();
                    }
                    _parents1.SetLink(child, parent);
                    _children1.SetLink(parent, child);
                    break;

                case Pairing.OneToMany:
                    if (_parents1 == null)
                    {
                        _parents1 = new MapToOne<T1>();
                        _children2 = new MapToMany<T2>();
                    }
                    _parents1.SetLink(child, parent);
                    _children2.SetLink(parent, child);
                    break;

                case Pairing.ManyToMany:
                    if (_parents2 == null)
                    {
                        _parents2 = new MapToMany<T1>();
                        _children2 = new MapToMany<T2>();
                    }
                    _parents2.SetLink(child, parent);
                    _children2.SetLink(parent, child);
                    break;
            }
            return GetIndex(parent, child);
        }
        override internal (int ParentIndex, int ChildIndex) GetIndex(Item parentItem, Item childItem)
        {
            var parent = parentItem as T1;
            var child = childItem as T2;
            return (parent == null || child == null) ? (-1, -1) : GetIndex(parent, child);
        }
        internal (int ParentIndex, int ChildIndex) GetIndex(T1 parent, T2 child)
        {
            var parentIndex = -1;
            var childIndex = -1;

            if (_parents1 != null) parentIndex = _parents1.GetIndex(child, parent);
            else if (_parents2 != null) parentIndex = _parents2.GetIndex(child, parent);

            if (_children2 != null) childIndex = _children2.GetIndex(parent, child);
            else if (_children1 != null) childIndex = _children1.GetIndex(parent, child);

            return (parentIndex, childIndex);
        }
        internal override void RemoveLink(Item parentItem, Item childItem)
        {
            var parent = parentItem as T1;
            var child = childItem as T2;

            if (parent != null && child != null) RemoveLink(parent, child);
        }
        internal override (int Index1, int Index2) GetParentsIndex(Item keyRef, Item item1Ref, Item item2Ref)
        {
            var key = keyRef as T2;
            var item1 = item1Ref as T1;
            var item2 = item2Ref as T1;

            if (key != null && item1 != null && item2 != null) return GetParentsIndex(key, item1, item2);

            return (-1, -1);
        }
        internal (int Index1, int Index2) GetParentsIndex(T2 key, T1 item1, T1 item2)
        {
            if (_parents2 != null && _parents2.TryGetValue(key, out List<T1> items))
            {
                return (items.IndexOf(item1), items.IndexOf(item2));
            }
            return (-1, -1);
        }
        internal override (int Index1, int Index2) GetChildrenIndex(Item keyRef, Item item1Ref, Item item2Ref)
        {
            var key = keyRef as T1;
            var item1 = item1Ref as T2;
            var item2 = item2Ref as T2;

            if (key != null && item1 != null && item2 != null) return GetChildrenIndex(key, item1, item2);

            return (-1, -1);
        }
        internal (int Index1, int Index2) GetChildrenIndex(T1 key, T2 item1, T2 item2)
        {
            if (_children2 != null && _children2.TryGetValue(key, out List<T2> items))
            {
                return (items.IndexOf(item1), items.IndexOf(item2));
            }
            return (-1, -1);
        }
        internal void RemoveLink(T1 parent, T2 child)
        {
            if (_parents1 != null) _parents1.RemoveLink(child, parent);
            else if (_parents2 != null) _parents2.RemoveLink(child, parent);

            if (_children2 != null) _children2.RemoveLink(parent, child);
            else if (_children1 != null) _children1.RemoveLink(parent, child);
        }
        #endregion

        #region HasKey<1,2>  ==================================================
        /// <summary>
        /// Dose the key item have at least one child item?
        /// </summary>
        internal override bool HasKey1(Item key)
        {
            if (_children1 != null) return _children1.ContainsKey(key);
            if (_children2 != null) return _children2.ContainsKey(key);

            return false;
        }

        /// <summary>
        /// Dose the key item have at least one parent item?
        /// </summary>
        internal override bool HasKey2(Item key)
        {
            if (_parents1 != null) return _parents1.ContainsKey(key);
            if (_parents2 != null) return _parents2.ContainsKey(key);

            return false;
        }

        /// <summary>
        /// Does the relation contain this link?
        /// </summary>
        internal bool ContainsLink(Item parent, T2 child)
        {
            if (Pairing == Pairing.OneToOne &&
                _children1 != null) return _children1.ContainsLink(parent, child);
            if (_children2 != null) return _children2.ContainsLink(parent, child);

            return false;
        }
        #endregion

        #region Load/Save  ====================================================
        // properties and methods primarily used by the Load and Store operation 
        internal override int KeyCount => GetKeyCount();
        private int GetKeyCount()
        {
            if (Pairing == Pairing.OneToOne &&
                _children1 != null) return _children1.KeyCount;
            if (_children2 != null) return _children2.KeyCount;

            return 0;
        }
        internal int ValueCount => GetValueCount();
        private int GetValueCount()
        {
            if (Pairing == Pairing.OneToOne &&
                _children1 != null) return _children1.ValueCount;
            if (_children2 != null) return _children2.ValueCount;

            return 0;
        }
        internal int GetKeys(out Item[] keys)
        {
            if (Pairing == Pairing.OneToOne &&
                _children1 != null) return _children1.GetKeys(out keys);
            if (_children2 != null) return _children2.GetKeys(out keys);

            keys = new Item[0];
            return 0;
        }
        internal int GetValues(out Item[] values)
        {
            if (Pairing == Pairing.OneToOne &&
                _children1 != null) return _children1.GetValues(out values);
            if (_children2 != null) return _children2.GetValues(out values);

            values = new Item[0];
            return 0;
        }
        internal override int GetLinksCount()
        {
            if (Pairing == Pairing.OneToOne &&
                _children1 != null) return _children1.GetLinksCount();
            if (_children2 != null) return _children2.GetLinksCount();

            return 0;
        }

        internal bool HasLinks => GetHasLinks();
        private bool GetHasLinks()
        {
            if (Pairing == Pairing.OneToOne &&
                _children1 != null && _children1.KeyCount > 0) return true;
            if (_children2 != null && _children2.KeyCount > 0) return true;

            return false;
        }
        internal override int GetLinks(out Item[] parents, out Item[] children)
        {
            if (Pairing == Pairing.OneToOne &&
                _children1 != null) return _children1.GetLinks(out parents, out children);
            if (_children2 != null) return _children2.GetLinks(out parents, out children);

            parents = new Item[0];
            children = new Item[0];
            return 0;
        }

        internal override void SetLink(Item parent, Item child, int capacity = 0)
        {
            var key = parent as T1;
            var val = child as T2;
            if (key != null && val != null) SetLink(key, val, capacity);
        }
        internal void SetLink(T1 key, T2 val, int capacity) // only used only by Load - NO NULL CHECKING
        {
            switch (Pairing)
            {
                case Pairing.OneToMany:
                    _parents1.SetLink(val, key);
                    _children2.SetLink(key, val, capacity);
                    break;

                case Pairing.OneToOne:
                    _parents1.SetLink(val, key);
                    _children1.SetLink(key, val);
                    break;

                case Pairing.ManyToMany:
                    _parents2.SetLink(val, key, 2);
                    _children2.SetLink(key, val, capacity);
                    break;
            }
        }
        internal void SetLink(T1 key, T2[] vals)
        {
            var cap = vals.Length;
            foreach (var val in vals)
            {
                SetLink(key, val, cap);
            }

        }
        #endregion
    }
}
