﻿using System.Text;

namespace ModelGraph.Internals
{
    internal class GreaterThanDouble : ValueOfBool
    {
        internal GreaterThanDouble(ComputeStep step) { _step = step; }

        internal override string Text => " > ";

        protected override bool GetVal()
        {
            var val = (_step.Count != 2) ? false : _step.Input[0].Evaluate.AsDouble() > _step.Input[1].Evaluate.AsDouble();
            return _step.IsNegated ? !val : val;
        }
    }
}
