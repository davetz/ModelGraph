
using System.Collections.Generic;

namespace ModelGraph.Internals
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

        internal IEnumerable<Item> GetKeys() => (Count == 0) ? null : _values.Keys;
        internal IEnumerable<T> GetValues() => (Count == 0) ? null : _values.Values;

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
    }
}
