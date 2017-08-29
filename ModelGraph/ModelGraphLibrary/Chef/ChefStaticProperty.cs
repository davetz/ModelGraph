using System;
using System.Collections.Generic;

namespace ModelGraphLibrary
{/*

 */
    public partial class Chef
    {
        private PropertyOf<ViewX> _viewXNameProperty;
        private PropertyOf<ViewX> _viewXSummaryProperty;

        private PropertyOf<EnumX> _enumXNameProperty;
        private PropertyOf<EnumX> _enumXSummaryProperty;

        private PropertyOf<PairX> _pairXTextProperty;
        private PropertyOf<PairX> _pairXValueProperty;

        private PropertyOf<TableX> _tableXNameProperty;
        private PropertyOf<TableX> _tableXSummaryProperty;

        private PropertyOf<ColumnX> _columnXNameProperty;
        private PropertyOf<ColumnX> _columnXSummaryProperty;
        private PropertyOf<ColumnX> _columnXTypeOfProperty;
        private PropertyOf<ColumnX> _columnXDefaultProperty;
        private PropertyOf<ColumnX> _columnXIsChoiceProperty;

        private PropertyOf<ComputeX> _computeXNameProperty;
        private PropertyOf<ComputeX> _computeXSummaryProperty;
        private PropertyOf<ComputeX> _computeXWhereProperty;
        private PropertyOf<ComputeX> _computeXSelectProperty;
        private PropertyOf<ComputeX> _computeXSeparatorProperty;
        private PropertyOf<ComputeX> _computeXCompuTypeProperty;
        private PropertyOf<ComputeX> _computeXNumericSetProperty;
        private PropertyOf<ComputeX> _computeXNativeTypeProperty;

        private PropertyOf<RelationX> _relationXNameProperty;
        private PropertyOf<RelationX> _relationXSummaryProperty;
        private PropertyOf<RelationX> _relationXPairingProperty;
        private PropertyOf<RelationX> _relationXIsRequiredProperty;
        private PropertyOf<RelationX> _relationXIsLimitedProperty;
        private PropertyOf<RelationX> _relationXMinOccuranceProperty;
        private PropertyOf<RelationX> _relationXMaxOccuranceProperty;

        private PropertyOf<GraphX> _graphXNameProperty;
        private PropertyOf<GraphX> _graphXSummaryProperty;

        private PropertyOf<SymbolX> _symbolXNameProperty;
        private PropertyOf<SymbolX> _symbolXTopContactProperty;
        private PropertyOf<SymbolX> _symbolXLeftContacttProperty;
        private PropertyOf<SymbolX> _symbolXRightContactProperty;
        private PropertyOf<SymbolX> _symbolXBottomContactProperty;

        private PropertyOf<QueryX> _queryXRootWhereProperty;
        private PropertyOf<QueryX> _queryXConnect1Property;
        private PropertyOf<QueryX> _queryXConnect2Property;
        private PropertyOf<QueryX> _queryXRelationProperty;
        private PropertyOf<QueryX> _queryXIsReversedProperty;
        private PropertyOf<QueryX> _queryXIsPersistentProperty;
        private PropertyOf<QueryX> _queryXIsBreakPointProperty;
        private PropertyOf<QueryX> _queryXExclusiveKeyProperty;
        private PropertyOf<QueryX> _queryXWhereProperty;
        private PropertyOf<QueryX> _queryXSelectProperty;
        private PropertyOf<QueryX> _queryXNativeTypeProperty;



        private PropertyOf<Node> _nodeCenterXYProperty;
        private PropertyOf<Node> _nodeSizeWHProperty;
        private PropertyOf<Node> _nodeLabelingProperty;
        private PropertyOf<Node> _nodeResizingProperty;
        private PropertyOf<Node> _nodeBarWidthProperty;
        private PropertyOf<Node> _nodeOrientationProperty;
        private PropertyOf<Node> _nodeFlipRotateProperty;

        private PropertyOf<Edge> _edgeFace1Property;
        private PropertyOf<Edge> _edgeFace2Property;
        private PropertyOf<Edge> _edgeGnarl1Property;
        private PropertyOf<Edge> _edgeGnarl2Property;
        private PropertyOf<Edge> _edgeConnect1Property;
        private PropertyOf<Edge> _edgeConnect2Property;


        private void InitializeProperties()
        {
            var props = new List<Property>();

            #region ViewX  ==================================================
            props.Clear();
            {
                var p = _viewXNameProperty = new PropertyOf<ViewX>(_propertyStore, Trait.ViewName_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Name; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                props.Add(p);
            }
            {
                var p = _viewXSummaryProperty = new PropertyOf<ViewX>(_propertyStore, Trait.ViewSummary_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Summary; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                props.Add(p);
            }
            Store_Property.SetLink(_tableXStore, props.ToArray());
            #endregion

            #region EnumX  ====================================================
            props.Clear();
            {
                var p = _enumXNameProperty = new PropertyOf<EnumX>(_propertyStore, Trait.EnumName_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Name; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                props.Add(p);
            }
            {
                var p = _enumXSummaryProperty = new PropertyOf<EnumX>(_propertyStore, Trait.EnumSummary_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Summary; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                props.Add(p);
            }
            Store_Property.SetLink(_enumXStore, props.ToArray());
            #endregion

            #region PairX  ====================================================
            {
                var p = _pairXTextProperty = new PropertyOf<PairX>(_propertyZStore, Trait.EnumText_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).DisplayValue; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).DisplayValue = value; return true; };
            }
            {
                var p = _pairXValueProperty = new PropertyOf<PairX>(_propertyZStore, Trait.EnumValue_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).ActualValue; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).ActualValue = value; return true; };
            }
            #endregion

            #region TableX  ===================================================
            props.Clear();
            {
                var p = _tableXNameProperty = new PropertyOf<TableX>(_propertyStore, Trait.TableName_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Name; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                props.Add(p);
            }
            {
                var p = _tableXSummaryProperty = new PropertyOf<TableX>(_propertyStore, Trait.TableSummary_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Summary; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                props.Add(p);
            }
            Store_Property.SetLink(_tableXStore, props.ToArray());
            #endregion

            #region ColumnX  ==================================================
            props.Clear();
            {
                var p = _columnXNameProperty = new PropertyOf<ColumnX>(_propertyStore, Trait.ColumnName_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Name; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                props.Add(p);
            }
            {
                var p = _columnXSummaryProperty = new PropertyOf<ColumnX>(_propertyStore, Trait.ColumnSummary_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Summary; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                props.Add(p);
            }
            {
                var p = _columnXTypeOfProperty = new PropertyOf<ColumnX>(_propertyStore, Trait.ColumnValueType_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_valueTypeEnum, (int)p.Cast(item).ValueType); };
                p.TrySetValueFunc = (item, value) => { return TrySetValueType(p.Cast(item), value); };
                props.Add(p);
            }
            {
                var p = _columnXIsChoiceProperty = new PropertyOf<ColumnX>(_propertyStore, Trait.ColumnIsChoice_P, ValueType.Bool);
                p.GetValueFunc = (item) => { return p.Cast(item).IsChoice.ToString(); };
                p.TrySetValueFunc = (item, value) => { return TrySetColumnIsChoice(p.Cast(item), value); };
                props.Add(p);
            }
            {
                var p = _columnXDefaultProperty = new PropertyOf<ColumnX>(_propertyStore, Trait.ColumnDefault_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Default; };
                props.Add(p);
            }
            Store_Property.SetLink(_columnXStore, props.ToArray());
            #endregion

            #region ComputeX  =================================================
            props.Clear();
            {
                var p = _computeXNameProperty = new PropertyOf<ComputeX>(_propertyStore, Trait.ComputeXName_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Name; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                props.Add(p);
            }
            {
                var p = _computeXSummaryProperty = new PropertyOf<ComputeX>(_propertyStore, Trait.ComputeXSummary_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Summary; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                props.Add(p);
            }
            {
                var p = _computeXCompuTypeProperty = new PropertyOf<ComputeX>(_propertyStore, Trait.ComputeXCompuType_P, ValueType.String);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_computeTypeEnum, (int)p.Cast(item).CompuType); };
                p.TrySetValueFunc = (item, value) => { MajorDelta += 1; return TrySetComputeTypeProperty(p.Cast(item), value); };
                props.Add(p);
            }
            {
                var p = _computeXNumericSetProperty = new PropertyOf<ComputeX>(_propertyStore, Trait.ComputeXNumericSet_P, ValueType.String);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_numericSetEnum, (int)p.Cast(item).NumericSet); };
                p.TrySetValueFunc = (item, value) => { return TrySetNumericSetProperty(p.Cast(item), value); };
                props.Add(p);
            }
            {
                var p = _computeXWhereProperty = new PropertyOf<ComputeX>(_propertyStore, Trait.ComputeXWhere_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).SelectString; };
                p.TrySetValueFunc = (item, value) => { return SetComputeXWhere(p.Cast(item), value); };
                p.GetItemNameFunc = (item) => { return GetSelectorName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _computeXSelectProperty = new PropertyOf<ComputeX>(_propertyStore, Trait.ComputeXSelect_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).SelectString; };
                p.TrySetValueFunc = (item, value) => { return SetComputeXSelect(p.Cast(item), value); };
                p.GetItemNameFunc = (item) => { return GetSelectorName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _computeXSeparatorProperty = new PropertyOf<ComputeX>(_propertyStore, Trait.ComputeXSeparator_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Separator; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Separator = value; return true; };
                props.Add(p);
            }
            {
                var p = _computeXNativeTypeProperty = new PropertyOf<ComputeX>(_propertyStore, Trait.ComputeXNativeType_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).NativeType.ToString(); };
                props.Add(p);
            }
            Store_Property.SetLink(_computeXStore, props.ToArray());
            #endregion

            #region RelationX  ================================================
            props.Clear();
            {
                var p = _relationXNameProperty = new PropertyOf<RelationX>(_propertyStore, Trait.RelationName_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Name; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                props.Add(p);
            }
            {
                var p = _relationXSummaryProperty = new PropertyOf<RelationX>(_propertyStore, Trait.RelationSummary_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Summary; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                props.Add(p);
            }
            {
                var p = _relationXPairingProperty = new PropertyOf<RelationX>(_propertyStore, Trait.RelationPairing_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_pairingEnum, (int)p.Cast(item).Pairing); };
                p.TrySetValueFunc = (item, value) => { return TrySetPairing(p.Cast(item), value); };
                props.Add(p);
            }
            {
                var p = _relationXIsRequiredProperty = new PropertyOf<RelationX>(_propertyStore, Trait.RelationIsRequired_P, ValueType.Bool);
                p.GetValueFunc = (item) => { return p.Cast(item).IsRequired.ToString(); };
                p.TrySetValueFunc = (item, value) => { return TrySetRelationIsRequired(p.Cast(item), value); };
                props.Add(p);
            }
            {
                var p = _relationXIsLimitedProperty = new PropertyOf<RelationX>(_propertyStore, Trait.RelationIsReference_P, ValueType.Bool);
                p.GetValueFunc = (item) => { return p.Cast(item).IsLimited.ToString(); };
                p.TrySetValueFunc = (item, value) => { return TrySetRelationIsLimited(p.Cast(item), value); };
                props.Add(p);
            }
            {
                var p = _relationXMinOccuranceProperty = new PropertyOf<RelationX>(_propertyStore, Trait.RelationMinOccurance_P, ValueType.Int16);
                p.GetValueFunc = (item) => { return p.Cast(item).MinOccurance.ToString(); };
                p.TrySetValueFunc = (item, value) => { return Value.TryParse(value, out p.Cast(item).MinOccurance); };
                props.Add(p);
            }
            {
                var p = _relationXMaxOccuranceProperty = new PropertyOf<RelationX>(_propertyStore, Trait.RelationMaxOccurance_P, ValueType.Int16);
                p.GetValueFunc = (item) => { return p.Cast(item).MaxOccurance.ToString(); };
                p.TrySetValueFunc = (item, value) => { return Value.TryParse(value, out p.Cast(item).MaxOccurance); };
                props.Add(p);
            }
            Store_Property.SetLink(_relationXStore, props.ToArray());
            #endregion


            #region GraphX  ===================================================
            props.Clear();
            {
                var p = _graphXNameProperty = new PropertyOf<GraphX>(_propertyStore, Trait.GraphName_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Name; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                props.Add(p);
            }
            {
                var p = _graphXSummaryProperty = new PropertyOf<GraphX>(_propertyStore, Trait.GraphSummary_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Summary; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Summary = value; return true; };
                props.Add(p);
            }
            Store_Property.SetLink(_graphXStore, props.ToArray());
            #endregion

            #region SymbolX  ==================================================
            props.Clear();
            {
                var p = _symbolXNameProperty = new PropertyOf<SymbolX>(_propertyStore, Trait.SymbolName_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).Name; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).Name = value; return true; };
                props.Add(p);
            }
            {
                var p = _symbolXTopContactProperty = new PropertyOf<SymbolX>(_propertyStore, Trait.SymbolTopContact_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_contactEnum, (int)p.Cast(item).TopContact); };
                p.TrySetValueFunc = (item, value) => { Contact cval; if (Enum.TryParse(value, out cval)) { p.Cast(item).TopContact = cval; RefreshDrawing(p.Cast(item)); return true; } return false; };
                props.Add(p);
            }
            {
                var p = _symbolXLeftContacttProperty = new PropertyOf<SymbolX>(_propertyStore, Trait.SymbolLeftContactt_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_contactEnum, (int)p.Cast(item).LeftContact); };
                p.TrySetValueFunc = (item, value) => { Contact cval; if (Enum.TryParse(value, out cval)) { p.Cast(item).LeftContact = cval; RefreshDrawing(p.Cast(item)); return true; } return false; };
                props.Add(p);
            }
            {
                var p = _symbolXRightContactProperty = new PropertyOf<SymbolX>(_propertyStore, Trait.SymbolRightContact_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_contactEnum, (int)p.Cast(item).RightContact); };
                p.TrySetValueFunc = (item, value) => { Contact cval; if (Enum.TryParse(value, out cval)) { p.Cast(item).RightContact = cval; RefreshDrawing(p.Cast(item)); return true; } return false; };
                props.Add(p);
            }
            {
                var p = _symbolXBottomContactProperty = new PropertyOf<SymbolX>(_propertyStore, Trait.SymbolBottomContact_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_contactEnum, (int)p.Cast(item).BottomContact); };
                p.TrySetValueFunc = (item, value) => { Contact cval; if (Enum.TryParse(value, out cval)) { p.Cast(item).BottomContact = cval; RefreshDrawing(p.Cast(item)); return true; } return false; };
                props.Add(p);
            }
            Store_Property.SetLink(_symbolXStore, props.ToArray());
            #endregion

            #region QueryX  ===================================================
            props.Clear();
            {
                var p = _queryXRootWhereProperty = new PropertyOf<QueryX>(_propertyStore, Trait.QueryXFilter_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).WhereString; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).WhereString = value; return ValidateQueryX(p.Cast(item)); };
                p.GetItemNameFunc = (item) => { return GetWhereName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _queryXConnect1Property = new PropertyOf<QueryX>(_propertyStore, Trait.QueryXConnect1_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_connectEnum, (int)p.Cast(item).Connect1); };
                p.TrySetValueFunc = (item, value) => { Connect cval; if (Enum.TryParse(value, out cval)) { p.Cast(item).Connect1 = cval; RefreshDrawing(p.Cast(item)); return true; } return false; };
                props.Add(p);
            }
            {
                var p = _queryXConnect2Property = new PropertyOf<QueryX>(_propertyStore, Trait.QueryXConnect2_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_connectEnum, (int)p.Cast(item).Connect2); };
                p.TrySetValueFunc = (item, value) => { Connect cval; if (Enum.TryParse(value, out cval)) { p.Cast(item).Connect2 = cval; return true; } return false; };
                props.Add(p);
            }
            {
                var p = _queryXRelationProperty = new PropertyOf<QueryX>(_propertyStore, Trait.QueryXRelation_P, ValueType.String);
                p.GetValueFunc = (item) =>{ return GetQueryXRelationName(p.Cast(item)); } ;
                p.TrySetValueFunc = (item, value) => { return true; };
                props.Add(p);
            }
            {
                var p = _queryXIsReversedProperty = new PropertyOf<QueryX>(_propertyStore, Trait.QueryXIsReversed_P, ValueType.Bool);
                p.GetValueFunc = (item) => { return p.Cast(item).IsReversed.ToString(); };
                p.TrySetValueFunc = (item, value) => { Value.TryParse(value, out bool val); p.Cast(item).IsReversed = val; return true; };
                props.Add(p);
            }
            {
                var p = _queryXIsPersistentProperty = new PropertyOf<QueryX>(_propertyStore, Trait.QueryXIsPersistent_P, ValueType.Bool);
                p.GetValueFunc = (item) => { return p.Cast(item).IsPersistent.ToString(); };
                p.TrySetValueFunc = (item, value) => { Value.TryParse(value, out bool val); p.Cast(item).IsPersistent = val; return true; };
                props.Add(p);
            }
            {
                var p = _queryXIsBreakPointProperty = new PropertyOf<QueryX>(_propertyStore, Trait.QueryXIsBreakPoint_P, ValueType.Bool);
                p.GetValueFunc = (item) => { return p.Cast(item).IsBreakPoint.ToString(); };
                p.TrySetValueFunc = (item, value) => { Value.TryParse(value, out bool val); p.Cast(item).IsBreakPoint = val; return true; };
                props.Add(p);
            }
            {
                var p = _queryXExclusiveKeyProperty = new PropertyOf<QueryX>(_propertyStore, Trait.QueryXExclusiveKey_P, ValueType.Byte);
                p.GetValueFunc = (item) => { return p.Cast(item).ExclusiveKey.ToString(); };
                p.TrySetValueFunc = (item, value) => { Value.TryParse(value, out byte val); p.Cast(item).ExclusiveKey = val; return true; };
                props.Add(p);
            }
            {
                var p = _queryXWhereProperty = new PropertyOf<QueryX>(_propertyStore, Trait.QueryXWhere_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).WhereString; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).WhereString = value; return ValidateQueryX(p.Cast(item)); };
                p.GetItemNameFunc = (item) => { return GetWhereName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _queryXSelectProperty = new PropertyOf<QueryX>(_propertyStore, Trait.ValueXSelect_P, ValueType.String);
                p.GetValueFunc = (item) => { return p.Cast(item).SelectString; };
                p.TrySetValueFunc = (item, value) => { p.Cast(item).SelectString = value; ValidateValueXChange(p.Cast(item)); return true; };
                p.GetItemNameFunc = (item) => { return GetSelectName(p.Cast(item)); };
                props.Add(p);
            }
            {
                var p = _queryXNativeTypeProperty = new PropertyOf<QueryX>(_propertyStore, Trait.ValueXNativeType_P, ValueType.String);
                p.GetValueFunc = (item) => { return GetNativeType(p.Cast(item)); };
                props.Add(p);
            }
            Store_Property.SetLink(_queryXStore, props.ToArray());
            #endregion


            #region Node  =====================================================
            {
                var p = _nodeCenterXYProperty = new PropertyOf<Node>(_propertyZStore, Trait.NodeCenterXY_P, ValueType.DoubleArray);
                p.GetValueFunc = (item) => { return Value.ToString(p.Cast(item).Core.CenterXY); };
                p.TrySetValueFunc = (item, value) => { int[] v; if (Value.TryParse(value, out v)) { p.Cast(item).Core.CenterXY = v; RefreshDrawing(p.Cast(item)); return true; } return false; };
            }
            {
                var p = _nodeSizeWHProperty = new PropertyOf<Node>(_propertyZStore, Trait.NodeSizeWH_P, ValueType.DoubleArray);
                p.GetValueFunc = (item) => { return Value.ToString(p.Cast(item).Core.SizeWH); };
                p.TrySetValueFunc = (item, value) => { int[] v; if (Value.TryParse(value, out v)) { p.Cast(item).Core.SizeWH = v; RefreshDrawing(p.Cast(item)); return true; } return false; };
            }
            {
                var p = _nodeOrientationProperty = new PropertyOf<Node>(_propertyZStore, Trait.NodeOrientation_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_orientationEnum, (int)p.Cast(item).Core.Orientation); };
                p.TrySetValueFunc = (item, value) => { Orientation v; if (Enum.TryParse(value, out v)) { p.Cast(item).Core.Orientation = v; RefreshDrawing(p.Cast(item)); return true; } return false; };
            }
            {
                var p = _nodeFlipRotateProperty = new PropertyOf<Node>(_propertyZStore, Trait.NodeFlipRotate_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_flipRotateEnum, (int)p.Cast(item).Core.FlipRotate); };
                p.TrySetValueFunc = (item, value) => { FlipRotate v; if (Enum.TryParse(value, out v)) { p.Cast(item).Core.FlipRotate = v; RefreshDrawing(p.Cast(item)); return true; } return false; };
            }
            {
                var p = _nodeLabelingProperty = new PropertyOf<Node>(_propertyZStore, Trait.NodeLabeling_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_labelingEnum, (int)p.Cast(item).Core.Labeling); };
               p.TrySetValueFunc = (item, value) => { Labeling v; if (Enum.TryParse(value, out v)) { p.Cast(item).Core.Labeling = v; RefreshDrawing(p.Cast(item)); return true; } return false; };
            }
            {
                var p = _nodeResizingProperty = new PropertyOf<Node>(_propertyZStore, Trait.NodeResizing_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_resizingEnum, (int)p.Cast(item).Core.Resizing); };
                p.TrySetValueFunc = (item, value) => { Resizing v; if (Enum.TryParse(value, out v)) { p.Cast(item).Core.Resizing = v; RefreshDrawing(p.Cast(item)); return true; } return false; };
            }
            {
                var p = _nodeBarWidthProperty = new PropertyOf<Node>(_propertyZStore, Trait.NodeBarWidth_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_barWidthEnum, (int)p.Cast(item).Core.BarWidth); };
                p.TrySetValueFunc = (item, value) => { BarWidth v; if (Enum.TryParse(value, out v)) { p.Cast(item).Core.BarWidth = v; RefreshDrawing(p.Cast(item)); return true; } return false; };
            }
            #endregion

            #region Edge  =====================================================
            {
                var p = _edgeFace1Property = new PropertyOf<Edge>(_propertyZStore, Trait.EdgeFace1_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_sideEnum, (int)p.Cast(item).Core.Face1.Side); };
                p.TrySetValueFunc = (item, value) => { Side v; if (Enum.TryParse(value, out v)) { p.Cast(item).Core.Face1.Side = v; RefreshDrawing(p.Cast(item)); return true; } return false; };
            }
            {
                var p = _edgeFace2Property = new PropertyOf<Edge>(_propertyZStore, Trait.EdgeFace2_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_sideEnum, (int)p.Cast(item).Core.Face2.Side); };
                p.TrySetValueFunc = (item, value) => { Side v; if (Enum.TryParse(value, out v)) { p.Cast(item).Core.Face2.Side = v; RefreshDrawing(p.Cast(item)); return true; } return false; };
            }
            {
                var p =  _edgeGnarl1Property = new PropertyOf<Edge>(_propertyZStore, Trait.EdgeGnarl1_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_facetEnum, (int)p.Cast(item).Core.Facet1); };
                p.TrySetValueFunc = (item, value) => { FacetOf v; if (Enum.TryParse(value, out v)) { p.Cast(item).Core.Facet1 = v; RefreshDrawing(p.Cast(item)); return true; } return false; };
            }
            {
                var p = _edgeGnarl2Property = new PropertyOf<Edge>(_propertyZStore, Trait.EdgeGnarl1_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_facetEnum, (int)p.Cast(item).Core.Facet2); };
                p.TrySetValueFunc = (item, value) => { FacetOf v; if (Enum.TryParse(value, out v)) { p.Cast(item).Core.Facet2 = v; RefreshDrawing(p.Cast(item)); return true; } return false; };
            }
            {
                var p = _edgeConnect1Property = new PropertyOf<Edge>(_propertyZStore, Trait.EdgeConnect1_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_connectEnum, (int)p.Cast(item).Connect1); };
            }
            {
                var p = _edgeConnect2Property = new PropertyOf<Edge>(_propertyZStore, Trait.EdgeConnect2_P, ValueType.InternalEnum);
                p.GetValueFunc = (item) => { return GetEnumDisplayValue(_connectEnum, (int)p.Cast(item).Connect2); };
            }
            #endregion
        }

        #region LookUpProperty  ===============================================
        static char[] _dotSplit = ".".ToCharArray();
        internal bool TryLookUpProperty(Store store, string name, out Property prop, out NumericTerm term)
        {
            prop = null;
            term = NumericTerm.None;

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
                                if (string.Compare(parts[1], "COUNT") == 0) { prop = cd; term = NumericTerm.Count; return true; }
                                if (string.Compare(parts[1], "MIN") == 0) { prop = cd; term = NumericTerm.Min; return true; }
                                if (string.Compare(parts[1], "MAX") == 0) { prop = cd; term = NumericTerm.Max; return true; }
                                if (string.Compare(parts[1], "SUM") == 0) { prop = cd; term = NumericTerm.Sum; return true; }
                                if (string.Compare(parts[1], "AVE") == 0) { prop = cd; term = NumericTerm.Ave; return true; }
                                if (string.Compare(parts[1], "STD") == 0) { prop = cd; term = NumericTerm.Std; return true; }
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
                        if (string.Compare(name, _resourceLoader.GetString(pr.NameKey), true) == 0) { prop = pr; return true; }
                    }
                }
            }
            return false;
        }
        #endregion

        #region Helpers  ======================================================
        private bool TrySetValueType(ColumnX col, string value)
        {
            if (Enum.TryParse<ValueType>(value, out ValueType type))
            {
                var tbl = TableX_ColumnX.GetParent(col);
                if (tbl != null) return col.TrySetValueType(tbl.Items, type);
            }
            return false;
        }
        private bool TrySetPairing(RelationX rel, string value)
        {
            if (Enum.TryParse<Pairing>(value, out Pairing type))
            {
                return rel.TrySetPairing(type);                
            }
            return false;
        }

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
        private bool TrySetColumnIsChoice(Item item, string value)
        {
            bool val;
            var isValid = Value.TryParse(value, out val);

            if (isValid)
            {
                var col = item as ColumnX;
                col.IsChoice = val;

                var tbl = TableX_ColumnX.GetParent(col);
                if (tbl != null) ValidateTableChoiceColumns(tbl);
            }
            return isValid;
        }
        private bool TrySetRelationIsLimited(Item item, string value)
        {
            bool val;
            var isValid = Value.TryParse(value, out val);

            if (isValid)
            {
                var rel = item as RelationX;
                rel.IsLimited = val;
            }
            return isValid;
        }
        private bool TrySetRelationIsRequired(Item item, string value)
        {
            bool val;
            var isValid = Value.TryParse(value, out val);

            if (isValid)
            {
                var rel = item as RelationX;
                rel.IsRequired = val;
            }
            return isValid;
        }
        private bool TrySetQueryXIsReversed(Item item, string value)
        {
            bool val;
            var isValid = Value.TryParse(value, out val);

            if (isValid)
            {
                var itm = item as QueryX;
                itm.IsReversed = val;
            }
            return isValid;
        }
        #endregion
    }
}
