using System;
using System.Linq;
using System.Text;

namespace ModelGraphSTD
{
    internal class AscendInt32 : ValueOfInt32Array
    {
        internal AscendInt32(ComputeStep step) { _step = step; }

        internal override string Text => "Ascend";

        protected override Int32[] GetVal()
        {
            var v = _step.Input[0].Evaluate.AsInt32Array();
            return v.OrderBy((s) => s).ToArray();
        }
    }
}
