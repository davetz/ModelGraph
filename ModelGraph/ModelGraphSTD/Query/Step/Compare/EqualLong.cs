﻿
namespace ModelGraphSTD
{
    internal class EqualLong : ValueOfBool
    {
        internal EqualLong(ComputeStep step) { _step = step; }

        internal override string Text => " = ";

        protected override bool GetVal()
        {
            var val = (_step.Count != 2) ? false : _step.Input[0].Evaluate.AsInt64() == _step.Input[1].Evaluate.AsInt64();
            return _step.IsNegated ? !val : val;
        }
    }
}
