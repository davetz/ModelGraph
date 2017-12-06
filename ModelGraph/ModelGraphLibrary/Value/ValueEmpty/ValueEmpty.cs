using System;

namespace ModelGraphLibrary
{
    internal abstract class ValueEmpty : Value
    {
        protected ValType _valueType;
        protected string _idString;
        internal override ValType ValType => _valueType;

        #region Required  =====================================================
        internal override bool GetValue(Item key, out string value) { value = _idString; return true; }
        internal override int Count => 0;
        internal override bool SetValue(Item key, string value) => false;
        internal override void Clear() { }
        internal override void Remove(Item key) { }
        #endregion
    }
}

