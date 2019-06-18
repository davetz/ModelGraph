using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    public partial class Chef
    {
        const int NT = 10; //number of test tables (must be 10 or more)
        const int NR = 10; //number of test rows/table (1 or more)

        #region CreateTestData  ===============================================
        public void CreateTestModel()
        {
            CreateTestTables();

            // 1 child
            CreateTestRelation(0, 1, Pairing.OneToOne);

            // fan out to 2 grand-children
            CreateTestRelation(1, 2, Pairing.OneToMany);
            CreateTestRelation(1, 3, Pairing.OneToMany);

            // fan out to 6 great-grand-children
            CreateTestRelation(2, 4, Pairing.ManyToMany);
            CreateTestRelation(2, 5, Pairing.ManyToMany);
            CreateTestRelation(2, 6, Pairing.ManyToMany);
            CreateTestRelation(3, 7, Pairing.ManyToMany);
            CreateTestRelation(3, 8, Pairing.ManyToMany);
            CreateTestRelation(3, 9, Pairing.ManyToMany);

            // merge in to 1 great-great-grand-child
            CreateTestRelation(3, 0, Pairing.ManyToMany);
            CreateTestRelation(4, 0, Pairing.ManyToMany);
            CreateTestRelation(5, 0, Pairing.ManyToMany);
            CreateTestRelation(6, 0, Pairing.ManyToMany);
            CreateTestRelation(7, 0, Pairing.ManyToMany);
            CreateTestRelation(8, 0, Pairing.ManyToMany);
            CreateTestRelation(9, 0, Pairing.ManyToMany);

            //(X = 0)
            //
            //====1===2===3====4=======6====(Y = 0)
            //:   :   :   :    :       : ...
            //1             /--T4
            //2          -T2---T5  
            //3         /   \--T6
            //4  T0---T1               T0 ...
            //5         \   /--T7
            //6          -T3---T8  
            //7             \--T9
            //:   :   :    :    :      : ...
            //====1===2====3====4======6
        }
        #endregion

        #region CheckTestData  ===============================================
        public bool CheckTestModel()
        {
            var NC = (int)ValType.MaximumType; // expected number of columns

            if (TableXStore.Count != NT) return false;
            foreach (var tx in TableXStore.Items)
            {
                if (tx.Items.Count != NR) return false;

                if (!TableX_ColumnX.TryGetChildren(tx, out IList<ColumnX> columns)) return false;
                if (columns.Count != NC) return false;

                foreach (var rx in tx.Items)
                {
                    foreach (var cx in columns) { if (!CheckColumnTestValue(rx, cx)) return false; }
                }                
            }

            // 1 child
            if (!CheckTestRelation(0, 1, Pairing.OneToOne)) return false;

            // fan out to 2 grand-children
            if (!CheckTestRelation(1, 2, Pairing.OneToMany)) return false;
            if (!CheckTestRelation(1, 3, Pairing.OneToMany)) return false;

            // fan out to 6 great-grand-children
            if (!CheckTestRelation(2, 4, Pairing.ManyToMany)) return false;
            if (!CheckTestRelation(2, 5, Pairing.ManyToMany)) return false;
            if (!CheckTestRelation(2, 6, Pairing.ManyToMany)) return false;
            if (!CheckTestRelation(3, 7, Pairing.ManyToMany)) return false;
            if (!CheckTestRelation(3, 8, Pairing.ManyToMany)) return false;
            if (!CheckTestRelation(3, 9, Pairing.ManyToMany)) return false;

            // merge in to 1 great-great-grand-child
            if (!CheckTestRelation(3, 0, Pairing.ManyToMany)) return false;
            if (!CheckTestRelation(4, 0, Pairing.ManyToMany)) return false;
            if (!CheckTestRelation(5, 0, Pairing.ManyToMany)) return false;
            if (!CheckTestRelation(6, 0, Pairing.ManyToMany)) return false;
            if (!CheckTestRelation(7, 0, Pairing.ManyToMany)) return false;
            if (!CheckTestRelation(8, 0, Pairing.ManyToMany)) return false;
            if (!CheckTestRelation(9, 0, Pairing.ManyToMany)) return false;

            return true;
        }
        #endregion

        #region CreateTestTables  =============================================
        internal void CreateTestTables()
        {
            for (int i = 0; i < NR; i++)
            {
                var tx = new TableX(TableXStore);
                tx.SetCapacity(NR);

                CreateAllValTypeColumns(tx);
                for (int j = 0; j < NR; j++)
                {
                    CreateRowAssigTestValues(tx);
                }
            }
        }
        #endregion

        #region CreateTableTestRelation  ======================================
        internal bool CreateTestRelation(int tx1Index, int tx2Index, Pairing pairing)
        {
            if (IsInvalidTableIndex(tx1Index)) return false;
            if (IsInvalidTableIndex(tx2Index)) return false;

            var rx = new RelationX(RelationXStore);
            rx.Pairing = pairing;
            rx.Initialize(NR, NR);

            var tx1 = TableXStore.Items[tx1Index];
            var tx2 = TableXStore.Items[tx2Index];

            TableX_ChildRelationX.SetLink(tx1, rx);
            TableX_ParentRelationX.SetLink(tx2, rx);

            for (int i = 0; i < NR; i++)
            {
                rx.SetLink(tx1.Items[i], tx2.Items[i]);   
            }
            return true;

            bool IsInvalidTableIndex(int txI) => (txI < 0) ? true : (txI < TableXStore.Count) ? false : true;
        }
        #endregion


        #region CheckTableTestRelation  =======================================
        internal bool CheckTestRelation(int tx1Index, int tx2Index, Pairing pairing)
        {
            var tx1 = TableXStore.Items[tx1Index];
            var tx2 = TableXStore.Items[tx2Index];

            if (!TableX_ChildRelationX.TryGetChildren(tx1, out IList<RelationX> relations)) return false;
            foreach (var rx in relations)
            {
                if (!TableX_ParentRelationX.TryGetParent(rx, out TableX tx)) return false;
                if (tx == tx2)
                {
                    for (int i = 0; i < NR; i++)
                    {
                        var nx = tx1.Items[i];
                        if (!rx.TryGetChildren(nx, out IList<RowX> children)) return false;
                        if (!children.Contains(tx2.Items[i])) return false;
                    }
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region CreateAllValTypeColumns  ======================================
        internal void CreateAllValTypeColumns(TableX tx)
        {
            for (int i = 0; i < (int)ValType.MaximumType; i++)
            {
                var cx = new ColumnX(ColumnXStore);
                TableX_ColumnX.SetLink(tx, cx);
                SetColumnValueType(cx, i);
            }
        }
        #endregion

        #region CreateRowAssigTestValues  =====================================
        internal void CreateRowAssigTestValues(TableX tx)
        {
            var rx = new RowX(tx);
            if (TableX_ColumnX.TryGetChildren(tx, out IList<ColumnX> colums))
            {
                foreach (var col in colums)
                {
                    col.Value.SetValue(rx, AllValTypeValueStrings[(int)col.Value.ValType]);
                }
            }
        }
        #endregion

        #region AllValTypeValueStrings  =======================================
        static string[] AllValTypeValueStrings =   
        {
            "T",                                            // 0 Bool
            "T,F,T,F,1,0,1,0",                              // 1 BoolArray
            "K",                                            // 2 Char
            "White-Dog",                                    // 3 CharArray
            "254",                                          // 4 Byte
            "2,5,4",                                        // 5 ByteArray
            "-128",                                         // 6 SByte
            "-128, 0, 127",                                 // 7 SByteArray
            "-32768",                                       // 8 Int16
            "-32768, 0, 32767",                             // 9 Int16Array
            "0xF0F0",                                       //10 UInt16
            "0xF0F0, 0, 0x0F0F",                            //11 UInt16Array
            "-2147483648",                                  //12 Int32
            "-2147483648, 0, 2147483647",                   //13 Int32Array
            "0xF0F0F0F0",                                   //14 UInt32
            "0xF0F0F0F0, 0, 0x0F0F0F0F",                    //15 UInt32Array
            "-9223372036854775808",                         //16 Int64
            "-9223372036854775808, 0, 9223372036854775807", //17 Int64Array
            "0xF0F0F0F0F0F0F0F0",                           //18 UInt64
            "0xF0F0F0F0F0F0F0F0, 0, 0x0F0F0F0F0F0F0F0F",    //19 UInt64Array
            "-4.0E10",                                      //20 Single
            "-4.0E10, 0, 4.0E10",                           //21 SingleArray
            "-1.7E308",                                     //22 Double
            "-1.7E308, 0 1.7E308",                          //23 DoubleArray
            "-5.100300456789",                              //24 Decimal
            "-5.100300456789, 0, 5.100300456789",           //25 DecimalArray
            "5/1/2008 8:30:52 AM",                          //26 DateTime
            "5/1/2008, 6/2/2009",                           //27 DataTimeArray
            "What is this?",                                //28 String
            "Can't create a string array from a string",    //29 StringArray
        };
        #endregion

        #region CheckColumnTestValues  ========================================
        internal bool CheckColumnTestValue(RowX row, ColumnX column)
        {
            switch (column.Value.ValType)
            {
                case ValType.Bool:
                    column.Value.GetValue(row, out bool vBool);
                    if (!vBool) return false;
                    break;
                case ValType.IsArray: //is bool array
                    column.Value.GetValue(row, out bool[] vBools);
                    if (8 != vBools.Length) return false;
                    if (!vBools[0]) return false;
                    if (vBools[1]) return false;
                    if (!vBools[2]) return false;
                    if (vBools[3]) return false;
                    if (!vBools[4]) return false;
                    if (vBools[5]) return false;
                    if (!vBools[6]) return false;
                    if (vBools[7]) return false;
                    break;
                case ValType.Char:
                    column.Value.GetValue(row, out int vChar);
                    if ((int)'K' != vChar) return false;
                    break;
                case ValType.CharArray:
                    column.Value.GetValue(row, out string vChars);
                    if ("W, h, i, t, e, -, D, o, g" != vChars) return false;
                    break;
                case ValType.Byte:
                    column.Value.GetValue(row, out int vByte);
                    if (254 != vByte) return false;
                    break;
                case ValType.ByteArray:
                    column.Value.GetValue(row, out int[] vBytes);
                    if (3 != vBytes.Length) return false;
                    if (2 != vBytes[0]) return false;
                    if (5 != vBytes[1]) return false;
                    if (4 != vBytes[2]) return false;
                    break;
                case ValType.SByte:
                    column.Value.GetValue(row, out int vSByte);
                    if (-128 != vSByte) return false;
                    break;
                case ValType.SByteArray:
                    column.Value.GetValue(row, out int[] vSBytes);
                    if (3 != vSBytes.Length) return false;
                    if (-128 != vSBytes[0]) return false;
                    if (0 != vSBytes[1]) return false;
                    if (127 != vSBytes[2]) return false;
                    break;
                case ValType.Int16:
                    column.Value.GetValue(row, out int vInt16);
                    if (-32768 != vInt16) return false;
                    break;
                case ValType.Int16Array:
                    column.Value.GetValue(row, out int[] vInt16s);
                    if (3 != vInt16s.Length) return false;
                    if (-32768 != vInt16s[0]) return false;
                    if (0 != vInt16s[1]) return false;
                    if (32767 != vInt16s[2]) return false;
                    break;
                case ValType.UInt16:
                    column.Value.GetValue(row, out int vUInt16);
                    if (0xF0F0 != vUInt16) return false;
                    break;
                case ValType.UInt16Array:
                    column.Value.GetValue(row, out int[] vUInt16s);
                    if (3 != vUInt16s.Length) return false;
                    if (0xF0F0 != vUInt16s[0]) return false;
                    if (0 != vUInt16s[1]) return false;
                    if (0xF0F != vUInt16s[2]) return false;
                    break;
                case ValType.Int32:
                    column.Value.GetValue(row, out int vInt32);
                    if (-2147483648 != vInt32) return false;
                    break;
                case ValType.Int32Array:
                    column.Value.GetValue(row, out int[] vInt32s);
                    if (3 != vInt32s.Length) return false;
                    if (-2147483648 != vInt32s[0]) return false;
                    if (0 != vInt32s[1]) return false;
                    if (2147483647 != vInt32s[2]) return false;
                    break;
                case ValType.UInt32:
                    column.Value.GetValue(row, out int vUInt32);
                    if (0xF0F0F0F0 != (uint)vUInt32) return false;
                    break;
                case ValType.UInt32Array:
                    column.Value.GetValue(row, out int[] vUInt32s);
                    if (3 != vUInt32s.Length) return false;
                    if (0xF0F0F0F0 != (uint)vUInt32s[0]) return false;
                    if (0 != vUInt32s[1]) return false;
                    if (0x0F0F0F0F != vUInt32s[2]) return false;
                    break;
                case ValType.Int64:
                    column.Value.GetValue(row, out long vInt64);
                    if (-9223372036854775808 != vInt64) return false;
                    break;
                case ValType.Int64Array:
                    column.Value.GetValue(row, out long[] vInt64s);
                    if (3 != vInt64s.Length) return false;
                    if (-9223372036854775808 != vInt64s[0]) return false;
                    if (0 != vInt64s[1]) return false;
                    if (9223372036854775807 != vInt64s[2]) return false;
                    break;
                case ValType.UInt64:
                    column.Value.GetValue(row, out long vUInt64);
                    if (0xF0F0F0F0F0F0F0F0 != (ulong)vUInt64) return false;
                    break;
                case ValType.UInt64Array:
                    column.Value.GetValue(row, out long[] vUInt64s);
                    if (3 != vUInt64s.Length) return false;
                    if (0xF0F0F0F0F0F0F0F0 != (ulong)vUInt64s[0]) return false;
                    if (0 != vUInt64s[1]) return false;
                    if (0x0F0F0F0F0F0F0F0F != vUInt64s[2]) return false;
                    break;
                case ValType.Single:
                    column.Value.GetValue(row, out double vSingle);
                    if (-4.0E10 != vSingle) return false;
                    break;
                case ValType.SingleArray:
                    column.Value.GetValue(row, out double[] vSingles);
                    if (3 != vSingles.Length) return false;
                    if (-4.0E10 != vSingles[0]) return false;
                    if (0 != vSingles[1]) return false;
                    if (4.0E10 != vSingles[2]) return false;
                    break;
                case ValType.Double:
                    column.Value.GetValue(row, out double vDouble);
                    if (-1.7E308 != vDouble) return false;
                    break;
                case ValType.DoubleArray:
                    column.Value.GetValue(row, out double[] vDoubles);
                    if (3 != vDoubles.Length) return false;
                    if (-1.7E308 != vDoubles[0]) return false;
                    if (0 != vDoubles[1]) return false;
                    if (1.7E308 != vDoubles[2]) return false;
                    break;
                case ValType.Decimal:
                    column.Value.GetValue(row, out double vDecimal);
                    if (-5.100300456789 != vDecimal) return false;
                    break;
                case ValType.DecimalArray:
                    column.Value.GetValue(row, out double[] vDecimals);
                    if (3 != vDecimals.Length) return false;
                    if (-5.100300456789 != vDecimals[0]) return false;
                    if (0 != vDecimals[1]) return false;
                    if (5.100300456789 != vDecimals[2]) return false;
                    break;
                case ValType.DateTime:
                    column.Value.GetValue(row, out DateTime vDateTime);
                    if (DateTime.Parse("5/1/2008 8:30:52 AM") != vDateTime) return false;
                    break;
                case ValType.DateTimeArray:
                    column.Value.GetValue(row, out DateTime[] vDateTimes);
                    if (2 != vDateTimes.Length) return false;
                    if (DateTime.Parse("5/1/2008") != vDateTimes[0]) return false;
                    if (DateTime.Parse("6/2/2009") != vDateTimes[1]) return false;
                    break;
                case ValType.String:
                    column.Value.GetValue(row, out string vString);
                    if ("What is this?" != vString) return false;
                    break;
                case ValType.StringArray:
                    column.Value.GetValue(row, out string[] vStrings);
                    var len = vStrings.Length;
                    if (8 != len) return false;
                    if ("Can't" != vStrings[0]) return false;
                    if ("create" != vStrings[1]) return false;
                    if ("a" != vStrings[2]) return false;
                    if ("string" != vStrings[3]) return false;
                    if ("array" != vStrings[4]) return false;
                    if ("from" != vStrings[5]) return false;
                    if ("a" != vStrings[6]) return false;
                    if ("string" != vStrings[7]) return false;
                    break;
            }
            return true;
        }
        #endregion
    }
}
