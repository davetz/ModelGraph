using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        #region GetHeadTail  ==================================================
        internal void GetHeadTail(Relation rel, out Store head, out Store tail)
        {
            if (rel == null)
            {
                head = null;
                tail = null;
            }
            else if (rel.IsRelationX)
            {
                head = TableX_ChildRelationX.GetParent(rel);
                tail = TableX_ParentRelationX.GetParent(rel);
            }
            else
            {
                head = Store_ChildRelation.GetParent(rel);
                tail = Store_ParentRelation.GetParent(rel);
            }
        }
        #endregion

        #region <Get,Set>RelationName =========================================
        private const string parentNameSuffix = " --> ";
        private const string childNameSuffix = "       (";
        private const string identitySuffix = ")";
        internal string GetRelationName(RelationX rel)
        {
            var identity = string.IsNullOrWhiteSpace(rel.Name) ? string.Empty : rel.Name;
            var childName = BlankName;
            var parentName = BlankName;
            if (TableX_ParentRelationX.TryGetParent(rel, out TableX childTable)) childName = childTable.Name;
            if (TableX_ChildRelationX.TryGetParent(rel, out TableX parentTable)) parentName = parentTable.Name;
            StringBuilder sb = new StringBuilder(132);
            sb.Append(parentName);
            sb.Append(parentNameSuffix);
            sb.Append(childName);
            sb.Append(childNameSuffix);
            sb.Append(identity);
            sb.Append(identitySuffix);
            return sb.ToString();
        }
        internal void SetRelationName(RelationX rel, string value)
        {
            var childName = BlankName;
            var parentName = BlankName;
            if (TableX_ParentRelationX.TryGetParent(rel, out TableX childTable)) childName = childTable.Name;
            if (TableX_ChildRelationX.TryGetParent(rel, out TableX parentTable)) parentName = parentTable.Name;
            StringBuilder sb = new StringBuilder(value);
            sb.Replace(parentName + parentNameSuffix, "");
            sb.Replace(childName + childNameSuffix, "");
            sb.Replace(identitySuffix, "");
            rel.Name = sb.ToString();
        }
        string GetRelationName(QueryX sd)
        {
            return (Relation_QueryX.TryGetParent(sd, out Relation rel) ? GetRelationName(rel as RelationX) : null);
        }
        #endregion

        #region <Remove,Append>Link  ==========================================
        internal void RemoveLink(Relation rel, Item parent, Item child)
        {
            MarkItemUnlinked(rel, parent, child);
            Redo(_changeSet);
        }

        internal void AppendLink(Relation rel, Item parent, Item child)
        {
            AddLinkCheck(rel, parent, child);
            ItemLinked(rel, parent, child);
        }
        private void AddLinkCheck(Relation rel, Item parent, Item child)
        {
            //Item prevParent;
            //List<Item> prevParents;
            //RemoveMutuallyExclusiveLinks(rel, parent, child);
            //switch (rel.Pairing)
            //{
            //    case Pairing.OneToOne:
            //    case Pairing.OneToMany:
            //        if (rel.Parents.TryGetVal(child, out prevParent) && rel.Children.TryGetIndex(prevParent, child, out childIndex) && rel.Parents.TryGetIndex(child, prevParent, out parentIndex))
            //        {
            //            if (prevParent == parent) return; // the link already exists
            //            RemoveLink(rel, prevParent, child);
            //        }
            //        break;
            //    case Pairing.ManyToMany:
            //        if (rel.Parents.TryGetVals(child, out prevParents))
            //        {
            //            foreach (var testParent in prevParents)
            //            {
            //                if (testParent == parent) return; // the link already exists
            //            }
            //        }
            //        break;
            //}
        }
        #endregion

    }
}
