using System.Text;

namespace ModelGraphSTD
{
    internal class LiteralString : ValueOfString
    {
        private string _value;
        internal LiteralString(ComputeStep step, string value)
        {
            _step = step;
            _value = value;
        }
        internal override string Text => $"\"{_value}\"";

        protected override string GetVal() => _value;
    }
}