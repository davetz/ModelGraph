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

        #region GetText  ======================================================
        [TestMethod]
        public void WhereSelect_NumericExpression()
        {
            RunTest("1", 1);
            RunTest("1 + 2", 3);
            RunTest("5 * 3 + 2", 17);
            RunTest("5 * (3 + 2)", 25);
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

        #region GetText  ======================================================
        [TestMethod]
        public void WhereSelect_Negate()
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
    }
}
