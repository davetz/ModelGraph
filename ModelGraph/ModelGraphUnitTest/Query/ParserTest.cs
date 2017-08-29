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
            RunTest("\"should work\"", true, 1, StepType.String, "should work");
            RunTest("   \"0.4-+=/*&|\" ", true, 1, StepType.String, "0.4-+=/*&|");

            void RunTest(string inText, bool isValid, int childCount, StepType childType, string childText)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Children.Count == childCount);
                var q = p.Children[0];
                Assert.IsTrue(q.StepType == childType);
                Assert.IsTrue(q.Text == childText);
            }
        }
        [TestMethod]
        public void ParserParens()
        {
            RunTest("(\"should work\")", true, 1, 1, StepType.String, "should work");
            RunTest(" (  \"should work\"  ) ", true, 1, 1, StepType.String, "should work");
            RunTest(" (  \"should work\") ", true, 1, 1, StepType.String, "should work");

            void RunTest(string inText, bool isValid, int childCount, int grandChildCount, StepType grandChildType, string grandChildText)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Children.Count == childCount);

                var q = p.Children[0];
                Assert.IsTrue(q.Children.Count == grandChildCount);

                var r = q.Children[0];
                Assert.IsTrue(r.StepType == grandChildType);
                Assert.IsTrue(r.Text == grandChildText);
            }
        }
        [TestMethod]
        public void ParserNumber()
        {
            RunTest("13", true, 1, StepType.Integer, "13");
            RunTest("0.45", true, 1, StepType.Double, "0.45");
            RunTest(" 16 ", true, 1, StepType.Integer, "16");
            RunTest(" 0.55 ", true, 1, StepType.Double, "0.55");

            void RunTest(string inText, bool isValid, int childCount, StepType childType, string childText)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Children.Count == childCount);

                var q = p.Children[0];
                Assert.IsTrue(q.StepType == childType);
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
            RunTest("|", true, 1, StepType.BitOr);
            RunTest("||", true, 1, StepType.Or);
            RunTest("&", true, 1, StepType.BitAnd);
            RunTest("&&", true, 1, StepType.And);
            RunTest("!", true, 1, StepType.Not);
            RunTest("+", true, 1, StepType.Plus);
            RunTest("-", true, 1, StepType.Minus);
            RunTest("=", true, 1, StepType.Equals);
            RunTest("==", true, 1, StepType.Equals);
            RunTest("~", true, 1, StepType.Negate);
            RunTest("/", true, 1, StepType.Divide);
            RunTest("*", true, 1, StepType.Multiply);
            RunTest("<", true, 1, StepType.LessThan);
            RunTest(">", true, 1, StepType.GreaterThan);
            RunTest(">=", true, 1, StepType.NotLessThan);
            RunTest("<=", true, 1, StepType.NotGreaterThan);
            RunTest("Has", true, 1, StepType.Has);
            RunTest("Ends", true, 1, StepType.Ends);
            RunTest("Starts", true, 1, StepType.Starts);
            RunTest(" | ", true, 1, StepType.BitOr);
            RunTest(" || ", true, 1, StepType.Or);
            RunTest(" & ", true, 1, StepType.BitAnd);
            RunTest(" && ", true, 1, StepType.And);
            RunTest(" ! ", true, 1, StepType.Not);
            RunTest(" + ", true, 1, StepType.Plus);
            RunTest(" - ", true, 1, StepType.Minus);
            RunTest(" = ", true, 1, StepType.Equals);
            RunTest(" == ", true, 1, StepType.Equals);
            RunTest(" ~ ", true, 1, StepType.Negate);
            RunTest(" / ", true, 1, StepType.Divide);
            RunTest(" * ", true, 1, StepType.Multiply);
            RunTest(" < ", true, 1, StepType.LessThan);
            RunTest(" > ", true, 1, StepType.GreaterThan);
            RunTest(" >= ", true, 1, StepType.NotLessThan);
            RunTest(" <= ", true, 1, StepType.NotGreaterThan);
            RunTest(" Has ", true, 1, StepType.Has);
            RunTest(" Ends ", true, 1, StepType.Ends);
            RunTest(" Starts ", true, 1, StepType.Starts);

            void RunTest(string inText, bool isValid, int childCount, StepType childType)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                if (isValid)
                {
                    Assert.IsTrue(p.Children.Count == childCount);

                    var q = p.Children[0];
                    Assert.IsTrue(q.StepType == childType);
                }
            }
        }
    }
}
