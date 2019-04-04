using System;
using System.Text;

namespace ModelGraphSTD
{
    internal class LiteralProperty : EvaluateStep
    {
        private Func<Item> _getItem;

        internal LiteralProperty(ComputeStep step, Property property, Func<Item> getItem)
        {
            Property = property;
            _getItem = getItem;
            _step = step;
        }
        internal Property Property { get; }

        internal override ValType ValType => Property.Value.ValType;
        internal override string Text => (Property is ColumnX col) ? col.Name : ((Property is ComputeX cmp) ? cmp.Name : Property.GetChef().GetIdentity(Property, IdentityStyle.Single));

        internal override bool AsBool()
        {
            if (!Property.Value.GetValue(_getItem(), out bool val))
                val = false;
            return (_step.IsNegated) ? !val : val;
        }
        internal override Int64 AsInt64()
        {
            if (!Property.Value.GetValue(_getItem(), out Int64 val))
                val = 0;
            return (_step.IsNegated) ? ~val : val;
        }
        internal override double AsDouble()
        {
            if (!Property.Value.GetValue(_getItem(), out double val))
                val = 0;
            return (_step.IsNegated) ? -val : val;
        }
        internal override string AsString()
        {
            if (!Property.Value.GetValue(_getItem(), out string val))
                val = string.Empty;
            return val;
        }
        internal override DateTime AsDateTime()
        {
            if (!Property.Value.GetValue(_getItem(), out DateTime val))
                val = default(DateTime);
            return val;
        }

        internal override Int32 AsLength()
        {
            if (!Property.Value.GetLength(_getItem(), out Int32 val))
                val = 0;
            return val;
        }

        internal override bool[] AsBoolArray()
        {
            if (!Property.Value.GetValue(_getItem(), out bool[] val))
                val = new bool[0];
            return val;
        }
        internal override Int64[] AsInt64Array()
        {
            if (!Property.Value.GetValue(_getItem(), out Int64[] val))
                val = new Int64[0];
            return val;
        }
        internal override double[] AsDoubleArray()
        {
            if (!Property.Value.GetValue(_getItem(), out double[] val))
                val = new double[0];
            return val;
        }
        internal override string[] AsStringArray()
        {
            if (!Property.Value.GetValue(_getItem(), out string[] val))
                val = new string[0];
            return val;
        }
        internal override DateTime[] AsDateTimeArray()
        {
            if (!Property.Value.GetValue(_getItem(), out DateTime[] val))
                val = new DateTime[0];
            return val;
        }
    }
}
