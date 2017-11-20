using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraph.Internals
{
    internal abstract partial class Value
    {
        internal FormatType Format;

        static Dictionary<FormatType, Func<bool, string>> _boolFormat = new Dictionary<FormatType, Func<bool, string>>()
        {
            [FormatType.Bool1] = (v) => v ? "True" : "False",
            [FormatType.Bool2] = (v) => v ? "T" : "F",
            [FormatType.Bool3] = (v) => v ? "1" : "0",
        };
        static Dictionary<FormatType, Func<long, string>> _longFormat = new Dictionary<FormatType, Func<long, string>>()
        {
            [FormatType.Int1] = (v) => AugmentString(v.ToString(), 3),
            [FormatType.Int2] = (v) => v.ToString("N"),

            [FormatType.Bin1] = (v) => BinaryString((ulong)v),

            [FormatType.Hex1] = (v) => AugmentString(v.ToString("X"), 2, "0x"),
            [FormatType.Hex2] = (v) => AugmentString(v.ToString("X"), 0, "#"),
            [FormatType.Hex3] = (v) => v.ToString("x"),
        };
        static Dictionary<FormatType, Func<double, string>> _doubleFormat = new Dictionary<FormatType, Func<double, string>>()
        {
            [FormatType.Float1] = (v) => v.ToString("N0"),
            [FormatType.Float2] = (v) => v.ToString("N3"),
            [FormatType.Float2] = (v) => v.ToString("N4"),
            [FormatType.Float2] = (v) => v.ToString("e"),

            [FormatType.Currency1] = (v) => v.ToString("C0"),
            [FormatType.Currency2] = (v) => v.ToString("C2"),
            [FormatType.Currency3] = (v) => v.ToString("C5"),

            [FormatType.Percent1] = (v) => v.ToString("P0"),
            [FormatType.Percent2] = (v) => v.ToString("P1"),
        };
        static internal string ValueFormat(bool v, FormatType ft) => (_boolFormat.TryGetValue(ft, out Func<bool, string> fs)) ? fs(v) : v.ToString();
        static internal string ValueFormat(int v, FormatType ft) => (_longFormat.TryGetValue(ft, out Func<long, string> fs)) ? fs(v) : v.ToString();
        static internal string ValueFormat(long v, FormatType ft) => (_longFormat.TryGetValue(ft, out Func<long, string> fs)) ? fs(v) : v.ToString();
        static internal string ValueFormat(double v, FormatType ft) => (_doubleFormat.TryGetValue(ft, out Func<double, string> fs)) ? fs(v) : v.ToString();
        static internal string ValueFormat(DateTime v, FormatType f) =>  v.ToString();

        #region AugmentString  ================================================
        static string AugmentString(string input, int groupSize, string prefix = null)
        {
            var N = input.Length;
            var sb = new StringBuilder(N + 20);

            if (prefix != null)
                sb.Append(prefix);

            if (groupSize < 1)
            {
                sb.Append(input);
                return sb.ToString();
            }

            var n = N % groupSize; // leading groupSize

            for (int i = 0; i < n; i++)
            {
                sb.Append(input[i]);
            }
            for (int i = n; i < N; i++)
            {
                if (i != 0 && (N - i) % groupSize == 0)
                    sb.Append('_');
                sb.Append(input[i]);
            }
            return sb.ToString();
        }
        #endregion

        #region BinaryString  =================================================
        static string BinaryString(ulong v)
        {
            var sb = new StringBuilder(100);
            sb.Append("0b");

            var b = 0x8000000000000000; // bit mask
            var t = 0xFF00000000000000; // test byte mask
            var n = 64;                 // number of bits

            // determine number of bits and first bit mask
            for (; n > 0; n-=8)
            {
                if ((v & t) != 0) break; // found it

                b = b >> 8;  // try the next byte
                t = t >> 8;
            }

            // generate the binary representation

            for (int i = 0; i < n; i++)
            {
                if (i != 0 && (i % 4) == 0)
                    sb.Append('_');

                sb.Append((v & b) == 0 ? '0' : '1');
                b = b >> 1;
            }

            return sb.ToString();
        }
        #endregion

        #region ArrayFormat  ==================================================
        static string ArrayElementSeperator = ", ";
        static char[] ArrayElementSeperatorSplit = ArrayElementSeperator.ToCharArray();
        static protected string ArrayFormat(Array a, Func<int, string> valueAt)
        {
            var N = (a == null) ? 0 : a.Length;
            if (N == 0) return string.Empty;

            var sb = new StringBuilder(N * 10);
            for (int i = 0; i < N; i++)
            {
                if (i != 0) sb.Append(ArrayElementSeperator);
                sb.Append(valueAt(i));
            }
            return sb.ToString();
        }
        #endregion
    }
}
