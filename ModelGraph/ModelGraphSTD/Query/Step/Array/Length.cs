using System;
using System.Text;

namespace ModelGraphSTD
{
    internal class Length : ValueOfInt32
    {
        internal Length(ComputeStep step) { _step = step; }

        internal override string Text => "Length";

        protected override Int32 GetVal()
        {
            return _step.Input[0].Evaluate.AsLength();
        }
    }
}
