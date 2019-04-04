using System.Text;

namespace ModelGraphSTD
{
    internal class AveDouble : ValueOfDouble
    {
        internal AveDouble(ComputeStep step) { _step = step; }

        internal override string Text => "Ave";

        protected override double GetVal()
        {
            var v = _step.Input[0].Evaluate.AsDoubleArray();
            var N = (v == null || v.Length < 1) ? 0 : v.Length;
            var val = 0.0;
            for (int i = 0; i < N; i++)
            {
                val += v[i];
            }
            if (N > 1) val = val / N;

            return val;
        }
    }
}
