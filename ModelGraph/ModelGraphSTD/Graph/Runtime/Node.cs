
using System.Numerics;

namespace ModelGraphSTD
{
    public class Node : NodeEdge
    {
        public Item Item;
        public int OpenPathIndex = -1;

        public float X;
        public float Y;
        public byte DX;
        public byte DY;
        public byte Color;
        public byte Symbol;
        public Labeling Labeling;
        public Sizing Sizing;
        public BarWidth BarWidth;
        public FlipState FlipState;
        public Aspect Aspect;

        #region Snapshot  =====================================================
        internal (float X, float Y, byte DX, byte DY, byte Color, byte Symbol, Labeling Labeling, Sizing Resizing, BarWidth BarWidth, FlipState FlipRotate, Aspect Orientation)
            Snapshot
        {
            get { return (X, Y, DX, DY, Color, Symbol, Labeling, Sizing, BarWidth, FlipState, Aspect); }
            set
            {
                X = value.X;
                Y = value.Y;
                DX = value.DX;
                DY = value.DY;
                Color = value.Color;
                Symbol = value.Symbol;
                Labeling = value.Labeling;
                Sizing = value.Resizing;
                BarWidth = value.BarWidth;
                FlipState = value.FlipRotate;
                Aspect = value.Orientation;
            }
        }
        #endregion

        #region Constructor  ==================================================
        internal Node()
        {
            Owner = null;
            Trait = Trait.Node;
            DX = DY = (byte)GraphDefault.MinNodeSize;
        }
        #endregion

        #region Booleans  =====================================================
        public bool IsGraphPoint => Aspect == Aspect.Point;

        public bool IsGraphNode => Symbol == 0;
        public bool IsGraphEgress => Symbol == 1;
        public bool IsGraphSymbol => Symbol > 1;


        public bool IsAutoResizing => Sizing == Sizing.Auto;

        public bool IsMasked { get { return (IsGraphNode && Aspect != Aspect.Square && Sizing == Sizing.Manual); } }

        public bool IsNodePoint => IsGraphNode && Aspect == Aspect.Point; 
        public bool IsAutoSizing { get { return (Sizing == Sizing.Auto && Aspect != Aspect.Point); } }
        public bool IsFixedSizing { get { return (Sizing == Sizing.Fixed && Aspect != Aspect.Point); } }
        public bool IsManualSizing { get { return (IsGraphNode && Sizing == Sizing.Manual && Aspect != Aspect.Point); } }
        #endregion

        #region Center, Extent, Radius  =======================================
        public (float x, float y) Center { get { return (X, Y); } set { X = value.x; Y = value.y; } }
        public (float X1, float Y1, float X2, float Y2) Extent => (X - DX, Y - DY, X + DX, Y + DY);
        public (float X1, float Y1, float X2, float Y2, float DX, float DY, Node node) FullExtent(int ds)
        {
            var x1 = X - DX - ds;
            var y1 = Y - DY - ds;
            var x2 = X + DX + ds;
            var y2 = Y + DY + ds;
            var dx = x2 - x1;
            var dy = y2 - y1;
            return (x1, y1, x2, y2, dx, dy, this);
        }
        public int Radius => (DX + DY) / 2;
        #endregion

        #region Tryinitialize  ================================================
        static byte _min = (byte)GraphDefault.MinNodeSize;
        static byte _max = byte.MaxValue;

        internal bool TryInitialize(int cp)
        {
            if (DX == 0 || DY == 0) { DX = DY = _min; }
            if (X == 0 || Y == 0) { X = Y = cp; return true; }

            return false;
        }
        #endregion

        #region SetSize, GetValues  ===========================================
        internal void SetSize(float x, float y)
        {
            DX = (byte)((x < _min) ? _min : ((x > _max) ? _max : x));
            DY = (byte)((y < _min) ? _min : ((y > _max) ? _max : y));
        }
        internal (float x, float y, float w, float h) Values()
        {
            return IsGraphPoint ? (X, Y, 1, 1) : (X, Y, DX, DY);
        }
        internal (float X, float Y) GetCenter() => (X, Y);
        internal int[] CenterXY
        {
            get { return new int[] { (int)X, (int)Y }; }
            set { if (value != null && value.Length == 2) { X = value[0]; Y = value[1]; } }
        }

        internal int[] SizeWH
        {
            get { return new int[] { DX, DY }; }
            set { if (value != null && value.Length == 2) { DX = ValidSize(value[0]); DY = ValidSize(value[1]); } }
        }
        private byte ValidSize(int val)
        {
            if (val < _min) return _min;
            if (val > _max) return _max;
            return (byte)val;
        }
        #endregion

        #region Flip, Align  ==================================================
        internal void AlignTop(float y) { Y = y + DY; }

        internal void AlignLeft(float x) { X = x + DX; }

        internal void AlignRight(float x) { X = x - DX; }

        internal void AlignBottom(float y) { Y = y - DY; }

        internal void AlignVert(float x) { X = x; }

        internal void AlignHorz(float y) { Y = y; }

        internal void FlipVert(float y) { Y = y + (y - Y); }

        internal void FlipHorz(float x) { X = x + (x - X); }
        #endregion

        #region Move, Resize  =================================================
        internal void Resize((float X, float Y) delta)
        {
            var dx = DX + delta.X;
            var dy = DY + delta.Y;
            DX = (byte)((dx < _min) ? _min : ((dx > _max) ? _max : dx));
            DY = (byte)((dy < _min) ? _min : ((dy > _max) ? _max : dx));
        }
        internal void Move((float X, float Y) delta)
        {
            X = X + delta.X;
            Y = Y + delta.Y;
        }
        #endregion

        #region Minimize, HitTest  ============================================
        static readonly int _ds = GraphDefault.HitMargin;

        public void Minimize(Extent e)
        {
            var t = e;
            var p = Center;
            t.Point2 = p;
            if (t.Diagonal < e.Diagonal) e.Point2 = p;
        }


        // quickly eliminate nodes that don't qaulify
        public bool HitTest((float X, float Y) p)
        {
            var x = p.X + 1;
            if (x < (X - DX - _ds)) return false;
            if (x > (X + DX + _ds)) return false;

            var y = p.Y + 1;
            if (y < (Y - DY - _ds)) return false;
            if (y > (Y + DY + _ds)) return false;
            return true;
        }

        public (HitLocation hit, (float X, float Y) pnt) RefinedHit((float X, float Y) p)
        {
            float ds;
            var x = p.X + 1;
            var y = p.Y + 1;
            var hit = HitLocation.Node;
            var pnt = (X, Y);

            if (DX >= _ds)
            {
                if (x < X)
                {
                    ds = X - DX - x;
                    if (ds < 0 && ds + _ds >= 0) hit |= HitLocation.Left;
                    else if (ds > 0 && ds - _ds <= 0) hit |= HitLocation.Left;
                }
                else
                {
                    ds = X + DX - x;
                    if (ds < 0 && ds + _ds >= 0) hit |= HitLocation.Right;
                    else if (ds > 0 && ds - _ds <= 0) hit |= HitLocation.Right;
                }
            }

            if (DY >= _ds)
            {
                if (y < Y)
                {
                    ds = Y - DY - y;
                    if (ds < 0 && ds + _ds >= 0) hit |= HitLocation.Top;
                    else if (ds > 0 && ds - _ds <= 0) hit |= HitLocation.Top;
                }
                else
                {
                    ds = Y + DY - y;
                    if (ds < 0 && ds + _ds >= 0) hit |= HitLocation.Bottom;
                    else if (ds > 0 && ds - _ds <= 0) hit |= HitLocation.Bottom;
                }
            }
            return (hit, pnt);
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal Graph Graph { get { return Owner as Graph; } }
        internal GraphX GraphX { get { return (Owner == null) ? null : Owner.Owner as GraphX; } }

        internal bool HasOpenPaths => (OpenPathIndex >= 0);
        internal int OpenPathCount => (Graph == null) ? 0 : Graph.OpenPathCount(OpenPathIndex);
        public override string ToString() => GetChef().GetIdentity(Item, IdentityStyle.Double);
        #endregion
    }
}
