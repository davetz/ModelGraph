using System.Text;

namespace ModelGraph.Internals
{
    internal class ConcatString : ValueOfString
    {
        internal ConcatString(ComputeStep step) { _step = step; }
        internal override string Text => " | ";

        protected override string GetVal()
        {
            var N = _step.Count;
            var val = _step.Input[0].Evaluate.AsString();
            for (int i = 1; i < N; i++)
            {
                val += _step.Input[i].Evaluate.AsString();
            }
            return val;
        }
    }
}
