﻿using System.Text;

namespace ModelGraphSTD
{
    internal class LessThanDouble : ValueOfBool
    {
        internal LessThanDouble(ComputeStep step) { _step = step; }

        internal override string Text => " < ";

        protected override bool GetVal()
        {
            var val = (_step.Count != 2) ? false : _step.Input[0].Evaluate.AsDouble() < _step.Input[1].Evaluate.AsDouble();
            return _step.IsNegated ? !val : val;
        }
    }
}
