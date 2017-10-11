using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphLibrary
{/*
    ComputeStep is a node in an expression tree. The expression tree is used in
    a query's where/select clause. When employed in a select clause, it computes
    a value based on an item's properties. In the context of a where clause it can
    qualify specific relational paths based on the properties of the encountered rows
    while traversing the relational path.

    The choice the evaluation function is set durring the where/select clause
    validation. The actual value-types and choice of evaluation function
    ripple up from a depth-first traversal of the expression tree.
 */
    internal class ComputeStep
    {
        internal ComputeStep[] Inputs;  // a step may have zero or more inputs
        internal EvaluateBase Evaluate;    // evaluate this step and produce a value
        internal StepType StepType;     // parser's initial clasification of this step
        internal StepError Error;       // keeps track of TryParse and TryResove errors
        private StepFlag1 _flags1;      // used when creating the expression string
        private StepFlag2 _flags2;      // used by TryResolve()

        #region Constructor  ==================================================
        internal ComputeStep(EvaluateParse evaluate)
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

        internal bool IsNegated { get { return GetF1(StepFlag1.IsNegated); } set { SetF1(value, StepFlag1.IsNegated); } }
        internal bool IsBatched { get { return GetF1(StepFlag1.IsBatched); } set { SetF1(value, StepFlag1.IsBatched); } }
        internal bool HasParens { get { return GetF1(StepFlag1.HasParens); } set { SetF1(value, StepFlag1.HasParens); } }
        internal bool HasNewLine { get { return GetF1(StepFlag1.HasNewLine); } set { SetF1(value, StepFlag1.HasNewLine); } }
        internal bool ParseAborted { get { return GetF1(StepFlag1.ParseAborted); } set { SetF1(value, StepFlag1.ParseAborted); } }
        #endregion

        #region Flags2  =======================================================
        internal bool AnyChange => (_flags2 & StepFlag2.AnyChange) != 0;

        internal bool AnyUnresolved => (_flags2 & StepFlag2.AnyUnresolved) != 0;

        internal bool AnyInt => (_flags2 & StepFlag2.AnyInt) != 0;
        internal bool AnyUInt => (_flags2 & StepFlag2.AnyUInt) != 0;
        internal bool AnyULong => (_flags2 & StepFlag2.AnyULong) != 0;
        internal bool AnyDouble => (_flags2 & StepFlag2.AnyDouble) != 0;
        internal bool AnyString => (_flags2 & StepFlag2.AnyString) != 0;
        internal bool AnyOtherType => (_flags2 & StepFlag2.AnyOtherType) != 0;

        internal void UpdateResolveState()
        {
            var previous = _flags2 & ~StepFlag2.AnyChange;  // previous state minus the AnyChange flag
            _flags2 = StepFlag2.None;                       // clear the current state

            if (ValueType > ValueType.MaximumType) _flags2 |= StepFlag2.AnyUnresolved;

            var N = Count;
            for (int i = 0; i < N; i++)
            {
                if (Inputs[i].AnyChange) _flags2 |= StepFlag2.AnyChange; // bubble-up any step has changed
                if (Inputs[i].AnyUnresolved) _flags2 |= StepFlag2.AnyUnresolved; // bubble-up any step is unresoved

                var valType = Inputs[i].ValueType;

                if (valType > ValueType.MaximumType) _flags2 = StepFlag2.AnyUnresolved;

                else if (valType == ValueType.String) _flags2 |= StepFlag2.AnyString;

                else if (valType == ValueType.SByte) _flags2 |= StepFlag2.AnyInt;
                else if (valType == ValueType.Int16) _flags2 |= StepFlag2.AnyInt;
                else if (valType == ValueType.Int32) _flags2 |= StepFlag2.AnyInt;

                else if (valType == ValueType.Int64) _flags2 |= StepFlag2.AnyDouble;
                else if (valType == ValueType.Single) _flags2 |= StepFlag2.AnyDouble;
                else if (valType == ValueType.Double) _flags2 |= StepFlag2.AnyDouble;
                else if (valType == ValueType.Decimal) _flags2 |= StepFlag2.AnyDouble;

                else if (valType == ValueType.Byte) _flags2 |= StepFlag2.AnyUInt;
                else if (valType == ValueType.UInt16) _flags2 |= StepFlag2.AnyUInt;
                else if (valType == ValueType.UInt32) _flags2 |= StepFlag2.AnyUInt;

                else if (valType == ValueType.UInt64) _flags2 |= StepFlag2.AnyULong;

                else _flags2 |= StepFlag2.AnyOtherType;
            }

            var current = _flags2 & ~StepFlag2.AnyChange; //current state minus the AnyChange flag
            if (previous != current) _flags2 |= StepFlag2.AnyChange;
        }
        #endregion

        #region IsValid  ======================================================
        internal bool IsValid => GetIsValid();
        private bool GetIsValid()
        {
            if (ParseAborted) return false;
            if (Error != StepError.None) return false;

            if (Inputs != null)
            {
                foreach (var step in Inputs)
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
            if (StepType == StepType.Property && Evaluate is EvaluateParse e)
            {
                if (sto.TryLookUpProperty(e.Text, out Property property, out int index))
                {
                    StepType = StepType.Property;
                    Evaluate = new EvaluateProperty(property, index, getItem);
                }
                else
                {
                    Error = StepError.UnknownProperty;
                    return false;
                }
            }
            else if (Inputs != null)
            {
                foreach (var step in Inputs)
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
            if (Inputs != null) foreach (var step in Inputs) { step.TryResolve(); } // recursive depth-first traversal

            if (StepTypeResolve.TryGetValue(StepType, out Action<ComputeStep> resolve)) resolve(this);

            return AnyChange;
        }
        #endregion


        #region Property  =====================================================
        internal int Count => (Inputs == null) ? 0 : Inputs.Length;
        internal ValueType ValueType => Evaluate.ValueType;

        internal void GetValue(out bool value) => Evaluate.GetValue(this, out value);
        internal void GetValue(out byte value) => Evaluate.GetValue(this, out value);
        internal void GetValue(out sbyte value) => Evaluate.GetValue(this, out value);
        internal void GetValue(out uint value) => Evaluate.GetValue(this, out value);
        internal void GetValue(out int value) => Evaluate.GetValue(this, out value);
        internal void GetValue(out ushort value) => Evaluate.GetValue(this, out value);
        internal void GetValue(out short value) => Evaluate.GetValue(this, out value);
        internal void GetValue(out ulong value) => Evaluate.GetValue(this, out value);
        internal void GetValue(out long value) => Evaluate.GetValue(this, out value);
        internal void GetValue(out double value) => Evaluate.GetValue(this, out value);
        internal void GetValue(out string value) => Evaluate.GetValue(this, out value);
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
                    case ValueType.Bool:
                        sb.Append('!');
                        break;
                    case ValueType.SByte:
                    case ValueType.Int16:
                    case ValueType.Int32:
                    case ValueType.Int64:
                    case ValueType.Double:
                        sb.Append('-');
                        break;
                    case ValueType.Byte:
                    case ValueType.UInt16:
                    case ValueType.UInt32:
                    case ValueType.UInt64:
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
                Inputs[0].GetText(sb); //<- - - - - recursive call
            }
            else
            {
                Inputs[0].GetText(sb); //<- - - - - recursive call
                for (int i = 1; i < N; i++)
                {
                    sb.Append(Evaluate.Text);
                    Inputs[i].GetText(sb); //<- - - recursive call
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
                [StepType.Or1] = ResolveFails,
                [StepType.Or2] = ResolveFails,
                [StepType.And1] = ResolveFails,
                [StepType.And2] = ResolveFails,
                [StepType.Not] = ResolveFails,
                [StepType.Plus] = ResolvePlus,
                [StepType.Minus] = ResolveMinus,
                [StepType.Equals] = ResolveFails,
                [StepType.Negate] = ResolveFails,
                [StepType.Divide] = ResolveDivide,
                [StepType.Multiply] = ResolveMultiply,
                [StepType.LessThan] = ResolveFails,
                [StepType.GreaterThan] = ResolveFails,
                [StepType.NotLessThan] = ResolveFails,
                [StepType.NotGreaterThan] = ResolveFails,
            };
        #endregion

        #region Resolve  ======================================================

        #region ResolveFails  =================================================
        static void ResolveFails(ComputeStep step)
        {
            step.Evaluate = Chef.EvaluateUnresolved;
        }
        #endregion

        #region ResolvePlus  ==================================================
        static void ResolvePlus(ComputeStep step)
        {
            step.UpdateResolveState();

            if (step.Inputs[0].Evaluate.ValueType == ValueType.String && step.Evaluate.ValueType != ValueType.String)
                step.Evaluate = new EvaluateConcat();
            else if (step.Inputs[0].Evaluate.ValueType == ValueType.Bool && step.Evaluate.ValueType != ValueType.Bool)
                step.Evaluate = new EvaluateOr2();
            else
            {
                if (step.AnyOtherType) { if (step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoublePlus(); }
                else if (step.AnyDouble) { if (step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoublePlus(); }
                else if (step.AnyInt) { if (step.ValueType != ValueType.Int32) step.Evaluate = new EvaluateIntegerPlus(); }
                else if (step.AnyULong) { if (step.ValueType != ValueType.UInt64) step.Evaluate = new EvaluateULongOr1(); }
                else if (step.AnyUInt) { if (step.ValueType != ValueType.UInt32) step.Evaluate = new EvaluateUIntOr1(); }
            }
        }
        #endregion

        #region ResolveMinus  =================================================
        static void ResolveMinus(ComputeStep step)
        {
            step.UpdateResolveState();

            if (step.Inputs[0].Evaluate.ValueType == ValueType.String && step.Evaluate.ValueType != ValueType.String)
                step.Evaluate = new EvaluateRemove();
            else if (step.Inputs[0].Evaluate.ValueType == ValueType.Bool && step.Evaluate.ValueType != ValueType.Bool)
                step.Evaluate = new EvaluateAnd2();
            else
            {
                if (step.AnyOtherType) { if (step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoubleMinus(); }
                else if (step.AnyDouble) { if (step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoubleMinus(); }
                else if (step.AnyInt) { if (step.ValueType != ValueType.Int32) step.Evaluate = new EvaluateIntegerMinus(); }
                else if (step.AnyULong) { if (step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoubleMinus(); }
                else if (step.AnyUInt) { if (step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoubleMinus(); }
            }
        }
        #endregion

        #region ResolveDivide  ================================================
        static void ResolveDivide(ComputeStep step)
        {
            step.UpdateResolveState();
            if (step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoubleDivide();
        }
        #endregion

        #region ResolveMultiply  ==============================================
        static void ResolveMultiply(ComputeStep step)
        {
            step.UpdateResolveState();
            if (step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoubleMultiply();
        }
        #endregion

        #endregion
    }
}