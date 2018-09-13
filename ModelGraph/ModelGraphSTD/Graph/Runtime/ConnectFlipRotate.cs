using System;

namespace ModelGraphSTD
{
    public class ConnectFlipRotate
    {
        // restrictions as defined in QueryX for this edge
        private byte N; // allow north side connections
        private byte S; // allow south side connections
        private byte W; // allow west side connections
        private byte E; // allow east side connections

        // same restrictions after the symbol has been flipped/rotated
        private byte n;
        private byte s;
        private byte w;
        private byte e;

        #region Constructors  =============================================
        internal ConnectFlipRotate(Connect connect)
        {
            if (connect == Connect.Any)
            {
                N = n = (byte)Connect.North;
                S = s = (byte)Connect.South;
                W = w = (byte)Connect.West;
                E = e = (byte)Connect.East;
            }
            else
            {
                N = n = (byte)(connect & Connect.North);
                S = s = (byte)(connect & Connect.South);
                W = w = (byte)(connect & Connect.West);
                E = e = (byte)(connect & Connect.East);
            }
        }
        #endregion

        internal void SetFlipRotate(int value) { _action[value & 7](this); }

        internal bool CanConnect(int index) { return _connect[index & 3](this); } // (E, S, W, N)

        #region VectorTables  =============================================
        private static Func<ConnectFlipRotate, bool>[] _connect = new Func<ConnectFlipRotate, bool>[]
        {
                (r) => { return r.e != 0; },
                (r) => { return r.s != 0; },
                (r) => { return r.w != 0; },
                (r) => { return r.n != 0; },
        };

        private static Action<ConnectFlipRotate>[] _action = new Action<ConnectFlipRotate>[]
        {
                FlipRotateNone,
                FlipVertical,
                FlipHorizontal,
                FlipBothWays,
                RotateClockWise,
                RotateFlipVertical,
                RotateFlipHorizontal,
                RotateFlipBothWays,
        };

        private static void FlipRotateNone(ConnectFlipRotate r)
        {
            r.n = r.N;
            r.s = r.S;
            r.w = r.W;
            r.e = r.E;
        }
        private static void FlipVertical(ConnectFlipRotate r)
        {
            r.n = r.S;
            r.s = r.N;
            r.w = r.W;
            r.e = r.E;
        }
        private static void FlipHorizontal(ConnectFlipRotate r)
        {
            r.n = r.N;
            r.s = r.S;
            r.w = r.E;
            r.e = r.W;
        }
        private static void FlipBothWays(ConnectFlipRotate r)
        {
            r.n = r.S;
            r.s = r.N;
            r.w = r.E;
            r.e = r.W;
        }
        private static void RotateClockWise(ConnectFlipRotate r)
        {
            r.n = r.W;
            r.s = r.E;
            r.w = r.S;
            r.e = r.N;
        }
        private static void RotateFlipVertical(ConnectFlipRotate r)
        {
            r.n = r.E;
            r.s = r.W;
            r.w = r.S;
            r.e = r.N;
        }
        private static void RotateFlipHorizontal(ConnectFlipRotate r)
        {
            r.n = r.W;
            r.s = r.E;
            r.w = r.N;
            r.e = r.S;
        }
        private static void RotateFlipBothWays(ConnectFlipRotate r)
        {
            r.n = r.E;
            r.s = r.W;
            r.w = r.N;
            r.e = r.S;
        }
        #endregion
    }
}
