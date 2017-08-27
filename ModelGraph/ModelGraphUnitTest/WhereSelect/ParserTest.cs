
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
    }
}
