
namespace ModelGraphSTD
{
    /// <summary>
    /// Complete catalog all items, models, and commands 
    /// </summary>
    public enum Trait : ushort
    {
        // Trait provides flags and identity for items, models, and commands. 
        // It also is used as a key to locate resource strings.
        //
        // Resource string keys are of the form:
        // xxxK - the item's Kind 
        // xxxN - the item's Name
        // xxxS - the item's Summary (tooltip text)
        // xxxV - the item's Description
        // where xxx are the three hex digits enumerated in this file

        #region Flags  ========================================================
        Empty = 0,

        IsExternal = 0x8000, // Item, specify load/save to storage file

        IsReadOnly = 0x2000, // Property
        CanMultiline = 0x1000, // Property

        GetStorageFile = 0x1000, // Command

        KeyMask = 0xFFF,
        FlagMask = 0xF000,
        EnumMask = 0x3F,
        IndexMask = 0xF,

        #endregion

        #region MainUI  ==============================================(000-01F)
        // resource string keys used by the main UI 
        // (not associated with any individual item, model or command)

        BlankName = 0x001,
        InvalidItem = 0x002,
        ModelGraphTitle = 0x003,
        AppRootModelTab = 0x004,
        SortMode = 0x005,
        ExpandLeft = 0x006,
        TotalCount = 0x007,
        FilterText = 0x008,
        FilterCount = 0X009,
        ExpandRight = 0x00A,
        FilterExpand = 0x00B,
        NewModel = 0x00C,
        EditSymbol = 0x00D,

        #endregion

        #region Command  =============================================(020-07F)

        NewCommand = 0x21,
        OpenCommand = 0x22 | GetStorageFile,
        SaveCommand = 0x23,
        SaveAsCommand = 0x24 | GetStorageFile,
        ReloadCommand = 0x25,
        CloseCommand = 0x26,

        EditCommand = 0x30,
        ViewCommand = 0x31,
        UndoCommand = 0x32,
        RedoCommand = 0x33,
        MergeCommand = 0x34,
        InsertCommand = 0x35,
        RemoveCommand = 0x36,
        CreateCommand = 0x37,
        RefreshCommand = 0x38,
        ExpandAllCommand = 0x39,
        MakeRootLinkCommand = 0x3A,
        MakePathHeadCommand = 0x3B,
        MakeGroupHeadCommand = 0x3C,
        MakeEgressHeadCommand = 0x3D,
        #endregion

        #region Store ================================================(0E0-0FF)
        // root level containers for the hierarchal item trees

        EnumXStore = 0x0E1,
        ViewXStore = 0x0E2,
        TableXStore = 0x0E3,
        GraphXStore = 0x0E4,
        QueryXStore = 0x0E5,
        ValueXStore = 0x0E6,
        SymbolXStore = 0x0E7,
        ColumnXStore = 0x0E8,
        ComputeXStore = 0x0E9,
        RelationXStore = 0x0EA,

        PrimeStore = 0x0F0, // exposes internal tables (metadata / configuration)
        EnumZStore = 0x0F1,
        ErrorStore = 0x0F2,
        GroupStore = 0x0F3,
        PropertyStore = 0x0F4,
        RelationStore = 0x0F5,
        RelationZStore = 0x0F6,

        #endregion

        #region Item  ================================================(100-2FF)

        //=========================================
        Dummy = 0x110,
        RootChef = 0x111,
        DataChef = 0x112,

        //=========================================
        ErrorImport = 0x121,
        ErrorExport = 0x122,
        ErrorChildren = 0x123,
        ErrorProperty = 0x124,
        ErrorRelation = 0x125,

        //=========================================
        ChangeRoot = 0x131,
        ChangeSet = 0x132,
        ItemUpdated = 0x133,
        ItemCreated = 0x134,
        ItemRemoved = 0x135,
        ItemLinked = 0x136,
        ItemUnlinked = 0x137,
        ItemMoved = 0x138,
        ItemChildMoved = 0x139,
        ItemParentMoved = 0x13A,

        //=========================================
        // External (user-defined) item classes
        RowX = 0x141 | IsExternal,
        PairX = 0x142 | IsExternal,
        EnumX = 0x143 | IsExternal,
        ViewX = 0x144 | IsExternal,
        TableX = 0x145 | IsExternal,
        GraphX = 0x146 | IsExternal,
        QueryX = 0x147 | IsExternal,
        SymbolX = 0x148 | IsExternal,
        ColumnX = 0x149 | IsExternal,
        ComputeX = 0x14A | IsExternal,
        CommandX = 0x14B | IsExternal,
        RelationX = 0x14C | IsExternal,

        //=========================================
        // QueryX detail, used to lookup resource strings
        QueryIsCorrupt = 0x150,
        QueryGraphRoot = 0x151,
        QueryGraphLink = 0x152,
        QueryViewRoot = 0x153,
        QueryViewHead = 0x154,
        QueryViewLink = 0x155,
        QueryPathHead = 0x156,
        QueryPathLink = 0x157,
        QueryGroupHead = 0x158,
        QueryGroupLink = 0x159,
        QuerySegueHead = 0x15A,
        QuerySegueLink = 0x15B,
        QueryValueRoot = 0x15C,
        QueryValueHead = 0x15D,
        QueryValueLink = 0x15E,
        QueryNodeSymbol = 0x15F,

        //=========================================
        QueryWhere = 0x161, // used to lookup kind resource string "Where"
        QuerySelect = 0x162, // used to lookup kind resource string "Select"

        //=========================================

        //=========================================

        //=========================================

        //=========================================
        Graph = 0x1C1,
        Query = 0x1B0,
        Level = 0x1C2,
        Node = 0x1C3,
        Edge = 0x1C4,
        Open = 0x1C5,

        //=========================================
        QueryPath = 0x1E3,
        FlaredPath = 0x1E4,
        ForkedPath = 0x1E5,
        SeriesPath = 0x1E6,
        ParallelPath = 0x1E7,

        LinkPath = 0x1EE, // used to lookup kind resource string "_Link"
        RadialPath = 0x1EF, // used to lookup kind resource string "_Radial"

        #endregion

        #region Relation  ============================================(300-3FF)

        Relation = 0x300,

        //=========================================
        EnumX_ColumnX = 0x311,
        TableX_ColumnX = 0x312,
        TableX_NameProperty = 0x313,
        TableX_SummaryProperty = 0x314,
        TableX_ChildRelationX = 0x315,
        TableX_ParentRelationX = 0x316,

        //=========================================
        TableChildRelationGroup = 0x321,
        TableParentRelationGroup = 0x322,
        TableReverseRelationGroup = 0x323,
        TableRelationGroupRelation = 0x324,
        ParentRelationGroupRelation = 0x325,
        ReverseRelationGroupRelation = 0x326,

        //=========================================
        Item_Error = 0x331,
        ViewX_ViewX = 0x332,
        ViewX_QueryX = 0x333,
        QueryX_ViewX = 0x334,
        Property_ViewX = 0x335,
        Relation_ViewX = 0x336,
        ViewX_Property = 0x337,
        QueryX_Property = 0x338,

        //=========================================
        GraphX_SymbolX = 0x341,
        SymbolX_QueryX = 0x342,
        GraphX_QueryX = 0x343,
        QueryX_QueryX = 0x344,
        GraphX_ColorColumnX = 0x345,
        GraphX_SymbolQueryX = 0x346,

        //=========================================
        Store_QueryX = 0x351,
        Relation_QueryX = 0x352,

        //=========================================
        Store_ComputeX = 0x361,
        ComputeX_QueryX = 0x362,

        //=========================================
        Store_Property = 0x3FD,
        Store_ChildRelation = 0x3FE,
        Store_ParentRelation = 0x3FF,

        #endregion

        #region Property  ============================================(400-5FF)

        Property = 0x400,

        //=========================================
        ViewName_P = 0x401,
        ViewSummary_P = 0x402,

        //=========================================
        EnumName_P = 0x411,
        EnumSummary_P = 0x412,
        EnumText_P = 0x413,
        EnumValue_P = 0x414,

        //=========================================
        TableName_P = 0x421,
        TableSummary_P = 0x422,

        //=========================================
        ColumnName_P = 0x431,
        ColumnSummary_P = 0x432,
        ColumnValueType_P = 0x433,
        ColumnAccess_P = 0x434,
        ColumnInitial_P = 0x435,
        ColumnIsChoice_P = 0x436,

        //=========================================
        RelationName_P = 0x441,
        RelationSummary_P = 0x442,
        RelationPairing_P = 0x443,
        RelationIsRequired_P = 0x444,
        RelationIsReference_P = 0x445,
        RelationMinOccurance_P = 0x446,
        RelationMaxOccurance_P = 0x447,

        //=========================================
        GraphName_P = 0x451,
        GraphSummary_P = 0x452,

        //=========================================
        QueryXFilter_P = 0x461 | CanMultiline,
        QueryXWhere_P = 0x463 | CanMultiline,
        QueryXConnect1_P = 0x464,
        QueryXConnect2_P = 0x465,
        QueryXRelation_P = 0x466,
        QueryXIsReversed_P = 0x467,
        QueryXIsImmediate_P = 0x468,
        QueryXIsPersistent_P = 0x469,
        QueryXIsBreakPoint_P = 0x46A,
        QueryXExclusiveKey_P = 0x46B,
        QueryXAllowSelfLoop_P = 0x46C,
        QueryXIsPathReversed_P = 0x46D,
        QueryXIsFullTableRead_P = 0x46E,

        //=========================================
        ValueXWhere_P = 0x471 | CanMultiline,
        ValueXSelect_P = 0x472 | CanMultiline,
        ValueXIsReversed_P = 0x473,
        ValueXValueType_P = 0x474 | IsReadOnly,


        //=========================================
        SymbolName_P = 0x481,
        SymbolTopContact_P = 0x482,
        SymbolLeftContactt_P = 0x483,
        SymbolRightContact_P = 0x484,
        SymbolBottomContact_P = 0x485,

        //=========================================
        NodeCenterXY_P = 0x491,
        NodeSizeWH_P = 0x492,
        NodeLabeling_P = 0x493,
        NodeResizing_P = 0x494,
        NodeBarWidth_P = 0x495,
        NodeOrientation_P = 0x496,
        NodeFlipRotate_P = 0x497,

        //=========================================
        EdgeFace1_P = 0x4A1,
        EdgeFace2_P = 0x4A2,
        EdgeGnarl1_P = 0x4A3,
        EdgeGnarl2_P = 0x4A4,
        EdgeConnect1_P = 0x4A5,
        EdgeConnect2_P = 0x4A6,

        //=========================================
        ComputeXName_P = 0x4B1,
        ComputeXSummary_P = 0x4B2,
        ComputeXCompuType_P = 0x4B3,
        ComputeXWhere_P = 0x4B4,
        ComputeXSelect_P = 0x4B5,
        ComputeXSeparator_P = 0x4B6,
        ComputeXValueType_P = 0x4B7 | IsReadOnly,
        ComputeXNumericSet_P = 0x4B8,

        #endregion

        #region Model ================================================(600-7FF)

        //=====================================================================
        RootChef_M = 0x611,
        DataChef_M = 0x612,
        MockChef_M = 0x613,
        TextColumn_M = 0x614,
        CheckColumn_M = 0x615,
        ComboColumn_M = 0x616,
        TextProperty_M = 0x617,
        CheckProperty_M = 0x618,
        ComboProperty_M = 0x619,
        TextCompute_M = 0x61A,

        //=====================================================================
        ErrorRoot_M = 0x621,
        ChangeRoot_M = 0x622,
        MetadataRoot_M = 0x623,
        ModelingRoot_M = 0x624,
        MetaRelation_ZM = 0x625,
        ErrorType_M = 0x626,
        ErrorText_M = 0x627,
        ChangeSet_M = 0x628,
        ItemChange_M = 0x629,

        //=====================================================================
        ViewXView_ZM = 0x631,
        ViewXView_M = 0x632,
        ViewXQuery_M = 0x633,
        ViewXCommand_M = 0x634,
        ViewXProperty_M = 0x635,

        ViewView_ZM = 0x63A,
        ViewView_M = 0x63B,
        ViewItem_M = 0x63C,
        ViewQuery_M = 0x63D,

        //=====================================================================
        EnumX_ZM = 0x642,
        TableX_ZM = 0x643,
        GraphX_ZM = 0x644,
        SymbolX_ZM = 0x645,
        Table_ZM = 0x647,
        Graph_ZM = 0x648,

        //=====================================================================
        PairX_M = 0x652,
        EnumX_M = 0x653,
        TableX_M = 0x654,
        GraphX_M = 0x655,
        SymbolX_M = 0x656,
        ColumnX_M = 0x657,
        ComputeX_M = 0x658,

        //=====================================================================
        ColumnX_ZM = 0x661,
        ChildRelationX_ZM = 0x662,
        ParentRelatationX_ZM = 0x663,
        EnumValue_ZM = 0x664,
        EnumColumn_ZM = 0x665,
        ComputeX_ZM = 0x666,

        //=====================================================================
        ChildRelationX_M = 0x671,
        ParentRelationX_M = 0x672,
        NameColumnRelation_M = 0x673,
        SummaryColumnRelation_M = 0x674,
        NameColumn_M = 0x675,
        SummaryColumn_M = 0x676,

        //=====================================================================
        GraphXColoring_M = 0x681,
        GraphXRoot_ZM = 0x682,
        GraphXNode_ZM = 0x683,
        GraphXNode_M = 0x684,
        GraphXColorColumn_M = 0x685,

        //=====================================================================
        GraphXRoot_M = 0x691,
        GraphXLink_M = 0x692,
        GraphXPathHead_M = 0x693,
        GraphXPathLink_M = 0x694,
        GraphXGroupHead_M = 0x695,
        GraphXGroupLink_M = 0x696,
        GraphXEgressHead_M = 0x697,
        GraphXEgressLink_M = 0x698,
        GraphXNodeSymbol_M = 0x699,

        ValueXHead_M = 0x69E,
        ValueXLink_M = 0x69F,

        //=====================================================================
        Row_M = 0x6A1,
        Table_M = 0x6A4,
        Graph_M = 0x6A5,
        GraphRef_M = 0x6A6,
        RowChildRelation_M = 0x6A7,
        RowParentRelation_M = 0x6A8,
        RowRelatedChild_M = 0x6A9,
        RowRelatedParent_M = 0x6AA,
        EnumRelatedColumn_M = 0x6AB,

        //=====================================================================
        RowProperty_ZM = 0x6B1,
        RowChildRelation_ZM = 0x6B2,
        RowParentRelation_ZM = 0x6B3,
        RowDefaultProperty_ZM = 0x6B4,
        RowUnusedChildRelation_ZM = 0x6B5,
        RowUnusedParentRelation_ZM = 0x6B6,
        RowCompute_ZM = 0x6B7,

        //=====================================================================
        QueryRootLink_M = 0x6C1,
        QueryPathHead_M = 0x6C2,
        QueryPathLink_M = 0x6C3,
        QueryGroupHead_M = 0x6C4,
        QueryGroupLink_M = 0x6C5,
        QueryEgressHead_M = 0x6C6,
        QueryEgressLink_M = 0x6C7,

        //=====================================================================
        QueryRootItem_M = 0x6D1,
        QueryPathStep_M = 0x6D2,
        QueryPathTail_M = 0x6D3,
        QueryGroupStep_M = 0x6D4,
        QueryGroupTail_M = 0x6D5,
        QueryEgressStep_M = 0x6D6,
        QueryEgressTail_M = 0x6D7,

        //=====================================================================
        GraphXRef_M = 0x6E1,
        GraphNode_ZM = 0x6E2,
        GraphEdge_ZM = 0x6E3,
        GraphRoot_ZM = 0x6E4,
        GraphLevel_ZM = 0x6E5,
        GraphLevel_M = 0x6E6,
        GraphPath_M = 0x6E7,
        GraphRoot_M = 0x6E8,
        GraphNode_M = 0x6E9,
        GraphEdge_M = 0x6EA,
        GraphOpen_ZM = 0x6EB,
        GraphOpen_M = 0x6EC,

        //=====================================================================
        PrimeCompute_M = 0x7D0,
        ComputeStore_M = 0x7D1,

        //=====================================================================
        InternalStore_ZM = 0x7F0,
        InternalStore_M = 0x7F1,

        StoreItem_M = 0x7F2,

        StoreItemItem_ZM = 0x7F4,
        StoreRelationLink_ZM = 0x7F5,

        StoreChildRelation_ZM = 0x7F6,
        StoreParentRelation_ZM = 0x7F7,

        StoreItemItem_M = 0x7F8,
        StoreRelationLink_M = 0x7F9,

        StoreChildRelation_M = 0x7FA,
        StoreParentRelation_M = 0x7FB,

        StoreRelatedItem_M = 0x7FC,
        #endregion

        #region Enum  ================================================(800-FFF)
        // facilitates text localization for static enums/pairs

        ValueType_Bool = 0x800,
        ValueType_Char = 0x801,

        ValueType_Byte = 0x802,
        ValueType_SByte = 0x803,

        ValueType_Int16 = 0x804,
        ValueType_UInt16 = 0x805,

        ValueType_Int32 = 0x806,
        ValueType_UInt32 = 0x807,

        ValueType_Int64 = 0x808,
        ValueType_UInt64 = 0x809,

        ValueType_Single = 0x80A,
        ValueType_Double = 0x80B,

        ValueType_Decimal = 0x80C,
        ValueType_DateTime = 0x80D,

        ValueType_String = 0x80E,

        ValueType_BoolArray = 0x80F,
        ValueType_CharArray = 0x810,

        ValueType_ByteArray = 0x811,
        ValueType_SByteArray = 0x812,

        ValueType_Int16Array = 0x813,
        ValueType_UInt16Array = 0x814,

        ValueType_Int32Array = 0x815,
        ValueType_UInt32Array = 0x816,

        ValueType_Int64Array = 0x817,
        ValueType_UInt64Array = 0x818,

        ValueType_SingleArray = 0x819,
        ValueType_DoubleArray = 0x81A,

        ValueType_DecimalArray = 0x81B,
        ValueType_DateTimeArray = 0x81C,

        ValueType_StringArray = 0x81D,
        ValueTypeEnum = 0x83F,

        xxValueType_None = 0x840,
        xxValueTypeEnum = 0x87F,

        Pairing_OneToOne = 0x880,
        Pairing_OneToMany = 0x881,
        Pairing_ManyToMany = 0x882,
        PairingEnum = 0x8BF,

        Orientation_Point = 0x8C0,
        Orientation_Central = 0x8C1,
        Orientation_Vertical = 0x8C2,
        Orientation_Horizontal = 0x8C3,
        OrientationEnum = 0x8FF,

        Labeling_None = 0x900,
        Labeling_Top = 0x901,
        Labeling_Left = 0x902,
        Labeling_Right = 0x903,
        Labeling_Bottom = 0x904,
        Labeling_Center = 0x905,
        Labeling_TopLeft = 0x906,
        Labeling_TopRight = 0x907,
        Labeling_BottomLeft = 0x908,
        Labeling_BottomRight = 0x909,
        Labeling_TopLeftSide = 0x90A,
        Labeling_TopRightSide = 0x90B,
        Labeling_TopLeftCorner = 0x90C,
        Labeling_TopRightCorner = 0x90D,
        Labeling_BottomLeftSide = 0x90E,
        Labeling_BottomRightSide = 0x90F,
        Labeling_BottomLeftCorner = 0x910,
        Labeling_BottomRightCorner = 0x911,
        LabelingEnum = 0x93F,

        FlipRotate_None = 0x940,
        FlipRotate_FlipVertical = 0x941,
        FlipRotate_FlipHorizontal = 0x942,
        FlipRotate_FlipBothWays = 0x943,
        FlipRotate_RotateClockwise = 0x944,
        FlipRotate_RotateFlipVertical = 0x945,
        FlipRotate_RotateFlipHorizontal = 0x946,
        FlipRotate_RotateFlipBothWays = 0x947,
        FlipRotateEnum = 0x97F,

        Resizing_Auto = 0x980,
        Resizing_Fixed = 0x981,
        Resizing_Manual = 0x982,
        ResizingEnum = 0x9BF,

        Naming_None = 0x9C0,
        Naming_Default = 0x9C1,
        Naming_UniqueNumber = 0x9C2,
        Naming_Alphabetic = 0x9C3,
        Naming_SubstituteParent = 0x9C4,
        NamingEnum = 0x9FF,

        BarWidth_Normal = 0xA00,
        BarWidth_Thin = 0xA01,
        BarWidth_Wide = 0xA02,
        BarWidthEnum = 0xA3F,

        Contact_Any = 0xA40,
        Contact_One = 0xA41,
        Contact_None = 0xA42,
        ContactEnum = 0xA7F,

        Side_Any = 0xA80,
        Side_East = 0xA81,
        Side_West = 0xA82,
        Side_North = 0xA84,
        Side_South = 0xA88,
        SideEnum = 0xABF,

        Connect_Any = 0xAC0, //0
        Connect_East = 0xAC1, //1
        Connect_West = 0xAC2, //2
        Connect_North = 0xAC4, //4
        Connect_South = 0xAC8, //8
        Connect_East_West = 0xAC3, //1+2
        Connect_North_South = 0xACC, //4+8
        Connect_North_East = 0xAC5, //4+1
        Connect_North_West = 0xAC6, //4+2
        Connect_North_East_West = 0xAC7, //4+1+2
        Connect_North_South_East = 0xACD, //4+8+1
        Connect_North_South_West = 0xACE, //4+8+2
        Connect_South_East = 0xAC9, //8+1
        Connect_South_West = 0xACA, //8+2
        Connect_South_East_West = 0xACB, //8+1+2
        ConnectEnum = 0xAFF,

        Facet_None = 0xB00,
        Facet_Nubby = 0xB01,
        Facet_Diamond = 0xB02,
        Facet_InArrow = 0xB03,
        FacetEnum = 0xB3F,

        CompuType_RowValue = 0xB40,
        CompuType_RelatedValue = 0xB41,
        CompuType_NumericValueSet = 0xB42,
        CompuType_CompositeString = 0xB43,
        CompuType_CompositeReversed = 0xB44,
        CompuTypeEnum = 0xB7F,

        NumericSet_Count = 0xB80,
        NumericSet_Count_Min_Max = 0xB81,
        NumericSet_Count_Min_Max_Sum = 0xB82,
        NumericSet_Count_Min_Max_Sum_Ave = 0xB83,
        NumericSet_Count_Min_Max_Sum_Ave_Std = 0xB84,
        NumericSetEnum = 0xBBF,

        NumericTerm_Count = 0xBC0,
        NumericTerm_Min = 0xBC1,
        NumericTerm_Max = 0xBC2,
        NumericTerm_Sum = 0xBC3,
        NumericTerm_Ave = 0xBC4,
        NumericTerm_Std = 0xBC5,
        NumericTermEnum = 0xBFF,

        StartLine_Flat = 0xC00,
        StartLine_Square = 0xC01,
        StartLine_Round = 0xC02,
        StartLine_Triangle = 0xC03,
        StartLineEnum = 0xC3F,

        EndLine_Flat = 0xC40,
        EndLine_Square = 0xC41,
        EndLine_Round = 0xC42,
        EndLine_Triangle = 0xC43,
        EndLineEnum = 0xC7F,

        StaticPairC80 = 0xC80,
        StaticEnumCBF = 0xCBF,

        StaticPairCC0 = 0xCC0,
        StaticEnumCFF = 0xCFF,

        StaticPairD00 = 0xD00,
        StaticEnumD3F = 0xD3F,

        StaticPairD40 = 0xD40,
        StaticEnumD7F = 0xD7F,

        StaticPairD80 = 0xD80,
        StaticEnumDBF = 0xDBF,

        StaticPairDC0 = 0xDC0,
        StaticEnumDFF = 0xDFF,

        StaticPairE00 = 0xE00,
        StaticEnumE3F = 0xE3F,

        StaticPairE40 = 0xE40,
        StaticEnumE7F = 0xE7F,

        StaticPairE80 = 0xE80,
        StaticEnumEBF = 0xEBF,

        StaticPairEC0 = 0xEC0,
        StaticEnumEFF = 0xEFF,

        StaticPairF00 = 0xF00,
        StaticEnumF3F = 0xF3F,

        StaticPairF40 = 0xF40,
        StaticEnumF7F = 0xF7F,

        StaticPairF80 = 0xF80,
        StaticEnumFBF = 0xFBF,

        StaticPairFC0 = 0xFC0,
        StaticEnumFFF = 0xFFF,

        #endregion
    }
}
