using System;

namespace ModelGraphLibrary
{/*

 */
    public partial class Chef
    {
        private Dummy _dummy;
        private ChangeSet _changeSet;
        private ChangeRoot _changeRoot;
        private int _changeSequence;

        private Store[] _primeStores;
        private StoreOf<EnumX> _enumXStore;
        private StoreOf<ViewX> _viewXStore;
        private StoreOf<TableX> _tableXStore;
        private StoreOf<GraphX> _graphXStore;
        private StoreOf<QueryX> _queryXStore;
        private StoreOf<ColumnX> _columnXStore;
        private StoreOf<SymbolX> _symbolXStore;
        private StoreOf<ComputeX> _computeXStore;
        private StoreOf<RelationX> _relationXStore;

        private StoreOf<Error> _errorStore;
        private StoreOf<EnumZ> _enumZStore;
        private StoreOf<Property> _propertyStore;
        private StoreOf<Relation> _relationStore;
        private StoreOf<Property> _propertyZStore;
        private StoreOf<Relation> _relationZStore;


        #region InitializeStores  =============================================
        private void InitializeStores()
        {
            _dummy = new Dummy(this);
            _changeRoot = new ChangeRoot(this);
            _changeSequence = 1;
            _changeSet = new ChangeSet(_changeRoot, _changeSequence);

            _propertyZStore = new StoreOf<Property>(this, Trait.PropertyStore, Guid.Empty, 30);
            _relationZStore = new StoreOf<Relation>(this, Trait.RelationZStore, Guid.Empty, 10);
            _enumZStore = new StoreOf<EnumZ>(this, Trait.EnumZStore, Guid.Empty, 10);
            _errorStore = new StoreOf<Error>(this, Trait.ErrorStore, Guid.Empty, 10);

            _propertyStore = new StoreOf<Property>(this, Trait.PropertyStore, new System.Guid("BA10F400-9A33-4F65-80A1-C2259D17A938"), 100);
            _relationStore = new StoreOf<Relation>(this, Trait.RelationStore, new System.Guid("42743CEF-2172-4C55-A575-9A26357E4FB5"), 30);
            _enumXStore = new StoreOf<EnumX>(this, Trait.EnumXStore, new System.Guid("EC7B6089-AD64-4100-8F65-BA8130969EB0"), 10);
            _viewXStore = new StoreOf<ViewX>(this, Trait.ViewXStore, new System.Guid("C11EAF6E-20A2-4F2E-AF19-0BC49DF561AB"), 10);
            _tableXStore = new StoreOf<TableX>(this, Trait.TableXStore, new System.Guid("0E00F963-18F6-4C7C-A6E9-71C4CCE001DC"), 30);
            _graphXStore = new StoreOf<GraphX>(this, Trait.GraphXStore, new System.Guid("72C2BEC8-B8C8-44A1-ADF0-3832416820F3"), 30);
            _queryXStore = new StoreOf<QueryX>(this, Trait.QueryXStore, new System.Guid("085A1887-03FE-4DA1-9B54-9BED3B34F518"), 300);
            _columnXStore = new StoreOf<ColumnX>(this, Trait.ColumnXStore, new System.Guid("44F4B1B2-927C-40DF-A8E6-60A1E4DA58A6"), 300);
            _symbolXStore = new StoreOf<SymbolX>(this, Trait.SymbolXStore, new System.Guid("4ED54C41-4EDD-41D6-8451-2FEF0967C12F"), 100);
            _computeXStore = new StoreOf<ComputeX>(this, Trait.ComputeXStore, new System.Guid("A3F850B4-B498-4339-94B8-4F0E355BAD92"), 300);
            _relationXStore = new StoreOf<RelationX>(this, Trait.RelationXStore, new System.Guid("BD104B70-CB79-42C3-858D-588B6B868269"), 300);

            _primeStores = new Store[]
            {
                _enumXStore,
                _viewXStore,
                _tableXStore,
                _graphXStore,
                _queryXStore,
                _columnXStore,
                _symbolXStore,
                _computeXStore,
                _relationXStore,
                _relationStore,
                _propertyStore,
            };
        }
        #endregion
    }
}
