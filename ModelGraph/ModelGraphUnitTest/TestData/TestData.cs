using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelGraphLibrary;

namespace ModelGraphUnitTest
{/*
    Singleton of test data for ModelGraphUnitTest
 */
    public sealed class TestData : IDisposable
    {
        private bool _disposed;
        private static volatile TestData _instance;
        private static readonly object _syncLock = new object();
        public Chef RootChef { get; private set; }
        public (TableX Table, List<(ColumnX A, ColumnX B)> Columns) TestData_0 { get; private set; }

        #region Singleton  ====================================================
        /// <summary>
        /// Create the one instance of RootChef
        /// </summary>
        private TestData()
        {
            RootChef = new Chef();

            CreateTestData_0();
        }
        public static TestData Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (_syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new TestData();
                    }
                }
                return _instance;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _instance = null;
                // Dispose managed resources.
            }
            // Dispose unmanaged resources.
            _disposed = true;
        }
        ~TestData()
        {
            Dispose(false);
        }
        #endregion

        #region TestData_0  ===================================================
        private void CreateTestData_0()
        {
            var dc = new Chef(RootChef);

            var MaxRows = 36; // max number of rows for the test data
            var MaxCols = (int)ModelGraphLibrary.ValueType.MaximumType;

            //=================================================================
            // Create table 

            var tbl = new TableX(dc.T_TableXStore);
            tbl.Name = "T1";
            tbl.SetCapacity(MaxRows);

            //=================================================================
            // Create test columns(A and B) for each of all posible ValueTypes

            var cols = new List<(ColumnX A, ColumnX B)>(MaxCols);
            for (int i = 0; i < MaxCols; i++)
            {
                var type = (ModelGraphLibrary.ValueType)i;
                var name = Enum.GetName(typeof(ModelGraphLibrary.ValueType), i);
                var a = CreateColumn($"c{name}A", type);
                var b = CreateColumn($"c{name}B", type);
                cols.Add((a, b));
            }
            const string strA = "abcdefghijklmnopqrstuvwxyz0123456789";
            const string strB = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            //=================================================================
            // Create the test data rows

            for (int i = 0; i < MaxRows; i++)
            {
                var r = new RowX(tbl);
                if (i == 0)
                {
                    int j = (int)ModelGraphLibrary.ValueType.Bool;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{false}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{true}"));
                    j = (int)ModelGraphLibrary.ValueType.Char;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, "a"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, "Z"));
                    j = (int)ModelGraphLibrary.ValueType.Byte;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{byte.MinValue}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{byte.MaxValue}"));
                    j = (int)ModelGraphLibrary.ValueType.SByte;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{sbyte.MinValue}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{sbyte.MaxValue}"));
                    j = (int)ModelGraphLibrary.ValueType.Int16;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{short.MinValue}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{short.MaxValue}"));
                    j = (int)ModelGraphLibrary.ValueType.Int32;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{int.MinValue}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{int.MaxValue}"));
                    j = (int)ModelGraphLibrary.ValueType.Int64;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{long.MinValue}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{long.MaxValue}"));
                    j = (int)ModelGraphLibrary.ValueType.UInt16;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{ushort.MinValue}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{ushort.MaxValue}"));
                    j = (int)ModelGraphLibrary.ValueType.UInt32;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{uint.MinValue}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{uint.MaxValue}"));
                    j = (int)ModelGraphLibrary.ValueType.UInt64;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{ulong.MinValue}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{ulong.MaxValue}"));
                    j = (int)ModelGraphLibrary.ValueType.Single;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{float.MinValue}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{float.MaxValue}"));
                    j = (int)ModelGraphLibrary.ValueType.Double;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{float.MinValue}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{float.MaxValue}"));
                    j = (int)ModelGraphLibrary.ValueType.Decimal;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{decimal.MinValue}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{decimal.MaxValue}"));
                    j = (int)ModelGraphLibrary.ValueType.Guid;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{Guid.Empty}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"7E96CD53-9042-40A3-B808-2D9BC49D78C0"));
                    j = (int)ModelGraphLibrary.ValueType.DateTime;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{DateTime.MinValue}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{DateTime.MaxValue}"));
                    j = (int)ModelGraphLibrary.ValueType.String;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, strA));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, strB));
                }
                else
                {
                    int j = (int)ModelGraphLibrary.ValueType.Bool;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{!(i % 2 == 0)}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{(i % 2 == 0)}"));
                    j = (int)ModelGraphLibrary.ValueType.Char;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, strA.Substring(i, 1)));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, strB.Substring(i, 1)));
                    j = (int)ModelGraphLibrary.ValueType.Byte;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{(i * 2)}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{(i * 3)}"));
                    j = (int)ModelGraphLibrary.ValueType.SByte;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{-(i * 2)}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{(i * 3)}"));
                    j = (int)ModelGraphLibrary.ValueType.Int16;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{-(i * 4)}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{(i * 5)}"));
                    j = (int)ModelGraphLibrary.ValueType.Int32;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{-(i * 6)}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{(i * 7)}"));
                    j = (int)ModelGraphLibrary.ValueType.Int64;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{-(i * 8)}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{(i * 9)}"));
                    j = (int)ModelGraphLibrary.ValueType.UInt16;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{(i * 4)}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{(i * 5)}"));
                    j = (int)ModelGraphLibrary.ValueType.UInt32;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{(i * 6)}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{(i * 7)}"));
                    j = (int)ModelGraphLibrary.ValueType.UInt64;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{(i * 8)}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{(i * 9)}"));
                    j = (int)ModelGraphLibrary.ValueType.Single;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{(i * 2.2)}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{(i * 3.3)}"));
                    j = (int)ModelGraphLibrary.ValueType.Double;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{(i * 4.44)}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{(i * 5.55)}"));
                    j = (int)ModelGraphLibrary.ValueType.Decimal;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{(i * 6.66)}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{(i * 7.77)}"));
                    j = (int)ModelGraphLibrary.ValueType.Guid;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{Guid.NewGuid()}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{Guid.NewGuid()}"));
                    j = (int)ModelGraphLibrary.ValueType.DateTime;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, $"{DateTime.Now}"));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, $"{DateTime.Now}"));
                    j = (int)ModelGraphLibrary.ValueType.String;
                    Assert.IsTrue(cols[j].A.TrySetValue(r, strA.Substring(0, i)));
                    Assert.IsTrue(cols[j].B.TrySetValue(r, strB.Substring(0, i)));
                }
            }
            TestData_0 = (tbl, cols);

            ColumnX CreateColumn(string name, ModelGraphLibrary.ValueType type)
            {
                var c = new ColumnX(dc.T_ColumnXStore);
                c.Initialize(type, null, MaxRows);
                c.Name = name;
                dc.R_TableX_ColumnX.SetLink(tbl, c);
                return c;
            }
        }
    }
    #endregion
}

