using System;
using System.Text;

namespace ModelGraphSTD
{/*
 */
    internal class LiteralProperty : EvaluateStep
    {
        private Property _property;
        private Func<Item> _getItem;
        private int _index;

        internal LiteralProperty(ComputeStep step, Property property, int index, Func<Item> getItem)
        {
            _property = property;
            _getItem = getItem;
            _index = index;
            _step = step;
        }
        internal Property Property => _property;

        internal override ValType ValType => _property.Value.ValType;
        internal override string Text => (_property is ColumnX col) ? col.Name : ((_property is ComputeX cmp) ? cmp.Name : _property.GetChef().GetIdentity(_property, IdentityStyle.Single));

        internal override bool AsBool()
        {
            if (!_property.Value.GetValue(_getItem(), out bool val))
                val = false;
            return (_step.IsNegated) ? !val : val;
        }
        internal override Int64 AsInt64()
        {
            if (!_property.Value.GetValue(_getItem(), out Int64 val))
                val = 0;
            return (_step.IsNegated) ? ~val : val;
        }
        internal override double AsDouble()
        {
            if (!_property.Value.GetValue(_getItem(), out double val))
                val = 0;
            return (_step.IsNegated) ? -val : val;
        }
        internal override string AsString()
        {
            if (!_property.Value.GetValue(_getItem(), out string val))
                val = string.Empty;
            return val;
        }
        internal override DateTime AsDateTime()
        {
            if (!_property.Value.GetValue(_getItem(), out DateTime val))
                val = default(DateTime);
            return val;
        }

        internal override Int32 AsLength()
        {
            if (!_property.Value.GetLength(_getItem(), out Int32 val))
                val = 0;
            return val;
        }

        internal override bool[] AsBoolArray()
        {
            if (!_property.Value.GetValue(_getItem(), out bool[] val))
                val = new bool[0];
            return val;
        }
        internal override Int64[] AsInt64Array()
        {
            if (!_property.Value.GetValue(_getItem(), out Int64[] val))
                val = new Int64[0];
            return val;
        }
        internal override double[] AsDoubleArray()
        {
            if (!_property.Value.GetValue(_getItem(), out double[] val))
                val = new double[0];
            return val;
        }
        internal override string[] AsStringArray()
        {
            if (!_property.Value.GetValue(_getItem(), out string[] val))
                val = new string[0];
            return val;
        }
        internal override DateTime[] AsDateTimeArray()
        {
            if (!_property.Value.GetValue(_getItem(), out DateTime[] val))
                val = new DateTime[0];
            return val;
        }
    }
}
