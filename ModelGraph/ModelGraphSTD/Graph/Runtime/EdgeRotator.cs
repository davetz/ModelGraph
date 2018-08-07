using System;

namespace ModelGraphSTD
{
    public class EdgeRotator
    {
        private byte n;
        private byte s;
        private byte w;
        private byte e;

        private byte N;
        private byte S;
        private byte W;
        private byte E;

        #region Constructors  =============================================
        internal EdgeRotator(Connect connect)
        {
            if (connect == Connect.Any)
            {
                n = N = (byte)Connect.North;
                s = S = (byte)Connect.South;
                w = W = (byte)Connect.West;
                e = E = (byte)Connect.East;
            }
            else
            {
                n = N = (byte)(connect & Connect.North);
                s = S = (byte)(connect & Connect.South);
                w = W = (byte)(connect & Connect.West);
                e = E = (byte)(connect & Connect.East);
            }
        }
        #endregion

        internal void SetFlipRotate(int value) { _action[value & 7](this); }

        internal bool CanConnect(int index) { return _connect[index & 3](this); } // (E, S, W, N)

        #region VectorTables  =============================================
        private static Func<EdgeRotator, bool>[] _connect = new Func<EdgeRotator, bool>[]
        {
                (r) => { return r.E != 0; },
                (r) => { return r.S != 0; },
                (r) => { return r.W != 0; },
                (r) => { return r.N != 0; },
        };

        private static Action<EdgeRotator>[] _action = new Action<EdgeRotator>[]
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

        private static void FlipRotateNone(EdgeRotator r)
        {
            r.N = r.n;
            r.S = r.s;
            r.W = r.w;
            r.E = r.e;
        }
        private static void FlipVertical(EdgeRotator r)
        {
            r.N = r.s;
            r.S = r.n;
            r.W = r.w;
            r.E = r.e;
        }
        private static void FlipHorizontal(EdgeRotator r)
        {
            r.N = r.n;
            r.S = r.s;
            r.W = r.e;
            r.E = r.w;
        }
        private static void FlipBothWays(EdgeRotator r)
        {
            r.N = r.s;
            r.S = r.n;
            r.W = r.e;
            r.E = r.w;
        }
        private static void RotateClockWise(EdgeRotator r)
        {
            r.N = r.w;
            r.S = r.e;
            r.W = r.s;
            r.E = r.n;
        }
        private static void RotateFlipVertical(EdgeRotator r)
        {
            r.N = r.e;
            r.S = r.w;
            r.W = r.s;
            r.E = r.n;
        }
        private static void RotateFlipHorizontal(EdgeRotator r)
        {
            r.N = r.w;
            r.S = r.e;
            r.W = r.n;
            r.E = r.s;
        }
        private static void RotateFlipBothWays(EdgeRotator r)
        {
            r.N = r.e;
            r.S = r.w;
            r.W = r.n;
            r.E = r.s;
        }
        #endregion
    }
}
