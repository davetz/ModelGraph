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
            _property.Value.GetValue(_getItem(), out bool value);
            return value;
        }

        internal override DateTime AsDateTime()
        {
            _property.Value.GetValue(_getItem(), out DateTime value);
            return value;
        }

        internal override double AsDouble()
        {
            _property.Value.GetValue(_getItem(), out double value);
            return value;
        }
        internal override long AsLong()
        {
            _property.Value.GetValue(_getItem(), out long value);
            return value;
        }

        internal override string AsString()
        {
            _property.Value.GetValue(_getItem(), out string value);
            return value;
        }
    }
}
