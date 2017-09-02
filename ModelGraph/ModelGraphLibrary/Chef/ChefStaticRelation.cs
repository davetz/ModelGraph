using System;
using System.Collections.Generic;

namespace ModelGraphLibrary
{/*

 */
    public partial class Chef
    {
        private RelationOf<Store, Property> Store_Property;
        private RelationOf<Store, Relation> Store_ChildRelation;
        private RelationOf<Store, Relation> Store_ParentRelation;

        private RelationOf<TableX, ColumnX> TableX_ColumnX;
        private RelationOf<TableX, Property> TableX_NameProperty; // can be a ColumnX or a ComputeX
        private RelationOf<TableX, Property> TableX_SummaryProperty; // can be a ColumnX or a ComputeX
        private RelationOf<TableX, RelationX> TableX_ChildRelationX;
        private RelationOf<TableX, RelationX> TableX_ParentRelationX;

        //private Relation Table_ChildRelationGroup;
        //private Relation Table_ParentRelationGroup;
        //private Relation Table_ReverseRelationGroup;
        //private Relation ChildRelationGroup_Relation;
        //private Relation ParentRelationGroup_Relation;
        //private Relation ReverseRelationGroup_Relation;

        private RelationOf<Item, Error> Item_Error;
        private RelationOf<ViewX, ViewX> ViewX_ViewX;
        private RelationOf<QueryX, ViewX> QueryX_ViewX;
        private RelationOf<Property, ViewX> Property_ViewX;
        private RelationOf<Relation, ViewX>  Relation_ViewX;

        private RelationOf<GraphX, QueryX> GraphX_SymbolQueryX; //REDUNDANT ??

        private RelationOf<GraphX, ColumnX>  GraphX_ColorColumnX;
        private RelationOf<GraphX, SymbolX>   GraphX_SymbolX;

        private RelationOf<ViewX, QueryX> ViewX_QueryX;
        private RelationOf<Store, QueryX> Store_QueryX;
        private RelationOf<GraphX, QueryX> GraphX_QueryX;
        private RelationOf<QueryX, QueryX> QueryX_QueryX;
        private RelationOf<SymbolX, QueryX> SymbolX_QueryX;
        private RelationOf<ComputeX, QueryX> ComputeX_QueryX;
        private RelationOf<Relation, QueryX> Relation_QueryX;

        private RelationOf<ViewX, Property> ViewX_Property;
        private RelationOf<QueryX, Property> QueryX_Property;

        private RelationOf<EnumX, ColumnX> EnumX_ColumnX;
        private RelationOf<Store, ComputeX>   Store_ComputeX;

        #region UnitTestAccess  ===============================================
        /*
            ModelGraphUnitTest : TestData references these relations
        */
        internal RelationOf<Store, Property> R_Store_Property => Store_Property;
        internal RelationOf<Store, Relation> R_Store_ChildRelation => Store_ChildRelation;
        internal RelationOf<Store, Relation> R_Store_ParentRelation => Store_ParentRelation;

        internal RelationOf<TableX, ColumnX> R_TableX_ColumnX => TableX_ColumnX;
        internal RelationOf<TableX, Property> R_TableX_NameProperty => TableX_NameProperty; // can be a ColumnX or a ComputeX
        internal RelationOf<TableX, Property> R_TableX_SummaryProperty => TableX_SummaryProperty; // can be a ColumnX or a ComputeX
        internal RelationOf<TableX, RelationX> R_TableX_ChildRelationX => TableX_ChildRelationX;
        internal RelationOf<TableX, RelationX> R_TableX_ParentRelationX => TableX_ParentRelationX;


        internal RelationOf<Item, Error> R_Item_Error => Item_Error;
        internal RelationOf<ViewX, ViewX> R_ViewX_ViewX => ViewX_ViewX;
        internal RelationOf<QueryX, ViewX> R_QueryX_ViewX => QueryX_ViewX;
        internal RelationOf<Property, ViewX> R_Property_ViewX => Property_ViewX;
        internal RelationOf<Relation, ViewX> R_Relation_ViewX => Relation_ViewX;

        internal RelationOf<GraphX, QueryX> R_GraphX_SymbolQueryX => GraphX_SymbolQueryX; //REDUNDANT ??

        internal RelationOf<GraphX, ColumnX> R_GraphX_ColorColumnX => GraphX_ColorColumnX;
        internal RelationOf<GraphX, SymbolX> R_GraphX_SymbolX => GraphX_SymbolX;

        internal RelationOf<ViewX, QueryX> R_ViewX_QueryX => ViewX_QueryX;
        internal RelationOf<Store, QueryX> R_Store_QueryX => Store_QueryX;
        internal RelationOf<GraphX, QueryX> R_GraphX_QueryX => GraphX_QueryX;
        internal RelationOf<QueryX, QueryX> R_QueryX_QueryX => QueryX_QueryX;
        internal RelationOf<SymbolX, QueryX> R_SymbolX_QueryX => SymbolX_QueryX;
        internal RelationOf<ComputeX, QueryX> R_ComputeX_QueryX => ComputeX_QueryX;
        internal RelationOf<Relation, QueryX> R_Relation_QueryX => Relation_QueryX;

        internal RelationOf<ViewX, Property> R_ViewX_Property => ViewX_Property;
        internal RelationOf<QueryX, Property> R_QueryX_Property => QueryX_Property;

        internal RelationOf<EnumX, ColumnX> R_EnumX_ColumnX => EnumX_ColumnX;
        internal RelationOf<Store, ComputeX> R_Store_ComputeX => Store_ComputeX;
        #endregion

        #region InitializeRelations  ==========================================
        private void InitializeRelations()
        {
            Item_Error = new RelationOf<Item, Error>(_relationZStore, Trait.Item_Error, new Guid("3BD787B0-C38A-4288-9E12-1C688EEF984E"), Pairing.OneToMany, 25, 25);
            Store_Property = new RelationOf<Store, Property>(_relationZStore, Trait.Store_Property, Guid.Empty, Pairing.OneToMany, 40, 100, true);
            Store_ChildRelation = new RelationOf<Store, Relation>(_relationZStore, Trait.Store_ChildRelation, Guid.Empty, Pairing.ManyToMany, 25, 25, true);
            Store_ParentRelation = new RelationOf<Store, Relation>(_relationZStore, Trait.Store_ParentRelation, Guid.Empty, Pairing.ManyToMany, 25, 25, true);

            EnumX_ColumnX = new RelationOf<EnumX, ColumnX>(_relationStore, Trait.EnumX_ColumnX, new Guid("9179013E-57D5-4265-965B-1633B9252EDF"), Pairing.OneToMany, 25, 25);
            AddIntegrityCheck(EnumX_ColumnX, _enumXStore, _columnXStore);

            TableX_ColumnX = new RelationOf<TableX,ColumnX>(_relationStore, Trait.TableX_ColumnX, new Guid("C6374551-5BC9-4906-B565-91A85D42FCEE"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(TableX_ColumnX, _tableXStore, _columnXStore);

            TableX_NameProperty = new RelationOf<TableX, Property>(_relationStore, Trait.TableX_NameProperty, new Guid("83717F0C-1C43-44A4-B3DF-7E81CDBCE638"), Pairing.OneToOne, 25, 25);
            AddIntegrityCheck(TableX_NameProperty, _tableXStore, _columnXStore);
            AddIntegrityCheck(TableX_NameProperty, _tableXStore, _computeXStore);

            TableX_SummaryProperty = new RelationOf<TableX, Property>(_relationStore, Trait.TableX_SummaryProperty, new Guid("332DBDC6-AAC2-4C8F-A192-D5780DE680F0"), Pairing.OneToOne, 25, 25);
            AddIntegrityCheck(TableX_SummaryProperty, _tableXStore, _columnXStore);
            AddIntegrityCheck(TableX_SummaryProperty, _tableXStore, _computeXStore);

            TableX_ChildRelationX = new RelationOf<TableX, RelationX>(_relationStore, Trait.TableX_ChildRelationX, new Guid("DDA99343-07F6-419B-9227-EB20EDC98982"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(TableX_ChildRelationX, _tableXStore, _relationXStore);

            TableX_ParentRelationX = new RelationOf<TableX, RelationX>(_relationStore, Trait.TableX_ParentRelationX, new Guid("E38B8AA9-1F38-400E-8822-31E70DA1ECD7"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(TableX_ParentRelationX, _tableXStore, _relationXStore);

            Store_ComputeX = new RelationOf<Store, ComputeX>(_relationStore, Trait.Store_ComputeX, new Guid("937964ED-21BB-429D-B829-4CC3FA3539FC"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(Store_ComputeX, _enumXStore, _computeXStore);
            AddIntegrityCheck(Store_ComputeX, _tableXStore, _computeXStore);
            AddIntegrityCheck(Store_ComputeX, _columnXStore, _computeXStore);
            AddIntegrityCheck(Store_ComputeX, _relationXStore, _computeXStore);
            AddIntegrityCheck(Store_ComputeX, _viewXStore, _computeXStore);
            AddIntegrityCheck(Store_ComputeX, _symbolXStore, _computeXStore);
            AddIntegrityCheck(Store_ComputeX, _computeXStore, _computeXStore);
            AddIntegrityCheck(Store_ComputeX, _propertyStore, _computeXStore);
            AddIntegrityCheck(Store_ComputeX, _relationStore, _computeXStore);

            //Table_ChildRelationGroup = new Relation(_relationStore, Trait.TableChildRelationGroup, new Guid("E2A067DF-29B2-413A-B8F3-EEC2E46E0980"), Pairing.OneToMany, 25, 25, true);
            //Table_ParentRelationGroup = new Relation(_relationStore, Trait.TableParentRelationGroup, new Guid("5D785910-EEF3-4E2D-AE38-B7BC2EB87D42"), Pairing.OneToMany, 25, 25, true);
            //Table_ReverseRelationGroup = new Relation(_relationStore, Trait.TableReverseRelationGroup, new Guid("699D5034-EBDC-4AEA-9754-430BC6A72295"), Pairing.OneToMany, 25, 25, true);
            //ChildRelationGroup_Relation = new Relation(_relationStore, Trait.TableRelationGroupRelation, new Guid("DD96E791-920B-4C60-B245-A9C2F868E539"), Pairing.OneToMany, 25, 25);
            //ParentRelationGroup_Relation = new Relation(_relationStore, Trait.ParentRelationGroupRelation, new Guid("1892E827-042D-4700-99B2-25E504908EB7"), Pairing.OneToMany, 25, 25);
            //ReverseRelationGroup_Relation = new Relation(_relationStore, Trait.ReverseRelationGroupRelation, new Guid("0DCE9775-4944-4958-BE41-373401CCD564"), Pairing.OneToMany, 25, 25);

            ViewX_ViewX = new RelationOf<ViewX, ViewX>(_relationStore, Trait.ViewX_ViewX, new Guid("6CC10237-CD1D-4469-879E-020902ADA85C"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(ViewX_ViewX, _viewXStore, _viewXStore);

            QueryX_ViewX = new RelationOf<QueryX, ViewX>(_relationStore, Trait.QueryX_ViewX, new Guid("C5A5DBAD-5C95-4043-BC2B-75ED4C3F2B56"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(QueryX_ViewX, _queryXStore, _viewXStore);

            ViewX_QueryX = new RelationOf<ViewX, QueryX>(_relationStore, Trait.ViewX_QueryX, new Guid("80DF4DF6-4B8D-4D47-B7BE-C8EFF11CF8BB"), Pairing.OneToOne, 25, 25, true);
            AddIntegrityCheck(ViewX_QueryX, _viewXStore, _queryXStore);

            Property_ViewX = new RelationOf<Property, ViewX>(_relationStore, Trait.Property_ViewX, new Guid("3C73D6CC-1879-4450-BC8F-1D38F299617E"), Pairing.OneToMany, 25, 25);
            AddIntegrityCheck(Property_ViewX, _columnXStore, _viewXStore);
            AddIntegrityCheck(Property_ViewX, _computeXStore, _viewXStore);

            Relation_ViewX = new RelationOf<Relation, ViewX>(_relationStore, Trait.Relation_ViewX, new Guid("7FF00C7B-6EB2-40CF-A749-60E3E8E64AEF"), Pairing.OneToMany, 25, 25);
            AddIntegrityCheck(Relation_ViewX, _relationXStore, _viewXStore);


            GraphX_SymbolQueryX = new RelationOf<GraphX, QueryX>(_relationStore, Trait.GraphX_SymbolQueryX, new Guid("478BA83E-1EA7-48B1-9F18-8FC69B503215"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(GraphX_SymbolQueryX, _graphXStore, _queryXStore);

            GraphX_SymbolX = new RelationOf<GraphX, SymbolX>(_relationStore, Trait.GraphX_SymbolX, new Guid("82FA283D-7A07-452F-A5C8-0FF6E568D64D"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(GraphX_SymbolX, _graphXStore, _symbolXStore);

            GraphX_ColorColumnX = new RelationOf<GraphX, ColumnX>(_relationStore, Trait.GraphX_ColorColumnX, new Guid("66C6D619-648A-4A7B-92E0-7224780819F0"), Pairing.OneToOne, 25, 25);
            AddIntegrityCheck(GraphX_ColorColumnX, _graphXStore, _columnXStore);

            GraphX_QueryX = new RelationOf<GraphX, QueryX>(_relationStore, Trait.GraphX_QueryX, new Guid("B740311F-553A-4302-BDE3-53707B51DFCA"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(GraphX_QueryX, _graphXStore, _queryXStore);

            QueryX_QueryX = new RelationOf<QueryX, QueryX>(_relationStore, Trait.QueryX_QueryX, new Guid("41030BD7-4986-4CA0-B3E6-B194AFA75E14"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(QueryX_QueryX, _queryXStore, _queryXStore);
            AddIntegrityCheck(QueryX_QueryX, _queryXStore, _queryXStore);
            AddIntegrityCheck(QueryX_QueryX, _queryXStore, _queryXStore);

            QueryX_Property = new RelationOf<QueryX, Property>(_relationStore, Trait.QueryX_Property, new Guid("CE770665-173A-4B9C-8288-803FEFC0BC11"), Pairing.OneToMany, 25, 25);
            AddIntegrityCheck(QueryX_Property, _queryXStore, _columnXStore);
            AddIntegrityCheck(QueryX_Property, _queryXStore, _computeXStore);

            ViewX_Property = new RelationOf<ViewX, Property>(_relationStore, Trait.ViewX_Property, new Guid("87E57D4F-C9B8-46FD-930E-85E6DA77C169"), Pairing.OneToMany, 25, 25);
            AddIntegrityCheck(ViewX_Property, _viewXStore, _columnXStore);
            AddIntegrityCheck(ViewX_Property, _viewXStore, _computeXStore);

            Store_QueryX = new RelationOf<Store, QueryX>(_relationStore, Trait.Store_QueryX, new Guid("08A8840E-D218-4827-B442-F08DED3B82B8"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(Store_QueryX, _enumXStore, _queryXStore);
            AddIntegrityCheck(Store_QueryX, _viewXStore, _queryXStore);
            AddIntegrityCheck(Store_QueryX, _tableXStore, _queryXStore);
            AddIntegrityCheck(Store_QueryX, _queryXStore, _queryXStore);
            AddIntegrityCheck(Store_QueryX, _queryXStore, _queryXStore);
            AddIntegrityCheck(Store_QueryX, _symbolXStore, _queryXStore);
            AddIntegrityCheck(Store_QueryX, _columnXStore, _queryXStore);
            AddIntegrityCheck(Store_QueryX, _computeXStore, _queryXStore);
            AddIntegrityCheck(Store_QueryX, _computeXStore, _queryXStore);
            AddIntegrityCheck(Store_QueryX, _propertyStore, _queryXStore);
            AddIntegrityCheck(Store_QueryX, _relationStore, _queryXStore);
            AddIntegrityCheck(Store_QueryX, _relationXStore, _queryXStore);


            SymbolX_QueryX = new RelationOf<SymbolX, QueryX>(_relationStore, Trait.SymbolX_QueryX, new Guid("0D2836A0-A482-4E1F-8752-86A69BA5E5A6"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(SymbolX_QueryX, _symbolXStore, _queryXStore);

            Relation_QueryX = new RelationOf<Relation, QueryX>(_relationStore, Trait.Relation_QueryX, new Guid("7F44E7A0-CEEF-4751-A377-C6B01BC1E06E"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(Relation_QueryX, _relationXStore, _queryXStore);
            AddIntegrityCheck(Relation_QueryX, _relationXStore, _queryXStore);

            AddIntegrityCheck(Relation_QueryX, _relationStore, _queryXStore);
            AddIntegrityCheck(Relation_QueryX, _relationStore, _queryXStore);


            ComputeX_QueryX = new RelationOf<ComputeX, QueryX>(_relationStore, Trait.ComputeX_QueryX, new Guid("3EFADD1B-28FF-4A84-90B1-A7948DB7FBBD"), Pairing.OneToMany, 25, 25, true);
            AddIntegrityCheck(ComputeX_QueryX, _computeXStore, _queryXStore);
        }
        /// <summary>
        /// Build dependency dictionaries used for maintaining relational integrity
        /// </summary>
        private void AddIntegrityCheck(Relation relation, Store parentStore, Store childStore)
        {
            if (!Store_ChildRelation.ContainsLink(parentStore, relation))
                Store_ChildRelation.SetLink(parentStore, relation);

            if (!Store_ParentRelation.ContainsLink(childStore, relation))
                Store_ParentRelation.SetLink(childStore, relation);
        }
        #endregion
    }
}
