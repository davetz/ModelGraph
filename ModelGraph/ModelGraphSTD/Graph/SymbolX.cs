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

        #region GetFlipTargetSurface  =========================================
        internal List<(float x, float y, float dx, float dy, float siz)> GetFlipTargetSurface(FlipState flip, float cx, float cy, float scale)
        {
            var list = new List<(float x, float y, float dx, float dy, float siz)>(Target_Contacts.Count);
            foreach (var e in Target_Contacts)
            {
                var fix = (int)flip;
                var (tdx, tdy) = ToFloat(e.Value.point);
                var (fdx, fdy) = _flipper[fix](tdx, tdy);
                var x = fdx * scale + cx;
                var y = fdy * scale + cy;
                var sz = e.Value.size * scale / 255 + cy;
                var (tx, ty) = _targetSurface[TIX(e.Key)];
                var (dx, dy) = _flipper[fix](tx, ty);
                list.Add((x, y, dx, dy, sz));
            }
            return list;
        }
        private const float Q = 0.7071067811865f; // 1 / SQRT(2)
        private static readonly (float dx, float dy)[] _targetSurface =
        {
            (0, 1),  //EN
            (0, 1),  //E
            (0, 1),  //ES
            (-Q, Q), //SEC
            (1, 0),  //SE
            (1, 0),  //S
            (1, 0),  //SW
            (Q, Q),  //SWC
            (0, 1),  //WS
            (0, 1),  //W
            (0, 1),  //WN
            (-Q, Q), //NWC
            (1, 0),  //NW
            (1, 0),  //N
            (1, 0),  //NE
            (Q, Q),  //NEC
        };
        #endregion

        #region GetFlipTargetConnect  =========================================
        internal void GetFlipTargetConnect(FlipState flip, float cx, float cy, float scale, List<(Target trg, byte tix, Contact con, (float x, float y) pnt)> list)
        {
            list.Clear();
            foreach (var e in Target_Contacts)
            {
                var fix = (int)flip;
                var (tdx, tdy) = ToFloat(e.Value.point);
                var (fdx, fdy) = _flipper[fix](tdx, tdy);
                var x = fdx * scale + cx;
                var y = fdy * scale + cy;

                list.Add((e.Key, TIX(e.Key), e.Value.contact, (x, y)));
            }           
        }
        private static byte TIX(Target tg)
        {
            if (tg == Target.E) return 1;
            if (tg == Target.W) return 9;
            if (tg == Target.N) return 13;
            if (tg == Target.S) return 5;

            if (tg == Target.EN) return 0;
            if (tg == Target.ES) return 2;
            if (tg == Target.WS) return 8;
            if (tg == Target.WN) return 10;

            if (tg == Target.NW) return 12;
            if (tg == Target.NE) return 14;
            if (tg == Target.SE) return 4;
            if (tg == Target.SW) return 6;

            if (tg == Target.SEC) return 3;
            if (tg == Target.SWC) return 7;
            if (tg == Target.NWC) return 11;
            if (tg == Target.NEC) return 15;
            return 0;
        }
        private static (float u, float v) ToFloat((sbyte dx, sbyte dy) p) => (p.dx / 127f, p.dy / 127f);
        private static (float, float) ToNone(float x, float y) => (x, y);
        private static (float, float) ToVertFlip(float x, float y) => (x, -y);
        private static (float, float) ToHorzFlip(float x, float y) => (-x, y);
        private static (float, float) ToVertHorzFlip(float x, float y) => (-x, -y);
        private static (float, float) ToLeftRotate(float x, float y) => (y, -x);
        private static (float, float) ToLeftHorzFlip(float x, float y) => (-y, -x);
        private static (float, float) ToRightRotate(float x, float y) => (-y, x);
        private static (float, float) ToRightHorzFlip(float x, float y) => (y, x);

        private static Func<float, float, (float x, float y)>[] _flipper = { ToNone, ToVertFlip, ToHorzFlip, ToVertHorzFlip, ToLeftRotate, ToLeftHorzFlip, ToRightRotate, ToRightHorzFlip };
        #endregion

        #region GetFlipTargetPenalty  =========================================
        internal byte[][] GetFlipTargetPenalty(FlipState flip) => _flipTargetPenalty[(int)flip];
        internal static byte MaxPenalty => 5;
        internal static float[] PenaltyFactor = { 1, 1.1f, 1.2f, 1.3f, 1.4f, 100f };
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
