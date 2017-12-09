using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{/*
    ComputeStep is a node in an expression tree. The expression tree is used in
    a query's where/select clause. When employed in a select clause, it computes
    a value based on an item's properties. In the context of a where clause it can
    qualify specific relational paths based on the properties of the encountered rows
    while traversing the relational path.

    The evaluation function is set durring the where/select clause
    validation. The actual value-types and choice of evaluation function
    ripple up from a depth-first traversal of the expression tree.
 */
    internal class ComputeStep
    {
        internal ComputeStep[] Input;  // a step may have zero or more inputs
        internal EvaluateStep Evaluate; // evaluate this step and produce a value
        internal StepType StepType;     // parser's initial clasification of this step
        internal StepError Error;       // keeps track of TryParse and TryResove errors
        private StepFlag1 _flags1;      // used when creating the expression string
        private StepFlag2 _flags2;      // used by TryResolve()

        #region Constructor  ==================================================
        internal ComputeStep(LiteralParse evaluate)
        {
            StepType = StepType.Parse;
            Evaluate = evaluate;
        }
        internal ComputeStep(StepType stepType)
        {
            StepType = stepType;
            Evaluate = Chef.EvaluateUnresolved;
        }
        #endregion

        #region Flags1  =======================================================
        bool GetF1(StepFlag1 flag) => (_flags1 & flag) != 0;
        void SetF1(bool val, StepFlag1 flag) { if (val) _flags1 |= flag; else _flags1 &= ~flag; }

        internal bool IsInverse { get { return GetF1(StepFlag1.IsInverse); } set { SetF1(value, StepFlag1.IsInverse); } }
        internal bool IsNegated { get { return GetF1(StepFlag1.IsNegated); } set { SetF1(value, StepFlag1.IsNegated); } }
        internal bool IsBatched { get { return GetF1(StepFlag1.IsBatched); } set { SetF1(value, StepFlag1.IsBatched); } }
        internal bool HasParens { get { return GetF1(StepFlag1.HasParens); } set { SetF1(value, StepFlag1.HasParens); } }
        internal bool HasNewLine { get { return GetF1(StepFlag1.HasNewLine); } set { SetF1(value, StepFlag1.HasNewLine); } }
        internal bool ParseAborted { get { return GetF1(StepFlag1.ParseAborted); } set { SetF1(value, StepFlag1.ParseAborted); } }
        #endregion

        #region Flags2  =======================================================
        bool GetF2(StepFlag2 flag) => (_flags2 & flag) != 0;
        void SetF2(bool val, StepFlag2 flag) { if (val) _flags2 |= flag; else _flags2 &= ~flag; }

        internal bool IsError { get { return GetF2(StepFlag2.IsError); } set { SetF2(value, StepFlag2.IsError); if (value) _flags2 |= StepFlag2.AnyError; } }
        internal bool IsChanged { get { return GetF2(StepFlag2.IsChanged); } set { SetF2(value, StepFlag2.IsChanged); if (value) _flags2 |= StepFlag2.AnyChange; } }
        internal bool IsOverflow { get { return GetF2(StepFlag2.IsOverflow); } set { SetF2(value, StepFlag2.IsOverflow); if (value) _flags2 |= StepFlag2.AnyOverflow; } }
        internal bool IsUnresolved { get { return GetF2(StepFlag2.IsUnresolved); } set { SetF2(value, StepFlag2.IsUnresolved); if (value) _flags2 |= StepFlag2.AnyUnresolved; } }

        internal bool AnyError => (_flags2 & StepFlag2.AnyError) != 0;
        internal bool AnyChange => (_flags2 & StepFlag2.AnyChange) != 0;
        internal bool AnyOverflow => (_flags2 & StepFlag2.AnyOverflow) != 0;
        internal bool AnyUnresolved => (_flags2 & StepFlag2.AnyUnresolved) != 0;

        void InitResolveFlags() => _flags2 = StepFlag2.None;


        protected ValGroup ScanInputsAndReturnCompositeValueGroup()
        {
            var result = ValGroup.None;
            var N = Count;
            for (int i = 0; i < N; i++)
            {
                var input = Input[i];
                var group = input.Evaluate.ValueGroup;

                if (group == ValGroup.None)
                    IsUnresolved = true;
                else
                    result |= group;

                // bubble-up (IsError, IsChanged, IsOverflow, IsUnresolved)

                var flags = input._flags2;
                _flags2 |= (StepFlag2)((int)(flags & StepFlag2.LowerMask) << 4);
            }
            return result;
        }

        #endregion

        #region IsValid  ======================================================
        internal bool IsValid => GetIsValid();
        private bool GetIsValid()
        {
            if (ParseAborted) return false;
            if (Error != StepError.None) return false;

            if (Input != null)
            {
                foreach (var step in Input)
                {
                    if (step.IsValid) continue; // recursive depth-first traversal
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region TryValidate  ==================================================
        internal bool TryValidate(Store sto, Func<Item> getItem)
        {
            bool result = true;
            if (StepType == StepType.Property && Evaluate is LiteralParse e)
            {
                if (sto.TryLookUpProperty(e.Text, out Property property, out int index))
                {
                    StepType = StepType.Property;
                    Evaluate = new LiteralProperty(this, property, index, getItem);
                }
                else
                {
                    Error = StepError.UnknownProperty;
                    return false;
                }
            }
            else if (Input != null)
            {
                foreach (var step in Input)
                {
                    result &= step.TryValidate(sto, getItem); // recursive depth-first traversal
                }
            }
            return result;
        }
        #endregion

        #region TryResolve  ===================================================
        /// <summary>
        /// Depth-First traversal of the expression tree
        /// </summary>
        internal bool TryResolve()
        {
            InitResolveFlags();

            if (Input != null) foreach (var step in Input) { step.TryResolve(); } // recursive depth-first traversal

            if (StepTypeResolve.TryGetValue(StepType, out Action<ComputeStep> resolve)) resolve(this);

            return AnyChange;
        }
        #endregion


        #region Property  =====================================================
        internal int Count => (Input == null) ? 0 : Input.Length;
        internal ValType ValueType => Evaluate.ValueType;
        #endregion

        #region GetText  ======================================================
        internal void GetText(StringBuilder sb)
        {
            // Prefix  ========================================================

            if (HasNewLine) sb.Append(Environment.NewLine);
            if (IsNegated)
            {
                switch (ValueType)
                {
                    case ValType.Bool:
                        sb.Append('!');
                        break;
                    case ValType.SByte:
                    case ValType.Int16:
                    case ValType.Int32:
                    case ValType.Int64:
                    case ValType.Double:
                        sb.Append('-');
                        break;
                    case ValType.Byte:
                    case ValType.UInt16:
                    case ValType.UInt32:
                    case ValType.UInt64:
                        sb.Append('~');
                        break;
                }
            }
            if (HasParens) sb.Append('(');

            // Text  ==========================================================

            var N = Count;
            if (N == 0)
            {
                sb.Append(Evaluate.Text);
            }
            else if (N == 1)
            {
                sb.Append(Evaluate.Text);
                Input[0].GetText(sb); //<- - - - - recursive call
            }
            else
            {
                Input[0].GetText(sb); //<- - - - - recursive call
                for (int i = 1; i < N; i++)
                {
                    sb.Append(Evaluate.Text);
                    Input[i].GetText(sb); //<- - - recursive call
                }
            }

            // Suffix  ========================================================

            if (HasParens) sb.Append(')');
        }

        #endregion

        #region StepTypeResolve  ==============================================
        static Dictionary<StepType, Action<ComputeStep>> StepTypeResolve =
            new Dictionary<StepType, Action<ComputeStep>>()
            {
                [StepType.Or1] = ResolveOr1,
                [StepType.Or2] = ResolveFails,

                [StepType.And1] = ResolveFails,
                [StepType.And2] = ResolveFails,

                [StepType.Plus] = ResolvePlus,
                [StepType.Minus] = ResolveMinus,
                [StepType.Divide] = ResolveDivide,
                [StepType.Multiply] = ResolveMultiply,

                [StepType.Equal] = ResolveEqual,
                [StepType.NotEqual] = ResolveNotEqual,

                [StepType.LessThan] = ResolveLessThan,
                [StepType.LessEqual] = ResolveLessEqual,

                [StepType.GreaterThan] = ResolveGreaterThan,
                [StepType.GreaterEqual] = ResolveGreaterEqual,
            };
        #endregion

        #region Resolve  ======================================================

        #region ResolveFails  =================================================
        static void ResolveFails(ComputeStep step)
        {
            step.Evaluate = Chef.EvaluateUnresolved;
        }
        #endregion

        #region ResolveOr1  ===================================================
        static void ResolveOr1(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValueGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.Bool:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(Or2Bool))
                        step.Evaluate = new Or2Bool(step);
                    break;

                case ValGroup.String:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(ConcatString))
                        step.Evaluate = new ConcatString(step);
                    break;

                case ValGroup.Long:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if ((composite & ValGroup.Double) != 0)
                    {
                        step.Error = StepError.InvalidArgsRHS;
                    }
                    else
                    {
                        if (type != typeof(Or1Long))
                            step.Evaluate = new Or1Long(step);
                    }
                    break;

                default:
                    step.Error = StepError.InvalidArgsLHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValueType == ValType.IsUnresolved);
        }
        #endregion


        #region ResolvePlus  ==================================================
        static void ResolvePlus(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValueGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.Bool:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(Or2Bool))
                        step.Evaluate = new Or2Bool(step);
                    break;

                case ValGroup.String:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(ConcatString))
                        step.Evaluate = new ConcatString(step);
                    break;

                case ValGroup.Double:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(PlusDouble))
                        step.Evaluate = new PlusDouble(step);
                    break;

                case ValGroup.Long:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if ((composite & ValGroup.Double) != 0)
                    {
                        if (type != typeof(PlusDouble))
                            step.Evaluate = new PlusDouble(step);
                    }
                    else
                    {
                        if (type != typeof(PlusLong))
                            step.Evaluate = new PlusLong(step);
                    }
                    break;

                default:
                    step.Error = StepError.InvalidArgsLHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValueType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveMinus  =================================================
        static void ResolveMinus(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValueGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.String:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(RemoveString))
                        step.Evaluate = new RemoveString(step);
                    break;

                case ValGroup.Double:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(MinusDouble))
                        step.Evaluate = new MinusDouble(step);
                    break;

                case ValGroup.Long:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if ((composite & ValGroup.Double) != 0)
                    {
                        if (type != typeof(MinusDouble))
                            step.Evaluate = new MinusDouble(step);
                    }
                    else
                    {
                        if (type != typeof(MinusLong))
                            step.Evaluate = new MinusLong(step);
                    }
                    break;

                default:
                    step.Error = StepError.InvalidArgsLHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValueType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveDivide  ================================================
        static void ResolveDivide(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValueGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.Long:
                case ValGroup.Double:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(DivideDouble))
                        step.Evaluate = new DivideDouble(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsLHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValueType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveMultiply  ==============================================
        static void ResolveMultiply(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValueGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.Long:
                case ValGroup.Double:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(MultiplyDouble))
                        step.Evaluate = new MultiplyDouble(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsLHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValueType == ValType.IsUnresolved);
        }
        #endregion


        #region ResolveEqual  =================================================
        static void ResolveEqual(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValueGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.Bool:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(EqualBool))
                        step.Evaluate = new EqualBool(step);
                    break;

                case ValGroup.String:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(EqualString))
                        step.Evaluate = new EqualString(step);
                    break;

                case ValGroup.Double:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(EqualDouble))
                        step.Evaluate = new EqualDouble(step);
                    break;

                case ValGroup.Long:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(EqualLong))
                            step.Evaluate = new EqualLong(step);
                    break;

                case ValGroup.DateTime:
                    if (composite != ValGroup.DateTime)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(EqualDateTime))
                        step.Evaluate = new EqualDateTime(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsLHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValueType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveNotEqual  ==============================================
        static void ResolveNotEqual(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValueGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.Bool:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(NotEqualBool))
                        step.Evaluate = new NotEqualBool(step);
                    break;

                case ValGroup.String:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(NotEqualString))
                        step.Evaluate = new NotEqualString(step);
                    break;

                case ValGroup.Double:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(NotEqualDouble))
                        step.Evaluate = new NotEqualDouble(step);
                    break;

                case ValGroup.Long:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(NotEqualLong))
                        step.Evaluate = new NotEqualLong(step);
                    break;

                case ValGroup.DateTime:
                    if (composite != ValGroup.DateTime)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(NotEqualDateTime))
                        step.Evaluate = new NotEqualDateTime(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsLHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValueType == ValType.IsUnresolved);
        }
        #endregion


        #region ResolveLessThan  ==============================================
        static void ResolveLessThan(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValueGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.String:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(LessThanString))
                        step.Evaluate = new LessThanString(step);
                    break;

                case ValGroup.Double:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(LessThanDouble))
                        step.Evaluate = new LessThanDouble(step);
                    break;

                case ValGroup.Long:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(LessThanLong))
                        step.Evaluate = new LessThanLong(step);
                    break;

                case ValGroup.DateTime:
                    if (composite != ValGroup.DateTime)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(LessThanDateTime))
                        step.Evaluate = new LessThanDateTime(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsLHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValueType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveLessEqual  =============================================
        static void ResolveLessEqual(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValueGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.String:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(LessEqualString))
                        step.Evaluate = new LessEqualString(step);
                    break;

                case ValGroup.Double:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(LessEqualDouble))
                        step.Evaluate = new LessEqualDouble(step);
                    break;

                case ValGroup.Long:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(LessEqualLong))
                        step.Evaluate = new LessEqualLong(step);
                    break;

                case ValGroup.DateTime:
                    if (composite != ValGroup.DateTime)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(LessEqualDateTime))
                        step.Evaluate = new LessEqualDateTime(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsLHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValueType == ValType.IsUnresolved);
        }
        #endregion


        #region ResolveGreaterThan  ===========================================
        static void ResolveGreaterThan(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValueGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.String:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(GreaterThanString))
                        step.Evaluate = new GreaterThanString(step);
                    break;

                case ValGroup.Double:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(GreaterThanDouble))
                        step.Evaluate = new GreaterThanDouble(step);
                    break;

                case ValGroup.Long:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(GreaterThanLong))
                        step.Evaluate = new GreaterThanLong(step);
                    break;

                case ValGroup.DateTime:
                    if (composite != ValGroup.DateTime)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(GreaterThanDateTime))
                        step.Evaluate = new GreaterThanDateTime(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsLHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValueType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveGreaterEqual  ==========================================
        static void ResolveGreaterEqual(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValueGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.String:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(GreaterEqualString))
                        step.Evaluate = new GreaterEqualString(step);
                    break;

                case ValGroup.Double:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(GreaterEqualDouble))
                        step.Evaluate = new GreaterEqualDouble(step);
                    break;

                case ValGroup.Long:
                    if ((composite & ~ValGroup.ScalarGroup) != 0)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(GreaterEqualLong))
                        step.Evaluate = new GreaterEqualLong(step);
                    break;

                case ValGroup.DateTime:
                    if (composite != ValGroup.DateTime)
                        step.Error = StepError.InvalidArgsRHS;
                    else if (type != typeof(GreaterEqualDateTime))
                        step.Evaluate = new GreaterEqualDateTime(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsLHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValueType == ValType.IsUnresolved);
        }
        #endregion
        #endregion
    }
}