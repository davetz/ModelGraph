using System.Text;

namespace ModelGraphLibrary
{
    internal class LiteralBool : ValueOfBool
    {
        private string _text;
        private bool _value;

        internal LiteralBool(ComputeStep step, bool value, string text)
        {
            _step = step;
            _text = text;
            _value = value;
        }
        internal override string Text => _text;

        protected override bool GetVal() => _step.IsNegated ? !_value : _value;
    }
}
