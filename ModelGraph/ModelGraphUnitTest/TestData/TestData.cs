using ModelGraphLibrary;
using System;

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

        #region Singleton  ====================================================
        /// <summary>
        /// Create the one instance of RootChef
        /// </summary>
        private TestData()
        {
            RootChef = new Chef();

            CreateDataChef_0();
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

        #region DataChef_0  ===================================================
        private void CreateDataChef_0()
        {
            var dc = new Chef(RootChef);

            var t1 = new TableX(dc.T_TableXStore);
            t1.SetCapacity(100);
            for (int i = 0; i < 10; i++) { new RowX(t1); }

            var c1 = new ColumnX(dc.T_ColumnXStore);
            c1.Initialize(ModelGraphLibrary.ValueType.String, null, 100);
            c1.Name = "Id";           
            dc.R_TableX_ColumnX.SetLink(t1, c1);

            var c2 = new ColumnX(dc.T_ColumnXStore);
            c2.Initialize(ModelGraphLibrary.ValueType.UInt16, null, 100);
            c1.Name = "I2Val";
            dc.R_TableX_ColumnX.SetLink(t1, c2);
        }
        #endregion
    }
}
