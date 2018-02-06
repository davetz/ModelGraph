using System;
using System.Linq;

namespace ModelGraphSTD
{
    internal class DescendInt64 : ValueOfInt64Array
    {
        internal DescendInt64(ComputeStep step) { _step = step; }

        internal override string Text => "Descend";

        protected override Int64[] GetVal()
        {
            var v = _step.Input[0].Evaluate.AsInt64Array();
            return v.OrderByDescending((s) => s).ToArray();
        }
    }
}
