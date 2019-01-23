using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public class SymbolX : Item
    {
        internal Guid Guid;
        public string Name;
        public string Summary;
        public string Description;
        public byte[] Data;
        public Attach Attach;
        public Graphic Graphic;
        public Dictionary<Target, (Contact contact, (sbyte dx, sbyte dy) point, byte size)> Target_Contacts = new Dictionary<Target, (Contact contact, (sbyte dx, sbyte dy) point, byte size)>(4);
        public Target AllTargets = Target.None;
        public FlipState FlipState;

        public Contact NorthContact;
        public Contact WestContact;
        public Contact EastContact;
        public Contact SouthContact;
        public Contact SouthEastContact;
        public Contact SouthWestContact;
        public Contact NorthEastContact;
        public Contact NorthWestContact;


        #region Constructors  =================================================
        public SymbolX(Store owner)
        {
            Guid = Guid.NewGuid();
            Trait = Trait.SymbolX;
            Owner = owner;
            AutoExpandRight = true;

            owner.Add(this);
        }
        public SymbolX(Store owner, Guid guid)
        {
            Guid = guid;
            Trait = Trait.SymbolX;
            Owner = owner;

            owner.Add(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        private static readonly int minData = 15; //{ w, h, A, R, G, B, SC, EC, DC, DS, PC, x1, y1, x2, y2,..}
        private static readonly byte minSize = 10; // nominal size when there is no symbol data

        public bool NoData { get { return (Data == null || Data.Length < minData); } }
        public byte Width { get { return NoData ? minSize : Data[0]; } }
        public byte Height { get { return NoData ? minSize : Data[1]; } }
        #endregion
    }
}
