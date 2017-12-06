using System.Text;

namespace ModelGraphLibrary
{
    /// <summary>
    /// The default ComputeStep.Evaluate function set by the ComputeStep constructor
    /// </summary>
    internal class LiteralUnresolved : ValueOfString
    {
        internal override ValType ValueType => ValType.IsUnresolved;
        internal override string Text => "????";
        protected override string GetVal() => Text;
    }
}
