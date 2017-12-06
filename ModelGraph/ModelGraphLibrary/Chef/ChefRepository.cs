using System;
using System.Collections.Generic;

namespace ModelGraphLibrary
{/*

 */
    public partial class Chef
    {
        public IRepository Repository { get; set; }

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
            InitializeChoiceColumns();
            ValidateQueryXStore();
        }

        #region GetGuidItems  =================================================
        internal Dictionary<Guid, Item> GetGuidItems()
        {
            Guid[] guids;
            Dictionary<Item, int> itemIndex;
            var count = GetGuidItemIndex(out guids, out itemIndex);
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
            int count = 23; // allow for static store guids

            foreach (var item in _enumXStore.Items)
            {
                count += (item as EnumX).Count; // PairX count
            }

            foreach (var item in _tableXStore.Items)
            {
                count += (item as TableX).Count; // RowX count
            }

            count += _viewXStore.Count;
            count += _enumXStore.Count;
            count += _tableXStore.Count;
            count += _graphXStore.Count;
            count += _queryXStore.Count;
            count += _symbolXStore.Count;
            count += _columnXStore.Count;
            count += _computeXStore.Count;
            count += _relationXStore.Count;
            count += _relationStore.Count;

            // allocate memory
            //=============================================
            guids = new Guid[count];
            itemIndex = new Dictionary<Item, int>(count);


            // populate the item and guid arrays
            //=============================================
            int j = 0;
            itemIndex.Add(_dummy, j);
            guids[j++] = _dummy.Guid;

            itemIndex.Add(_viewXStore, j);
            guids[j++] = _viewXStore.Guid;
            foreach (var itm in _viewXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_enumXStore, j);
            guids[j++] = _enumXStore.Guid;
            foreach (var itm in _enumXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_tableXStore, j);
            guids[j++] = _tableXStore.Guid;
            foreach (var itm in _tableXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_graphXStore, j);
            guids[j++] = _graphXStore.Guid;
            foreach (var itm in _graphXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_queryXStore, j);
            guids[j++] = _queryXStore.Guid;
            foreach (var itm in _queryXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_symbolXStore, j);
            guids[j++] = _symbolXStore.Guid;
            foreach (var itm in _symbolXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_columnXStore, j);
            guids[j++] = _columnXStore.Guid;
            foreach (var itm in _columnXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_computeXStore, j);
            guids[j++] = _computeXStore.Guid;
            foreach (var itm in _computeXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_relationXStore, j);
            guids[j++] = _relationXStore.Guid;
            foreach (var itm in _relationXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_relationStore, j);
            guids[j++] = _relationStore.Guid;
            foreach (var rel in _relationStore.Items)
            {
                itemIndex.Add(rel, j);
                guids[j++] = rel.Guid;
            }

            itemIndex.Add(_propertyStore, j); //for posible compute reference
            guids[j++] = _propertyStore.Guid;

            // put grandchild items at the end
            //=============================================
            foreach (var parent in _enumXStore.Items)
            {
                foreach (var child in parent.Items)
                {
                    itemIndex.Add(child, j);
                    guids[j++] = child.Guid;
                }
            }
            foreach (var parent in _tableXStore.Items)
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
            var relationList = new List<Relation>(_relationStore.Count + _relationXStore.Count);

            Item[] parents;
            Item[] children;

            foreach (var rel in _relationStore.Items)
            {
                var len = rel.GetLinks(out parents, out children);
                for (int i = 0; i < len; i++)
                {
                    if (parents[i].IsExternal || children[i].IsExternal)
                    {
                        relationList.Add(rel);
                        break;
                    }
                }
            }

            foreach (var item in _relationXStore.Items)
            {
                var rel = item as RelationX;
                if (rel.HasLinks) relationList.Add(rel);
            }

            return relationList;
        }
        #endregion
    }
}
