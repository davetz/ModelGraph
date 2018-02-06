using System.Text;

namespace ModelGraphSTD
{
    internal class LiteralStringArray : ValueOfStringArray
    {
        private string[] _value;
        internal LiteralStringArray(ComputeStep step, string[] value)
        {
            _step = step;
            _value = value;
        }
        internal override string Text => GetText();
        string GetText()
        {
            var sb = new StringBuilder(128);
            var N = _value.Length;
            for (int i = 0; i < N; i++)
            {
                if (sb.Length > 0) sb.Append(", ");
                sb.Append($"\"{ _value[i]}\"");
            }
            return sb.ToString();
        }
        protected override string[] GetVal() => _value;
    }
}