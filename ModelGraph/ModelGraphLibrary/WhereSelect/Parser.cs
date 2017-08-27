using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{
    public class Parser
    {
        public Step Step;
        public Parser Parent;
        public List<Parser> Children = new List<Parser>(4);

        public string Text;
        public int Index1;
        public int Index2;

        public ParseError ParseError;
        public ParseType ParseType;

        #region Constructor  ==================================================
        public Parser(string text)
        {
            Text = text;
            if (string.IsNullOrWhiteSpace(text))
            {
                Text = string.Empty;
                ParseError = ParseError.InvalidText;
                return;
            }
            if (HasInvalidString()) return;
            if (HasInvalidParens()) return;
            if (!TryParse()) return;
        }
        public Parser(Parser parent, string text, ParseType parseType)
        {
            Parent = parent;
            Text = text;
            ParseType = parseType;

            switch (parseType)
            {
                case ParseType.None:
                    if (!TryParse()) return;
                    break;
                case ParseType.Index:
                    break;
                case ParseType.String:
                    break;
                case ParseType.Double:
                    break;
                case ParseType.Integer:
                    break;
                case ParseType.Property:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region IsValid  ======================================================
        public bool IsValid => GetIsValid();
        private bool GetIsValid()
        {
            if (ParseError != ParseError.None) return false;
            foreach (var child in Children)
            {
                if (!child.IsValid) return false;
            }
            return true;
        }
        #endregion

        #region HasInvalidString  =============================================
        bool HasInvalidString()
        {
            var last = -1;
            var isOn = false;
            for (int i = 0; i < Text.Length; i++)
            {
                var c = Text[i];
                if (c == '"')
                {
                    last = i;
                    isOn = !isOn;
                }
            }
            if (!isOn) return false;

            Index1 = last;
            Index2 = Text.Length;
            ParseError = ParseError.InvalidString;
            return true;
        }
        #endregion

        #region HasInvalidParens  =============================================
        bool HasInvalidParens()
        {
            var count = 0;
            var first = -1;
            var isOn = false;
            for (int i = 0; i < Text.Length; i++)
            {
                var c = Text[i];
                if (c == '"') isOn = !isOn;
                if (isOn) continue;

                if (c == '(')
                {
                    if (count == 0) first = i;
                    count++;
                }
                if (c == ')')
                {
                    if (first < 0) first = i;
                    count--;
                }
            }
            if (count == 0) return false;

            if (first >= 0) Index1 = first;
            Index2 = Text.Length;
            ParseError = ParseError.InvalidParens;
            return true;
        }
        #endregion

        #region TryParse  =====================================================
        static Dictionary<string, ParseType> operatorParseType = new Dictionary<string, ParseType>
        {
            ["|"] = ParseType.OrOperator,
            ["||"] = ParseType.OrOperator,
            ["&"] = ParseType.AndOperator,
            ["&&"] = ParseType.AndOperator,
            ["!"] = ParseType.NotOperator,
            ["+"] = ParseType.PlusOperator,
            ["-"] = ParseType.MinusOperator,
            ["~"] = ParseType.NegateOperator,
            ["="] = ParseType.EqualsOperator,
            ["=="] = ParseType.EqualsOperator,
            ["/"] = ParseType.DivideOperator,
            ["*"] = ParseType.MultiplyOpartor,
            ["<"] = ParseType.LessThanOperator,
            [">"] = ParseType.GreaterThanOperator,
            [">="] = ParseType.NotLessThanOperator,
            ["<="] = ParseType.NotGreaterThanOperator,
        };
        bool TryParse()
        {
            while (Index1 < Text.Length)
            {
                var c = Text[Index1];
                if (c == '"')
                {
                    Index1 = Index2 = (Index1 + 1);
                    while (Text[Index2] != '"') { Index2++; }
                    Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), ParseType.String));
                    Index1 = (Index2 + 1);
                }
                else if (c == '(')
                {
                    Index1 = Index2 = (Index1 + 1);
                    var count = 1;
                    while (count > 0)
                    {
                        var t = Text[Index2];
                        if (t == '(')
                            count++;
                        else if (t == ')')
                            count--;
                        Index2++;
                    }
                    Index2--;
                    Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), ParseType.None));
                    Index1 = Index2 + 1;
                }
                else if ("0123456789".Contains(c))
                {
                    Index2 = (Index1 + 1);
                    bool isDouble = false;
                    while (Index2 < Text.Length)
                    {
                        var t = Text[Index2];
                        if (t == '.') isDouble = true;
                        else if (!"0123456789".Contains(t)) break;
                        Index2++;
                    }

                    var parseType = (isDouble) ? ParseType.Double : ParseType.Integer;
                    Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), parseType));
                    Index1 = Index2;
                }
                else if ("!~|&+-/*<>=".Contains(c))
                {
                    Index2 = (Index1 + 1);
                    while (Index2 < Text.Length)
                    {
                        var t = Text[Index2];
                        if (!"~|&+-/*<>=".Contains(t)) break;
                        Index2++;
                    }
                    var key = Text.Substring(Index1, Index2 - Index1);
                    if (operatorParseType.TryGetValue(key, out ParseType parseType))
                    {
                        Children.Add(new Parser(this, key, parseType));
                        Index1 = Index2;
                    }
                    else
                    {
                        ParseError = ParseError.InvalidText;
                        return false;
                    }
                }
                else
                {
                    Index1++;
                }
            }
            return IsValid;
        }
        #endregion
    }
}
