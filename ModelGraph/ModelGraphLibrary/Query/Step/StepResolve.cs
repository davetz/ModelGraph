﻿using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    internal partial class ComputeStep
    {
        #region TryResolve  ===================================================
        /// <summary>
        /// Depth-First traversal of the expression tree
        /// </summary>
        internal bool TryResolve()
        {
            InitResolveFlags();

            if (Input != null) foreach (var step in Input) { step.TryResolve(); } // recursive depth-first traversal

            if (_stepTypeResolve.TryGetValue(StepType, out Action<ComputeStep> resolve)) resolve(this);

            return AnyChange;
        }
        static Dictionary<StepType, Action<ComputeStep>> _stepTypeResolve =
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
            var group = step.Input[0].Evaluate.ValGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            if ((composite & ValGroup.String) != 0) group = ValGroup.String;

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
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion


        #region ResolvePlus  ==================================================
        static void ResolvePlus(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
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
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveMinus  =================================================
        static void ResolveMinus(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
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
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveDivide  ================================================
        static void ResolveDivide(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
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
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveMultiply  ==============================================
        static void ResolveMultiply(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
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
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion


        #region ResolveEqual  =================================================
        static void ResolveEqual(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
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
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveNotEqual  ==============================================
        static void ResolveNotEqual(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
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
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion


        #region ResolveLessThan  ==============================================
        static void ResolveLessThan(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
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
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveLessEqual  =============================================
        static void ResolveLessEqual(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
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
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion


        #region ResolveGreaterThan  ===========================================
        static void ResolveGreaterThan(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
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
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveGreaterEqual  ==========================================
        static void ResolveGreaterEqual(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
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
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion
    }
}