using System;
using ModelGraphSTD;
using Windows.Storage.Streams;
using System.Collections.Generic;

namespace RepositoryUWP
{
    public partial class RepositoryStorageFile : IRepository
    {
        Guid _formatGuid = new Guid("3DB85BFF-F448-465C-996D-367E6284E913");
        Guid _serilizerGuid = new Guid("DE976A9D-0C50-4B4E-9B46-74404A64A703");

        #region Write  ========================================================
        private void Write2(Chef chef, DataWriter w)
        {
            chef.GetGuidItemIndex(out Guid[] guids, out Dictionary<Item, int> itemIndex);
            var relationList = chef.GetRelationList();

            w.WriteGuid(_formatGuid);
            w.WriteInt32(itemIndex.Count);

            WriteReferenceItems(chef, w, itemIndex);

            if (chef.TableXStore.Count > 0) WriteTableX2(chef, w, itemIndex);
            if (chef.ColumnXStore.Count > 0) WriteColumnX2(chef, w, itemIndex);
            if (chef.RelationXStore.Count > 0) WriteRelationX2(chef, w, itemIndex);

            var relLink = new RelLink(relationList);
            relLink.WriteData2(w, itemIndex);

            w.WriteGuid(_formatGuid);
        }
        #endregion

        #region WriteReferenceItems  ==========================================
        public void WriteReferenceItems(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {

            var referenceItems = new List<Item>();
            var index = 0;
            var items = new List<Item>(itemIndex.Keys);
            foreach (var item in items)
            {
                if (item.IsExternal) continue;

                referenceItems.Add(item);
                itemIndex[item] = index++;
            }
            
            w.WriteGuid(new Guid("DE976A9D-0C50-4B4E-9B46-74404A64A703"));
            w.WriteUInt16((ushort)index);
            w.WriteByte(1);

            foreach (var item in referenceItems)
            {
                w.WriteUInt16((ushort)((int)item.Trait & 0xFFF)); //referenced internal item
            }

            foreach (var item in items)
            {
                if (item.IsExternal) itemIndex[item] = index++;
            }
        }
        #endregion

        #region WriteTableX  ==================================================
        private void WriteTableX2(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteGuid(new Guid("93EC136C-6C38-474D-844B-6B8326526CB5"));
            w.WriteInt32(chef.TableXStore.Count);
            w.WriteByte(1);

            foreach (var tx in chef.TableXStore.Items)
            {
                w.WriteInt32(itemIndex[tx]);

                var b = BZ;
                if (tx.HasState()) b |= B1;
                if (!string.IsNullOrWhiteSpace(tx.Name)) b |= B2;
                if (!string.IsNullOrWhiteSpace(tx.Summary)) b |= B3;
                if (!string.IsNullOrWhiteSpace(tx.Description)) b |= B4;

                w.WriteByte(b);
                if ((b & B1) != 0) w.WriteUInt16(tx.GetState());
                if ((b & B2) != 0) WriteString(w, tx.Name);
                if ((b & B3) != 0) WriteString(w, tx.Summary);
                if ((b & B4) != 0) WriteString(w, tx.Description);

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
        }
        #endregion

        #region WriteColumnX  =================================================
        private void WriteColumnX2(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteGuid(new Guid("3E7097FE-22D5-43B2-964A-9DB843F6D55B"));
            w.WriteInt32(chef.ColumnXStore.Count);
            w.WriteByte(1);

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
                if ((b & B4) != 0) WriteString(w, cx.Description);

                w.WriteByte((byte)cx.Value.ValType);
                WriteValueDictionary(w, cx, itemIndex);
            }
        }
        #endregion

        #region WriteRelationX  ===============================================
        private void WriteRelationX2(Chef chef, DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteGuid(new Guid("D950F508-B774-4838-B81A-757EFDC40518"));
            w.WriteInt32(chef.RelationXStore.Count);
            w.WriteByte(1);

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
                if ((keyCount + valCount) > 0) b |= B6;

                w.WriteByte(b);
                if ((b & B1) != 0) w.WriteUInt16(rx.GetState());
                if ((b & B2) != 0) WriteString(w, rx.Name);
                if ((b & B3) != 0) WriteString(w, rx.Summary);
                if ((b & B4) != 0) WriteString(w, rx.Description);
                if ((b & B5) != 0) w.WriteByte((byte)rx.Pairing);
                if ((b & B6) != 0) w.WriteInt32(keyCount);
                if ((b & B6) != 0) w.WriteInt32(valCount);
            }
        }
        #endregion
    }
    #region WriteRelationLinks  ===============================================
    internal interface IRelationStore
    {
        Relation[] GetRelationArray();
    }

    class RelLink : LinkSerializer, IRelationStore
    {
        static Guid _serializerGuid => new Guid("6E4E6626-98BC-483E-AC9B-C7799511ECF2");
        Relation[] _relations;

        internal RelLink(List<Relation> relations)
        {
            _relations = relations.ToArray();
            _relationStore = this;
        }

        internal void WriteData2(DataWriter w, Dictionary<Item, int> itemIndex)
        {
            w.WriteGuid(_serializerGuid);
            WriteData(w, itemIndex);
        }

        public Relation[] GetRelationArray()
        {
            return _relations;
        }
    }

    #region LinkSerializer  ===================================================
    internal abstract class LinkSerializer
    {
        static byte _formatVersion = 1;
        protected IRelationStore _relationStore;

        #region ListSizeCode  =================================================
        // Endode the size range info { (n < 256), (n < 65536), (n > 65535) }
        // for the four lists that occur in a Relation object.

        const byte BZ = 0x00;
        //-------------------------List1
        const byte B01 = 0x01;  // n < 256   -- the list size is stored in a byte
        const byte B02 = 0x02;  // n < 65536 -- the list size is stored in a ushort
        const byte B03 = 0x03;  // n > 65535 -- the list size is stored in an int
        //-------------------------List2
        const byte B04 = 0x04;
        const byte B08 = 0x08;
        const byte B0C = 0x0C;
        //-------------------------List3
        const byte B10 = 0x10;
        const byte B20 = 0x20;
        const byte B30 = 0x30;
        //-------------------------List4
        const byte B40 = 0x40;
        const byte B80 = 0x80;
        const byte BC0 = 0xC0;

        byte GetCompositeCode((int, int)[] list) //=== OneToOne Relation
        {
            var len = list.Length;
            if (len < 256) return B10;
            if (len < 65356) return B20;
            return B30;
        }
        byte GetCompositeCode((int, int[])[] children) //=== OneToMany Relation
        {
            var code = BZ;
            var len1 = 0;
            var len3 = children.Length;
            foreach (var (ix1, ix2List) in children)
            {
                var len = ix2List.Length;
                if (len > len1) len1 = len;
            }
            if (len1 < 256) code |= B01;
            else if (len1 < 65356) code |= B02;
            else code |= B03;

            if (len3 < 256) code |= B10;
            else if (len3 < 65536) code |= B20;
            else code |= B30;

            return code;
        }
        byte GetCompositeCode((int, int[])[] parents, (int, int[])[] children) //=== ManyToMany Relation
        {
            var code = BZ;
            var len1 = 0;
            var len2 = 0;
            var len3 = children.Length;
            var len4 = parents.Length;
            foreach (var (ix1, ix2List) in children)
            {
                var len = ix2List.Length;
                if (len > len1) len1 = len;
            }
            if (len1 < 256) code |= B01;
            else if (len1 < 65356) code |= B02;
            else code |= B03;

            foreach (var (ix1, ix2List) in parents)
            {
                var len = ix2List.Length;
                if (len > len2) len2 = len;
            }
            if (len2 < 256) code |= B04;
            else if (len2 < 65356) code |= B08;
            else code |= B0C;

            if (len3 < 256) code |= B10;
            else if (len3 < 65536) code |= B20;
            else code |= B30;

            if (len4 < 256) code |= B40;
            else if (len4 < 65536) code |= B80;
            else code |= BC0;

            return code;
        }

        //=====================================================================
        // Extract the list size information from the composite sizing code
        // and express it in a regular format for each of relation pairing type         
        byte SizeCodeOneToOne(byte code)
        {
            if (code == B10) return B10;// n < 256   -- the list size is stored in a byte
            if (code == B20) return B20;// n < 65536 -- the list size is stored in a ushort
            if (code == B30) return B40;// n > 65535 -- the list size is stored in an int
            throw new Exception($"LinkSerializer ReadData, invalid list size code");
        }
        byte SizeCodeOneToMany(byte code)
        {
            var code1 = BZ;
            if ((code & B03) == B01)
                code1 |= B01;// n < 256   -- the sub list size is stored in a byte
            else if ((code & B03) == B02)
                code1 |= B02;// n < 256   -- the sub list size is stored in a byte
            else if ((code & B03) == B03)
                code1 |= B04;// n < 256   -- the sub list size is stored in a byte
            else
                throw new Exception($"LinkSerializer ReadData, invalid list size code");

            if ((code & B30) == B10)
                code1 |= B10;// n < 256   -- the list size is stored in a byte
            else if ((code & B30) == B20)
                code1 |= B20;// n < 256   -- the list size is stored in a byte
            else if ((code & B30) == B30)
                code1 |= B40;// n < 256   -- the list size is stored in a byte
            else
                throw new Exception($"LinkSerializer ReadData, invalid list size code");

            return code1;
        }
        (byte, byte) SizeCodeManyToMany(byte code)
        {
            var code1 = BZ;
            var code2 = BZ;

            if ((code & B03) == B01)
                code1 |= B01;
            else if ((code & B03) == B02)
                code1 |= B02;
            else if ((code & B03) == B03)
                code1 |= B04;
            else
                throw new Exception($"LinkSerializer ReadData, invalid list size code");

            if ((code & B0C) == B04)
                code2 |= B01;
            else if ((code & B0C) == B08)
                code2 |= B02;
            else if ((code & B0C) == B0C)
                code2 |= B04;
            else
                throw new Exception($"LinkSerializer ReadData, invalid list size code");

            if ((code & B30) == B10)
                code1 |= B10;
            else if ((code & B30) == B20)
                code1 |= B20;
            else if ((code & B30) == B30)
                code1 |= B40;
            else
                throw new Exception($"LinkSerializer ReadData, invalid list size code");

            if ((code & BC0) == B40)
                code2 |= B10;
            else if ((code & BC0) == B80)
                code2 |= B20;
            else if ((code & BC0) == BC0)
                code2 |= B40;
            else
                throw new Exception($"LinkSerializer ReadData, invalid list size code");

            return (code2, code1);
        }
        #endregion

        #region WriteData  ====================================================
        public void WriteData(DataWriter w, Dictionary<Item, int> itemIndex)
        {
            var rlationArray = _relationStore.GetRelationArray();
            var N = 0;
            foreach (var rel in rlationArray) { if (rel.HasLinks) N++; } //count number of serialized relations 

            w.WriteInt32(N);                //number of serialized relations 
            w.WriteByte(_formatVersion);    //format version

            foreach (var rel in rlationArray)  //foreach relation entry
            {
                if (rel.HasLinks)
                {
                    w.WriteInt32(itemIndex[rel]);    //relation index
                    w.WriteByte((byte)rel.Pairing);  //pairing cross check, it should match the relation.pairing on reading

                    switch (rel.Pairing)
                    {
                        case Pairing.OneToOne:

                            var list1 = rel.GetChildren1Items(itemIndex);
                            var len = list1.Length;

                            var code1 = GetCompositeCode(list1);
                            w.WriteByte(code1);//===================write composite sizing code;

                            var listSize1 = SizeCodeOneToOne(code1);

                            if ((listSize1 & B10) != 0)
                            {//============================list length < 256
                                w.WriteByte((byte)len);
                            }
                            else if ((listSize1 & B20) != 0)
                            {//============================list length < 65536
                                w.WriteUInt16((ushort)len);
                            }
                            else
                            {//============================list length > 65535
                                w.WriteInt32(len);
                            }

                            foreach (var (ix1, ix2) in list1)
                            {
                                w.WriteInt32(ix1);      //parent item
                                w.WriteInt32(ix2);      //child item
                            }
                            break;

                        case Pairing.OneToMany:

                            var list2 = rel.GetChildren2Items(itemIndex);

                            var code2 = GetCompositeCode(list2);
                            w.WriteByte(code2);//===================write composite sizing code;

                            var listSize2 = SizeCodeOneToMany(code2);

                            WriteList(w, list2, listSize2);   //write the compound list
                            break;

                        case Pairing.ManyToMany:

                            var list3 = rel.GetChildren2Items(itemIndex);
                            var list4 = rel.GetParents2Items(itemIndex);

                            var code3 = GetCompositeCode(list4, list3);
                            w.WriteByte(code3);//===================write composite sizing code;

                            var (listSize4, listSize3) = SizeCodeManyToMany(code3);

                            WriteList(w, list3, listSize3);   //write the compound list
                            WriteList(w, list4, listSize4);  //write the compound list
                            break;
                    }
                }
            }
        }
        #endregion

        #region WriteList - write the compond list  ===========================
        void WriteList(DataWriter w, (int, int[])[] list, byte listSize)
        {
            var len = list.Length;

            if ((listSize & B10) != 0)
            {//============================list length < 256
                w.WriteByte((byte)len);
            }
            else if ((listSize & B20) != 0)
            {//============================list length < 65536
                w.WriteUInt16((ushort)len);
            }
            else
            {//============================list length > 65535
                w.WriteInt32(len);
            }

            foreach (var ent in list) { WriteOneToMany(w, ent, listSize); }
        }
        #endregion

        #region WriteOneToMany  ===============================================
        void WriteOneToMany(DataWriter w, (int, int[]) pcList, byte listSize)
        {
            var (ix1, ix2List) = pcList;
            w.WriteInt32(ix1);

            var len = ix2List.Length;
            if ((listSize & B01) != 0)
            {//============================list length < 256
                w.WriteByte((byte)len);
            }
            else if ((listSize & B02) != 0)
            {//============================list length < 65536
                w.WriteUInt16((ushort)len);
            }
            else
            {//============================list length > 65535
                w.WriteInt32(len);
            }

            foreach (var ix2 in ix2List) { w.WriteInt32(ix2); }
        }
        #endregion

        #region ReadOneToMany  ================================================
        (int, int[]) ReadOneToMany(DataReader r, byte listSize)
        {
            int len;
            var ix1 = r.ReadInt32();

            if ((listSize & B01) != 0)
            {//============================list length < 256
                len = r.ReadByte();
            }
            else if ((listSize & B02) != 0)
            {//============================list length < 65536
                len = r.ReadUInt16();
            }
            else
            {//============================list length > 65535
                len = r.ReadInt32();
            }
            var ix2List = new int[len];
            for (int i = 0; i < len; i++) { ix2List[i] = r.ReadInt32(); }
            return (ix1, ix2List);
        }
        #endregion
    }
    #endregion

    #endregion

}
