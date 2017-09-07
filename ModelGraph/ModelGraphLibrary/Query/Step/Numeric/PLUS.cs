using System.Text;

namespace ModelGraphLibrary
{
    internal class PLUS : Step
    {
        internal PLUS(Parser p) : base(p) { p.Step = this; }

        internal double GetVal()
        {
            double val = 0;
            var N = Count;
            for (int i = 0; i < N; i++)
            {
                Inputs[i].GetValue(out double v1);
                val += v1;
            }
            return val;
        }

        #region Methods  ======================================================
        internal override NativeType NativeType => NativeType.Double;
        internal override IStepValue GetValue() => new DoubleValue(GetVal()); 
        internal override void GetValue(out bool value) { value = Value.ToBool(GetVal()); }
        internal override void GetValue(out byte value) { value = Value.ToByte(GetVal()); }
        internal override void GetValue(out int value) { value = Value.ToInt32(GetVal()); }
        internal override void GetValue(out long value) { value = Value.ToInt64(GetVal()); }
        internal override void GetValue(out short value) { value = Value.ToInt16(GetVal()); }
        internal override void GetValue(out double value) { value = GetVal(); }
        internal override void GetValue(out string value) { value = Value.ToString(GetVal()); }
        internal override void GetText(StringBuilder sb)
        {
            GetPrefix(sb);
            Inputs[0].GetText(sb);
            for (int i = 1; i < Count; i++)
            {
                sb.Append(" +");
                Inputs[i].GetText(sb);
            }
            GetSufix(sb);
        }
        #endregion
    }
}
