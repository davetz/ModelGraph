 namespace ModelGraphLibrary
{
    internal abstract class EvaluateBase // version - v1
    {
        internal EvaluateBase(ComputeStep step) { if (step != null) step.AnyChange = true; }
        internal abstract string Text { get; }
        internal abstract ValueType ValueType { get; }

        internal abstract void GetValue(ComputeStep step, out bool value);
        internal abstract void GetValue(ComputeStep step, out byte value);
        internal abstract void GetValue(ComputeStep step, out sbyte value);
        internal abstract void GetValue(ComputeStep step, out uint value);
        internal abstract void GetValue(ComputeStep step, out int value);
        internal abstract void GetValue(ComputeStep step, out ushort value);
        internal abstract void GetValue(ComputeStep step, out short value);
        internal abstract void GetValue(ComputeStep step, out ulong value);
        internal abstract void GetValue(ComputeStep step, out long value);
        internal abstract void GetValue(ComputeStep step, out double value);
        internal abstract void GetValue(ComputeStep step, out string value);
    }
}
