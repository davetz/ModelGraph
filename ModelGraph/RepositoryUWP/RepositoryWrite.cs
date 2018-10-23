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
            var fileFormat = _fileFormat_E;
            var itemCount = chef.GetGuidItemIndex(out Guid[] guids, out Dictionary<Item, int> itemIndex);
            var relationList = chef.GetRelationList();

            w.WriteInt32(0);
            w.WriteGuid(fileFormat);

            WriteGuids(w, guids);

            if (chef.ViewXStore.Count > 0) WriteViewX(chef, w, itemIndex);
            if (chef.EnumXStore.Count > 0) WriteEnumX(chef, w, itemIndex);
            if (chef.TableXStore.Count > 0) WriteTableX(chef, w, itemIndex);
            if (chef.GraphXStore.Count > 0) WriteGraphX(chef, w, itemIndex);
            if (chef.QueryXStore.Count > 0) WriteQueryX(chef, w, itemIndex);
            if (chef.SymbolStore.Count > 0) WriteSymbolX(chef, w, itemIndex);
            if (chef.ColumnXStore.Count > 0) WriteColumnX(chef, w, itemIndex);
            if (chef.ComputeXStore.Count > 0) WriteComputeX(chef, w, itemIndex);
            if (chef.RelationXStore.Count > 0) WriteRelationX(chef, w, itemIndex);

            if (chef.GraphParms.Count > 0) WriteGraphParm(chef, w, itemIndex);
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
            w.WriteInt32(chef.ViewXStore.Count);

            foreach (var view in chef.ViewXStore.Items)
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
            w.WriteInt32(chef.EnumXStore.Count);

            foreach (var ex in chef.EnumXStore.Items)
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
            w.WriteInt32(chef.TableXStore.Count);

            foreach (var tx in chef.TableXStore.Items)
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
            w.WriteInt32(chef.GraphXStore.Count);

            foreach (var gx in chef.GraphXStore.Items)
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

                w.WriteByte(gx.MinNodeSize);
                w.WriteByte(gx.ThinBusSize);
                w.WriteByte(gx.WideBusSize);
                w.WriteByte(gx.ExtraBusSize);

                w.WriteByte(gx.SurfaceSkew);
                w.WriteByte(gx.TerminalSkew);
                w.WriteByte(gx.TerminalLength);
                w.WriteByte(gx.TerminalSpacing);
            }
            w.WriteByte((byte)Mark.GraphXEnding); // itegrity marker
        }
        #endregion

        #region WriteQueryX  ==================================================
        private void WriteQueryX(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.QueryXBegin); // type vector
            w.WriteInt32(chef.QueryXStore.Count);

            foreach (var qx in chef.QueryXStore.Items)
            {
                w.WriteInt32(itemIndex[qx]);

                var b = SZ;
                if (qx.HasState()) b |= S1;
                if (!string.IsNullOrWhiteSpace(qx.WhereString)) b |= S2;
                if (!string.IsNullOrWhiteSpace(qx.SelectString)) b |= S3;
                if (qx.IsExclusive) b |= S4;
                if (qx.QueryKind == QueryType.Path && qx.IsHead == true && qx.PathParm != null)
                {
                    if (qx.PathParm.Facet1 != Facet.None) b |= S5;
                    if (qx.PathParm.Connect1 != Connect.Any) b |= S6;

                    if (qx.PathParm.Facet2 != Facet.None) b |= S7;
                    if (qx.PathParm.Connect2 != Connect.Any) b |= S8;

                    if (qx.PathParm.DashStyle != DashStyle.Solid) b |= S9;
                    if (qx.PathParm.LineStyle != LineStyle.PointToPoint) b |= S10;
                    if (!string.IsNullOrWhiteSpace(qx.PathParm.LineColor)) b |= S11;
                }

                w.WriteUInt16(b);
                if ((b & S1) != 0) w.WriteUInt16(qx.GetState());
                if ((b & S2) != 0) WriteString(w, qx.WhereString);
                if ((b & S3) != 0) WriteString(w, qx.SelectString);
                if ((b & S4) != 0) w.WriteByte(qx.ExclusiveKey);

                if ((b & S5) != 0) w.WriteByte((byte)qx.PathParm.Facet1);
                if ((b & S6) != 0) w.WriteByte((byte)qx.PathParm.Connect1);

                if ((b & S7) != 0) w.WriteByte((byte)qx.PathParm.Facet2);
                if ((b & S8) != 0) w.WriteByte((byte)qx.PathParm.Connect2);

                if ((b & S9) != 0) w.WriteByte((byte)qx.PathParm.DashStyle);
                if ((b & S10) != 0) w.WriteByte((byte)qx.PathParm.LineStyle);
                if ((b & S11) != 0) WriteString(w, qx.PathParm.LineColor);
            }
            w.WriteByte((byte)Mark.QueryXEnding); // itegrity marker
        }
        #endregion

        #region WriteSymbolX  =================================================
        private void WriteSymbolX(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.SymbolXBegin); // type index
            w.WriteInt32(chef.SymbolStore.Count);

            foreach (var sx in chef.SymbolStore.Items)
            {
                w.WriteInt32(itemIndex[sx]);

                var b = SZ;
                if (sx.HasState()) b |= S1;
                if (!string.IsNullOrWhiteSpace(sx.Name)) b |= S2;
                if (!string.IsNullOrWhiteSpace(sx.Summary)) b |= S3;
                if (!string.IsNullOrWhiteSpace(sx.Description)) b |= S4;
                if (sx.Data != null && sx.Data.Length > 12) b |= S5;
                if (sx.Attach != Attach.Normal) b |= S6;
                if (sx.TopContact != Contact.Any) b |= S7;
                if (sx.LeftContact != Contact.Any) b |= S8;
                if (sx.RightContact != Contact.Any) b |= S9;
                if (sx.BottomContact != Contact.Any) b |= S10;

                w.WriteUInt16(b);
                if ((b & S1) != 0) w.WriteUInt16(sx.GetState());
                if ((b & S2) != 0) WriteString(w, sx.Name);
                if ((b & S3) != 0) WriteString(w, sx.Summary);
                if ((b & S4) != 0) WriteString(w, sx.Description);
                if ((b & S5) != 0) WriteBytes(w, sx.Data);
                if ((b & S6) != 0) w.WriteByte((byte)sx.Attach);
                if ((b & S7) != 0) w.WriteByte((byte)sx.TopContact);
                if ((b & S8) != 0) w.WriteByte((byte)sx.LeftContact);
                if ((b & S9) != 0) w.WriteByte((byte)sx.RightContact);
                if ((b & S10) != 0) w.WriteByte((byte)sx.BottomContact);
            }
            w.WriteByte((byte)Mark.SymbolXEnding); // itegrity marker
        }
        #endregion

        #region WriteColumnX  =================================================
        private void WriteColumnX(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteByte((byte)Mark.ColumnXBegin); // type index
            w.WriteInt32(chef.ColumnXStore.Count);

            foreach (var cx in chef.ColumnXStore.Items)
            {
                w.WriteInt32(itemIndex[cx]);

                var b = BZ;
                if (cx.HasState()) b |= B1;
                if (!string.IsNullOrWhiteSpace(cx.Name)) b |= B2;
                if (!string.IsNullOrWhiteSpace(cx.Summary)) b |= B3;
                if (!string.IsNullOrWhiteSpace(cx.Description)) b |= B4;

                w.WriteByte(b);
                if ((b & B1) != 0) w.WriteUInt16(cx.GetState());
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
            w.WriteInt32(chef.ComputeXStore.Count);

            foreach (var cx in chef.ComputeXStore.Items)
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
            w.WriteInt32(chef.RelationXStore.Count);

            foreach (var rx in chef.RelationXStore.Items)
            {
                w.WriteInt32(itemIndex[rx]);

                var keyCount = rx.KeyCount;
                var valCount = rx.ValueCount;

                var b = BZ;
                if (rx.HasState()) b |= B1;
                if (!string.IsNullOrWhiteSpace(rx.Name)) b |= B2;
                if (!string.IsNullOrWhiteSpace(rx.Summary)) b |= B3;
                if (!string.IsNullOrWhiteSpace(rx.Description)) b |= B4;
                if (rx.Pairing != Pairing.OneToMany) b |= B5;
                if ((keyCount + valCount) > 0) b |= B7;

                w.WriteByte(b);
                if ((b & B1) != 0) w.WriteUInt16(rx.GetState());
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
            var gxrtList = new List<(GraphX gx, Item ri)>();
            var gxrtsgList = new List<(GraphX gx, Item ri, QueryX qx)>();
            var gxrtsgpmList = new List<(GraphX gx, Item ri, QueryX qx, NodeEdge ge)>();
            var graphParms = chef.GraphParms;

            // find items that are referenced in the graph parms, but no longer exist
            foreach (var e1 in chef.GraphParms)//GD
            {
                var gx = e1.Key;
                if (itemIndex.ContainsKey(gx))
                {
                    foreach (var e2 in e1.Value)
                    {
                        var ri = e2.Key;
                        if (itemIndex.ContainsKey(ri))
                        {
                            foreach (var e3 in e2.Value)
                            {
                                var qx = e3.Key;
                                if (itemIndex.ContainsKey(qx))
                                {
                                    if (qx == chef.QueryXNode)
                                    {
                                        foreach (var ge in e3.Value)
                                        {
                                            var nd = ge as Node;
                                            if (itemIndex.ContainsKey(nd.Item)) continue;
                                            gxrtsgpmList.Add((gx, ri, qx, ge));
                                        }
                                    }
                                    else
                                    {
                                        foreach (var ge in e3.Value)//GP
                                        {
                                            var eg = ge as Edge;
                                            if (itemIndex.ContainsKey(eg.Node1.Item) && itemIndex.ContainsKey(eg.Node2.Item)) continue;
                                            gxrtsgpmList.Add((gx, ri, qx, ge));
                                        }
                                    }
                                }
                                else
                                {
                                    gxrtsgList.Add((gx, ri, qx));
                                }
                            }
                        }
                        else
                        {
                            gxrtList.Add((gx, ri));
                        }
                    }
                }
                else
                {
                    gxList.Add(gx);
                }
            }

            // remove params for items which no longer exists
            foreach (var gx in gxList)
            {
                graphParms.Remove(gx);
            }
            foreach (var (gx, ri) in gxrtList)
            {
                graphParms[gx].Remove(ri);
                if (graphParms[gx].Count == 0)
                    graphParms.Remove(gx);
            }
            foreach (var (gx, ri, qx) in gxrtsgList)
            {
                graphParms[gx][ri].Remove(qx);
                if (graphParms[gx][ri].Count == 0)
                    graphParms[gx].Remove(ri);
                if (graphParms[gx].Count == 0)
                    graphParms.Remove(gx);
            }
            foreach (var (gx, ri, qx, ge) in gxrtsgpmList)
            {
                graphParms[gx][ri][qx].Remove(ge);
                if (graphParms[gx][ri][qx].Count == 0)
                    graphParms[gx][ri].Remove(qx);
                if (graphParms[gx][ri].Count == 0)
                    graphParms[gx].Remove(ri);
                if (graphParms[gx].Count == 0)
                    graphParms.Remove(gx);
            }
            #endregion

            // now write the remaining valid graph params to the storage file
            foreach (var e1 in graphParms)//GraphX
            {
                if (e1.Value.Count == 0) continue;

                w.WriteByte((byte)Mark.GraphParmBegin); // type index

                w.WriteInt32(itemIndex[e1.Key]);
                w.WriteInt32(e1.Value.Count);

                foreach (var e2 in e1.Value)//RootItem
                {
                    w.WriteInt32(itemIndex[e2.Key]);
                    w.WriteInt32(e2.Value.Count);

                    if (e2.Value.Count > 0)
                    {
                        #region WriteRoots  ===================================
                        var (x0, y0) = GetCenter(chef, e2.Value); // used to center the drawing arround point(0,0)

                        foreach (var e3 in e2.Value)//QueryX
                        {
                            w.WriteInt32(itemIndex[e3.Key]);
                            w.WriteInt32(e3.Value.Count);

                            if (e3.Value.Count > 0)
                            {
                                #region WriteQuerys  ==========================
                                if (e3.Key == chef.QueryXNode)
                                {
                                    #region WriteNodes  =======================
                                    foreach (var en in e3.Value)//NodeEdge
                                    {
                                        var nd = en as Node;
                                        w.WriteInt32(itemIndex[nd.Item]);

                                        w.WriteInt32(nd.X - x0);
                                        w.WriteInt32(nd.Y - y0);
                                        w.WriteByte(nd.DX);
                                        w.WriteByte(nd.DY);
                                        w.WriteByte((byte)nd.Aspect);
                                        w.WriteByte((byte)nd.FlipRotate);
                                        w.WriteByte((byte)nd.Labeling);
                                        w.WriteByte((byte)nd.Sizing);
                                        w.WriteByte((byte)nd.BarWidth);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region WriteEdges  =======================
                                    foreach (var ne in e3.Value)//NodeEdge
                                    {
                                        var eg = ne as Edge;
                                        w.WriteInt32(itemIndex[eg.Node1.Item]);
                                        w.WriteInt32(itemIndex[eg.Node2.Item]);

                                        var b = BZ;
                                        if (eg.Facet1 != Facet.None) b |= B1;
                                        if (eg.Facet2 != Facet.None) b |= B2;
                                        if (eg.HasBends) b |= B3;

                                        w.WriteByte(b);

                                        if ((b & B1) != 0) w.WriteByte((byte)eg.Facet1);
                                        if ((b & B2) != 0) w.WriteByte((byte)eg.Facet2);
                                        if ((b & B3) != 0)
                                        {
                                            var len = eg.Bends.Length;
                                            w.WriteUInt16((ushort)len);
                                            for (int i = 0; i < len; i++)
                                            {
                                                w.WriteInt32(eg.Bends[i].X - x0);
                                                w.WriteInt32(eg.Bends[i].Y - y0);
                                            }
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
                w.WriteByte((byte)Mark.GraphParmEnding);
            }
        }
        #region TryGetOffset  =================================================
        private (int X0, int Y0) GetCenter(Chef chef, Dictionary<QueryX, List<NodeEdge>> qxParams)
        {
            int x1, y1, x2, y2;
            x1 = y1 = int.MaxValue;
            x2 = y2 = int.MinValue;
            foreach (var e3 in qxParams)
            {
                if (e3.Key == chef.QueryXNode)
                {
                    foreach (var gp in e3.Value)//GP
                    {
                        var nd = gp as Node;
                        var (x, y) = nd.GetCenter();
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
