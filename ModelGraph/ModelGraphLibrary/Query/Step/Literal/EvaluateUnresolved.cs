using System.Text;

namespace ModelGraphLibrary
{
    /// <summary>
    /// The default ComputeStep.Evaluate function set by the ComputeStep constructor
    /// </summary>
    internal class EvaluateUnresolved : EvaluateBase
    {
        internal override ValueType ValueType => ValueType.IsUnresolved;
        internal override string Text => "????";

        #region EvaluateBase-v1 (Specialized)  ================================
        internal override void GetValue(ComputeStep step, out bool value) => value = Value.ToBool(false);
        internal override void GetValue(ComputeStep step, out byte value) => value = Value.ToByte(0);
        internal override void GetValue(ComputeStep step, out int value) => value = Value.ToInt32(0);
        internal override void GetValue(ComputeStep step, out long value) => value = Value.ToInt64(0);
        internal override void GetValue(ComputeStep step, out short value) => value = Value.ToInt16(0);
        internal override void GetValue(ComputeStep step, out sbyte value) => value = Value.ToSByte(0);
        internal override void GetValue(ComputeStep step, out uint value) => value = Value.ToUInt32(0);
        internal override void GetValue(ComputeStep step, out ulong value) => value = Value.ToUInt64(0);
        internal override void GetValue(ComputeStep step, out ushort value) => value = Value.ToUInt16(0);
        internal override void GetValue(ComputeStep step, out double value) => value = Value.ToDouble(0);
        internal override void GetValue(ComputeStep step, out string value) => value = Value.ToString(Text);
        #endregion
    }
}
