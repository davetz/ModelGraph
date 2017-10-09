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
        internal bool IsUnresolved { get { return GetF1(StepFlag1.IsUnresolved); } set { SetF1(value, StepFlag1.IsUnresolved); } }
        internal bool IsPropertyRef { get { return GetF1(StepFlag1.IsPropertyRef); } set { SetF1(value, StepFlag1.IsPropertyRef); } }
        #endregion

        #region Flags2  =======================================================
        bool GetF2(StepFlag2 flag) => (_flags2 & flag) != 0;
        void SetF2(bool val, StepFlag2 flag) { if (val) _flags2 |= flag; else _flags2 &= ~flag; }

        internal bool AnyChange { get { return GetF2(StepFlag2.AnyChange); } set { SetF2(value, StepFlag2.AnyChange); } }
        internal bool AnyInvalid { get { return GetF2(StepFlag2.AnyInvalid); } set { SetF2(value, StepFlag2.AnyInvalid); } }
        internal bool AnyUnresolved { get { return GetF2(StepFlag2.AnyUnresolved); } set { SetF2(value, StepFlag2.AnyUnresolved); } }
        internal bool AnyUInt { get { return GetF2(StepFlag2.AnyUInt); } set { SetF2(value, StepFlag2.AnyUInt); } }
        internal bool AnyULong { get { return GetF2(StepFlag2.AnyULong); } set { SetF2(value, StepFlag2.AnyULong); } }
        internal bool AnyInt { get { return GetF2(StepFlag2.AnyInt); } set { SetF2(value, StepFlag2.AnyInt); } }
        internal bool AnyDouble { get { return GetF2(StepFlag2.AnyDouble); } set { SetF2(value, StepFlag2.AnyDouble); } }
        internal bool AnyNonNumeric { get { return GetF2(StepFlag2.AnyNonNumeric); } set { SetF2(value, StepFlag2.AnyNonNumeric); } }

        internal void ResetFlags2() => _flags2 = StepFlag2.None;
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
            if (IsPropertyRef && StepType == StepType.Parse && Evaluate is EvaluateParse e)
            {
                if (sto.TryLookUpProperty(e.Text, out Property property, out int index))
                {
                    StepType = StepType.Property;
                    Evaluate = new EvaluateProperty(this, property, index, getItem);
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
        internal bool TryResolve ()
        {
            if (Inputs != null) foreach (var step in Inputs) { TryResolve(); } // recursive depth-first traversal

            if (StepTypeResolve.TryGetValue(StepType, out Action<ComputeStep> resolve)) resolve(this);

            return AnyChange;
        }
        #endregion


        #region Property  =====================================================
        internal int Count => (Inputs == null) ? 0 : Inputs.Length;
        internal ValueType ValueType => Evaluate.ValueType;

        internal void GetValue(out bool value) => Evaluate.GetValue(this, out value);
        internal  void GetValue(out byte value) => Evaluate.GetValue(this, out value);
        internal  void GetValue(out sbyte value) => Evaluate.GetValue(this, out value);
        internal  void GetValue(out uint value) => Evaluate.GetValue(this, out value);
        internal  void GetValue(out int value) => Evaluate.GetValue(this, out value);
        internal  void GetValue(out ushort value) => Evaluate.GetValue(this, out value);
        internal  void GetValue(out short value) => Evaluate.GetValue(this, out value);
        internal  void GetValue(out ulong value) => Evaluate.GetValue(this, out value);
        internal  void GetValue(out long value) => Evaluate.GetValue(this, out value);
        internal  void GetValue(out double value) => Evaluate.GetValue(this, out value);
        internal  void GetValue(out string value) => Evaluate.GetValue(this, out value);
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

            Inputs[0].GetText(sb); //<- - - - - recursive call
            for (int i = 1; i < Count; i++)
            {
                sb.Append(Evaluate.Text);
                Inputs[i].GetText(sb); //<- - - recursive call
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
                [StepType.Divide] = ResolveFails,
                [StepType.Multiply] = ResolveFails,
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
            GetStepFlags2(step);
            if (step.AnyInvalid) return;

            if (step.Inputs[0].Evaluate.ValueType == ValueType.String && step.Evaluate.ValueType != ValueType.String)
                step.Evaluate = new EvaluateConcat(step);
            else if (step.Inputs[0].Evaluate.ValueType == ValueType.Bool && step.Evaluate.ValueType != ValueType.Bool)
                step.Evaluate = new EvaluateOr2(step);
            else
            {
                if (step.AnyNonNumeric && step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoublePlus(step);
                else if (step.AnyDouble && step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoublePlus(step);
                else if (step.AnyInt && step.ValueType != ValueType.Int32) step.Evaluate = new EvaluateIntegerPlus(step);
                else if (step.AnyULong && step.ValueType != ValueType.UInt64) step.Evaluate = new EvaluateULongOr1(step);
                else if (step.AnyUInt && step.ValueType != ValueType.UInt32) step.Evaluate = new EvaluateUIntOr1(step);
            }
        }
        #endregion

        #region ResolveMinus  =================================================
        static void ResolveMinus(ComputeStep step)
        {
            GetStepFlags2(step);
            if (step.AnyInvalid) return;

            if (step.Inputs[0].Evaluate.ValueType == ValueType.String && step.Evaluate.ValueType != ValueType.String)
                step.Evaluate = new EvaluateRemove(step);
            else if (step.Inputs[0].Evaluate.ValueType == ValueType.Bool && step.Evaluate.ValueType != ValueType.Bool)
                step.Evaluate = new EvaluateAnd2(step);
            else
            {
                if (step.AnyNonNumeric && step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoubleMinus(step);
                else if (step.AnyDouble && step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoubleMinus(step);
                else if (step.AnyInt && step.ValueType != ValueType.Int32) step.Evaluate = new EvaluateIntegerMinus(step);
                else if (step.AnyULong && step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoubleMinus(step);
                else if (step.AnyUInt && step.ValueType != ValueType.Double) step.Evaluate = new EvaluateDoubleMinus(step);
            }
        }
        #endregion

        #region GetStepFlags2  =================================================
        /// <summary>
        /// Get aggregate of numeric types for all inputs
        /// </summary>
        static void GetStepFlags2(ComputeStep step)
        {
            step.ResetFlags2();

            var N = step.Count;
            for (int i = 0; i < N; i++)
            {
                if (step.Inputs[i].AnyChange) step.AnyChange = true; // bubble-up any step changed
                if (step.Inputs[i].AnyInvalid) step.AnyInvalid = true; // bubble-up any step is invalid
                if (step.Inputs[i].AnyUnresolved) step.AnyUnresolved = true; // bubble-up any step is unresoved

                if (step.Inputs[i].ValueType > ValueType.MaximumType)
                {
                    if (step.Inputs[i].ValueType == ValueType.IsInvalid) step.AnyInvalid = true;

                    step.AnyChange = !step.IsUnresolved;
                    step.IsUnresolved = true;
                    step.AnyUnresolved = true;
                    break;
                }
                else if (step.Inputs[i].ValueType == ValueType.Byte) step.AnyUInt = true;
                else if (step.Inputs[i].ValueType == ValueType.UInt16) step.AnyUInt = true;
                else if (step.Inputs[i].ValueType == ValueType.UInt32) step.AnyUInt = true;
                else if (step.Inputs[i].ValueType == ValueType.UInt64) step.AnyULong = true;
                else if (step.Inputs[i].ValueType == ValueType.SByte) step.AnyInt = true;
                else if (step.Inputs[i].ValueType == ValueType.Int16) step.AnyInt = true;
                else if (step.Inputs[i].ValueType == ValueType.Int32) step.AnyInt = true;
                else if (step.Inputs[i].ValueType == ValueType.Int64) step.AnyDouble = true;
                else if (step.Inputs[i].ValueType == ValueType.Single) step.AnyDouble = true;
                else if (step.Inputs[i].ValueType == ValueType.Double) step.AnyDouble = true;
                else if (step.Inputs[i].ValueType == ValueType.Decimal) step.AnyDouble = true;
                else step.AnyNonNumeric = true;
            }
        }
        #endregion

        #endregion
    }
}