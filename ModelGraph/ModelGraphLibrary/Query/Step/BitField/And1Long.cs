
namespace ModelGraphSTD
{
    internal class And1Long : ValueOfLong
    {
        internal And1Long(ComputeStep step) { _step = step; }

        internal override string Text => " | ";

        protected override long GetVal()
        {
            var N = _step.Count;
            var val = _step.Input[0].Evaluate.AsLong();
            for (int i = 0; i < N; i++)
            {
                val &= _step.Input[i].Evaluate.AsLong();
            }
            return _step.IsInverse ? ~val : val;
        }
    }
}
