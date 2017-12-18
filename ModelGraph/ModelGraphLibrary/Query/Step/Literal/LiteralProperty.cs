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
            throw new NotImplementedException();
        }

        internal override DateTime AsDateTime()
        {
            throw new NotImplementedException();
        }

        internal override double AsDouble()
        {
            throw new NotImplementedException();
        }
        internal override long AsLong()
        {
            throw new NotImplementedException();
        }

        internal override string AsString()
        {
            throw new NotImplementedException();
        }
    }
}
