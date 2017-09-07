using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{
    internal class OR : Step
    {
        internal bool GetVal()
        {
            var N = Count;
            for (int i = 0; i < N; i++)
            {
                Inputs[i].GetValue(out bool v1);
                if (v1) return true;
            }
            return false;
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
            for (int i = 1; i < Count; i++)
            {
                sb.Append(" ||");
                Inputs[i].GetText(sb);
            }
            GetSufix(sb);
        }
        #endregion
    }
}
