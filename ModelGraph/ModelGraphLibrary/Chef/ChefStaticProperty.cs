using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
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

        private PropertyOf<RelationX, string> _relationXNameProperty;
        private PropertyOf<RelationX, string> _relationXSummaryProperty;
        private PropertyOf<RelationX, string> _relationXPairingProperty;
        private PropertyOf<RelationX, bool> _relationXIsRequiredProperty;
        private PropertyOf<RelationX, bool> _relationXIsLimitedProperty;
        private PropertyOf<RelationX, short> _relationXMinOccuranceProperty;
        private PropertyOf<RelationX, short> _relationXMaxOccuranceProperty;

        private PropertyOf<GraphX, string> _graphXNameProperty;
        private PropertyOf<GraphX, string> _graphXSummaryProperty;

        private PropertyOf<SymbolX, string> _symbolXNameProperty;
        private PropertyOf<SymbolX, string> _symbolXTopContactProperty;
        private PropertyOf<SymbolX, string> _symbolXLeftContacttProperty;
        private PropertyOf<SymbolX, string> _symbolXRightContactProperty;
        private PropertyOf<SymbolX, string> _symbolXBottomContactProperty;

        private PropertyOf<QueryX, string> _queryXRootWhereProperty;
        private PropertyOf<QueryX, string> _queryXConnect1Property;
        private PropertyOf<QueryX, string> _queryXConnect2Property;
        private PropertyOf<QueryX, string> _queryXRelationProperty;
        private PropertyOf<QueryX, bool> _queryXIsReversedProperty;
        private PropertyOf<QueryX, bool> _queryXIsPersistentProperty;
        private PropertyOf<QueryX, bool> _queryXIsBreakPointProperty;
        private PropertyOf<QueryX, byte> _queryXExclusiveKeyProperty;
        private PropertyOf<QueryX, string> _queryXWhereProperty;
        private PropertyOf<QueryX, string> _queryXSelectProperty;
        private PropertyOf<QueryX, string> _queryXValueTypeProperty;



        private PropertyOf<Node, int[]> _nodeCenterXYProperty;
        private PropertyOf<Node, int[]> _nodeSizeWHProperty;
        private PropertyOf<Node, string> _nodeLabelingProperty;
        private PropertyOf<Node, string> _nodeResizingProperty;
        private PropertyOf<Node, string> _nodeBarWidthProperty;
        private PropertyOf<Node, string> _nodeOrientationProperty;
        private PropertyOf<Node, string> _nodeFlipRotateProperty;

        private PropertyOf<Edge, string> _edgeFace1Property;
        private PropertyOf<Edge, string> _edgeFace2Property;
        private PropertyOf<Edge, string> _edgeGnarl1Property;
        private PropertyOf<Edge, string> _edgeGnarl2Property;
        private PropertyOf<Edge, string> _edgeConnect1Property;
        private PropertyOf<Edge, string> _edgeConnect2Property;


        private void InitializeProperties()
        {
            var props = new List<Property>();

            #region ViewX  ====================================================
            props.Clear();
            {
                var p = _viewXNameProperty = new PropertyOf<ViewX, string>(_propertyStore, Trait.ViewName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _viewXSummaryProperty = new PropertyOf<ViewX, string>(_propertyStore, Trait.ViewSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(_tableXStore, props.ToArray());
            #endregion

            #region EnumX  ====================================================
            props.Clear();
            {
                var p = _enumXNameProperty = new PropertyOf<EnumX, string>(_propertyStore, Trait.EnumName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _enumXSummaryProperty = new PropertyOf<EnumX, string>(_propertyStore, Trait.EnumSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(_enumXStore, props.ToArray());
            #endregion

            #region PairX  ====================================================
            {
                var p = _pairXTextProperty = new PropertyOf<PairX, string>(_propertyZStore, Trait.EnumText_P);
                p.GetValFunc = (item) => p.Cast(item).DisplayValue;
                p.SetValFunc = (item, value) => { p.Cast(item).DisplayValue = value; return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _pairXValueProperty = new PropertyOf<PairX, string>(_propertyZStore, Trait.EnumValue_P);
                p.GetValFunc = (item) => p.Cast(item).ActualValue;
                p.SetValFunc = (item, value) => { p.Cast(item).ActualValue = value; return true; };
                p.Value = new StringValue(p);
            }
            #endregion

            #region TableX  ===================================================
            props.Clear();
            {
                var p = _tableXNameProperty = new PropertyOf<TableX, string>(_propertyStore, Trait.TableName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _tableXSummaryProperty = new PropertyOf<TableX, string>(_propertyStore, Trait.TableSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(_tableXStore, props.ToArray());
            #endregion

            #region ColumnX  ==================================================
            props.Clear();
            {
                var p = _columnXNameProperty = new PropertyOf<ColumnX, string>(_propertyStore, Trait.ColumnName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _columnXSummaryProperty = new PropertyOf<ColumnX, string>(_propertyStore, Trait.ColumnSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _columnXTypeOfProperty = new PropertyOf<ColumnX, string>(_propertyStore, Trait.ColumnValueType_P, _valueTypeEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Value.ValType);
                p.SetValFunc = (item, value) => SetColumnValueType(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _columnXIsChoiceProperty = new PropertyOf<ColumnX, bool>(_propertyStore, Trait.ColumnIsChoice_P);
                p.GetValFunc = (item) => p.Cast(item).IsChoice;
                p.SetValFunc = (item, value) => { return SetColumnIsChoice(p.Cast(item), value); };
                p.Value = new BoolValue(p);
                props.Add(p);
            }
            {
                var p = _columnXInitialProperty = new PropertyOf<ColumnX, string>(_propertyStore, Trait.ColumnInitial_P);
                p.GetValFunc = (item) => p.Cast(item).Initial;
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(_columnXStore, props.ToArray());
            #endregion

            #region ComputeX  =================================================
            props.Clear();
            {
                var p = _computeXNameProperty = new PropertyOf<ComputeX, string>(_propertyStore, Trait.ComputeXName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXSummaryProperty = new PropertyOf<ComputeX, string>(_propertyStore, Trait.ComputeXSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXCompuTypeProperty = new PropertyOf<ComputeX, string>(_propertyStore, Trait.ComputeXCompuType_P, _computeTypeEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).CompuType);
                p.SetValFunc = (item, value) => TrySetComputeTypeProperty(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXNumericSetProperty = new PropertyOf<ComputeX, string>(_propertyStore, Trait.ComputeXNumericSet_P, _numericSetEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).NumericSet);
                p.SetValFunc = (item, value) => TrySetNumericSetProperty(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXWhereProperty = new PropertyOf<ComputeX, string>(_propertyStore, Trait.ComputeXWhere_P);
                p.GetValFunc = (item) => GetWhereString(p.Cast(item));
                p.SetValFunc = (item, value) => SetWhereString(p.Cast(item), value);
                p.Value = new StringValue(p);
                p.GetItemNameFunc = (item) => GetSelectorName(p.Cast(item));
                props.Add(p);
            }
            {
                var p = _computeXSelectProperty = new PropertyOf<ComputeX, string>(_propertyStore, Trait.ComputeXSelect_P);
                p.GetValFunc = (item) => GetSelectString(p.Cast(item));
                p.SetValFunc = (item, value) => SetSelectString(p.Cast(item), value);
                p.Value = new StringValue(p);
                p.GetItemNameFunc = (item) => { return GetSelectorName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _computeXSeparatorProperty = new PropertyOf<ComputeX, string>(_propertyStore, Trait.ComputeXSeparator_P);
                p.GetValFunc = (item) => p.Cast(item).Separator;
                p.SetValFunc = (item, value) => { p.Cast(item).Separator = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _computeXValueTypeProperty = new PropertyOf<ComputeX, string>(_propertyStore, Trait.ComputeXValueType_P, _valueTypeEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Value.ValType);
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(_computeXStore, props.ToArray());
            #endregion

            #region RelationX  ================================================
            props.Clear();
            {
                var p = _relationXNameProperty = new PropertyOf<RelationX, string>(_propertyStore, Trait.RelationName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _relationXSummaryProperty = new PropertyOf<RelationX, string>(_propertyStore, Trait.RelationSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _relationXPairingProperty = new PropertyOf<RelationX, string>(_propertyStore, Trait.RelationPairing_P, _pairingEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Pairing);
                p.SetValFunc = (item, value) => p.Cast(item).TrySetPairing((Pairing)GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _relationXIsRequiredProperty = new PropertyOf<RelationX, bool>(_propertyStore, Trait.RelationIsRequired_P);
                p.GetValFunc = (item) => p.Cast(item).IsRequired;
                p.SetValFunc = (item, value) => { p.Cast(item).IsRequired = value; return true; };
                p.Value = new BoolValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(_relationXStore, props.ToArray());
            #endregion


            #region GraphX  ===================================================
            props.Clear();
            {
                var p = _graphXNameProperty = new PropertyOf<GraphX, string>(_propertyStore, Trait.GraphName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _graphXSummaryProperty = new PropertyOf<GraphX, string>(_propertyStore, Trait.GraphSummary_P);
                p.GetValFunc = (item) => p.Cast(item).Summary;
                p.SetValFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(_graphXStore, props.ToArray());
            #endregion

            #region SymbolX  ==================================================
            props.Clear();
            {
                var p = _symbolXNameProperty = new PropertyOf<SymbolX, string>(_propertyStore, Trait.SymbolName_P);
                p.GetValFunc = (item) => p.Cast(item).Name;
                p.SetValFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _symbolXTopContactProperty = new PropertyOf<SymbolX, string>(_propertyStore, Trait.SymbolTopContact_P, _contactEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).TopContact);
                p.SetValFunc = (item, value) => TrySetSymbolTopContact(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _symbolXLeftContacttProperty = new PropertyOf<SymbolX, string>(_propertyStore, Trait.SymbolLeftContactt_P, _contactEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).LeftContact);
                p.SetValFunc = (item, value) => TrySetSymbolLeftContact(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _symbolXRightContactProperty = new PropertyOf<SymbolX, string>(_propertyStore, Trait.SymbolRightContact_P, _contactEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).RightContact);
                p.SetValFunc = (item, value) => TrySetSymbolRightContact(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _symbolXBottomContactProperty = new PropertyOf<SymbolX, string>(_propertyStore, Trait.SymbolBottomContact_P, _contactEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).BottomContact);
                p.SetValFunc = (item, value) => TrySetSymbolBottomContact(p.Cast(item), GetEnumZKey(p.EnumZ, value));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(_symbolXStore, props.ToArray());
            #endregion

            #region QueryX  ===================================================
            props.Clear();
            {
                var p = _queryXRootWhereProperty = new PropertyOf<QueryX, string>(_propertyStore, Trait.QueryXFilter_P);
                p.GetValFunc = (item) => p.Cast(item).WhereString;
                p.SetValFunc = (item, value) => TrySetQueryXWhereProperty(p.Cast(item), value);
                p.Value = new StringValue(p);
                p.GetItemNameFunc = (item) => { return GetWhereName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _queryXConnect1Property = new PropertyOf<QueryX, string>(_propertyStore, Trait.QueryXConnect1_P, _connectEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Connect1);
                p.SetValFunc = (item, value) => { p.Cast(item).Connect1 = (Connect)GetEnumZKey(p.EnumZ, value); RefreshDrawing(p.Cast(item)); return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _queryXConnect2Property = new PropertyOf<QueryX, string>(_propertyStore, Trait.QueryXConnect2_P, _connectEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Connect2);
                p.SetValFunc = (item, value) => { p.Cast(item).Connect2 = (Connect)GetEnumZKey(p.EnumZ, value); RefreshDrawing(p.Cast(item)); return true; };
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _queryXRelationProperty = new PropertyOf<QueryX, string>(_propertyStore, Trait.QueryXRelation_P);
                p.GetValFunc = (item) => GetQueryXRelationName(p.Cast(item));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            {
                var p = _queryXIsReversedProperty = new PropertyOf<QueryX, bool>(_propertyStore, Trait.QueryXIsReversed_P);
                p.GetValFunc = (item) => p.Cast(item).IsReversed;
                p.SetValFunc = (item, value) => { p.Cast(item).IsReversed = value; return true; };
                p.Value = new BoolValue(p);
                props.Add(p);
            }
            {
                var p = _queryXIsPersistentProperty = new PropertyOf<QueryX, bool>(_propertyStore, Trait.QueryXIsPersistent_P);
                p.GetValFunc = (item) => p.Cast(item).IsPersistent;
                p.SetValFunc = (item, value) => { p.Cast(item).IsPersistent = value; return true; };
                p.Value = new BoolValue(p);
                props.Add(p);
            }
            {
                var p = _queryXIsBreakPointProperty = new PropertyOf<QueryX, bool>(_propertyStore, Trait.QueryXIsBreakPoint_P);
                p.GetValFunc = (item) => p.Cast(item).IsBreakPoint;
                p.SetValFunc = (item, value) => { p.Cast(item).IsBreakPoint = value; return true; };
                p.Value = new BoolValue(p);
                props.Add(p);
            }
            {
                var p = _queryXExclusiveKeyProperty = new PropertyOf<QueryX, byte>(_propertyStore, Trait.QueryXExclusiveKey_P);
                p.GetValFunc = (item) => p.Cast(item).ExclusiveKey;
                p.SetValFunc = (item, value) => { p.Cast(item).ExclusiveKey = (byte)value; return true; };
                p.Value = new ByteValue(p);
                props.Add(p);
            }
            {
                var p = _queryXWhereProperty = new PropertyOf<QueryX, string>(_propertyStore, Trait.QueryXWhere_P);
                p.GetValFunc = (item) => p.Cast(item).WhereString;
                p.SetValFunc = (item, value) => TrySetQueryXWhereProperty(p.Cast(item), value);
                p.Value = new StringValue(p);
                p.GetItemNameFunc = (item) => { return GetWhereName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _queryXSelectProperty = new PropertyOf<QueryX, string>(_propertyStore, Trait.ValueXSelect_P);
                p.GetValFunc = (item) => p.Cast(item).SelectString;
                p.SetValFunc = (item, value) => TrySetQueryXSelectProperty(p.Cast(item), value);
                p.Value = new StringValue(p);
                p.GetItemNameFunc = (item) => { return GetSelectName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _queryXValueTypeProperty = new PropertyOf<QueryX, string>(_propertyStore, Trait.ValueXValueType_P, _valueTypeEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, GetValueType(p.Cast(item)));
                p.Value = new StringValue(p);
                props.Add(p);
            }
            Store_Property.SetLink(_queryXStore, props.ToArray());
            #endregion


            #region Node  =====================================================
            {
                var p = _nodeCenterXYProperty = new PropertyOf<Node, int[]>(_propertyZStore, Trait.NodeCenterXY_P);
                p.GetValFunc = (item) => p.Cast(item).Core.CenterXY;
                p.SetValFunc = (item, value) => { p.Cast(item).Core.CenterXY = value; RefreshDrawing(p.Cast(item)); return true; };
                p.Value = new Int32ArrayValue(p);
            }
            {
                var p = _nodeSizeWHProperty = new PropertyOf<Node, int[]>(_propertyZStore, Trait.NodeSizeWH_P);
                p.GetValFunc = (item) => p.Cast(item).Core.SizeWH;
                p.SetValFunc = (item, value) => { p.Cast(item).Core.SizeWH = value; RefreshDrawing(p.Cast(item)); return true; };
                p.Value = new Int32ArrayValue(p);
            }
            {
                var p = _nodeOrientationProperty = new PropertyOf<Node, string>(_propertyZStore, Trait.NodeOrientation_P, _orientationEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Core.Orientation);
                p.SetValFunc = (item, value) => { p.Cast(item).Core.Orientation = (Orientation)GetEnumZKey(p.EnumZ, value); RefreshDrawing(p.Cast(item)); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _nodeFlipRotateProperty = new PropertyOf<Node, string>(_propertyZStore, Trait.NodeFlipRotate_P, _flipRotateEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Core.FlipRotate);
                p.SetValFunc = (item, value) => { p.Cast(item).Core.FlipRotate = (FlipRotate)GetEnumZKey(p.EnumZ, value); RefreshDrawing(p.Cast(item)); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _nodeLabelingProperty = new PropertyOf<Node, string>(_propertyZStore, Trait.NodeLabeling_P, _labelingEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Core.Labeling);
               p.SetValFunc = (item, value) => { p.Cast(item).Core.Labeling = (Labeling)GetEnumZKey(p.EnumZ, value); RefreshDrawing(p.Cast(item)); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _nodeResizingProperty = new PropertyOf<Node, string>(_propertyZStore, Trait.NodeResizing_P, _resizingEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Core.Resizing);
                p.SetValFunc = (item, value) => { p.Cast(item).Core.Resizing = (Resizing)GetEnumZKey(p.EnumZ, value); RefreshDrawing(p.Cast(item)); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _nodeBarWidthProperty = new PropertyOf<Node, string>(_propertyZStore, Trait.NodeBarWidth_P, _barWidthEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Core.BarWidth);
                p.SetValFunc = (item, value) => { p.Cast(item).Core.BarWidth = (BarWidth)GetEnumZKey(p.EnumZ, value); RefreshDrawing(p.Cast(item)); return true; };
                p.Value = new StringValue(p);
            }
            #endregion

            #region Edge  =====================================================
            {
                var p = _edgeFace1Property = new PropertyOf<Edge, string>(_propertyZStore, Trait.EdgeFace1_P, _sideEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Core.Face1.Side);
                p.SetValFunc = (item, value) => { p.Cast(item).Core.Face1.Side = (Side)GetEnumZKey(p.EnumZ, value); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _edgeFace2Property = new PropertyOf<Edge, string>(_propertyZStore, Trait.EdgeFace2_P, _sideEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Core.Face2.Side);
                p.SetValFunc = (item, value) => { p.Cast(item).Core.Face2.Side = (Side)GetEnumZKey(p.EnumZ, value); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p =  _edgeGnarl1Property = new PropertyOf<Edge, string>(_propertyZStore, Trait.EdgeGnarl1_P, _facetEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Core.Facet1);
                p.SetValFunc = (item, value) => { p.Cast(item).Core.Facet1 = (FacetOf)GetEnumZKey(p.EnumZ, value); RefreshDrawing(p.Cast(item)); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _edgeGnarl2Property = new PropertyOf<Edge, string>(_propertyZStore, Trait.EdgeGnarl1_P, _facetEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Core.Facet2);
                p.SetValFunc = (item, value) => { p.Cast(item).Core.Facet2 = (FacetOf)GetEnumZKey(p.EnumZ, value); RefreshDrawing(p.Cast(item)); return true; };
                p.Value = new StringValue(p);
            }
            {
                var p = _edgeConnect1Property = new PropertyOf<Edge, string>(_propertyZStore, Trait.EdgeConnect1_P, _connectEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Connect1);
                p.Value = new StringValue(p);
            }
            {
                var p = _edgeConnect2Property = new PropertyOf<Edge, string>(_propertyZStore, Trait.EdgeConnect2_P, _connectEnum);
                p.GetValFunc = (item) => GetEnumZName(p.EnumZ, (int)p.Cast(item).Connect2);
                p.Value = new StringValue(p);
            }
            #endregion
        }

        #region LookUpProperty  ===============================================
        static char[] _dotSplit = ".".ToCharArray();
        internal bool TryLookUpProperty(Store store, string name, out Property prop, out int index)
        {
            prop = null;
            index = (int)NumericTerm.None;

            if (string.IsNullOrWhiteSpace(name)) return false;

            if (store.IsTableX)
            {
                var cols = TableX_ColumnX.GetChildren(store);
                if (cols != null)
                {
                    foreach (var col in cols)
                    {
                        if (string.IsNullOrWhiteSpace(col.Name)) continue;
                        if (string.Compare(col.Name, name, true) == 0) { prop = col; return true; }
                    }
                }
                var cds = Store_ComputeX.GetChildren(store);
                if (cds != null)
                {
                    foreach (var cd in cds)
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
                var props = Store_Property.GetChildren(store);
                if (props != null)
                {
                    foreach (var pr in props)
                    {
                        if (string.Compare(name, _localize(pr.NameKey), true) == 0) { prop = pr; return true; }
                    }
                }
            }
            return false;
        }
        #endregion

        #region Helpers  ======================================================

        private void RefreshDrawing(Item item)
        {
            if (item.IsNode && item.Owner != null && item.Owner.IsGraph)
            {
                var node = item as Node;
                node.Graph.MinorDelta += 1;
            }
            else if (item.IsEdge && item.Owner != null && item.Owner.IsGraph)
            {
                var edge = item as Edge;
                edge.Graph.MinorDelta += 1;
            }
        }

        private string GetQueryXRelationName(Item item)
        {
            var rel = Relation_QueryX.GetParent(item);
            return (rel == null) ? string.Empty : GetRelationName(rel as RelationX);
        }
        #endregion
    }
}
