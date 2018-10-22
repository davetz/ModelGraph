using System;
using ModelGraphSTD;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Collections.Generic;
using System.Diagnostics;

namespace RepositoryUWP
{
    public partial class RepositoryStorageFile : IRepository
    {
        #region Read  =========================================================
        public async void Read(Chef chef)
        {
            try
            {
                using (var stream = await _storageFile.OpenAsync(FileAccessMode.Read))
                {
                    using (DataReader r = new DataReader(stream))
                    {
                        r.ByteOrder = ByteOrder.LittleEndian;
                        UInt64 size = stream.Size;
                        if (size < UInt32.MaxValue)
                        {
                            var byteCount = await r.LoadAsync((UInt32)size);
                            Read(chef, r);
                        }
                    }
                }
                chef.PostReadValidation();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }
        #endregion

        #region Read  =========================================================
        private void Read(Chef chef, DataReader r)
        {
            var guidItems = chef.GetGuidItems();
            Item[] items;

            // determine the data file format
            var header = r.ReadInt32();
            var fileFormat = r.ReadGuid();

            Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[] vector = null;

            if (header == 0)
            {
                if (fileFormat == _fileFormat_D)
                {
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_3,       // 4 GraphX
                        ReadQueryX_6,       // 5 QueryX
                        ReadSymbolX_3,      // 6 SymbolX
                        ReadColumnX_4,      // 7 ColumnX
                        ReadComputeX_3,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_2,    // 10 RelationX
                        ReadGraphParm_6,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
                else if (fileFormat == _fileFormat_C)
                {
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_3,       // 4 GraphX
                        ReadQueryX_5,       // 5 QueryX
                        ReadSymbolX_2,      // 6 SymbolX
                        ReadColumnX_4,      // 7 ColumnX
                        ReadComputeX_3,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_2,    // 10 RelationX
                        ReadGraphParm_6,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
                else if (fileFormat == _fileFormat_B)
                {
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_3,       // 4 GraphX
                        ReadQueryX_5,       // 5 QueryX
                        ReadSymbolX_2,      // 6 SymbolX
                        ReadColumnX_4,      // 7 ColumnX
                        ReadComputeX_3,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_2,    // 10 RelationX
                        ReadGraphParm_5,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
                else if (fileFormat == _fileFormat_A)
                {
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_2,       // 4 GraphX
                        ReadQueryX_5,       // 5 QueryX
                        ReadSymbolX_2,      // 6 SymbolX
                        ReadColumnX_4,      // 7 ColumnX
                        ReadComputeX_3,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_2,    // 10 RelationX
                        ReadGraphParm_4,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
                else if (fileFormat == _fileFormat_9)
                {
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_1,       // 4 GraphX
                        ReadQueryX_5,       // 5 QueryX
                        ReadSymbolX_2,      // 6 SymbolX
                        ReadColumnX_4,      // 7 ColumnX
                        ReadComputeX_3,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_2,    // 10 RelationX
                        ReadGraphParm_3,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
                else if (fileFormat == _fileFormat_8)
                {
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_1,       // 4 GraphX
                        ReadQueryX_5,       // 5 QueryX
                        ReadSymbolX_2,      // 6 SymbolX
                        ReadColumnX_4,      // 7 ColumnX
                        ReadComputeX_3,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_2,    // 10 RelationX
                        ReadGraphParm_2,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
                else if (fileFormat == _fileFormat_7)
                {
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_1,       // 4 GraphX
                        ReadQueryX_4,       // 5 QueryX
                        ReadSymbolX_2,      // 6 SymbolX
                        ReadColumnX_4,      // 7 ColumnX
                        ReadComputeX_3,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_2,    // 10 RelationX
                        ReadGraphParm_2,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
                else if (fileFormat == _fileFormat_6)
                {
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_1,       // 4 GraphX
                        ReadQueryX_3,       // 5 QueryX
                        ReadSymbolX_2,      // 6 SymbolX
                        ReadColumnX_4,      // 7 ColumnX
                        ReadComputeX_3,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_2,    // 10 RelationX
                        ReadGraphParm_2,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
                else if (fileFormat == _fileFormat_5)
                {
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_1,       // 4 GraphX
                        ReadQueryX_3,       // 5 QueryX
                        ReadSymbolX_2,      // 6 SymbolX
                        ReadColumnX_4,      // 7 ColumnX
                        ReadComputeX_3,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_2,    // 10 RelationX
                        ReadGraphParm_1,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
                else if (fileFormat == _fileFormat_4)
                {
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_1,       // 4 GraphX
                        ReadQueryX_2,       // 5 QueryX
                        ReadSymbolX_1,      // 6 SymbolX
                        ReadColumnX_4,      // 7 ColumnX
                        ReadComputeX_3,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_2,    // 10 RelationX
                        ReadGraphParm_1,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
                else if (fileFormat == _fileFormat_3)
                {
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
                    {
                        null,               // 0
                        ReadViewX_1,        // 1 ViewX
                        ReadEnumX_1,        // 2 EnumX
                        ReadTableX_1,       // 3 TableX
                        ReadGraphX_1,       // 4 GraphX
                        ReadQueryX_2,       // 5 QueryX
                        ReadSymbolX_1,      // 6 SymbolX
                        ReadColumnX_3,      // 7 ColumnX
                        ReadComputeX_2,     // 8 ComputeX 
                        null,               // 9 CommandX
                        ReadRelationX_2,    // 10 RelationX
                        ReadGraphParm_1,    // 11 GraphParam
                        ReadRelationLink_1, // 12 RelationLink
                    };
                }
                else if (fileFormat == _fileFormat_2)
                {
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
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
                    vector = new Action<Chef, DataReader, Guid[], Item[], Dictionary<Guid, Item>>[]
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
                if (guids[i] != chef.Dummy.Guid) continue;
                items[i] = chef.Dummy; // DummyRef is a place holder used in the graph params dictionary 
                break;
            }

            for (; ; )
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
                    vector[vect](chef, r, guids, items, guidItems);
                }
                else
                {
                    throw new Exception($"Invalid Marker {mark}");
                }
            }
        }
        #endregion


        #region Guids  ========================================================
        private Guid[] ReadGuids(DataReader r)
        {
            var count = r.ReadInt32();

            var guids = new Guid[count];
            for (int i = 0; i < count; i++) { guids[i] = r.ReadGuid(); }

            return guids;
        }
        #endregion

        #region ReadViewX_1  ==================================================
        private void ReadViewX_1(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.ViewXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var vx = new ViewX(store, guids[index]);
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

        #region ReadEnumX_1  ==================================================
        private void ReadEnumX_1(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.EnumXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var ex = new EnumX(store, guids[index]);
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

        #region ReadTableX_1  =================================================
        private void ReadTableX_1(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.TableXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid table index {index}");

                var tx = new TableX(store, guids[index]);
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

        #region ReadGraphX_1  =================================================
        private void ReadGraphX_1(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.GraphXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var gx = new GraphX(store, guids[index]);
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

        #region ReadGraphX_2  =================================================
        private void ReadGraphX_2(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.GraphXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var gx = new GraphX(store, guids[index]);
                items[index] = gx;

                var b = r.ReadByte();
                if ((b & B1) != 0) gx.Name = ReadString(r);
                if ((b & B2) != 0) gx.Summary = ReadString(r);
                if ((b & B3) != 0) gx.Description = ReadString(r);
                gx.MinNodeSize = r.ReadByte();
                gx.ThinBusSize = r.ReadByte();
                gx.WideBusSize = r.ReadByte();
                gx.ExtraBusSize = r.ReadByte();
                gx.TerminalLength = r.ReadByte();
                gx.TerminalSpacing = r.ReadByte();
                gx.TerminalSkew = r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.GraphXEnding) throw new Exception($"Expected GraphXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadGraphX_3  =================================================
        private void ReadGraphX_3(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.GraphXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var gx = new GraphX(store, guids[index]);
                items[index] = gx;

                var b = r.ReadByte();
                if ((b & B1) != 0) gx.Name = ReadString(r);
                if ((b & B2) != 0) gx.Summary = ReadString(r);
                if ((b & B3) != 0) gx.Description = ReadString(r);

                gx.MinNodeSize = r.ReadByte();
                gx.ThinBusSize = r.ReadByte();
                gx.WideBusSize = r.ReadByte();
                gx.ExtraBusSize = r.ReadByte();

                gx.SurfaceSkew = r.ReadByte();
                gx.TerminalSkew = r.ReadByte();
                gx.TerminalLength = r.ReadByte();
                gx.TerminalSpacing = r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.GraphXEnding) throw new Exception($"Expected GraphXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadQueryX_1  =================================================
        private void ReadQueryX_1(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.QueryXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var qx = new QueryX(store, guids[index]);
                items[index] = qx;

                var n = r.ReadByte() & 0xF;
                qx.QueryKind = QueryType.Graph;
                qx.IsHead = (n == 3 || n == 5 || n == 7 || n == 13);
                qx.IsRoot = (n == 1 || n == 12 || n == 15);
                if (n == 3 || n == 4) qx.QueryKind = QueryType.Path;
                if (n == 5 || n == 6) qx.QueryKind = QueryType.Group;
                if (n == 7 || n == 8) qx.QueryKind = QueryType.Egress;
                if (n == 12 || n == 13 || n == 14) qx.QueryKind = QueryType.Value;
                if (n == 15) qx.QueryKind = QueryType.Symbol;

                var f = (r.ReadByte());
                if ((f & B1) != 0) qx.IsReversed = true;

                var b = r.ReadByte();
                if ((b & B1) != 0) qx.WhereString = ReadString(r);
                if ((b & B2) != 0) qx.SelectString = ReadString(r);
                if ((b & B3) != 0) qx.ExclusiveKey = r.ReadByte();

                if (qx.QueryKind == QueryType.Path && qx.IsHead) qx.PathParm = new PathParm();

                if ((b & B4) != 0) qx.PathParm.Connect1 = (Connect)r.ReadByte();
                if ((b & B5) != 0) qx.PathParm.Connect2 = (Connect)r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.QueryXEnding) throw new Exception($"Expected QueryXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadQueryX_2  =================================================
        private void ReadQueryX_2(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.QueryXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var qx = new QueryX(store, guids[index]);
                items[index] = qx;

                var b = r.ReadByte();
                if ((b & B1) != 0) qx.SetState(r.ReadUInt16());
                if ((b & B2) != 0) qx.WhereString = ReadString(r);
                if ((b & B3) != 0) qx.SelectString = ReadString(r);
                if ((b & B4) != 0) qx.ExclusiveKey = r.ReadByte();

                if (qx.QueryKind == QueryType.Path && qx.IsHead) qx.PathParm = new PathParm();

                if ((b & B5) != 0) qx.PathParm.Connect1 = (Connect)r.ReadByte();
                if ((b & B6) != 0) qx.PathParm.Connect2 = (Connect)r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.QueryXEnding) throw new Exception($"Expected QueryXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadQueryX_3  =================================================
        private void ReadQueryX_3(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.QueryXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var qx = new QueryX(store, guids[index]);
                items[index] = qx;

                var b = r.ReadByte();
                if ((b & B1) != 0) qx.SetState(r.ReadUInt16());
                if ((b & B2) != 0) qx.WhereString = ReadString(r);
                if ((b & B3) != 0) qx.SelectString = ReadString(r);
                if ((b & B4) != 0) qx.ExclusiveKey = r.ReadByte();

                if (qx.QueryKind == QueryType.Path && qx.IsHead) qx.PathParm = new PathParm();

                if ((b & B5) != 0) qx.PathParm.Connect1 = (Connect)r.ReadByte();
                if ((b & B6) != 0) qx.PathParm.Connect2 = (Connect)r.ReadByte();
                if ((b & B7) != 0) r.ReadByte();
                if ((b & B8) != 0) r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.QueryXEnding) throw new Exception($"Expected QueryXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadQueryX_4  =================================================
        private void ReadQueryX_4(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.QueryXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var qx = new QueryX(store, guids[index]);
                items[index] = qx;


                var b = r.ReadUInt16();
                if ((b & S1) != 0) qx.SetState(r.ReadUInt16());
                if ((b & S2) != 0) qx.WhereString = ReadString(r);
                if ((b & S3) != 0) qx.SelectString = ReadString(r);
                if ((b & S4) != 0) qx.ExclusiveKey = r.ReadByte();

                if (qx.QueryKind == QueryType.Path && qx.IsHead) qx.PathParm = new PathParm();

                if ((b & S5) != 0) qx.PathParm.Facet1 = (Facet)r.ReadByte();
                if ((b & S6) != 0) r.ReadByte();
                if ((b & S7) != 0) qx.PathParm.Connect1 = (Connect)r.ReadByte();

                if ((b & S8) != 0) qx.PathParm.Facet2 = (Facet)r.ReadByte();
                if ((b & S9) != 0) r.ReadByte();
                if ((b & S10) != 0) qx.PathParm.Connect2 = (Connect)r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.QueryXEnding) throw new Exception($"Expected QueryXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadQueryX_5  =================================================
        private void ReadQueryX_5(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.QueryXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var qx = new QueryX(store, guids[index]);
                items[index] = qx;


                var b = r.ReadUInt16();
                if ((b & S1) != 0) qx.SetState(r.ReadUInt16());
                if ((b & S2) != 0) qx.WhereString = ReadString(r);
                if ((b & S3) != 0) qx.SelectString = ReadString(r);
                if ((b & S4) != 0) qx.ExclusiveKey = r.ReadByte();

                if (qx.QueryKind == QueryType.Path && qx.IsHead) qx.PathParm = new PathParm();

                if ((b & S5) != 0) qx.PathParm.Facet1 = (Facet)r.ReadByte();
                if ((b & S6) != 0) r.ReadByte();
                if ((b & S7) != 0) qx.PathParm.Connect1 = (Connect)r.ReadByte();

                if ((b & S8) != 0) qx.PathParm.Facet2 = (Facet)r.ReadByte();
                if ((b & S9) != 0) r.ReadByte();
                if ((b & S10) != 0) qx.PathParm.Connect2 = (Connect)r.ReadByte();

                if ((b & S11) != 0) qx.PathParm.DashStyle = (DashStyle)r.ReadByte();
                if ((b & S12) != 0) qx.PathParm.LineStyle = (LineStyle)r.ReadByte();
                if ((b & S13) != 0) qx.PathParm.LineColor = ReadString(r);
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.QueryXEnding) throw new Exception($"Expected QueryXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadQueryX_6  =================================================
        private void ReadQueryX_6(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.QueryXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var qx = new QueryX(store, guids[index]);
                items[index] = qx;


                var b = r.ReadUInt16();
                if ((b & S1) != 0) qx.SetState(r.ReadUInt16());
                if ((b & S2) != 0) qx.WhereString = ReadString(r);
                if ((b & S3) != 0) qx.SelectString = ReadString(r);
                if ((b & S4) != 0) qx.ExclusiveKey = r.ReadByte();

                if (qx.QueryKind == QueryType.Path && qx.IsHead) qx.PathParm = new PathParm();

                if ((b & S5) != 0) qx.PathParm.Facet1 = (Facet)r.ReadByte();
                if ((b & S6) != 0) qx.PathParm.Connect1 = (Connect)r.ReadByte();

                if ((b & S7) != 0) qx.PathParm.Facet2 = (Facet)r.ReadByte();
                if ((b & S8) != 0) qx.PathParm.Connect2 = (Connect)r.ReadByte();

                if ((b & S9) != 0) qx.PathParm.DashStyle = (DashStyle)r.ReadByte();
                if ((b & S10) != 0) qx.PathParm.LineStyle = (LineStyle)r.ReadByte();
                if ((b & S11) != 0) qx.PathParm.LineColor = ReadString(r);
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.QueryXEnding) throw new Exception($"Expected QueryXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadSymbolX_1  ================================================
        private void ReadSymbolX_1(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.SymbolStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var sx = new SymbolX(store, guids[index]);
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

        #region ReadSymbolX_2  ================================================
        private void ReadSymbolX_2(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.SymbolStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var sx = new SymbolX(store, guids[index]);
                items[index] = sx;

                var b = r.ReadUInt16();
                if ((b & S1) != 0) sx.SetState(r.ReadUInt16());
                if ((b & S2) != 0) sx.Name = ReadString(r);
                if ((b & S3) != 0) sx.Summary = ReadString(r);
                if ((b & S4) != 0) sx.Description = ReadString(r);
                if ((b & S5) != 0) sx.Data = ReadBytes(r);
                if ((b & S6) != 0) sx.TopContact = (Contact)r.ReadByte();
                if ((b & S7) != 0) sx.LeftContact = (Contact)r.ReadByte();
                if ((b & S8) != 0) sx.RightContact = (Contact)r.ReadByte();
                if ((b & S9) != 0) sx.BottomContact = (Contact)r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.SymbolXEnding) throw new Exception($"Expected SymbolXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadSymbolX_3  ================================================
        private void ReadSymbolX_3(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.SymbolStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var sx = new SymbolX(store, guids[index]);
                items[index] = sx;

                var b = r.ReadUInt16();
                if ((b & S1) != 0) sx.SetState(r.ReadUInt16());
                if ((b & S2) != 0) sx.Name = ReadString(r);
                if ((b & S3) != 0) sx.Summary = ReadString(r);
                if ((b & S4) != 0) sx.Description = ReadString(r);
                if ((b & S5) != 0) sx.Data = ReadBytes(r);
                if ((b & S6) != 0) sx.Attach = (Attach)r.ReadByte();
                if ((b & S7) != 0) sx.TopContact = (Contact)r.ReadByte();
                if ((b & S8) != 0) sx.LeftContact = (Contact)r.ReadByte();
                if ((b & S9) != 0) sx.RightContact = (Contact)r.ReadByte();
                if ((b & S10) != 0) sx.BottomContact = (Contact)r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.SymbolXEnding) throw new Exception($"Expected SymbolXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadColumnX_1  ================================================
        private void ReadColumnX_1(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.ColumnXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid column index {index}");

                var cx = new ColumnX(store, guids[index]);
                items[index] = cx;

                var f = r.ReadByte();
                if ((f & B1) != 0) cx.IsChoice = true;

                var b = r.ReadByte();
                if ((b & B1) != 0) cx.Name = ReadString(r);
                if ((b & B2) != 0) cx.Summary = ReadString(r);
                if ((b & B3) != 0) cx.Initial = ReadString(r);
                if ((b & B4) != 0) cx.Description = ReadString(r);

                var type = ((b & B5) != 0) ? NewValType1(r.ReadByte()) : ValType.String;
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
                    if (!cx.Value.SetValue(rx, val)) throw new Exception($"Could not set value {val}");
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.ColumnXEnding) throw new Exception($"Expected ColumnXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadColumnX_2  ================================================
        private void ReadColumnX_2(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.ColumnXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid column index {index}");

                var cx = new ColumnX(store, guids[index]);
                items[index] = cx;

                var b = r.ReadByte();
                if ((b & B1) != 0) cx.SetState(r.ReadUInt16());
                if ((b & B2) != 0) cx.Name = ReadString(r);
                if ((b & B3) != 0) cx.Summary = ReadString(r);
                if ((b & B4) != 0) cx.Initial = ReadString(r);
                if ((b & B5) != 0) cx.Description = ReadString(r);

                var type = ((b & B6) != 0) ? NewValType1(r.ReadByte()) : ValType.String;
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
                    if (!cx.Value.SetValue(rx, val)) throw new Exception($"Could not set value {val}");
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.ColumnXEnding) throw new Exception($"Expected ColumnXEnding marker, instead got {mark}");
        }

        static ValType NewValType1(byte v) => (ValType)_newValType1[v];
        static byte[] _newValType1 = { 0, 2, 4, 6, 8, 10, 16, 10, 14, 18, 20, 22, 24, 28, 26, 28, 28,  3,  5,  5,  7,  9, 13, 17, 11, 15, 19, 21, 23, 25};
        //            _oldValType1 = { 0, 1, 2, 3, 4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29}; 
        static ValType NewValType2(byte v) => (ValType)_newValType2[v];
        static byte[] _newValType2 = {  0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28,  1,  3,  5,  7,  9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29};
        //            _oldValType2 = {  0, 1, 2, 3, 4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29}; 
        #endregion

        #region ReadColumnX_3  ================================================
        private void ReadColumnX_3(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.ColumnXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid column index {index}");

                var cx = new ColumnX(store, guids[index]);
                items[index] = cx;

                var b = r.ReadByte();
                if ((b & B1) != 0) cx.SetState(r.ReadUInt16());
                if ((b & B2) != 0) cx.Name = ReadString(r);
                if ((b & B3) != 0) cx.Summary = ReadString(r);
                if ((b & B5) != 0) cx.Description = ReadString(r);

                var t = NewValType2(r.ReadByte());

                ReadValueDictionary(r, t, cx, items);
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.ColumnXEnding) throw new Exception($"Expected ColumnXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadColumnX_4  ================================================
        private void ReadColumnX_4(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.ColumnXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid column index {index}");

                var cx = new ColumnX(store, guids[index]);
                items[index] = cx;

                var b = r.ReadByte();
                if ((b & B1) != 0) cx.SetState(r.ReadUInt16());
                if ((b & B2) != 0) cx.Name = ReadString(r);
                if ((b & B3) != 0) cx.Summary = ReadString(r);
                if ((b & B5) != 0) cx.Description = ReadString(r);

                var t = (ValType)r.ReadByte();

                ReadValueDictionary(r, t, cx, items);
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.ColumnXEnding) throw new Exception($"Expected ColumnXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadComputeX_1  ===============================================
        private void ReadComputeX_1(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.ComputeXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var cx = new ComputeX(store, guids[index]);
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
        private void ReadComputeX_2(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.ComputeXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var cx = new ComputeX(store, guids[index]);
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

        #region ReadComputeX_3  ===============================================
        private void ReadComputeX_3(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.ComputeXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var cx = new ComputeX(store, guids[index]);
                items[index] = cx;

                var S = r.ReadUInt16();                                     //01-27-2018 _fileFormat_4
                if ((S & S1) != 0) cx.Name = ReadString(r);
                if ((S & S2) != 0) cx.Summary = ReadString(r);
                if ((S & S3) != 0) cx.Description = ReadString(r);
                if ((S & S4) != 0) cx.Separator = ReadString(r);
                if ((S & S5) != 0) cx.CompuType = (CompuType)r.ReadByte();
                if ((S & S6) != 0) cx.NumericSet = (NumericSet)r.ReadByte();

                if ((S & S7) != 0) cx.Results = (Results)r.ReadByte();      //01-27-2018 _fileFormat_4
                if ((S & S8) != 0) cx.Sorting = (Sorting)r.ReadByte();
                if ((S & S9) != 0) cx.TakeSet = (TakeSet)r.ReadByte();
                if ((S & S10) != 0) cx.TakeLimit = r.ReadByte();
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.ComputeXEnding) throw new Exception($"Expected ComputeXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadRelationX_1  ==============================================
        private void ReadRelationX_1(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.RelationXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var rx = new RelationX(store, guids[index]);
                items[index] = rx;

                var f = r.ReadByte();
                if ((f & B2) != 0) rx.IsRequired = true;

                var b = r.ReadByte();
                if ((b & B1) != 0) rx.Name = ReadString(r);
                if ((b & B2) != 0) rx.Summary = ReadString(r);
                if ((b & B3) != 0) rx.Description = ReadString(r);
                if ((b & B4) != 0) rx.Pairing = (Pairing)r.ReadByte();
                if ((b & B5) != 0) r.ReadInt16();
                if ((b & B5) != 0) r.ReadInt16();
                var keyCount = ((b & B6) != 0) ? r.ReadInt32() : 0;
                var valCount = ((b & B6) != 0) ? r.ReadInt32() : 0;
                rx.Initialize(keyCount, valCount);
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.RelationXEnding) throw new Exception($"Expected RelationXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadRelationX_2  ==============================================
        private void ReadRelationX_2(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            var store = chef.RelationXStore;
            var count = r.ReadInt32();
            if (count < 0) throw new Exception($"Invalid count {count}");

            for (int i = 0; i < count; i++)
            {
                var index = r.ReadInt32();
                if (index < 0 || index >= items.Length) throw new Exception($"Invalid index {index}");

                var rx = new RelationX(store, guids[index]);
                items[index] = rx;

                var b = r.ReadByte();
                if ((b & B1) != 0) rx.SetState(r.ReadUInt16());
                if ((b & B2) != 0) rx.Name = ReadString(r);
                if ((b & B3) != 0) rx.Summary = ReadString(r);
                if ((b & B4) != 0) rx.Description = ReadString(r);
                if ((b & B5) != 0) rx.Pairing = (Pairing)r.ReadByte();
                if ((b & B6) != 0) r.ReadInt16();
                if ((b & B6) != 0) r.ReadInt16();
                var keyCount = ((b & B7) != 0) ? r.ReadInt32() : 0;
                var valCount = ((b & B7) != 0) ? r.ReadInt32() : 0;
                rx.Initialize(keyCount, valCount);
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.RelationXEnding) throw new Exception($"Expected RelationXEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadGraphParm_1  ==============================================
        private void ReadGraphParm_1(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            Dictionary<QueryX, List<NodeEdge>> Qx_NE = null;
            List<NodeEdge> NE = null;

            var gxIndex = r.ReadInt32();
            if (gxIndex < 0 || gxIndex >= items.Length) throw new Exception($"Invalid index {gxIndex}");

            var gxLen = r.ReadInt32();
            if (gxLen < 0) throw new Exception($"Invalid count {gxLen}");

            if (!(items[gxIndex] is GraphX gx)) throw new Exception($"Expected graphDef object, got null {gxIndex}");

            var graphParms = new Dictionary<GraphX, Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>>(gxLen);
            chef.GraphParms = graphParms;


            #region FindCreate Ri_Qx_NE =======================================
            if (!graphParms.TryGetValue(gx, out Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>> Ri_Qx_NE))
            {
                Ri_Qx_NE = new Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>(gxLen);
                graphParms.Add(gx, Ri_Qx_NE);
            }
            #endregion

            for (int i = 0; i < gxLen; i++)
            {
                var rtIndex = r.ReadInt32();
                if (rtIndex < 0 || rtIndex >= items.Length) throw new Exception($"Invalid index {rtIndex}");

                var rtLen = r.ReadInt32();
                if (rtLen < 0) throw new Exception($"Invalid count {rtLen}");

                var rt = items[rtIndex];
                if (rt == null) throw new Exception($"Expected root object, got null {rtIndex}");

                #region FindCreate Qx_NE ======================================
                if (!Ri_Qx_NE.TryGetValue(rt, out Qx_NE))
                {
                    Qx_NE = new Dictionary<QueryX, List<NodeEdge>>(rtLen);
                    Ri_Qx_NE.Add(rt, Qx_NE);
                }
                #endregion

                var Item_Node = new Dictionary<Item, Node>();

                for (int j = 0; j < rtLen; j++)
                {
                    var qxIndex = r.ReadInt32();
                    if (qxIndex < 0 || qxIndex >= items.Length) throw new Exception($"Invalid index {qxIndex}");

                    var npLen = r.ReadInt32();
                    if (npLen < 0) throw new Exception($"Invalid count {npLen}");

                    var qx = items[qxIndex] as QueryX;
                    if (qx == null) qx = chef.QueryXNode;
                    //if (qx == null)
                    //{
                    //    var guid = guids[qxIndex];
                    //    if (!guidItems.TryGetValue(guid, out Item itm)) throw new Exception($"Could not find QueryX for guid {guid}");
                    //    items[qxIndex] = itm;
                    //}

                    #region FindCreate  NE ====================================
                    if (!Qx_NE.TryGetValue(qx, out NE))
                    {
                        NE = new List<NodeEdge>(npLen);
                        Qx_NE.Add(qx, NE);
                    }
                    #endregion

                    if (npLen > 0)
                    {
                        if (qx == chef.QueryXNode)
                        {
                            #region ReadNodeParms  ============================
                            for (int k = 0; k < npLen; k++)
                            {
                                var itmIndex = r.ReadInt32();
                                if (itmIndex < 0 || itmIndex >= items.Length) throw new Exception($"Invalid index {itmIndex}");

                                var itm = items[itmIndex];
                                if (itm == null) throw new Exception($"Expected node item object, got null {itmIndex}");

                                if (!Item_Node.TryGetValue(itm, out Node node))
                                {
                                    node = new Node() { Item = itm };

                                    Item_Node.Add(itm, node);
                                    NE.Add(node);
                                }

                                node.X = r.ReadInt32();
                                node.Y = r.ReadInt32();
                                node.DX = r.ReadByte();
                                node.DY = r.ReadByte();
                                node.Symbol = r.ReadByte();
                                node.Aspect = (Aspect)r.ReadByte();
                                node.FlipRotate = (FlipRotate)r.ReadByte();
                                node.Labeling = (Labeling)r.ReadByte();
                                node.Sizing = (Sizing)r.ReadByte();
                                node.BarWidth = (BarWidth)r.ReadByte();
                            }
                            #endregion
                        }
                        else
                        {
                            #region ReadEdgeParms  ============================
                            for (int k = 0; k < npLen; k++)
                            {
                                var itm1Index = r.ReadInt32();
                                if (itm1Index < 0 || itm1Index >= items.Length) throw new Exception($"Invalid index {itm1Index}");

                                var itm1 = items[itm1Index];
                                if (itm1 == null) throw new Exception($"Expected node object, got null {itm1Index}");

                                var itm2Index = r.ReadInt32();
                                if (itm2Index < 0 || itm2Index >= items.Length) throw new Exception($"Invalid index {itm2Index}");

                                var itm2 = items[itm2Index];
                                if (itm2 == null) throw new Exception($"Expected node object, got null {itm2Index}");

                                if (!Item_Node.TryGetValue(itm1, out Node node1)) throw new Exception("Could not Finde Item1Node");
                                if (!Item_Node.TryGetValue(itm2, out Node node2)) throw new Exception("Could not Finde Item2Node");

                                var edge = new Edge(qx);
                                NE.Add(edge);

                                edge.Node1 = node1;
                                edge.Node2 = node2;

                                r.ReadByte();
                                r.ReadByte(); r.ReadByte();
                                r.ReadByte();
                                r.ReadByte(); r.ReadByte();
                                edge.Face1.Facet = (Facet)r.ReadByte();
                                edge.Face2.Facet = (Facet)r.ReadByte();

                                var pnCount = r.ReadUInt16();
                                if (pnCount > 0)
                                {
                                    edge.Bends = new(int X, int Y)[pnCount];
                                    for (int n = 0; n < pnCount; n++)
                                    {
                                        edge.Bends[n].X = r.ReadInt32();
                                        edge.Bends[n].Y = r.ReadInt32();
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.GraphParmEnding) throw new Exception($"Expected GraphParamEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadGraphParm_2  ==============================================
        private void ReadGraphParm_2(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            Dictionary<QueryX, List<NodeEdge>> Qx_NE = null;
            List<NodeEdge> NE = null;

            var gxIndex = r.ReadInt32();
            if (gxIndex < 0 || gxIndex >= items.Length) throw new Exception($"Invalid index {gxIndex}");

            var gxLen = r.ReadInt32();
            if (gxLen < 0) throw new Exception($"Invalid count {gxLen}");

            if (!(items[gxIndex] is GraphX gx)) throw new Exception($"Expected graphDef object, got null {gxIndex}");

            var graphParms = new Dictionary<GraphX, Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>>(gxLen);
            chef.GraphParms = graphParms;


            #region FindCreate Ri_Qx_NE =======================================
            if (!graphParms.TryGetValue(gx, out Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>> Ri_Qx_NE))
            {
                Ri_Qx_NE = new Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>(gxLen);
                graphParms.Add(gx, Ri_Qx_NE);
            }
            #endregion

            for (int i = 0; i < gxLen; i++)
            {
                var rtIndex = r.ReadInt32();
                if (rtIndex < 0 || rtIndex >= items.Length) throw new Exception($"Invalid index {rtIndex}");

                var rtLen = r.ReadInt32();
                if (rtLen < 0) throw new Exception($"Invalid count {rtLen}");

                var rt = items[rtIndex];
                if (rt == null) throw new Exception($"Expected root object, got null {rtIndex}");

                #region FindCreate Qx_NE ======================================
                if (!Ri_Qx_NE.TryGetValue(rt, out Qx_NE))
                {
                    Qx_NE = new Dictionary<QueryX, List<NodeEdge>>(rtLen);
                    Ri_Qx_NE.Add(rt, Qx_NE);
                }
                #endregion

                var Item_Node = new Dictionary<Item, Node>();

                for (int j = 0; j < rtLen; j++)
                {
                    var qxIndex = r.ReadInt32();
                    if (qxIndex < 0 || qxIndex >= items.Length) throw new Exception($"Invalid index {qxIndex}");

                    var npLen = r.ReadInt32();
                    if (npLen < 0) throw new Exception($"Invalid count {npLen}");

                    var qx = items[qxIndex] as QueryX;
                    if (qx == null)
                    {
                        var guid = guids[qxIndex];
                        if (!guidItems.TryGetValue(guid, out Item itm)) throw new Exception($"Could not find QueryX for guid {guid}");
                        items[qxIndex] = itm;
                        qx = itm as QueryX;
                    }

                    #region FindCreate  NE ====================================
                    if (!Qx_NE.TryGetValue(qx, out NE))
                    {
                        NE = new List<NodeEdge>(npLen);
                        Qx_NE.Add(qx, NE);
                    }
                    #endregion

                    if (npLen > 0)
                    {
                        if (qx == chef.QueryXNode)
                        {
                            #region ReadNodeParms  ============================
                            for (int k = 0; k < npLen; k++)
                            {
                                var itmIndex = r.ReadInt32();
                                if (itmIndex < 0 || itmIndex >= items.Length) throw new Exception($"Invalid index {itmIndex}");

                                var itm = items[itmIndex];
                                if (itm == null) throw new Exception($"Expected node item object, got null {itmIndex}");

                                if (!Item_Node.TryGetValue(itm, out Node node))
                                {
                                    node = new Node() { Item = itm };

                                    Item_Node.Add(itm, node);
                                    NE.Add(node);
                                }

                                node.X = r.ReadInt32();
                                node.Y = r.ReadInt32();
                                node.DX = r.ReadByte();
                                node.DY = r.ReadByte();
                                node.Symbol = r.ReadByte();
                                node.Aspect = (Aspect)r.ReadByte();
                                node.FlipRotate = (FlipRotate)r.ReadByte();
                                node.Labeling = (Labeling)r.ReadByte();
                                node.Sizing = (Sizing)r.ReadByte();
                                node.BarWidth = (BarWidth)r.ReadByte();
                            }
                            #endregion
                        }
                        else
                        {
                            #region ReadEdgeParms  ============================
                            for (int k = 0; k < npLen; k++)
                            {
                                var itm1Index = r.ReadInt32();
                                if (itm1Index < 0 || itm1Index >= items.Length) throw new Exception($"Invalid index {itm1Index}");

                                var itm1 = items[itm1Index];
                                if (itm1 == null) throw new Exception($"Expected node object, got null {itm1Index}");

                                var itm2Index = r.ReadInt32();
                                if (itm2Index < 0 || itm2Index >= items.Length) throw new Exception($"Invalid index {itm2Index}");

                                var itm2 = items[itm2Index];
                                if (itm2 == null) throw new Exception($"Expected node object, got null {itm2Index}");

                                if (!Item_Node.TryGetValue(itm1, out Node node1)) throw new Exception("Could not Finde Item1Node");
                                if (!Item_Node.TryGetValue(itm2, out Node node2)) throw new Exception("Could not Finde Item2Node");

                                var edge = new Edge(qx);
                                NE.Add(edge);

                                edge.Node1 = node1;
                                edge.Node2 = node2;

                                r.ReadByte();
                                r.ReadByte(); r.ReadByte();
                                r.ReadByte();
                                r.ReadByte(); //new parameter

                                r.ReadByte();
                                r.ReadByte(); r.ReadByte();
                                r.ReadByte();
                                r.ReadByte(); //new parameter

                                var pnCount = r.ReadUInt16();
                                if (pnCount > 0)
                                {
                                    edge.Bends = new(int X, int Y)[pnCount];
                                    for (int n = 0; n < pnCount; n++)
                                    {
                                        edge.Bends[n].X = r.ReadInt32();
                                        edge.Bends[n].Y = r.ReadInt32();
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.GraphParmEnding) throw new Exception($"Expected GraphParamEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadGraphParm_3  ==============================================
        private void ReadGraphParm_3(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            Dictionary<QueryX, List<NodeEdge>> Qx_NE = null;
            List<NodeEdge> NE = null;

            var gxIndex = r.ReadInt32();
            if (gxIndex < 0 || gxIndex >= items.Length) throw new Exception($"Invalid index {gxIndex}");

            var gxLen = r.ReadInt32();
            if (gxLen < 0) throw new Exception($"Invalid count {gxLen}");

            if (!(items[gxIndex] is GraphX gx)) throw new Exception($"Expected graphDef object, got null {gxIndex}");

            var graphParms = new Dictionary<GraphX, Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>>(gxLen);
            chef.GraphParms = graphParms;


            #region FindCreate Ri_Qx_NE =======================================
            if (!graphParms.TryGetValue(gx, out Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>> Ri_Qx_NE))
            {
                Ri_Qx_NE = new Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>(gxLen);
                graphParms.Add(gx, Ri_Qx_NE);
            }
            #endregion

            for (int i = 0; i < gxLen; i++)
            {
                var rtIndex = r.ReadInt32();
                if (rtIndex < 0 || rtIndex >= items.Length) throw new Exception($"Invalid index {rtIndex}");

                var rtLen = r.ReadInt32();
                if (rtLen < 0) throw new Exception($"Invalid count {rtLen}");

                var rt = items[rtIndex];
                if (rt == null) throw new Exception($"Expected root object, got null {rtIndex}");

                #region FindCreate Qx_NE ======================================
                if (!Ri_Qx_NE.TryGetValue(rt, out Qx_NE))
                {
                    Qx_NE = new Dictionary<QueryX, List<NodeEdge>>(rtLen);
                    Ri_Qx_NE.Add(rt, Qx_NE);
                }
                #endregion

                var Item_Node = new Dictionary<Item, Node>();

                for (int j = 0; j < rtLen; j++)
                {
                    var qxIndex = r.ReadInt32();
                    if (qxIndex < 0 || qxIndex >= items.Length) throw new Exception($"Invalid index {qxIndex}");

                    var npLen = r.ReadInt32();
                    if (npLen < 0) throw new Exception($"Invalid count {npLen}");

                    var qx = items[qxIndex] as QueryX;
                    if (qx == null)
                    {
                        var guid = guids[qxIndex];
                        if (!guidItems.TryGetValue(guid, out Item itm)) throw new Exception($"Could not find QueryX for guid {guid}");
                        items[qxIndex] = itm;
                        qx = itm as QueryX;
                    }

                    #region FindCreate  NE ====================================
                    if (!Qx_NE.TryGetValue(qx, out NE))
                    {
                        NE = new List<NodeEdge>(npLen);
                        Qx_NE.Add(qx, NE);
                    }
                    #endregion

                    if (npLen > 0)
                    {
                        if (qx == chef.QueryXNode)
                        {
                            #region ReadNodeParms  ============================
                            for (int k = 0; k < npLen; k++)
                            {
                                var itmIndex = r.ReadInt32();
                                if (itmIndex < 0 || itmIndex >= items.Length) throw new Exception($"Invalid index {itmIndex}");

                                var itm = items[itmIndex];
                                if (itm == null) throw new Exception($"Expected node item object, got null {itmIndex}");

                                if (!Item_Node.TryGetValue(itm, out Node node))
                                {
                                    node = new Node() { Item = itm };

                                    Item_Node.Add(itm, node);
                                    NE.Add(node);
                                }

                                node.X = r.ReadInt32();
                                node.Y = r.ReadInt32();
                                node.DX = r.ReadByte();
                                node.DY = r.ReadByte();
                                node.Symbol = r.ReadByte();
                                node.Aspect = (Aspect)r.ReadByte();
                                node.FlipRotate = (FlipRotate)r.ReadByte();
                                node.Labeling = (Labeling)r.ReadByte();
                                node.Sizing = (Sizing)r.ReadByte();
                                node.BarWidth = (BarWidth)r.ReadByte();
                            }
                            #endregion
                        }
                        else
                        {
                            #region ReadEdgeParms  ============================
                            for (int k = 0; k < npLen; k++)
                            {
                                var itm1Index = r.ReadInt32();
                                if (itm1Index < 0 || itm1Index >= items.Length) throw new Exception($"Invalid index {itm1Index}");

                                var itm1 = items[itm1Index];
                                if (itm1 == null) throw new Exception($"Expected node object, got null {itm1Index}");

                                var itm2Index = r.ReadInt32();
                                if (itm2Index < 0 || itm2Index >= items.Length) throw new Exception($"Invalid index {itm2Index}");

                                var itm2 = items[itm2Index];
                                if (itm2 == null) throw new Exception($"Expected node object, got null {itm2Index}");

                                if (!Item_Node.TryGetValue(itm1, out Node node1)) throw new Exception("Could not Finde Item1Node");
                                if (!Item_Node.TryGetValue(itm2, out Node node2)) throw new Exception("Could not Finde Item2Node");

                                var edge = new Edge(qx);
                                NE.Add(edge);

                                edge.Node1 = node1;
                                edge.Node2 = node2;

                                r.ReadByte();
                                r.ReadByte(); r.ReadByte();
                                edge.Face1.Facet = (Facet)r.ReadByte();

                                r.ReadByte();
                                r.ReadByte(); r.ReadByte();
                                edge.Face2.Facet = (Facet)r.ReadByte();

                                var pnCount = r.ReadUInt16();
                                if (pnCount > 0)
                                {
                                    edge.Bends = new(int X, int Y)[pnCount];
                                    for (int n = 0; n < pnCount; n++)
                                    {
                                        edge.Bends[n].X = r.ReadInt32();
                                        edge.Bends[n].Y = r.ReadInt32();
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.GraphParmEnding) throw new Exception($"Expected GraphParamEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadGraphParm_4  ==============================================
        private void ReadGraphParm_4(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            Dictionary<QueryX, List<NodeEdge>> Qx_NE = null;
            List<NodeEdge> NE = null;

            var gxIndex = r.ReadInt32();
            if (gxIndex < 0 || gxIndex >= items.Length) throw new Exception($"Invalid index {gxIndex}");

            var gxLen = r.ReadInt32();
            if (gxLen < 0) throw new Exception($"Invalid count {gxLen}");

            if (!(items[gxIndex] is GraphX gx)) throw new Exception($"Expected graphDef object, got null {gxIndex}");

            var graphParms = new Dictionary<GraphX, Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>>(gxLen);
            chef.GraphParms = graphParms;


            #region FindCreate Ri_Qx_NE =======================================
            if (!graphParms.TryGetValue(gx, out Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>> Ri_Qx_NE))
            {
                Ri_Qx_NE = new Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>(gxLen);
                graphParms.Add(gx, Ri_Qx_NE);
            }
            #endregion

            for (int i = 0; i < gxLen; i++)
            {
                var rtIndex = r.ReadInt32();
                if (rtIndex < 0 || rtIndex >= items.Length) throw new Exception($"Invalid index {rtIndex}");

                var rtLen = r.ReadInt32();
                if (rtLen < 0) throw new Exception($"Invalid count {rtLen}");

                var rt = items[rtIndex];
                if (rt == null) throw new Exception($"Expected root object, got null {rtIndex}");

                #region FindCreate Qx_NE ======================================
                if (!Ri_Qx_NE.TryGetValue(rt, out Qx_NE))
                {
                    Qx_NE = new Dictionary<QueryX, List<NodeEdge>>(rtLen);
                    Ri_Qx_NE.Add(rt, Qx_NE);
                }
                #endregion

                var Item_Node = new Dictionary<Item, Node>();

                for (int j = 0; j < rtLen; j++)
                {
                    var qxIndex = r.ReadInt32();
                    if (qxIndex < 0 || qxIndex >= items.Length) throw new Exception($"Invalid index {qxIndex}");

                    var npLen = r.ReadInt32();
                    if (npLen < 0) throw new Exception($"Invalid count {npLen}");

                    var qx = items[qxIndex] as QueryX;
                    if (qx == null)
                    {
                        var guid = guids[qxIndex];
                        if (!guidItems.TryGetValue(guid, out Item itm)) throw new Exception($"Could not find QueryX for guid {guid}");
                        items[qxIndex] = itm;
                        qx = itm as QueryX;
                    }

                    #region FindCreate  NE ====================================
                    if (!Qx_NE.TryGetValue(qx, out NE))
                    {
                        NE = new List<NodeEdge>(npLen);
                        Qx_NE.Add(qx, NE);
                    }
                    #endregion

                    if (npLen > 0)
                    {
                        if (qx == chef.QueryXNode)
                        {
                            #region ReadNodeParms  ============================
                            for (int k = 0; k < npLen; k++)
                            {
                                var itmIndex = r.ReadInt32();
                                if (itmIndex < 0 || itmIndex >= items.Length) throw new Exception($"Invalid index {itmIndex}");

                                var itm = items[itmIndex];
                                if (itm == null) throw new Exception($"Expected node item object, got null {itmIndex}");

                                if (!Item_Node.TryGetValue(itm, out Node node))
                                {
                                    node = new Node() { Item = itm };

                                    Item_Node.Add(itm, node);
                                    NE.Add(node);
                                }

                                node.X = r.ReadInt32();
                                node.Y = r.ReadInt32();
                                node.DX = r.ReadByte();
                                node.DY = r.ReadByte();
                                node.Symbol = r.ReadByte();
                                node.Aspect = (Aspect)r.ReadByte();
                                node.FlipRotate = (FlipRotate)r.ReadByte();
                                node.Labeling = (Labeling)r.ReadByte();
                                node.Sizing = (Sizing)r.ReadByte();
                                node.BarWidth = (BarWidth)r.ReadByte();
                            }
                            #endregion
                        }
                        else
                        {
                            #region ReadEdgeParms  ============================
                            for (int k = 0; k < npLen; k++)
                            {
                                var itm1Index = r.ReadInt32();
                                if (itm1Index < 0 || itm1Index >= items.Length) throw new Exception($"Invalid index {itm1Index}");

                                var itm1 = items[itm1Index];
                                if (itm1 == null) throw new Exception($"Expected node object, got null {itm1Index}");

                                var itm2Index = r.ReadInt32();
                                if (itm2Index < 0 || itm2Index >= items.Length) throw new Exception($"Invalid index {itm2Index}");

                                var itm2 = items[itm2Index];
                                if (itm2 == null) throw new Exception($"Expected node object, got null {itm2Index}");

                                if (!Item_Node.TryGetValue(itm1, out Node node1)) throw new Exception("Could not Finde Item1Node");
                                if (!Item_Node.TryGetValue(itm2, out Node node2)) throw new Exception("Could not Finde Item2Node");

                                var edge = new Edge(qx);
                                NE.Add(edge);

                                edge.Node1 = node1;
                                edge.Node2 = node2;

                                r.ReadByte();
                                r.ReadByte();
                                r.ReadInt16();

                                r.ReadByte();
                                edge.Face2.Facet = (Facet)r.ReadByte();
                                r.ReadInt16();

                                var pnCount = r.ReadUInt16();
                                if (pnCount > 0)
                                {
                                    edge.Bends = new (int X, int Y)[pnCount];
                                    for (int n = 0; n < pnCount; n++)
                                    {
                                        edge.Bends[n].X = r.ReadInt32();
                                        edge.Bends[n].Y = r.ReadInt32();
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.GraphParmEnding) throw new Exception($"Expected GraphParamEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadGraphParm_5  ==============================================
        private void ReadGraphParm_5(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            Dictionary<QueryX, List<NodeEdge>> Qx_NE = null;
            List<NodeEdge> NE = null;

            var gxIndex = r.ReadInt32();
            if (gxIndex < 0 || gxIndex >= items.Length) throw new Exception($"Invalid index {gxIndex}");

            var gxLen = r.ReadInt32();
            if (gxLen < 0) throw new Exception($"Invalid count {gxLen}");

            if (!(items[gxIndex] is GraphX gx)) throw new Exception($"Expected graphDef object, got null {gxIndex}");

            var graphParms = new Dictionary<GraphX, Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>>(gxLen);
            chef.GraphParms = graphParms;


            #region FindCreate Ri_Qx_NE =======================================
            if (!graphParms.TryGetValue(gx, out Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>> Ri_Qx_NE))
            {
                Ri_Qx_NE = new Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>(gxLen);
                graphParms.Add(gx, Ri_Qx_NE);
            }
            #endregion

            for (int i = 0; i < gxLen; i++)
            {
                var rtIndex = r.ReadInt32();
                if (rtIndex < 0 || rtIndex >= items.Length) throw new Exception($"Invalid index {rtIndex}");

                var rtLen = r.ReadInt32();
                if (rtLen < 0) throw new Exception($"Invalid count {rtLen}");

                var rt = items[rtIndex];
                if (rt == null) throw new Exception($"Expected root object, got null {rtIndex}");

                #region FindCreate Qx_NE ======================================
                if (!Ri_Qx_NE.TryGetValue(rt, out Qx_NE))
                {
                    Qx_NE = new Dictionary<QueryX, List<NodeEdge>>(rtLen);
                    Ri_Qx_NE.Add(rt, Qx_NE);
                }
                #endregion

                var Item_Node = new Dictionary<Item, Node>();

                for (int j = 0; j < rtLen; j++)
                {
                    var qxIndex = r.ReadInt32();
                    if (qxIndex < 0 || qxIndex >= items.Length) throw new Exception($"Invalid index {qxIndex}");

                    var npLen = r.ReadInt32();
                    if (npLen < 0) throw new Exception($"Invalid count {npLen}");

                    var qx = items[qxIndex] as QueryX;
                    if (qx == null)
                    {
                        var guid = guids[qxIndex];
                        if (!guidItems.TryGetValue(guid, out Item itm)) throw new Exception($"Could not find QueryX for guid {guid}");
                        items[qxIndex] = itm;
                        qx = itm as QueryX;
                    }

                    #region FindCreate  NE ====================================
                    if (!Qx_NE.TryGetValue(qx, out NE))
                    {
                        NE = new List<NodeEdge>(npLen);
                        Qx_NE.Add(qx, NE);
                    }
                    #endregion

                    if (npLen > 0)
                    {
                        if (qx == chef.QueryXNode)
                        {
                            #region ReadNodeParms  ============================
                            for (int k = 0; k < npLen; k++)
                            {
                                var itmIndex = r.ReadInt32();
                                if (itmIndex < 0 || itmIndex >= items.Length) throw new Exception($"Invalid index {itmIndex}");

                                var itm = items[itmIndex];
                                if (itm == null) throw new Exception($"Expected node item object, got null {itmIndex}");

                                if (!Item_Node.TryGetValue(itm, out Node node))
                                {
                                    node = new Node() { Item = itm };

                                    Item_Node.Add(itm, node);
                                    NE.Add(node);
                                }

                                node.X = r.ReadInt32();
                                node.Y = r.ReadInt32();
                                node.DX = r.ReadByte();
                                node.DY = r.ReadByte();
                                node.Symbol = r.ReadByte();
                                node.Aspect = (Aspect)r.ReadByte();
                                node.FlipRotate = (FlipRotate)r.ReadByte();
                                node.Labeling = (Labeling)r.ReadByte();
                                node.Sizing = (Sizing)r.ReadByte();
                                node.BarWidth = (BarWidth)r.ReadByte();
                            }
                            #endregion
                        }
                        else
                        {
                            #region ReadEdgeParms  ============================
                            for (int k = 0; k < npLen; k++)
                            {
                                var itm1Index = r.ReadInt32();
                                if (itm1Index < 0 || itm1Index >= items.Length) throw new Exception($"Invalid index {itm1Index}");

                                var itm1 = items[itm1Index];
                                if (itm1 == null) throw new Exception($"Expected node object, got null {itm1Index}");

                                var itm2Index = r.ReadInt32();
                                if (itm2Index < 0 || itm2Index >= items.Length) throw new Exception($"Invalid index {itm2Index}");

                                var itm2 = items[itm2Index];
                                if (itm2 == null) throw new Exception($"Expected node object, got null {itm2Index}");

                                if (!Item_Node.TryGetValue(itm1, out Node node1)) throw new Exception("Could not Finde Item1Node");
                                if (!Item_Node.TryGetValue(itm2, out Node node2)) throw new Exception("Could not Finde Item2Node");

                                var edge = new Edge(qx);
                                NE.Add(edge);

                                edge.Node1 = node1;
                                edge.Node2 = node2;

                                r.ReadByte();
                                edge.Face1.Facet = (Facet)r.ReadByte();
                                r.ReadByte(); r.ReadByte();
                                r.ReadByte(); r.ReadByte();

                                r.ReadByte();
                                edge.Face2.Facet = (Facet)r.ReadByte();
                                r.ReadByte(); r.ReadByte();
                                r.ReadByte(); r.ReadByte();

                                var pnCount = r.ReadUInt16();
                                if (pnCount > 0)
                                {
                                    edge.Bends = new (int X, int Y)[pnCount];
                                    for (int n = 0; n < pnCount; n++)
                                    {
                                        edge.Bends[n].X = r.ReadInt32();
                                        edge.Bends[n].Y = r.ReadInt32();
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.GraphParmEnding) throw new Exception($"Expected GraphParamEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadGraphParm_6  ==============================================
        private void ReadGraphParm_6(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
        {
            Dictionary<QueryX, List<NodeEdge>> Qx_NE = null;
            List<NodeEdge> NE = null;

            var gxIndex = r.ReadInt32();
            if (gxIndex < 0 || gxIndex >= items.Length) throw new Exception($"Invalid index {gxIndex}");

            var gxLen = r.ReadInt32();
            if (gxLen < 0) throw new Exception($"Invalid count {gxLen}");

            if (!(items[gxIndex] is GraphX gx)) throw new Exception($"Expected graphDef object, got null {gxIndex}");

            var graphParms = new Dictionary<GraphX, Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>>(gxLen);
            chef.GraphParms = graphParms;


            #region FindCreate Ri_Qx_NE =======================================
            if (!graphParms.TryGetValue(gx, out Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>> Ri_Qx_NE))
            {
                Ri_Qx_NE = new Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>(gxLen);
                graphParms.Add(gx, Ri_Qx_NE);
            }
            #endregion

            for (int i = 0; i < gxLen; i++)
            {
                var rtIndex = r.ReadInt32();
                if (rtIndex < 0 || rtIndex >= items.Length) throw new Exception($"Invalid index {rtIndex}");

                var rtLen = r.ReadInt32();
                if (rtLen < 0) throw new Exception($"Invalid count {rtLen}");

                var rt = items[rtIndex];
                if (rt == null) throw new Exception($"Expected root object, got null {rtIndex}");

                #region FindCreate Qx_NE ======================================
                if (!Ri_Qx_NE.TryGetValue(rt, out Qx_NE))
                {
                    Qx_NE = new Dictionary<QueryX, List<NodeEdge>>(rtLen);
                    Ri_Qx_NE.Add(rt, Qx_NE);
                }
                #endregion

                var Item_Node = new Dictionary<Item, Node>();

                for (int j = 0; j < rtLen; j++)
                {
                    var qxIndex = r.ReadInt32();
                    if (qxIndex < 0 || qxIndex >= items.Length) throw new Exception($"Invalid index {qxIndex}");

                    var npLen = r.ReadInt32();
                    if (npLen < 0) throw new Exception($"Invalid count {npLen}");

                    if (!(items[qxIndex] is QueryX qx))
                    {
                        var guid = guids[qxIndex];
                        if (!guidItems.TryGetValue(guid, out Item itm)) throw new Exception($"Could not find QueryX for guid {guid}");
                        items[qxIndex] = itm;
                        qx = itm as QueryX;
                    }

                    #region FindCreate  NE ====================================
                    if (!Qx_NE.TryGetValue(qx, out NE))
                    {
                        NE = new List<NodeEdge>(npLen);
                        Qx_NE.Add(qx, NE);
                    }
                    #endregion

                    if (npLen > 0)
                    {
                        if (qx == chef.QueryXNode)
                        {
                            #region ReadNodeParms  ============================
                            for (int k = 0; k < npLen; k++)
                            {
                                var itmIndex = r.ReadInt32();
                                if (itmIndex < 0 || itmIndex >= items.Length) throw new Exception($"Invalid index {itmIndex}");

                                var itm = items[itmIndex];
                                if (itm == null) throw new Exception($"Expected node item object, got null {itmIndex}");

                                if (!Item_Node.TryGetValue(itm, out Node node))
                                {
                                    node = new Node() { Item = itm };

                                    Item_Node.Add(itm, node);
                                    NE.Add(node);
                                }

                                node.X = r.ReadInt32();
                                node.Y = r.ReadInt32();
                                node.DX = r.ReadByte();
                                node.DY = r.ReadByte();
                                node.Symbol = r.ReadByte();
                                node.Aspect = (Aspect)r.ReadByte();
                                node.FlipRotate = (FlipRotate)r.ReadByte();
                                node.Labeling = (Labeling)r.ReadByte();
                                node.Sizing = (Sizing)r.ReadByte();
                                node.BarWidth = (BarWidth)r.ReadByte();
                            }
                            #endregion
                        }
                        else
                        {
                            #region ReadEdgeParms  ============================
                            for (int k = 0; k < npLen; k++)
                            {
                                var itm1Index = r.ReadInt32();
                                if (itm1Index < 0 || itm1Index >= items.Length) throw new Exception($"Invalid index {itm1Index}");

                                var itm1 = items[itm1Index];
                                if (itm1 == null) throw new Exception($"Expected node object, got null {itm1Index}");

                                var itm2Index = r.ReadInt32();
                                if (itm2Index < 0 || itm2Index >= items.Length) throw new Exception($"Invalid index {itm2Index}");

                                var itm2 = items[itm2Index];
                                if (itm2 == null) throw new Exception($"Expected node object, got null {itm2Index}");

                                if (!Item_Node.TryGetValue(itm1, out Node node1)) throw new Exception("Could not Finde Item1Node");
                                if (!Item_Node.TryGetValue(itm2, out Node node2)) throw new Exception("Could not Finde Item2Node");

                                var edge = new Edge(qx);
                                NE.Add(edge);

                                edge.Node1 = node1;
                                edge.Node2 = node2;

                                edge.Face1 = new Face((Facet)r.ReadByte(), (r.ReadInt16(), r.ReadInt16()), (r.ReadInt16(), r.ReadInt16()), (r.ReadInt16(), r.ReadInt16()));
                                edge.Face2 = new Face((Facet)r.ReadByte(), (r.ReadInt16(), r.ReadInt16()), (r.ReadInt16(), r.ReadInt16()), (r.ReadInt16(), r.ReadInt16()));

                                var pnCount = r.ReadUInt16();
                                if (pnCount > 0)
                                {
                                    edge.Bends = new (int X, int Y)[pnCount];
                                    for (int n = 0; n < pnCount; n++)
                                    {
                                        edge.Bends[n].X = r.ReadInt32();
                                        edge.Bends[n].Y = r.ReadInt32();
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            var mark = (Mark)r.ReadByte();
            if (mark != Mark.GraphParmEnding) throw new Exception($"Expected GraphParamEnding marker, instead got {mark}");
        }
        #endregion

        #region ReadRelationLink_1  ===========================================
        private void ReadRelationLink_1(Chef chef, DataReader r, Guid[] guids, Item[] items, Dictionary<Guid, Item> guidItems)
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

        #region Read String/Bytes  ============================================
        static string ReadString(DataReader r)
        {
            var len = (UInt32)r.ReadUInt16();
            var str = r.ReadString(len);
            return (str == "^") ? string.Empty : str;
        }
        static byte[] ReadBytes(DataReader r)
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
    }
}
