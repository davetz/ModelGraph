using Windows.Foundation;

namespace ModelGraphLibrary
{/*

 */
    internal struct NodeX
    {
        internal int X;
        internal int Y;
        internal byte DX;
        internal byte DY;
        internal byte Symbol;
        internal Labeling Labeling;
        internal Resizing Resizing;
        internal BarWidth BarWidth;
        internal FlipRotate FlipRotate;
        internal Orientation Orientation;

        internal XYPoint Center => new XYPoint(X, Y);
        internal Extent Extent => new Extent(X - DX, Y - DY, X + DX, Y + DY);
        internal Rect Rect => new Rect(X - DX, Y - DY, DX * 2, DY * 2);
        internal int Radius => (DX + DY) / 2;

        #region Booleans  =====================================================
        internal bool IsVertical { get { return Orientation == Orientation.Vertical; } }
        internal bool IsHorizontal { get { return Orientation == Orientation.Horizontal; } }

        internal bool IsNode { get { return Symbol == 0; } }
        internal bool IsEgress { get { return Symbol == 1; } }
        internal bool IsSymbol { get { return Symbol > 1; } }
        internal bool IsMasked { get { return (IsNode && Orientation != Orientation.Central && Resizing == Resizing.Manual); } }

        internal bool IsPointNode { get { return IsNode && Orientation == Orientation.Point; } }
        internal bool IsAutoSizing { get { return (Resizing == Resizing.Auto && Orientation != Orientation.Point); } }
        internal bool IsFixedSizing { get { return (Resizing == Resizing.Fixed && Orientation != Orientation.Point); } }
        internal bool IsManualSizing { get { return (IsNode && Resizing == Resizing.Manual && Orientation != Orientation.Point); } }
        internal bool IsVerticalSizing { get { return (Resizing == Resizing.Auto && (Orientation == Orientation.Vertical || Orientation == Orientation.Central)); } }
        internal bool IsHorizontalSizing { get { return (Resizing == Resizing.Auto && (Orientation == Orientation.Horizontal || Orientation == Orientation.Central)); } }
        #endregion

        #region Tryinitialize  ================================================
        static byte _min = (byte)GraphParm.MinNodeSize;
        static byte _max = byte.MaxValue;

        internal bool TryInitialize(int cp)
        {
            if (DX == 0 || DY == 0) { DX = DY = _min; }
            if (X == 0 || Y == 0) { X = Y = cp; return true; }

            return false;
        }
        #endregion

        #region SetSize, GetValues  ===========================================
        static int _maxValue = GraphParm.CenterOffset * 2;

        internal void SetSize(int x, int y)
        {
            DX = (byte)((x < _min) ? _min : ((x > _max) ? _max : x));
            DY = (byte)((y < _min) ? _min : ((y > _max) ? _max : y));
        }
        internal (int x, int y, int w, int h) Values()
        {
            return IsPointNode ? (X, Y, 1, 1) : (X, Y, DX, DY);
        }
        internal bool TryGetCenter(out int x, out int y)
        {
            x = X;
            y = Y;
            return (X != 0) && (Y != 0);
        }
        internal int[] CenterXY
        {
            get { return new int[] { X, Y }; }
            set { if (value != null && value.Length == 2) { X = ValidValue(value[0]); Y = ValidValue(value[1]); } }
        }
        internal int ValidValue(int val)
        {
            if (val < 0) return 0;
            if (val > _maxValue) return _maxValue;
            return val;
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

        internal void Resize(XYPoint delta)
        {
            var dx = DX + delta.X;
            var dy = DY + delta.Y;
            DX = (byte)((dx < _min) ? _min : ((dx > _max) ? _max : dx));
            DY = (byte)((dy < _min) ? _min : ((dy > _max) ? _max : dx));
        }

        internal void Rotate(XYPoint p, SymbolX sym)
        {
            var x = X;
            var y = Y;
            var dx = DX;
            var dy = DY;
            X = (int)(p.X + (y - p.Y));
            Y = (int)(p.Y - (x - p.X));
            DX = dy;
            DY = dx;

            switch (Orientation)
            {
                case Orientation.Point:
                case Orientation.Central: break;
                case Orientation.Vertical: Orientation = Orientation.Horizontal; break;
                case Orientation.Horizontal: Orientation = Orientation.Vertical; break;
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

        internal void SetOrientation(Orientation val, SymbolX sym)
        {
            if (sym == null)
            {
                switch (Orientation)
                {
                    case Orientation.Point:
                    case Orientation.Central: break;
                    case Orientation.Vertical: Orientation = Orientation.Horizontal; break;
                    case Orientation.Horizontal: Orientation = Orientation.Vertical; break;
                }
                FlipRotate = FlipRotate.None;
            }
            else
            {
                switch (Orientation)
                {
                    case Orientation.Point:
                    case Orientation.Central: break;
                    case Orientation.Vertical: Orientation = Orientation.Horizontal; break;
                    case Orientation.Horizontal: Orientation = Orientation.Vertical; break;
                }
            }
        }
        #endregion

        #region Minimize, HitTest  ============================================
        static int _ds = GraphParm.HitMargin;
        static int _ds2 = GraphParm.HitMarginSquared;

        internal void Minimize(ref Extent e)
        {
            var t = e;
            var p = Center;
            t.Point2 = p;
            if (t.Diagonal < e.Diagonal) e.Point2 = p;
        }


        // quickly eliminate nodes that don't qaulify
        internal bool HitTest(XYPoint p)
        {
            var x = p.X + 1;
            if (x < (X - DX - _ds)) return false;
            if (x > (X + DX + _ds)) return false;

            var y = p.Y + 1;
            if (y < (Y - DY - _ds)) return false;
            if (y > (Y + DY + _ds)) return false;
            return true;
        }

        internal (HitLocation hit, XYPoint pnt) RefinedHit(XYPoint p)
        {
            int ds;
            int x = p.X + 1;
            int y = p.Y + 1;
            var hit = HitLocation.Node;
            var pnt = new XYPoint(X, Y);

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

        internal void RefineHitTest(XYPoint p, ref HitLocation hit, ref XYPoint hitPoint)
        {
            hit |= HitLocation.Node;
            hitPoint = new XYPoint(X, Y);
            int ds;

            if (DX >= _ds)
            {
                if (p.X < X)
                {
                    ds = X - DX - p.X;
                    if (ds < 0 && ds + _ds >= 0) hit |= HitLocation.Left;
                    else if (ds > 0 && ds - _ds <= 0) hit |= HitLocation.Left;
                }
                else
                {
                    ds = X + DX - p.X;
                    if (ds < 0 && ds + _ds >= 0) hit |= HitLocation.Right;
                    else if (ds > 0 && ds - _ds <= 0) hit |= HitLocation.Right;
                }
            }

            if (DY >= _ds)
            {
                if (p.Y < Y)
                {
                    ds = Y - DY - p.Y;
                    if (ds < 0 && ds + _ds >= 0) hit |= HitLocation.Top;
                    else if (ds > 0 && ds - _ds <= 0) hit |= HitLocation.Top;
                }
                else
                {
                    ds = Y + DY - p.Y;
                    if (ds < 0 && ds + _ds >= 0) hit |= HitLocation.Bottom;
                    else if (ds > 0 && ds - _ds <= 0) hit |= HitLocation.Bottom;
                }
            }
        }
        #endregion
    }
}
