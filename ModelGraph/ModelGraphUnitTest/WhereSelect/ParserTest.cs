
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelGraphLibrary;

namespace ModelGraphUnitTest
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void ParserNullText()
        {
            var p = new Parser(null);
            Assert.IsFalse(p.IsValid);
            Assert.IsTrue(p.Error == ParseError.InvalidText);
            Assert.IsTrue(p.Index1 == 0);
            Assert.IsTrue(p.Index1 == 0);
            Assert.IsTrue(p.Text == string.Empty);
        }
        [TestMethod]
        public void ParserWhiteSpaceText()
        {
            var p = new Parser("  ");
            Assert.IsFalse(p.IsValid);
            Assert.IsTrue(p.Error == ParseError.InvalidText);
            Assert.IsTrue(p.Index1 == 0);
            Assert.IsTrue(p.Index1 == 0);
            Assert.IsTrue(p.Text == string.Empty);
        }
        [TestMethod]
        public void ParserInvalidString1()
        {
            var text = "012\"";
            var p = new Parser(text);
            Assert.IsFalse(p.IsValid);
            Assert.IsTrue(p.Error == ParseError.InvalidString);
            Assert.IsTrue(p.Index1 == 3);
            Assert.IsTrue(p.Index2 == 4);
            Assert.IsTrue(p.Text == text);
        }
        [TestMethod]
        public void ParserInvalidParens1()
        {
            var text = "012(4(67)9";
            var p = new Parser(text);
            Assert.IsFalse(p.IsValid);
            Assert.IsTrue(p.Error == ParseError.InvalidParens);
            Assert.IsTrue(p.Index1 == 3);
            Assert.IsTrue(p.Index2 == 10);
            Assert.IsTrue(p.Text == text);
        }
        [TestMethod]
        public void ParserInvalidParens3()
        {
            var text = "0()(4\"))\"9";
            var p = new Parser(text);
            Assert.IsFalse(p.IsValid);
            Assert.IsTrue(p.Error == ParseError.InvalidParens);
            Assert.IsTrue(p.Index1 == 3);
            Assert.IsTrue(p.Index2 == 10);
            Assert.IsTrue(p.Text == text);
        }
        [TestMethod]
        public void ParserString1()
        {
            var text = "s\"s\"";
            var p = new Parser(text);
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 1);
            Assert.IsTrue(p.Children[0].Text == "s");
        }
        [TestMethod]
        public void ParserParams1()
        {
            var text = "a(\"s\")";
            var p = new Parser(text);
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 1);
            Assert.IsTrue(p.Children[0].Children.Count == 1);
            Assert.IsTrue(p.Children[0].Children[0].Text == "s");
        }
        [TestMethod]
        public void ParserNumber1()
        {
            var text = "13";
            var p = new Parser(text);
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 1);
            var child_0 = p.Children[0];
            Assert.IsTrue(child_0.Children.Count == 0);
            Assert.IsTrue(child_0.Text == "13");
        }
        [TestMethod]
        public void ParserParenStringNumber()
        {
            var text = " ( \" rip 0 \" 0.45 ) ";
            var p = new Parser(text);
            Assert.IsTrue(p.IsValid);
            Assert.IsTrue(p.Children.Count == 1);
            var child_0 = p.Children[0];
            Assert.IsTrue(child_0.Children.Count == 2);
            var child_0_0 = child_0.Children[0];
            Assert.IsTrue(child_0_0.Text == " rip 0 ");
            var child_0_1 = child_0.Children[1];
            Assert.IsTrue(child_0_1.Text == "0.45");
        }
    }
}
