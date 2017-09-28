using System;

namespace ModelGraphLibrary
{/*
    There are four cases where the values of computeX are unavailable 
    amd dummy values are supplied by the abstract class ValueDefault

    ValueType.None      - a computeX is created and prior to validateion
    ValueType.Invalid   - a computeX has errors in its definition
    ValueType.Circular  - a computeX has a circular reference
    ValueType.Unresolved - a computeX has an unresolved reference
 */
    class ValueNone : ValueDefault
    {
        protected override string IdString => "......";
        internal override ValueType ValueType => ValueType.None;
    }
    class ValueInvalid : ValueDefault
    {
        protected override string IdString => "######";
        internal override ValueType ValueType => ValueType.Invalid;
    }
    class ValueCircular : ValueDefault
    {
        protected override string IdString => "@@@@@@";
        internal override ValueType ValueType => ValueType.Circular;
    }
    class ValueUnresolved : ValueDefault
    {
        protected override string IdString => "??????";
        internal override ValueType ValueType => ValueType.Unresolved;
    }


    abstract class ValueDefault : Value
    {/*
        Base class for the ValueType "None, Invalid, Circular, Unresolved"

        It provides the computeX's default, do no harm, behavior
     */
        internal override int Count => 0;

        internal override void Clear() { }

        internal override string GetDefault() => Chef.InvalidItem;

        internal override string GetValue(Item item) => IdString;

        internal override void GetValue(Item item, out bool value, short index = 0) => value = false;

        internal override void GetValue(Item item, out byte value, short index = 0) => value = 0;

        internal override void GetValue(Item item, out sbyte value, short index = 0) => value = 0;

        internal override void GetValue(Item item, out uint value, short index = 0) => value = 0;

        internal override void GetValue(Item item, out int value, short index = 0) => value = 0;

        internal override void GetValue(Item item, out ushort value, short index = 0) => value = 0;

        internal override void GetValue(Item item, out short value, short index = 0) => value = 0;

        internal override void GetValue(Item item, out ulong value, short index = 0) => value = 0;

        internal override void GetValue(Item item, out long value, short index = 0) => value = 0;

        internal override void GetValue(Item item, out double value, short index = 0) => value = 0;

        internal override void GetValue(Item item, out string value, short index = 0) => value = IdString;

        internal override void GetValue(ComputeX cx, Item item, out bool value, short index = 0) => value = false;

        internal override void GetValue(ComputeX cx, Item item, out byte value, short index = 0) => value = 0;

        internal override void GetValue(ComputeX cx, Item item, out sbyte value, short index = 0) => value = 0;

        internal override void GetValue(ComputeX cx, Item item, out uint value, short index = 0) => value = 0;

        internal override void GetValue(ComputeX cx, Item item, out int value, short index = 0) => value = 0;

        internal override void GetValue(ComputeX cx, Item item, out ushort value, short index = 0) => value = 0;

        internal override void GetValue(ComputeX cx, Item item, out short value, short index = 0) => value = 0;

        internal override void GetValue(ComputeX cx, Item item, out ulong value, short index = 0) => value = 0;

        internal override void GetValue(ComputeX cx, Item item, out long value, short index = 0) => value = 0;

        internal override void GetValue(ComputeX cx, Item item, out double value, short index = 0) => value = 0;

        internal override void GetValue(ComputeX cx, Item item, out string value, short index = 0) => value = IdString;

        internal override bool HasSpecificDefault() => false;

        internal override void Initialize(string defaultValue, int capacity) { }

        internal override bool IsSpecific(Item item) => false;

        internal override bool IsValid(string value) => false;

        internal override void RemoveValue(Item item) { }

        internal override void SetCapacity(int capacity) { }

        internal override void SetDefault(Item[] items) { }

        internal override bool TryGetKeys(out Item[] keys) { keys = null; return false; }

        internal override bool TryGetSerializedValue(Item item, out string value) { value = null; return false; }

        internal override bool TrySetDefault(Item[] items, string value) => false;

        internal override bool TrySetValue(Item item, string value) => false;
    }
}
