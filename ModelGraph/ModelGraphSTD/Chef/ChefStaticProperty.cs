using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        private PropertyOf<Chef, bool> _showItemIndexProperty;

        private PropertyOf<ViewX, string> _viewXNameProperty;
        private PropertyOf<ViewX, string> _viewXSummaryProperty;

        private PropertyOf<EnumX, string> _enumXNameProperty;
        private PropertyOf<EnumX, string> _enumXSummaryProperty;

        private PropertyOf<PairX, string> _pairXTextProperty;
        private PropertyOf<PairX, string> _pairXValueProperty;

        private PropertyOf<TableX, string> _tableXNameProperty;
        private PropertyOf<TableX, string> _tableXSummaryProperty;

        private PropertyOf<ColumnX, string> _columnXNameProperty;
        private PropertyOf<ColumnX, string> _columnXSummaryProperty;
        private PropertyOf<ColumnX, string> _columnXTypeOfProperty;
        private PropertyOf<ColumnX, string> _columnXInitialProperty;
        private PropertyOf<ColumnX, bool> _columnXIsChoiceProperty;

        private PropertyOf<ComputeX, string> _computeXNameProperty;
        private PropertyOf<ComputeX, string> _computeXSummaryProperty;
        private PropertyOf<ComputeX, string> _computeXWhereProperty;
        private PropertyOf<ComputeX, string> _computeXSelectProperty;
        private PropertyOf<ComputeX, string> _computeXSeparatorProperty;
        private PropertyOf<ComputeX, string> _computeXCompuTypeProperty;
        private PropertyOf<ComputeX, string> _computeXNumericSetProperty;
        private PropertyOf<ComputeX, string> _computeXValueTypeProperty;
        private PropertyOf<ComputeX, string> _computeXResultsProperty;
        private PropertyOf<ComputeX, string> _computeXSortingProperty;
        private PropertyOf<ComputeX, string> _computeXTakeSetProperty;
        private PropertyOf<ComputeX, byte> _computeXTakeLimitProperty;

        private PropertyOf<RelationX, string> _relationXNameProperty;
        private PropertyOf<RelationX, string> _relationXSummaryProperty;
        private PropertyOf<RelationX, string> _relationXPairingProperty;
        private PropertyOf<RelationX, bool> _relationXIsRequiredProperty;

        private PropertyOf<GraphX, string> _graphXNameProperty;
        private PropertyOf<GraphX, string> _graphXSummaryProperty;

        private PropertyOf<SymbolX, string> _symbolXNameProperty;
        private PropertyOf<SymbolX, string> _symbolXAttachProperty;
        private PropertyOf<SymbolX, string> _symbolXTopContactProperty;
        private PropertyOf<SymbolX, string> _symbolXLeftContacttProperty;
        private PropertyOf<SymbolX, string> _symbolXRightContactProperty;
        private PropertyOf<SymbolX, string> _symbolXBottomContactProperty;

        private PropertyOf<QueryX, string> _queryXRootWhereProperty;
        private PropertyOf<QueryX, string> _queryXFacet1Property;
        private PropertyOf<QueryX, string> _queryXConnect1Property;
        private PropertyOf<QueryX, string> _queryXFacet2Property;
        private PropertyOf<QueryX, string> _queryXConnect2Property;
        private PropertyOf<QueryX, string> _queryXRelationProperty;
        private PropertyOf<QueryX, bool> _queryXIsReversedProperty;
        private PropertyOf<QueryX, bool> _queryXIsPersistentProperty;
        private PropertyOf<QueryX, bool> _queryXIsBreakPointProperty;
        private PropertyOf<QueryX, byte> _queryXExclusiveKeyProperty;
        private PropertyOf<QueryX, string> _queryXWhereProperty;
        private PropertyOf<QueryX, string> _queryXSelectProperty;
        private PropertyOf<QueryX, string> _queryXValueTypeProperty;
        private PropertyOf<QueryX, string> _queryXLineStyleProperty;
        private PropertyOf<QueryX, string> _queryXDashStyleProperty;
        private PropertyOf<QueryX, string> _queryXLineColorProperty;



        private PropertyOf<Node, int[]> _nodeCenterXYProperty;
        private PropertyOf<Node, int[]> _nodeSizeWHProperty;
        private PropertyOf<Node, string> _nodeLabelingProperty;
        private PropertyOf<Node, string> _nodeResizingProperty;
        private PropertyOf<Node, string> _nodeBarWidthProperty;
        private PropertyOf<Node, string> _nodeOrientationProperty;
        private PropertyOf<Node, string> _nodeFlipRotateProperty;

        private PropertyOf<Edge, string> _edgeFacet1Property;
        private PropertyOf<Edge, string> _edgeFacet2Property;
        private PropertyOf<Edge, string> _edgeConnect1Property;
        private PropertyOf<Edge, string> _edgeConnect2Property;

        private PropertyOf<GraphX, int> _graphXTerminalLengthProperty;
        private PropertyOf<GraphX, int> _graphXTerminalSpacingProperty;
        private PropertyOf<GraphX, int> _graphXTerminalStretchProperty;

        private void InitializeProperties()
        {
            var props = new List<Property>();

            #region Chef  =====================================================
            props.Clear();
            {
                {
                    var p = _showItemIndexProperty = new PropertyOf<Chef, bool>(PropertyStore, Trait.IncludeItemIdentityIndex_P);
                    p.GetValFunc = (item) => p.Cast(item).ShowItemIndex;
                    p.SetValFunc = (item, value) => p.Cast(item).ShowItemIndex = value;
                    p.Value = new BoolValue(p);
                    props.Add(p);
                }
            }
            #endregion

            #region ViewX  ====================================================
            props.Clear();
            {
                var p = _viewXNameProperty = new PropertyOf<ViewX, string>(PropertyStore, Trait.ViewName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _viewXSummaryProperty = new PropertyOf<ViewX, string>(PropertyStore, Trait.ViewSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(TableXStore, props);
            #endregion

            #region EnumX  ====================================================
            props.Clear();
            {
                var p = _enumXNameProperty = new PropertyOf<EnumX, string>(PropertyStore, Trait.EnumName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _enumXSummaryProperty = new PropertyOf<EnumX, string>(PropertyStore, Trait.EnumSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(EnumXStore, props);
            #endregion

            #region PairX  ====================================================
            {
                var p = _pairXTextProperty = new PropertyOf<PairX, string>(PropertyZStore, Trait.EnumText_P);
                p.GetValFunc = (item) => p.Cast(item).DisplayValue;
                p.SetValFunc = (item, value) => { p.Cast(item).DisplayValue = value; p.Owner.ChildDelta++; return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _pairXValueProperty = new PropertyOf<PairX, string>(PropertyZStore, Trait.EnumValue_P);
                p.GetValFunc = (item) => p.Cast(item).ActualValue;
                p.SetValFunc = (item, value) => { p.Cast(item).ActualValue = value; p.Owner.ChildDelta++; return true; };
                p.Value = new StringValue(p);
            }
            #endregion

            #region TableX  ===================================================
            props.Clear();
            {
                var p = _tableXNameProperty = new PropertyOf<TableX, string>(PropertyStore, Trait.TableName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _tableXSummaryProperty = new PropertyOf<TableX, string>(PropertyStore, Trait.TableSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(TableXStore, props);
            #endregion

            #region ColumnX  ==================================================
            props.Clear();
            {
                var p = _columnXNameProperty = new PropertyOf<ColumnX, string>(PropertyStore, Trait.ColumnName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _columnXSummaryProperty = new PropertyOf<ColumnX, string>(PropertyStore, Trait.ColumnSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _columnXTypeOfProperty = new PropertyOf<ColumnX, string>(PropertyStore, Trait.ColumnValueType_P, _valueTypeEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Value.ValType);
                p.SetValFunc = (item, value) => SetColumnValueType(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _columnXIsChoiceProperty = new PropertyOf<ColumnX, bool>(PropertyStore, Trait.ColumnIsChoice_P);
                p.GetValFunc = (item) => p.Cast(item).IsChoice;
                p.SetValFunc = (item, value) => p.Cast(item).IsChoice = value;
                p.Value = new BoolValue(p);
                props.Add(p);
            }
            {
                var p = _columnXInitialProperty = new PropertyOf<ColumnX, string>(PropertyStore, Trait.ColumnInitial_P);
                p.GetValFunc = (item) => p.Cast(item).Initial;
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(ColumnXStore, props);
            #endregion

            #region ComputeX  =================================================
            props.Clear();
            {
                var p = _computeXNameProperty = new PropertyOf<ComputeX, string>(PropertyStore, Trait.ComputeXName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXSummaryProperty = new PropertyOf<ComputeX, string>(PropertyStore, Trait.ComputeXSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXCompuTypeProperty = new PropertyOf<ComputeX, string>(PropertyStore, Trait.ComputeXCompuType_P, _computeTypeEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).CompuType);
                p.SetValFunc = (item, value) => TrySetComputeTypeProperty(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXNumericSetProperty = new PropertyOf<ComputeX, string>(PropertyStore, Trait.ComputeXNumericSet_P, _numericSetEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).NumericSet);
                p.SetValFunc = (item, value) => TrySetNumericSetProperty(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXWhereProperty = new PropertyOf<ComputeX, string>(PropertyStore, Trait.ComputeXWhere_P);
                p.GetValFunc = (item) => GetWhereProperty(p.Cast(item));
                p.SetValFunc = (item, value) => TrySetWhereProperty(p.Cast(item), value);
                p.Value = new StringValue(p);
                p.GetItemNameFunc = (item) => GetSelectorName(p.Cast(item));
                props.Add(p);
            }
            {
                var p = _computeXSelectProperty = new PropertyOf<ComputeX, string>(PropertyStore, Trait.ComputeXSelect_P);
                p.GetValFunc = (item) => GetSelectProperty(p.Cast(item));
                p.SetValFunc = (item, value) => TrySetSelectProperty(p.Cast(item), value);
                p.Value = new StringValue(p);
                p.GetItemNameFunc = (item) => { return GetSelectorName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _computeXSeparatorProperty = new PropertyOf<ComputeX, string>(PropertyStore, Trait.ComputeXSeparator_P);
                p.GetValFunc = (item) => p.Cast(item).Separator;
                p.SetValFunc = (item, value) => { p.Cast(item).Separator = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXValueTypeProperty = new PropertyOf<ComputeX, string>(PropertyStore, Trait.ComputeXValueType_P, _valueTypeEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Value.ValType);
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXResultsProperty = new PropertyOf<ComputeX, string>(PropertyStore, Trait.ComputeXResults_P, _computeResultsEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Results);
                p.SetValFunc = (item, value) => TrySetResultsProperty(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXSortingProperty = new PropertyOf<ComputeX, string>(PropertyStore, Trait.ComputeXSorting_P, _computeSortingEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Sorting);
                p.SetValFunc = (item, value) => TrySetSortingProperty(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXTakeSetProperty = new PropertyOf<ComputeX, string>(PropertyStore, Trait.ComputeXTakeSet_P, _computeTakeSetEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).TakeSet);
                p.SetValFunc = (item, value) => TrySetTakeSetProperty(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXTakeLimitProperty = new PropertyOf<ComputeX, byte>(PropertyStore, Trait.ComputeXTakeLimit_P);
                p.GetValFunc = (item) => p.Cast(item).TakeLimit;
                p.SetValFunc = (item, value) => { p.Cast(item).TakeLimit = (byte)value; return true; };
                p.Value = new ByteValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(ComputeXStore, props);
            #endregion

            #region RelationX  ================================================
            props.Clear();
            {
                var p = _relationXNameProperty = new PropertyOf<RelationX, string>(PropertyStore, Trait.RelationName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _relationXSummaryProperty = new PropertyOf<RelationX, string>(PropertyStore, Trait.RelationSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _relationXPairingProperty = new PropertyOf<RelationX, string>(PropertyStore, Trait.RelationPairing_P, _pairingEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Pairing);
                p.SetValFunc = (item, value) => p.Cast(item).TrySetPairing((Pairing)GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _relationXIsRequiredProperty = new PropertyOf<RelationX, bool>(PropertyStore, Trait.RelationIsRequired_P);
                p.GetValFunc = (item) => p.Cast(item).IsRequired;
                p.SetValFunc = (item, value) => { p.Cast(item).IsRequired = value; return true; };
                p.Value = new BoolValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(RelationXStore, props);
            #endregion


            #region GraphX  ===================================================
            props.Clear();
            {
                var p = _graphXNameProperty = new PropertyOf<GraphX, string>(PropertyStore, Trait.GraphName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _graphXSummaryProperty = new PropertyOf<GraphX, string>(PropertyStore, Trait.GraphSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _graphXTerminalLengthProperty = new PropertyOf<GraphX, int>(PropertyStore, Trait.GraphTerminalLength_P);
                p.GetValFunc = (item) => p.Cast(item).TerminalLength;
                p.SetValFunc = (item, value) => { p.Cast(item).TerminalLength = (byte)value; return true; };
                p.Value = new Int32Value(p);
                props.Add(p);
            }
            {
                var p = _graphXTerminalSpacingProperty = new PropertyOf<GraphX, int>(PropertyStore, Trait.GraphTerminalSpacing_P);
                p.GetValFunc = (item) => p.Cast(item).TerminalSpacing;
                p.SetValFunc = (item, value) => { p.Cast(item).TerminalSpacing = (byte)value; return true; };
                p.Value = new Int32Value(p);
                props.Add(p);
            }
            {
                var p = _graphXTerminalStretchProperty = new PropertyOf<GraphX, int>(PropertyStore, Trait.GraphTerminalStretch_P);
                p.GetValFunc = (item) => p.Cast(item).TerminalSkew;
                p.SetValFunc = (item, value) => { p.Cast(item).TerminalSkew = (byte)value; return true; };
                p.Value = new Int32Value(p);
                props.Add(p);
            }
            Store_Property.SetLink(GraphXStore, props);
            #endregion

            #region SymbolX  ==================================================
            props.Clear();
            {
                var p = _symbolXNameProperty = new PropertyOf<SymbolX, string>(PropertyStore, Trait.SymbolXName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _symbolXAttachProperty = new PropertyOf<SymbolX, string>(PropertyStore, Trait.SymbolXAttatch_P, _attatchEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Attach);
                p.SetValFunc = (item, value) => { p.Cast(item).Attach = (Attach)GetEnumZKey(p.EnumZ, value); return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _symbolXTopContactProperty = new PropertyOf<SymbolX, string>(PropertyStore, Trait.SymbolXTopContact_P, _contactEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).NorthContact);
                p.SetValFunc = (item, value) => TrySetSymbolTopContact(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _symbolXLeftContacttProperty = new PropertyOf<SymbolX, string>(PropertyStore, Trait.SymbolXLeftContactt_P, _contactEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).WestContact);
                p.SetValFunc = (item, value) => TrySetSymbolLeftContact(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _symbolXRightContactProperty = new PropertyOf<SymbolX, string>(PropertyStore, Trait.SymbolXRightContact_P, _contactEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).EastContact);
                p.SetValFunc = (item, value) => TrySetSymbolRightContact(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _symbolXBottomContactProperty = new PropertyOf<SymbolX, string>(PropertyStore, Trait.SymbolXBottomContact_P, _contactEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).SouthContact);
                p.SetValFunc = (item, value) => TrySetSymbolBottomContact(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(SymbolStore, props);
            #endregion

            #region QueryX  ===================================================
            props.Clear();
            {
                var p = _queryXRootWhereProperty = new PropertyOf<QueryX, string>(PropertyStore, Trait.QueryXSelect_P);
                p.GetValFunc = (item) => p.Cast(item).WhereString;
                p.SetValFunc = (item, value) => TrySetWhereProperty(p.Cast(item), value);
                p.Value = new StringValue(p);
                p.GetItemNameFunc = (item) => { return GetWhereName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _queryXFacet1Property = new PropertyOf<QueryX, string>(PropertyStore, Trait.QueryXFacet1_P, _facetEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).PathParm.Facet1);
                p.SetValFunc = (item, value) => { p.Cast(item).PathParm.Facet1 = (Facet)GetEnumZKey(p.EnumZ, value); return RefreshGraphX(p.Cast(item)); };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _queryXConnect1Property = new PropertyOf<QueryX, string>(PropertyStore, Trait.QueryXConnect1_P, _connectEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).PathParm.Connect1);
                p.SetValFunc = (item, value) => { p.Cast(item).PathParm.Connect1 = (Connect)GetEnumZKey(p.EnumZ, value); return RefreshGraphX(p.Cast(item)); ; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _queryXFacet2Property = new PropertyOf<QueryX, string>(PropertyStore, Trait.QueryXFacet2_P, _facetEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).PathParm.Facet2);
                p.SetValFunc = (item, value) => { p.Cast(item).PathParm.Facet2 = (Facet)GetEnumZKey(p.EnumZ, value); return RefreshGraphX(p.Cast(item)); ; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _queryXConnect2Property = new PropertyOf<QueryX, string>(PropertyStore, Trait.QueryXConnect2_P, _connectEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).PathParm.Connect2);
                p.SetValFunc = (item, value) => { p.Cast(item).PathParm.Connect2 = (Connect)GetEnumZKey(p.EnumZ, value); return RefreshGraphX(p.Cast(item)); ; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _queryXLineStyleProperty = new PropertyOf<QueryX, string>(PropertyStore, Trait.QueryXLineStyle_P, _lineStyleEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).PathParm.LineStyle);
                p.SetValFunc = (item, value) => { p.Cast(item).PathParm.LineStyle = (LineStyle)GetEnumZKey(p.EnumZ, value); return RefreshGraphX(p.Cast(item)); ; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _queryXDashStyleProperty = new PropertyOf<QueryX, string>(PropertyStore, Trait.QueryXDashStyle_P, _dashStyleEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).PathParm.DashStyle);
                p.SetValFunc = (item, value) => { p.Cast(item).PathParm.DashStyle = (DashStyle)GetEnumZKey(p.EnumZ, value); return RefreshGraphX(p.Cast(item)); ; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _queryXLineColorProperty = new PropertyOf<QueryX, string>(PropertyStore, Trait.QueryXLineColor_P);
                p.GetValFunc = (item) => p.Cast(item).PathParm.LineColor;
                p.SetValFunc = (item, value) => { p.Cast(item).PathParm.LineColor = value; return RefreshGraphX(p.Cast(item)); ; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _queryXRelationProperty = new PropertyOf<QueryX, string>(PropertyStore, Trait.QueryXRelation_P);
                p.GetValFunc = (item) => GetQueryXRelationName(p.Cast(item));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _queryXIsReversedProperty = new PropertyOf<QueryX, bool>(PropertyStore, Trait.QueryXIsReversed_P);
                p.GetValFunc = (item) => p.Cast(item).IsReversed;
                p.SetValFunc = (item, value) => { p.Cast(item).IsReversed = value; return true; };
                p.Value = new BoolValue(p);
                props.Add(p);
            }
            {
                var p = _queryXIsPersistentProperty = new PropertyOf<QueryX, bool>(PropertyStore, Trait.QueryXIsPersistent_P);
                p.GetValFunc = (item) => p.Cast(item).IsPersistent;
                p.SetValFunc = (item, value) => { p.Cast(item).IsPersistent = value; return true; };
                p.Value = new BoolValue(p);
                props.Add(p);
            }
            {
                var p = _queryXIsBreakPointProperty = new PropertyOf<QueryX, bool>(PropertyStore, Trait.QueryXIsBreakPoint_P);
                p.GetValFunc = (item) => p.Cast(item).IsBreakPoint;
                p.SetValFunc = (item, value) => { p.Cast(item).IsBreakPoint = value; return true; };
                p.Value = new BoolValue(p);
                props.Add(p);
            }
            {
                var p = _queryXExclusiveKeyProperty = new PropertyOf<QueryX, byte>(PropertyStore, Trait.QueryXExclusiveKey_P);
                p.GetValFunc = (item) => p.Cast(item).ExclusiveKey;
                p.SetValFunc = (item, value) => { p.Cast(item).ExclusiveKey = (byte)value; return true; };
                p.Value = new ByteValue(p);
                props.Add(p);
            }
            {
                var p = _queryXWhereProperty = new PropertyOf<QueryX, string>(PropertyStore, Trait.QueryXWhere_P);
                p.GetValFunc = (item) => p.Cast(item).WhereString;
                p.SetValFunc = (item, value) => TrySetWhereProperty(p.Cast(item), value);
                p.Value = new StringValue(p);
                p.GetItemNameFunc = (item) => { return GetWhereName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _queryXSelectProperty = new PropertyOf<QueryX, string>(PropertyStore, Trait.ValueXSelect_P);
                p.GetValFunc = (item) => p.Cast(item).SelectString;
                p.SetValFunc = (item, value) => TrySetSelectProperty(p.Cast(item), value);
                p.Value = new StringValue(p);
                p.GetItemNameFunc = (item) => { return GetSelectName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _queryXValueTypeProperty = new PropertyOf<QueryX, string>(PropertyStore, Trait.ValueXValueType_P, _valueTypeEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, GetValueType(p.Cast(item)));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(QueryXStore, props);
            #endregion


            #region Node  =====================================================
            {
                var p = _nodeCenterXYProperty = new PropertyOf<Node, int[]>(PropertyZStore, Trait.NodeCenterXY_P);
                p.GetValFunc = (item) => p.Cast(item).CenterXY;
                p.SetValFunc = (item, value) => { p.Cast(item).CenterXY = value; return true; };
                p.Value = new Int32ArrayValue(p);
            }
            {
                var p = _nodeSizeWHProperty = new PropertyOf<Node, int[]>(PropertyZStore, Trait.NodeSizeWH_P);
                p.GetValFunc = (item) => p.Cast(item).SizeWH;
                p.SetValFunc = (item, value) => { p.Cast(item).SizeWH = value; return true; };
                p.Value = new Int32ArrayValue(p);
            }
            {
                var p = _nodeOrientationProperty = new PropertyOf<Node, string>(PropertyZStore, Trait.NodeOrientation_P, _orientationEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Aspect);
                p.SetValFunc = (item, value) => { p.Cast(item).Aspect = (Aspect)GetEnumZKey(p.EnumZ, value); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _nodeFlipRotateProperty = new PropertyOf<Node, string>(PropertyZStore, Trait.NodeFlipRotate_P, _flipRotateEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).FlipRotate);
                p.SetValFunc = (item, value) => { p.Cast(item).FlipRotate = (FlipRotate)GetEnumZKey(p.EnumZ, value); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _nodeLabelingProperty = new PropertyOf<Node, string>(PropertyZStore, Trait.NodeLabeling_P, _labelingEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Labeling);
               p.SetValFunc = (item, value) => { p.Cast(item).Labeling = (Labeling)GetEnumZKey(p.EnumZ, value); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _nodeResizingProperty = new PropertyOf<Node, string>(PropertyZStore, Trait.NodeResizing_P, _resizingEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Sizing);
                p.SetValFunc = (item, value) => { p.Cast(item).Sizing = (Sizing)GetEnumZKey(p.EnumZ, value); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _nodeBarWidthProperty = new PropertyOf<Node, string>(PropertyZStore, Trait.NodeBarWidth_P, _barWidthEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).BarWidth);
                p.SetValFunc = (item, value) => { p.Cast(item).BarWidth = (BarWidth)GetEnumZKey(p.EnumZ, value); return true; };
                p.Value = new StringValue(p);
            }
            #endregion

            #region Edge  =====================================================
            {
                var p =  _edgeFacet1Property = new PropertyOf<Edge, string>(PropertyZStore, Trait.EdgeFacet1_P, _facetEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Facet1);
                p.SetValFunc = (item, value) => { p.Cast(item).Facet1 = (Facet)GetEnumZKey(p.EnumZ, value); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _edgeFacet2Property = new PropertyOf<Edge, string>(PropertyZStore, Trait.EdgeFacet2_P, _facetEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Facet2);
                p.SetValFunc = (item, value) => { p.Cast(item).Facet2 = (Facet)GetEnumZKey(p.EnumZ, value); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _edgeConnect1Property = new PropertyOf<Edge, string>(PropertyZStore, Trait.EdgeConnect1_P, _connectEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Connect1);
                p.Value = new StringValue(p);
            }
            {
                var p = _edgeConnect2Property = new PropertyOf<Edge, string>(PropertyZStore, Trait.EdgeConnect2_P, _connectEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Connect2);
                p.Value = new StringValue(p);
            }
            #endregion
        }

        #region ReleaseProperties  ============================================
        private void ReleaseProperties()
        {
            _showItemIndexProperty = null;

            _viewXNameProperty = null;
            _viewXSummaryProperty = null;

            _enumXNameProperty = null;
            _enumXSummaryProperty = null;

            _pairXTextProperty = null;
            _pairXValueProperty = null;

            _tableXNameProperty = null;
            _tableXSummaryProperty = null;

            _columnXNameProperty = null;
            _columnXSummaryProperty = null;
            _columnXTypeOfProperty = null;
            _columnXInitialProperty = null;
            _columnXIsChoiceProperty = null;

            _computeXNameProperty = null;
            _computeXSummaryProperty = null;
            _computeXWhereProperty = null;
            _computeXSelectProperty = null;
            _computeXSeparatorProperty = null;
            _computeXCompuTypeProperty = null;
            _computeXNumericSetProperty = null;
            _computeXValueTypeProperty = null;
            _computeXResultsProperty = null;
            _computeXSortingProperty = null;
            _computeXTakeSetProperty = null;
            _computeXTakeLimitProperty = null;

            _relationXNameProperty = null;
            _relationXSummaryProperty = null;
            _relationXPairingProperty = null;
            _relationXIsRequiredProperty = null;

            _graphXNameProperty = null;
            _graphXSummaryProperty = null;

            _symbolXNameProperty = null;
            _symbolXAttachProperty = null;
            _symbolXTopContactProperty = null;
            _symbolXLeftContacttProperty = null;
            _symbolXRightContactProperty = null;
            _symbolXBottomContactProperty = null;

            _queryXRootWhereProperty = null;
            _queryXFacet1Property = null;
            _queryXConnect1Property = null;
            _queryXFacet2Property = null;
            _queryXConnect2Property = null;
            _queryXRelationProperty = null;
            _queryXIsReversedProperty = null;
            _queryXIsPersistentProperty = null;
            _queryXIsBreakPointProperty = null;
            _queryXExclusiveKeyProperty = null;
            _queryXWhereProperty = null;
            _queryXSelectProperty = null;
            _queryXValueTypeProperty = null;
            _queryXLineStyleProperty = null;
            _queryXDashStyleProperty = null;
            _queryXLineColorProperty = null;

            _nodeCenterXYProperty = null;
            _nodeSizeWHProperty = null;
            _nodeLabelingProperty = null;
            _nodeResizingProperty = null;
            _nodeBarWidthProperty = null;
            _nodeOrientationProperty = null;
            _nodeFlipRotateProperty = null;

            _edgeFacet1Property = null;
            _edgeFacet2Property = null;
            _edgeConnect1Property = null;
            _edgeConnect2Property = null;

            _graphXTerminalLengthProperty = null;
            _graphXTerminalSpacingProperty = null;
            _graphXTerminalStretchProperty = null;
        }
        #endregion

        #region LookUpProperty  ===============================================
        static char[] _dotSplit = ".".ToCharArray();
        internal bool TryLookUpProperty(Store store, string name, out Property prop, out int index)
        {
            prop = null;
            index = (int)NumericTerm.None;

            if (string.IsNullOrWhiteSpace(name)) return false;

            if (store.IsTableX)
            {
                if (TableX_ColumnX.TryGetChildren(store, out IList<ColumnX> ls1))
                {
                    foreach (var col in ls1)
                    {
                        if (string.IsNullOrWhiteSpace(col.Name)) continue;
                        if (string.Compare(col.Name, name, true) == 0) { prop = col; return true; }
                    }
                }
                if (Store_ComputeX.TryGetChildren(store, out IList<ComputeX> ls2))
                {
                    foreach (var cd in ls2)
                    {
                        var n = cd.Name;
                        if (string.IsNullOrWhiteSpace(n)) continue;

                        if (cd.CompuType == CompuType.NumericValueSet)
                        {
                            var parts = name.Split(_dotSplit);
                            if (parts.Length == 2 && (string.Compare(parts[0], n, true) == 0))
                            {
                                if (string.Compare(parts[1], "COUNT") == 0) { prop = cd; index = (int)NumericTerm.Count; return true; }
                                if (string.Compare(parts[1], "MIN") == 0) { prop = cd; index = (int)NumericTerm.Min; return true; }
                                if (string.Compare(parts[1], "MAX") == 0) { prop = cd; index = (int)NumericTerm.Max; return true; }
                                if (string.Compare(parts[1], "SUM") == 0) { prop = cd; index = (int)NumericTerm.Sum; return true; }
                                if (string.Compare(parts[1], "AVE") == 0) { prop = cd; index = (int)NumericTerm.Ave; return true; }
                                if (string.Compare(parts[1], "STD") == 0) { prop = cd; index = (int)NumericTerm.Std; return true; }
                            }
                        }
                        else if (string.Compare(n, name, true) == 0) { prop = cd; return true; }
                    }
                }
            }
            else
            {
                if (Store_Property.TryGetChildren(store, out IList<Property> ls3))
                {
                    foreach (var pr in ls3)
                    {
                        if (string.Compare(name, _localize(pr.NameKey), true) == 0) { prop = pr; return true; }
                    }
                }
            }
            return false;
        }
        #endregion

        #region Helpers  ======================================================

        private string GetQueryXRelationName(Item item)
        {
            return Relation_QueryX.TryGetParent(item, out Relation rel) ? GetRelationName(rel as RelationX) : string.Empty;
        }
        #endregion
    }
}
