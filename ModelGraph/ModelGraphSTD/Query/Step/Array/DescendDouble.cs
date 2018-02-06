using System.Linq;

namespace ModelGraphSTD
{
    internal class DescendDouble : ValueOfDoubleArray
    {
        internal DescendDouble(ComputeStep step) { _step = step; }

        internal override string Text => "Descend";

        protected override double[] GetVal()
        {
            var v = _step.Input[0].Evaluate.AsDoubleArray();
            return v.OrderByDescending((s) => s).ToArray();
        }
    }
}
