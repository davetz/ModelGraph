using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
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
        public byte Version;

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
        public bool NoData { get { return (Data == null || Data.Length < 2); } }
        public float Width { get { return NoData ? 1f : Data[0] / 255f; } } //overall width factor 0..1f
        public float Height { get { return NoData ? 1f : Data[1] / 255f; } } //overall height factor 0..1f 

        #region GetFlipTargetConnect  =========================================
        internal void GetFlipTargetConnect(FlipState flip, List<(Target trg, Contact con, (float dx, float dy) pnt, float siz)> list)
        {
            list.Clear();
            foreach (var e in Target_Contacts)
            {
                list.Add((e.Key, e.Value.contact, _flipper[(int)flip](e.Value.point.dx, e.Value.point.dy), (float)e.Value.size / 255f));
            }
        }
        private static (float, float) ToNone(sbyte dx, sbyte dy) => ((float)dx / 127, (float)dy / 127);
        private static (float, float) ToVertFlip(sbyte dx, sbyte dy) => ((float)dx / 127, (float)dy / -127);
        private static (float, float) ToHorzFlip(sbyte dx, sbyte dy) => ((float)dx / -127, (float)dy / 127);
        private static (float, float) ToVertHorzFlip(sbyte dx, sbyte dy) => ((float)dx / -127, (float)dy / -127);
        private static (float, float) ToLeftRotate(sbyte dx, sbyte dy) => ((float)dy / -127, (float)dx / 127);
        private static (float, float) ToLeftHorzFlip(sbyte dx, sbyte dy) => ((float)dy / 127, (float)dx / 127);
        private static (float, float) ToRightRotate(sbyte dx, sbyte dy) => ((float)dy / 127, (float)dx / 127);
        private static (float, float) ToRightHorzFlip(sbyte dx, sbyte dy) => ((float)dy / -127, (float)dx / 127);

        private static Func<sbyte, sbyte, (float, float)>[] _flipper = { ToNone, ToVertFlip, ToHorzFlip, ToVertHorzFlip, ToLeftRotate, ToLeftHorzFlip, ToRightRotate, ToRightHorzFlip };
        #endregion

        #region GetFlipTargetPenalty  =========================================
        internal byte[][] GetFlipTargetPenalty(FlipState flip) => _flipTargetPenalty[(int)flip];

        //=== non-flipped normal penalty of each dx/dy directional sector to the symbol's target contact point
        //========== dx/dy directional sector  0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F ================
        private static readonly byte[] _EN = { 1, 2, 3, 4, 5, 5, 5, 5, 5, 5, 5, 5, 1, 0, 0, 0 }; //EN
        private static readonly byte[] _E  = { 0, 0, 0, 3, 5, 5, 5, 5, 5, 5, 5, 5, 3, 0, 0, 0 }; //E
        private static readonly byte[] _ES = { 0, 0, 0, 1, 5, 5, 5, 5, 5, 5, 5, 5, 4, 3, 2, 1 }; //ES

        private static readonly byte[] _SEC = { 0, 0, 0, 0, 0, 0, 1, 2, 5, 5, 5, 5, 2, 1, 0, 0 }; //SEC
        private static readonly byte[] _SE  = { 1, 0, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5, 5, 5, 5, 5 }; //SE
        private static readonly byte[] _S   = { 3, 0, 0, 0, 0, 0, 0, 3, 5, 5, 5, 5, 5, 5, 5, 5 }; //S
        private static readonly byte[] _SW  = { 4, 3, 2, 1, 0, 0, 0, 1, 5, 5, 5, 5, 5, 5, 5, 5 }; //SW
        private static readonly byte[] _SWC = { 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 5, 5, 5, 5 }; //SWC

        private static readonly byte[] _WS  = { 5, 5, 5, 5, 1, 0, 0, 0, 1, 2, 3, 4, 5, 5, 5, 0 }; //WS
        private static readonly byte[] _W   = { 5, 5, 5, 3, 0, 0, 0, 0, 0, 0, 0, 3, 5, 5, 5, 5 }; //W
        private static readonly byte[] _WN  = { 4, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 5, 5, 5, 5 }; //WN

        private static readonly byte[] _NWC = { 5, 5, 5, 5, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2 }; //NWC
        private static readonly byte[] _NW  = { 5, 5, 5, 5, 5, 5, 5, 5, 1, 0, 0, 0, 2, 3, 3, 4 }; //NW
        private static readonly byte[] _N   = { 5, 5, 5, 5, 5, 5, 5, 5, 3, 0, 0, 0, 0, 0, 0, 3 }; //N
        private static readonly byte[] _NE  = { 5, 5, 5, 5, 5, 5, 5, 5, 4, 3, 2, 1, 0, 0, 0, 1 }; //NE 
        private static readonly byte[] _NEC = { 0, 0, 1, 2, 5, 5, 5, 5, 2, 1, 0, 0, 0, 0, 0, 0 }; //NEC

        //==============================  EN   E   ES     SEC   SE   S   SW   SWC     WS   W   WN     NWC   NW   N   NE   NEC
        private static byte[][] _pNo = { _EN, _E, _ES,   _SEC, _SE, _S, _SW, _SWC,   _WS, _W, _WN,   _NWC, _NW, _N, _NE, _NEC };//
        private static byte[][] _pVF = { _ES, _E, _EN,   _NEC, _NE, _N, _NW, _NWC,   _WN, _W, _WS,   _SWC, _SW, _S, _SE, _SEC };//
        private static byte[][] _pHF = { _WN, _W, _WS,   _SWC, _SW, _S, _SE, _SEC,   _ES, _E, _EN,   _NEC, _NE, _N, _NW, _NWC };//
        private static byte[][] _pVH = { _WS, _W, _WN,   _NWC, _NW, _N, _NE, _NEC,   _EN, _E, _ES,   _SEC, _SE, _S, _SW, _SWC };//
        private static byte[][] _pLR = { _SE, _S, _SW,   _SWC, _WS, _S, _WN, _NWC,   _NW, _N, _NE,   _NEC, _EN, _E, _ES, _SEC };//
        private static byte[][] _pLH = { _NE, _N, _NW,   _NWC, _WN, _W, _WS, _SWC,   _SW, _S, _SE,   _SEC, _ES, _E, _EN, _NEC };//
        private static byte[][] _pRR = { _NW, _N, _NE,   _NEC, _EN, _E, _EN, _NEC,   _NE, _N, _NW,   _NWC, _WN, _W, _WS, _NWC };//
        private static byte[][] _pRH = { _SW, _S, _SE,   _SEC, _ES, _E, _EN, _NEC,   _NE, _N, _NW,   _NWC, _WN, _W, _WS, _SWC };//

        private static byte[][][] _flipTargetPenalty = { _pNo, _pVF, _pHF, _pVH, _pLR, _pLH, _pRR, _pRH };
        #endregion

        #endregion
    }
}
