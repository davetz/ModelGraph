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
        internal static bool TryParse(string input, out char val) => char.TryParse(input, out val);
        internal static bool TryParse(string input, out byte val) => byte.TryParse(input, out val);
        internal static bool TryParse(string input, out sbyte val) => sbyte.TryParse(input, out val);
        internal static bool TryParse(string input, out short val) => short.TryParse(input, out val);
        internal static bool TryParse(string input, out int val) => int.TryParse(input, out val);
        internal static bool TryParse(string input, out long val) => long.TryParse(input, out val);
        internal static bool TryParse(string input, out ushort val) => ushort.TryParse(input, out val);
        internal static bool TryParse(string input, out uint val) => uint.TryParse(input, out val);
        internal static bool TryParse(string input, out ulong val) => ulong.TryParse(input, out val);
        internal static bool TryParse(string input, out float val) => float.TryParse(input, out val);
        internal static bool TryParse(string input, out double val) => double.TryParse(input, out val);
        internal static bool TryParse(string input, out decimal val) => decimal.TryParse(input, out val);
        internal static bool TryParse(string input, out Guid val) => Guid.TryParse(input, out val);
        internal static bool TryParse(string input, out DateTime val) => DateTime.TryParse(input, out val);
        internal static bool TryParse(string input, out TimeSpan val) => TimeSpan.TryParse(input, out val);
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
        internal static bool ToBool(bool val) => val;
        internal static bool ToBool(char val) => (val == '0') ? false : true;
        internal static bool ToBool(byte val) => (val == 0) ? false : true;
        internal static bool ToBool(sbyte val) => (val == 0) ? false : true;
        internal static bool ToBool(short val) => (val == 0) ? false : true;
        internal static bool ToBool(int val) => (val == 0) ? false : true;
        internal static bool ToBool(long val) => (val == 0) ? false : true;
        internal static bool ToBool(ushort val) => (val == 0) ? false : true;
        internal static bool ToBool(uint val) => (val == 0) ? false : true;
        internal static bool ToBool(ulong val) => (val == 0) ? false : true;
        internal static bool ToBool(float val) => (val == 0) ? false : true;
        internal static bool ToBool(double val) => (val == 0) ? false : true;
        internal static bool ToBool(decimal val) => (val == 0) ? false : true;

        internal static bool ToBool(Guid val) => (val == Guid.Empty) ? false : true;
        internal static bool ToBool(DateTime val) => true;
        internal static bool ToBool(TimeSpan val) => (ToDouble(val) == 0) ? false : true;

        internal static bool ToBool(String val) => TryParse(val, out bool v) ? v : false;

        internal static bool ToBool(char[] val) => (val == null || val.Length == 0) ? false : true;
        internal static bool ToBool(byte[] val) => (val == null || val.Length == 0) ? false : true;
        internal static bool ToBool(sbyte[] val) => (val == null || val.Length == 0) ? false : true;
        internal static bool ToBool(short[] val) => (val == null || val.Length == 0) ? false : true;
        internal static bool ToBool(int[] val) => (val == null || val.Length == 0) ? false : true;
        internal static bool ToBool(long[] val) => (val == null || val.Length == 0) ? false : true;
        internal static bool ToBool(ushort[] val) => (val == null || val.Length == 0) ? false : true;
        internal static bool ToBool(uint[] val) => (val == null || val.Length == 0) ? false : true;
        internal static bool ToBool(ulong[] val) => (val == null || val.Length == 0) ? false : true;
        internal static bool ToBool(float[] val) => (val == null || val.Length == 0) ? false : true;
        internal static bool ToBool(double[] val) => (val == null || val.Length == 0) ? false : true;
        internal static bool ToBool(decimal[] val) => (val == null || val.Length == 0) ? false : true;
        #endregion

        #region ToChar  =======================================================
        internal static char ToChar(bool val) => val ? 'T' : 'F'; 
        internal static char ToChar(char val) => val;
        internal static char ToChar(byte val) => (char)val;
        internal static char ToChar(sbyte val) => (char)val;
        internal static char ToChar(short val) => (char)val;
        internal static char ToChar(ushort val) => (char)val;
        internal static char ToChar(int val) => (val < char.MinValue) ? char.MinValue : (val > char.MaxValue) ? char.MaxValue : (char)val;
        internal static char ToChar(uint val) => (val > char.MaxValue) ? char.MaxValue : (char)val;
        internal static char ToChar(long val) => (val < char.MinValue) ? char.MinValue : (val > char.MaxValue) ? char.MaxValue : (char)val;
        internal static char ToChar(ulong val) => (val > char.MaxValue) ? char.MaxValue : (char)val;
        internal static char ToChar(float val) => (val < char.MinValue) ? char.MinValue : (val > char.MaxValue) ? char.MaxValue : (char)val;
        internal static char ToChar(double val) => (val < char.MinValue) ? char.MinValue : (val > char.MaxValue) ? char.MaxValue : (char)val;
        internal static char ToChar(decimal val) => (val < char.MinValue) ? char.MinValue : (val > char.MaxValue) ? char.MaxValue : (char)val;

        internal static char ToChar(Guid val) => ToChar(val.GetHashCode());
        internal static char ToChar(DateTime val) => ToChar(val.Ticks);
        internal static char ToChar(TimeSpan val) => ToChar(ToDouble(val));

        internal static char ToChar(String val) => (string.IsNullOrWhiteSpace(val) || val.Length == 0) ? ' ' : val[0];

        internal static char ToChar(char[] val) => (val == null) ? (char)0 : ToChar(val.Length);
        internal static char ToChar(byte[] val) => (val == null) ? (char)0 : ToChar(val.Length);
        internal static char ToChar(sbyte[] val) => (val == null) ? (char)0 : ToChar(val.Length);
        internal static char ToChar(short[] val) => (val == null) ? (char)0 : ToChar(val.Length);
        internal static char ToChar(int[] val) => (val == null) ? (char)0 : ToChar(val.Length);
        internal static char ToChar(long[] val) => (val == null) ? (char)0 : ToChar(val.Length);
        internal static char ToChar(ushort[] val) => (val == null) ? (char)0 : ToChar(val.Length);
        internal static char ToChar(uint[] val) => (val == null) ? (char)0 : ToChar(val.Length);
        internal static char ToChar(ulong[] val) => (val == null) ? (char)0 : ToChar(val.Length);
        internal static char ToChar(float[] val) => (val == null) ? (char)0 : ToChar(val.Length);
        internal static char ToChar(double[] val) => (val == null) ? (char)0 : ToChar(val.Length);
        internal static char ToChar(decimal[] val) => (val == null) ? (char)0 : ToChar(val.Length);
        #endregion

        #region ToByte  =======================================================
        internal static byte ToByte(bool val) => (byte)(val ? 0 : 1);
        internal static byte ToByte(char val) => ToByte(ToInt16(val));
        internal static byte ToByte(byte val) => val;
        internal static byte ToByte(sbyte val) => (byte)val;
        internal static byte ToByte(short val) => (val < byte.MinValue) ? byte.MinValue : (val > byte.MaxValue) ? byte.MaxValue : (byte)val;
        internal static byte ToByte(int val) => (val < byte.MinValue) ? byte.MinValue : (val > byte.MaxValue) ? byte.MaxValue : (byte)val;
        internal static byte ToByte(long val) => (val < byte.MinValue) ? byte.MinValue : (val > byte.MaxValue) ? byte.MaxValue : (byte)val;
        internal static byte ToByte(ushort val) => (val > byte.MaxValue) ? byte.MaxValue : (byte)val;
        internal static byte ToByte(uint val) => (val > byte.MaxValue) ? byte.MaxValue : (byte)val;
        internal static byte ToByte(ulong val) => (val > byte.MaxValue) ? byte.MaxValue : (byte)val;
        internal static byte ToByte(float val) => (val < byte.MinValue) ? byte.MinValue : (val > byte.MaxValue) ? byte.MaxValue : (byte)val;
        internal static byte ToByte(double val) => (val < byte.MinValue) ? byte.MinValue : (val > byte.MaxValue) ? byte.MaxValue : (byte)val;
        internal static byte ToByte(decimal val) => (val < byte.MinValue) ? byte.MinValue : (val > byte.MaxValue) ? byte.MaxValue : (byte)val;

        internal static byte ToByte(Guid val) => ToByte(val.GetHashCode());
        internal static byte ToByte(DateTime val) => ToByte(val.Ticks);
        internal static byte ToByte(TimeSpan val) => ToByte(ToDouble(val));

        internal static byte ToByte(String val) => TryParse(val, out byte v) ? v : (byte)0;

        internal static byte ToByte(char[] val) => (val == null) ? (byte)0 : ToByte(val.Length);
        internal static byte ToByte(byte[] val) => (val == null) ? (byte)0 : ToByte(val.Length);
        internal static byte ToByte(sbyte[] val) => (val == null) ? (byte)0 : ToByte(val.Length);
        internal static byte ToByte(short[] val) => (val == null) ? (byte)0 : ToByte(val.Length);
        internal static byte ToByte(int[] val) => (val == null) ? (byte)0 : ToByte(val.Length);
        internal static byte ToByte(long[] val) => (val == null) ? (byte)0 : ToByte(val.Length);
        internal static byte ToByte(ushort[] val) => (val == null) ? (byte)0 : ToByte(val.Length);
        internal static byte ToByte(uint[] val) => (val == null) ? (byte)0 : ToByte(val.Length);
        internal static byte ToByte(ulong[] val) => (val == null) ? (byte)0 : ToByte(val.Length);
        internal static byte ToByte(float[] val) => (val == null) ? (byte)0 : ToByte(val.Length);
        internal static byte ToByte(double[] val) => (val == null) ? (byte)0 : ToByte(val.Length);
        internal static byte ToByte(decimal[] val) => (val == null) ? (byte)0 : ToByte(val.Length);
        #endregion

        #region ToSByte  ======================================================
        internal static sbyte ToSByte(bool val) => (sbyte)(val ? 0 : 1);
        internal static sbyte ToSByte(char val) => ToSByte(ToInt16(val));
        internal static sbyte ToSByte(byte val) => (sbyte)val;
        internal static sbyte ToSByte(sbyte val) => val;
        internal static sbyte ToSByte(short val) => (val < sbyte.MinValue) ? sbyte.MinValue : (val > sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)val;
        internal static sbyte ToSByte(int val) => (val < sbyte.MinValue) ? sbyte.MinValue : (val > sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)val;
        internal static sbyte ToSByte(long val) => (val < sbyte.MinValue) ? sbyte.MinValue : (val > sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)val;
        internal static sbyte ToSByte(ushort val) => (val > sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)val;
        internal static sbyte ToSByte(uint val) => (val > sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)val;
        internal static sbyte ToSByte(ulong val) => ((long)val > sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)val;
        internal static sbyte ToSByte(float val) => (val < sbyte.MinValue) ? sbyte.MinValue : (val > sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)val;
        internal static sbyte ToSByte(double val) => (val < sbyte.MinValue) ? sbyte.MinValue : (val > sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)val;
        internal static sbyte ToSByte(decimal val) => (val < sbyte.MinValue) ? sbyte.MinValue : (val > sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)val;

        internal static sbyte ToSByte(Guid val) => ToSByte(val.GetHashCode());
        internal static sbyte ToSByte(DateTime val) => ToSByte(val.Ticks);
        internal static sbyte ToSByte(TimeSpan val) => ToSByte(ToDouble(val));

        internal static sbyte ToSByte(String val) => TryParse(val, out sbyte v) ? v : (sbyte)0;

        internal static sbyte ToSByte(char[] val) => (sbyte)((val == null) ? 0 : ToSByte(val.Length));
        internal static sbyte ToSByte(byte[] val) => (sbyte)((val == null) ? 0 : ToSByte(val.Length));
        internal static sbyte ToSByte(sbyte[] val) => (sbyte)((val == null) ? 0 : ToSByte(val.Length));
        internal static sbyte ToSByte(short[] val) => (sbyte)((val == null) ? 0 : ToSByte(val.Length));
        internal static sbyte ToSByte(int[] val) => (sbyte)((val == null) ? 0 : ToSByte(val.Length));
        internal static sbyte ToSByte(long[] val) => (sbyte)((val == null) ? 0 : ToSByte(val.Length));
        internal static sbyte ToSByte(ushort[] val) => (sbyte)((val == null) ? 0 : ToSByte(val.Length));
        internal static sbyte ToSByte(uint[] val) => (sbyte)((val == null) ? 0 : ToSByte(val.Length));
        internal static sbyte ToSByte(ulong[] val) => (sbyte)((val == null) ? 0 : ToSByte(val.Length));
        internal static sbyte ToSByte(float[] val) => (sbyte)((val == null) ? 0 : ToSByte(val.Length));
        internal static sbyte ToSByte(double[] val) => (sbyte)((val == null) ? 0 : ToSByte(val.Length));
        internal static sbyte ToSByte(decimal[] val) => (sbyte)((val == null) ? 0 : ToSByte(val.Length));
        #endregion

        #region ToInt16  ======================================================
        internal static short ToInt16(bool val) => (short)(val ? 0 : 1);
        internal static short ToInt16(char val) => (short)val;
        internal static short ToInt16(byte val) => val;
        internal static short ToInt16(sbyte val) => val;
        internal static short ToInt16(short val) => val;
        internal static short ToInt16(int val) => (val < short.MinValue) ? short.MinValue : (val > short.MaxValue) ? short.MaxValue : (short)val;
        internal static short ToInt16(long val) => (val < short.MinValue) ? short.MinValue : (val > short.MaxValue) ? short.MaxValue : (short)val;
        internal static short ToInt16(ushort val) => (val > short.MaxValue) ? short.MaxValue : (short)val;
        internal static short ToInt16(uint val) => (val > short.MaxValue) ? short.MaxValue : (short)val;
        internal static short ToInt16(ulong val) => (val > (long)short.MaxValue) ? short.MaxValue : (short)val;
        internal static short ToInt16(float val) => (val < short.MinValue) ? short.MinValue : ((val > short.MaxValue) ? short.MaxValue : (short)val);
        internal static short ToInt16(double val) => (val < short.MinValue) ? short.MinValue : ((val > short.MaxValue) ? short.MaxValue : (short)val);
        internal static short ToInt16(decimal val) => (val < short.MinValue) ? short.MinValue : ((val > short.MaxValue) ? short.MaxValue : (short)val);

        internal static short ToInt16(Guid val) => (short)val.GetHashCode();
        internal static short ToInt16(DateTime val) => (short)val.Ticks;
        internal static short ToInt16(TimeSpan val) => ToInt16(ToDouble(val));
        internal static short ToInt16(String val) => TryParse(val, out short v) ? v : (short)0;

        internal static short ToInt16(char[] val) => (val == null) ? (short)0 : ToInt16(val.Length);
        internal static short ToInt16(byte[] val) => (val == null) ? (short)0 : ToInt16(val.Length);
        internal static short ToInt16(sbyte[] val) => (val == null) ? (short)0 : ToInt16(val.Length);
        internal static short ToInt16(short[] val) => (val == null) ? (short)0 : ToInt16(val.Length);
        internal static short ToInt16(int[] val) => (val == null) ? (short)0 : ToInt16(val.Length);
        internal static short ToInt16(long[] val) => (val == null) ? (short)0 : ToInt16(val.Length);
        internal static short ToInt16(ushort[] val) => (val == null) ? (short)0 : ToInt16(val.Length);
        internal static short ToInt16(uint[] val) => (val == null) ? (short)0 : ToInt16(val.Length);
        internal static short ToInt16(ulong[] val) => (val == null) ? (short)0 : ToInt16(val.Length);
        internal static short ToInt16(float[] val) => (val == null) ? (short)0 : ToInt16(val.Length);
        internal static short ToInt16(double[] val) => (val == null) ? (short)0 : ToInt16(val.Length);
        internal static short ToInt16(decimal[] val) => (val == null) ? (short)0 : ToInt16(val.Length);
        #endregion

        #region ToUInt16  =====================================================
        internal static ushort ToUInt16(bool val) => (ushort)(val ? 0 : 1);
        internal static ushort ToUInt16(char val) => val;
        internal static ushort ToUInt16(byte val) => val;
        internal static ushort ToUInt16(sbyte val) => (ushort)val;
        internal static ushort ToUInt16(short val) => (ushort)val;
        internal static ushort ToUInt16(int val) => (val < ushort.MinValue) ? ushort.MinValue : (val > ushort.MaxValue) ? ushort.MaxValue : (ushort)val;
        internal static ushort ToUInt16(long val) => (val < ushort.MinValue) ? ushort.MinValue : (val > ushort.MaxValue) ? ushort.MaxValue : (ushort)val;
        internal static ushort ToUInt16(ushort val) => val;
        internal static ushort ToUInt16(uint val) => (val > ushort.MaxValue) ? ushort.MaxValue : (ushort)val;
        internal static ushort ToUInt16(ulong val) => (val > ushort.MaxValue) ? ushort.MaxValue : (ushort)val;
        internal static ushort ToUInt16(float val) => (val < ushort.MinValue) ? ushort.MinValue : ((val > ushort.MaxValue) ? ushort.MaxValue : (ushort)val);
        internal static ushort ToUInt16(double val) => (val < ushort.MinValue) ? ushort.MinValue : ((val > ushort.MaxValue) ? ushort.MaxValue : (ushort)val);
        internal static ushort ToUInt16(decimal val) => (val < ushort.MinValue) ? ushort.MinValue : ((val > ushort.MaxValue) ? ushort.MaxValue : (ushort)val);

        internal static ushort ToUInt16(Guid val) => (ushort)val.GetHashCode();
        internal static ushort ToUInt16(DateTime val) => (ushort)val.Ticks;
        internal static ushort ToUInt16(TimeSpan val) => ToUInt16(ToDouble(val));

        internal static ushort ToUInt16(String val) => TryParse(val, out ushort v) ? v : (ushort)0;

        internal static ushort ToUInt16(char[] val) => (val == null) ? (ushort)0 : ToUInt16(val.Length);
        internal static ushort ToUInt16(byte[] val) => (val == null) ? (ushort)0 : ToUInt16(val.Length);
        internal static ushort ToUInt16(sbyte[] val) => (val == null) ? (ushort)0 : ToUInt16(val.Length);
        internal static ushort ToUInt16(short[] val) => (val == null) ? (ushort)0 : ToUInt16(val.Length);
        internal static ushort ToUInt16(int[] val) => (val == null) ? (ushort)0 : ToUInt16(val.Length);
        internal static ushort ToUInt16(long[] val) => (val == null) ? (ushort)0 : ToUInt16(val.Length);
        internal static ushort ToUInt16(ushort[] val) => (val == null) ? (ushort)0 : ToUInt16(val.Length);
        internal static ushort ToUInt16(uint[] val) => (val == null) ? (ushort)0 : ToUInt16(val.Length);
        internal static ushort ToUInt16(ulong[] val) => (val == null) ? (ushort)0 : ToUInt16(val.Length);
        internal static ushort ToUInt16(float[] val) => (val == null) ? (ushort)0 : ToUInt16(val.Length);
        internal static ushort ToUInt16(double[] val) => (val == null) ? (ushort)0 : ToUInt16(val.Length);
        internal static ushort ToUInt16(decimal[] val) => (val == null) ? (ushort)0 : ToUInt16(val.Length);
        #endregion

        #region ToInt32  ======================================================
        internal static int ToInt32(bool val) => val ? 1 : 0;
        internal static int ToInt32(char val) => val;
        internal static int ToInt32(byte val) => val;
        internal static int ToInt32(sbyte val) => val;
        internal static int ToInt32(short val) => val;
        internal static int ToInt32(int val) => val;
        internal static int ToInt32(long val) => (val < int.MinValue) ? int.MinValue : ((val > int.MaxValue) ? int.MaxValue : (int)val);
        internal static int ToInt32(ushort val) => val;
        internal static int ToInt32(uint val) => (int)val;
        internal static int ToInt32(ulong val) => (val > int.MaxValue) ? int.MaxValue : (int)val;
        internal static int ToInt32(float val) => (val < int.MinValue) ? int.MinValue : ((val > int.MaxValue) ? int.MaxValue : (int)val);
        internal static int ToInt32(double val) => (val < int.MinValue) ? int.MinValue : ((val > int.MaxValue) ? int.MaxValue : (int)val);
        internal static int ToInt32(decimal val) => (val < int.MinValue) ? int.MinValue : ((val > int.MaxValue) ? int.MaxValue : (int)val);

        internal static int ToInt32(Guid val) => val.GetHashCode(); 
        internal static int ToInt32(DateTime val) => val.GetHashCode();
        internal static int ToInt32(TimeSpan val) => ToInt32(ToDouble(val));

        internal static int ToInt32(String val) => TryParse(val, out int v) ? v : 0;

        internal static int ToInt32(char[] val) => (val == null) ? 0 : val.Length;
        internal static int ToInt32(byte[] val) => (val == null) ? 0 : val.Length;
        internal static int ToInt32(sbyte[] val) => (val == null) ? 0 : val.Length;
        internal static int ToInt32(short[] val) => (val == null) ? 0 : val.Length;
        internal static int ToInt32(int[] val) => (val == null) ? 0 : val.Length;
        internal static int ToInt32(long[] val) => (val == null) ? 0 : val.Length;
        internal static int ToInt32(ushort[] val) => (val == null) ? 0 : val.Length;
        internal static int ToInt32(uint[] val) => (val == null) ? 0 : val.Length;
        internal static int ToInt32(ulong[] val) => (val == null) ? 0 : val.Length;
        internal static int ToInt32(float[] val) => (val == null) ? 0 : val.Length;
        internal static int ToInt32(double[] val) => (val == null) ? 0 : val.Length;
        internal static int ToInt32(decimal[] val) => (val == null) ? 0 : val.Length;

        #endregion

        #region ToUInt32  =====================================================
        internal static uint ToUInt32(bool val) => val ? 1u : 0;
        internal static uint ToUInt32(char val) => val;
        internal static uint ToUInt32(byte val) => val;
        internal static uint ToUInt32(sbyte val) => (uint)val;
        internal static uint ToUInt32(short val) => (uint)val;
        internal static uint ToUInt32(int val) => (uint)val;
        internal static uint ToUInt32(long val) => (val < uint.MinValue) ? uint.MinValue : ((val > uint.MaxValue) ? uint.MaxValue : (uint)val);
        internal static uint ToUInt32(ushort val) => val;
        internal static uint ToUInt32(uint val) => val;
        internal static uint ToUInt32(ulong val) => (val > uint.MaxValue) ? uint.MaxValue : (uint)val;
        internal static uint ToUInt32(float val) => (val < uint.MinValue) ? uint.MinValue : ((val > uint.MaxValue) ? uint.MaxValue : (uint)val);
        internal static uint ToUInt32(double val) => (val < uint.MinValue) ? uint.MinValue : ((val > uint.MaxValue) ? uint.MaxValue : (uint)val);
        internal static uint ToUInt32(decimal val) => (val < uint.MinValue) ? uint.MinValue : ((val > uint.MaxValue) ? uint.MaxValue : (uint)val);

        internal static uint ToUInt32(Guid val) => ToUInt32(val.GetHashCode());
        internal static uint ToUInt32(DateTime val) => ToUInt32(val.GetHashCode());
        internal static uint ToUInt32(TimeSpan val) => ToUInt32(ToDouble(val));

        internal static uint ToUInt32(String val) => TryParse(val, out uint v) ? v : 0;

        internal static uint ToUInt32(char[] val) => (val == null) ? 0 : ToUInt32(val.Length);
        internal static uint ToUInt32(byte[] val) => (val == null) ? 0 : ToUInt32(val.Length);
        internal static uint ToUInt32(sbyte[] val) => (val == null) ? 0 : ToUInt32(val.Length);
        internal static uint ToUInt32(short[] val) => (val == null) ? 0 : ToUInt32(val.Length);
        internal static uint ToUInt32(int[] val) => (val == null) ? 0 : ToUInt32(val.Length);
        internal static uint ToUInt32(long[] val) => (val == null) ? 0 : ToUInt32(val.Length);
        internal static uint ToUInt32(ushort[] val) => (val == null) ? 0 : ToUInt32(val.Length);
        internal static uint ToUInt32(uint[] val) => (val == null) ? 0 : ToUInt32(val.Length);
        internal static uint ToUInt32(ulong[] val) => (val == null) ? 0 : ToUInt32(val.Length);
        internal static uint ToUInt32(float[] val) => (val == null) ? 0 : ToUInt32(val.Length);
        internal static uint ToUInt32(double[] val) => (val == null) ? 0 : ToUInt32(val.Length);
        internal static uint ToUInt32(decimal[] val) => (val == null) ? 0 : ToUInt32(val.Length);

        #endregion

        #region ToInt64  ======================================================
        internal static long ToInt64(bool val) => (long)(val ? 0 : 1);
        internal static long ToInt64(char val) => val;
        internal static long ToInt64(byte val) => val;
        internal static long ToInt64(sbyte val) => val;
        internal static long ToInt64(short val) => val;
        internal static long ToInt64(int val) => val;
        internal static long ToInt64(long val) => val;
        internal static long ToInt64(ushort val) => val;
        internal static long ToInt64(uint val) => val;
        internal static long ToInt64(ulong val) => (val > long.MaxValue) ? long.MaxValue : (long)val;
        internal static long ToInt64(float val) => (val < long.MinValue) ? long.MinValue : (val > long.MaxValue) ? long.MaxValue : (long)val;
        internal static long ToInt64(double val) => (val < long.MinValue) ? long.MinValue : (val > long.MaxValue) ? long.MaxValue : (long)val;
        internal static long ToInt64(decimal val) => (val < long.MinValue) ? long.MinValue : (val > long.MaxValue) ? long.MaxValue : (long)val;

        internal static long ToInt64(Guid val) => val.GetHashCode();
        internal static long ToInt64(DateTime val) => val.Ticks;
        internal static long ToInt64(TimeSpan val) => ToInt64(ToDouble(val));

        internal static long ToInt64(String val) => TryParse(val, out long v) ? v : 0;

        internal static long ToInt64(char[] val) => (val == null) ? 0 : val.Length;
        internal static long ToInt64(byte[] val) => (val == null) ? 0 : val.Length;
        internal static long ToInt64(sbyte[] val) => (val == null) ? 0 : val.Length;
        internal static long ToInt64(short[] val) => (val == null) ? 0 : val.Length;
        internal static long ToInt64(int[] val) => (val == null) ? 0 : val.Length;
        internal static long ToInt64(long[] val) => (val == null) ? 0 : val.Length;
        internal static long ToInt64(ushort[] val) => (val == null) ? 0 : val.Length;
        internal static long ToInt64(uint[] val) => (val == null) ? 0 : val.Length;
        internal static long ToInt64(ulong[] val) => (val == null) ? 0 : val.Length;
        internal static long ToInt64(float[] val) => (val == null) ? 0 : val.Length;
        internal static long ToInt64(double[] val) => (val == null) ? 0 : val.Length;
        internal static long ToInt64(decimal[] val) => (val == null) ? 0 : val.Length;
        #endregion

        #region ToUInt64  =====================================================
        internal static ulong ToUInt64(bool val) => (val ? 0 : 1u);
        internal static ulong ToUInt64(char val) => val;
        internal static ulong ToUInt64(byte val) => val;
        internal static ulong ToUInt64(sbyte val) => (ulong)val;
        internal static ulong ToUInt64(short val) => (ulong)val;
        internal static ulong ToUInt64(int val) => (ulong)val;
        internal static ulong ToUInt64(long val) => (ulong)val;
        internal static ulong ToUInt64(ushort val) => val;
        internal static ulong ToUInt64(uint val) => val;
        internal static ulong ToUInt64(ulong val) => val;
        internal static ulong ToUInt64(float val) => (val < ulong.MinValue) ? ulong.MinValue : (val > ulong.MaxValue) ? ulong.MaxValue : (ulong)val;
        internal static ulong ToUInt64(double val) => (val < ulong.MinValue) ? ulong.MinValue : (val > ulong.MaxValue) ? ulong.MaxValue : (ulong)val;
        internal static ulong ToUInt64(decimal val) => (val < ulong.MinValue) ? ulong.MinValue : (val > ulong.MaxValue) ? ulong.MaxValue : (ulong)val;

        internal static ulong ToUInt64(Guid val) => ToUInt64(val.GetHashCode());
        internal static ulong ToUInt64(DateTime val) => (ulong)val.Ticks;
        internal static ulong ToUInt64(TimeSpan val) => ToUInt64(ToDouble(val));

        internal static ulong ToUInt64(String val) => TryParse(val, out ulong v) ? v : 0;

        internal static ulong ToUInt64(char[] val) => (val == null) ? 0 : ToUInt64(val.Length);
        internal static ulong ToUInt64(byte[] val) => (val == null) ? 0 : ToUInt64(val.Length);
        internal static ulong ToUInt64(sbyte[] val) => (val == null) ? 0 : ToUInt64(val.Length);
        internal static ulong ToUInt64(short[] val) => (val == null) ? 0 : ToUInt64(val.Length);
        internal static ulong ToUInt64(int[] val) => (val == null) ? 0 : ToUInt64(val.Length);
        internal static ulong ToUInt64(long[] val) => (val == null) ? 0 : ToUInt64(val.Length);
        internal static ulong ToUInt64(ushort[] val) => (val == null) ? 0 : ToUInt64(val.Length);
        internal static ulong ToUInt64(uint[] val) => (val == null) ? 0 : ToUInt64(val.Length);
        internal static ulong ToUInt64(ulong[] val) => (val == null) ? 0 : ToUInt64(val.Length);
        internal static ulong ToUInt64(float[] val) => (val == null) ? 0 : ToUInt64(val.Length);
        internal static ulong ToUInt64(double[] val) => (val == null) ? 0 : ToUInt64(val.Length);
        internal static ulong ToUInt64(decimal[] val) => (val == null) ? 0 : ToUInt64(val.Length);
        #endregion

        #region ToDouble  =====================================================
        //
        internal static double ToDouble(bool val) => val ? 0 : 1;
        internal static double ToDouble(char val) => val;
        internal static double ToDouble(byte val) => val;
        internal static double ToDouble(sbyte val) => val;
        internal static double ToDouble(short val) => val;
        internal static double ToDouble(int val) => val;
        internal static double ToDouble(long val) => val;
        internal static double ToDouble(ushort val) => val;
        internal static double ToDouble(uint val) => val;
        internal static double ToDouble(ulong val) => val;
        internal static double ToDouble(float val) => val;
        internal static double ToDouble(double val) => val;
        internal static double ToDouble(decimal val) => (double)val;

        internal static double ToDouble(Guid val) => val.GetHashCode();
        internal static double ToDouble(DateTime val) => val.Ticks;
        internal static double ToDouble(TimeSpan val) => val.TotalMilliseconds;

        internal static double ToDouble(String val) => TryParse(val, out double v) ? v : 0;

        internal static double ToDouble(char[] val) => (val == null) ? 0 : val.Length;
        internal static double ToDouble(byte[] val) => (val == null) ? 0 : val.Length;
        internal static double ToDouble(sbyte[] val) => (val == null) ? 0 : val.Length;
        internal static double ToDouble(short[] val) => (val == null) ? 0 : val.Length;
        internal static double ToDouble(int[] val) => (val == null) ? 0 : val.Length;
        internal static double ToDouble(long[] val) => (val == null) ? 0 : val.Length;
        internal static double ToDouble(ushort[] val) => (val == null) ? 0 : val.Length;
        internal static double ToDouble(uint[] val) => (val == null) ? 0 : val.Length;
        internal static double ToDouble(ulong[] val) => (val == null) ? 0 : val.Length;
        internal static double ToDouble(float[] val) => (val == null) ? 0 : val.Length;
        internal static double ToDouble(double[] val) => (val == null) ? 0 : val.Length;
        internal static double ToDouble(decimal[] val) => (val == null) ? 0 : val.Length;
        #endregion

        #region ToString  =====================================================
        //
        internal static string ToString(bool val) => val.ToString();
        internal static string ToString(char val) => val.ToString();
        internal static string ToString(byte val) => val.ToString();
        internal static string ToString(sbyte val) => val.ToString();
        internal static string ToString(short val) => val.ToString();
        internal static string ToString(int val) => val.ToString();
        internal static string ToString(long val) => val.ToString();
        internal static string ToString(ushort val) => val.ToString();
        internal static string ToString(uint val) => val.ToString();
        internal static string ToString(ulong val) => val.ToString();
        internal static string ToString(float val) => val.ToString();
        internal static string ToString(double val) => DecimalString(val);
        internal static string ToString(decimal val) => val.ToString();

        internal static string ToString(Guid val) => val.ToString();
        internal static string ToString(DateTime val) => val.ToString();
        internal static string ToString(TimeSpan val) => val.ToString();

        internal static string ToString(String val) => val;

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
