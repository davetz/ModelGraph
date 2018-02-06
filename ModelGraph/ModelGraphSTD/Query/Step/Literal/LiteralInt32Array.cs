using System;

namespace ModelGraphSTD
{
    internal class LiteralInt32Array : ValueOfInt32Array
    {
        private string _text;
        private Int32[] _value;

        internal LiteralInt32Array(ComputeStep step, Int32[] value, string text)
        {
            _step = step;
            _text = text;
            _value = value;
        }
        internal override string Text => _text;

        protected override Int32[] GetVal()
        {
            if (_step.IsNegated)
            {
                var N = _value.Length;
                var negate = new Int32[N];
                for (int i = 0; i < N; i++) { negate[i] = -_value[i]; }
                return negate;
            }
            else
                return _value;
        }
    }
}