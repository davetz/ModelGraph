﻿using System;
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
        internal override ValueType ValueType => _property.ValueType;
        internal override void GetValue(out bool value)
        {
            _property.GetValue(_getItem(), out value);
            if (IsNegated) value = !value;
        }
        internal override void GetValue(out byte value)
        {
            _property.GetValue(_getItem(), out value);
            if (IsNegated) value = (byte)~value;
        }
        internal override void GetValue(out sbyte value)
        {
            _property.GetValue(_getItem(), out value);
            if (IsNegated) value = (sbyte)-value;
        }
        internal override void GetValue(out int value)
        {
            _property.GetValue(_getItem(), out value);
            if (IsNegated) value = -value;
        }
        internal override void GetValue(out uint value)
        {
            _property.GetValue(_getItem(), out value);
            if (IsNegated) value = ~value;
        }
        internal override void GetValue(out long value)
        {
            _property.GetValue(_getItem(), out value);
            if (IsNegated) value = -value;
        }
        internal override void GetValue(out ulong value)
        {
            _property.GetValue(_getItem(), out value);
            if (IsNegated) value = ~value;
        }
        internal override void GetValue(out short value)
        {
            _property.GetValue(_getItem(), out value);
            if (IsNegated) value = (short)-value;
        }
        internal override void GetValue(out ushort value)
        {
            _property.GetValue(_getItem(), out value);
            if (IsNegated) value = (ushort)~value;
        }
        internal override void GetValue(out double value)
        {
            _property.GetValue(_getItem(), _term, out value);
            if (IsNegated) value = -value;
        }
        internal override void GetValue(out string value)
        {
            _property.GetValue(_getItem(), _term, out value);
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
