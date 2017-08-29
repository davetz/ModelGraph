using System;
using System.Text;

namespace ModelGraphLibrary
{/*
    Value is the abstract base class for ValuesOf<T>.
    The enum ValueType list all possible types of T. The actual values
    of each type are maintained in a specialized instance, for example,
    ValuesOfBool or ValuesOfDouble. Each table column references exactly
    one dedicated instance of ValuesOf<T>. When a user changes a columns 
    valueType a new instance of that type is created and (if posisible) it 
    is populated with converted values from the previous ValueOf<T> instance.    
 */
    public abstract class Value
    {
        #region Methods  ======================================================
        internal abstract int Count { get; }
        internal abstract ValueType ValueType { get; }
        internal abstract NativeType PreferredType { get; }
        internal abstract string GetValue(Item item);
        internal abstract string GetDefault();
        internal abstract void RemoveValue(Item item);
        internal abstract bool TryGetKeys(out Item[] keys);
        internal abstract bool IsValid(string value);
        internal abstract bool TrySetValue(Item item, string value);
        internal abstract void SetCapacity(int capacity);
        internal abstract void SetDefault(Item[] items);
        internal abstract void Initialize(string defaultValue, int capacity);
        internal abstract bool TrySetDefault(Item[] items, string value);
        internal abstract bool IsSpecific(Item item);
        internal abstract bool TryGetSerializedValue(Item item, out string value);
        internal abstract bool HasSpecificDefault();
        internal abstract void Clear();

        // get the computed value types
        internal abstract void GetValue(Item item, out bool value);
        internal abstract void GetValue(Item item, out byte value);
        internal abstract void GetValue(Item item, out int value);
        internal abstract void GetValue(Item item, out short value);
        internal abstract void GetValue(Item item, out long value);
        internal abstract void GetValue(Item item, out double value);
        internal abstract void GetValue(Item item, out string value);

        #endregion

        #region Create  =======================================================
        internal static Value Create(ValueType type)
        {
            var index = (int)type;
            if (index < 0 || index > (int)ValueType.MaximumType)
                throw new ArgumentException("Values.Create(ValueType type) : Invalid type value");
            return _valueTypes[index]();
        }
        private static Func<Value>[] _valueTypes =
        {
            () => { return new ValuesOfBool(); },   // 0
            () => { return new ValuesOfChar(); },   // 1
            () => { return new ValuesOfByte(); },   // 2
            () => { return new ValuesOfSByte(); },  // 3
            () => { return new ValuesOfInt16(); },  // 4
            () => { return new ValuesOfInt32(); },  // 5
            () => { return new ValuesOfInt64(); },  // 6
            () => { return new ValuesOfUInt16(); }, // 7
            () => { return new ValuesOfUInt32(); }, // 8
            () => { return new ValuesOfUInt64(); }, // 9
            () => { return new ValuesOfSingle(); },   // 10
            () => { return new ValuesOfDouble(); },   // 11
            () => { return new ValuesOfDecimal(); },  // 12
            () => { return new ValuesOfGuid(); },     // 13
            () => { return new ValuesOfDateTime(); }, // 14
            () => { return new ValuesOfTimeSpan(); }, // 15
            () => { return new ValuesOfString(); },   // 16
            () => { return new ValuesOfCharArray(); },  // 17
            () => { return new ValuesOfHexArray(); },    // 18
            () => { return new ValuesOfByteArray(); },    // 19
            () => { return new ValuesOfSByteArray(); },   // 20
            () => { return new ValuesOfInt16Array(); },   // 21
            () => { return new ValuesOfInt32Array(); },   // 22
            () => { return new ValuesOfInt64Array(); },   // 23
            () => { return new ValuesOfUInt16Array(); },  // 24
            () => { return new ValuesOfUInt32Array(); },  // 25
            () => { return new ValuesOfUInt64Array(); },  // 26
            () => { return new ValuesOfSingleArray(); },  // 27
            () => { return new ValuesOfDoubleArray(); },  // 28
            () => { return new ValuesOfDecimalArray(); }, // 29
        };
        #endregion

        #region Convert  ======================================================
        #region TryParse  =====================================================
        internal static bool TryParse(string input, out bool val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = false; return true; }
            int n;
            if (int.TryParse(input, out n)) { val = (n == 0) ? false : true; return true; }

            return bool.TryParse(input, out val);
        }
        internal static bool TryParse(string input, out char val) { return char.TryParse(input, out val); }
        internal static bool TryParse(string input, out byte val) { return byte.TryParse(input, out val); }
        internal static bool TryParse(string input, out sbyte val) { return sbyte.TryParse(input, out val); }
        internal static bool TryParse(string input, out short val) { return short.TryParse(input, out val); }
        internal static bool TryParse(string input, out int val) { return int.TryParse(input, out val); }
        internal static bool TryParse(string input, out long val) { return long.TryParse(input, out val); }
        internal static bool TryParse(string input, out ushort val) { return ushort.TryParse(input, out val); }
        internal static bool TryParse(string input, out uint val) { return uint.TryParse(input, out val); }
        internal static bool TryParse(string input, out ulong val) { return ulong.TryParse(input, out val); }
        internal static bool TryParse(string input, out float val) { return float.TryParse(input, out val); }
        internal static bool TryParse(string input, out double val) { return double.TryParse(input, out val); }
        internal static bool TryParse(string input, out decimal val) { return decimal.TryParse(input, out val); }
        internal static bool TryParse(string input, out Guid val) { return Guid.TryParse(input, out val); }
        internal static bool TryParse(string input, out DateTime val) { return DateTime.TryParse(input, out val); }
        internal static bool TryParse(string input, out TimeSpan val) { return TimeSpan.TryParse(input, out val); }
        internal static bool TryParse(string input, out string val) { val = input; return true; }


        #region TryParseArray  ================================================
        internal static bool TryParse(string input, out char[] val)
        {
            if (string.IsNullOrWhiteSpace(input))
                val = null;
            else
                val = input.ToCharArray();

            return true;
        }
        internal static bool TryParseHex(string input, out byte[] val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = null; return true; }

            var str = input.ToLower().ToCharArray();
            var len = (str.Length + 1) / 2;
            val = new byte[len];
            int j, k, n;
            for (int i = 0; i < str.Length; i += 1)
            {
                n = "0123456789abcdef".IndexOf(str[i]);
                if (n < 0) { val = null; return false; }
                j = i / 2;
                k = i % 2;
                val[j] = (byte)((k == 0) ? (n << 4) : (val[j] | n));
            }
            return true;
        }
        static char[] commaSplit = ",".ToCharArray();

        internal static bool TryParse(string input, out byte[] val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = null; return true; }

            var str = input.Split(commaSplit);
            var len = str.Length;
            val = new byte[len];

            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrWhiteSpace(str[i]))
                {
                    byte v;
                    if (!byte.TryParse(str[i], out v)) { val = null; return false; }
                    val[i] = v;
                }
            }
            return true;
        }

        internal static bool TryParse(string input, out sbyte[] val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = null; return true; }

            var str = input.Split(commaSplit);
            var len = str.Length;
            val = new sbyte[len];

            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrWhiteSpace(str[i]))
                {
                    sbyte v;
                    if (!sbyte.TryParse(str[i], out v)) { val = null; return false; }
                    val[i] = v;
                }
            }
            return true;
        }

        internal static bool TryParse(string input, out short[] val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = null; return true; }

            var str = input.Split(commaSplit);
            var len = str.Length;
            val = new short[len];

            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrWhiteSpace(str[i]))
                {
                    short v;
                    if (!short.TryParse(str[i], out v)) { val = null; return false; }
                    val[i] = v;
                }
            }
            return true;
        }

        internal static bool TryParse(string input, out int[] val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = null; return true; }

            var str = input.Split(commaSplit);
            var len = str.Length;
            val = new int[len];

            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrWhiteSpace(str[i]))
                {
                    int v;
                    if (!int.TryParse(str[i], out v)) { val = null; return false; }
                    val[i] = v;
                }
            }
            return true;
        }

        internal static bool TryParse(string input, out long[] val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = null; return true; }

            var str = input.Split(commaSplit);
            var len = str.Length;
            val = new long[len];

            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrWhiteSpace(str[i]))
                {
                    long v;
                    if (!long.TryParse(str[i], out v)) { val = null; return false; }
                    val[i] = v;
                }
            }
            return true;
        }

        internal static bool TryParse(string input, out ushort[] val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = null; return true; }

            var str = input.Split(commaSplit);
            var len = str.Length;
            val = new ushort[len];

            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrWhiteSpace(str[i]))
                {
                    ushort v;
                    if (!ushort.TryParse(str[i], out v)) { val = null; return false; }
                    val[i] = v;
                }
            }
            return true;
        }

        internal static bool TryParse(string input, out uint[] val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = null; return true; }

            var str = input.Split(commaSplit);
            var len = str.Length;
            val = new uint[len];

            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrWhiteSpace(str[i]))
                {
                    uint v;
                    if (!uint.TryParse(str[i], out v)) { val = null; return false; }
                    val[i] = v;
                }
            }
            return true;
        }

        internal static bool TryParse(string input, out ulong[] val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = null; return true; }

            var str = input.Split(commaSplit);
            var len = str.Length;
            val = new ulong[len];

            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrWhiteSpace(str[i]))
                {
                    ulong v;
                    if (!ulong.TryParse(str[i], out v)) { val = null; return false; }
                    val[i] = v;
                }
            }
            return true;
        }

        internal static bool TryParse(string input, out float[] val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = null; return true; }

            var str = input.Split(commaSplit);
            var len = str.Length;
            val = new float[len];

            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrWhiteSpace(str[i]))
                {
                    float v;
                    if (!float.TryParse(str[i], out v)) { val = null; return false; }
                    val[i] = v;
                }
            }
            return true;
        }

        internal static bool TryParse(string input, out double[] val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = null; return true; }

            var str = input.Split(commaSplit);
            var len = str.Length;
            val = new double[len];

            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrWhiteSpace(str[i]))
                {
                    double v;
                    if (!double.TryParse(str[i], out v)) { val = null; return false; }
                    val[i] = v;
                }
            }
            return true;
        }

        internal static bool TryParse(string input, out decimal[] val)
        {
            if (string.IsNullOrWhiteSpace(input)) { val = null; return true; }

            var str = input.Split(commaSplit);
            var len = str.Length;
            val = new decimal[len];

            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrWhiteSpace(str[i]))
                {
                    decimal v;
                    if (!decimal.TryParse(str[i], out v)) { val = null; return false; }
                    val[i] = v;
                }
            }
            return true;
        }
        #endregion
        #endregion

        #region ToBool  =======================================================
        internal static bool ToBool(bool val) { return val; }
        internal static bool ToBool(char val) { return (val == '0') ? false : true; }
        internal static bool ToBool(byte val) { return (val == 0) ? false : true; }
        internal static bool ToBool(sbyte val) { return (val == 0) ? false : true; }
        internal static bool ToBool(short val) { return (val == 0) ? false : true; }
        internal static bool ToBool(int val) { return (val == 0) ? false : true; }
        internal static bool ToBool(long val) { return (val == 0) ? false : true; }
        internal static bool ToBool(ushort val) { return (val == 0) ? false : true; }
        internal static bool ToBool(uint val) { return (val == 0) ? false : true; }
        internal static bool ToBool(ulong val) { return (val == 0) ? false : true; }
        internal static bool ToBool(float val) { return (val == 0) ? false : true; }
        internal static bool ToBool(double val) { return (val == 0) ? false : true; }
        internal static bool ToBool(decimal val) { return (val == 0) ? false : true; }
        internal static bool ToBool(Guid val) { return (val == Guid.Empty) ? false : true; }
        internal static bool ToBool(DateTime val) { return true; }
        internal static bool ToBool(TimeSpan val) { return (ToDouble(val) == 0) ? false : true; }
        internal static bool ToBool(String val) { bool v; return TryParse(val, out v) ? v : false; }
        internal static bool ToBool(char[] val) { return (val == null || val.Length == 0) ? false : true; }
        internal static bool ToBool(byte[] val) { return (val == null || val.Length == 0) ? false : true; }
        internal static bool ToBool(sbyte[] val) { return (val == null || val.Length == 0) ? false : true; }
        internal static bool ToBool(short[] val) { return (val == null || val.Length == 0) ? false : true; }
        internal static bool ToBool(int[] val) { return (val == null || val.Length == 0) ? false : true; }
        internal static bool ToBool(long[] val) { return (val == null || val.Length == 0) ? false : true; }
        internal static bool ToBool(ushort[] val) { return (val == null || val.Length == 0) ? false : true; }
        internal static bool ToBool(uint[] val) { return (val == null || val.Length == 0) ? false : true; }
        internal static bool ToBool(ulong[] val) { return (val == null || val.Length == 0) ? false : true; }
        internal static bool ToBool(float[] val) { return (val == null || val.Length == 0) ? false : true; }
        internal static bool ToBool(double[] val) { return (val == null || val.Length == 0) ? false : true; }
        internal static bool ToBool(decimal[] val) { return (val == null || val.Length == 0) ? false : true; }
        #endregion

        #region ToByte  ======================================================
        internal static byte ToByte(bool val) { return (byte)(val ? 0 : 1); }
        internal static byte ToByte(char val) { return ToByte(ToInt16(val)); }
        internal static byte ToByte(byte val) { return val; }
        internal static byte ToByte(sbyte val) { return (byte)val; }
        internal static byte ToByte(short val) { return (val < byte.MinValue) ? byte.MinValue : (val > byte.MaxValue) ? byte.MaxValue : (byte)val; }
        internal static byte ToByte(int val) { return (val < byte.MinValue) ? byte.MinValue : (val > byte.MaxValue) ? byte.MaxValue : (byte)val; }
        internal static byte ToByte(long val) { return (val < byte.MinValue) ? byte.MinValue : (val > byte.MaxValue) ? byte.MaxValue : (byte)val; }
        internal static byte ToByte(ushort val) { return (val > byte.MaxValue) ? byte.MaxValue : (byte)val; }
        internal static byte ToByte(uint val) { return (val > byte.MaxValue) ? byte.MaxValue : (byte)val; }
        internal static byte ToByte(ulong val) { return (val > byte.MaxValue) ? byte.MaxValue : (byte)val; }
        internal static byte ToByte(float val) { return (val < byte.MinValue) ? byte.MinValue : (val > byte.MaxValue) ? byte.MaxValue : (byte)val; }
        internal static byte ToByte(double val) { return (val < byte.MinValue) ? byte.MinValue : (val > byte.MaxValue) ? byte.MaxValue : (byte)val; }
        internal static byte ToByte(decimal val) { return (val < byte.MinValue) ? byte.MinValue : (val > byte.MaxValue) ? byte.MaxValue : (byte)val; }
        internal static byte ToByte(Guid val) { return ToByte(val.GetHashCode()); }
        internal static byte ToByte(DateTime val) { return ToByte(val.Ticks); }
        internal static byte ToByte(TimeSpan val) { return ToByte(ToDouble(val)); }
        internal static byte ToByte(String val) { byte v; return TryParse(val, out v) ? v : (byte)0; }
        internal static byte ToByte(char[] val) { return (byte)((val == null) ? 0 : val.Length); }
        internal static byte ToByte(byte[] val) { return (byte)((val == null) ? 0 : val.Length); }
        internal static byte ToByte(sbyte[] val) { return (byte)((val == null) ? 0 : val.Length); }
        internal static byte ToByte(short[] val) { return (byte)((val == null) ? 0 : val.Length); }
        internal static byte ToByte(int[] val) { return (byte)((val == null) ? 0 : val.Length); }
        internal static byte ToByte(long[] val) { return (byte)((val == null) ? 0 : val.Length); }
        internal static byte ToByte(ushort[] val) { return (byte)((val == null) ? 0 : val.Length); }
        internal static byte ToByte(uint[] val) { return (byte)((val == null) ? 0 : val.Length); }
        internal static byte ToByte(ulong[] val) { return (byte)((val == null) ? 0 : val.Length); }
        internal static byte ToByte(float[] val) { return (byte)((val == null) ? 0 : val.Length); }
        internal static byte ToByte(double[] val) { return (byte)((val == null) ? 0 : val.Length); }
        internal static byte ToByte(decimal[] val) { return (byte)((val == null) ? 0 : val.Length); }
        #endregion

        #region ToInt16  ======================================================
        internal static short ToInt16(bool val) { return (short)(val ? 0 : 1); }
        internal static short ToInt16(char val) { return (short)val; }
        internal static short ToInt16(byte val) { return val; }
        internal static short ToInt16(sbyte val) { return val; }
        internal static short ToInt16(short val) { return val; }
        internal static short ToInt16(int val) { return (val < short.MinValue) ? short.MinValue : (val > short.MaxValue) ? short.MaxValue : (short)val; }
        internal static short ToInt16(long val) { return (val < short.MinValue) ? short.MinValue : (val > short.MaxValue) ? short.MaxValue : (short)val; }
        internal static short ToInt16(ushort val) { return (val > short.MaxValue) ? short.MaxValue : (short)val; }
        internal static short ToInt16(uint val) { return (val > short.MaxValue) ? short.MaxValue : (short)val; }
        internal static short ToInt16(ulong val) { return (val > (long)short.MaxValue) ? short.MaxValue : (short)val; }
        internal static short ToInt16(float val) { return (val < short.MinValue) ? short.MinValue : ((val > short.MaxValue) ? short.MaxValue : (short)val); }
        internal static short ToInt16(double val) { return (val < short.MinValue) ? short.MinValue : ((val > short.MaxValue) ? short.MaxValue : (short)val); }
        internal static short ToInt16(decimal val) { return (val < short.MinValue) ? short.MinValue : ((val > short.MaxValue) ? short.MaxValue : (short)val); }
        internal static short ToInt16(Guid val) { return (short)val.GetHashCode(); }
        internal static short ToInt16(DateTime val) { return (short)val.Ticks; }
        internal static short ToInt16(TimeSpan val) { return ToInt16(ToDouble(val)); }
        internal static short ToInt16(String val) { short v; return TryParse(val, out v) ? v : (short)0; }
        internal static short ToInt16(char[] val) { return (short)((val == null) ? 0 : val.Length); }
        internal static short ToInt16(byte[] val) { return (short)((val == null) ? 0 : val.Length); }
        internal static short ToInt16(sbyte[] val) { return (short)((val == null) ? 0 : val.Length); }
        internal static short ToInt16(short[] val) { return (short)((val == null) ? 0 : val.Length); }
        internal static short ToInt16(int[] val) { return (short)((val == null) ? 0 : val.Length); }
        internal static short ToInt16(long[] val) { return (short)((val == null) ? 0 : val.Length); }
        internal static short ToInt16(ushort[] val) { return (short)((val == null) ? 0 : val.Length); }
        internal static short ToInt16(uint[] val) { return (short)((val == null) ? 0 : val.Length); }
        internal static short ToInt16(ulong[] val) { return (short)((val == null) ? 0 : val.Length); }
        internal static short ToInt16(float[] val) { return (short)((val == null) ? 0 : val.Length); }
        internal static short ToInt16(double[] val) { return (short)((val == null) ? 0 : val.Length); }
        internal static short ToInt16(decimal[] val) { return (short)((val == null) ? 0 : val.Length); }
        #endregion

        #region ToInt32  ======================================================
        internal static int ToInt32(bool val) { return val ? 1 : 0; }
        internal static int ToInt32(char val) { return val; }
        internal static int ToInt32(byte val) { return val; }
        internal static int ToInt32(sbyte val) { return val; }
        internal static int ToInt32(short val) { return val; }
        internal static int ToInt32(int val) { return val; }
        internal static int ToInt32(long val) { return (val < int.MinValue) ? int.MinValue : ((val > int.MaxValue) ? int.MaxValue : (int)val); }
        internal static int ToInt32(ushort val) { return val; }
        internal static int ToInt32(uint val) { return (int)val; }
        internal static int ToInt32(ulong val) { return (val > int.MaxValue) ? int.MaxValue : (int)val; }
        internal static int ToInt32(float val) { return (val < int.MinValue) ? int.MinValue : ((val > int.MaxValue) ? int.MaxValue : (int)val); }
        internal static int ToInt32(double val) { return (val < int.MinValue) ? int.MinValue : ((val > int.MaxValue) ? int.MaxValue : (int)val); }
        internal static int ToInt32(decimal val) { return (val < int.MinValue) ? int.MinValue : ((val > int.MaxValue) ? int.MaxValue : (int)val); }
        internal static int ToInt32(Guid val) { return val.GetHashCode(); }
        internal static int ToInt32(DateTime val) { return val.GetHashCode(); }
        internal static int ToInt32(TimeSpan val) { return ToInt32(ToDouble(val)); }
        internal static int ToInt32(String val) { int v; return TryParse(val, out v) ? v : 0; }
        internal static int ToInt32(char[] val) { return (val == null) ? 0 : val.Length; }
        internal static int ToInt32(byte[] val) { return (val == null) ? 0 : val.Length; }
        internal static int ToInt32(sbyte[] val) { return (val == null) ? 0 : val.Length; }
        internal static int ToInt32(short[] val) { return (val == null) ? 0 : val.Length; }
        internal static int ToInt32(int[] val) { return (val == null) ? 0 : val.Length; }
        internal static int ToInt32(long[] val) { return (val == null) ? 0 : val.Length; }
        internal static int ToInt32(ushort[] val) { return (val == null) ? 0 : val.Length; }
        internal static int ToInt32(uint[] val) { return (val == null) ? 0 : val.Length; }
        internal static int ToInt32(ulong[] val) { return (val == null) ? 0 : val.Length; }
        internal static int ToInt32(float[] val) { return (val == null) ? 0 : val.Length; }
        internal static int ToInt32(double[] val) { return (val == null) ? 0 : val.Length; }
        internal static int ToInt32(decimal[] val) { return (val == null) ? 0 : val.Length; }

        #endregion

        #region ToInt64  ======================================================
        internal static long ToInt64(bool val) { return (long)(val ? 0 : 1); }
        internal static long ToInt64(char val) { return val; }
        internal static long ToInt64(byte val) { return val; }
        internal static long ToInt64(sbyte val) { return val; }
        internal static long ToInt64(short val) { return val; }
        internal static long ToInt64(int val) { return val; }
        internal static long ToInt64(long val) { return val; }
        internal static long ToInt64(ushort val) { return val; }
        internal static long ToInt64(uint val) { return val; }
        internal static long ToInt64(ulong val) { return (val > long.MaxValue) ? long.MaxValue : (long)val; }
        internal static long ToInt64(float val) { return (val < long.MinValue) ? long.MinValue : (val > long.MaxValue) ? long.MaxValue : (long)val; }
        internal static long ToInt64(double val) { return (val < long.MinValue) ? long.MinValue : (val > long.MaxValue) ? long.MaxValue : (long)val; }
        internal static long ToInt64(decimal val) { return (val < long.MinValue) ? long.MinValue : (val > long.MaxValue) ? long.MaxValue : (long)val; }
        internal static long ToInt64(Guid val) { return val.GetHashCode(); }
        internal static long ToInt64(DateTime val) { return val.Ticks; }
        internal static long ToInt64(TimeSpan val) { return ToInt64(ToDouble(val)); }
        internal static long ToInt64(String val) { long v; return TryParse(val, out v) ? v : 0; }
        internal static long ToInt64(char[] val) { return (long)((val == null) ? 0 : val.Length); }
        internal static long ToInt64(byte[] val) { return (long)((val == null) ? 0 : val.Length); }
        internal static long ToInt64(sbyte[] val) { return (long)((val == null) ? 0 : val.Length); }
        internal static long ToInt64(short[] val) { return (long)((val == null) ? 0 : val.Length); }
        internal static long ToInt64(int[] val) { return (long)((val == null) ? 0 : val.Length); }
        internal static long ToInt64(long[] val) { return (long)((val == null) ? 0 : val.Length); }
        internal static long ToInt64(ushort[] val) { return (long)((val == null) ? 0 : val.Length); }
        internal static long ToInt64(uint[] val) { return (long)((val == null) ? 0 : val.Length); }
        internal static long ToInt64(ulong[] val) { return (long)((val == null) ? 0 : val.Length); }
        internal static long ToInt64(float[] val) { return (long)((val == null) ? 0 : val.Length); }
        internal static long ToInt64(double[] val) { return (long)((val == null) ? 0 : val.Length); }
        internal static long ToInt64(decimal[] val) { return (long)((val == null) ? 0 : val.Length); }
        #endregion

        #region ToDouble  =====================================================
        //
        internal static double ToDouble(bool val) { return val ? 0 : 1; }
        internal static double ToDouble(char val) { return val; }
        internal static double ToDouble(byte val) { return val; }
        internal static double ToDouble(sbyte val) { return val; }
        internal static double ToDouble(short val) { return val; }
        internal static double ToDouble(int val) { return val; }
        internal static double ToDouble(long val) { return val; }
        internal static double ToDouble(ushort val) { return val; }
        internal static double ToDouble(uint val) { return val; }
        internal static double ToDouble(ulong val) { return val; }
        internal static double ToDouble(float val) { return val; }
        internal static double ToDouble(double val) { return val; }
        internal static double ToDouble(decimal val) { return (double)val; }
        internal static double ToDouble(Guid val) { return val.GetHashCode(); }
        internal static double ToDouble(DateTime val) { return val.Ticks; }
        internal static double ToDouble(TimeSpan val) { return val.TotalMilliseconds; }
        internal static double ToDouble(String val) { double v; return TryParse(val, out v) ? v : 0; }
        internal static double ToDouble(char[] val) { return (val == null) ? 0 : val.Length; }
        internal static double ToDouble(byte[] val) { return (val == null) ? 0 : val.Length; }
        internal static double ToDouble(sbyte[] val) { return (val == null) ? 0 : val.Length; }
        internal static double ToDouble(short[] val) { return (val == null) ? 0 : val.Length; }
        internal static double ToDouble(int[] val) { return (val == null) ? 0 : val.Length; }
        internal static double ToDouble(long[] val) { return (val == null) ? 0 : val.Length; }
        internal static double ToDouble(ushort[] val) { return (val == null) ? 0 : val.Length; }
        internal static double ToDouble(uint[] val) { return (val == null) ? 0 : val.Length; }
        internal static double ToDouble(ulong[] val) { return (val == null) ? 0 : val.Length; }
        internal static double ToDouble(float[] val) { return (val == null) ? 0 : val.Length; }
        internal static double ToDouble(double[] val) { return (val == null) ? 0 : val.Length; }
        internal static double ToDouble(decimal[] val) { return (val == null) ? 0 : val.Length; }
        #endregion

        #region ToString  =====================================================
        //
        internal static string ToString(bool val) { return val.ToString(); }
        internal static string ToString(char val) { return val.ToString(); }
        internal static string ToString(byte val) { return val.ToString(); }
        internal static string ToString(sbyte val) { return val.ToString(); }
        internal static string ToString(short val) { return val.ToString(); }
        internal static string ToString(int val) { return val.ToString(); }
        internal static string ToString(long val) { return val.ToString(); }
        internal static string ToString(ushort val) { return val.ToString(); }
        internal static string ToString(uint val) { return val.ToString(); }
        internal static string ToString(ulong val) { return val.ToString(); }
        internal static string ToString(float val) { return val.ToString(); }
        internal static string ToString(double val) { return DecimalString(val); }
        internal static string ToString(decimal val) { return val.ToString(); }
        internal static string ToString(Guid val) { return val.ToString(); }
        internal static string ToString(DateTime val) { return val.ToString(); }
        internal static string ToString(TimeSpan val) { return val.ToString(); }
        internal static string ToString(String val) { return val; }

        #region ArrayToString  ================================================
        internal static string ToString(char[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            if (len == 0) return null;

            var sb = new StringBuilder(len);
            for (int i = 0; i < len; i++) { sb.Append(val[i]); }
            return sb.ToString();
        }

        internal static string ToStringHex(byte[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            if (len == 0) return null;

            var sb = new StringBuilder(len * 2);
            for (int i = 0; i < len; i++) { sb.Append(hexChar[((val[i] & 0xF0) >> 4)]); sb.Append(hexChar[(val[i] & 0x0F)]); }
            return sb.ToString();
        }
        static char[] hexChar = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        internal static string ToString(byte[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            var A = len > 1;
            var E = len - 1;
            if (len == 0) return null;

            var sb = new StringBuilder(len * 20);
            for (int i = 0; i < len; i++) { sb.Append(val[i].ToString()); if (A && i != E) sb.Append(", "); }
            return sb.ToString();
        }
        internal static string ToString(sbyte[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            var A = len > 1;
            var E = len - 1;
            if (len == 0) return null;

            var sb = new StringBuilder(len * 20);
            for (int i = 0; i < len; i++) { sb.Append(val[i].ToString()); if (A && i != E) sb.Append(", "); }
            return sb.ToString();
        }
        internal static string ToString(short[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            var A = len > 1;
            var E = len - 1;
            if (len == 0) return null;

            var sb = new StringBuilder(len * 20);
            for (int i = 0; i < len; i++) { sb.Append(val[i].ToString()); if (A && i != E) sb.Append(", "); }
            return sb.ToString();
        }
        internal static string ToString(int[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            var A = len > 1;
            var E = len - 1;
            if (len == 0) return null;

            var sb = new StringBuilder(len * 20);
            for (int i = 0; i < len; i++) { sb.Append(val[i].ToString()); if (A && i != E) sb.Append(", "); }
            return sb.ToString();
        }
        internal static string ToString(long[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            var A = len > 1;
            var E = len - 1;
            if (len == 0) return null;

            var sb = new StringBuilder(len * 20);
            for (int i = 0; i < len; i++) { sb.Append(val[i].ToString()); if (A && i != E) sb.Append(", "); }
            return sb.ToString();
        }
        internal static string ToString(ushort[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            var A = len > 1;
            var E = len - 1;
            if (len == 0) return null;

            var sb = new StringBuilder(len * 20);
            for (int i = 0; i < len; i++) { sb.Append(val[i].ToString()); if (A && i != E) sb.Append(", "); }
            return sb.ToString();
        }
        internal static string ToString(uint[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            var A = len > 1;
            var E = len - 1;
            if (len == 0) return null;

            var sb = new StringBuilder(len * 20);
            for (int i = 0; i < len; i++) { sb.Append(val[i].ToString()); if (A && i != E) sb.Append(", "); }
            return sb.ToString();
        }
        internal static string ToString(ulong[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            var A = len > 1;
            var E = len - 1;
            if (len == 0) return null;

            var sb = new StringBuilder(len * 20);
            for (int i = 0; i < len; i++) { sb.Append(val[i].ToString()); if (A && i != E) sb.Append(", "); }
            return sb.ToString();
        }
        internal static string ToString(float[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            var A = len > 1;
            var E = len - 1;
            if (len == 0) return null;

            var sb = new StringBuilder(len * 20);
            for (int i = 0; i < len; i++) { sb.Append(val[i].ToString()); if (A && i != E) sb.Append(", "); }
            return sb.ToString();
        }
        internal static string ToString(double[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            var A = len > 1;
            var E = len - 1;
            if (len == 0) return null;

            var sb = new StringBuilder(len * 20);
            for (int i = 0; i < len; i++) { sb.Append(val[i].ToString()); if (A && i != E) sb.Append(", "); }
            return sb.ToString();
        }
        internal static string ToString(decimal[] val)
        {
            if (val == null) return null;

            var len = val.Length;
            var A = len > 1;
            var E = len - 1;
            if (len == 0) return null;

            var sb = new StringBuilder(len * 20);
            for (int i = 0; i < len; i++) { sb.Append(val[i].ToString()); if (A && i != E) sb.Append(", "); }
            return sb.ToString();
        }
        #endregion

        #region DecimalString  ================================================
        internal static string DecimalString(double val, int digits = 4)
        {
            if (Double.IsNaN(val) || Double.IsNegativeInfinity(val) || Double.IsPositiveInfinity(val) || val == double.MinValue || val == double.MaxValue) return "??";
            return Math.Round(val, digits, MidpointRounding.ToEven).ToString();
        }
        #endregion
        #endregion
        #endregion
    }
}
