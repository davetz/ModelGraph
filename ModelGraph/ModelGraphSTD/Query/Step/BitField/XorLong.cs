using System;

namespace ModelGraphSTD
{
    internal class XorLong : ValueOfInt64
    {
        internal XorLong(ComputeStep step) { _step = step; }

        internal override string Text => " ^ ";

        protected override Int64 GetVal()
        {
            var N = _step.Count;
            var val = _step.Input[0].Evaluate.AsInt64();
            for (int i = 0; i < N; i++)
            {
                val ^= _step.Input[i].Evaluate.AsInt64();
            }
            return _step.IsInverse ? ~val : val;
        }
    }
}
