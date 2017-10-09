using System;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Collections.Generic;
using System.Diagnostics;

namespace ModelGraphLibrary
{/*

 */
    public partial class Chef
    {
        static Guid _fileFormat_1 = new Guid("D8CA7983-98BC-49CC-B821-432BDA6BADE6");
        static Guid _fileFormat_2 = new Guid("7DD885AE-7004-4ECC-9B9F-B84330326129");

        #region TextFiles  ====================================================

        #region WriteModelTextFile  ===========================================
        private async void WriteModelTextFile(StorageFile sf)
        {
            List<string> lines = new List<string>(10000);
            try
            {
                await FileIO.WriteLinesAsync(sf, lines);
            }
            catch
            {
                int oops = -1;
            }
        }
        #endregion

        #region ReadModelTextFile  ============================================
        private async void ReadModelTextFile(StorageFile sf)
        {
            try
            {
                var list = await FileIO.ReadLinesAsync(sf);
            }
            catch
            {
                int oops = -1;
            }
        }

        #endregion

        #endregion

        #region BinaryFiles  ==================================================
        //
        #region WriteModelDataFile  ===========================================
        private async void WriteModelDataFile(StorageFile file)
        {
            try
            {
                using (var tran = await file.OpenTransactedWriteAsync())
                {
                    using (var w = new DataWriter(tran.Stream))
                    {
                        w.ByteOrder = ByteOrder.LittleEndian;
                        WriteModelDataFile(w);
                        tran.Stream.Size = await w.StoreAsync(); // reset stream size to override the file
                        await tran.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }
        #endregion

        #region WriteModelDataFile  ===========================================
        private void WriteModelDataFile(DataWriter w)
        {
            Guid[] guids;
            Dictionary<Item, int> itemIndex;
            var itemCount = GetGuidItemIndex(out guids, out itemIndex);
            var relationList = GetRelationList();

            w.WriteInt32(0);
            w.WriteGuid(_fileFormat_2);

            WriteGuids(w, guids);

            if (_viewXStore.Count > 0) WriteViewX(w, itemIndex);
            if (_enumXStore.Count > 0) WriteEnumX(w, itemIndex);
            if (_tableXStore.Count > 0) WriteTableX(w, itemIndex);
            if (_graphXStore.Count > 0) WriteGraphX(w, itemIndex);
            if (_queryXStore.Count > 0) WriteQueryX(w, itemIndex);
            if (_symbolXStore.Count > 0) WriteSymbolX(w, itemIndex);
            if (_columnXStore.Count > 0) WriteColumnX(w, itemIndex);
            if (_computeXStore.Count > 0) WriteComputeX(w, itemIndex);
            if (_relationXStore.Count > 0) WriteRelationX(w, itemIndex);

            if (_graphParms.Count > 0) WriteGraphParm(w, itemIndex);
            if (relationList.Count > 0) WriteRelationLink(w, relationList, itemIndex);

            w.WriteByte((byte)Mark.StorageFileEnding);
            w.WriteGuid(_fileFormat_2);
            w.WriteInt32(0);

        }
        #endregion


        #region ReadModelDataFile  ============================================
        // The entire model and metadata are stored together in one file.
        // Nothing should be missing or incosistant, if it is, it's a bug.
        // Only file access or a program bug would cause the read to fail.
        // If anything goes wrong:  Throw an exception and Fix the bug.
        private async void ReadModelDataFile(StorageFile file)
        {
            try
            {
                var stream = await file.OpenAsync(FileAccessMode.Read);
                using (DataReader r = new DataReader(stream))
                {
                    r.ByteOrder = ByteOrder.LittleEndian;
                    UInt64 size = stream.Size;
                    if (size < UInt32.MaxValue)
                    {
                        var byteCount = await r.LoadAsync((UInt32)size);
                        ReadModelDataFile(r);
                    }
                }
                InitializeChoiceColumns();
                ValidateQueryXStore();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }
        #endregion

        #region ReadModelDataFile  ============================================
        private void ReadModelDataFile(DataReader r)
        {
            var guidItems = GetGuidItems();
            Item[] items;

            // determine the data file format
            var header = r.ReadInt32();
            var fileFormat = r.ReadGuid();

            Action<DataReader, Guid[], Item[], Dictionary<Guid, Item>>[] vector = null;

            if (header == 0)
            {
                if (fileFormat == _fileFormat_2)
                {
                    vector = new Action<DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_1,       // 4 GraphX
                        ReadQueryX_2,       // 5 QueryX
                        ReadSymbolX_1,      // 6 SymbolX
                        ReadColumnX_2,      // 7 ColumnX
                        ReadComputeX_2,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_2,    // 10 RelationX
                        ReadGraphParm_1,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
                else if (fileFormat == _fileFormat_1)
                {
                    vector = new Action<DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_1,       // 4 GraphX
                        ReadQueryX_1,       // 5 QueryX
                        ReadSymbolX_1,      // 6 SymbolX
                        ReadColumnX_1,      // 7 ColumnX
                        ReadComputeX_1,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_1,    // 10 RelationX
                        ReadGraphParm_1,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
            }

            if (vector == null) throw new Exception($"Unkown File Format Id {fileFormat}");

            var guids = ReadGuids(r);
            items = new Item[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                if (guids[i] != _dummy.Guid) continue;
                items[i] = _dummy; // DummyRef is a place holder used in the graph params dictionary 
                break;
            }

            for (;;)
            {
                var mark = (Mark)r.ReadByte();
                var vect = (int)mark;
                if (mark == Mark.StorageFileEnding)
                {
                    var format = r.ReadGuid();
                    if (format != fileFormat) throw new Exception($"Ending Format Id Does Not Match {format}");
                    return; // appearently there were no errors!
                }
                else if (vect > 0 && vect < vector.Length) 
                {
                    vector[vect](r, guids, items, guidItems);
                }
                else
                {
                    throw new Exception($"Invalid Marker {mark}");
                }
            }
        }
        #endregion


        #region Mark  =========================================================
        private enum Mark : byte
        {
            ViewXBegin = 1,
            EnumXBegin = 2,
            TableXBegin = 3,
            GraphXBegin = 4,
            QueryXBegin = 5,
            SymbolXBegin = 6,
            ColumnXBegin = 7,
            ComputeXBegin = 8,
            CommandXBegin = 9,
            RelationXBegin = 10,
            GraphParamBegin = 11,
            RelationLinkBegin = 12,

            ViewXEnding = 61,
            EnumXEnding = 62,
            TableXEnding = 63,
            GraphXEnding = 64,
            QueryXEnding = 65,
            SymbolXEnding = 66,
            ColumnXEnding = 67,
            ComputeXEnding = 68,
            CommandXEnding = 69,
            RelationXEnding = 70,
            GraphParamEnding = 71,
            RelationLinkEnding = 72,

            StorageFileEnding = 99,
        }
        #endregion

        #region Flags  ========================================================
        // don't read/write missing or default-value propties
        // these flags indicate which properties were non-default
        const byte BZ = 0;
        const byte B1 = 0x1;
        const byte B2 = 0x2;
        const byte B3 = 0x4;
        const byte B4 = 0x8;
        const byte B5 = 0x10;
        const byte B6 = 0x20;
        const byte B7 = 0x40;
        const byte B8 = 0x80;

        const ushort SZ = 0;
        const ushort S1 = 0x1;
        const ushort S2 = 0x2;
        const ushort S3 = 0x4;
        const ushort S4 = 0x8;
        const ushort S5 = 0x10;
        const ushort S6 = 0x20;
        const ushort S7 = 0x40;
        const ushort S8 = 0x80;
        const ushort S9 = 0x100;
        const ushort S10 = 0x200;
        const ushort S11 = 0x400;
        const ushort S12 = 0x800;
        const ushort S13 = 0x1000;
        const ushort S14 = 0x2000;
        const ushort S15 = 0x4000;
        const ushort S16 = 0x8000;
        #endregion

        #region Guids  ========================================================
        private void WriteGuids(DataWriter w, Guid[] guids)
        {
            w.WriteInt32(guids.Length);

            foreach (var g in guids) { w.WriteGuid(g); }
        }
        private Guid[] ReadGuids(DataReader r)
        {
            var count = r.ReadInt32();

            var guids = new Guid[count];
            for (int i = 0; i < count; i++) { guids[i] = r.ReadGuid(); }

            return guids;
        }
        #endregion

        #region ViewX  ========================================================
        //
        #region WriteViewX  ===================================================
        private void WriteViewX(DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.ViewXBegin); // type index
            w.WriteInt32(_viewXStore.Count);

            foreach (var view in _viewXStore.Items)
            {
                w.WriteInt32(itemIndex[view]);

                var b = BZ;
                if (!string.IsNullOrWhiteSpace(view.Name)) b |= B1;
                if (!string.IsNullOrWhiteSpace(view.Summary)) b |= B2;
                if (!string.IsNullOrWhiteSpace(view.Description)) b |= B3;

                w.WriteByte(b);
                if ((b & B1) != 0) WriteString(w, view.Name);
                if ((b & B2) != 0) WriteString(w, view.Summary);
                if ((b & B3) != 0) WriteString(w, view.Description);
            }
            w.WriteByte((byte)Mark.ViewXEnding); // itegrity marker
        }
        #endregion

        #region ReadViewX_1  ==================================================
        private void ReadViewX_1(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var vx = new ViewX(_viewXStore, guids[index]);
                items[index] = vx;

                var b = r.ReadByte();
                if ((b & B1) != 0) vx.Name = ReadString(r);
                if ((b & B2) != 0) vx.Summary = ReadString(r);
                if ((b & B3) != 0) vx.Description = ReadString(r);
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.ViewXEnding) throw new Exception($"Expected ViewXEnding marker, instead got {mark}");
        }
        #endregion
        //
        #endregion

        #region EmumX  ========================================================
        //
        #region WriteEnumX  ===================================================
        private void WriteEnumX(DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.EnumXBegin); // type vector index
            w.WriteInt32(_enumXStore.Count);

            foreach (var ex in _enumXStore.Items)
            {
                w.WriteInt32(itemIndex[ex]);

                var b = BZ;
                if (!string.IsNullOrWhiteSpace(ex.Name)) b |= B1;
                if (!string.IsNullOrWhiteSpace(ex.Summary)) b |= B2;
                if (!string.IsNullOrWhiteSpace(ex.Description)) b |= B3;

                w.WriteUInt16(b);
                if ((b & B1) != 0) WriteString(w, ex.Name);
                if ((b & B2) != 0) WriteString(w, ex.Summary);
                if ((b & B3) != 0) WriteString(w, ex.Description);

                if (ex.Count > 0 && ex.Count < byte.MaxValue)
                {
                    w.WriteByte((byte)ex.Count);

                    foreach (var px in ex.Items)
                    {
                        w.WriteInt32(itemIndex[px]);

                        WriteString(w, string.IsNullOrWhiteSpace(px.ActualValue) ? "0" : px.ActualValue);
                        WriteString(w, string.IsNullOrWhiteSpace(px.DisplayValue) ? "?" : px.DisplayValue);
                    }
                }
                else
                {
                    w.WriteByte((byte)0);
                }
            }
            w.WriteByte((byte)Mark.EnumXEnding); // itegrity marker
        }
        #endregion

        #region ReadEnumX_1  ==================================================
        private void ReadEnumX_1(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var ex = new EnumX(_enumXStore, guids[index]);
                items[index] = ex;

                var b = r.ReadByte();
                if ((b & B1) != 0) ex.Name = ReadString(r);
                if ((b & B2) != 0) ex.Summary = ReadString(r);
                if ((b & B3) != 0) ex.Description = ReadString(r);

                var pxCount = r.ReadByte();
                if (pxCount > 0) ex.SetCapacity(pxCount);

                for (int j = 0; j < pxCount; j++)
                {
                    var index2 = r.ReadInt32();
                    if (index2 < 0 || index2 >= items.Length) throw new Exception($"Invalid value index {index2}");

                    var px = new PairX(ex, guids[index2]);
                    items[index2] = px;

                    px.ActualValue = ReadString(r);
                    px.DisplayValue = ReadString(r);
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.EnumXEnding) throw new Exception($"Expected EnumXEnding marker, instead got {mark}");
        }
        #endregion
        //
        #endregion

        #region TableX  =======================================================
        //
        #region WriteTableX  ==================================================
        private void WriteTableX(DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.TableXBegin); // type index
            w.WriteInt32(_tableXStore.Count);

            foreach (var tx in _tableXStore.Items)
            {
                w.WriteInt32(itemIndex[tx]);

                var b = BZ;
                if (!string.IsNullOrWhiteSpace(tx.Name)) b |= B1;
                if (!string.IsNullOrWhiteSpace(tx.Summary)) b |= B2;
                if (!string.IsNullOrWhiteSpace(tx.Description)) b |= B3;

                w.WriteByte(b);
                if ((b & B1) != 0) WriteString(w, tx.Name);
                if ((b & B2) != 0) WriteString(w, tx.Summary);
                if ((b & B3) != 0) WriteString(w, tx.Description);

                if (tx.Count > 0)
                {
                    w.WriteInt32(tx.Count);
                    foreach (var rx in tx.Items)
                    {
                        w.WriteInt32(itemIndex[rx]);
                    }
                }
                else
                {
                    w.WriteInt32(0);
                }
            }
            w.WriteByte((byte)Mark.TableXEnding); // itegrity marker
        }
        #endregion

        #region ReadTableX_1  =================================================
        private void ReadTableX_1(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid table index {index}");

                var tx = new TableX(_tableXStore, guids[index]);
                items[index] = tx;

                var b = r.ReadByte();
                if ((b & B1) != 0) tx.Name = ReadString(r);
                if ((b & B2) != 0) tx.Summary = ReadString(r);
                if ((b & B3) != 0) tx.Description = ReadString(r);

                var rxCount = r.ReadInt32();
                if (rxCount < 0) throw new Exception($"Invalid row count {count}");
                if (rxCount > 0) tx.SetCapacity(rxCount);

                for (int j = 0; j < rxCount; j++)
                {
                    var index2 = r.ReadInt32();
                    if (index2 < 0 || index2 >= items.Length) throw new Exception($"Invalid row index {index2}");

                    items[index2] = new RowX(tx, guids[index2]);
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.TableXEnding) throw new Exception($"Expected TableXEnding marker, instead got {mark}");
        }
        #endregion
        //
        #endregion

        #region GraphX  =======================================================
        //
        #region WriteGraphX  ==================================================
        private void WriteGraphX(DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.GraphXBegin); // type index
            w.WriteInt32(_graphXStore.Count);

            foreach (var gx in _graphXStore.Items)
            {
                w.WriteInt32(itemIndex[gx]);

                var b = BZ;
                if (!string.IsNullOrWhiteSpace(gx.Name)) b |= B1;
                if (!string.IsNullOrWhiteSpace(gx.Summary)) b |= B2;
                if (!string.IsNullOrWhiteSpace(gx.Description)) b |= B3;

                w.WriteByte(b);
                if ((b & B1) != 0) WriteString(w, gx.Name);
                if ((b & B2) != 0) WriteString(w, gx.Summary);
                if ((b & B3) != 0) WriteString(w, gx.Description);
            }
            w.WriteByte((byte)Mark.GraphXEnding); // itegrity marker
        }
        #endregion

        #region ReadGraphX_1  =================================================
        private void ReadGraphX_1(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var gx = new GraphX(_graphXStore, guids[index]);
                items[index] = gx;

                var b = r.ReadByte();
                if ((b & B1) != 0) gx.Name = ReadString(r);
                if ((b & B2) != 0) gx.Summary = ReadString(r);
                if ((b & B3) != 0) gx.Description = ReadString(r);
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.GraphXEnding) throw new Exception($"Expected GraphXEnding marker, instead got {mark}");
        }
        #endregion
        //
        #endregion

        #region QueryX  =======================================================
        //
        #region WriteQueryX  ==================================================
        private void WriteQueryX(DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.QueryXBegin); // type vector
            w.WriteInt32(_queryXStore.Count);

            foreach (var qx in _queryXStore.Items)
            {
                w.WriteInt32(itemIndex[qx]);

                var b = BZ;
                if (qx.HasFlags()) b |= B1;
                if (!string.IsNullOrWhiteSpace(qx.WhereString)) b |= B2;
                if (!string.IsNullOrWhiteSpace(qx.SelectString)) b |= B3;
                if (qx.IsExclusive) b |= B4;
                if (qx.QueryKind == QueryType.Path && qx.IsHead == true && qx.Connect1 != Connect.Any) b |= B5;
                if (qx.QueryKind == QueryType.Path && qx.IsHead == true && qx.Connect2 != Connect.Any) b |= B6;

                w.WriteByte((byte)b);
                if ((b & B1) != 0) w.WriteUInt16(qx.GetFlags());
                if ((b & B2) != 0) WriteString(w, qx.WhereString);
                if ((b & B3) != 0) WriteString(w, qx.SelectString);
                if ((b & B4) != 0) w.WriteByte(qx.ExclusiveKey);
                if ((b & B5) != 0) w.WriteByte((byte)qx.Connect1);
                if ((b & B6) != 0) w.WriteByte((byte)qx.Connect2);
            }
            w.WriteByte((byte)Mark.QueryXEnding); // itegrity marker
        }
        #endregion

        #region ReadQueryX_1  =================================================
        private void ReadQueryX_1(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var qx = new QueryX(_queryXStore, guids[index]);
                items[index] = qx;

                var n = r.ReadByte() & 0xF;
                qx.QueryKind = QueryType.Graph;
                qx.IsHead = (n==3 || n == 5 || n == 7 || n == 13 );
                qx.IsRoot = (n == 1 || n == 12 || n == 15);
                if (n == 3 || n == 4) qx.QueryKind = QueryType.Path;
                if (n == 5 || n == 6) qx.QueryKind = QueryType.Group;
                if (n == 7 || n == 8) qx.QueryKind = QueryType.Segue;
                if (n == 12 || n == 13 || n == 14) qx.QueryKind = QueryType.Value;
                if (n == 15) qx.QueryKind = QueryType.Symbol;

                var f = (r.ReadByte());
                if ((f & B1) != 0) qx.IsReversed = true;


                var b = r.ReadByte();
                if ((b & B1) != 0) qx.WhereString = ReadString(r);
                if ((b & B2) != 0) qx.SelectString = ReadString(r);
                if ((b & B3) != 0) qx.ExclusiveKey = r.ReadByte();
                if ((b & B4) != 0) qx.Connect1 = (Connect)r.ReadByte();
                if ((b & B5) != 0) qx.Connect2 = (Connect)r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.QueryXEnding) throw new Exception($"Expected QueryXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadQueryX_2  =================================================
        private void ReadQueryX_2(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var qx = new QueryX(_queryXStore, guids[index]);
                items[index] = qx;

                var b = r.ReadByte();
                if ((b & B1) != 0) qx.SetFlags(r.ReadUInt16());
                if ((b & B2) != 0) qx.WhereString = ReadString(r);
                if ((b & B3) != 0) qx.SelectString = ReadString(r);
                if ((b & B4) != 0) qx.ExclusiveKey = r.ReadByte();
                if ((b & B5) != 0) qx.Connect1 = (Connect)r.ReadByte();
                if ((b & B6) != 0) qx.Connect2 = (Connect)r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.QueryXEnding) throw new Exception($"Expected QueryXEnding marker, instead got {mark}");
        }
        #endregion
        //
        #endregion

        #region SymbolX  ======================================================
        //
        #region WriteSymbolX  =================================================
        private void WriteSymbolX(DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.SymbolXBegin); // type index
            w.WriteInt32(_symbolXStore.Count);

            foreach (var sx in _symbolXStore.Items)
            {
                w.WriteInt32(itemIndex[sx]);

                var b = BZ;
                if (!string.IsNullOrWhiteSpace(sx.Name)) b |= B1;
                if (!string.IsNullOrWhiteSpace(sx.Summary)) b |= B2;
                if (!string.IsNullOrWhiteSpace(sx.Description)) b |= B3;
                if (sx.Data != null && sx.Data.Length > 12) b |= B4;
                if (sx.TopContact != Contact.Any) b |= B5;
                if (sx.LeftContact != Contact.Any) b |= B6;
                if (sx.RightContact != Contact.Any) b |= B7;
                if (sx.BottomContact != Contact.Any) b |= B8;

                w.WriteByte(b);
                if ((b & B1) != 0) WriteString(w, sx.Name);
                if ((b & B2) != 0) WriteString(w, sx.Summary);
                if ((b & B3) != 0) WriteString(w, sx.Description);
                if ((b & B4) != 0) WriteBytes(w, sx.Data);
                if ((b & B5) != 0) w.WriteByte((byte)sx.TopContact);
                if ((b & B6) != 0) w.WriteByte((byte)sx.LeftContact);
                if ((b & B7) != 0) w.WriteByte((byte)sx.RightContact);
                if ((b & B8) != 0) w.WriteByte((byte)sx.BottomContact);
            }
            w.WriteByte((byte)Mark.SymbolXEnding); // itegrity marker
        }
        #endregion

        #region ReadSymbolX_1  ===========================================
        private void ReadSymbolX_1(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var sx = new SymbolX(_symbolXStore, guids[index]);
                items[index] = sx;

                var b = r.ReadByte();
                if ((b & B1) != 0) sx.Name = ReadString(r);
                if ((b & B2) != 0) sx.Summary = ReadString(r);
                if ((b & B3) != 0) sx.Description = ReadString(r);
                if ((b & B4) != 0) sx.Data = ReadBytes(r);
                if ((b & B5) != 0) sx.TopContact = (Contact)r.ReadByte();
                if ((b & B6) != 0) sx.LeftContact = (Contact)r.ReadByte();
                if ((b & B7) != 0) sx.RightContact = (Contact)r.ReadByte();
                if ((b & B8) != 0) sx.BottomContact = (Contact)r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.SymbolXEnding) throw new Exception($"Expected SymbolXEnding marker, instead got {mark}");
        }
        #endregion
        //
        #endregion

        #region ColumnX  ======================================================
        //
        #region WriteColumnX  =================================================
        private void WriteColumnX(DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.ColumnXBegin); // type index
            w.WriteInt32(_columnXStore.Count);

            foreach (var cx in _columnXStore.Items)
            {
                w.WriteInt32(itemIndex[cx]);

                var b = BZ;
                if (cx.HasFlags()) b |= B1;
                if (!string.IsNullOrWhiteSpace(cx.Name)) b |= B2;
                if (!string.IsNullOrWhiteSpace(cx.Summary)) b |= B3;
                if (!string.IsNullOrWhiteSpace(cx.Initial)) b |= B4;
                if (!string.IsNullOrWhiteSpace(cx.Description)) b |= B5;
                if (cx.ValueType != ValueType.String) b |= B6;
                if (cx.HasSpecificDefault) b |= B7;

                w.WriteByte(b);
                if ((b & B1) != 0) w.WriteUInt16(cx.GetFlags());
                if ((b & B2) != 0) WriteString(w, cx.Name);
                if ((b & B3) != 0) WriteString(w, cx.Summary);
                if ((b & B4) != 0) WriteString(w, cx.Initial);
                if ((b & B5) != 0) WriteString(w, cx.Description);
                if ((b & B6) != 0) w.WriteByte((byte)cx.ValueType);
                if ((b & B7) != 0) WriteString(w, cx.Default);

                if (cx.TryGetKeys(out Item[] rows))
                {
                    w.WriteInt32(rows.Length);
                    foreach (var rx in rows)
                    {
                        string val = cx.GetValue(rx);
                        w.WriteInt32(itemIndex[rx]);
                        WriteString(w, ((val != null) ? val : string.Empty));
                    }
                }
                else
                {
                    w.WriteInt32(0);
                }
            }
            w.WriteByte((byte)Mark.ColumnXEnding); // itegrity marker
        }
        #endregion

        #region ReadColumnX_1  ================================================
        private void ReadColumnX_1(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid column index {index}");

                var cx = new ColumnX(_columnXStore, guids[index]);
                items[index] = cx;

                var f = r.ReadByte();
                if ((f & B1) != 0) cx.IsChoice = true;

                var b = r.ReadByte();
                if ((b & B1) != 0) cx.Name = ReadString(r);
                if ((b & B2) != 0) cx.Summary = ReadString(r);
                if ((b & B3) != 0) cx.Initial = ReadString(r);
                if ((b & B4) != 0) cx.Description = ReadString(r);

                var type = ((b & B5) != 0) ? (ValueType)r.ReadByte() : ValueType.String;
                var defaultVal = ((b & B6) != 0) ? ReadString(r) : null;

                var rowCount = r.ReadInt32();
                if (rowCount < 0) throw new Exception($"Invalid row count {rowCount}");

                cx.Initialize(type, defaultVal, rowCount);

                for (int j = 0; j < rowCount; j++)
                {
                    var rowIndex = r.ReadInt32();
                    if (rowIndex < 0 || rowIndex >= items.Length) throw new Exception($"Invalid row index {rowIndex}");

                    var rx = items[rowIndex];
                    if (rx == null) throw new Exception($"Column row is null, index {rowIndex}");

                    var val = ReadString(r);
                    if (!cx.TrySetValue(rx, val)) throw new Exception($"Could not set value {val}");
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.ColumnXEnding) throw new Exception($"Expected ColumnXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadColumnX_2  ================================================
        private void ReadColumnX_2(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid column index {index}");

                var cx = new ColumnX(_columnXStore, guids[index]);
                items[index] = cx;

                var b = r.ReadByte();
                if ((b & B1) != 0) cx.SetFlags(r.ReadUInt16());
                if ((b & B2) != 0) cx.Name = ReadString(r);
                if ((b & B3) != 0) cx.Summary = ReadString(r);
                if ((b & B4) != 0) cx.Initial = ReadString(r);
                if ((b & B5) != 0) cx.Description = ReadString(r);

                var type = ((b & B6) != 0) ? (ValueType)r.ReadByte() : ValueType.String;
                var defaultVal = ((b & B7) != 0) ? ReadString(r) : null;

                var rowCount = r.ReadInt32();
                if (rowCount < 0) throw new Exception($"Invalid row count {rowCount}");

                cx.Initialize(type, defaultVal, rowCount);

                for (int j = 0; j < rowCount; j++)
                {
                    var rowIndex = r.ReadInt32();
                    if (rowIndex < 0 || rowIndex >= items.Length) throw new Exception($"Invalid row index {rowIndex}");

                    var rx = items[rowIndex];
                    if (rx == null) throw new Exception($"Column row is null, index {rowIndex}");

                    var val = ReadString(r);
                    if (!cx.TrySetValue(rx, val)) throw new Exception($"Could not set value {val}");
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.ColumnXEnding) throw new Exception($"Expected ColumnXEnding marker, instead got {mark}");
        }
        #endregion
        //
        #endregion

        #region ComputeX  =====================================================
        //
        #region WriteComputeX  ================================================
        private void WriteComputeX(DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.ComputeXBegin); // type vector
            w.WriteInt32(_computeXStore.Count);

            var items = _computeXStore.Items;
            foreach (var cx in items)
            {
                w.WriteInt32(itemIndex[cx]);

                var  b = BZ;
                if (!string.IsNullOrWhiteSpace(cx.Name)) b |= B1;
                if (!string.IsNullOrWhiteSpace(cx.Summary)) b |= B2;
                if (!string.IsNullOrWhiteSpace(cx.Description)) b |= B3;
                if (cx.Separator != ComputeX.DefaultSeparator) b |= B4;
                if (cx.CompuType != CompuType.RowValue) b |= B5;
                if (cx.NumericSet != NumericSet.Count) b |= B6;

                w.WriteByte(b);
                if ((b & B1) != 0) WriteString(w, cx.Name);
                if ((b & B2) != 0) WriteString(w, cx.Summary);
                if ((b & B3) != 0) WriteString(w, cx.Description);
                if ((b & B4) != 0) WriteString(w, ((cx.Separator != null) ? cx.Separator : string.Empty));
                if ((b & B5) != 0) w.WriteByte((byte)cx.CompuType);
                if ((b & B6) != 0) w.WriteByte((byte)cx.NumericSet);
            }
            w.WriteByte((byte)Mark.ComputeXEnding); // itegrity marker
        }
        #endregion

        #region ReadComputeX_1  ===============================================
        private void ReadComputeX_1(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var cx = new ComputeX(_computeXStore, guids[index]);
                items[index] = cx;

                var b = r.ReadByte();
                if ((b & B1) != 0) cx.Name = ReadString(r);
                if ((b & B2) != 0) cx.Summary = ReadString(r);
                if ((b & B3) != 0) cx.Description = ReadString(r);
                if ((b & B4) != 0) cx.Separator = ReadString(r);
                if ((b & B5) != 0)
                {
                    var n = r.ReadByte();
                    if (n > 1) n += 1;
                    cx.CompuType = (CompuType)n;
                }
                if ((b & B6) != 0)
                {
                    var n = r.ReadByte();
                    if (n > 1) n += 1;
                    cx.NumericSet = (NumericSet)n;
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.ComputeXEnding) throw new Exception($"Expected ComputeXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadComputeX_2  ===============================================
        private void ReadComputeX_2(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var cx = new ComputeX(_computeXStore, guids[index]);
                items[index] = cx;

                var b = r.ReadByte();
                if ((b & B1) != 0) cx.Name = ReadString(r);
                if ((b & B2) != 0) cx.Summary = ReadString(r);
                if ((b & B3) != 0) cx.Description = ReadString(r);
                if ((b & B4) != 0) cx.Separator = ReadString(r);
                if ((b & B5) != 0) cx.CompuType = (CompuType)r.ReadByte();
                if ((b & B6) != 0) cx.NumericSet = (NumericSet)r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.ComputeXEnding) throw new Exception($"Expected ComputeXEnding marker, instead got {mark}");
        }
        #endregion
        //
        #endregion

        #region RelationX  ====================================================
        //
        #region WriteRelationX  ===============================================
        private void WriteRelationX(DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.RelationXBegin); // type index
            w.WriteInt32(_relationXStore.Count);

            foreach (var rx in _relationXStore.Items)
            {
                w.WriteInt32(itemIndex[rx]);

                var keyCount = rx.KeyCount;
                var valCount = rx.ValueCount;

                var b = BZ;
                if (rx.HasFlags()) b |= B1;
                if (!string.IsNullOrWhiteSpace(rx.Name)) b |= B2;
                if (!string.IsNullOrWhiteSpace(rx.Summary)) b |= B3;
                if (!string.IsNullOrWhiteSpace(rx.Description)) b |= B4;
                if (rx.Pairing != Pairing.OneToMany) b |= B5;
                if (rx.IsLimited && (rx.MinOccurance != 0 || rx.MaxOccurance != 0)) b |= B6;
                if ((keyCount + valCount) > 0) b |= B7;

                w.WriteByte(b);
                if ((b & B1) != 0) w.WriteUInt16(rx.GetFlags());
                if ((b & B2) != 0) WriteString(w, rx.Name);
                if ((b & B3) != 0) WriteString(w, rx.Summary);
                if ((b & B4) != 0) WriteString(w, rx.Description);
                if ((b & B5) != 0) w.WriteByte((byte)rx.Pairing);
                if ((b & B6) != 0) w.WriteInt16(rx.MinOccurance);
                if ((b & B6) != 0) w.WriteInt16(rx.MaxOccurance);
                if ((b & B7) != 0) w.WriteInt32(keyCount);
                if ((b & B7) != 0) w.WriteInt32(valCount);
            }
            w.WriteByte((byte)Mark.RelationXEnding); // itegrity marker
        }
        #endregion

        #region ReadRelationX_1  ==============================================
        private void ReadRelationX_1(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var rx = new RelationX(_relationXStore, guids[index]);
                items[index] = rx;

                var f = r.ReadByte();
                if ((f & B2) != 0) rx.IsRequired = true;

                var b = r.ReadByte();
                if ((b & B1) != 0) rx.Name = ReadString(r);
                if ((b & B2) != 0) rx.Summary = ReadString(r);
                if ((b & B3) != 0) rx.Description = ReadString(r);
                if ((b & B4) != 0) rx.Pairing = (Pairing)r.ReadByte(); 
                if ((b & B5) != 0) rx.MinOccurance = r.ReadInt16();
                if ((b & B5) != 0) rx.MaxOccurance = r.ReadInt16();
                var keyCount = ((b & B6) != 0) ? r.ReadInt32() : 0;
                var valCount = ((b & B6) != 0) ? r.ReadInt32() : 0;
                rx.Initialize(keyCount, valCount);
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.RelationXEnding) throw new Exception($"Expected RelationXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadRelationX_2  ==============================================
        private void ReadRelationX_2(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var rx = new RelationX(_relationXStore, guids[index]);
                items[index] = rx;

                var b = r.ReadByte();
                if ((b & B1) != 0) rx.SetFlags(r.ReadUInt16());
                if ((b & B2) != 0) rx.Name = ReadString(r);
                if ((b & B3) != 0) rx.Summary = ReadString(r);
                if ((b & B4) != 0) rx.Description = ReadString(r);
                if ((b & B5) != 0) rx.Pairing = (Pairing)r.ReadByte();
                if ((b & B6) != 0) rx.MinOccurance = r.ReadInt16();
                if ((b & B6) != 0) rx.MaxOccurance = r.ReadInt16();
                var keyCount = ((b & B7) != 0) ? r.ReadInt32() : 0;
                var valCount = ((b & B7) != 0) ? r.ReadInt32() : 0;
                rx.Initialize(keyCount, valCount);
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.RelationXEnding) throw new Exception($"Expected RelationXEnding marker, instead got {mark}");
        }
        #endregion
        //
        #endregion

        #region GraphParm  ====================================================
        //
        #region WriteGraphParm  ===============================================
        private void WriteGraphParm(DataWriter w, Dictionary<Item, int> itemIndex)
        {
            #region RemoveInvalidItems  =======================================
            // hit list of items that no longer exists
            var gxList = new List<GraphX>();
            var gxrtList = new List<Tuple<GraphX, Item>>();
            var gxrtsgList = new List<Tuple<GraphX, Item, Item>>();
            var gxrtsgpmList = new List<Tuple<GraphX, Item, Item, Item>>();

            // find items that are referenced in the graph parms, but no longer exist
            foreach (var e1 in _graphParms)//GD
            {
                var gd = e1.Key;
                if (itemIndex.ContainsKey(gd))
                {
                    foreach (var e2 in e1.Value)//RT
                    {
                        var rt = e2.Key;
                        if (itemIndex.ContainsKey(rt))
                        {
                            foreach (var e3 in e2.Value)//SG
                            {
                                var sg = e3.Key;
                                if (itemIndex.ContainsKey(sg))
                                {
                                    if (sg == _dummy)
                                    {
                                        foreach (var pm in e3.Value)//PM
                                        {
                                            var nd = pm as Node;
                                            if (itemIndex.ContainsKey(nd.Item)) continue;
                                            gxrtsgpmList.Add(new Tuple<GraphX, Item, Item, Item>(gd, rt, sg, pm));
                                        }
                                    }
                                    else
                                    {
                                        foreach (var pm in e3.Value)//GP
                                        {
                                            var eg = pm as Edge;
                                            if (itemIndex.ContainsKey(eg.Node1.Item) && itemIndex.ContainsKey(eg.Node2.Item)) continue;
                                            gxrtsgpmList.Add(new Tuple<GraphX, Item, Item, Item>(gd, rt, sg, pm));
                                        }
                                    }
                                }
                                else
                                {
                                    gxrtsgList.Add(new Tuple<GraphX, Item, Item>(gd, rt, sg));
                                }
                            }
                        }
                        else
                        {
                            gxrtList.Add(new Tuple<GraphX, Item>(gd, rt));
                        }
                    }
                }
                else
                {
                    gxList.Add(gd);
                }
            }

            // remove params for items which no longer exists
            foreach (var gd in gxList)
            {
                _graphParms.Remove(gd);
            }
            foreach (var gdrt in gxrtList)
            {
                var gd = gdrt.Item1;
                var rt = gdrt.Item2;
                _graphParms[gd].Remove(rt);
                if (_graphParms[gd].Count == 0)
                    _graphParms.Remove(gd);
            }
            foreach (var gdrtsg in gxrtsgList)
            {
                var gd = gdrtsg.Item1;
                var rt = gdrtsg.Item2;
                var sg = gdrtsg.Item3;
                _graphParms[gd][rt].Remove(sg);
                if (_graphParms[gd][rt].Count == 0)
                    _graphParms[gd].Remove(rt);
                if (_graphParms[gd].Count == 0)
                    _graphParms.Remove(gd);
            }
            foreach (var gdrtsgpm in gxrtsgpmList)
            {
                var gd = gdrtsgpm.Item1;
                var rt = gdrtsgpm.Item2;
                var sg = gdrtsgpm.Item3;
                var pm = gdrtsgpm.Item4;
                _graphParms[gd][rt][sg].Remove(pm);
                if (_graphParms[gd][rt][sg].Count == 0)
                    _graphParms[gd][rt].Remove(sg);
                if (_graphParms[gd][rt].Count == 0)
                    _graphParms[gd].Remove(rt);
                if (_graphParms[gd].Count == 0)
                    _graphParms.Remove(gd);
            }
            #endregion

            // now write the remaining valid graph params to the storage file
            foreach (var e1 in _graphParms)//GD
            {
                if (e1.Value.Count == 0) continue;

                w.WriteByte((byte)Mark.GraphParamBegin); // type index

                w.WriteInt32(itemIndex[e1.Key]);
                w.WriteInt32(e1.Value.Count);

                foreach (var e2 in e1.Value)//RT
                {
                    w.WriteInt32(itemIndex[e2.Key]);
                    w.WriteInt32(e2.Value.Count);

                    if (e2.Value.Count > 0)
                    {
                        #region WriteRoots  ===================================
                        int x0, y0;
                        GetOffset(e2.Value, out x0, out y0);

                        foreach (var e3 in e2.Value)//SG
                        {
                            w.WriteInt32(itemIndex[e3.Key]);
                            w.WriteInt32(e3.Value.Count);

                            if (e3.Value.Count > 0)
                            {
                                #region WriteQuerys  ==========================
                                if (e3.Key == _dummy)
                                {
                                    #region WriteNodes  =======================
                                    foreach (var gp in e3.Value)//GP
                                    {
                                        var nd = gp as Node;
                                        w.WriteInt32(itemIndex[nd.Item]);

                                        w.WriteInt32(nd.Core.X - x0);
                                        w.WriteInt32(nd.Core.Y - y0);
                                        w.WriteByte(nd.Core.DX);
                                        w.WriteByte(nd.Core.DY);
                                        w.WriteByte(nd.Core.Symbol);
                                        w.WriteByte((byte)nd.Core.Orientation);
                                        w.WriteByte((byte)nd.Core.FlipRotate);
                                        w.WriteByte((byte)nd.Core.Labeling);
                                        w.WriteByte((byte)nd.Core.Resizing);
                                        w.WriteByte((byte)nd.Core.BarWidth);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region WriteEdges  =======================
                                    foreach (var gp in e3.Value)//GP
                                    {
                                        var eg = gp as Edge;
                                        w.WriteInt32(itemIndex[eg.Node1.Item]);
                                        w.WriteInt32(itemIndex[eg.Node2.Item]);

                                        w.WriteByte((byte)eg.Core.Face1.Side);
                                        w.WriteByte((byte)eg.Core.Face1.Index);
                                        w.WriteByte((byte)eg.Core.Face1.Count);
                                        w.WriteByte((byte)eg.Core.Face2.Side);
                                        w.WriteByte((byte)eg.Core.Face2.Index);
                                        w.WriteByte((byte)eg.Core.Face2.Count);
                                        w.WriteByte((byte)eg.Core.Facet1);
                                        w.WriteByte((byte)eg.Core.Facet2);

                                        var len = (eg.Core.Bends == null) ? 0 : eg.Core.Bends.Length;
                                        if (len > 0)
                                        {

                                            w.WriteUInt16((ushort)len);
                                            for (int i = 0; i < len; i++)
                                            {
                                                w.WriteInt32(eg.Core.Bends[i].X - x0);
                                                w.WriteInt32(eg.Core.Bends[i].Y - y0);
                                            }
                                        }
                                        else
                                        {
                                            w.WriteUInt16(0);
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                        }
                        #endregion
                    }
                }
                w.WriteByte((byte)Mark.GraphParamEnding);
            }
        }
        #region TryGetOffset  =================================================
        private void GetOffset(Dictionary<Item, List<Item>> sgParams, out int x0, out int y0)
        {
            int x, y, x1, y1, x2, y2;
            x1 = y1 = int.MaxValue;
            x2 = y2 = int.MinValue;
            foreach (var e3 in sgParams)//SG
            {
                if (e3.Key == _dummy)
                {
                    foreach (var gp in e3.Value)//GP
                    {
                        var nd = gp as Node;
                        if (nd.Core.TryGetCenter(out x, out y))
                        {
                            if (x < x1) x1 = x;
                            if (y < y1) y1 = y;
                            if (x > x2) x2 = x;
                            if (y > y2) y2 = y;
                        }
                    }
                }
            }
            x0 = (x1 + x2) / 2;
            y0 = (y1 + y2) / 2;
            if (x1 == int.MaxValue)
            {
                x0 = y0 = GraphParm.CenterOffset;
            }
        }
        #endregion
        #endregion

        #region ReadGraphParm_1  ==============================================
        private void ReadGraphParm_1(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            Dictionary<Item, Dictionary<Item, List<Item>>> RtMsParms = null;
            Dictionary<Item, List<Item>> MsParms = null;
            List<Item> Parms = null;

            var gdIndex = r.ReadInt32();
            if (gdIndex < 0 || gdIndex >= items.Length) throw new Exception($"Invalid index {gdIndex}");

            var gdLen = r.ReadInt32();
            if (gdLen < 0) throw new Exception($"Invalid count {gdLen}");

            var gd = items[gdIndex] as GraphX;
            if (gd == null) throw new Exception($"Expected graphDef object, got null {gdIndex}");

            if (_graphParms == null) _graphParms = new Dictionary<GraphX, Dictionary<Item, Dictionary<Item, List<Item>>>>(gdLen);

            #region FindCreate RtMsParms ======================================
            if (!_graphParms.TryGetValue(gd, out RtMsParms))
            {
                RtMsParms = new Dictionary<Item, Dictionary<Item, List<Item>>>(gdLen);
                _graphParms.Add(gd, RtMsParms);
            }
            #endregion

            for (int i = 0; i < gdLen; i++)
            {
                var rtIndex = r.ReadInt32();
                if (rtIndex < 0 || rtIndex >= items.Length) throw new Exception($"Invalid index {rtIndex}");

                var rtLen = r.ReadInt32();
                if (rtLen < 0) throw new Exception($"Invalid count {rtLen}");

                var rt = items[rtIndex];
                if (rt == null) throw new Exception($"Expected root object, got null {rtIndex}");

                var cp = GraphParm.CenterOffset;

                #region FindCreate MsParms ====================================
                if (!RtMsParms.TryGetValue(rt, out MsParms))
                {
                    MsParms = new Dictionary<Item, List<Item>>(rtLen);
                    RtMsParms.Add(rt, MsParms);
                }
                #endregion

                var itemNodeParms = new Dictionary<Item, Node>();

                for (int j = 0; j < rtLen; j++)
                {
                    var msIndex = r.ReadInt32();
                    if (msIndex < 0 || msIndex >= items.Length) throw new Exception($"Invalid index {msIndex}");

                    var msLen = r.ReadInt32();
                    if (msLen < 0) throw new Exception($"Invalid count {msLen}");

                    var ms = items[msIndex];
                    if (ms == null)
                    {
                        var guid = guids[msIndex];
                        if (!guidItems.TryGetValue(guid, out ms)) throw new Exception($"Could not find QueryX for guid {guid}");
                        items[msIndex] = ms;
                    }

                    #region FindCreate Parms ==================================
                    if (!MsParms.TryGetValue(ms, out Parms))
                    {
                        Parms = new List<Item>(msLen);
                        MsParms.Add(ms, Parms);
                    }
                    #endregion

                    if (msLen > 0)
                    {
                        Node ndParm, ndParm1, ndParm2;
                        Edge edParm;

                        if (ms == _dummy)
                        {
                            #region ReadNodeParms  ============================
                            for (int k = 0; k < msLen; k++)
                            {
                                var ndIndex = r.ReadInt32();
                                if (ndIndex < 0 || ndIndex >= items.Length) throw new Exception($"Invalid index {ndIndex}");

                                var nd = items[ndIndex];
                                if (nd == null) throw new Exception($"Expected node object, got null {ndIndex}");

                                if (!itemNodeParms.TryGetValue(nd, out ndParm))
                                {
                                    ndParm = new Node(cp);
                                    ndParm.Item = nd;

                                    itemNodeParms.Add(nd, ndParm);
                                    Parms.Add(ndParm);
                                }

                                ndParm.Core.X = r.ReadInt32() + cp;
                                ndParm.Core.Y = r.ReadInt32() + cp;
                                ndParm.Core.DX = r.ReadByte();
                                ndParm.Core.DY = r.ReadByte();
                                ndParm.Core.Symbol = r.ReadByte();
                                ndParm.Core.Orientation = (Orientation)r.ReadByte();
                                ndParm.Core.FlipRotate = (FlipRotate)r.ReadByte();
                                ndParm.Core.Labeling = (Labeling)r.ReadByte();
                                ndParm.Core.Resizing = (Resizing)r.ReadByte();
                                ndParm.Core.BarWidth = (BarWidth)r.ReadByte();
                            }
                            #endregion
                        }
                        else
                        {
                            #region ReadEdgeParms  ============================
                            for (int k = 0; k < msLen; k++)
                            {
                                var nd1Index = r.ReadInt32();
                                if (nd1Index < 0 || nd1Index >= items.Length) throw new Exception($"Invalid index {nd1Index}");

                                var nd1 = items[nd1Index];
                                if (nd1 == null) throw new Exception($"Expected node object, got null {nd1Index}");

                                var nd2Index = r.ReadInt32();
                                if (nd2Index < 0 || nd2Index >= items.Length) throw new Exception($"Invalid index {nd2Index}");

                                var nd2 = items[nd2Index];
                                if (nd2 == null) throw new Exception($"Expected node object, got null {nd2Index}");

                                if (!itemNodeParms.TryGetValue(nd1, out ndParm1)) throw new Exception("Could not Finde NodeParm1");
                                if (!itemNodeParms.TryGetValue(nd2, out ndParm2)) throw new Exception("Could not Finde NodeParm2");

                                edParm = new Edge(ms);
                                Parms.Add(edParm);

                                edParm.Node1 = ndParm1;
                                edParm.Node2 = ndParm2;

                                edParm.Core.Face1.Side = (Side)r.ReadByte();
                                edParm.Core.Face1.Index = r.ReadByte();
                                edParm.Core.Face1.Count = r.ReadByte();
                                edParm.Core.Face2.Side = (Side)r.ReadByte();
                                edParm.Core.Face2.Index = r.ReadByte();
                                edParm.Core.Face2.Count = r.ReadByte();
                                edParm.Core.Facet1 = (FacetOf)r.ReadByte();
                                edParm.Core.Facet2 = (FacetOf)r.ReadByte();

                                var pnCount = r.ReadUInt16();
                                if (pnCount > 0)
                                {
                                    edParm.Core.Bends = new XYPoint[pnCount];
                                    for (int n = 0; n < pnCount; n++)
                                    {
                                        edParm.Core.Bends[n].X = r.ReadInt32();
                                        edParm.Core.Bends[n].Y = r.ReadInt32();
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            var mark = (Mark)r.ReadByte();
            //if (mark != Mark.GraphParamEnding) throw new Exception($"Expected GraphParamEnding marker, instead got {mark}");
        }
        #endregion
        //
        #endregion

        #region RelationLink  =================================================
        //
        #region WriteRelationLink  ============================================
        private void WriteRelationLink(DataWriter w, List<Relation> relationList, Dictionary<Item, int> itemIndex)
        {
            foreach (var rx in relationList)
            {
                var count = rx.GetLinksCount();
                if (count == 0) continue;

                ushort len = 0;
                Item itm = this;
                Item[] parents;
                Item[] children;
                rx.GetLinks(out parents, out children);

                int N = count;
                for (int j = 0; j < count; j++)
                {
                    var child = children[j];
                    var parent = parents[j];
                    if (itemIndex.ContainsKey(child) && itemIndex.ContainsKey(parent)) continue;

                    // null out this is link, it should not be serialized
                    children[j] = null;
                    parents[j] = null;
                    N -= 1;
                }
                if (N == 0) continue;

                w.WriteByte((byte)Mark.RelationLinkBegin); // type index
                w.WriteInt32(itemIndex[rx]);
                w.WriteInt32(N);

                for (int j = 0; j < count; j++)
                {
                    var child = children[j];
                    var parent = parents[j];
                    if (child == null || parent == null) continue;

                    if (itm != parent)
                    {
                        len = 1;
                        itm = parent;
                        for (int k = j + 1; k < count; k++)
                        {
                            if (parents[k] != itm) break;
                            if (len < ushort.MaxValue) len += 1;
                        }
                    }

                    w.WriteInt32(itemIndex[parent]);
                    w.WriteInt32(itemIndex[child]);
                    w.WriteUInt16(len);
                    len = 0;
                }
                w.WriteByte((byte)Mark.RelationLinkEnding); // type index
            }
        }
        #endregion

        #region ReadRelationLink_1  ===========================================
        private void ReadRelationLink_1(DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var index = r.ReadInt32();
            var count = r.ReadInt32();

            if (index < 0 || index >= items.Length) throw new Exception($"Invalid relation index {index}");

            var item = items[index];
            if (item == null)
            {
                var guid = guids[index];
                if (guidItems.TryGetValue(guid, out item)) items[index] = item;
            }

            var rel = item as Relation;
            //if (rel == null) throw new Exception("The item is not a relation");

            for (int i = 0; i < count; i++)
            {
                var index1 = r.ReadInt32();
                var index2 = r.ReadInt32();
                var len = r.ReadUInt16();

                if (index1 < 0 || index1 >= items.Length) throw new Exception($"Invalid index1 {index1}");

                var item1 = items[index1];
                if (item1 == null)
                {
                    var guid = guids[index1];
                    if (guidItems.TryGetValue(guid, out item1)) items[index1] = item1;
                }

                if (index2 < 0 || index2 >= items.Length) throw new Exception($"Invalid index2 {index2}");

                var item2 = items[index2];
                if (item2 == null)
                {
                    var guid = guids[index2];
                    if (guidItems.TryGetValue(guid, out item2)) items[index2] = item2;
                }
                if (rel != null && item1 != null && item2 != null)
                    rel.SetLink(item1, item2, len);
                else
                    Debug.WriteLine("missing a relation");
            }
            var mark = (Mark)r.ReadByte();
            //if (mark != Mark.RelationLinkEnding) throw new Exception($"Expected RelationLinkEnding marker, instead got {mark}");
        }
        #endregion
        //
        #endregion


        #region ReadWriteString  ==============================================
        private void WriteString(DataWriter w, string text)
        {
            var len = w.MeasureString(text);
            if (len > UInt16.MaxValue)
            {
                var r = (double)len / (double)UInt16.MaxValue;
                var n = (UInt16)((text.Length / r) - 2);
                var trucated = text.Substring(0, n);
                w.WriteUInt16((UInt16)w.MeasureString(trucated));
                w.WriteString(trucated);
            }
            else
            {
                w.WriteUInt16((UInt16)len);
                w.WriteString(text);
            }
        }
        private string ReadString(DataReader r)
        {
            var len = (UInt32)r.ReadUInt16();
            return r.ReadString(len);
        }
        #endregion

        #region ReadWriteBytes  ===============================================
        private void WriteBytes(DataWriter w, byte[] data)
        {
            var len = data.Length;
            w.WriteInt32(len);
            foreach (var b in data)
            {
                w.WriteByte(b);
            }
        }
        private byte[] ReadBytes(DataReader r)
        {
            var len = r.ReadInt32();
            var data = new byte[len];
            for (int i = 0; i < len; i++)
            {
                data[i] = r.ReadByte();
            }
            return data;
        }
        #endregion
        //
        #endregion


        #region GetGuidItems  =================================================
        private Dictionary<Guid, Item> GetGuidItems()
        {
            Guid[] guids;
            Dictionary<Item, int> itemIndex;
            var count = GetGuidItemIndex(out guids, out itemIndex);
            var guidItems = new Dictionary<Guid, Item>(count);
            foreach (var e in itemIndex)
            {
                guidItems.Add(guids[e.Value], e.Key);
            }
            return guidItems;
        }
        #endregion

        #region GetGuidItemIndex  =============================================
        private int GetGuidItemIndex(out Guid[] guids, out Dictionary<Item, int> itemIndex)
        {
            // count all items that have guids
            //=============================================
            int count = 23; // allow for static store guids

            foreach (var item in _enumXStore.Items)
            {
                count += (item as EnumX).Count; // PairX count
            }

            foreach (var item in _tableXStore.Items)
            {
                count += (item as TableX).Count; // RowX count
            }

            count += _viewXStore.Count;
            count += _enumXStore.Count;
            count += _tableXStore.Count;
            count += _graphXStore.Count;
            count += _queryXStore.Count;
            count += _symbolXStore.Count;
            count += _columnXStore.Count;
            count += _computeXStore.Count;
            count += _relationXStore.Count;
            count += _relationStore.Count;

            // allocate memory
            //=============================================
            guids = new Guid[count];
            itemIndex = new Dictionary<Item, int>(count);


            // populate the item and guid arrays
            //=============================================
            int j = 0;
            itemIndex.Add(_dummy, j);
            guids[j++] = _dummy.Guid;

            itemIndex.Add(_viewXStore, j);
            guids[j++] = _viewXStore.Guid;
            foreach (var itm in _viewXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_enumXStore, j);
            guids[j++] = _enumXStore.Guid;
            foreach (var itm in _enumXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_tableXStore, j);
            guids[j++] = _tableXStore.Guid;
            foreach (var itm in _tableXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_graphXStore, j);
            guids[j++] = _graphXStore.Guid;
            foreach (var itm in _graphXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_queryXStore, j);
            guids[j++] = _queryXStore.Guid;
            foreach (var itm in _queryXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_symbolXStore, j);
            guids[j++] = _symbolXStore.Guid;
            foreach (var itm in _symbolXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_columnXStore, j);
            guids[j++] = _columnXStore.Guid;
            foreach (var itm in _columnXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_computeXStore, j);
            guids[j++] = _computeXStore.Guid;
            foreach (var itm in _computeXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_relationXStore, j);
            guids[j++] = _relationXStore.Guid;
            foreach (var itm in _relationXStore.Items)
            {
                itemIndex.Add(itm, j);
                guids[j++] = itm.Guid;
            }

            itemIndex.Add(_relationStore, j);
            guids[j++] = _relationStore.Guid;
            foreach (var rel in _relationStore.Items)
            {
                itemIndex.Add(rel, j);
                guids[j++] = rel.Guid;
            }

            itemIndex.Add(_propertyStore, j); //for posible compute reference
            guids[j++] = _propertyStore.Guid;

            // put grandchild items at the end
            //=============================================
            foreach (var parent in _enumXStore.Items)
            {
                foreach (var child in parent.Items)
                {
                    itemIndex.Add(child, j);
                    guids[j++] = child.Guid;
                }
            }
            foreach (var parent in _tableXStore.Items)
            {
                foreach (var itm in parent.Items)
                {
                    var child = itm;
                    itemIndex.Add(child, j);
                    guids[j++] = child.Guid;
                }
            }
            return count;
        }
        #endregion

        #region GetRelationList  ==============================================
        // Get list of relations that reference at least one serialized item
        private List<Relation> GetRelationList()
        {
            var relationList = new List<Relation>(_relationStore.Count + _relationXStore.Count);

            Item[] parents;
            Item[] children;

            foreach (var rel in _relationStore.Items)
            {
                var len = rel.GetLinks(out parents, out children);
                for (int i = 0; i < len; i++)
                {
                    if (parents[i].IsExternal || children[i].IsExternal)
                    {
                        relationList.Add(rel);
                        break;
                    }
                }
            }

            foreach (var item in _relationXStore.Items)
            {
                var rel = item as RelationX;
                if (rel.HasLinks) relationList.Add(rel);
            }

            return relationList;
        }
        #endregion
    }
}
