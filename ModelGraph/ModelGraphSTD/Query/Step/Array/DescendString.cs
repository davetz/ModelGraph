using System.Linq;
using System.Text;

namespace ModelGraphSTD
{
    internal class DescendString : ValueOfStringArray
    {
        internal DescendString(ComputeStep step) { _step = step; }

        internal override string Text => "Descend";

        protected override string[] GetVal()
        {
            var v = _step.Input[0].Evaluate.AsStringArray();
            return v.OrderByDescending((s) => s).ToArray();
        }
    }
}
