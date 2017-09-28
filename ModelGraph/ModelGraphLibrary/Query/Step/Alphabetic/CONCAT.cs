using System.Text;

namespace ModelGraphLibrary
{
    internal class CONCAT : Step
    {
        internal CONCAT(Parser p) : base(p) { p.Step = this; }

        internal string GetVal()
        {
            var N = Count;
            Inputs[0].GetValue(out string val);
            for (int i = 1; i < N; i++)
            {
                Inputs[i].GetValue(out string v1);
                val += v1;
            }
            return val;
        }

        #region Methods  ======================================================
        internal override ValueType ValueType => ValueType.Double;

        internal override void GetValue(out bool value) => value = Value.ToBool(GetVal());
        internal override void GetValue(out byte value) => value = Value.ToByte(GetVal());
        internal override void GetValue(out int value) => value = Value.ToInt32(GetVal());
        internal override void GetValue(out long value) => value = Value.ToInt64(GetVal());
        internal override void GetValue(out short value) => value = Value.ToInt16(GetVal());
        internal override void GetValue(out sbyte value) => value = Value.ToSByte(GetVal());
        internal override void GetValue(out uint value) => value = Value.ToUInt32(GetVal());
        internal override void GetValue(out ulong value) => value = Value.ToUInt64(GetVal());
        internal override void GetValue(out ushort value) => value = Value.ToUInt16(GetVal());
        internal override void GetValue(out double value) => value = Value.ToDouble(GetVal());
        internal override void GetValue(out string value) => value = GetVal();

        internal override void GetText(StringBuilder sb)
        {
            GetPrefix(sb);
            Inputs[0].GetText(sb);
            for (int i = 1; i < Count; i++)
            {
                sb.Append(" |");
                Inputs[i].GetText(sb);
            }
            GetSufix(sb);
        }
        #endregion
    }
}
