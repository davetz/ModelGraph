using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelGraphLibrary;
using System;

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
                var p = Parser.Create(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Error == error);
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
                var p = Parser.Create(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Error == error);
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
                var p = Parser.Create(inText);
                Assert.IsTrue(p.IsValid == isValid);
                Assert.IsTrue(p.Error == error);
                Assert.IsTrue(p.Index1 == index1);
                Assert.IsTrue(p.Index2 == index2);
                Assert.IsTrue(p.Text == outText);
            }
        }
        [TestMethod]
        public void ParserString()
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
        [TestMethod]
        public void ParserCommas()
        {
            var p = Parser.Create("5.4,3");
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 2);

            var q = p.Children[0];
            Assert.IsTrue(q.StepType == StepType.Double);

            var r = p.Children[1];
            Assert.IsTrue(r.StepType == StepType.Integer);
        }
        [TestMethod]
        public void ParserList()
        {
            var p = Parser.Create("((5 + 2),3)");
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 2);

            var q = p.Children[0];
            Assert.IsTrue(q.StepType == StepType.None);

            var r = p.Children[1];
            Assert.IsTrue(r.StepType == StepType.Integer);
        }
        [TestMethod]
        public void ParserVector()
        {
            var p = Parser.Create("{(5 + 2),3}");
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 2);

            var q = p.Children[0];
            Assert.IsTrue(q.StepType == StepType.None);

            var r = p.Children[1];
            Assert.IsTrue(r.StepType == StepType.Integer);
        }
        [TestMethod]
        public void ParserParens()
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
        [TestMethod]
        public void ParserNumber()
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
        [TestMethod]
        public void ParserParenStringNumber()
        {
            var text = " ( \" rip 0 \" 0.45 ) ";
            var p = Parser.Create(text);
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 2);

            var r = p.Children[0];
            Assert.IsTrue(r.Text == " rip 0 ");

            var s = p.Children[1];
            Assert.IsTrue(s.Text == "0.45");
        }
        [TestMethod]
        public void ParserGoNoGo()
        {
            RunTest("5 * 2 / 3 < 8", true);
            RunTest("5 + 3 + 6 + 7", true);
            RunTest("5 / 3", true);
            RunTest("(dog & cat == fight) || ((13.8 / 12.3) > 5.1)", true);
            RunTest("(dog & cat == fight) || (13.8 / 12.3) > 5.1)", false);

            void RunTest(string text, bool isValid)
            {
                var p = Parser.Create(text);
                Assert.IsTrue(p.IsValid == isValid);
            }
        }
        [TestMethod]
        public void ParserOperator()
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
            dc.R_TableX_ColumnX.SetLink(t1, c1);

            // create column c2

            var c2 = new ColumnX(dc.T_ColumnXStore);    
            c2.Initialize(ModelGraphLibrary.ValueType.UInt16, null, T1RC);
            c2.Name = "I2Val";
            dc.R_TableX_ColumnX.SetLink(t1, c2);

            // add rows to table t1

            for (int i = 0; i < T1RC; i++)
            {
                var r = new RowX(t1);
                c1.TrySetValue(r, $"R{i}");             // {R0, R1, R2, R3, ...}
                c2.TrySetValue(r, $"{(i + 1) * 2}");    // { 2,  4,  8, 16, ...}
            }
            var rows = t1.Items;

            // begin test

            var w = new WhereSelect("I2Val + 5");
            Assert.IsTrue(w.IsValid);

            w.Validate(t1);
            var str = w.InputString;
            int value;
            for (int i = 0; i < T1RC; i++)
            {
                w.GetValue(rows[i], out value);
                Assert.IsTrue(value == ((i + 1) * 2) + 5);
            }
        }
        #endregion

    }
}
