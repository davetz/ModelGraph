using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelGraphLibrary
{
    /* 
        Parse an expression string (or substring) and recursivly build a 
        tree of elemental steps. From this and depending on the context of 
        neighboring parse steps, a computable expression tree is built.
        The final stage simplifies that tree so that all constant elements
        are rolled-up into the simplest form.

        If the input expression string is invalid the parser aborts and the
        incomplete parse tree is kept arround for error reporting. However,  
        if there were no errors the parser tree and original expression string
        is destroyed. Whenever needed a properly formatted expression string
        can be created from the simplified computable expression tree. The 
        benifit of this is standard expressing string format and also it
        solves the problem of someone renaming a property (the expression
        tree references the property object, not its name)
    */

    /// <summary>
    /// Parse a string and create an expession tree
    /// </summary>
    public class Parser
    {
        public Step Step;
        public Parser Parent;
        public List<Parser> Children = new List<Parser>(4);

        public string Text;
        public int Index1;
        public int Index2;

        public ParseError ParseError;
        public StepType StepType;

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
            if (!TryCompose()) return;
        }
        public Parser(Parser parent, string text, StepType parseType)
        {
            Parent = parent;
            Text = text;
            StepType = parseType;

            switch (parseType)
            {
                case StepType.None:
                    if (!TryParse()) return;
                    break;
                case StepType.Index:
                    break;
                case StepType.String:
                    break;
                case StepType.Double:
                    break;
                case StepType.Integer:
                    break;
                case StepType.Property:
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
        /* 
            Parse an expression string (or substring) and recursivly build a 
            tree of elemental parse steps.
        */
        static Dictionary<string, StepType> operatorParseType = new Dictionary<string, StepType>
        {
            ["||"] = StepType.Or,
            ["|"] = StepType.BitOr,
            ["&&"] = StepType.And,
            ["&"] = StepType.BitAnd,
            ["!"] = StepType.Not,
            ["+"] = StepType.Plus,
            ["-"] = StepType.Minus,
            ["~"] = StepType.Negate,
            ["="] = StepType.Equals,
            ["=="] = StepType.Equals,
            ["/"] = StepType.Divide,
            ["*"] = StepType.Multiply,
            ["<"] = StepType.LessThan,
            [">"] = StepType.GreaterThan,
            [">="] = StepType.NotLessThan,
            ["<="] = StepType.NotGreaterThan,
        };
        static Dictionary<string, StepType> functionParseType = new Dictionary<string, StepType>
        {
            ["has"] = StepType.Has,
            ["ends"] = StepType.Ends,
            ["starts"] = StepType.Starts,
        };
        static string numberString = "0123456789";
        static string alphaString = "abcdefghijklmnopqrstuvwxyz";
        static string operatorString = "!~|&+-/*<>=";
        static string newLineString = Environment.NewLine;

        bool TryParse()
        {
            while (Index1 < Text.Length)
            {
                var c = char.ToLower(Text[Index1]);
                if (c == '"')
                {
                    Index1 = Index2 = (Index1 + 1);
                    while (Text[Index2] != '"') { Index2++; }
                    Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), StepType.String));
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
                    Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), StepType.None));
                    Index1 = Index2 + 1;
                }
                else if (numberString.Contains(c))
                {
                    Index2 = (Index1 + 1);
                    bool isDouble = false;
                    while (Index2 < Text.Length)
                    {
                        var t = Text[Index2];
                        if (t == '.') isDouble = true;
                        else if (!numberString.Contains(t)) break;
                        Index2++;
                    }

                    var parseType = (isDouble) ? StepType.Double : StepType.Integer;
                    Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), parseType));
                    Index1 = Index2;
                }
                else if (operatorString.Contains(c))
                {
                    Index2 = (Index1 + 1);
                    while (Index2 < Text.Length)
                    {
                        var t = Text[Index2];
                        if (!operatorString.Contains(t)) break;
                        Index2++;
                    }
                    var key = Text.Substring(Index1, Index2 - Index1);
                    if (operatorParseType.TryGetValue(key, out StepType parseType))
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
                else if (newLineString.Contains(c))
                {
                    Index2 = (Index1 + 1);
                    while (Index2 < Text.Length)
                    {
                        var t = Text[Index2];
                        if (!newLineString.Contains(t)) break;
                        Index2++;
                    }
                    var key = Text.Substring(Index1, Index2 - Index1);
                    if (key == newLineString)
                    {
                        Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), StepType.NewLine));
                        Index1 = Index2;
                    }
                    else
                    {
                        ParseError = ParseError.InvalidText;
                        return false;
                    }
                }
                else if (alphaString.Contains(c))
                {
                    Index2 = (Index1 + 1);
                    while (Index2 < Text.Length)
                    {
                        var t = char.ToLower(Text[Index2]);
                        if (!alphaString.Contains(t)) break;
                        Index2++;
                    }
                    var key = Text.Substring(Index1, Index2 - Index1).ToLower();
                    if (functionParseType.TryGetValue(key, out StepType parseType))
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

        #region TryCompose  ===================================================
        /* 
           Use the parse tree and depending on the context of neighboring parse
           tree steps, compose a computable expression tree.
        */
        enum ValType : byte //the step's normal inut/output value type
        {
            None, // properties and literal constants have input ValType.None 
            Bool,
            Number, //could be (byte, short, int, long, or double)
            String,
            Flexible, //it depends on context involving nieboring steps
            Property, // determined by the property's NativeType 
        }
        struct StepParam
        {
            internal byte MinArgs;
            internal byte MaxArgs;
            internal ValType InType;
            internal ValType OutType;

            internal StepParam(ValType inType, byte minArgs, byte maxArgs, ValType outType)
            {
                InType = inType;
                MinArgs = minArgs;
                MaxArgs = maxArgs;
                OutType = outType;
            }
        }
        static Dictionary<StepType, StepParam> stepParams = new Dictionary<StepType, StepParam>
        {
            [StepType.Double] = new StepParam(ValType.None, 0, 0, ValType.Number),
            [StepType.Integer] = new StepParam(ValType.None, 0, 0, ValType.Number),
            [StepType.Property] = new StepParam(ValType.None, 0, 0, ValType.Property),

            [StepType.BitOr] = new StepParam(ValType.Flexible, 2, 255, ValType.Flexible),
            [StepType.BitAnd] = new StepParam(ValType.Flexible, 2, 255, ValType.Flexible),

            [StepType.Or] = new StepParam(ValType.Bool, 2, 255, ValType.Bool),
            [StepType.And] = new StepParam(ValType.Bool, 2, 255, ValType.Bool),
        };
        bool TryCompose()
        {
            return true;
        }
        #endregion
    }
}
