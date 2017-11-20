using System;

namespace ModelGraph.Internals
{
    internal abstract partial class Value
    {
        static protected bool ValueArray(Array a, out bool[] value, Func<int, (bool, bool)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new bool[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, bool v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out byte[] value, Func<int, (bool, byte)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new byte[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, byte v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out sbyte[] value, Func<int, (bool, sbyte)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new sbyte[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, sbyte v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out short[] value, Func<int, (bool, short)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new short[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, short v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out ushort[] value, Func<int, (bool, ushort)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new ushort[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, ushort v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out int[] value, Func<int, (bool, int)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new int[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, int v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out uint[] value, Func<int, (bool, uint)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new uint[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, uint v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out long[] value, Func<int, (bool, long)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new long[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, long v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out ulong[] value, Func<int, (bool, ulong)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new ulong[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, ulong v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out float[] value, Func<int, (bool, float)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new float[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, float v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out double[] value, Func<int, (bool, double)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new double[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, double v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out decimal[] value, Func<int, (bool, decimal)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new decimal[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, decimal v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out DateTime[] value, Func<int, (bool, DateTime)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new DateTime[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, DateTime v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out char[] value, Func<int, (bool, char)> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new char[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, char v) = valueAt(i);
                    if (!ok) return false; // abort
                    value[i] = v;
                }
                return true;
            }
            value = null;
            return false;
        }
        static protected bool ValueArray(Array a, out string[] value, Func<int, string> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N > 0)
            {
                value = new string[N];
                for (int i = 0; i < N; i++)
                {
                    value[i] = valueAt(i);
                }
                return true;
            }
            value = null;
            return false;
        }
    }
}
