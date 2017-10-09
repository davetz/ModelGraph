using System.Text;

namespace ModelGraphLibrary
{
    internal class EvaluateAnd2 : EvaluateBase
    {
        internal EvaluateAnd2(ComputeStep step) : base(step) { }

        bool GetVal(ComputeStep step)
        {
            var N = step.Count;
            for (int i = 0; i < N; i++)
            {
                step.Inputs[i].GetValue(out bool v1);
                if (!v1) return step.IsNegated ? true : false;
            }
            return step.IsNegated ? true : false;
        }
        internal override ValueType ValueType => ValueType.Bool;
        internal override string Text => " && ";

        #region EvaluateBase-v1  ==============================================
        internal override void GetValue(ComputeStep step, out bool value) => value = Value.ToBool(GetVal(step));
        internal override void GetValue(ComputeStep step, out byte value) => value = Value.ToByte(GetVal(step));
        internal override void GetValue(ComputeStep step, out int value) => value = Value.ToInt32(GetVal(step));
        internal override void GetValue(ComputeStep step, out long value) => value = Value.ToInt64(GetVal(step));
        internal override void GetValue(ComputeStep step, out short value) => value = Value.ToInt16(GetVal(step));
        internal override void GetValue(ComputeStep step, out sbyte value) => value = Value.ToSByte(GetVal(step));
        internal override void GetValue(ComputeStep step, out uint value) => value = Value.ToUInt32(GetVal(step));
        internal override void GetValue(ComputeStep step, out ulong value) => value = Value.ToUInt64(GetVal(step));
        internal override void GetValue(ComputeStep step, out ushort value) => value = Value.ToUInt16(GetVal(step));
        internal override void GetValue(ComputeStep step, out double value) => value = Value.ToDouble(GetVal(step));
        internal override void GetValue(ComputeStep step, out string value) => value = Value.ToString(GetVal(step));
        #endregion
    }
}
