﻿using System;
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
        public static Parser Create(string text)
        {
            var p = new Parser(text).RemoveRedundantParens();
            p.HasParens = false;
            return p;
        }
        protected Parser(string text)
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
        protected Parser(Parser parent, string text, StepType parseType, bool hasParens = false)
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
        public bool IsNegated { get { return GetFlag(StepFlag.IsNegated); } set { SetFlag(value, StepFlag.IsNegated); } }
        public bool HasParens { get { return GetFlag(StepFlag.HasParens); } set { SetFlag(value, StepFlag.HasParens); } }
        public bool HasNewLine { get { return GetFlag(StepFlag.HasNewLine); } set { SetFlag(value, StepFlag.HasNewLine); } }
        public bool IsUnresolved { get { return GetFlag(StepFlag.IsUnresolved); } set { SetFlag(value, StepFlag.IsUnresolved); } }
        public bool IsInvalidReference { get { return GetFlag(StepFlag.InvalidReference); } set { SetFlag(value, StepFlag.InvalidReference); } }
        public bool IsCircularReference { get { return GetFlag(StepFlag.CircularReference); } set { SetFlag(value, StepFlag.CircularReference); } }
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

        #region TryAddLiteralNumber  ==========================================
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

        #region TryValidate  ==================================================
        internal bool TryValidate(Store sto, Func<Item> getItem)
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
                if (!child.TryValidate(sto, getItem)) return false;
            }
            return (!GetFlag(StepFlag.IsUnresolved | StepFlag.InvalidReference | StepFlag.CircularReference));
        }
        #endregion

        #region TryCompose  ===================================================
        /* 
            At this phase, only some of the parse elements have an assigned Step
            the rest has be noodled-out based on context
        */
        internal bool TryCompose(out Step step)
        {
            step = null;
            return true;
        }

        #region ParseParam  ===================================================
        [Flags]
        enum PFlag : ushort
        {/*
            Facilitate the transformtion of a parse tree to an expression tree
         */
            None = 0,
            Priority1 = 1,  // this parce step is evaluted first
            Priority2 = 2,  // then this
            Priority3 = 3,  // and this
            Priority4 = 4,  // and this
            Priority5 = 5,  // and this
            Priority6 = 6,  // and this
            Priority7 = 7,  // and finally this
            CanBatch = 0x8, // can batch a succession of repeats, e.g. "A + B + C" becomes ADD(A, B, C)

            HasLHS = 0x10, // takes a left hand side argument
            HasRHS = 0x20, // takes a right hand side argument
            OkListLHS = 0X40, // takes a left hand side argument list
            OkListRHS = 0X80, // takes a right hand side argument list

            IsNegateKey1 = 0x100, // this parse step must precede the negate operator
            IsNegateKey2 = 0x200, // this parse step is the negate operator
            IsNegateKey3 = 0x400, // this parse step will accept negation          
        }
        struct PParm
        {
            readonly internal Func<Parser, Step> ResolveStep;
            private PFlag _flags;
            readonly internal byte MinArgs;
            readonly internal byte MaxArgs;

            internal byte Priority => (byte)(_flags & PFlag.Priority7); // parse steps with lower value are evaluated first
            internal bool CanBatch => (_flags & PFlag.CanBatch) != 0;

            internal bool HasLHS => (_flags & PFlag.HasLHS) != 0;
            internal bool HasRHS => (_flags & PFlag.HasRHS) != 0;
            internal bool OkListLHS => (_flags & PFlag.OkListLHS) != 0;
            internal bool OkListRHS => (_flags & PFlag.OkListRHS) != 0;

            internal bool IsNegateKey1 => (_flags & PFlag.IsNegateKey1) != 0;
            internal bool IsNegateKey2 => (_flags & PFlag.IsNegateKey2) != 0;
            internal bool IsNegateKey3 => (_flags & PFlag.IsNegateKey3) != 0;

            internal PParm(Func<Parser, Step> resolve, byte minArgs = 0, byte maxArgs = 0, PFlag flags = PFlag.None)
            {
                MinArgs = minArgs;
                MaxArgs = maxArgs;
                _flags = flags;
                ResolveStep = resolve;
            }
        }
        #endregion

        #region StepTypeParm  =================================================
        static Dictionary<StepType, PParm> StepTypeParm = new Dictionary<StepType, PParm>
        {
            [StepType.None] = new PParm(Parenthetical, 0, 0, PFlag.IsNegateKey3),

            [StepType.String] = new PParm(LiteralValue, 0, 0, PFlag.Priority7),
            [StepType.Double] = new PParm(LiteralValue, 0, 0, PFlag.Priority7 | PFlag.IsNegateKey3),
            [StepType.Boolean] = new PParm(LiteralValue, 0, 0, PFlag.Priority7 | PFlag.IsNegateKey3),
            [StepType.Integer] = new PParm(LiteralValue, 0, 0, PFlag.Priority7 | PFlag.IsNegateKey3),
            [StepType.Property] = new PParm(LiteralValue, 0, 0, PFlag.Priority7 | PFlag.IsNegateKey3),

            //[StepType.Negate] = new SParam(1, VType.Flexible, 2, 255, VType.Number, PFlag.IsNegater),
            //[StepType.Minus] = new SParam(2, VType.Flexible, 2, 255, VType.Number, PFlag.IsNegater),
            //[StepType.Plus] = new SParam(2, VType.Flexible, 2, 255, VType.Flexible),
            //[StepType.Divide] = new SParam(1, VType.Number, 2, 255, VType.Number),
            //[StepType.Multiply] = new SParam(1, VType.Number, 2, 255, VType.Number),
        };
        #endregion

        #region LiteralValue  =================================================
        static Step LiteralValue(Parser p)
        {
            return p.Step;
        }
        #endregion

        #region Parenthetical  ================================================
        static Step Parenthetical(Parser p)
        {
            return null;
        }
        #endregion

        #region NumericOperator  ==============================================
        static Step NumericOperator(Parser p)
        {
            var pp = p.Parent;
            return null;
        }
        #endregion

        #region ResolveNegation  ==============================================
        // Scan the list of children looking for the negate patern (key1, key2, key3)
        // when found, remove the negate operator and set the IsNegated flag on
        // the step that is being negated
        void ResolveNagation()
        {
            var i = 0;
            var N = Children.Count - 2;
            while(i < N)
            {
                if (StepTypeParm[Children[i].StepType].IsNegateKey1 &&
                    StepTypeParm[Children[i+1].StepType].IsNegateKey2 &&
                    StepTypeParm[Children[i+2].StepType].IsNegateKey3)
                {
                    Children[i + 2].IsNegated = true;
                    Children.RemoveAt(i + 1);

                    N--;
                }
                else
                {
                    i++;
                }
            }
        }
        #endregion

        #region ResolveParens  ================================================
        internal Parser RemoveRedundantParens()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var c = Children[i];
                Children[i] = c.RemoveRedundantParens();
            }
            return (StepType != StepType.None || Children.Count != 1) ? this : Children[0];
        }
        #endregion

        #region ResolveComplexity  ============================================
        bool TryResolveComplexity()
        {
            while (TryFindNextOperaor(out int index))
            {
                var p = Children[index];
                var parm = StepTypeParm[p.StepType];
                if (parm.HasLHS && index < 1)
                {
                    p.ParseError = ParseError.MissingArgLHS;
                    return false;
                }
                if (parm.HasRHS && index >= (Children.Count - 1))
                {
                    p.ParseError = ParseError.MissingArgRHS;
                    return false;
                }
            }
            return false;
        }
        #endregion

        #region TryFindNextOperaor  =========================================
        bool TryFindNextOperaor(out int index)
        {
            index = -1;
            var priority = byte.MaxValue;

            for (int i = 0; i < Children.Count; i++)
            {
                var p = Children[i];
                var parm = StepTypeParm[p.StepType];
                if (parm.HasLHS || parm.HasRHS)
                {
                    if (parm.Priority < priority)
                    {
                        priority = parm.Priority;
                        index = i;
                    }
                }
            }
            return (index >= 0);
        }
        #endregion
        #endregion


        #region TrySimplify  ==================================================
        internal bool TrySimplify()
        {
            return true;
        }
        #endregion
    }
}