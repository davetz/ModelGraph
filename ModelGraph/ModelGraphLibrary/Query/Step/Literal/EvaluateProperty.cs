using System;
using System.Text;

namespace ModelGraphLibrary
{/*
 */
    internal class EvaluateProperty : EvaluateBase
    {
        private Property _property;
        private Func<Item> _getItem;
        private int _index;

        internal EvaluateProperty(ComputeStep step, Property property, int index, Func<Item> getItem)
            :base(step)
        {
            _property = property;
            _getItem = getItem;
            _index = index;
        }
        internal Property Property => _property;

        internal override ValueType ValueType => _property.Values.ValueType;
        internal override string Text => (_property is ColumnX col) ? col.Name : ((_property is ComputeX cmp) ? cmp.Name : _property.GetChef().GetIdentity(_property, IdentityStyle.Single));

        #region EvaluateBase-v1 (Specialized)  ================================
        internal override void GetValue(ComputeStep step, out bool value)
        {
            _property.Values.GetValue(_getItem(), out value);
            if (step.IsNegated) value = !value;
        }
        internal override void GetValue(ComputeStep step, out byte value)
        {
            _property.Values.GetValue(_getItem(), out value);
            if (step.IsNegated) value = (byte)~value;
        }
        internal override void GetValue(ComputeStep step, out sbyte value)
        {
            _property.Values.GetValue(_getItem(), out value);
            if (step.IsNegated) value = (sbyte)-value;
        }
        internal override void GetValue(ComputeStep step, out int value)
        {
            _property.Values.GetValue(_getItem(), out value);
            if (step.IsNegated) value = -value;
        }
        internal override void GetValue(ComputeStep step, out uint value)
        {
            _property.Values.GetValue(_getItem(), out value);
            if (step.IsNegated) value = ~value;
        }
        internal override void GetValue(ComputeStep step, out long value)
        {
            _property.Values.GetValue(_getItem(), out value);
            if (step.IsNegated) value = -value;
        }
        internal override void GetValue(ComputeStep step, out ulong value)
        {
            _property.Values.GetValue(_getItem(), out value);
            if (step.IsNegated) value = ~value;
        }
        internal override void GetValue(ComputeStep step, out short value)
        {
            _property.Values.GetValue(_getItem(), out value);
            if (step.IsNegated) value = (short)-value;
        }
        internal override void GetValue(ComputeStep step, out ushort value)
        {
            _property.Values.GetValue(_getItem(), out value);
            if (step.IsNegated) value = (ushort)~value;
        }
        internal override void GetValue(ComputeStep step, out double value)
        {
            _property.Values.GetValue(_getItem(), out value);
            if (step.IsNegated) value = -value;
        }
        internal override void GetValue(ComputeStep step, out string value)
        {
            _property.Values.GetValue(_getItem(), out value);
        }
        #endregion
    }
}
