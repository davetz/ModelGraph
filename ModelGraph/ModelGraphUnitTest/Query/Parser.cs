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
            RunTest(null, false, ParseError.InvalidText, 0, 0, string.Empty);
            RunTest("   ", false, ParseError.InvalidText, 0, 0, string.Empty);

            void RunTest(string inText, bool isValid, ParseError error, int index1, int index2, string outText)
            {
                var p = Parser.Create(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Error == error);
                Assert.IsTrue(p.Index1 == index1);
                Assert.IsTrue(p.Index2 == index2);
                Assert.IsTrue(p.Text == outText);
            }
        }
        #endregion

        #region InvalidString  ================================================
        [TestMethod]
        public void Parser_InvalidString()
        {
            RunTest("ab\"c", false, ParseError.InvalidString, 2, 4, "ab\"c");
            RunTest("a\"b\"c\"", false, ParseError.InvalidString, 5, 6, "a\"b\"c\"");

            void RunTest(string inText, bool isValid, ParseError error, int index1, int index2, string outText)
            {
                var p = Parser.Create(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Error == error);
                Assert.IsTrue(p.Index1 == index1);
                Assert.IsTrue(p.Index2 == index2);
                Assert.IsTrue(p.Text == outText);
            }
        }
        #endregion

        #region InvalidParens  ================================================
        [TestMethod]
        public void Parser_InvalidParens()
        {
            RunTest("012(4(67)9", false, ParseError.InvalidParens, 3, 10, "012(4(67)9");
            RunTest("0()(4\"))\"9", false, ParseError.InvalidParens, 3, 10, "0()(4\"))\"9");

            void RunTest(string inText, bool isValid, ParseError error, int index1, int index2, string outText)
            {
                var p = Parser.Create(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Error == error);
                Assert.IsTrue(p.Index1 == index1);
                Assert.IsTrue(p.Index2 == index2);
                Assert.IsTrue(p.Text == outText);
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
                var p = Parser.Create(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.StepType == childType);
                Assert.IsTrue(p.Text == childText);
            }
        }
        #endregion

        #region Commas  =======================================================
        [TestMethod]
        public void Parser_Commas()
        {
            var p = Parser.Create("5.4,3");
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
            var p = Parser.Create("((5 + 2),3)");
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 2);

            var q = p.Children[0];
            Assert.IsTrue(q.StepType == StepType.None);

            var r = p.Children[1];
            Assert.IsTrue(r.StepType == StepType.Integer);
        }
        #endregion

        #region Vector  =======================================================
        [TestMethod]
        public void Parser_Vector()
        {
            var p = Parser.Create("{(5 + 2),3}");
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 2);

            var q = p.Children[0];
            Assert.IsTrue(q.StepType == StepType.None);

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
                var p = Parser.Create(inText);
                Assert.IsTrue(p.IsValid == isValid);

                Assert.IsTrue(p.StepType == type2);
                Assert.IsTrue(p.Text == text2);
            }
        }
        #endregion

        #region Number  =======================================================
        [TestMethod]
        public void Parser_Number()
        {
            RunTest("13", true, 1, StepType.Integer, "13", typeof(BYTE), 13.0);
            RunTest("0.45", true, 1, StepType.Double, "0.45", typeof(DOUBLE), 0.45);
            RunTest(" 16 ", true, 1, StepType.Integer, "16", typeof(BYTE), 16.0);
            RunTest(" 0.55 ", true, 1, StepType.Double, "0.55", typeof(DOUBLE), 0.55);

            RunTest("0", true, 1, StepType.Integer, "0", typeof(BYTE), 0.0);
            RunTest("255", true, 1, StepType.Integer, "255", typeof(BYTE), 255.0);
            RunTest("256", true, 1, StepType.Integer, "256", typeof(INT16), 256.0);
            RunTest("32767", true, 1, StepType.Integer, "32767", typeof(INT16), 32767.0);
            RunTest("32768", true, 1, StepType.Integer, "32768", typeof(INT32), 32768.0);
            RunTest("2147483647", true, 1, StepType.Integer, "2147483647", typeof(INT32), 2147483647.0);
            RunTest("2147483648", true, 1, StepType.Integer, "2147483648", typeof(INT64), 2147483648.0);
            RunTest("2147483648.2147483648", true, 1, StepType.Double, "2147483648.2147483648", typeof(DOUBLE), 2147483648.2147483648);

            void RunTest(string inText, bool isValid, int count0, StepType childType, string childText, Type stepType, double val)
            {
                var p = Parser.Create(inText);
                Assert.IsTrue(p.IsValid == isValid);
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
            RunTest("Has", true, StepType.Has);
            RunTest("Ends", true, StepType.Ends);
            RunTest("Starts", true, StepType.Starts);
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
            RunTest(" Has ", true, StepType.Has);
            RunTest(" Ends ", true, StepType.Ends);
            RunTest(" Starts ", true, StepType.Starts);

            void RunTest(string inText, bool isValid, StepType stepType)
            {
                var p = Parser.Create(inText);
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
