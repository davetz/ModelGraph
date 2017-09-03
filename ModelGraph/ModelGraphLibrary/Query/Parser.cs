using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelGraphLibrary
{/* 
    Parse an expression string (or substring) and recursivly build a 
    tree of elemental steps. From this and depending on the context of 
    neighboring parse steps, compose a computation expression tree.
    In the final stage the tree is simplified so that all constant
    elements are rolled-up into the simplest form.

    If the input expression string is invalid the parser aborts and the
    incomplete parse tree is kept arround for error reporting. However,  
    if there were no errors the parser tree and original expression string
    are destroyed. Whenever needed a properly formatted expression string
    can be created from the expression tree. The benifit of this is it 
    produces a standard expressing string format and also it solves the 
    problem of what happens when someone renames a column not knowing that
    it was hardcoded in the expression string.
    (the expression tree references the property object, not it's name)
 */
    /// <summary>
    /// Parse a string and create an expession tree
    /// </summary>
    internal class Parser
    {
        internal Step Step;
        internal Parser Parent;
        internal List<Parser> Children = new List<Parser>(4);

        internal string Text;
        internal int Index1;
        internal int Index2;

        internal ParseError ParseError;
        internal StepType StepType;
        private StepFlag _stepFlags;

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
        public Parser(Parser parent, string text, StepType parseType, bool hasParens = false)
        {
            Parent = parent;
            Text = text;
            StepType = parseType;
            HasParens = hasParens;

            switch (parseType)
            {
                case StepType.None:
                    TryParse();
                    break;

                case StepType.Index:
                    break;

                case StepType.String:
                    Step = new STRING(Text);
                    break;

                case StepType.Double:
                    TryAddLiteralNumber(true);
                    break;

                case StepType.Integer:
                    TryAddLiteralNumber();
                    break;

                case StepType.Property:
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Flags  ========================================================
        private bool GetFlag(StepFlag flag) => (_stepFlags & flag) != 0;
        private void SetFlag(bool val, StepFlag flag) { if (val) _stepFlags |= flag; else _stepFlags &= ~flag; }

        public bool IsWierd { get { return GetFlag(StepFlag.IsWierd); } set { SetFlag(value, StepFlag.IsWierd); } }
        public bool HasParens { get { return GetFlag(StepFlag.HasParens); } set { SetFlag(value, StepFlag.HasParens); } }
        public bool HasNewLine { get { return GetFlag(StepFlag.HasNewLine); } set { SetFlag(value, StepFlag.HasNewLine); } }
        public bool IsUnresolved { get { return GetFlag(StepFlag.IsUnresolved); } set { SetFlag(value, StepFlag.IsUnresolved); } }
        public bool IsInvalidReference { get { return GetFlag(StepFlag.InvalidReference); } set { SetFlag(value, StepFlag.InvalidReference); } }
        public bool IsCircularReference { get { return GetFlag(StepFlag.CircularReference); } set { SetFlag(value, StepFlag.CircularReference); } }
        #endregion

        #region Validate  =====================================================
        internal bool Validate(Store sto, Func<Item> getItem)
        {
            if (ParseError != ParseError.None) return false;
            if (StepType == StepType.Property)
            {
                if (sto.TryLookUpProperty(Text, out Property property, out NumericTerm term))
                {
                    Step = new PROPERTY(property, term, getItem);
                    if (property is ComputeX compute)
                    {
                        if (compute.NativeType == NativeType.Invalid)
                            IsInvalidReference = true;
                        else if (compute.NativeType == NativeType.Circular)
                            IsCircularReference = true;
                        else if (compute.NativeType == NativeType.Unresolved)
                            IsUnresolved = true;
                    }
                }
                else
                {
                    ParseError = ParseError.UnknownProperty;
                    return false;
                }
            }
            foreach (var child in Children)
            {
                if (!child.Validate(sto, getItem)) return false;
            }
            return (!GetFlag(StepFlag.IsUnresolved | StepFlag.InvalidReference | StepFlag.CircularReference));
        }
        #endregion

        #region IsValid  ======================================================
        private bool GetIsValid()
        {
            if (ParseError != ParseError.None) return false;
            foreach (var child in Children)
            {
                if (!child.GetIsValid()) return false;
            }
            return true;
        }
        public bool IsValid => GetIsValid();
        public ParseError CompositeError()
        {
            var error = ParseError;
            foreach (var child in Children)
            {
                error |= child.CompositeError();
            }
            return error;
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
            ["|"] = StepType.Or1,
            ["||"] = StepType.Or2,
            ["&"] = StepType.And1,
            ["&&"] = StepType.And2,
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
        static string operatorString = "!~|&+-/*<>=";
        static string newLineString = Environment.NewLine;

        bool TryParse()
        {
            while (Index1 < Text.Length)
            {
                var c = Text[Index1];
                if (char.IsWhiteSpace(c))
                {// - - - - - - - - - - - - - - - - - - - - - start of white space
                    Index1++; //- - - - - - - - - - - - - - - skip over it, and continue
                }
                else if (char.IsDigit(c))
                {// - - - - - - - - - - - - - - - - - - - - - start of a numeric literal value
                    Index2 = (Index1 + 1);
                    bool isDouble = false;
                    while (Index2 < Text.Length)
                    {//- - - - - - - - - - - - - - - - - - - find the ending index
                        var t = Text[Index2];
                        if (t == '.') isDouble = true;
                        else if (!char.IsDigit(t)) break;
                        Index2++;
                    }

                    var parseType = (isDouble) ? StepType.Double : StepType.Integer;
                    Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), parseType));
                    Index1 = Index2;
                }
                else if (c == '"')
                {// - - - - - - - - - - - - - - - - - - - - - start of a string literal value
                    Index1 = Index2 = (Index1 + 1);
                    while (Text[Index2] != '"')
                    {//- - - - - - - - - - - - - - - - - - - find the ending index
                        Index2++;
                    }
                    Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), StepType.String));
                    Index1 = (Index2 + 1);
                }
                else if (operatorString.Contains(c))
                {// - - - - - - - - - - - - - - - - - - - - - start of a operator expression
                    Index2 = (Index1 + 1);
                    while (Index2 < Text.Length)
                    {//- - - - - - - - - - - - - - - - - - - find the ending index
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
                else if (char.IsLetter(c))
                {// - - - - - - - - - - - - - - - - - - - - - start of either a function or parameter
                    Index2 = (Index1 + 1);
                    while (Index2 < Text.Length)
                    {//- - - - - - - - - - - - - - - - - - - find the ending index
                        var t = Text[Index2];                        
                        if (!(char.IsLetterOrDigit(t) || t == '.' || t == '_')) break;
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
                        Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), StepType.Property));
                        Index1 = Index2;
                    }
                }
                else if (c == '(')
                {// - - - - - - - - - - - - - - - - - - - - - start of a parethetical subexpression
                    Index1 = Index2 = (Index1 + 1);
                    var count = 1;
                    while (count > 0)
                    {//- - - - - - - - - - - - - - - - - - - find the ending index
                        var t = Text[Index2];
                        if (t == '(')
                            count++;
                        else if (t == ')')
                            count--;
                        Index2++;
                    }
                    Index2--;
                    Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), StepType.None, true));
                    Index1 = Index2 + 1;
                }
                else if (newLineString.Contains(c))
                {// - - - - - - - - - - - - - - - - - - - - - start of a new line declaration
                    Index2 = (Index1 + 1);
                    while (Index2 < Text.Length)
                    {//- - - - - - - - - - - - - - - - - - - find the ending index
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
                else
                {
                    ParseError = ParseError.InvalidText;
                    return false;
                }
            }
            return IsValid;
        }
        #endregion

        #region TryAddLiteralNumber  =========================================
        private bool TryAddLiteralNumber(bool hasDecimalPoint = false)
        {
            if (double.TryParse(Text, out double val))
            {
                if (hasDecimalPoint)
                {
                    Step = new DOUBLE(val);
                }
                else
                {
                    if (val <= byte.MaxValue)
                        Step = new BYTE(val);
                    else if (val <= short.MaxValue)
                        Step = new INT16(val);
                    else if (val <= int.MaxValue)
                        Step = new INT32(val);
                    else if (val <= long.MaxValue)
                        Step = new INT64(val);
                    else
                        Step = new DOUBLE(val);
                }
                return true;
            }
            ParseError = ParseError.InvalidNumber;
            return false;
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

            [StepType.Or1] = new StepParam(ValType.Flexible, 2, 255, ValType.Flexible),
            [StepType.And1] = new StepParam(ValType.Flexible, 2, 255, ValType.Flexible),

            [StepType.Or2] = new StepParam(ValType.Bool, 2, 255, ValType.Bool),
            [StepType.And2] = new StepParam(ValType.Bool, 2, 255, ValType.Bool),
        };
        bool TryCompose()
        {
            return true;
        }
        #endregion
    }
}
