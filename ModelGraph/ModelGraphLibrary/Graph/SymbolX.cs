using System;
using System.Collections.Generic;

namespace ModelGraphLibrary
{/*
 */
    public class SymbolX : Item
    {
        internal Guid Guid;
        internal string Name;
        internal string Summary;
        internal string Description;
        internal byte[] Data;
        internal Contact TopContact;
        internal Contact LeftContact;
        internal Contact RightContact;
        internal Contact BottomContact;

        #region Constructors  =================================================
        internal SymbolX(Store owner)
        {
            Guid = Guid.NewGuid();
            Trait = Trait.SymbolX;
            Owner = owner;
            AutoExpandRight = true;

            owner.Add(this);
        }
        internal SymbolX(Store owner, Guid guid)
        {
            Guid = guid;
            Trait = Trait.SymbolX;
            Owner = owner;

            owner.Add(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        private static int minData = 15; //{ w, h, A, R, G, B, SC, EC, DC, DS, PC, x1, y1, x2, y2,..}
        private static byte minSize = 10; // nominal size when there is no symbol data

        internal bool NoData { get { return (Data == null || Data.Length < minData); } }
        internal byte Width { get { return NoData ? minSize : Data[0]; } }
        internal byte Height { get { return NoData ? minSize : Data[1]; } }

        internal byte[] GetOptions()
        {
            var options = new byte[4];
            options[0] = (byte)TopContact;
            options[1] = (byte)LeftContact;
            options[2] = (byte)RightContact;
            options[3] = (byte)BottomContact;
            return options;
        }
        #endregion
    }
}
