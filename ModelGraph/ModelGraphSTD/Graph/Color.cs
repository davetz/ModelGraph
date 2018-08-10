using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    internal class Color
    {
        public List<(byte A, byte R, byte G, byte B)> ARGBList = new List<(byte A, byte R, byte G, byte B)>() { _invalidColor };

        internal void Reset()
        {
            ARGBList.Clear();
            ARGBList.Add(_invalidColor);
        }
        internal void BuildARGBList(string argbString)
        {
            if (IsValid(argbString))
            {
                var j = 0;
                var N = ARGBList.Count;
                var a = GetARGB(argbString);

                for (int i = 0; i < N; i++)
                {
                    var v = Compare(ARGBList[i], a);
                    if (v == 0) return;
                    if (v < 0) j = i;
                }
                if (++j < N)
                    ARGBList.Insert(j, a);
                else
                    ARGBList.Add(a);
            }
        }

        internal byte ColorIndex(string argbString)
        {
            if (IsInvalid(argbString)) return 0;

            var N = ARGBList.Count;
            var a = GetARGB(argbString);

            for (int i = 0; i < N; i++)
            {
                var v = Compare(ARGBList[i], a);
                if (v == 0) return (byte)i;
            }
            ARGBList.Add(a);
            return (byte)N;
        }

        #region Has/GetARGB  ==================================================
        internal static bool HasARGB(string argbStr)
        {
            if (IsInvalid(argbStr)) return false;
            return (Compare(GetARGB(argbStr), _invalidColor) == 0);
        }
        internal static (byte, byte, byte, byte) GetARGB(string argbStr)
        {
            if (IsInvalid(argbStr)) return _invalidColor;
            var argb = _invalidColor; // default color when there is a bad color string

            var ca = argbStr.ToLower().ToCharArray();
            if (ca[0] != '#') return _invalidColor;

            var N = _argbLength;
            int[] va = new int[N];
            for (int j = 1; j < N; j++)
            {
                va[j] = _hexValues.IndexOf(ca[j]);
                if (va[j] < 0) return _invalidColor;
            }
            return ((byte)((va[1] << 4) | va[2]), (byte)((va[3] << 4) | va[4]), (byte)((va[5] << 4) | va[6]), (byte)((va[7] << 4) | va[8]));
        }
        private static bool AreNotSame((byte, byte, byte, byte) a, (byte, byte, byte, byte) b) => Compare(a, b) != 0;
        private static int Compare((byte, byte, byte, byte) a, (byte, byte, byte, byte) b)
        {
            if (a.Item1 < b.Item1) return -1;
            if (a.Item1 > b.Item1) return 1;

            if (a.Item2 < b.Item2) return -1;
            if (a.Item2 > b.Item2) return 1;

            if (a.Item3 < b.Item3) return -1;
            if (a.Item3 > b.Item3) return 1;

            if (a.Item4 < b.Item4) return -1;
            if (a.Item4 > b.Item4) return 1;

            return 0;
        }
        static bool IsValid(string argbStr) => !IsInvalid(argbStr);
        static bool IsInvalid(string argbStr) => (string.IsNullOrWhiteSpace(argbStr) || argbStr.Length != _argbLength);
        static readonly (byte, byte, byte, byte) _invalidColor = (0x88, 0x87, 0x86, 0x85);
        static readonly string _hexValues = "0123456789abcdef";
        const int _argbLength = 9;
        #endregion
    }
}
