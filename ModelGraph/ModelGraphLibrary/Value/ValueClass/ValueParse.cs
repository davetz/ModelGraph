using System;

namespace ModelGraph.Internals
{
    internal abstract partial class Value
    {/*
        Convert string to value, with a liberal interpretation of format
     */
        static System.Globalization.NumberStyles _numberStyle = System.Globalization.NumberStyles.Any;
        static System.Globalization.DateTimeStyles _dateStyle = System.Globalization.DateTimeStyles.AllowWhiteSpaces;

        static protected (bool ok, bool val) BoolParse(string str) => ScanBool(str);

        static protected (bool ok, byte val) ByteParse(string str)
        {
            (bool ok, bool isD, long v, double d) = ParseNumber(str);
            return (!ok || v < byte.MinValue || v > byte.MaxValue) ? (false, (byte)0) : (true, (byte)v);
        }
        static protected (bool ok, sbyte val) SByteParse(string str)
        {
            (bool ok, bool isD, long v, double d) = ParseNumber(str);
            return (!ok || v < sbyte.MinValue || v > sbyte.MaxValue) ? (false, (sbyte)0) : (true, (sbyte)v);
        }
        static protected (bool ok, short val) Int16Parse(string str)
        {
            (bool ok, bool isD, long v, double d) = ParseNumber(str);
            return (!ok || v < short.MinValue || v > short.MaxValue) ? (false, (short)0) : (true, (short)v);
        }
        static protected (bool ok, ushort val) UInt16Parse(string str)
        {
            (bool ok, bool isD, long v, double d) = ParseNumber(str);
            return (!ok || v < ushort.MinValue || v > ushort.MaxValue) ? (false, (ushort)0) : (true, (ushort)v);
        }
        static protected (bool ok, int val) Int32Parse(string str)
        {
            (bool ok, bool isD, long v, double d) = ParseNumber(str);
            return (!ok || v < int.MinValue || v > int.MaxValue) ? (false, 0) : (true, (int)v);
        }
        static protected (bool ok, uint val) UInt32Parse(string str)
        {
            (bool ok, bool isD, long v, double d) = ParseNumber(str);
            return (!ok || v < uint.MinValue || v > uint.MaxValue) ? (false, 0) : (true, (uint)v);
        }
        static protected (bool ok, long val) Int64Parse(string str)
        {
            (bool ok, bool isD, long v, double d) = ParseNumber(str);
            return (!ok) ? (false, 0) : (true, v);
        }
        static protected (bool ok, ulong val) UInt64Parse(string str)
        {
            (bool ok, bool isD, long v, double d) = ParseNumber(str);
            return (!ok) ? (false, 0) : (true, (ulong)v);
        }
        static protected (bool ok, float val) SingleParse(string str) => float.TryParse(str, _numberStyle, null, out float v) ? (true, v) : (false, 0);
        static protected (bool ok, double val) DoubleParse(string str) => double.TryParse(str, _numberStyle, null, out double v) ? (true, v) : (false, 0);
        static protected (bool ok, decimal val) DecimalParse(string str) => decimal.TryParse(str, _numberStyle, null, out decimal v) ? (true, v) : (false, 0);

        static protected (bool ok, DateTime val) DateTimeParse(string str) => DateTime.TryParse(str, null, _dateStyle, out DateTime v) ? (true, v) : (false, default(DateTime));

        static internal (bool ok, bool isDouble, long v, double d) ParseNumber(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return (false, false, 0, 0);

            (FormatType f, string s) = ScanInteger(str);
            long v = 0;

            switch (f)
            {
                case FormatType.IsInt:
                    return long.TryParse(s, _numberStyle, null, out v) ? (true, false, v, 0) : (false, false, 0, 0);

                case FormatType.IsBin:
                    foreach (var c in s)
                    {
                        v = v << 1;
                        if (c == '1')
                            v = v + 1;
                        else if (c != '0')
                            return (false, false, 0, 0);
                    }
                    return (true, false, v, 0);

                case FormatType.IsHex:
                    foreach (var c in s)
                    {
                        v = v << 4;
                        var n = _hexString.IndexOf(char.ToLower(c));
                        if (n < 0)
                            return (false, false, 0, 0);
                        v = v + n;
                    }
                    return (true, false, v, 0);

                case FormatType.IsFloat:
                    return double.TryParse(s, _numberStyle, null, out double d) ? (true, true, 0, d) : (false, false, 0, 0);
            }
            return (false, false, 0, 0);
        }
        static string _hexString = "0123456789abcdef";

        #region ArrayParse  ===================================================
        static protected (bool ok, bool[] val) ArrayParse(string str, Func<string, (bool, bool)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new bool[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, bool v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, byte[] val) ArrayParse(string str, Func<string, (bool, byte)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new byte[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, byte v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, sbyte[] val) ArrayParse(string str, Func<string, (bool, sbyte)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new sbyte[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, sbyte v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, short[] val) ArrayParse(string str, Func<string, (bool, short)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new short[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, short v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, ushort[] val) ArrayParse(string str, Func<string, (bool, ushort)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new ushort[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, ushort v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, int[] val) ArrayParse(string str, Func<string, (bool, int)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new int[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, int v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, uint[] val) ArrayParse(string str, Func<string, (bool, uint)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new uint[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, uint v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, long[] val) ArrayParse(string str, Func<string, (bool, long)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new long[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, long v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, ulong[] val) ArrayParse(string str, Func<string, (bool, ulong)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new ulong[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, ulong v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, float[] val) ArrayParse(string str, Func<string, (bool, float)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new float[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, float v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, double[] val) ArrayParse(string str, Func<string, (bool, double)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new double[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, double v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, decimal[] val) ArrayParse(string str, Func<string, (bool, decimal)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new decimal[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, decimal v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, DateTime[] val) ArrayParse(string str, Func<string, (bool, DateTime)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new DateTime[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, DateTime v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, char[] val) ArrayParse(string str, Func<string, (bool, char)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new char[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, char v) = parse(part[i]);
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        static protected (bool ok, string[] val) ArrayParse(string str, Func<string, (bool, string)> parse)
        {
            var part = str.Split(ArrayElementSeperatorSplit, StringSplitOptions.RemoveEmptyEntries);
            var N = (part == null) ? 0 : part.Length;

            if (N > 0)
            {
                var a = new string[N];
                for (int i = 0; i < N; i++)
                {
                    (bool ok, string v) = parse(part[i]); //allow for posibility of interpretation and substitution
                    if (!ok) return (false, null);
                    a[i] = v;
                }
                return (true, a);
            }
            return (false, null);
        }
        #endregion
    }
}
