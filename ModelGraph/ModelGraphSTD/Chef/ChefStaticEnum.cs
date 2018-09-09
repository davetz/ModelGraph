namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        EnumZ _valueTypeEnum;
        EnumZ _pairingEnum;

        EnumZ _orientationEnum;
        EnumZ _labelingEnum;
        EnumZ _flipRotateEnum;
        EnumZ _resizingEnum;
        EnumZ _barWidthEnum;

        EnumZ _sideEnum;
        EnumZ _facetEnum;
        EnumZ _connectEnum;
        EnumZ _contactEnum;
        EnumZ _numericSetEnum;
        EnumZ _numericTermEnum;
        EnumZ _computeTypeEnum;
        EnumZ _attatchEnum;
        EnumZ _lineStyleEnum;
        EnumZ _dashStyleEnum;
        EnumZ _computeResultsEnum;
        EnumZ _computeSortingEnum;
        EnumZ _computeTakeSetEnum;

        private void InitializeEnums()
        {
            _valueTypeEnum = new EnumZ(EnumZStore,Trait.ValueTypeEnum);
            new PairZ(_valueTypeEnum, Trait.ValueType_Bool);
            new PairZ(_valueTypeEnum, Trait.ValueType_Char);
            new PairZ(_valueTypeEnum, Trait.ValueType_SByte);
            new PairZ(_valueTypeEnum, Trait.ValueType_Int16);
            new PairZ(_valueTypeEnum, Trait.ValueType_Int32);
            new PairZ(_valueTypeEnum, Trait.ValueType_Int64);
            new PairZ(_valueTypeEnum, Trait.ValueType_Single);
            new PairZ(_valueTypeEnum, Trait.ValueType_Double);
            new PairZ(_valueTypeEnum, Trait.ValueType_Decimal);
            new PairZ(_valueTypeEnum, Trait.ValueType_DateTime);
            new PairZ(_valueTypeEnum, Trait.ValueType_String);
            new PairZ(_valueTypeEnum, Trait.ValueType_Byte);
            new PairZ(_valueTypeEnum, Trait.ValueType_UInt16);
            new PairZ(_valueTypeEnum, Trait.ValueType_UInt32);
            new PairZ(_valueTypeEnum, Trait.ValueType_UInt64);
            new PairZ(_valueTypeEnum, Trait.ValueType_BoolArray);
            new PairZ(_valueTypeEnum, Trait.ValueType_CharArray);
            new PairZ(_valueTypeEnum, Trait.ValueType_SByteArray);
            new PairZ(_valueTypeEnum, Trait.ValueType_Int16Array);
            new PairZ(_valueTypeEnum, Trait.ValueType_Int32Array);
            new PairZ(_valueTypeEnum, Trait.ValueType_Int64Array);
            new PairZ(_valueTypeEnum, Trait.ValueType_SingleArray);
            new PairZ(_valueTypeEnum, Trait.ValueType_DoubleArray);
            new PairZ(_valueTypeEnum, Trait.ValueType_DecimalArray);
            new PairZ(_valueTypeEnum, Trait.ValueType_StringArray);
            new PairZ(_valueTypeEnum, Trait.ValueType_ByteArray);
            new PairZ(_valueTypeEnum, Trait.ValueType_UInt16Array);
            new PairZ(_valueTypeEnum, Trait.ValueType_UInt32Array);
            new PairZ(_valueTypeEnum, Trait.ValueType_UInt64Array);

            _pairingEnum = new EnumZ(EnumZStore, Trait.PairingEnum);
            new PairZ(_pairingEnum, Trait.Pairing_OneToOne);
            new PairZ(_pairingEnum, Trait.Pairing_OneToMany);
            new PairZ(_pairingEnum, Trait.Pairing_ManyToMany);

            _orientationEnum = new EnumZ(EnumZStore, Trait.AspectEnum);
            new PairZ(_orientationEnum, Trait.Aspect_Point);
            new PairZ(_orientationEnum, Trait.Aspect_Central);
            new PairZ(_orientationEnum, Trait.Aspect_Vertical);
            new PairZ(_orientationEnum, Trait.Aspect_Horizontal);

            _labelingEnum = new EnumZ(EnumZStore, Trait.LabelingEnum);
            new PairZ(_labelingEnum, Trait.Labeling_None);
            new PairZ(_labelingEnum, Trait.Labeling_Top);
            new PairZ(_labelingEnum, Trait.Labeling_Left);
            new PairZ(_labelingEnum, Trait.Labeling_Right);
            new PairZ(_labelingEnum, Trait.Labeling_Bottom);
            new PairZ(_labelingEnum, Trait.Labeling_Center);
            new PairZ(_labelingEnum, Trait.Labeling_TopLeft);
            new PairZ(_labelingEnum, Trait.Labeling_TopRight);
            new PairZ(_labelingEnum, Trait.Labeling_BottomLeft);
            new PairZ(_labelingEnum, Trait.Labeling_BottomRight);
            new PairZ(_labelingEnum, Trait.Labeling_TopLeftSide);
            new PairZ(_labelingEnum, Trait.Labeling_TopRightSide);
            new PairZ(_labelingEnum, Trait.Labeling_TopLeftCorner);
            new PairZ(_labelingEnum, Trait.Labeling_TopRightCorner);
            new PairZ(_labelingEnum, Trait.Labeling_BottomLeftSide);
            new PairZ(_labelingEnum, Trait.Labeling_BottomRightSide);
            new PairZ(_labelingEnum, Trait.Labeling_BottomLeftCorner);
            new PairZ(_labelingEnum, Trait.Labeling_BottomRightCorner);

            _resizingEnum = new EnumZ(EnumZStore, Trait.ResizingEnum);
            new PairZ(_resizingEnum, Trait.Resizing_Auto);
            new PairZ(_resizingEnum, Trait.Resizing_Fixed);
            new PairZ(_resizingEnum, Trait.Resizing_Manual);

            _flipRotateEnum = new EnumZ(EnumZStore, Trait.FlipRotateEnum);
            new PairZ(_flipRotateEnum, Trait.FlipRotate_None);
            new PairZ(_flipRotateEnum, Trait.FlipRotate_FlipVertical);
            new PairZ(_flipRotateEnum, Trait.FlipRotate_FlipHorizontal);
            new PairZ(_flipRotateEnum, Trait.FlipRotate_FlipBothWays);
            new PairZ(_flipRotateEnum, Trait.FlipRotate_RotateClockwise);
            new PairZ(_flipRotateEnum, Trait.FlipRotate_RotateFlipVertical);
            new PairZ(_flipRotateEnum, Trait.FlipRotate_RotateFlipHorizontal);
            new PairZ(_flipRotateEnum, Trait.FlipRotate_RotateFlipBothWays);

            _barWidthEnum = new EnumZ(EnumZStore, Trait.BarWidthEnum);
            new PairZ(_barWidthEnum, Trait.BarWidth_Thin);
            new PairZ(_barWidthEnum, Trait.BarWidth_Wide);
            new PairZ(_barWidthEnum, Trait.BarWidth_ExtraWide);

            _sideEnum = new EnumZ(EnumZStore, Trait.SideEnum);
            new PairZ(_sideEnum, Trait.Side_Any);
            new PairZ(_sideEnum, Trait.Side_East);
            new PairZ(_sideEnum, Trait.Side_West);
            new PairZ(_sideEnum, Trait.Side_North);
            new PairZ(_sideEnum, Trait.Side_South);

            _facetEnum = new EnumZ(EnumZStore, Trait.Facet_None);
            new PairZ(_facetEnum, Trait.Facet_None);
            new PairZ(_facetEnum, Trait.Facet_Nubby);
            new PairZ(_facetEnum, Trait.Facet_Diamond);
            new PairZ(_facetEnum, Trait.Facet_InArrow);
            new PairZ(_facetEnum, Trait.Facet_Force_None);
            new PairZ(_facetEnum, Trait.Facet_Force_Nubby);
            new PairZ(_facetEnum, Trait.Facet_Force_Diamond);
            new PairZ(_facetEnum, Trait.Facet_Force_InArrow);

            _contactEnum = new EnumZ(EnumZStore, Trait.ContactEnum);
            new PairZ(_contactEnum, Trait.Contact_Any);
            new PairZ(_contactEnum, Trait.Contact_One);
            new PairZ(_contactEnum, Trait.Contact_None);

            _connectEnum = new EnumZ(EnumZStore, Trait.ConnectEnum);
            new PairZ(_connectEnum, Trait.Connect_Any);
            new PairZ(_connectEnum, Trait.Connect_East);
            new PairZ(_connectEnum, Trait.Connect_West);
            new PairZ(_connectEnum, Trait.Connect_North);
            new PairZ(_connectEnum, Trait.Connect_South);
            new PairZ(_connectEnum, Trait.Connect_East_West);
            new PairZ(_connectEnum, Trait.Connect_North_South);
            new PairZ(_connectEnum, Trait.Connect_North_East);
            new PairZ(_connectEnum, Trait.Connect_North_West);
            new PairZ(_connectEnum, Trait.Connect_North_East_West);
            new PairZ(_connectEnum, Trait.Connect_North_South_East);
            new PairZ(_connectEnum, Trait.Connect_North_South_West);
            new PairZ(_connectEnum, Trait.Connect_South_East);
            new PairZ(_connectEnum, Trait.Connect_South_West);
            new PairZ(_connectEnum, Trait.Connect_South_East_West);

            _attatchEnum = new EnumZ(EnumZStore, Trait.AttatchEnum);
            new PairZ(_attatchEnum, Trait.Attatch_Normal);
            new PairZ(_attatchEnum, Trait.Attatch_Radial);
            new PairZ(_attatchEnum, Trait.Attatch_RightAngle);
            new PairZ(_attatchEnum, Trait.Attatch_SkewedAngle);

            _lineStyleEnum = new EnumZ(EnumZStore, Trait.LineStyleEnum);
            new PairZ(_lineStyleEnum, Trait.LineStyle_PointToPoint);
            new PairZ(_lineStyleEnum, Trait.LineStyle_SimpleSpline);
            new PairZ(_lineStyleEnum, Trait.LineStyle_DoubleSpline);

            _dashStyleEnum = new EnumZ(EnumZStore, Trait.DashStyleEnum);
            new PairZ(_dashStyleEnum, Trait.DashStyle_Solid);
            new PairZ(_dashStyleEnum, Trait.DashStyle_Dashed);
            new PairZ(_dashStyleEnum, Trait.DashStyle_Dotted);
            new PairZ(_dashStyleEnum, Trait.DashStyle_DashDot);
            new PairZ(_dashStyleEnum, Trait.DashStyle_DashDotDot);

            _numericSetEnum = new EnumZ(EnumZStore, Trait.NumericSetEnum);
            new PairZ(_numericSetEnum, Trait.NumericSet_Count);
            new PairZ(_numericSetEnum, Trait.NumericSet_Count_Min_Max);
            new PairZ(_numericSetEnum, Trait.NumericSet_Count_Min_Max_Sum);
            new PairZ(_numericSetEnum, Trait.NumericSet_Count_Min_Max_Sum_Ave);
            new PairZ(_numericSetEnum, Trait.NumericSet_Count_Min_Max_Sum_Ave_Std);

            _numericTermEnum = new EnumZ(EnumZStore, Trait.NumericTermEnum);
            new PairZ(_numericTermEnum, Trait.NumericTerm_Count);
            new PairZ(_numericTermEnum, Trait.NumericTerm_Min);
            new PairZ(_numericTermEnum, Trait.NumericTerm_Max);
            new PairZ(_numericTermEnum, Trait.NumericTerm_Sum);
            new PairZ(_numericTermEnum, Trait.NumericTerm_Ave);
            new PairZ(_numericTermEnum, Trait.NumericTerm_Std);

            _computeTypeEnum = new EnumZ(EnumZStore, Trait.CompuTypeEnum);
            new PairZ(_computeTypeEnum, Trait.CompuType_RowValue);
            new PairZ(_computeTypeEnum, Trait.CompuType_RelatedValue);
            new PairZ(_computeTypeEnum, Trait.CompuType_NumericValueSet);
            new PairZ(_computeTypeEnum, Trait.CompuType_CompositeString);
            new PairZ(_computeTypeEnum, Trait.CompuType_CompositeReversed);

            _computeResultsEnum = new EnumZ(EnumZStore, Trait.ResultsEnum);
            new PairZ(_computeResultsEnum, Trait.Results_OneValue);
            new PairZ(_computeResultsEnum, Trait.Results_AllValues);
            new PairZ(_computeResultsEnum, Trait.Results_LimitedSet);

            _computeSortingEnum = new EnumZ(EnumZStore, Trait.SortingEnum);
            new PairZ(_computeSortingEnum, Trait.Sorting_Unsorted);
            new PairZ(_computeSortingEnum, Trait.Sorting_Ascending);
            new PairZ(_computeSortingEnum, Trait.Sorting_Descending);

            _computeTakeSetEnum = new EnumZ(EnumZStore, Trait.TakeSetEnum);
            new PairZ(_computeTakeSetEnum, Trait.TakeSet_First);
            new PairZ(_computeTakeSetEnum, Trait.TakeSet_Last);
            new PairZ(_computeTakeSetEnum, Trait.TakeSet_Both);
        }
    }
}
