using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{
    internal class GT : Step
    {
        internal GT(Parser p) : base(p) { p.Step = this; }

        internal bool GetVal()
        {
            Inputs[0].GetValue(out double v1);
            Inputs[1].GetValue(out double v2);
            var val = v1 > v2;
            return IsNegated ? !val : val;
        }

        #region Methods  ======================================================
        internal override NativeType NativeType => NativeType.Bool;

        internal override void GetValue(out bool value) { value = GetVal(); }
        internal override void GetValue(out byte value) { value = Value.ToByte(GetVal()); }
        internal override void GetValue(out int value) { value = Value.ToInt32(GetVal()); }
        internal override void GetValue(out long value) { value = Value.ToInt64(GetVal()); }
        internal override void GetValue(out short value) { value = Value.ToInt16(GetVal()); }
        internal override void GetValue(out double value) { value = Value.ToDouble(GetVal()); }
        internal override void GetValue(out string value) { value = Value.ToString(GetVal()); }

        internal override void GetText(StringBuilder sb)
        {
            GetPrefix(sb);
            Inputs[0].GetText(sb);
            sb.Append(" > ");
            Inputs[1].GetText(sb);
            GetSufix(sb);
        }
        #endregion
    }
}
