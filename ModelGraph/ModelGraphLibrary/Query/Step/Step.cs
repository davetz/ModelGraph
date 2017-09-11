using System;
using System.Text;

namespace ModelGraphLibrary
{/*
    A Step is an expression tree node. The expression tree is used in a query's
    where and select clause. A Step computes a value based on an item's properties
    and when used in a where clause they qualify specific relational paths.
 */
    internal abstract class Step
    {
        internal Step[] Inputs;      // a step may have zero or more inputs
        private StepFlag _stepFlags; // used when generating a subexpression string

        #region Constructor  ==================================================
        internal Step() { }
        internal Step(Parser p)
        {
            IsBatched = p.IsBatched;
            HasParens = (p.HasParens || p.Parent.HasParens);
            IsNegated = (p.IsNegated || p.Parent.IsNegated);
            HasNewLine = (p.HasNewLine || p.Parent.HasNewLine);

            var N = p.Arguments.Count;
            if (N > 0)
            {
                Inputs = new Step[N];
                for (int i = 0; i < N; i++)
                {
                    Inputs[i] = p.Arguments[i].Step;
                    if (Inputs[i] == null && p.Arguments[i].Children.Count == 1)
                        Inputs[i] = p.Arguments[i].Children[0].Step;
                    if (Inputs[i] == null)
                        p.Error = ParseError.InvalidText;
                }
            }
        }
        #endregion

        #region Flags  ========================================================
        bool GetFlag(StepFlag flag) => (_stepFlags & flag) != 0;
        void SetFlag(bool val, StepFlag flag) { if (val) _stepFlags |= flag; else _stepFlags &= ~flag; }

        internal bool IsWierd { get { return GetFlag(StepFlag.IsWierd); } set { SetFlag(value, StepFlag.IsWierd); } }
        internal bool IsBatched { get { return GetFlag(StepFlag.IsBatched); } set { SetFlag(value, StepFlag.IsBatched); } }
        internal bool IsNegated { get { return GetFlag(StepFlag.IsNegated); } set { SetFlag(value, StepFlag.IsNegated); } }
        internal bool HasParens { get { return GetFlag(StepFlag.HasParens); } set { SetFlag(value, StepFlag.HasParens); } }
        internal bool HasNewLine { get { return GetFlag(StepFlag.HasNewLine); } set { SetFlag(value, StepFlag.HasNewLine); } }
        internal bool IsBitField { get { return GetFlag(StepFlag.IsBitField); } set { SetFlag(value, StepFlag.IsBitField); } }

        #endregion

        #region Methods  ======================================================
        internal int Count => (Inputs == null) ? 0 : Inputs.Length;
        internal abstract NativeType NativeType { get; } // output native data type
        internal abstract void GetValue(out bool value);
        internal abstract void GetValue(out byte value);
        internal abstract void GetValue(out int value);
        internal abstract void GetValue(out short value);
        internal abstract void GetValue(out long value);
        internal abstract void GetValue(out double value);
        internal abstract void GetValue(out string value);
        internal abstract void GetText(StringBuilder sb);
        protected void GetPrefix(StringBuilder sb)
        {
            if (HasNewLine) sb.Append(Environment.NewLine);
            if (IsNegated)
            {
                switch (NativeType)
                {
                    case NativeType.Bool:
                        sb.Append('!');
                        break;
                    case NativeType.SByte:
                    case NativeType.Int16:
                    case NativeType.Int32:
                    case NativeType.Int64:
                    case NativeType.Double:
                        sb.Append('-');
                        break;
                    case NativeType.Byte:
                    case NativeType.UInt16:
                    case NativeType.UInt32:
                    case NativeType.UInt64:
                        sb.Append('~');
                        break;
                }
            }
            if (HasParens) sb.Append('(');
        }
        protected void GetSufix(StringBuilder sb)
        {
            if (HasParens) sb.Append(')');
        }
        #endregion
    }
}