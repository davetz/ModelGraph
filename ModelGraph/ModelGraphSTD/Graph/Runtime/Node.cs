﻿
namespace ModelGraphSTD
{
    public class Node : NodeEdge
    {
        public Item Item;
        public int OpenPathIndex = -1;

        #region Parms  ========================================================
        public int X;
        public int Y;
        public byte DX;
        public byte DY;
        public byte Color;
        public byte Symbol;
        public Labeling Labeling;
        public Sizing Sizing;
        public BarWidth BarWidth;
        public FlipRotate FlipRotate;
        public Aspect Aspect;

        internal (int X, int Y, byte DX, byte DY, byte Color, byte Symbol, Labeling Labeling, Sizing Resizing, BarWidth BarWidth, FlipRotate FlipRotate, Aspect Orientation)
            Parms
        {
            get { return (X, Y, DX, DY, Color, Symbol, Labeling, Sizing, BarWidth, FlipRotate, Aspect); }
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
                FlipRotate = value.FlipRotate;
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

        public bool IsMasked { get { return (IsGraphNode && Aspect != Aspect.Central && Sizing == Sizing.Manual); } }

        public bool IsNodePoint => IsGraphNode && Aspect == Aspect.Point; 
        public bool IsAutoSizing { get { return (Sizing == Sizing.Auto && Aspect != Aspect.Point); } }
        public bool IsFixedSizing { get { return (Sizing == Sizing.Fixed && Aspect != Aspect.Point); } }
        public bool IsManualSizing { get { return (IsGraphNode && Sizing == Sizing.Manual && Aspect != Aspect.Point); } }
        #endregion

        #region Center, Extent, Radius  =======================================
        public (int x, int y) Center => (X, Y);
        public (int X1, int Y1, int X2, int Y2) Extent => (X - DX, Y - DY, X + DX, Y + DY);
        public (int X1, int Y1, int X2, int Y2, int DX, int DY, Node node) FullExtent(int ds)
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
        internal void SetSize(int x, int y)
        {
            DX = (byte)((x < _min) ? _min : ((x > _max) ? _max : x));
            DY = (byte)((y < _min) ? _min : ((y > _max) ? _max : y));
        }
        internal (int x, int y, int w, int h) Values()
        {
            return IsGraphPoint ? (X, Y, 1, 1) : (X, Y, DX, DY);
        }
        internal (int X, int Y) GetCenter() => (X, Y);
        internal int[] CenterXY
        {
            get { return new int[] { X, Y }; }
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

        #region Align, Flip  =================================================
        internal void AlignTop(int y) { Y = y + DY; }

        internal void AlignLeft(int x) { X = x + DX; }

        internal void AlignRight(int x) { X = x - DX; }

        internal void AlignBottom(int y) { Y = y - DY; }

        internal void AlignVertical(int x) { X = x; }

        internal void AlignHorizontal(int y) { Y = y; }

        internal void VerticalFlip(int y) { Y = y + (y - Y); }

        internal void HorizontalFlip(int x) { X = x + (x - X); }
        #endregion

        #region Move, Resize, Flip, Rotate  ===================================

        internal void Resize((int X, int Y) delta)
        {
            var dx = DX + delta.X;
            var dy = DY + delta.Y;
            DX = (byte)((dx < _min) ? _min : ((dx > _max) ? _max : dx));
            DY = (byte)((dy < _min) ? _min : ((dy > _max) ? _max : dx));
        }

        internal void Rotate((int X, int Y) p, SymbolX sym)
        {
            var x = X;
            var y = Y;
            var dx = DX;
            var dy = DY;
            X = (p.X + (y - p.Y));
            Y = (p.Y - (x - p.X));
            DX = dy;
            DY = dx;

            switch (Aspect)
            {
                case Aspect.Point:
                case Aspect.Central: break;
                case Aspect.Vertical: Aspect = Aspect.Horizontal; break;
                case Aspect.Horizontal: Aspect = Aspect.Vertical; break;
            }
        }

        internal void FlipVertical(SymbolX sym)
        {
            if (sym == null)
            {

            }
            else
            {

            }
        }

        internal void FlipHorizontal(SymbolX sym)
        {
            if (sym == null)
            {

            }
            else
            {

            }
        }

        internal void SetFlipRotate(FlipRotate val, SymbolX sym)
        {
            if (sym == null)
            {

            }
            else
            {
                switch (val)
                {
                    case FlipRotate.None:
                        break;
                    case FlipRotate.FlipVertical:
                        break;
                    case FlipRotate.FlipHorizontal:
                        break;
                    case FlipRotate.FlipBothWays:
                        break;
                    case FlipRotate.RotateClockWise:
                        break;
                    case FlipRotate.RotateFlipVertical:
                        break;
                    case FlipRotate.RotateFlipHorizontal:
                        break;
                    case FlipRotate.RotateFlipBothWays:
                        break;
                }
            }
        }

        internal void SetOrientation(Aspect val, SymbolX sym)
        {
            if (sym == null)
            {
                switch (Aspect)
                {
                    case Aspect.Point:
                    case Aspect.Central: break;
                    case Aspect.Vertical: Aspect = Aspect.Horizontal; break;
                    case Aspect.Horizontal: Aspect = Aspect.Vertical; break;
                }
                FlipRotate = FlipRotate.None;
            }
            else
            {
                switch (Aspect)
                {
                    case Aspect.Point:
                    case Aspect.Central: break;
                    case Aspect.Vertical: Aspect = Aspect.Horizontal; break;
                    case Aspect.Horizontal: Aspect = Aspect.Vertical; break;
                }
            }
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
        public bool HitTest((int X, int Y) p)
        {
            var x = p.X + 1;
            if (x < (X - DX - _ds)) return false;
            if (x > (X + DX + _ds)) return false;

            var y = p.Y + 1;
            if (y < (Y - DY - _ds)) return false;
            if (y > (Y + DY + _ds)) return false;
            return true;
        }

        public (HitLocation hit, (int X, int Y) pnt) RefinedHit((int X, int Y) p)
        {
            int ds;
            int x = p.X + 1;
            int y = p.Y + 1;
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

        internal void Move((int X, int Y) delta)
        {
            X = X + delta.X;
            Y = Y + delta.Y;
        }
        public void RotateFlip((int X, int Y) focus, FlipRotate flip)
        {
            FlipRotate = flip;

            var p = XYPair.RotateFlip((X, Y), focus, flip);
            X = p.X;
            Y = p.Y;

            switch (flip)
            {
                case ModelGraphSTD.FlipRotate.RotateClockWise:
                case ModelGraphSTD.FlipRotate.RotateFlipVertical:
                case ModelGraphSTD.FlipRotate.RotateFlipHorizontal:
                case ModelGraphSTD.FlipRotate.RotateFlipBothWays:

                    var t1 = DX;
                    DX = DY;
                    DY = t1; 
                    break;
            }
        }
        public override string ToString() => GetChef().GetIdentity(Item, IdentityStyle.Double);
        #endregion
    }
}
