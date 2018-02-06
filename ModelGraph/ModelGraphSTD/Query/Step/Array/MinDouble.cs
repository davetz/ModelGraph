using System.Text;

namespace ModelGraphSTD
{
    internal class MinDouble : ValueOfDouble
    {
        internal MinDouble(ComputeStep step) { _step = step; }

        internal override string Text => "Min";

        protected override double GetVal()
        {
            var v = _step.Input[0].Evaluate.AsDoubleArray();
            var N = (v == null || v.Length < 1) ? 0 : v.Length;
            var val = double.MaxValue;
            for (int i = 0; i < N; i++)
            {
                if (v[i] < val) val = v[i];
            }
            return val;
        }
    }
}
