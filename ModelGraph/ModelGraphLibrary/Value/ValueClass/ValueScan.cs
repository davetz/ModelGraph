using System.Text;

namespace ModelGraphLibrary
{
    internal abstract partial class Value
    {/*
        ScanBool - Scan string and try to determine the bool value
        ScanNumber - Scan string and try to determine numeric format type
     */
        static char[] _0BoolText = new char[] { '0' };
        static char[] _1BoolText = new char[] { '1' };
        static char[] _trueBoolText = new char[] { 't', 'r', 'u', 'e' };
        static char[] _falseBoolText = new char[] { 'f', 'a', 'l', 's', 'e' };
        /// <summary>
        /// Scan the callers string and try to determine the bool value
        /// </summary>
        static (bool, bool) ScanBool(string input)
        {
            var N = (input == null) ? 0 : input.Length;
            if (N > 0)
            {
                for (int i = 0; i < N; i++)
                {
                    var c = input[i];

                    if (char.IsWhiteSpace(c))
                        continue;
                    else if (c == '0' && IsEndOfInputOrEndsWith(i, _0BoolText))
                        return (true, false);
                    else if (c == '1' && IsEndOfInputOrEndsWith(i, _1BoolText))
                        return (true, true);
                    else if ((c == 't' || c == 'T') && IsEndOfInputOrEndsWith(i, _trueBoolText))
                        return (true, true);
                    else if ((c == 'f' || c == 'F') && IsEndOfInputOrEndsWith(i, _falseBoolText))
                        return (true, false);
                    else
                        break;
                }
            }
            return (false, false);

            bool IsEndOfInputOrEndsWith(int i, char[] ending)
            {
                var anySpace = false;
                for (int j = i, k = 0; j < N; j++)
                {
                    var t = input[j];
                    if (char.IsWhiteSpace(t))
                        anySpace = true;
                    else
                    {
                        if (anySpace)
                            return false; //can't have embedded whitespace

                        if (char.ToLower(t) != ending[k++])
                            return false; //doesn't have specified ending
                    }
                }
                return true;
            }
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

        static char _isFloatChar1 = '.';  //posible floating point trigger
        static char _isFloatChar2 = ',';  //posible floating point trigger
        /// <summary>
        /// Scan the callers string, removing the fluff, and try to determine format type
        /// </summary>
        static (FormatType, string) ScanInteger(string input)
        {
            var N = (input == null) ? 0 : input.Length;
            if (N > 0)
            {
                var fmt = FormatType.IsInt;
                var sb = new StringBuilder(N);

                for (int i = 0, j = -1; i < N; i++)
                {
                    var c = input[i];

                    if (char.IsWhiteSpace(c) || c == '_')
                        continue; //ignore whitespace and the digit separator
                    else
                    {
                        j++;
                        sb.Append(c);

                        if (c == '#')
                        {
                            if (IsBin(fmt) || IsHex(fmt) || IsFloat(fmt) || j != 0)
                                return (FormatType.FormatError, null);
                            fmt = FormatType.IsHex;
                            sb.Remove(j--, 1); //purge the '#' char
                        }
                        else if (c == 'x' || c == 'X')
                        {
                            if (IsBin(fmt) || IsHex(fmt) || IsFloat(fmt) || j != 1 || input[i - 1] != '0')
                                return (FormatType.FormatError, null);
                            fmt = FormatType.IsHex;
                            sb.Remove(j--, 1); //purge the 'x' char
                            sb.Remove(j--, 1); //purge the '0' char
                        }
                        else if (c == 'b' || c == 'B')
                        {
                            if (IsBin(fmt) || IsHex(fmt) || IsFloat(fmt) || j != 1 || input[i - 1] != '0')
                                return (FormatType.FormatError, null);
                            fmt = FormatType.IsBin;
                            sb.Remove(j--, 1); //purge the 'b' char
                            sb.Remove(j--, 1); //purge the '0' char
                        }
                        else if (c == _isFloatChar1 || c == _isFloatChar2)
                        {
                            if (IsBin(fmt) || IsHex(fmt))
                                return (FormatType.FormatError, null);
                            fmt = FormatType.IsFloat;
                        }
                    }
                }
                return (fmt, sb.ToString());
            }
            return (FormatType.EmptyString, null);

            bool IsBin(FormatType fmt) => (fmt & FormatType.FormatMask) == FormatType.IsBin;
            bool IsHex(FormatType fmt) => (fmt & FormatType.FormatMask) == FormatType.IsHex;
            bool IsFloat(FormatType fmt) => (fmt & FormatType.FormatMask) == FormatType.IsFloat;
        }
    }
}
