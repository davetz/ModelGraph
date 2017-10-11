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

        #region ParserOperator  ===============================================
        [TestMethod]
        public void Parser_Operator()
        {
            RunTest("|", StepType.Or1);
            RunTest("||", StepType.Or2);
            RunTest("&", StepType.And1);
            RunTest("&&", StepType.And2);
            RunTest("!", StepType.Not);
            RunTest("+", StepType.Plus);
            RunTest("-", StepType.Minus);
            RunTest("=", StepType.Equals);
            RunTest("==", StepType.Equals);
            RunTest("~", StepType.Negate);
            RunTest("/", StepType.Divide);
            RunTest("*", StepType.Multiply);
            RunTest("<", StepType.LessThan);
            RunTest(">", StepType.GreaterThan);
            RunTest(">=", StepType.NotLessThan);
            RunTest("<=", StepType.NotGreaterThan);
            RunTest("Contains", StepType.Contains);
            RunTest("EndsWith", StepType.EndsWith);
            RunTest("StartsWith", StepType.StartsWith);
            RunTest(" | ", StepType.Or1);
            RunTest(" || ", StepType.Or2);
            RunTest(" & ", StepType.And1);
            RunTest(" && ", StepType.And2);
            RunTest(" ! ", StepType.Not);
            RunTest(" + ", StepType.Plus);
            RunTest(" - ", StepType.Minus);
            RunTest(" = ", StepType.Equals);
            RunTest(" == ", StepType.Equals);
            RunTest(" ~ ", StepType.Negate);
            RunTest(" / ", StepType.Divide);
            RunTest(" * ", StepType.Multiply);
            RunTest(" < ", StepType.LessThan);
            RunTest(" > ", StepType.GreaterThan);
            RunTest(" >= ", StepType.NotLessThan);
            RunTest(" <= ", StepType.NotGreaterThan);
            RunTest(" Has ", StepType.Contains);
            RunTest(" Ends ", StepType.EndsWith);
            RunTest(" Starts ", StepType.StartsWith);

            void RunTest(string inText, StepType stepType)
            {
                var p = Parser.CreateExpressionTree(inText);
                Assert.IsTrue(p.IsValid);
            }
        }
        #endregion
    }
}
