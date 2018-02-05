
using System.Linq;

namespace ModelGraphSTD
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

        internal int Length(Item key) => (GetVal(key, out T[] v)) ? v.Length : 0;
        internal T[] Take(Item key, int c) => (GetVal(key, out T[] v) && c > 0) ? v.Take((v.Length < c) ? v.Length : c).ToArray() : new T[0];
        internal T[] SortAscending(Item key) =>  GetVal(key, out T[] v) ? v.OrderBy((s) => s).ToArray() : new T[0];
        internal T[] SortDescending(Item key) => GetVal(key, out T[] v) ? v.OrderByDescending((s) => s).ToArray() : new T[0];
        internal T[] SortAscendingTake(Item key, int c) => (GetVal(key, out T[] v) && c > 0) ? v.OrderBy((s) => s).Take((v.Length < c) ? v.Length : c).ToArray() : new T[0];
        internal T[] SortDescendingTake(Item key, int c) => (GetVal(key, out T[] v) && c > 0) ? v.OrderByDescending((s) => s).Take((v.Length < c) ? v.Length : c).ToArray() : new T[0];
    }
}
