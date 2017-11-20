
namespace ModelGraph.Internals
{
    internal class LessEqualLong : ValueOfBool
    {
        internal LessEqualLong(ComputeStep step) { _step = step; }

        internal override string Text => " <= ";

        protected override bool GetVal()
        {
            var val = (_step.Count != 2) ? false : _step.Input[0].Evaluate.AsLong() <= _step.Input[1].Evaluate.AsLong();
            return _step.IsNegated ? !val : val;
        }
    }
}
