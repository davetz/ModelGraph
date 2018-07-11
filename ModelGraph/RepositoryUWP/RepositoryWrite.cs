using System;
using ModelGraphSTD;
using Windows.Storage.Streams;
using System.Collections.Generic;

namespace RepositoryUWP
{
    public partial class RepositoryStorageFile : IRepository
    {

        #region Write  ========================================================
        public async void Write(Chef chef)
        {
            try
            {
                using (var tran = await _storageFile.OpenTransactedWriteAsync())
                {
                    using (var w = new DataWriter(tran.Stream))
                    {
                        w.ByteOrder = ByteOrder.LittleEndian;
                        Write(chef, w);
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

        #region Write  ========================================================
        private void Write(Chef chef, DataWriter w)
        {
            var fileFormat = _fileFormat_4;
            var itemCount = chef.GetGuidItemIndex(out Guid[] guids, out Dictionary<Item, int> itemIndex);
            var relationList = chef.GetRelationList();

            w.WriteInt32(0);
            w.WriteGuid(fileFormat);

            WriteGuids(w, guids);

            if (chef.T_ViewXStore.Count > 0) WriteViewX(chef, w, itemIndex);
            if (chef.T_EnumXStore.Count > 0) WriteEnumX(chef, w, itemIndex);
            if (chef.T_TableXStore.Count > 0) WriteTableX(chef, w, itemIndex);
            if (chef.T_GraphXStore.Count > 0) WriteGraphX(chef, w, itemIndex);
            if (chef.T_QueryXStore.Count > 0) WriteQueryX(chef, w, itemIndex);
            if (chef.T_SymbolStore.Count > 0) WriteSymbolX(chef, w, itemIndex);
            if (chef.T_ColumnXStore.Count > 0) WriteColumnX(chef, w, itemIndex);
            if (chef.T_ComputeXStore.Count > 0) WriteComputeX(chef, w, itemIndex);
            if (chef.T_RelationXStore.Count > 0) WriteRelationX(chef, w, itemIndex);

            if (chef.T_GraphParms.Count > 0) WriteGraphParm(chef, w, itemIndex);
            if (relationList.Count > 0) WriteRelationLink(chef, w, relationList, itemIndex);

            w.WriteByte((byte)Mark.StorageFileEnding);
            w.WriteGuid(fileFormat);
            w.WriteInt32(0);
        }
        #endregion


        #region Guids  ========================================================
        private void WriteGuids(DataWriter w, Guid[] guids)
        {
            w.WriteInt32(guids.Length);

            foreach (var g in guids) { w.WriteGuid(g); }
        }
        #endregion

        #region WriteViewX  ===================================================
        private void WriteViewX(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.ViewXBegin); // type index
            w.WriteInt32(chef.T_ViewXStore.Count);

            foreach (var view in chef.T_ViewXStore.Items)
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


        #region WriteEnumX  ===================================================
        private void WriteEnumX(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.EnumXBegin); // type vector index
            w.WriteInt32(chef.T_EnumXStore.Count);

            foreach (var ex in chef.T_EnumXStore.Items)
            {
                w.WriteInt32(itemIndex[ex]);

                var b = BZ;
                if (!string.IsNullOrWhiteSpace(ex.Name)) b |= B1;
                if (!string.IsNullOrWhiteSpace(ex.Summary)) b |= B2;
                if (!string.IsNullOrWhiteSpace(ex.Description)) b |= B3;

                w.WriteByte(b);
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

        #region WriteTableX  ==================================================
        private void WriteTableX(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.TableXBegin); // type index
            w.WriteInt32(chef.T_TableXStore.Count);

            foreach (var tx in chef.T_TableXStore.Items)
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

        #region WriteGraphX  ==================================================
        private void WriteGraphX(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.GraphXBegin); // type index
            w.WriteInt32(chef.T_GraphXStore.Count);

            foreach (var gx in chef.T_GraphXStore.Items)
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

        #region WriteQueryX  ==================================================
        private void WriteQueryX(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.QueryXBegin); // type vector
            w.WriteInt32(chef.T_QueryXStore.Count);

            foreach (var qx in chef.T_QueryXStore.Items)
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

        #region WriteSymbolX  =================================================
        private void WriteSymbolX(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.SymbolXBegin); // type index
            w.WriteInt32(chef.T_SymbolStore.Count);

            foreach (var sx in chef.T_SymbolStore.Items)
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

        #region WriteColumnX  =================================================
        private void WriteColumnX(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.ColumnXBegin); // type index
            w.WriteInt32(chef.T_ColumnXStore.Count);

            foreach (var cx in chef.T_ColumnXStore.Items)
            {
                w.WriteInt32(itemIndex[cx]);

                var b = BZ;
                if (cx.HasFlags()) b |= B1;
                if (!string.IsNullOrWhiteSpace(cx.Name)) b |= B2;
                if (!string.IsNullOrWhiteSpace(cx.Summary)) b |= B3;
                if (!string.IsNullOrWhiteSpace(cx.Description)) b |= B4;

                w.WriteByte(b);
                if ((b & B1) != 0) w.WriteUInt16(cx.GetFlags());
                if ((b & B2) != 0) WriteString(w, cx.Name);
                if ((b & B3) != 0) WriteString(w, cx.Summary);
                if ((b & B5) != 0) WriteString(w, cx.Description);

                w.WriteByte((byte)cx.Value.ValType);

                WriteValueDictionary(w, cx, itemIndex);
            }
            w.WriteByte((byte)Mark.ColumnXEnding); // itegrity marker
        }
        #endregion

        #region WriteComputeX  ================================================
        private void WriteComputeX(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.ComputeXBegin); // type vector
            w.WriteInt32(chef.T_ComputeXStore.Count);

            foreach (var cx in chef.T_ComputeXStore.Items)
            {
                w.WriteInt32(itemIndex[cx]);

                var S = SZ;
                if (!string.IsNullOrWhiteSpace(cx.Name)) S |= S1;
                if (!string.IsNullOrWhiteSpace(cx.Summary)) S |= S2;
                if (!string.IsNullOrWhiteSpace(cx.Description)) S |= S3;
                if (cx.Separator != ComputeX.DefaultSeparator) S |= S4;
                if (cx.CompuType != CompuType.RowValue) S |= S5;
                if (cx.NumericSet != NumericSet.Count) S |= S6;

                if (cx.Results != Results.OneValue) S |= S7;        //01-27-2018 _fileFormat_4
                if (cx.Sorting != Sorting.Unsorted) S |= S8;
                if (cx.TakeSet != TakeSet.First) S |= S9;
                if (cx.TakeLimit != 0) S |= S10;

                w.WriteUInt16(S);                                   //01-27-2018 _fileFormat_4
                if ((S & S1) != 0) WriteString(w, cx.Name);
                if ((S & S2) != 0) WriteString(w, cx.Summary);
                if ((S & S3) != 0) WriteString(w, cx.Description);
                if ((S & S4) != 0) WriteString(w, (cx.Separator ?? string.Empty));
                if ((S & S5) != 0) w.WriteByte((byte)cx.CompuType);
                if ((S & S6) != 0) w.WriteByte((byte)cx.NumericSet);

                if ((S & S7) != 0) w.WriteByte((byte)cx.Results);   //01-27-2018 _fileFormat_4
                if ((S & S8) != 0) w.WriteByte((byte)cx.Sorting);
                if ((S & S9) != 0) w.WriteByte((byte)cx.TakeSet);
                if ((S & S10) != 0) w.WriteByte(cx.TakeLimit);
            }
            w.WriteByte((byte)Mark.ComputeXEnding); // itegrity marker
        }
        #endregion

        #region WriteRelationX  ===============================================
        private void WriteRelationX(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.RelationXBegin); // type index
            w.WriteInt32(chef.T_RelationXStore.Count);

            foreach (var rx in chef.T_RelationXStore.Items)
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
                if ((keyCount + valCount) > 0) b |= B7;

                w.WriteByte(b);
                if ((b & B1) != 0) w.WriteUInt16(rx.GetFlags());
                if ((b & B2) != 0) WriteString(w, rx.Name);
                if ((b & B3) != 0) WriteString(w, rx.Summary);
                if ((b & B4) != 0) WriteString(w, rx.Description);
                if ((b & B5) != 0) w.WriteByte((byte)rx.Pairing);
                if ((b & B7) != 0) w.WriteInt32(keyCount);
                if ((b & B7) != 0) w.WriteInt32(valCount);
            }
            w.WriteByte((byte)Mark.RelationXEnding); // itegrity marker
        }
        #endregion

        #region WriteGraphParm  ===============================================
        private void WriteGraphParm(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            #region RemoveInvalidItems  =======================================
            // hit list of items that no longer exists
            var gxList = new List<GraphX>();
            var gxrtList = new List<Tuple<GraphX, Item>>();
            var gxrtsgList = new List<Tuple<GraphX, Item, Item>>();
            var gxrtsgpmList = new List<Tuple<GraphX, Item, Item, Item>>();
            var graphParms = chef.T_GraphParms;

            // find items that are referenced in the graph parms, but no longer exist
            foreach (var e1 in chef.T_GraphParms)//GD
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
                                    if (sg == chef.T_Dummy)
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
                graphParms.Remove(gd);
            }
            foreach (var gdrt in gxrtList)
            {
                var gd = gdrt.Item1;
                var rt = gdrt.Item2;
                graphParms[gd].Remove(rt);
                if (graphParms[gd].Count == 0)
                    graphParms.Remove(gd);
            }
            foreach (var gdrtsg in gxrtsgList)
            {
                var gd = gdrtsg.Item1;
                var rt = gdrtsg.Item2;
                var sg = gdrtsg.Item3;
                graphParms[gd][rt].Remove(sg);
                if (graphParms[gd][rt].Count == 0)
                    graphParms[gd].Remove(rt);
                if (graphParms[gd].Count == 0)
                    graphParms.Remove(gd);
            }
            foreach (var gdrtsgpm in gxrtsgpmList)
            {
                var gd = gdrtsgpm.Item1;
                var rt = gdrtsgpm.Item2;
                var sg = gdrtsgpm.Item3;
                var pm = gdrtsgpm.Item4;
                graphParms[gd][rt][sg].Remove(pm);
                if (graphParms[gd][rt][sg].Count == 0)
                    graphParms[gd][rt].Remove(sg);
                if (graphParms[gd][rt].Count == 0)
                    graphParms[gd].Remove(rt);
                if (graphParms[gd].Count == 0)
                    graphParms.Remove(gd);
            }
            #endregion

            // now write the remaining valid graph params to the storage file
            foreach (var e1 in graphParms)//GD
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
                        var (x0, y0) = GetCenter(chef, e2.Value); // used to center the drawing arround point(0,0)

                        foreach (var e3 in e2.Value)//SG
                        {
                            w.WriteInt32(itemIndex[e3.Key]);
                            w.WriteInt32(e3.Value.Count);

                            if (e3.Value.Count > 0)
                            {
                                #region WriteQuerys  ==========================
                                if (e3.Key == chef.T_Dummy)
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
        private (int X0, int Y0) GetCenter(Chef chef, Dictionary<Item, List<Item>> sgParams)
        {
            int x1, y1, x2, y2;
            x1 = y1 = int.MaxValue;
            x2 = y2 = int.MinValue;
            foreach (var e3 in sgParams)//SG
            {
                if (e3.Key == chef.T_Dummy)
                {
                    foreach (var gp in e3.Value)//GP
                    {
                        var nd = gp as Node;
                        var (x, y) = nd.Core.GetCenter();
                        {
                            if (x < x1) x1 = x;
                            if (y < y1) y1 = y;
                            if (x > x2) x2 = x;
                            if (y > y2) y2 = y;
                        }
                    }
                }
            }
            return (x1 == int.MaxValue) ? (0, 0) : ((x1 + x2) / 2, (y1 + y2) / 2);
        }
        #endregion
        #endregion

        #region WriteRelationLink  ============================================
        private void WriteRelationLink(Chef chef, DataWriter w, List<Relation> relationList, Dictionary<Item, int> itemIndex)
        {
            foreach (var rx in relationList)
            {
                var count = rx.GetLinksCount();
                if (count == 0) continue;

                ushort len = 0;
                Item itm = chef;
                rx.GetLinks(out List<Item> parents, out List<Item> children);

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

        #region Write String/Bytes  ===========================================
        static void WriteString(DataWriter w, string str)
        {
            var txt = str ?? string.Empty;
            if (txt.Length == 0) txt = "^";


            var len = w.MeasureString(txt);
            if (len > UInt16.MaxValue)
            {
                var r = (double)len / (double)UInt16.MaxValue;
                var n = (UInt16)((txt.Length / r) - 2);
                var trucated = txt.Substring(0, n);
                w.WriteUInt16((UInt16)w.MeasureString(trucated));
                w.WriteString(trucated);
            }
            else
            {
                w.WriteUInt16((UInt16)len);
                w.WriteString(txt);
            }
        }
        static void WriteBytes(DataWriter w, byte[] data)
        {
            var len = data.Length;
            w.WriteInt32(len);
            foreach (var b in data)
            {
                w.WriteByte(b);
            }
        }

        #endregion
    }
}
