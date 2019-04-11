using System;
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
                [StepType.Min] = ResolveMin,
                [StepType.Max] = ResolveMax,
                [StepType.Sum] = ResolveSum,
                [StepType.Ave] = ResolveAve,
                [StepType.Count] = ResolveCount,
                [StepType.Length] = ResolveLength,
                [StepType.Ascend] = ResolveAscend,
                [StepType.Descend] = ResolveDescend,

                [StepType.Or1] = ResolveOr1,
                [StepType.Or2] = ResolveOr2,

                [StepType.And1] = ResolveAnd1,
                [StepType.And2] = ResolveAnd2,

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
            step.Evaluate = Chef.LiteralUnresolved;
        }
        #endregion


        #region ResolveMin  ===================================================
        static void ResolveMin(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.IntArray:
                case ValGroup.LongArray:
                case ValGroup.DoubleArray:
                    step.Evaluate = new MinDouble(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsRHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveMax  ===================================================
        static void ResolveMax(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;

            switch (group)
            {
                case ValGroup.IntArray:
                case ValGroup.LongArray:
                case ValGroup.DoubleArray:
                    step.Evaluate = new MaxDouble(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsRHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveSum  ===================================================
        static void ResolveSum(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.IntArray:
                case ValGroup.LongArray:
                case ValGroup.DoubleArray:
                    step.Evaluate = new SumDouble(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsRHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveAve  ===================================================
        static void ResolveAve(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            switch (group)
            {
                case ValGroup.IntArray:
                case ValGroup.LongArray:
                case ValGroup.DoubleArray:
                    step.Evaluate = new AveDouble(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsRHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveCount  =================================================
        static void ResolveCount(ComputeStep step)
        {
            var type = step.Evaluate.GetType();

            if (step.Count == 1)
                step.Evaluate = new Count(step);
            else
                step.Error = StepError.InvalidArgsRHS;

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveLength  =================================================
        static void ResolveLength(ComputeStep step)
        {
            var type = step.Evaluate.GetType();

            if (step.Count == 1)
                step.Evaluate = new Length(step);
            else
                step.Error = StepError.InvalidArgsRHS;

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveAscend  ================================================
        static void ResolveAscend(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;

            switch (group)
            {
                case ValGroup.IntArray:
                    step.Evaluate = new AscendInt32(step);
                    break;
                case ValGroup.LongArray:
                    step.Evaluate = new AscendInt64(step);
                    break;
                case ValGroup.DoubleArray:
                    step.Evaluate = new AscendDouble(step);
                    break;
                case ValGroup.StringArray:
                    step.Evaluate = new AscendString(step);
                    break;
                case ValGroup.DateTimeArray:
                    step.Evaluate = new AscendDateTime(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsRHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion


        #region ResolveDescend  ===============================================
        static void ResolveDescend(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;

            switch (group)
            {
                case ValGroup.IntArray:
                    step.Evaluate = new DescendInt32(step);
                    break;
                case ValGroup.LongArray:
                    step.Evaluate = new DescendInt64(step);
                    break;
                case ValGroup.DoubleArray:
                    step.Evaluate = new DescendDouble(step);
                    break;
                case ValGroup.StringArray:
                    step.Evaluate = new DescendString(step);
                    break;
                case ValGroup.DateTimeArray:
                    step.Evaluate = new DescendDateTime(step);
                    break;

                default:
                    step.Error = StepError.InvalidArgsRHS;
                    break;
            }

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
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

        #region ResolveAnd1  ==================================================
        static void ResolveAnd1(ComputeStep step)
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
                    else if (type != typeof(And2Bool))
                        step.Evaluate = new And2Bool(step);
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
                        if (type != typeof(And1Long))
                            step.Evaluate = new And1Long(step);
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


        #region ResolveOr2  ===================================================
        static void ResolveOr2(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            if (composite == ValGroup.Bool)
                step.Evaluate = new Or2Bool(step);
            else
                step.Error = StepError.InvalidArgsLHS;

            step.IsError = (step.Error != StepError.None);
            step.IsChanged = (type != step.Evaluate.GetType());
            step.IsUnresolved = (step.Evaluate.ValType == ValType.IsUnresolved);
        }
        #endregion

        #region ResolveAnd2  ==================================================
        static void ResolveAnd2(ComputeStep step)
        {
            var type = step.Evaluate.GetType();
            var group = step.Input[0].Evaluate.ValGroup;
            var composite = step.ScanInputsAndReturnCompositeValueGroup();

            if (composite == ValGroup.Bool)
                step.Evaluate = new And2Bool(step);
            else
                step.Error = StepError.InvalidArgsLHS;

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