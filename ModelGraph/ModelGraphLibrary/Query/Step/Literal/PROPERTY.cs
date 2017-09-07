using System;
using System.Text;

namespace ModelGraphLibrary
{/*
 */
    internal class PROPERTY : Step
    {
        private Property _property;
        private Func<Item> _getItem;
        private NumericTerm _term;

        internal PROPERTY(Property property, NumericTerm term, Func<Item> getItem)
        {
            _property = property;
            _getItem = getItem;
            _term = term;
        }

        #region Methods  ======================================================
        internal Property Property => _property;
        internal override NativeType NativeType => _property.NativeType;
        internal override void GetValue(out bool value) { _property.GetValue(_getItem(), out value); }
        internal override void GetValue(out byte value) { _property.GetValue(_getItem(), out value); }
        internal override void GetValue(out int value) { _property.GetValue(_getItem(), out value); }
        internal override void GetValue(out long value) { _property.GetValue(_getItem(), out value); }
        internal override void GetValue(out short value) { _property.GetValue(_getItem(), out value); }
        internal override void GetValue(out double value) { _property.GetValue(_getItem(), _term, out value); }
        internal override void GetValue(out string value) { _property.GetValue(_getItem(), _term, out value); }
        internal override IStepValue GetValue()
        {
            var nativeType = _property.NativeType;
            switch (nativeType)
            {
                //case NativeType.None: { if (_property is ComputeX cx) return cx.GetStepValue(_getItem()); else return new InvalidStep(); }
                case NativeType.Bool: { _property.GetValue(_getItem(), out bool v); return new BoolValue(v); }
                case NativeType.Byte: { _property.GetValue(_getItem(), out byte v); return new ByteValue(v); }
                case NativeType.Int16: { _property.GetValue(_getItem(), out short v); return new Int16Value(v); }
                case NativeType.Int32: { _property.GetValue(_getItem(), out int v); return new Int32Value(v); }
                case NativeType.Int64: { _property.GetValue(_getItem(), out long v); return new Int64Value(v); }
                case NativeType.Double: { _property.GetValue(_getItem(), _term, out double v); return new DoubleValue(v); }
                case NativeType.String: { _property.GetValue(_getItem(), _term, out string v); return new StringValue(v); }
            }
            return new InvalidStep();
        }
        internal override void GetText(StringBuilder sb)
        {
            GetPrefix(sb);
            if (_property is ColumnX col)
                sb.Append(col.Name);
            else if (_property is ComputeX cmp)
                sb.Append(cmp.Name);
            else
                sb.Append(_property.GetChef().GetIdentity(_property, IdentityStyle.Single));
            GetSufix(sb);
        }
        #endregion
    }
}
