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
            return val;
        }
        internal override long AsLong()
        {
            if (!_property.Value.GetValue(_getItem(), out long val))
                val = 0;
            return val;
        }
        internal override double AsDouble()
        {
            if (!_property.Value.GetValue(_getItem(), out double val))
                val = 0;
            return val;
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
    }
}
