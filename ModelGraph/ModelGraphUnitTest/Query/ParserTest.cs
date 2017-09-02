using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelGraphLibrary;

namespace ModelGraphUnitTest
{
    [TestClass]
    public class ParserTest
    {
        #region Parser  =======================================================
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
            RunTest("\"ok good\"", true, 1, StepType.String, "ok good");
            RunTest("   \"0.4-+=/*&|\" ", true, 1, StepType.String, "0.4-+=/*&|");

            void RunTest(string inText, bool isValid, int count0, StepType childType, string childText)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Children.Count == count0);
                var q = p.Children[0];
                Assert.IsTrue(q.StepType == childType);
                Assert.IsTrue(q.Text == childText);
            }
        }
        [TestMethod]
        public void ParserParens1()
        {
            RunTest("(\"ok (good)\")", true, 1, 1, StepType.String, "ok (good)");
            RunTest(" (  \"ok good\"  ) ", true, 1, 1, StepType.String, "ok good");
            RunTest(" (  \"ok good\") ", true, 1, 1, StepType.String, "ok good");

            void RunTest(string inText, bool isValid, int count0, int count1, StepType type1, string text1)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Children.Count == count0);

                var q = p.Children[0];
                Assert.IsTrue(q.Children.Count == count1);

                var r = q.Children[0];
                Assert.IsTrue(r.StepType == type1);
                Assert.IsTrue(r.Text == text1);
            }
        }
        [TestMethod]
        public void ParserParens2()
        {
            RunTest("((\"ok (good)\"))", true, 1, 1, 1, StepType.String, "ok (good)");
            RunTest(" (  ( \"ok good\" ) ) ", true, 1, 1, 1, StepType.String, "ok good");
            RunTest(" (  (\"ok good\")) ", true, 1, 1, 1, StepType.String, "ok good");
            RunTest("((\"ok good\"  )  ) ", true, 1, 1, 1, StepType.String, "ok good");

            void RunTest(string inText, bool isValid, int count0, int count1, int count2, StepType type2, string text2)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Children.Count == count0);

                var q = p.Children[0];
                Assert.IsTrue(q.Children.Count == count1);

                var r = q.Children[0];
                Assert.IsTrue(r.Children.Count == count2);

                var s = r.Children[0];
                Assert.IsTrue(s.StepType == type2);
                Assert.IsTrue(s.Text == text2);
            }
        }
        [TestMethod]
        public void ParserNumber()
        {
            RunTest("13", true, 1, StepType.Integer, "13");
            RunTest("0.45", true, 1, StepType.Double, "0.45");
            RunTest(" 16 ", true, 1, StepType.Integer, "16");
            RunTest(" 0.55 ", true, 1, StepType.Double, "0.55");

            void RunTest(string inText, bool isValid, int count0, StepType childType, string childText)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Children.Count == count0);

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
        public void ParserGoNoGo()
        {
            RunTest("(dog & cat == fight) || ((13.8 / 12.3) > 5.1)", true);
            RunTest("(dog & cat == fight) || (13.8 / 12.3) > 5.1)", false);

            void RunTest(string text, bool isValid)
            {
                var p = new Parser(text);
                Assert.IsTrue(p.IsValid == isValid);
            }
        }
        [TestMethod]
        public void ParserOperator()
        {
            RunTest("|", true, 1, StepType.Or1);
            RunTest("||", true, 1, StepType.Or2);
            RunTest("&", true, 1, StepType.And1);
            RunTest("&&", true, 1, StepType.And2);
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
            RunTest(" | ", true, 1, StepType.Or1);
            RunTest(" || ", true, 1, StepType.Or2);
            RunTest(" & ", true, 1, StepType.And1);
            RunTest(" && ", true, 1, StepType.And2);
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

            void RunTest(string inText, bool isValid, int count0, StepType childType)
            {
                var p = new Parser(inText);
                Assert.IsTrue(p.IsValid == isValid);
                if (isValid)
                {
                    Assert.IsTrue(p.Children.Count == count0);

                    var q = p.Children[0];
                    Assert.IsTrue(q.StepType == childType);
                }
            }
        }
        #endregion

        #region WhereSelect  ==================================================
        [TestMethod]
        public void WhereSelectTest1()
        {
            var td = TestData.Instance;         // get the TestData singleton
            var rc = td.RootChef;               // get the rootChef
            var dc = rc.GetItems()[0] as Chef;  // get dataChef[0]

            var T1RC = 10; // number of rows in table t1

            // create table t1

            var t1 = new TableX(dc.T_TableXStore);      
            t1.SetCapacity(T1RC);

            // create column c1

            var c1 = new ColumnX(dc.T_ColumnXStore);
            c1.Initialize(ModelGraphLibrary.ValueType.String, null, T1RC);
            c1.Name = "Id";
            dc.R_Store_Property.SetLink(t1, c1);

            // create column c2

            var c2 = new ColumnX(dc.T_ColumnXStore);    
            c2.Initialize(ModelGraphLibrary.ValueType.UInt16, null, T1RC);
            c1.Name = "I2Val";
            dc.R_Store_Property.SetLink(t1, c2);

            // add rows to table t1

            for (int i = 0; i < T1RC; i++)
            {
                var r = new RowX(t1);
                c1.TrySetValue(r, $"R{i}");             // {R0, R1, R2, R3, ...}
                c2.TrySetValue(r, $"{(i + 1) * 2}");    // { 2,  4,  8, 16, ...}
            }

            // begin test

            var w = new WhereSelect("I2Val / 2");
            Assert.IsTrue(w.IsValid);

            w.Validate(t1);
            Assert.IsTrue(w.IsValid);
        }
        #endregion

    }
}
