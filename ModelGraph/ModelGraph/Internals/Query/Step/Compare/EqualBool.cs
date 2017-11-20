
namespace ModelGraph.Internals
{
    internal class EqualBool : ValueOfBool
    {
        internal EqualBool(ComputeStep step) { _step = step; }

        internal override string Text => " = ";

        protected override bool GetVal()
        {
            var val = (_step.Count != 2) ? false : _step.Input[0].Evaluate.AsBool() == _step.Input[1].Evaluate.AsBool();
            return _step.IsNegated ? !val : val;
        }
    }
}
