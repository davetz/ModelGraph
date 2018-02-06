using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelGraphSTD
{
    /// <summary>
    /// Parse a string and create an expession tree
    /// </summary>
    internal class Parser
    {
        internal ComputeStep Step;
        internal LiteralParse Evaluate;
        internal List<Parser> Children = new List<Parser>(4);
        internal StepType ParseType; // need to have the original stepType, because Step may be replaced 
        internal bool IsDone; // keep track of which parser nodes have already been processed

        public static ComputeStep CreateExpressionTree(string text)
        {
            var p = new Parser(text);   // create a parser tree

            p = p.RemoveRedundantParens(); // remove redundant parentheses

            p.Step.HasParens = false;   // not appropriate for the root
            p.Step.HasNewLine = false;  // not appropriate for the root

            p.TryAssignInputs();        // prune parse tree and assign step inputs

            return p.Step;              // return the root of the expression tree
        }

        #region Constructor  ==================================================
        protected Parser(string text)
        {
            Evaluate = new LiteralParse(text);
            Step = new ComputeStep(Evaluate);
            ParseType = StepType.Parse;

            if (Evaluate.IsEmpty)
            {
                Step.Error = StepError.EmptyString;
                Step.ParseAborted = true;
            }
            else if (Evaluate.HasUnbalancedQuotes())
            {
                Step.Error = StepError.UnbalancedQuotes;
                Step.ParseAborted = true;
            }
            else if (Evaluate.HasUnbancedParens())
            {
                Step.Error = StepError.UnbalancedParens;
                Step.ParseAborted = true;
            }
            else
            {
                TryParse();
            }
        }
        protected Parser(string text, StepType stepType, bool hasNewLine = false, bool hasParens = false)
        {
            Step = new ComputeStep(stepType);
            Evaluate = new LiteralParse(text);
            ParseType = stepType;

            Step.HasParens = hasParens;
            Step.HasNewLine = hasNewLine;

            switch (stepType)
            {
                case StepType.Parse:
                case StepType.List:
                case StepType.Vector:
                    Step.Evaluate = Evaluate;
                    TryParse();
                    break;

                case StepType.Index:
                    break;

                case StepType.String:
                    Step.Evaluate = new LiteralString(Step, text);
                    break;

                case StepType.Double:
                    TryAddLiteralNumber();
                    break;

                case StepType.Integer:
                    TryAddLiteralNumber();
                    break;

                case StepType.BitField:
                    TryAddLiteralNumber();
                    break;

                case StepType.Property:
                    Step.Evaluate = Evaluate; // the property name is needed by Step.TryValidate() 
                    break;

                default:
                    break;
            }

            #region TryAddLiteralNumber  ==========================================
            bool TryAddLiteralNumber()
            {
                (bool ok, bool isDouble, Int64 v, double d) = Value.ParseNumber(Evaluate.Text);

                if (ok)
                {
                    if (isDouble)
                        Step.Evaluate = new LiteralDouble(Step, d, Evaluate.Text);
                    else
                        Step.Evaluate = new LiteralInt64(Step, v, Evaluate.Text);
                    return true;
                }
                else
                {
                    Step.Error = StepError.InvalidNumber;
                    Step.Evaluate = Evaluate;
                }
                return false;
            }
            #endregion
        }
        #endregion


        #region RemoveRedundantParens  ========================================
        internal Parser RemoveRedundantParens()
        {/*
            recursively descend the parse tree and at each node conditionally
            replace it's child node with its grandchild. This is necessary
            when the expression string contains redundant parentheses. 
         */
            for (int i = 0; i < Children.Count; i++)
            {
                var c = Children[i];
                Children[i] = c.RemoveRedundantParens(); //<- - - recursive depth-first traversal
            }
            if (ParseType != StepType.Parse || Children.Count != 1 || Step.Error != StepError.None) return this;

            // propagate the flags to next generation

            Children[0].Step.HasParens |= Step.HasParens;
            Children[0].Step.HasNewLine |= Step.HasNewLine;

            // effectively remove this redunant parser from the tree

            return  Children[0];
        }
        #endregion

        #region TryParse  =====================================================
        /* 
            Parse an expression string (or substring) and recursivly build a 
            tree of elemental parse steps. return
        */
        void TryParse()
        {
            var e = Evaluate; // shorthand reference
            var hasNewLine = false;

            while (e.CanGetHead)
            {
                var c = e.HeadChar;
                if (char.IsWhiteSpace(c))
                {// - - - - - - - - - - - - - - - - - - - - ->  White Space
                    e.AdvanceHead(); // ignore white space
                }

                else if (c == ',')
                {// - - - - - - - - - - - - - - - - - - - - ->  Argument Separator
                    e.AdvanceHead();
                }


                else if (char.IsDigit(c))
                {// - - - - - - - - - - - - - - - - - - - - ->  Numeric Literal
                    e.SetTail();
                    bool isDouble = false;
                    bool isHex = false;
                    while (e.CanGetTail)
                    {//- - - - - - - - - - - - - - - - - - - find the end
                        var t = char.ToLower(e.TailChar);
                        if (t == '.') isDouble = true;
                        else if (t == 'x') isHex = true;
                        else if (!char.IsDigit(t))
                        {
                            if (!isHex) break;
                            if (t != 'a' && t != 'b' && t != 'c' && t != 'd' && t != 'e' && t != 'f') break;
                        }
                        e.AdvanceTail();
                    }
                    var stepType = isDouble ? StepType.Double : (isHex) ? StepType.BitField : StepType.Integer;

                    Children.Add(new Parser(e.HeadToTailString, stepType, hasNewLine));

                    hasNewLine = false;
                    e.SetNextHead();
                }


                else if (c == '"')
                {// - - - - - - - - - - - - - - - - - - - - ->  String Literal
                    e.SetNextHeadTail();
                    while (e.TailChar != '"')
                    {//- - - - - - - - - - - - - - - - - - find the end
                        e.AdvanceTail();
                    }

                    Children.Add(new Parser(e.HeadToTailString, StepType.String, hasNewLine));
                    hasNewLine = false;

                    e.AdvanceTail();
                    e.SetNextHead();
                }


                else if (operatorString.Contains(c))
                {// - - - - - - - - - - - - - - - - - - - - ->  Operator
                    e.SetTail();
                    while (e.CanGetTail)
                    {//- - - - - - - - - - - - - - - - - - find the end
                        var t = e.TailChar;
                        if (!operatorString.Contains(t)) break;
                        e.AdvanceTail();
                    }

                    var key = e.HeadToTailString;
                    if (operatorParseType.TryGetValue(key, out StepType parseType))
                    {
                        Children.Add(new Parser(key, parseType, hasNewLine));

                        hasNewLine = false;
                        e.SetNextHead();
                    }
                    else
                    {
                        Step.Error = StepError.InvalidOperator;
                        Step.ParseAborted = true;
                        return;
                    }
                }


                else if (char.IsLetter(c))
                {// - - - - - - - - - - - - - - - - - - - - ->  Function or Property
                    e.SetTail();
                    while (e.CanGetTail)
                    {//- - - - - - - - - - - - - - - - - - find the end
                        var t = e.TailChar;
                        if (!(char.IsLetterOrDigit(t) || t == '.' || t == '_')) break;
                        e.AdvanceTail();
                    }

                    var key = e.HeadToTailString.ToLower();

                    if (functionParseType.TryGetValue(key, out StepType parseType))
                        Children.Add(new Parser(key, parseType, hasNewLine));

                    else
                        Children.Add(new Parser(e.HeadToTailString, StepType.Property, hasNewLine));

                    hasNewLine = false;
                    e.SetNextHead();
                }


                else if (c == '(')
                {// - - - - - - - - - - - - - - - - - - - - ->  Parethetical
                    e.SetNextHeadTail();
                    var count = 1;
                    var onString = false;
                    var hasComma = false;
                    while (count > 0)
                    {//- - - - - - - - - - - - - - - - - find the end
                        var t = e.TailChar;
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
                        e.AdvanceTail();
                    }

                    e.DecrementTail(); // don't include the trailing closing paren

                    if (hasComma)
                        Children.Add(new Parser(e.HeadToTailString, StepType.List, hasNewLine));

                    else
                        Children.Add(new Parser(e.HeadToTailString, StepType.Parse, hasNewLine, true));

                    hasNewLine = false;
                    e.AdvanceTail();
                    e.SetNextHead();
                }


                else if (c == '{')
                {// - - - - - - - - - - - - - - - - - - - - ->  Vector 
                    e.SetNextHeadTail();
                    var count = 1;
                    var onString = false;
                    while (count > 0)
                    {//- - - - - - - - - - - - - - - - - - find the end
                        var t = e.TailChar;
                        if (t == '"')
                            onString = !onString;
                        if (!onString)
                        {
                            if (t == '{')
                                count++;
                            else if (t == '}')
                                count--;
                        }
                        e.AdvanceTail();
                    }

                    e.DecrementTail(); // don't include the trailing closing curly brace

                    Children.Add(new Parser(e.HeadToTailString, StepType.Vector, hasNewLine, true));

                    hasNewLine = false;
                    e.AdvanceTail();
                    e.SetNextHead();
                }


                else if (newLineString.Contains(c))
                {// - - - - - - - - - - - - - - - - - - - - ->  New Line
                    e.SetTail();
                    while (e.CanGetTail)
                    {//- - - - - - - - - - - - - - - - - - find the end
                        var t = e.TailChar;
                        if (!newLineString.Contains(t)) break;
                        e.AdvanceTail();
                    }

                    var key = e.HeadToTailString;
                    if (key == newLineString)
                    {
                        hasNewLine = true; //- - - - - will be used by the next child 
                        e.SetNextHead();
                    }
                    else
                    {
                        Step.Error = StepError.InvalidText;
                        Step.ParseAborted = true;
                        return;
                    }
                }


                else
                {// - - - - - - - - - - - - - - - - - - - - ->  Invalid Text
                    Step.Error = StepError.InvalidText;
                    Step.ParseAborted = true;
                    return;
                }
            }
        }

        #endregion

        #region TryAssignInputs  ==============================================
        void TryAssignInputs()
        {
            if (Children.Count == 0) return;

            ResolveNagation();

            foreach (var child in Children)
            {
                if (child.Children.Count > 0) child.TryAssignInputs();
            }

            var hitList = new List<Parser>(4);
            var inputList = new List<ComputeStep>(4);

            while (TryFindNextOperaor(out int index))
            {
                var p = Children[index];
                p.IsDone = true;

                var type = StepTypeParm[p.ParseType]; // need the original type because Step may have been replaced
                if (type.HasLHS && index < 1)
                {
                    p.Step.Error = StepError.MissingArgLHS;
                    Step.Error = StepError.InvalidSyntax;
                }
                if (type.HasRHS && index >= (Children.Count - 1))
                {
                    p.Step.Error = StepError.MissingArgRHS;
                    Step.Error = StepError.InvalidSyntax;
                }
                if (Step.Error != StepError.None) break;

                hitList.Clear();
                inputList.Clear();

                if (type.HasLHS)
                {
                    inputList.Add(Children[index - 1].Step);
                    hitList.Add(Children[index - 1]);
                }
                if (type.HasRHS)
                {
                    inputList.Add(Children[index + 1].Step);
                    hitList.Add(Children[index + 1]);
                    if (type.CanBatch)
                    {
                        for (int i = index + 2; i < (Children.Count - 1); i += 2)
                        {
                            if (Children[i].Step.StepType == p.Step.StepType)
                            {
                                p.IsDone = true;
                                hitList.Add(Children[i]);
                                hitList.Add(Children[i + 1]);
                                inputList.Add(Children[i + 1].Step);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                if (inputList.Count > 0) p.Step.Input = inputList.ToArray();
                foreach (var hit in hitList) { Children.Remove(hit); }
            }

            if (ParseType == StepType.Parse && Step.Error == StepError.None && Children.Count == 1)
            {
                var child = Children[0].Step;

                // propagate flags to child

                child.IsNegated |= Step.IsNegated;
                child.HasParens |= Step.HasParens;
                child.HasNewLine |= Step.HasNewLine;

                Step = child; // replace the parse step with the Children[0].Step
            }

            return;

            #region ResolveNegation  ==============================================
            void ResolveNagation()
            {
                if (Children.Count > 1)
                {
                    var i = 0;
                    var N = Children.Count - 1;

                    while (i < N)
                    {
                        var st = Children[i].Step.StepType;
                        if (i == 0)
                        {
                            switch (st)
                            {
                                case StepType.Not:
                                case StepType.Minus:
                                case StepType.Negate:
                                    Children[i + 1].Step.IsNegated = true;
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
                                    if (Children[i + 1].Step.StepType == StepType.Minus)
                                    {
                                        Children[i + 2].Step.IsNegated = true;
                                        Children.RemoveAt(i + 1);
                                        N--;
                                        i++;
                                    }
                                    else if (Children[i + 1].Step.StepType == StepType.Plus)
                                    {
                                        Children.RemoveAt(i + 1);
                                        N--;
                                        i++;
                                    }
                                    break;

                                case StepType.Not:
                                case StepType.Negate:
                                    Children[i + 1].Step.IsNegated = true;
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
                    var parm = StepTypeParm[p.Step.StepType];
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

            internal PParm(PFlag flags = PFlag.None)
            {
                _flags = flags;
            }
        }
        #endregion

        #region StepTypeParm  =================================================
        static Dictionary<StepType, PParm> StepTypeParm = new Dictionary<StepType, PParm>
        {
            [StepType.Parse] = new PParm(PFlag.CanNegate),

            [StepType.List] = new PParm(PFlag.Priority7),
            [StepType.Index] = new PParm(PFlag.Priority7),
            [StepType.Vector] = new PParm(PFlag.Priority7),
            [StepType.String] = new PParm(PFlag.Priority7),
            [StepType.Double] = new PParm(PFlag.Priority7 | PFlag.CanNegate),
            [StepType.Integer] = new PParm(PFlag.Priority7 | PFlag.CanNegate),
            [StepType.Boolean] = new PParm(PFlag.Priority7 | PFlag.CanNegate),
            [StepType.Property] = new PParm(PFlag.Priority7 | PFlag.CanNegate),
            [StepType.BitField] = new PParm(PFlag.Priority7 | PFlag.CanNegate),

            [StepType.Min] = new PParm(PFlag.Priority4 | PFlag.HasRHS),
            [StepType.Max] = new PParm(PFlag.Priority4 | PFlag.HasRHS),
            [StepType.Sum] = new PParm(PFlag.Priority4 | PFlag.HasRHS),
            [StepType.Ave] = new PParm(PFlag.Priority4 | PFlag.HasRHS),
            [StepType.Count] = new PParm(PFlag.Priority4 | PFlag.HasRHS),
            [StepType.Length] = new PParm(PFlag.Priority4 | PFlag.HasRHS),

            [StepType.Or1] = new PParm(PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
            [StepType.Or2] = new PParm(PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
            [StepType.And1] = new PParm(PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
            [StepType.And2] = new PParm(PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
            [StepType.Not] = new PParm(PFlag.Priority1 | PFlag.HasRHS | PFlag.IsNegate | PFlag.CanNegate),
            [StepType.Plus] = new PParm(PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS | PFlag.CanBatch | PFlag.PreNegate | PFlag.CanNegate),
            [StepType.Minus] = new PParm(PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS | PFlag.CanBatch | PFlag.PreNegate | PFlag.IsNegate | PFlag.CanNegate),
            [StepType.Equal] = new PParm(PFlag.Priority6 | PFlag.HasLHS | PFlag.HasRHS | PFlag.PreNegate),
            [StepType.Negate] = new PParm(PFlag.Priority1 | PFlag.HasRHS | PFlag.IsNegate),
            [StepType.Divide] = new PParm(PFlag.Priority2 | PFlag.HasLHS | PFlag.HasRHS | PFlag.CanBatch | PFlag.PreNegate | PFlag.CanNegate),
            [StepType.Multiply] = new PParm(PFlag.Priority2 | PFlag.HasLHS | PFlag.HasRHS | PFlag.CanBatch | PFlag.PreNegate | PFlag.CanNegate),
            [StepType.LessThan] = new PParm(PFlag.Priority6 | PFlag.HasLHS | PFlag.HasRHS | PFlag.PreNegate),
            [StepType.GreaterThan] = new PParm(PFlag.Priority6 | PFlag.HasLHS | PFlag.HasRHS | PFlag.PreNegate),
            [StepType.GreaterEqual] = new PParm(PFlag.Priority6 | PFlag.HasLHS | PFlag.HasRHS | PFlag.PreNegate),
            [StepType.LessEqual] = new PParm(PFlag.Priority6 | PFlag.HasLHS | PFlag.HasRHS | PFlag.PreNegate),

            [StepType.Contains] = new PParm(PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
            [StepType.EndsWith] = new PParm(PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
            [StepType.StartsWith] = new PParm(PFlag.Priority4 | PFlag.HasLHS | PFlag.HasRHS),
        };
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
            ["="] = StepType.Equal,
            ["=="] = StepType.Equal,
            ["/"] = StepType.Divide,
            ["*"] = StepType.Multiply,
            ["<"] = StepType.LessThan,
            [">"] = StepType.GreaterThan,
            [">="] = StepType.GreaterEqual,
            ["<="] = StepType.LessEqual,
        };
        static Dictionary<string, StepType> functionParseType = new Dictionary<string, StepType>
        {
            ["min"] = StepType.Min,
            ["max"] = StepType.Max,
            ["sum"] = StepType.Sum,
            ["ave"] = StepType.Ave,
            ["count"] = StepType.Count,
            ["length"] = StepType.Length,
            ["contains"] = StepType.Contains,
            ["endswith"] = StepType.EndsWith,
            ["startswith"] = StepType.StartsWith,
        };
        static string operatorString = "!~|&+-/*<>=";
        static string newLineString = Environment.NewLine;
        #endregion
    }
}