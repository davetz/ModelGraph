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

            //RunTestByte("cByteA", "cByteA", 0);
            //RunTestByte("cByteB", "cByteB", 0xFF);

            //RunTestByte("~cByteA", "~cByteA", 0xFF);
            //RunTestByte("~cByteB", "~cByteB", 0);

            //RunTestInt16("cByteA", "cByteA", 0);
            //RunTestInt16("cByteB", "cByteB", 0xFF);

            RunTestInt16("~cByteA", "~cByteA", 0xFF);
            RunTestInt16("~cByteB", "~cByteB", 0);

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
            void RunTestInt16(string inText, string outText, short value)
            {
                var w = new WhereSelect(inText);
                Assert.IsTrue(w.IsValid);

                w.Validate(tbl);
                Assert.IsTrue(w.IsValid);

                var txt = w.InputString;
                Assert.IsTrue(txt == outText);


                w.GetValue(row, out short val);
                Assert.IsTrue(val == value);
            }
            void RunTestInt32(string inText, string outText, int value)
            {
                var w = new WhereSelect(inText);
                Assert.IsTrue(w.IsValid);

                w.Validate(tbl);
                Assert.IsTrue(w.IsValid);

                var txt = w.InputString;
                Assert.IsTrue(txt == outText);


                w.GetValue(row, out int val);
                Assert.IsTrue(val == value);
            }
            void RunTestInt64(string inText, string outText, long value)
            {
                var w = new WhereSelect(inText);
                Assert.IsTrue(w.IsValid);

                w.Validate(tbl);
                Assert.IsTrue(w.IsValid);

                var txt = w.InputString;
                Assert.IsTrue(txt == outText);


                w.GetValue(row, out long val);
                Assert.IsTrue(val == value);
            }
        }
        #endregion
    }
}

