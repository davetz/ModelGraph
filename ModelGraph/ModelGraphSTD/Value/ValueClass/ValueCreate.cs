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
            ValGroup.BoolArray, // 1 BoolArry

            ValGroup.Long, // 2 Char
            ValGroup.LongArray, // 3 CharArray

            ValGroup.Long, // 4 Bype
            ValGroup.LongArray, // 5 ByteArray

            ValGroup.Long, // 6 SByte
            ValGroup.LongArray, // 7 SByteArray

            ValGroup.Long, // 8 Int16
            ValGroup.LongArray, // 9 Int16Array

            ValGroup.Long, // 10 UInt16
            ValGroup.LongArray, // 11 UInt15Array

            ValGroup.Long, // 12 Int32
            ValGroup.LongArray, // 13 Int32Array

            ValGroup.Long, // 14 UInt32
            ValGroup.LongArray, // 15 UInt32Array

            ValGroup.Long, // 16 Int64
            ValGroup.LongArray, // 17 Int64Array

            ValGroup.Long, // 18 UInt64
            ValGroup.LongArray, // 19 UInt64Array

            ValGroup.Double, // 20 Single
            ValGroup.DoubleArray, // 21 SingleArray

            ValGroup.Double, // 22 Double
            ValGroup.DoubleArray, // 23 DoubleArray

            ValGroup.Double, // 24 Decimal
            ValGroup.DoubleArray, // 25 DecimalArray

            ValGroup.DateTime, // 26 DateTime
            ValGroup.DateTimeArray, // 27 DateTimeArray

            ValGroup.String, // 28 String
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
            (c,s) => new BoolArrayValue(new ValueDictionary<bool[]>(c, null)), // 1 BoolArray

            (c,s) => new CharValue(new ValueDictionary<char>(c, (char.TryParse(s, out char v)) ? v : default(char))), // 2 Char
            (c,s) => new CharArrayValue(new ValueDictionary<char[]>(c, null)), // 3 CharArray

            (c,s) => new ByteValue(new ValueDictionary<byte>(c, (byte.TryParse(s, out byte v)) ? v : default(byte))), // 4 Bype
            (c,s) => new ByteArrayValue(new ValueDictionary<byte[]>(c, null)), // 5 BypeArray

            (c,s) => new SByteValue(new ValueDictionary<sbyte>(c, (sbyte.TryParse(s, out sbyte v)) ? v : default(sbyte))), // 6 SByte
            (c,s) => new SByteArrayValue(new ValueDictionary<sbyte[]>(c, null)), // 7 SByteArray

            (c,s) => new Int16Value(new ValueDictionary<short>(c, (short.TryParse(s, out short v)) ? v : default(short))), // 8 Int16
            (c,s) => new Int16ArrayValue(new ValueDictionary<short[]>(c, null)), // 9 Int16Array

            (c,s) => new UInt16Value(new ValueDictionary<ushort>(c, (ushort.TryParse(s, out ushort v)) ? v : default(ushort))), // 10 UInt16
            (c,s) => new UInt16ArrayValue(new ValueDictionary<ushort[]>(c, null)), // 11 UInt16Array

            (c,s) => new Int32Value(new ValueDictionary<int>(c, (int.TryParse(s, out int v)) ? v : default(int))), // 12 Int32
            (c,s) => new Int32ArrayValue(new ValueDictionary<int[]>(c, null)), // 13 Int32Array

            (c,s) => new UInt32Value(new ValueDictionary<uint>(c, (uint.TryParse(s, out uint v)) ? v : default(uint))), // 14 UInt32
            (c,s) => new UInt32ArrayValue(new ValueDictionary<uint[]>(c, null)), // 15 UInt32Array

            (c,s) => new Int64Value(new ValueDictionary<Int64>(c, (Int64.TryParse(s, out Int64 v)) ? v : default(Int64))), // 16 Int64
            (c,s) => new Int64ArrayValue(new ValueDictionary<Int64[]>(c, null)), // 17 Int64Array

            (c,s) => new UInt64Value(new ValueDictionary<ulong>(c, (ulong.TryParse(s, out ulong v)) ? v : default(ulong))), // 18 UInt64
            (c,s) => new UInt64ArrayValue(new ValueDictionary<ulong[]>(c, null)), // 19 UInt64Array

            (c,s) => new SingleValue(new ValueDictionary<float>(c, (float.TryParse(s, out float v)) ? v : default(float))), // 20 Single
            (c,s) => new SingleArrayValue(new ValueDictionary<float[]>(c, null)), // 21 SingleArray

            (c,s) => new DoubleValue(new ValueDictionary<double>(c, (double.TryParse(s, out double v)) ? v : default(double))), // 22 Double
            (c,s) => new DoubleArrayValue(new ValueDictionary<double[]>(c, null)), // 23 DoubleArray

            (c,s) => new DecimalValue(new ValueDictionary<decimal>(c, (decimal.TryParse(s, out decimal v)) ? v : default(decimal))), // 24 Decimal
            (c,s) => new DecimalArrayValue(new ValueDictionary<decimal[]>(c, null)), // 25 DecimalArray

            (c,s) => new DateTimeValue(new ValueDictionary<DateTime>(c, (DateTime.TryParse(s, out DateTime v)) ? v : default(DateTime))), // 26 DateTime
            (c,s) => new DateTimeArrayValue(new ValueDictionary<DateTime[]>(c, null)), // 27 DateTimeArray

            (c,s) => new StringValue(new ValueDictionary<string>(c, s)), // 28 String
            (c,s) => new StringArrayValue(new ValueDictionary<string[]>(c, null)), // 29 StringArray
        };
    }
}
