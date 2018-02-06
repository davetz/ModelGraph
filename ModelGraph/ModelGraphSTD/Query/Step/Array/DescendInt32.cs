using System;
using System.Linq;
using System.Text;

namespace ModelGraphSTD
{
    internal class DescendInt32 : ValueOfInt32Array
    {
        internal DescendInt32(ComputeStep step) { _step = step; }

        internal override string Text => "Descend";

        protected override Int32[] GetVal()
        {
            var v = _step.Input[0].Evaluate.AsInt32Array();
            return v.OrderByDescending((s) => s).ToArray();
        }
    }
}
