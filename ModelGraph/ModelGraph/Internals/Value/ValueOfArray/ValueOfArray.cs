using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraph.Internals
{
    internal abstract class ValueOfArray<T> : ValueOfType<T[]>
    {
        protected bool GetValAt(Item key, out T value, int index)
        {
            if (index >= 0 && GetVal(key, out T[] val) && val != null && index < val.Length)
            {
                value = val[index];
                return true;
            }
            value = default(T);
            return false;
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

        protected bool SetValAt(Item key, T value, int index)
        {
            if (index >= 0 && GetVal(key, out T[] val) && val != null && index < val.Length)
            {
                val[index] = value;
                return true;
            }
            return false;
        }
    }
}
