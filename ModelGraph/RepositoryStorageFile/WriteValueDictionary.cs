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
            WriteCharValueDictionary, // 1

            WriteByteValueDictionary, // 2
            WriteSByteValueDictionary, //3

            WriteInt16ValueDictionary, // 4
            WriteUInt16ValueDictionary, // 5

            WriteInt32ValueDictionary, // 6
            WriteUInt32ValueDictionary, // 7

            WriteInt64ValueDictionary, // 8
            WriteUInt64ValueDictionary, // 9

            WriteSingleValueDictionary, // 10
            WriteDoubleValueDictionary, // 11

            WriteDecimalValueDictionary, // 12
            WriteDateTimeValueDictionary, //13

            WriteStringValueDictionary, // 14

            WriteBoolArrayValueDictionary, // 15
            WriteCharArrayValueDictionary, // 16

            WriteByteArrayValueDictionary, // 17
            WriteSByteArrayValueDictionary, // 18

            WriteInt16ArrayValueDictionary, // 19
            WriteUInt16ArrayValueDictionary, // 20

            WriteInt32ArrayValueDictionary, // 21
            WriteUInt32ArrayValueDictionary, // 22

            WriteInt64ArrayValueDictionary, // 23
            WriteUInt64ArrayValueDictionary, // 24

            WriteSingleArrayValueDictionary, // 25
            WriteDoubleArrayValueDictionary, // 26

            WriteDecimalArrayValueDictionary, // 27
            WriteDateTimeArrayValueDictionary, // 28

            WriteStringArrayValueDictionary, // 29
        };
        #endregion

        #region WriteBoolValueDictionary  =====================================
        static void WriteBoolValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((BoolValue)cx.Value).ValueDictionary;

            w.WriteBoolean(d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteBoolean(val);
                }
            }
        }
        #endregion

        #region WriteCharValueDictionary  =====================================
        static void WriteCharValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((CharValue)cx.Value).ValueDictionary;

            w.WriteInt16((short)d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteInt16((short)val);
                }
            }
        }
        #endregion

        #region WriteByteValueDictionary  =====================================
        static void WriteByteValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((ByteValue)cx.Value).ValueDictionary;

            w.WriteByte(d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteByte(val);
                }
            }
        }
        #endregion

        #region WriteSByteValueDictionary  ====================================
        static void WriteSByteValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((SByteValue)cx.Value).ValueDictionary;

            w.WriteByte((byte)d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteByte((byte)val);
                }
            }
        }
        #endregion

        #region WriteInt16ValueDictionary  ====================================
        static void WriteInt16ValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((Int16Value)cx.Value).ValueDictionary;

            w.WriteInt16(d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteInt16(val);
                }
            }
        }
        #endregion

        #region WriteUInt16ValueDictionary  ===================================
        static void WriteUInt16ValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((UInt16Value)cx.Value).ValueDictionary;

            w.WriteUInt16(d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteUInt16(val);
                }
            }
        }
        #endregion

        #region WriteInt32ValueDictionary  ====================================
        static void WriteInt32ValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((Int32Value)cx.Value).ValueDictionary;

            w.WriteInt32(d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteInt32(val);
                }
            }
        }
        #endregion

        #region WriteUInt32ValueDictionary  ===================================
        static void WriteUInt32ValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((UInt32Value)cx.Value).ValueDictionary;

            w.WriteUInt32(d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteUInt32(val);
                }
            }
        }
        #endregion

        #region WriteInt64ValueDictionary  ====================================
        static void WriteInt64ValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((Int32Value)cx.Value).ValueDictionary;

            w.WriteInt64(d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteInt64(val);
                }
            }
        }
        #endregion

        #region WriteUInt64ValueDictionary  ===================================
        static void WriteUInt64ValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((UInt64Value)cx.Value).ValueDictionary;

            w.WriteUInt64(d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteUInt64(val);
                }
            }
        }
        #endregion

        #region WriteSingleValueDictionary  ===================================
        static void WriteSingleValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((SingleValue)cx.Value).ValueDictionary;

            w.WriteSingle(d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteSingle(val);
                }
            }
        }
        #endregion

        #region WriteDoubleValueDictionary  ===================================
        static void WriteDoubleValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((DoubleValue)cx.Value).ValueDictionary;

            w.WriteDouble(d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteDouble(val);
                }
            }
        }
        #endregion

        #region WriteDecimalValueDictionary  ==================================
        static void WriteDecimalValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((DecimalValue)cx.Value).ValueDictionary;

            WriteString(w, d.DefaultValue.ToString());
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    WriteString(w, val.ToString());
                }
            }
        }
        #endregion

        #region WriteDateTimeValueDictionary  =================================
        static void WriteDateTimeValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((DateTimeValue)cx.Value).ValueDictionary;

            w.WriteDateTime(d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    w.WriteDateTime(val);
                }
            }
        }
        #endregion

        #region WriteStringValueDictionary  ===================================
        static void WriteStringValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((StringValue)cx.Value).ValueDictionary;
            WriteString(w, d.DefaultValue);
            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
                    WriteString(w, val);
                }
            }
        }
        #endregion


        #region WriteBoolArrayValueDictionary  ================================
        static void WriteBoolArrayValueDictionary(DataWriter w, ColumnX cx, Dictionary<Item, int> itemIndex)
        {
            var d = ((BoolArrayValue)cx.Value).ValueDictionary;

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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

            w.WriteInt32(d.Count);

            if (d.Count > 0)
            {
                var keys = d.GetKeys();
                var vals = d.GetValues();

                foreach (var key in keys)
                {
                    w.WriteInt32(itemIndex[key]);
                }
                foreach (var val in vals)
                {
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
