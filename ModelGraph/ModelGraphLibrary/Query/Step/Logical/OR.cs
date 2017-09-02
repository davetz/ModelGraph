using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{
    public class OR : Step
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
        internal override void GetValue(out bool value) { value = GetVal(); }
        internal override void GetValue(out byte value) { value = Value.ToByte(GetVal()); }
        internal override void GetValue(out int value) { value = Value.ToInt32(GetVal()); }
        internal override void GetValue(out long value) { value = Value.ToInt64(GetVal()); }
        internal override void GetValue(out short value) { value = Value.ToInt16(GetVal()); }
        internal override void GetValue(out double value) { value = Value.ToDouble(GetVal()); }
        internal override void GetValue(out string value) { value = Value.ToString(GetVal()); }
        #endregion

    }
}
