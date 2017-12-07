using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal abstract partial class Value
    {
        /// <summary>
        /// Get the value compatibility group for the given value type
        /// </summary>
        static internal ValGroup GetValGroup(ValType type)
        {
            var index = (int)type;
            return (index < _valGroup.Length) ? _valGroup[index] : ValGroup.None;
        }
        static ValGroup[] _valGroup = new ValGroup[]
        {
            ValGroup.Bool, // 0 Bool
            ValGroup.Long, // 1 Char
            ValGroup.Long, // 2 Bype
            ValGroup.Long, // 3 SByte
            ValGroup.Long, // 4 Int16
            ValGroup.Long, // 5 UInt16
            ValGroup.Long, // 6 Int32
            ValGroup.Long, // 7 UInt32
            ValGroup.Long, // 8 Int64
            ValGroup.Long, // 9 UInt64
            ValGroup.Double, // 10 Single
            ValGroup.Double, // 11 Double
            ValGroup.Double, // 12 Decimal
            ValGroup.DateTime, // 13 DateTime
            ValGroup.String, // 14 String

            ValGroup.BoolArray, // 15 BoolArry
            ValGroup.LongArray, // 16 CharArray
            ValGroup.LongArray, // 17 ByteArray
            ValGroup.LongArray, // 18 SByteArray
            ValGroup.LongArray, // 19 Int16Array
            ValGroup.LongArray, // 20 UInt15Array
            ValGroup.LongArray, // 21 Int32Array
            ValGroup.LongArray, // 22 UInt32Array
            ValGroup.LongArray, // 23 Int64Array
            ValGroup.LongArray, // 24 UInt64Array
            ValGroup.DoubleArray, // 25 SingleArray
            ValGroup.DoubleArray, // 26 DoubleArray
            ValGroup.DoubleArray, // 27 DecimalArray
            ValGroup.DateTimeArray, // 28 DateTimeArray
            ValGroup.StringArray, // 29 StringArray
        };

        /// <summary>
        /// Create a value of the given type, injecting a ValueDictionary<T> IValueStore
        /// </summary>
        static internal Value Create(ValType type, int capacity = 0, string defaultValue = null)
        {
            int index = (int)type;
            return (index < _valCreate.Length) ? _valCreate[index](capacity, defaultValue) : Chef.ValuesInvalid; 
        }
        static Func<int, string, Value>[] _valCreate = new Func<int, string, Value>[]
        {
            (c,s) => new BoolValue(new ValueDictionary<bool>(c, (bool.TryParse(s, out bool v)) ? v : default(bool))), // 0 Bool
            (c,s) => new CharValue(new ValueDictionary<char>(c, (char.TryParse(s, out char v)) ? v : default(char))), // 1 Char
            (c,s) => new ByteValue(new ValueDictionary<byte>(c, (byte.TryParse(s, out byte v)) ? v : default(byte))), // 2 Bype
            (c,s) => new SByteValue(new ValueDictionary<sbyte>(c, (sbyte.TryParse(s, out sbyte v)) ? v : default(sbyte))), // 3 SByte
            (c,s) => new Int16Value(new ValueDictionary<short>(c, (short.TryParse(s, out short v)) ? v : default(short))), // 4 Int16
            (c,s) => new UInt16Value(new ValueDictionary<ushort>(c, (ushort.TryParse(s, out ushort v)) ? v : default(ushort))), // 5 UInt16
            (c,s) => new Int32Value(new ValueDictionary<int>(c, (int.TryParse(s, out int v)) ? v : default(int))), // 6 Int32
            (c,s) => new UInt32Value(new ValueDictionary<uint>(c, (uint.TryParse(s, out uint v)) ? v : default(uint))), // 7 UInt32
            (c,s) => new Int64Value(new ValueDictionary<long>(c, (long.TryParse(s, out long v)) ? v : default(long))), // 8 Int64
            (c,s) => new UInt64Value(new ValueDictionary<ulong>(c, (ulong.TryParse(s, out ulong v)) ? v : default(ulong))), // 9 UInt64
            (c,s) => new SingleValue(new ValueDictionary<float>(c, (float.TryParse(s, out float v)) ? v : default(float))), // 10 Single
            (c,s) => new DoubleValue(new ValueDictionary<double>(c, (double.TryParse(s, out double v)) ? v : default(double))), // 11 Double
            (c,s) => new DecimalValue(new ValueDictionary<decimal>(c, (decimal.TryParse(s, out decimal v)) ? v : default(decimal))), // 12 Decimal
            (c,s) => new DateTimeValue(new ValueDictionary<DateTime>(c, (DateTime.TryParse(s, out DateTime v)) ? v : default(DateTime))), // 13 DateTime
            (c,s) => new StringValue(new ValueDictionary<string>(c, s)), // 14 String

            (c,s) => new BoolArrayValue(new ValueDictionary<bool[]>(c, null)), // 15 BoolArray
            (c,s) => new CharArrayValue(new ValueDictionary<char[]>(c, null)), // 16 CharArray
            (c,s) => new ByteArrayValue(new ValueDictionary<byte[]>(c, null)), // 17 BypeArray
            (c,s) => new SByteArrayValue(new ValueDictionary<sbyte[]>(c, null)), // 18 SByteArray
            (c,s) => new Int16ArrayValue(new ValueDictionary<short[]>(c, null)), // 19 Int16Array
            (c,s) => new UInt16ArrayValue(new ValueDictionary<ushort[]>(c, null)), // 20 UInt16Array
            (c,s) => new Int32ArrayValue(new ValueDictionary<int[]>(c, null)), // 21 Int32Array
            (c,s) => new UInt32ArrayValue(new ValueDictionary<uint[]>(c, null)), // 22 UInt32Array
            (c,s) => new Int64ArrayValue(new ValueDictionary<long[]>(c, null)), // 23 Int64Array
            (c,s) => new UInt64ArrayValue(new ValueDictionary<ulong[]>(c, null)), // 24 UInt64Array
            (c,s) => new SingleArrayValue(new ValueDictionary<float[]>(c, null)), // 25 SingleArray
            (c,s) => new DoubleArrayValue(new ValueDictionary<double[]>(c, null)), // 26 DoubleArray
            (c,s) => new DecimalArrayValue(new ValueDictionary<decimal[]>(c, null)), // 27 DecimalArray
            (c,s) => new DateTimeArrayValue(new ValueDictionary<DateTime[]>(c, null)), // 28 DateTimeArray
            (c,s) => new StringArrayValue(new ValueDictionary<string[]>(c, null)), // 29 StringArray
        };
        static bool ToBool(string s) => (bool.TryParse(s, out bool v)) ? v : false;
    }
}
