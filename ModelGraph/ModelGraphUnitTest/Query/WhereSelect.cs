using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelGraphLibrary;
using System;
using System.Collections.Generic;

namespace ModelGraphUnitTest
{/*
    Test the WhereSelect class
 */
    public partial class ModelGraphLibraryTest
    {
        #region Test1
        [TestMethod]
        public void WhereSelect_BuildTestData()
        {
            var td = TestData.Instance;
            var td_0 = td.TestData_0;
            var tbl = td_0.Table;
            var cols = td_0.Columns;
            Assert.IsTrue(cols.Count == (int)ModelGraphLibrary.ValueType.MaximumType);
        }
        #endregion

        #region Numeric  ======================================================
        [TestMethod]
        public void WhereSelect_Numeric()
        {
            RunTest("1", 1);
            RunTest("1 + 2", 3);
            RunTest("5 * 3 + 2", 17);
            RunTest("5 * (3 + 2)", 25);
            RunTest("2 - 1.5", 0.5);
            RunTest("2 + 2 + (5 * (3 + (2 * 3))) / 2 - 1.5", 25);


            void RunTest(string text, double value)
            {
                var w = new WhereSelect(text);
                Assert.IsTrue(w.IsValid);

                w.Validate(null);
                Assert.IsTrue(w.IsValid);

                var txt = w.InputString;
                Assert.IsTrue(txt == text);

                w.GetValue(null, out double val);
                Assert.IsTrue(val == value);
            }

        }
        #endregion

        #region Negate1  ======================================================
        [TestMethod]
        public void WhereSelect_Negate1()
        {
            RunTest("0", 0);
            RunTest("-1", -1);
            RunTest("1 * 0", 0);
            RunTest("-1 * -1", 1);
            RunTest("-5 + -12", -17);
            RunTest("5 * -3 + 2", -13);
            RunTest("-5 * (3 + 2)", -25);
            RunTest("-5 * -3 / -2 - -4", -3.5);
            RunTest("-5 * -3 / -2 + -4", -11.5);
            RunTest("-5 * -3 / -2 - 7 / 4", -9.25);
            RunTest("-5 * -3 / -2 - 7 / -4", -5.75);
            RunTest("2 + 2 + (5 * -(3 + (2 * 3))) / 2", -18.5);
            RunTest("2 + 2 + -(5 * -(3 + (-2 * 3))) / 2", -3.5);
            RunTest("-2 + -1 + -(-5 * -(-3 + (-2 * -3))) / -2", 4.5);


            void RunTest(string text, double value)
            {
                var w = new WhereSelect(text);
                Assert.IsTrue(w.IsValid);

                w.Validate(null);
                Assert.IsTrue(w.IsValid);

                var txt = w.InputString;
                Assert.IsTrue(txt == text);

                w.GetValue(null, out double val);
                Assert.IsTrue(val == value);
            }

        }
        #endregion

        #region NegateBool  ===================================================
        [TestMethod]
        public void WhereSelect_NegateBool()
        {
            var td = TestData.Instance;
            var td_0 = td.TestData_0;
            var tbl = td_0.Table;
            var row = tbl.Items[0];

            RunTest("cBoolA", "cBoolA", false);
            RunTest("cBoolB", "cBoolB", true);

            RunTest("!cBoolA", "!cBoolA", true);
            RunTest("!cBoolB", "!cBoolB", false);

            void RunTest(string inText, string outText, bool value)
            {
                var w = new WhereSelect(inText);
                Assert.IsTrue(w.IsValid);

                w.Validate(tbl);
                Assert.IsTrue(w.IsValid);

                var txt = w.InputString;
                Assert.IsTrue(txt == outText);


                w.GetValue(row, out bool val);
                Assert.IsTrue(val == value);
            }
        }
        #endregion

        #region NegateByte  ===================================================
        [TestMethod]
        public void WhereSelect_NegateField()
        {
            var td = TestData.Instance;
            var td_0 = td.TestData_0;
            var tbl = td_0.Table;
            var row = tbl.Items[0];

            RunTestByte("cByteA", "cByteA", 0);
            RunTestByte("cByteB", "cByteB", 0xFF);

            RunTestByte("~cByteA", "~cByteA", 0xFF);
            RunTestByte("~cByteB", "~cByteB", 0);

            RunTestUInt16("cByteA", "cByteA", 0);
            RunTestUInt16("cByteB", "cByteB", 0xFF);

            RunTestUInt16("~cByteA", "~cByteA", 0xFFFF);
            RunTestUInt16("~cByteB", "~cByteB", 0xFF00);

            RunTestUInt32("cByteA", "cByteA", 0);
            RunTestUInt32("cByteB", "cByteB", 0xFF);

            RunTestUInt32("~cByteA", "~cByteA", 0xFFFFFFFF);
            RunTestUInt32("~cByteB", "~cByteB", 0xFFFFFF00);

            RunTestUInt64("cByteA", "cByteA", 0);
            RunTestUInt64("cByteB", "cByteB", 0xFF);

            RunTestUInt64("~cByteA", "~cByteA", 0xFFFFFFFFFFFFFFFF);
            RunTestUInt64("~cByteB", "~cByteB", 0xFFFFFFFFFFFFFF00);

            void RunTestByte(string inText, string outText, byte value)
            {
                var w = new WhereSelect(inText);
                Assert.IsTrue(w.IsValid);

                w.Validate(tbl);
                Assert.IsTrue(w.IsValid);

                var txt = w.InputString;
                Assert.IsTrue(txt == outText);


                w.GetValue(row, out byte val);
                Assert.IsTrue(val == value);
            }
            void RunTestUInt16(string inText, string outText, ushort value)
            {
                var w = new WhereSelect(inText);
                Assert.IsTrue(w.IsValid);

                w.Validate(tbl);
                Assert.IsTrue(w.IsValid);

                var txt = w.InputString;
                Assert.IsTrue(txt == outText);


                w.GetValue(row, out ushort val);
                Assert.IsTrue(val == value);
            }
            void RunTestUInt32(string inText, string outText, uint value)
            {
                var w = new WhereSelect(inText);
                Assert.IsTrue(w.IsValid);

                w.Validate(tbl);
                Assert.IsTrue(w.IsValid);

                var txt = w.InputString;
                Assert.IsTrue(txt == outText);


                w.GetValue(row, out uint val);
                Assert.IsTrue(val == value);
            }
            void RunTestUInt64(string inText, string outText, ulong value)
            {
                var w = new WhereSelect(inText);
                Assert.IsTrue(w.IsValid);

                w.Validate(tbl);
                Assert.IsTrue(w.IsValid);

                var txt = w.InputString;
                Assert.IsTrue(txt == outText);


                w.GetValue(row, out ulong val);
                Assert.IsTrue(val == value);
            }
        }
        #endregion
    }
}

