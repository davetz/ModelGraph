
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelGraphLibrary;

namespace ModelGraphUnitTest
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void ParserNullOrWhiteSpaceText()
        {
            RunTest(null, false, ParseError.InvalidText, 0, 0, string.Empty);
            RunTest("   ", false, ParseError.InvalidText, 0, 0, string.Empty);

            void RunTest(string inText, bool isValid, ParseError error, int index1, int index2, string outText)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.ParseError == error);
                Assert.IsTrue(p.Index1 == index1);
                Assert.IsTrue(p.Index2 == index2);
                Assert.IsTrue(p.Text == outText);
            }
        }


        [TestMethod]
        public void ParserInvalidString()
        {
            RunTest("ab\"c", false, ParseError.InvalidString, 2, 4, "ab\"c");
            RunTest("a\"b\"c\"", false, ParseError.InvalidString, 5, 6, "a\"b\"c\"");

            void RunTest(string inText, bool isValid, ParseError error, int index1, int index2, string outText)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.ParseError == error);
                Assert.IsTrue(p.Index1 == index1);
                Assert.IsTrue(p.Index2 == index2);
                Assert.IsTrue(p.Text == outText);
            }
        }
        [TestMethod]
        public void ParserInvalidParens()
        {
            RunTest("012(4(67)9", false, ParseError.InvalidParens, 3, 10, "012(4(67)9");
            RunTest("0()(4\"))\"9", false, ParseError.InvalidParens, 3, 10, "0()(4\"))\"9");

            void RunTest(string inText, bool isValid, ParseError error, int index1, int index2, string outText)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.ParseError == error);
                Assert.IsTrue(p.Index1 == index1);
                Assert.IsTrue(p.Index2 == index2);
                Assert.IsTrue(p.Text == outText);
            }
        }
        [TestMethod]
        public void ParserString()
        {
            RunTest("s\"should work\"", true, 1, ParseType.String, "should work");
            RunTest("   \"0.4-+=/*&|\" ", true, 1, ParseType.String, "0.4-+=/*&|");

            void RunTest(string inText, bool isValid, int childCount, ParseType childType, string childText)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Children.Count == childCount);
                var q = p.Children[0];
                Assert.IsTrue(q.ParseType == childType);
                Assert.IsTrue(q.Text == childText);
            }
        }
        [TestMethod]
        public void ParserParams()
        {
            RunTest("(\"should work\")", true, 1, 1, ParseType.String, "should work");
            RunTest(" (  \"should work\"  ) ", true, 1, 1, ParseType.String, "should work");
            RunTest(" (  \"should work\") ", true, 1, 1, ParseType.String, "should work");

            void RunTest(string inText, bool isValid, int childCount, int grandChildCount, ParseType grandChildType, string grandChildText)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Children.Count == childCount);

                var q = p.Children[0];
                Assert.IsTrue(q.Children.Count == grandChildCount);

                var r = q.Children[0];
                Assert.IsTrue(r.ParseType == grandChildType);
                Assert.IsTrue(r.Text == grandChildText);
            }
        }
        [TestMethod]
        public void ParserNumber()
        {
            RunTest("13", true, 1, ParseType.Integer, "13");
            RunTest("0.45", true, 1, ParseType.Double, "0.45");
            RunTest(" 16 ", true, 1, ParseType.Integer, "16");
            RunTest(" 0.55 ", true, 1, ParseType.Double, "0.55");

            void RunTest(string inText, bool isValid, int childCount, ParseType childType, string childText)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Children.Count == childCount);

                var q = p.Children[0];
                Assert.IsTrue(q.ParseType == childType);
                Assert.IsTrue(q.Text == childText);
            }

        }
        [TestMethod]
        public void ParserParenStringNumber()
        {
            var text = " ( \" rip 0 \" 0.45 ) ";
            var p = new Parser(text);
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 1);

            var q = p.Children[0];
            Assert.IsTrue(q.Children.Count == 2);

            var r = q.Children[0];
            Assert.IsTrue(r.Text == " rip 0 ");

            var s = q.Children[1];
            Assert.IsTrue(s.Text == "0.45");
        }
        [TestMethod]
        public void ParserOperator()
        {
            RunTest("|", true, 1, ParseType.OrOperator);
            RunTest("||", true, 1, ParseType.OrOperator);
            RunTest("&", true, 1, ParseType.AndOperator);
            RunTest("&&", true, 1, ParseType.AndOperator);
            RunTest("!", true, 1, ParseType.NotOperator);
            RunTest("+", true, 1, ParseType.PlusOperator);
            RunTest("-", true, 1, ParseType.MinusOperator);
            RunTest("=", true, 1, ParseType.EqualsOperator);
            RunTest("==", true, 1, ParseType.EqualsOperator);
            RunTest("~", true, 1, ParseType.NegateOperator);
            RunTest("/", true, 1, ParseType.DivideOperator);
            RunTest("*", true, 1, ParseType.MultiplyOpartor);
            RunTest("<", true, 1, ParseType.LessThanOperator);
            RunTest(">", true, 1, ParseType.GreaterThanOperator);
            RunTest(">=", true, 1, ParseType.NotLessThanOperator);
            RunTest("<=", true, 1, ParseType.NotGreaterThanOperator);
            RunTest(" | ", true, 1, ParseType.OrOperator);
            RunTest(" || ", true, 1, ParseType.OrOperator);
            RunTest(" & ", true, 1, ParseType.AndOperator);
            RunTest(" && ", true, 1, ParseType.AndOperator);
            RunTest(" ! ", true, 1, ParseType.NotOperator);
            RunTest(" + ", true, 1, ParseType.PlusOperator);
            RunTest(" - ", true, 1, ParseType.MinusOperator);
            RunTest(" = ", true, 1, ParseType.EqualsOperator);
            RunTest(" == ", true, 1, ParseType.EqualsOperator);
            RunTest(" ~ ", true, 1, ParseType.NegateOperator);
            RunTest(" / ", true, 1, ParseType.DivideOperator);
            RunTest(" * ", true, 1, ParseType.MultiplyOpartor);
            RunTest(" < ", true, 1, ParseType.LessThanOperator);
            RunTest(" > ", true, 1, ParseType.GreaterThanOperator);
            RunTest(" >= ", true, 1, ParseType.NotLessThanOperator);
            RunTest(" <= ", true, 1, ParseType.NotGreaterThanOperator);

            void RunTest(string inText, bool isValid, int childCount, ParseType childType)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                if (isValid)
                {
                    Assert.IsTrue(p.Children.Count == childCount);

                    var q = p.Children[0];
                    Assert.IsTrue(q.ParseType == childType);
                }
            }
        }
    }
}
