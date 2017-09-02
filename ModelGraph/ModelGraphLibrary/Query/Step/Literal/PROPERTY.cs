using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        internal override void GetValue(out bool value) { _property.GetValue(_getItem(), out value); }
        internal override void GetValue(out byte value) { _property.GetValue(_getItem(), out value); }
        internal override void GetValue(out int value) { _property.GetValue(_getItem(), out value); }
        internal override void GetValue(out long value) { _property.GetValue(_getItem(), out value); }
        internal override void GetValue(out short value) { _property.GetValue(_getItem(), out value); }
        internal override void GetValue(out double value) { _property.GetValue(_getItem(), _term, out value); }
        internal override void GetValue(out string value) { _property.GetValue(_getItem(), _term, out value); }
        #endregion

    }
}
