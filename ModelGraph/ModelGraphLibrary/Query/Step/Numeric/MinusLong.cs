using System.Text;

namespace ModelGraph.Internals
{
    internal class MinusLong : ValueOfLong
    {
        internal MinusLong(ComputeStep step) { _step = step; }
        internal override string Text => " - ";

        protected override long GetVal()
        {
            var N = _step.Count;
            var val = _step.Input[0].Evaluate.AsLong();
            for (int i = 1; i < N; i++)
            {
                val -= _step.Input[i].Evaluate.AsLong();
            }
            return _step.IsNegated ? -val : val;
        }
    }
}
