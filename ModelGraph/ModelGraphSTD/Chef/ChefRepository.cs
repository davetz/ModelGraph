using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        internal IRepository Repository;

        private void SaveToRepository()
        {
            if (Repository != null)
            {
                Repository.Write(this);
                CongealChanges();
            }
        }
        private void SaveToRepository(IRepository repository)
        {
            Repository = repository;
            SaveToRepository();
        }
        private string GetLongRepositoryName() => (Repository == null) ? NullStorageFileName : Repository.FullName;
        private string GetRepositoryName() => (Repository == null) ? NullStorageFileName : Repository.Name;
        private string NullStorageFileName => $"{_localize(GetNameKey(Trait.NewModel))} #{_newChefNumber}";


        internal void PostReadValidation()
        {
            ValidateQueryXStore();
        }

        #region GetGuidItems  =================================================
        internal Dictionary<Guid, Item> GetGuidItems()
        {
            var count = GetGuidItemIndex(out Guid[] guids, out Dictionary<Item, int> itemIndex);
            var guidItems = new Dictionary<Guid, Item>(count);
            foreach (var e in itemIndex)
            {
                guidItems.Add(guids[e.Value], e.Key);
            }
            return guidItems;
        }
        #endregion

        #region GetGuidItemIndex  =============================================
        internal int GetGuidItemIndex(out Guid[] guids, out Dictionary<Item, int> itemIndex)
        {
            // count all items that have guids
            //=============================================
            int count = 31; // allow for static store guids

            foreach (var item in EnumXStore.Items)
            {
                count += (item as EnumX).Count; // PairX count
            }

            foreach (var item in TableXStore.Items)
            {
                count += (item as TableX).Count; // RowX count
            }

            count += ViewXStore.Count;
            count += EnumXStore.Count;
            count += TableXStore.Count;
            count += GraphXStore.Count;
            count += QueryXStore.Count;
            count += SymbolStore.Count;
            count += ColumnXStore.Count;
            count += ComputeXStore.Count;
            count += RelationXStore.Count;
            count += RelationStore.Count;

            // allocate memory
            //=============================================
            guids = new Guid[count];
            itemIndex = new Dictionary<Item, int>(count);


            // populate the item and guid arrays
            //=============================================
            int j = 0;
            itemIndex.Add(Dummy, j);
            guids[j++] = Dummy.Guid;

            itemIndex.Add(QueryXNode, j);
            guids[j++] = QueryXNode.Guid;

            itemIndex.Add(ViewXStore, j);
            guids[j++] = ViewXStore.Guid;
            foreach (var itm in ViewXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(EnumXStore, j);
            guids[j++] = EnumXStore.Guid;
            foreach (var itm in EnumXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(TableXStore, j);
            guids[j++] = TableXStore.Guid;
            foreach (var itm in TableXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(GraphXStore, j);
            guids[j++] = GraphXStore.Guid;
            foreach (var itm in GraphXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(QueryXStore, j);
            guids[j++] = QueryXStore.Guid;
            foreach (var itm in QueryXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(SymbolStore, j);
            guids[j++] = SymbolStore.Guid;
            foreach (var itm in SymbolStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(ColumnXStore, j);
            guids[j++] = ColumnXStore.Guid;
            foreach (var itm in ColumnXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(ComputeXStore, j);
            guids[j++] = ComputeXStore.Guid;
            foreach (var itm in ComputeXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(RelationXStore, j);
            guids[j++] = RelationXStore.Guid;
            foreach (var itm in RelationXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(RelationStore, j);
            guids[j++] = RelationStore.Guid;
            foreach (var rel in RelationStore.Items)
            {
                itemIndex.Add(rel, j);
                guids[j++] = rel.Guid;
            }

            itemIndex.Add(PropertyStore, j); //for posible compute reference
            guids[j++] = PropertyStore.Guid;

            // put grandchild items at the end
            //=============================================
            foreach (var parent in EnumXStore.Items)
            {
                foreach (var child in parent.Items)
                {
                    itemIndex.Add(child, j);
                    guids[j++] = child.Guid;
                }
            }
            foreach (var parent in TableXStore.Items)
            {
                foreach (var itm in parent.Items)
                {
                    var child = itm;
                    itemIndex.Add(child, j);
                    guids[j++] = child.Guid;
                }
            }
            return count;
        }
        #endregion

        #region GetRelationList  ==============================================
        // Get list of relations that reference at least one serialized item
        internal List<Relation> GetRelationList()
        {
            var relationList = new List<Relation>(RelationStore.Count + RelationXStore.Count);

            foreach (var rel in RelationStore.Items)
            {
                var len = rel.GetLinks(out List<Item> parents, out List<Item> children);
                for (int i = 0; i < len; i++)
                {
                    if (parents[i].IsExternal || children[i].IsExternal)
                    {
                        relationList.Add(rel);
                        break;
                    }
                }
            }

            foreach (var item in RelationXStore.Items)
            {
                var rel = item as RelationX;
                if (rel.HasLinks) relationList.Add(rel);
            }

            return relationList;
        }
        #endregion
    }
}
