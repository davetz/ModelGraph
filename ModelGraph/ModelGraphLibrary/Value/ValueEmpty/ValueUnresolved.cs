
namespace ModelGraph.Internals
{
    internal class ValueUnresolved : ValueEmpty
    {
        internal ValueUnresolved()
        {
            _idString = "??????";
            _valueType = ValType.IsUnresolved;
        }
    }
}
