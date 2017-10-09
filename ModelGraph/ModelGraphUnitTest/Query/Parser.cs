using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelGraphLibrary;
using System;

namespace ModelGraphUnitTest
{/*
    Test the Parser class
 */
    public partial class ModelGraphLibraryTest
    {
        #region NullOrWhiteSpaceText  =========================================
        [TestMethod]
        public void Parser_NullOrWhiteSpaceText()
        {
            RunTest(null, false, StepError.InvalidText, 0, 0, string.Empty);
            RunTest("   ", false, StepError.InvalidText, 0, 0, string.Empty);

            void RunTest(string inText, bool isValid, StepError error, int index1, int index2, string outText)
            {
                var p = Parser.CreateExpressionTree(inText);
            }
        }
        #endregion

        #region String  =======================================================
        [TestMethod]
        public void Parser_String()
        {
            RunTest("\"ok good\"", true, StepType.String, "ok good");
            RunTest("   \"0.4-+=/*&|\" ", true, StepType.String, "0.4-+=/*&|");

            void RunTest(string inText, bool isValid, StepType childType, string childText)
            {
                var p = Parser.CreateExpressionTree(inText);
            }
        }
        #endregion

        #region Commas  =======================================================
        [TestMethod]
        public void Parser_Commas()
        {
            var p = Parser.CreateExpressionTree("5.4,3");
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 2);

            var q = p.Children[0];
            Assert.IsTrue(q.StepType == StepType.Double);

            var r = p.Children[1];
            Assert.IsTrue(r.StepType == StepType.Integer);
        }
        #endregion

        #region Lists  ========================================================
        [TestMethod]
        public void Parser_List()
        {
            var p = Parser.CreateExpressionTree("((5 + 2),3)");
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 2);

            var q = p.Children[0];
            Assert.IsTrue(q.StepType == StepType.Parse);

            var r = p.Children[1];
            Assert.IsTrue(r.StepType == StepType.Integer);
        }
        #endregion

        #region Vector  =======================================================
        [TestMethod]
        public void Parser_Vector()
        {
            var p = Parser.CreateExpressionTree("{(5 + 2),3}");
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 2);

            var q = p.Children[0];
            Assert.IsTrue(q.StepType == StepType.Parse);

            var r = p.Children[1];
            Assert.IsTrue(r.StepType == StepType.Integer);
        }
        #endregion

        #region Parens  =======================================================
        [TestMethod]
        public void Parser_Parens()
        {
            RunTest("((\"ok (good)\"))", true, StepType.String, "ok (good)");
            RunTest(" (  ( \"ok good\" ) ) ", true, StepType.String, "ok good");
            RunTest(" (  (\"ok good\")) ", true, StepType.String, "ok good");
            RunTest("((\"ok good\"  )  ) ", true, StepType.String, "ok good");

            void RunTest(string inText, bool isValid, StepType type2, string text2)
            {
                var p = Parser.CreateExpressionTree(inText);
                Assert.IsTrue(p.IsValid == isValid);
            }
        }
        #endregion

        #region Number  =======================================================
        [TestMethod]
        public void Parser_Number()
        {
            RunTest("13", StepType.Integer, "13", typeof(SBYTE), 13.0);
            RunTest("0.45", StepType.Double, "0.45", typeof(DOUBLE), 0.45);
            RunTest(" 16 ", StepType.Integer, "16", typeof(SBYTE), 16.0);
            RunTest(" 0.55 ", StepType.Double, "0.55", typeof(DOUBLE), 0.55);

            RunTest("0x0", StepType.BitField, "0x0", typeof(BYTE), 0x0);

            RunTest("0x55", StepType.BitField, "0x55", typeof(BYTE), 0x55);
            RunTest("0xFF", StepType.BitField, "0xFF", typeof(BYTE), byte.MaxValue);

            RunTest("0x100 ", StepType.BitField, "0x100", typeof(UINT16), 0x100);
            RunTest("0xFFFF", StepType.BitField, "0xFFFF", typeof(UINT16), ushort.MaxValue);

            RunTest("0x10000", StepType.BitField, "0x10000", typeof(UINT32), 0x10000);
            RunTest("0xFFFFFFFF", StepType.BitField, "0xFFFFFFFF", typeof(UINT32), uint.MaxValue);

            RunTest("0x100000000", StepType.BitField, "0x100000000", typeof(UINT64), 0x100000000);
            RunTest("0x1000000000000000", StepType.BitField, "0x1000000000000000", typeof(UINT64), 0x1000000000000000);
            RunTest("0x101010101010F0E0", StepType.BitField, "0x101010101010F0E0", typeof(UINT64), 0x101010101010F0E0);
            RunTest("0xFFFFFFFFFFFFFFFF", StepType.BitField, "0xFFFFFFFFFFFFFFFF", typeof(UINT64), ulong.MaxValue);

            RunTest("0", StepType.Integer, "0", typeof(SBYTE), 0.0);
            RunTest("127", StepType.Integer, "127", typeof(SBYTE), 127.0);
            RunTest("128", StepType.Integer, "128", typeof(INT16), 128.0);
            RunTest("32767", StepType.Integer, "32767", typeof(INT16), 32767.0);
            RunTest("32768", StepType.Integer, "32768", typeof(INT32), 32768.0);
            RunTest("2147483647", StepType.Integer, "2147483647", typeof(INT32), 2147483647.0);
            RunTest("2147483648", StepType.Integer, "2147483648", typeof(INT64), 2147483648.0);
            RunTest("2147483648.2147483648", StepType.Double, "2147483648.2147483648", typeof(DOUBLE), 2147483648.2147483648);

            void RunTest(string inText, StepType childType, string childText, Type stepType, double val)
            {
                var p = Parser.CreateExpressionTree(inText);
                Assert.IsTrue(p.IsValid);
                Assert.IsTrue(p.StepType == childType);
                Assert.IsTrue(p.Text == childText);

                p.Step.GetValue(out double tval);
                Assert.IsTrue(tval == val);

                Assert.IsTrue(p.Step != null);
                Assert.IsTrue(stepType == p.Step.GetType());
            }
        }
        #endregion

        #region ParserOperator  ===============================================
        [TestMethod]
        public void Parser_Operator()
        {
            RunTest("|", true, StepType.Or1);
            RunTest("||", true, StepType.Or2);
            RunTest("&", true, StepType.And1);
            RunTest("&&", true, StepType.And2);
            RunTest("!", true, StepType.Not);
            RunTest("+", true, StepType.Plus);
            RunTest("-", true, StepType.Minus);
            RunTest("=", true, StepType.Equals);
            RunTest("==", true, StepType.Equals);
            RunTest("~", true, StepType.Negate);
            RunTest("/", true, StepType.Divide);
            RunTest("*", true, StepType.Multiply);
            RunTest("<", true, StepType.LessThan);
            RunTest(">", true, StepType.GreaterThan);
            RunTest(">=", true, StepType.NotLessThan);
            RunTest("<=", true, StepType.NotGreaterThan);
            RunTest("Has", true, StepType.Contains);
            RunTest("Ends", true, StepType.EndsWith);
            RunTest("Starts", true, StepType.StartsWith);
            RunTest(" | ", true, StepType.Or1);
            RunTest(" || ", true, StepType.Or2);
            RunTest(" & ", true, StepType.And1);
            RunTest(" && ", true, StepType.And2);
            RunTest(" ! ", true, StepType.Not);
            RunTest(" + ", true, StepType.Plus);
            RunTest(" - ", true, StepType.Minus);
            RunTest(" = ", true, StepType.Equals);
            RunTest(" == ", true, StepType.Equals);
            RunTest(" ~ ", true, StepType.Negate);
            RunTest(" / ", true, StepType.Divide);
            RunTest(" * ", true, StepType.Multiply);
            RunTest(" < ", true, StepType.LessThan);
            RunTest(" > ", true, StepType.GreaterThan);
            RunTest(" >= ", true, StepType.NotLessThan);
            RunTest(" <= ", true, StepType.NotGreaterThan);
            RunTest(" Has ", true, StepType.Contains);
            RunTest(" Ends ", true, StepType.EndsWith);
            RunTest(" Starts ", true, StepType.StartsWith);

            void RunTest(string inText, bool isValid, StepType stepType)
            {
                var p = Parser.CreateExpressionTree(inText);
                Assert.IsTrue(p.IsValid == isValid);
                if (isValid)
                {
                    Assert.IsTrue(p.StepType == stepType);
                }
            }
        }
        #endregion
    }
}
