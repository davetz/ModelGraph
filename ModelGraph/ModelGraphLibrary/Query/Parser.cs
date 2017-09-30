using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelGraphLibrary
{/* 
    Parse an algebraic expression string and recursivly build a parse
    tree of elemental steps. 

    If the input expression string is invalid the parser aborts and the
    incomplete parse tree is kept arround for error reporting. However,  
    if there were no errors the parser tree and original expression string
    are destroyed. Whenever needed a properly formatted expression string
    can be created from the expression tree. The benifit of this is it 
    produces a standard expressing string format and also it solves the 
    problem of what happens when someone renames a column not knowing that
    it was hardcoded in the expression string.
    (the expression tree references the property object and not it's name)
 */
    /// <summary>
    /// Parse a string and create an expession tree
    /// </summary>
    internal class Parser
    {
        internal Step Step;
        internal Parser Parent;
        internal List<Parser> Children = new List<Parser>(4);
        internal List<Parser> Arguments = new List<Parser>(4);

        internal string Text;
        internal int Index1;
        internal int Index2;

        private ParseFlag _flags;
        internal ParseError Error;
        internal StepType StepType;
 
        #region Constructor  ==================================================
        public static Parser Create(string text)
        {
            var p = new Parser(text).RemoveRedundantParens();
            p.HasParens = false;
            p.ResolveStepArguments();
            return p;
        }
        protected Parser(string text)
        {
            Text = text;
            if (string.IsNullOrWhiteSpace(text))
            {
                Text = string.Empty;
                Error = ParseError.InvalidText;
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
                case StepType.List:
                case StepType.Vector:
                    TryParse();
                    break;

                case StepType.Index:
                    break;

                case StepType.String:
                    Step = new STRING(this, Text);
                    break;

                case StepType.Double:
                    TryAddLiteralNumber(true);
                    break;

                case StepType.Integer:
                    TryAddLiteralNumber();
                    break;

                case StepType.BitField:
                    TryAddLiteralBitField();
                    break;

                case StepType.Property:
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Flags  ========================================================
        private bool GetFlag(ParseFlag flag) => (_flags & flag) != 0;
        private void SetFlag(bool val, ParseFlag flag) { if (val) _flags |= flag; else _flags &= ~flag; }

        public bool IsDone { get { return GetFlag(ParseFlag.IsDone); } set { SetFlag(value, ParseFlag.IsDone); } }
        public bool IsWierd { get { return GetFlag(ParseFlag.IsWierd); } set { SetFlag(value, ParseFlag.IsWierd); } }
        public bool IsBatched { get { return GetFlag(ParseFlag.IsBatched); } set { SetFlag(value, ParseFlag.IsBatched); } }
        public bool IsNegated { get { return GetFlag(ParseFlag.IsNegated); } set { SetFlag(value, ParseFlag.IsNegated); } }
        public bool HasParens { get { return GetFlag(ParseFlag.HasParens); } set { SetFlag(value, ParseFlag.HasParens); } }
        public bool HasNewLine { get { return GetFlag(ParseFlag.HasNewLine); } set { SetFlag(value, ParseFlag.HasNewLine); } }
        public bool IsUnresolved { get { return GetFlag(ParseFlag.IsUnresolved); } set { SetFlag(value, ParseFlag.IsUnresolved); } }
        public bool IsInvalidRef { get { return GetFlag(ParseFlag.IsInvalidRef); } set { SetFlag(value, ParseFlag.IsInvalidRef); } }
        public bool IsCircularRef { get { return GetFlag(ParseFlag.IsCircularRef); } set { SetFlag(value, ParseFlag.IsCircularRef); } }
        #endregion


        #region IsValid, NativeType, CompositeFlags  ==========================
        internal bool IsValid => GetIsValid();
        internal ValueType ValueType => GetValueType();
        internal ParseFlag CompositeFlags => GetCompositeFlags();
        private bool GetIsValid()
        {
            if (Error != ParseError.None) return false;
            foreach (var child in Children)
            {
                if (child.Error != ParseError.None) return false;
                foreach (var arg in child.Arguments)
                {
                    if (arg.Error != ParseError.None) return false;
                }
            }
            return true;
        }
        private ValueType GetValueType()
        {
            if (Step != null)
                return Step.ValueType;

            var flags = GetCompositeFlags();
            if ((flags & ParseFlag.IsCircularRef) != 0)
                return ValueType.Circular;

            if ((flags & ParseFlag.IsUnresolved) != 0)
                return ValueType.Unresolved;

            if ((flags & ParseFlag.IsInvalidRef) != 0)
                return ValueType.Invalid;

            return ValueType.None;
        }
        private ParseFlag GetCompositeFlags()
        {
            var flags = _flags;
            foreach (var child in Children)
            {
                flags |= GetCompositeFlags();
                foreach (var arg in child.Arguments)
                {
                    flags |= arg.GetCompositeFlags();
                }
            }
            return flags;
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
            Error = ParseError.InvalidString;
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
            Error = ParseError.InvalidParens;
            return true;
        }
        #endregion


        #region TryParse  =====================================================
        /* 
            Parse an expression string (or substring) and recursivly build a 
            tree of elemental parse steps.
        */
        bool TryParse()
        {
            while (Index1 < Text.Length)
            {
                var c = Text[Index1];
                if (char.IsWhiteSpace(c))
                {// - - - - - - - - - - - - - - - - - - - - ->  White Space
                    Index1++;
                }

                else if (c == ',')
                {// - - - - - - - - - - - - - - - - - - - - ->  Argument Separator
                    Index1++;
                }


                else if (char.IsDigit(c))
                {// - - - - - - - - - - - - - - - - - - - - ->  Numeric Literal
                    Index2 = (Index1 + 1);
                    bool isDouble = false;
                    bool isHex = false;
                    while (Index2 < Text.Length)
                    {//- - - - - - - - - - - - - - - - - - - find the end
                        var t = char.ToLower(Text[Index2]);
                        if (t == '.') isDouble = true;
                        else if (t == 'x') isHex = true;
                        else if (!char.IsDigit(t))
                        {
                            if (!isHex) break;
                            if (t != 'a' && t != 'b' && t != 'c' && t != 'd' && t != 'e' && t != 'f') break;
                        }
                        Index2++;
                    }
                    var stepType = isDouble ? StepType.Double : (isHex) ? StepType.BitField : StepType.Integer;
                    Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), stepType));
                    Index1 = Index2;
                }


                else if (c == '"')
                {// - - - - - - - - - - - - - - - - - - - - ->  String Literal
                    Index1 = Index2 = (Index1 + 1);
                    while (Text[Index2] != '"')
                    {//- - - - - - - - - - - - - - - - - - find the end
                        Index2++;
                    }

                    Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), StepType.String));
                    Index1 = (Index2 + 1);
                }


                else if (operatorString.Contains(c))
                {// - - - - - - - - - - - - - - - - - - - - ->  Operator
                    Index2 = (Index1 + 1);
                    while (Index2 < Text.Length)
                    {//- - - - - - - - - - - - - - - - - - find the end
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
                        Error = ParseError.InvalidText;
                        return false;
                    }
                }


                else if (char.IsLetter(c))
                {// - - - - - - - - - - - - - - - - - - - - ->  Function or Property
                    Index2 = (Index1 + 1);
                    while (Index2 < Text.Length)
                    {//- - - - - - - - - - - - - - - - - - find the end
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
                {// - - - - - - - - - - - - - - - - - - - - ->  Parethetical
                    Index1 = Index2 = (Index1 + 1);
                    var count = 1;
                    var onString = false;
                    var hasComma = false;
                    while (count > 0)
                    {//- - - - - - - - - - - - - - - - - find the end
                        var t = Text[Index2];
                        if (t == '"')
                            onString = !onString;
                        if (!onString)
                        {
                            if (t == '(')
                                count++;
                            else if (t == ')')
                                count--;
                            else if (t == ',')
                                hasComma = true;
                        }
                        Index2++;
                    }

                    Index2--;
                    if (hasComma)
                        Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), StepType.List));
                    else
                        Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), StepType.None, true));

                    Index1 = Index2 + 1;
                }


                else if (c == '{')
                {// - - - - - - - - - - - - - - - - - - - - ->  Vector 
                    Index1 = Index2 = (Index1 + 1);
                    var count = 1;
                    var onString = false;
                    while (count > 0)
                    {//- - - - - - - - - - - - - - - - - - find the end
                        var t = Text[Index2];
                        if (t == '"')
                            onString = !onString;
                        if (!onString)
                        {
                            if (t == '{')
                                count++;
                            else if (t == '}')
                                count--;
                        }
                        Index2++;
                    }

                    Index2--;
                    Children.Add(new Parser(this, Text.Substring(Index1, Index2 - Index1), StepType.Vector, true));

                    Index1 = Index2 + 1;
                }


                else if (newLineString.Contains(c))
                {// - - - - - - - - - - - - - - - - - - - - ->  New Line
                    Index2 = (Index1 + 1);
                    while (Index2 < Text.Length)
                    {//- - - - - - - - - - - - - - - - - - find the end
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
                        Error = ParseError.InvalidText;
                        return false;
                    }
                }


                else
                {// - - - - - - - - - - - - - - - - - - - - ->  Invalid Text
                    Error = ParseError.InvalidText;
                    return false;
                }
            }
            return IsValid;
        }

        #endregion

        #region RemoveRedundantParens  ========================================
        internal Parser RemoveRedundantParens()
        {/*
            recursively descend the parse tree and at each node conditionally
            replace it's child node with its grand child 
         */
            for (int i = 0; i < Children.Count; i++)
            {
                var c = Children[i];
                Children[i] = c.RemoveRedundantParens();
            }
            return (StepType != StepType.None || Children.Count != 1) ? this : Children[0];
        }
        #endregion

        #region ResolveStepArguments  =========================================
        bool ResolveStepArguments()
        {
            if (Children.Count == 0) return true;

            ResolveNagation();
            foreach (var child in Children)
            {
                if (!child.ResolveStepArguments()) return false;
            }
            while (TryFindNextOperaor(out int index))
            {
                var p = Children[index];
                p.IsDone = true;

                var parm = StepTypeParm[p.StepType];
                if (parm.HasLHS && index < 1)
                {
                    p.Error = ParseError.MissingArgLHS;
                    return false;
                }
                if (parm.HasRHS && index >= (Children.Count - 1))
                {
                    p.Error = ParseError.MissingArgRHS;
                    return false;
                }
                List<Parser> hitList = new List<Parser>(4);
                if (parm.HasLHS)
                {
                    p.Arguments.Add(Children[index - 1]);
                    hitList.Add(Children[index - 1]);
                }
                if (parm.HasRHS)
                {
                    p.Arguments.Add(Children[index + 1]);
                    hitList.Add(Children[index + 1]);
                    if (parm.CanBatch)
                    {
                        for (int i = index + 2; i < (Children.Count - 1); i += 2)
                        {
                            if (Children[i].StepType == p.StepType)
                            {
                                p.IsDone = true;
                                hitList.Add(Children[i]);
                                hitList.Add(Children[i + 1]);
                                p.Arguments.Add(Children[i + 1]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                foreach (var hit in hitList)
                {
                    Children.Remove(hit);
                }
            }
            return true;
        }
        #endregion

        #region ResolveNegation  ==============================================
        void ResolveNagation()
        {
            if (Children.Count > 1)
            {
                var i = 0;
                var N = Children.Count - 1;

                while (i < N)
                {
                    var st = Children[i].StepType;
                    if (i == 0)
                    {
                        switch (st)
                        {
                            case StepType.Not:
                            case StepType.Minus:
                            case StepType.Negate:
                                Children[i + 1].IsNegated = true;
                                Children.RemoveAt(i);
                                N--;
                                break;
                        }
                    }
                    else
                    {
                        switch (st)
                        {
                            case StepType.Plus:
                            case StepType.Minus:
                            case StepType.Divide:
                            case StepType.Multiply:
                                if (i + 2 > N) return;
                                if (Children[i + 1].StepType == StepType.Minus)
                                {
                                    Children[i + 2].IsNegated = true;
                                    Children.RemoveAt(i + 1);
                                    N--;
                                    i++;
                                }
                                else if (Children[i + 1].StepType == StepType.Plus)
                                {
                                    Children.RemoveAt(i + 1);
                                    N--;
                                    i++;
                                }
                                break;

                            case StepType.Not:
                            case StepType.Negate:
                                Children[i + 1].IsNegated = true;
                                Children.RemoveAt(i);
                                N--;
                                break;
                        }
                    }
                    i++;
                }
            }
        }
        #endregion

        #region TryFindNextOperaor  ===========================================
        bool TryFindNextOperaor(out int index)
        {
            index = -1;
            var priority = byte.MaxValue;

            for (int i = 0; i < Children.Count; i++)
            {
                var p = Children[i];
                var parm = StepTypeParm[p.StepType];
                if (!p.IsDone && (parm.HasLHS || parm.HasRHS))
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
                    if (val <= sbyte.MaxValue)
                        Step = new SBYTE(val);
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
            Error = ParseError.InvalidNumber;
            return false;
        }
        #endregion

        #region TryAddLiteralBitField  ========================================
        private bool TryAddLiteralBitField()
        {
            ulong val = 0;
            var chars = Text.ToLower().ToCharArray();
            var N = chars.Length;
            if (N < 3 || chars[0] != '0' || chars[1] != 'x')
            {
                Error = ParseError.InvalidNumber;
                return false;
            }
            for (int i = 2; i < N; i++)
            {
                var c = chars[i];

                val = val << 4;

                if (c == '1') val += 1;
                else if (c == '2') val += 2;
                else if (c == '3') val += 3;
                else if (c == '4') val += 4;
                else if (c == '5') val += 5;
                else if (c == '6') val += 6;
                else if (c == '7') val += 7;
                else if (c == '8') val += 8;
                else if (c == '9') val += 9;
                else if (c == 'a') val += 10;
                else if (c == 'b') val += 11;
                else if (c == 'c') val += 12;
                else if (c == 'd') val += 13;
                else if (c == 'e') val += 14;
                else if (c == 'f') val += 15;
                else if (c != '0')
                {
                    Error = ParseError.InvalidNumber;
                    return false;
                }
            }
            if (val <= byte.MaxValue)
                Step = new BYTE(val);
            else if (val <= ushort.MaxValue)
                Step = new UINT16(val);
            else if (val <= uint.MaxValue)
                Step = new UINT32(val);
            else
                Step = new UINT64(val);

            return true;
        }
        #endregion

        #region TryValidate  ==================================================
        internal bool TryValidate(Store sto, Func<Item> getItem)
        {
            if (Error != ParseError.None) return false;
            foreach (var arg in Arguments)
            {
                if (!arg.TryValidate(sto, getItem)) return false;
            }
            foreach (var child in Children)
            {
                if (!child.TryValidate(sto, getItem)) return false;
            }
            if (StepType == StepType.Property)
            {
                if (sto.TryLookUpProperty(Text, out Property property, out int index))
                {
                    Step = new PROPERTY(property, index, getItem);
                    Step.IsNegated = IsNegated;
                    if (property is ComputeX compute)
                    {
                        if (compute.ValueType == ValueType.Invalid)
                            IsInvalidRef = true;
                        else if (compute.ValueType == ValueType.Circular)
                            IsCircularRef = true;
                        else if (compute.ValueType == ValueType.Unresolved)
                            IsUnresolved = true;
                    }
                }
                else
                {
                    Error = ParseError.UnknownProperty;
                    return false;
                }
            }
            else
            {
                var parm = StepTypeParm[StepType];
                parm.Resolve(this);
            }
            return (!GetFlag(ParseFlag.IsUnresolved | ParseFlag.IsInvalidRef | ParseFlag.IsCircularRef));
        }
        #endregion

        #region TrySimplify  ==================================================
        internal bool TrySimplify()
        {
            return true;
        }
        #endregion


        #region ParseParam  ===================================================
        [Flags]
        private enum PFlag : ushort
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

            IsNegate = 0x100,  // this parse step is the negate operator
            CanNegate = 0x200, // this parse step will accept negation          
            PreNegate = 0x400, // this parse step may precede the negate operator e.g. (A - B) : (A * - B)
        }
        private struct PParm
        {
            readonly internal Action<Parser> Resolve;
            private PFlag _flags;

            internal byte Priority => (byte)(_flags & PFlag.Priority7); // parse steps with lower value are evaluated first
            internal bool CanBatch => (_flags & PFlag.CanBatch) != 0;

            internal bool HasLHS => (_flags & PFlag.HasLHS) != 0;
            internal bool HasRHS => (_flags & PFlag.HasRHS) != 0;
            internal bool OkListLHS => (_flags & PFlag.OkListLHS) != 0;
            internal bool OkListRHS => (_flags & PFlag.OkListRHS) != 0;

            internal bool IsNegate => (_flags & PFlag.IsNegate) != 0;
            internal bool CanNegate => (_flags & PFlag.CanNegate) != 0;
            internal bool IsPreNegate => (_flags & PFlag.PreNegate) != 0;

            internal PParm(Action<Parser> resolve, PFlag flags = PFlag.None)
            {
                _flags = flags;
                Resolve = resolve;
            }
        }
        #endregion

        #region StepTypeParm  =================================================
        static Dictionary<StepType, PParm> StepTypeParm = new Dictionary<StepType, PParm>
        {
            [StepType.None] = new PParm((p) => { }, PFlag.CanNegate),

            [StepType.List] = new PParm((p) => { }, PFlag.Priority7),
            [StepType.Index] = new PParm((p) => { }, PFlag.Priority7),
            [StepType.Vector] = new PParm((p) => { }, PFlag.Priority7),
            [StepType.String] = new PParm((p) => { }, PFlag.Priority7),
            [StepType.Double] = new PParm((p) => { Literal(p); }, PFlag.Priority7 | PFlag.CanNegate),
            [StepType.Integer] = new PParm((p) => { Literal(p); }, PFlag.Priority7 | PFlag.CanNegate),
            [StepType.Boolean] = new PParm((p) => { Literal(p); }, PFlag.Priority7 | PFlag.CanNegate),
            [StepType.Property] = new PParm((p) => { Literal(p); }, PFlag.Priority7 | PFlag.CanNegate),
            [StepType.BitField] = new PParm((p) => { Literal(p); }, PFlag.Priority7 | PFlag.CanNegate),
 
            [StepType.Or1] = new PParm((p) => { Or1(p); }, PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
            [StepType.Or2] = new PParm((p) => { new OR2(p); }, PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
            [StepType.And1] = new PParm((p) => { }, PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
            [StepType.And2] = new PParm((p) => { new AND2(p); }, PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
            [StepType.Not] = new PParm((p) => { }, PFlag.Priority1 | PFlag.HasRHS | PFlag.IsNegate | PFlag.CanNegate),
            [StepType.Plus] = new PParm((p) => { Plus(p); }, PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS | PFlag.CanBatch | PFlag.PreNegate | PFlag.CanNegate),
            [StepType.Minus] = new PParm((p) => { new MINUS(p); }, PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS | PFlag.CanBatch | PFlag.PreNegate | PFlag.IsNegate | PFlag.CanNegate),
            [StepType.Equals] = new PParm((p) => { }, PFlag.Priority6 | PFlag.HasLHS | PFlag.HasRHS | PFlag.PreNegate),
            [StepType.Negate] = new PParm((p) => { }, PFlag.Priority1 | PFlag.HasRHS | PFlag.IsNegate),
            [StepType.Divide] = new PParm((p) => { new DIVIDE(p); }, PFlag.Priority2 | PFlag.HasLHS | PFlag.HasRHS | PFlag.CanBatch | PFlag.PreNegate | PFlag.CanNegate),
            [StepType.Multiply] = new PParm((p) => { new MULTIPLY(p); }, PFlag.Priority2 | PFlag.HasLHS | PFlag.HasRHS | PFlag.CanBatch | PFlag.PreNegate | PFlag.CanNegate),
            [StepType.LessThan] = new PParm((p) => { }, PFlag.Priority6 | PFlag.HasLHS | PFlag.HasRHS | PFlag.PreNegate),
            [StepType.GreaterThan] = new PParm((p) => { }, PFlag.Priority6 | PFlag.HasLHS | PFlag.HasRHS | PFlag.PreNegate),
            [StepType.NotLessThan] = new PParm((p) => { }, PFlag.Priority6 | PFlag.HasLHS | PFlag.HasRHS | PFlag.PreNegate),
            [StepType.NotGreaterThan] = new PParm((p) => { }, PFlag.Priority6 | PFlag.HasLHS | PFlag.HasRHS | PFlag.PreNegate),

            [StepType.Has] = new PParm((p) => { }, PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
            [StepType.Ends] = new PParm((p) => { }, PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
            [StepType.Starts] = new PParm((p) => { }, PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
        };

        static void Or1(Parser p)
        {
            var type = p.Arguments[0].ValueType;
            if (type == ValueType.String)
                new CONCAT(p);
            else if (type == ValueType.Bool)
                new OR2(p);
        }
        static void Plus(Parser p)
        {
            var type = p.Arguments[0].ValueType;
            if (type == ValueType.String)
                new CONCAT(p);
            else if (type == ValueType.Bool)
                new OR2(p);
            else
                new PLUS(p);
        }
        static void Literal(Parser p)
        {
            if (p.Step != null)
            {
                p.Step.IsNegated = p.IsNegated;
            }
        }

        #endregion

        #region StaticParms  ==================================================
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
        #endregion
    }
}