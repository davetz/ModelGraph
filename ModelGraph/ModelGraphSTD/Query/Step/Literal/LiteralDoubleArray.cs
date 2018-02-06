
namespace ModelGraphSTD
{
    internal class LiteralDoubleArray : ValueOfDoubleArray
    {
        private string _text;
        private double[] _value;

        internal LiteralDoubleArray(ComputeStep step, double[] value, string text)
        {
            _step = step;
            _text = text;
            _value = value;
        }
        internal override string Text => _text;

        protected override double[] GetVal()
        {
            if (_step.IsNegated)
            {
                var N = _value.Length;
                var negate = new double[N];
                for (int i = 0; i < N; i++) { negate[i] = -_value[i]; }
                return negate;
            }
            else
                return _value;
        }
    }
}