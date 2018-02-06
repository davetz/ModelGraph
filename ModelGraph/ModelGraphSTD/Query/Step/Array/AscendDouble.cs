using System.Linq;
using System.Text;

namespace ModelGraphSTD
{
    internal class AscendDouble : ValueOfDoubleArray
    {
        internal AscendDouble(ComputeStep step) { _step = step; }

        internal override string Text => "Ascend";

        protected override double[] GetVal()
        {
            var v = _step.Input[0].Evaluate.AsDoubleArray();
            return v.OrderBy((s) => s).ToArray();
        }
    }
}
