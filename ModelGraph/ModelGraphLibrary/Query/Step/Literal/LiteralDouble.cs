using System.Text;

namespace ModelGraphLibrary
{
    internal class LiteralDouble : ValueOfDouble
    {
        private string _text;
        private double _value;

        internal LiteralDouble(ComputeStep step, double value, string text)
        {
            _step = step;
            _text = text;
            _value = value;
        }
        internal override string Text => _text;

    protected override double GetVal() => _step.IsNegated ? -_value : _value;
    }
}