using System;
using System.Collections.Generic;
using Windows.Storage.Streams;
using ModelGraphSTD;

namespace RepositoryUWP
{
    public partial class RepositoryStorageFile
    {
        void WriteValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var vt = (int)cx.Value.ValType;
            if (vt > _writeValueDictionary.Length) throw new ArgumentException("Invalid Value Type");

            _writeValueDictionary[vt](w, cx, itemIndex);
        }

        #region _writeValueDictionary  ========================================
        static Action<DataWriter, ColumnX, Dictionary<Item, int>>[] _writeValueDictionary =
        {
            WriteBoolValueDictionary, // 0
            WriteBoolArrayValueDictionary, // 1

            WriteCharValueDictionary, // 2
            WriteCharArrayValueDictionary, // 3

            WriteByteValueDictionary, // 4
            WriteByteArrayValueDictionary, // 5

            WriteSByteValueDictionary, // 6
            WriteSByteArrayValueDictionary, // 7

            WriteInt16ValueDictionary, // 8
            WriteInt16ArrayValueDictionary, // 9

            WriteUInt16ValueDictionary, // 10
            WriteUInt16ArrayValueDictionary, // 11

            WriteInt32ValueDictionary, // 12
            WriteInt32ArrayValueDictionary, // 13

            WriteUInt32ValueDictionary, // 14
            WriteUInt32ArrayValueDictionary, // 15

            WriteInt64ValueDictionary, // 16
            WriteInt64ArrayValueDictionary, // 17

            WriteUInt64ValueDictionary, // 18
            WriteUInt64ArrayValueDictionary, // 19

            WriteSingleValueDictionary, // 20
            WriteSingleArrayValueDictionary, // 21

            WriteDoubleValueDictionary, // 22
            WriteDoubleArrayValueDictionary, // 23

            WriteDecimalValueDictionary, // 24
            WriteDecimalArrayValueDictionary, // 25

            WriteDateTimeValueDictionary, //26
            WriteDateTimeArrayValueDictionary, // 27

            WriteStringValueDictionary, // 28
            WriteStringArrayValueDictionary, // 29
        };
        #endregion

        #region WriteBoolValueDictionary  =====================================
        static void WriteBoolValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((BoolValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteBoolean(d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteBoolean(val);
                }
            }
        }
        #endregion

        #region WriteCharValueDictionary  =====================================
        static void WriteCharValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((CharValue)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteInt16((short)d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteInt16((short)val);
                }
            }
        }
        #endregion

        #region WriteByteValueDictionary  =====================================
        static void WriteByteValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((ByteValue)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteByte(d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteByte(val);
                }
            }
        }
        #endregion

        #region WriteSByteValueDictionary  ====================================
        static void WriteSByteValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((SByteValue)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteByte((byte)d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteByte((byte)val);
                }
            }
        }
        #endregion

        #region WriteInt16ValueDictionary  ====================================
        static void WriteInt16ValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((Int16Value)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteInt16(d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteInt16(val);
                }
            }
        }
        #endregion

        #region WriteUInt16ValueDictionary  ===================================
        static void WriteUInt16ValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((UInt16Value)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteUInt16(d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteUInt16(val);
                }
            }
        }
        #endregion

        #region WriteInt32ValueDictionary  ====================================
        static void WriteInt32ValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((Int32Value)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteInt32(d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteInt32(val);
                }
            }
        }
        #endregion

        #region WriteUInt32ValueDictionary  ===================================
        static void WriteUInt32ValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((UInt32Value)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteUInt32(d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteUInt32(val);
                }
            }
        }
        #endregion

        #region WriteInt64ValueDictionary  ====================================
        static void WriteInt64ValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((Int32Value)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteInt64(d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteInt64(val);
                }
            }
        }
        #endregion

        #region WriteUInt64ValueDictionary  ===================================
        static void WriteUInt64ValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((UInt64Value)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteUInt64(d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteUInt64(val);
                }
            }
        }
        #endregion

        #region WriteSingleValueDictionary  ===================================
        static void WriteSingleValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((SingleValue)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteSingle(d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteSingle(val);
                }
            }
        }
        #endregion

        #region WriteDoubleValueDictionary  ===================================
        static void WriteDoubleValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((DoubleValue)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteDouble(d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteDouble(val);
                }
            }
        }
        #endregion

        #region WriteDecimalValueDictionary  ==================================
        static void WriteDecimalValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((DecimalValue)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                WriteString(w, d.DefaultValue.ToString());

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    WriteString(w, val.ToString());
                }
            }
        }
        #endregion

        #region WriteDateTimeValueDictionary  =================================
        static void WriteDateTimeValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((DateTimeValue)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                w.WriteDateTime(d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    w.WriteDateTime(val);
                }
            }
        }
        #endregion

        #region WriteStringValueDictionary  ===================================
        static void WriteStringValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((StringValue)cx.Value).ValueDictionary;
            var N = d.Count;
            w.WriteInt32(N);

            if (N > 0)
            {
                WriteString(w, d.DefaultValue);

                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    var val = vals[i];

                    w.WriteInt32(itemIndex[key]);
                    WriteString(w, val);
                }
            }
        }
        #endregion


        #region WriteBoolArrayValueDictionary  ================================
        static void WriteBoolArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((BoolArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteBoolean(v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteCharArrayValueDictionary  ================================
        static void WriteCharArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((CharArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteInt16((short)v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteByteArrayValueDictionary  ================================
        static void WriteByteArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((ByteArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteByte(v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteSByteArrayValueDictionary  ===============================
        static void WriteSByteArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((SByteArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteByte((byte)v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteInt16ArrayValueDictionary  ===============================
        static void WriteInt16ArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((Int16ArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteInt16(v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteUInt16ArrayValueDictionary  ==============================
        static void WriteUInt16ArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((UInt16ArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteUInt16(v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteInt32ArrayValueDictionary  ===============================
        static void WriteInt32ArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((Int32ArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteInt32(v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteUInt32ArrayValueDictionary  ==============================
        static void WriteUInt32ArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((UInt32ArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteUInt32(v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteInt64ArrayValueDictionary  ===============================
        static void WriteInt64ArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((Int64ArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteInt64(v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteUInt64ArrayValueDictionary  ==============================
        static void WriteUInt64ArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((UInt64ArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteUInt64(v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteSingleArrayValueDictionary  ==============================
        static void WriteSingleArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((SingleArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteSingle(v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteDoubleArrayValueDictionary  ==============================
        static void WriteDoubleArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((DoubleArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteDouble(v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteDecimalArrayValueDictionary  =============================
        static void WriteDecimalArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((DecimalArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            WriteString(w, v.ToString());
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteDateTimeArrayValueDictionary  ============================
        static void WriteDateTimeArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((DateTimeArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            w.WriteDateTime(v);
                        }
                    }
                }
            }
        }
        #endregion

        #region WriteStringArrayValueDictionary  ==============================
        static void WriteStringArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((StringArrayValue)cx.Value).ValueDictionary;
            var N = d.Count;

            w.WriteInt32(N);

            if (N > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                for (int i = 0; i < N; i++)
                {
                    var key = keys[i];
                    w.WriteInt32(itemIndex[key]);

                    var val = vals[i];
                    var len = (val == null) ? 0 : val.Length;
                    w.WriteUInt16((ushort)len);

                    if (len > 0)
                    {
                        foreach (var v in val)
                        {
                            WriteString(w, v);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
