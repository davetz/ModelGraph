
namespace ModelGraphSTD
{
    internal class LessEqualDateTime : ValueOfBool
    {
        internal LessEqualDateTime(ComputeStep step) { _step = step; }

        internal override string Text => " <= ";

        protected override bool GetVal()
        {
            var val = (_step.Count != 2) ? false : _step.Input[0].Evaluate.AsDateTime() <= _step.Input[1].Evaluate.AsDateTime();
            return _step.IsNegated ? !val : val;
        }
    }
}
