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
        public List<(Target trg, TargetIndex tix, Contact con, (sbyte dx, sbyte dy) pnt, byte siz)> TargetContacts = new List<(Target, TargetIndex, Contact, (sbyte, sbyte), byte)>(4);
        public Attach Attach;
        public AutoFlip AutoFlip;
        public byte Version;

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
            var list = new List<(float x, float y, float dx, float dy, float siz)>(TargetContacts.Count);
            foreach (var e in TargetContacts)
            {
                var fix = (int)flip;
                var (tdx, tdy) = ToFloat(e.pnt);
                var (fdx, fdy) = _flipper[fix](tdx, tdy);
                var x = fdx * scale + cx;
                var y = fdy * scale + cy;
                var sz = e.siz * scale / 255;
                var (tx, ty) = _targetSurface[(int)e.tix];
                var (dx, dy) = _flipper[fix](tx, ty);
                list.Add((x, y, dx, dy, sz));
            }
            return list;
        }
        private const float Q = 0.7071067811865f; // 1 / SQRT(2)
        private static readonly (float dx, float dy)[] _targetSurface =
        {
            (1, 0),  //EN
            (1, 0),  //E
            (1, 0),  //ES
            (Q, Q), //SEC
            (0, 1),  //SE
            (0, 1),  //S
            (0, 1),  //SW
            (-Q, Q),  //SWC
            (-1, 0),  //WS
            (-1, 0),  //W
            (-1, 0),  //WN
            (-Q, -Q), //NWC
            (0, -1),  //NW
            (0, -1),  //N
            (0, -1),  //NE
            (Q, -Q),  //NEC
        };
        #endregion

        #region GetFlipTargetContacts  =========================================
        internal void GetFlipTargetContacts(FlipState flip, float cx, float cy, float scale, float tmLen, List<(Target trg, byte tix, Contact con, (float x, float y) pnt)> list)
        {
            list.Clear();
            foreach (var (trg, tix, con, pnt, siz) in TargetContacts)
            {
                var fix = (int)flip;
                var (tdx, tdy) = ToFloat(pnt);
                var (fdx, fdy) = _flipper[fix](tdx, tdy);

                var (tx, ty) = _targetSurface[(int)tix];
                var (dx, dy) = _flipper[fix](tx, ty);

                var x = fdx * scale + cx + dx * tmLen;
                var y = fdy * scale + cy + dy * tmLen;

                list.Add((trg, (byte)tix, con, (x, y)));
            }           
        }
        public void GetTargetContacts(Dictionary<Target, (Contact contact, (sbyte dx, sbyte dy) point, byte size)> dict)
        {
            dict.Clear();
            foreach (var (trg, tix, con, pnt, siz) in TargetContacts)
            {
                dict[trg] = (con, pnt, siz);
            }
        }
        public void SetTargetContacts(Dictionary<Target, (Contact contact, (sbyte dx, sbyte dy) point, byte size)> dict)
        {
            TargetContacts.Clear();
            foreach (var e in dict)
            {
                TargetContacts.Add((e.Key, GetTargetIndex(e.Key), e.Value.contact, e.Value.point, e.Value.size));
            }
            TargetContacts.Sort((u,v) => (u.tix < v.tix) ? -1 : (u.tix > v.tix) ? 1 : 0);
        }
        public static TargetIndex GetTargetIndex(Target tg)
        {
            if (tg == Target.E) return TargetIndex.E;
            if (tg == Target.W) return TargetIndex.W;
            if (tg == Target.N) return TargetIndex.N;
            if (tg == Target.S) return TargetIndex.S;

            if (tg == Target.EN) return TargetIndex.EN;
            if (tg == Target.ES) return TargetIndex.ES;
            if (tg == Target.WS) return TargetIndex.WS;
            if (tg == Target.WN) return TargetIndex.WN;

            if (tg == Target.NW) return TargetIndex.NW;
            if (tg == Target.NE) return TargetIndex.NE;
            if (tg == Target.SE) return TargetIndex.SE;
            if (tg == Target.SW) return TargetIndex.SW;

            if (tg == Target.SEC) return TargetIndex.SEC;
            if (tg == Target.SWC) return TargetIndex.SWC;
            if (tg == Target.NWC) return TargetIndex.NWC;
            if (tg == Target.NEC) return TargetIndex.NEC;
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
        private static readonly byte[] _ES  = { 0, 0, 1, 2, 3, 5, 5, 5, 5, 5, 5, 5, 5, 3, 2, 1 }; //ES
        private static readonly byte[] _SEC = { 1, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5, 5, 5, 4, 3, 2 }; //SEC  
        private static readonly byte[] _SE  = { 2, 1, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5, 5, 5, 5, 3 }; //SE
        private static readonly byte[] _S   = { 3, 2, 1, 0, 0, 1, 2, 3, 5, 5, 5, 5, 5, 5, 5, 5 }; //S
        private static readonly byte[] _SW  = { 4, 3, 2, 1, 0, 0, 1, 2, 3, 5, 5, 5, 5, 5, 5, 5 }; //SW
        private static readonly byte[] _SWC = { 5, 4, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5, 5 }; //SWC  
        private static readonly byte[] _WS  = { 5, 5, 5, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5 }; //WS
        private static readonly byte[] _W   = { 5, 5, 5, 5, 3, 2, 1, 0, 0, 1, 2, 3, 5, 5, 5, 5 }; //W
        private static readonly byte[] _WN  = { 5, 5, 5, 5, 5, 3, 2, 1, 0, 0, 1, 2, 3, 5, 5, 5 }; //WN
        private static readonly byte[] _NWC = { 5, 5, 5, 5, 5, 4, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5 }; //NWC  
        private static readonly byte[] _NW  = { 5, 5, 5, 5, 5, 5, 5, 3, 2, 1, 0, 0, 1, 2, 3, 5 }; //NW
        private static readonly byte[] _N   = { 5, 5, 5, 5, 5, 5, 5, 5, 3, 2, 1, 0, 0, 1, 2, 3 }; //N
        private static readonly byte[] _NE  = { 3, 5, 5, 5, 5, 5, 5, 5, 5, 3, 2, 1, 0, 0, 1, 2 }; //NE 
        private static readonly byte[] _NEC = { 2, 3, 4, 5, 5, 5, 5, 5, 5, 4, 3, 2, 1, 0, 0, 1 }; //NEC 
        private static readonly byte[] _EN  = { 1, 2, 3, 5, 5, 5, 5, 5, 5, 5, 5, 3, 2, 1, 0, 0 }; //EN
        private static readonly byte[] _E   = { 0, 1, 2, 3, 5, 5, 5, 5, 5, 5, 5, 5, 3, 2, 1, 0 }; //E

        //==============================  EN   E   ES     SEC   SE   S   SW   SWC     WS   W   WN     NWC   NW   N   NE   NEC
        private static byte[][] _pNo = { _EN, _E, _ES,   _SEC, _SE, _S, _SW, _SWC,   _WS, _W, _WN,   _NWC, _NW, _N, _NE, _NEC };//
        private static byte[][] _pVF = { _ES, _E, _EN,   _NEC, _NE, _N, _NW, _NWC,   _WN, _W, _WS,   _SWC, _SW, _S, _SE, _SEC };//
        private static byte[][] _pHF = { _WN, _W, _WS,   _SWC, _SW, _S, _SE, _SEC,   _ES, _E, _EN,   _NEC, _NE, _N, _NW, _NWC };//
        private static byte[][] _pVH = { _WS, _W, _WN,   _NWC, _NW, _N, _NE, _NEC,   _EN, _E, _ES,   _SEC, _SE, _S, _SW, _SWC };//
        private static byte[][] _pLR = { _NW, _N, _NE,   _NEC, _EN, _E, _EN, _NEC,   _NE, _N, _NW,   _NWC, _WN, _W, _WS, _NWC };//
        private static byte[][] _pLH = { _SW, _S, _SE,   _SEC, _ES, _E, _EN, _NEC,   _NE, _N, _NW,   _NWC, _WN, _W, _WS, _SWC };//
        private static byte[][] _pRR = { _SE, _S, _SW,   _SWC, _WS, _S, _WN, _NWC,   _NW, _N, _NE,   _NEC, _EN, _E, _ES, _SEC };//
        private static byte[][] _pRH = { _NE, _N, _NW,   _NWC, _WN, _W, _WS, _SWC,   _SW, _S, _SE,   _SEC, _ES, _E, _EN, _NEC };//

        private static byte[][][] _flipTargetPenalty = { _pNo, _pVF, _pHF, _pVH, _pLR, _pLH, _pRR, _pRH };
        #endregion

        #endregion
    }
}
