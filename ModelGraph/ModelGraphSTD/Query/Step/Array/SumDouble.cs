using System.Text;

namespace ModelGraphSTD
{
    internal class SumDouble : ValueOfDouble
    {
        internal SumDouble(ComputeStep step) { _step = step; }

        internal override string Text => "Sum";

        protected override double GetVal()
        {
            var v = _step.Input[0].Evaluate.AsDoubleArray();
            var N = (v == null || v.Length < 1) ? 0 : v.Length;
            var val = 0.0;
            for (int i = 0; i < N; i++)
            {
                val += v[i];
            }

            return val;
        }
    }
}
