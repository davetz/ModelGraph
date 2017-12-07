﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelGraphSTD
{
    internal class ValueDictionary<T> : IValueStore<T>
    {
        Dictionary<Item, T> _values;
        protected T _default;

        internal ValueDictionary(int capacity, T defaultValue)
        {
            _default = defaultValue;
            if (capacity > 0) _values = new Dictionary<Item, T>(capacity);
        }

        public int Count => (_values == null) ? 0 : _values.Count;

        public void Clear()
        {
            if (_values != null)
                _values.Clear();
        }
        public void Remove(Item key)
        {
            if (_values != null)
                _values.Remove(key);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

        internal T DefaultValue => _default;
        internal Item[] GetKeys() => (Count == 0) ? null : _values.Keys.ToArray();
        internal T[] GetValues() => (Count == 0) ? null : _values.Values.ToArray();

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

        public bool GetVal(Item key, out T val)
        {
            if (_values == null)
            {
                val = _default;
                return false;
            }
            return _values.TryGetValue(key, out val);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

        public bool SetVal(Item key, T value)
        {
            if (_values == null) _values = new Dictionary<Item, T>();

            if (_default == null)
            {
                if (value != null)
                    _values[key] = value;
            }
            else
            {
                if (value != null && value.Equals(_default))
                    _values.Remove(key);
                else
                    _values[key] = value;
            }
            return true;
        }

        // LoadValue() should only be used by RepositoryRead
        public void LoadValue(Item key, T value) => _values[key] = value;
    }
}
