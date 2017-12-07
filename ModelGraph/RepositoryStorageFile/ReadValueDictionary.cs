using System;
using System.Collections.Generic;
using Windows.Storage.Streams;
using ModelGraphSTD;

namespace RepositoryUWP
{
    public partial class RepositoryStorageFile
    {
        void ReadValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var vt = (int)t;
            if (vt > _readValueDictionary.Length) throw new ArgumentException("Invalid Value Type");

            _readValueDictionary[vt](r, t, cx, items);
        }

        #region _readValueDictionary  =========================================
        static Action<DataReader, ValType, ColumnX, Item[]>[] _readValueDictionary =
        {
            ReadBoolValueDictionary, // 0
            ReadCharValueDictionary, // 1

            ReadByteValueDictionary, // 2
            ReadSByteValueDictionary, //3

            ReadInt16ValueDictionary, // 4
            ReadUInt16ValueDictionary, // 5

            ReadInt32ValueDictionary, // 6
            ReadUInt32ValueDictionary, // 7

            ReadInt64ValueDictionary, // 8
            ReadUInt64ValueDictionary, // 9

            ReadSingleValueDictionary, // 10
            ReadDoubleValueDictionary, // 11

            ReadDecimalValueDictionary, // 12
            ReadDateTimeValueDictionary, //13

            ReadStringValueDictionary, // 14

            ReadBoolArrayValueDictionary, // 15
            ReadCharArrayValueDictionary, // 16

            ReadByteArrayValueDictionary, // 17
            ReadSByteArrayValueDictionary, // 18

            ReadInt16ArrayValueDictionary, // 19
            ReadUInt16ArrayValueDictionary, // 20

            ReadInt32ArrayValueDictionary, // 21
            ReadUInt32ArrayValueDictionary, // 22

            ReadInt64ArrayValueDictionary, // 23
            ReadUInt64ArrayValueDictionary, // 24

            ReadSingleArrayValueDictionary, // 25
            ReadDoubleArrayValueDictionary, // 26

            ReadDecimalArrayValueDictionary, // 27
            ReadDateTimeArrayValueDictionary, // 28

            ReadStringArrayValueDictionary, // 29
        };
        #endregion

        #region ReadBoolValueDictionary  =====================================
        static void ReadBoolValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = r.ReadBoolean();
                var dic = new ValueDictionary<bool>(count, def);
                cx.Value = new BoolValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = r.ReadBoolean();
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadCharValueDictionary  =====================================
        static void ReadCharValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = (char)r.ReadInt16();
                var dic = new ValueDictionary<char>(count, def);
                cx.Value = new CharValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = (char)r.ReadInt16();
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadByteValueDictionary  =====================================
        static void ReadByteValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = r.ReadByte();
                var dic = new ValueDictionary<byte>(count, def);
                cx.Value = new ByteValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = r.ReadByte();
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadSByteValueDictionary  ====================================
        static void ReadSByteValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = r.ReadByte();
                var dic = new ValueDictionary<sbyte>(count, def);
                cx.Value = new SByteValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = (sbyte)r.ReadByte();
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadInt16ValueDictionary  ====================================
        static void ReadInt16ValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = r.ReadInt16();
                var dic = new ValueDictionary<short>(count, def);
                cx.Value = new Int16Value(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = r.ReadInt16();
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadUInt16ValueDictionary  ===================================
        static void ReadUInt16ValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = r.ReadUInt16();
                var dic = new ValueDictionary<ushort>(count, def);
                cx.Value = new UInt16Value(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = r.ReadUInt16();
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadInt32ValueDictionary  ====================================
        static void ReadInt32ValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = r.ReadInt32();
                var dic = new ValueDictionary<int>(count, def);
                cx.Value = new Int32Value(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = r.ReadInt32();
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadUInt32ValueDictionary  ===================================
        static void ReadUInt32ValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = r.ReadUInt32();
                var dic = new ValueDictionary<uint>(count, def);
                cx.Value = new UInt32Value(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = r.ReadUInt32();
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadInt64ValueDictionary  ====================================
        static void ReadInt64ValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = r.ReadInt64();
                var dic = new ValueDictionary<long>(count, def);
                cx.Value = new Int64Value(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = r.ReadInt64();
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadUInt64ValueDictionary  ===================================
        static void ReadUInt64ValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = r.ReadUInt64();
                var dic = new ValueDictionary<ulong>(count, def);
                cx.Value = new UInt64Value(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = r.ReadUInt64();
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadSingleValueDictionary  ===================================
        static void ReadSingleValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = r.ReadSingle();
                var dic = new ValueDictionary<float>(count, def);
                cx.Value = new SingleValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = r.ReadSingle();
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadDoubleValueDictionary  ===================================
        static void ReadDoubleValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = r.ReadDouble();
                var dic = new ValueDictionary<double>(count, def);
                cx.Value = new DoubleValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = r.ReadDouble();
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadDecimalValueDictionary  ==================================
        static void ReadDecimalValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var str = ReadString(r);
                var def = decimal.Parse(str);
                var dic = new ValueDictionary<decimal>(count, def);
                cx.Value = new DecimalValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    str = ReadString(r);
                    var val = decimal.Parse(str);
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion

        #region ReadDateTimeValueDictionary  =================================
        static void ReadDateTimeValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = r.ReadDateTime();
                var dic = new ValueDictionary<DateTime>(count, def.DateTime);
                cx.Value = new DateTimeValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = r.ReadDateTime();
                    dic.LoadValue(rx, val.DateTime);
                }
            }
        }
        #endregion

        #region ReadStringValueDictionary  ===================================
        static void ReadStringValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var def = ReadString(r);
                if (def.Length == 0) def = null;
                var dic = new ValueDictionary<string>(count, def);
                cx.Value = new StringValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var val = ReadString(r);
                    dic.LoadValue(rx, val);
                }
            }
        }
        #endregion


        #region ReadBoolArrayValueDictionary  ================================
        static void ReadBoolArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<bool[]>(count, null);
                cx.Value = new BoolArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new bool[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            vals[j] = r.ReadBoolean();
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadCharArrayValueDictionary  ================================
        static void ReadCharArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<char[]>(count, null);
                cx.Value = new CharArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new char[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            vals[j] = (char)r.ReadInt16();
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadByteArrayValueDictionary  ================================
        static void ReadByteArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<byte[]>(count, null);
                cx.Value = new ByteArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new byte[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            vals[j] = r.ReadByte();
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadSByteArrayValueDictionary  ===============================
        static void ReadSByteArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<sbyte[]>(count, null);
                cx.Value = new SByteArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new sbyte[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            vals[j] = (sbyte)r.ReadByte();
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadInt16ArrayValueDictionary  ===============================
        static void ReadInt16ArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<short[]>(count, null);
                cx.Value = new Int16ArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new short[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            vals[j] = r.ReadInt16();
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadUInt16ArrayValueDictionary  ==============================
        static void ReadUInt16ArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<ushort[]>(count, null);
                cx.Value = new UInt16ArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new ushort[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            vals[j] = r.ReadUInt16();
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadInt32ArrayValueDictionary  ===============================
        static void ReadInt32ArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<int[]>(count, null);
                cx.Value = new Int32ArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new int[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            vals[j] = r.ReadInt32();
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadUInt32ArrayValueDictionary  ==============================
        static void ReadUInt32ArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<uint[]>(count, null);
                cx.Value = new UInt32ArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new uint[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            vals[j] = r.ReadUInt32();
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadInt64ArrayValueDictionary  ===============================
        static void ReadInt64ArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<long[]>(count, null);
                cx.Value = new Int64ArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new long[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            vals[j] = r.ReadInt64();
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadUInt64ArrayValueDictionary  ==============================
        static void ReadUInt64ArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<ulong[]>(count, null);
                cx.Value = new UInt64ArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new ulong[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            vals[j] = r.ReadUInt64();
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadSingleArrayValueDictionary  ==============================
        static void ReadSingleArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<float[]>(count, null);
                cx.Value = new SingleArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new float[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            vals[j] = r.ReadSingle();
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadDoubleArrayValueDictionary  ==============================
        static void ReadDoubleArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<double[]>(count, null);
                cx.Value = new DoubleArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new double[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            vals[j] = r.ReadDouble();
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadDecimalArrayValueDictionary  =============================
        static void ReadDecimalArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<decimal[]>(count, null);
                cx.Value = new DecimalArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new decimal[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            var str = ReadString(r);
                            var val = Decimal.Parse(str);
                            vals[j] = val;
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadDateTimeArrayValueDictionary  ============================
        static void ReadDateTimeArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<DateTime[]>(count, null);
                cx.Value = new DateTimeArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new DateTime[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            var val = r.ReadDateTime();
                            vals[j] = val.DateTime;
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion

        #region ReadStringArrayValueDictionary  ==============================
        static void ReadStringArrayValueDictionary(DataReader r, ValType t, ColumnX cx, Item[] items)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid row count {count}");

            if (count == 0)
            {
                cx.Value = Value.Create(t);
            }
            else
            {
                var dic = new ValueDictionary<string[]>(count, null);
                cx.Value = new StringArrayValue(dic);

                for (int i = 0; i < count; i++)
                {
                    var inx = r.ReadInt32();
                    if (inx < 0 || inx >= items.Length) throw new Exception($"Invalid row index {inx}");

                    var rx = items[inx];
                    if (rx == null) throw new Exception($"Column row is null, index {inx}");

                    var len = r.ReadUInt16();

                    var vals = new string[len];
                    if (len > 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            var val = ReadString(r);
                            vals[j] = val;
                        }
                    }
                    dic.LoadValue(rx, vals);
                }
            }
        }
        #endregion
    }
}
