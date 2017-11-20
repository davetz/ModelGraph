
using System;

namespace ModelGraph.Internals
{
    internal class EqualString : ValueOfBool
    {
        internal EqualString(ComputeStep step) { _step = step; }

        internal override string Text => " = ";

        protected override bool GetVal()
        {
            var val = (_step.Count != 2) ? false : (string.Compare(_step.Input[0].Evaluate.AsString(), _step.Input[1].Evaluate.AsString(), true) == 0);
            return _step.IsNegated ? !val : val;
        }
    }
}
