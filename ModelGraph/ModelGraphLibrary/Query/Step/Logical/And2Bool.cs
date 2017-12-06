
namespace ModelGraphLibrary
{
    internal class And2Bool : ValueOfBool
    {
        internal And2Bool(ComputeStep step) { _step = step; }

        internal override string Text => " || ";

        protected override bool GetVal()
        {
            var N = _step.Count;
            var val = _step.Input[0].Evaluate.AsBool();
            for (int i = 0; i < N; i++)
            {
                val = val && _step.Input[i].Evaluate.AsBool();
            }
            return _step.IsNegated ? !val : val;
        }
    }
}
