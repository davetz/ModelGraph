using System;
using System.Collections.Generic;

namespace ModelGraph.Internals
{
    internal abstract partial class Value
    {
        /// <summary>
        /// Get the value group for the given value type
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
        /// Create a value of the given type
        /// </summary>
        static internal Value Create(ValType type, int capacity = 0)
        {
            int index = (int)type;
            return (index < _valCreate.Length) ? _valCreate[index](capacity) : Chef.ValuesInvalid; 
        }
        static Func<int, Value>[] _valCreate = new Func<int, Value>[]
        {
            (c) => new BoolValue(new ValueDictionary<bool>(c)), // 0 Bool
            (c) => new CharValue(new ValueDictionary<char>(c)), // 1 Char
            (c) => new ByteValue(new ValueDictionary<byte>(c)), // 2 Bype
            (c) => new SByteValue(new ValueDictionary<sbyte>(c)), // 3 SByte
            (c) => new Int16Value(new ValueDictionary<short>(c)), // 4 Int16
            (c) => new UInt16Value(new ValueDictionary<ushort>(c)), // 5 UInt16
            (c) => new Int32Value(new ValueDictionary<int>(c)), // 6 Int32
            (c) => new UInt32Value(new ValueDictionary<uint>(c)), // 7 UInt32
            (c) => new Int64Value(new ValueDictionary<long>(c)), // 8 Int64
            (c) => new UInt64Value(new ValueDictionary<ulong>(c)), // 9 UInt64
            (c) => new SingleValue(new ValueDictionary<float>(c)), // 10 Single
            (c) => new DoubleValue(new ValueDictionary<double>(c)), // 11 Double
            (c) => new DecimalValue(new ValueDictionary<decimal>(c)), // 12 Decimal
            (c) => new DateTimeValue(new ValueDictionary<DateTime>(c)), // 13 DateTime
            (c) => new StringValue(new ValueDictionary<string>(c)), // 14 String

            (c) => new BoolArrayValue(new ValueDictionary<bool[]>(c)), // 15 BoolArray
            (c) => new CharArrayValue(new ValueDictionary<char[]>(c)), // 16 CharArray
            (c) => new ByteArrayValue(new ValueDictionary<byte[]>(c)), // 17 BypeArray
            (c) => new SByteArrayValue(new ValueDictionary<sbyte[]>(c)), // 18 SByteArray
            (c) => new Int16ArrayValue(new ValueDictionary<short[]>(c)), // 19 Int16Array
            (c) => new UInt16ArrayValue(new ValueDictionary<ushort[]>(c)), // 20 UInt16Array
            (c) => new Int32ArrayValue(new ValueDictionary<int[]>(c)), // 21 Int32Array
            (c) => new UInt32ArrayValue(new ValueDictionary<uint[]>(c)), // 22 UInt32Array
            (c) => new Int64ArrayValue(new ValueDictionary<long[]>(c)), // 23 Int64Array
            (c) => new UInt64ArrayValue(new ValueDictionary<ulong[]>(c)), // 24 UInt64Array
            (c) => new SingleArrayValue(new ValueDictionary<float[]>(c)), // 25 SingleArray
            (c) => new DoubleArrayValue(new ValueDictionary<double[]>(c)), // 26 DoubleArray
            (c) => new DecimalArrayValue(new ValueDictionary<decimal[]>(c)), // 27 DecimalArray
            (c) => new DateTimeArrayValue(new ValueDictionary<DateTime[]>(c)), // 28 DateTimeArray
            (c) => new StringArrayValue(new ValueDictionary<string[]>(c)), // 29 StringArray
        };
    }
}
