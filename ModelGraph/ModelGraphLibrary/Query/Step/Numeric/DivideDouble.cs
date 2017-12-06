using System.Text;

namespace ModelGraphLibrary
{
    internal class DivideDouble : ValueOfDouble
    {
        internal DivideDouble(ComputeStep step) { _step = step; }

        internal override string Text => " / ";

        protected override double GetVal()
        {
            var N = _step.Count;
            var val = _step.Input[0].Evaluate.AsDouble();
            for (int i = 1; i < N; i++)
            {
                val /= _step.Input[i].Evaluate.AsDouble();
            }
            return _step.IsNegated ? -val : val;
        }
    }
}
