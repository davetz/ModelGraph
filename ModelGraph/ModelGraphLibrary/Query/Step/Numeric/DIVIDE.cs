using System.Text;

namespace ModelGraphLibrary
{
    internal class DIVIDE : Step
    {
        internal DIVIDE(Parser p) : base(p) { p.Step = this; }

        internal double GetVal()
        {
            var N = Count;
            Inputs[0].GetValue(out double val);
            for (int i = 1; i < N; i++)
            {
                Inputs[i].GetValue(out double v1);
                val /= v1;
            }
            return val;
        }

        #region Methods  ======================================================
        internal override NativeType NativeType => NativeType.Double;
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
                sb.Append(" / ");
                Inputs[i].GetText(sb);
            }
            GetSufix(sb);
        }
        #endregion
    }
}
