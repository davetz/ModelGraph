
namespace ModelGraphSTD
{
    internal class LiteralBoolArray : ValueOfBoolArray
    {
        private string _text;
        private bool[] _value;

        internal LiteralBoolArray(ComputeStep step, bool[] value, string text)
        {
            _step = step;
            _text = text;
            _value = value;
        }
        internal override string Text => _text;

        protected override bool[] GetVal()
        {
            if (_step.IsNegated)
            {
                var N = _value.Length;
                var negate = new bool[N];
                for (int i = 0; i < N; i++) { negate[i] = !_value[i]; }
                return negate;
            }
            else
                return _value;
        }
    }
}
