using System;
using System.Numerics;
using System.Windows;

namespace ModelGraphSTD
{
    internal abstract class Setting<T>
    {
        private Setting<T> _parent;
        private T _value;
        private bool _hasValue;
        protected abstract T DefaultValue { get; }
        protected abstract bool IsValid(T value);

        internal T GetValue() => (_hasValue) ? _value : (_parent != null) ? _parent.GetValue() : DefaultValue;
        internal bool SetValue(T value)
        {
            if (IsValid(value))
            {
                _value = value;
                _hasValue = true;
                return true;
            }
            return false;
        }
        internal void Reset() => _hasValue = false;
    }
}

