using System;

namespace ModelGraphSTD
{
    public class ConnectFlip
    {
        // restrictions as defined in QueryX for this edge
        private byte ConnectNorth; // allow north side connections
        private byte ConnectSouth; // allow south side connections
        private byte ConnectWest; // allow west side connections
        private byte ConnectEast; // allow east side connections

        // same restrictions after the symbol has been flipped/rotated
        private byte north;
        private byte south;
        private byte west;
        private byte east;
        private bool hasBeenUsed;
        private bool isPriority1;

        #region Constructors  =============================================
        internal ConnectFlip(Connect connect)
        {
            if (connect == Connect.Any)
            {
                ConnectNorth = north = (byte)Connect.North;
                ConnectSouth = south = (byte)Connect.South;
                ConnectWest = west = (byte)Connect.West;
                ConnectEast = east = (byte)Connect.East;
            }
            else
            {
                isPriority1 = true;
                ConnectNorth = north = (byte)(connect & Connect.North);
                ConnectSouth = south = (byte)(connect & Connect.South);
                ConnectWest = west = (byte)(connect & Connect.West);
                ConnectEast = east = (byte)(connect & Connect.East);
            }
        }
        #endregion

        internal void SetFlip(int value) { _setFlipRotate[value & 7](this); hasBeenUsed = false; }

        internal bool CanConnect(int index)
        {
            if (hasBeenUsed || !_canConnect[index & 3](this)) return false;
            hasBeenUsed = true;
            return true;
        }

        internal bool HasNotBeenUsed => !hasBeenUsed;
        internal bool IsPriority1 => isPriority1;

        #region VectorTables  =============================================
        private static Func<ConnectFlip, bool>[] _canConnect = new Func<ConnectFlip, bool>[]
        {
                (r) => { return r.east != 0; },
                (r) => { return r.south != 0; }, 
                (r) => { return r.west != 0; },
                (r) => { return r.north != 0; },
        };

        private static Action<ConnectFlip>[] _setFlipRotate = new Action<ConnectFlip>[]
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

        private static void FlipRotateNone(ConnectFlip r)
        {
            r.north = r.ConnectNorth;
            r.south = r.ConnectSouth;
            r.west = r.ConnectWest;
            r.east = r.ConnectEast;
        }
        private static void FlipVertical(ConnectFlip r)
        {
            r.north = r.ConnectSouth;
            r.south = r.ConnectNorth;
            r.west = r.ConnectWest;
            r.east = r.ConnectEast;
        }
        private static void FlipHorizontal(ConnectFlip r)
        {
            r.north = r.ConnectNorth;
            r.south = r.ConnectSouth;
            r.west = r.ConnectEast;
            r.east = r.ConnectWest;
        }
        private static void FlipBothWays(ConnectFlip r)
        {
            r.north = r.ConnectSouth;
            r.south = r.ConnectNorth;
            r.west = r.ConnectEast;
            r.east = r.ConnectWest;
        }
        private static void RotateClockWise(ConnectFlip r)
        {
            r.north = r.ConnectWest;
            r.south = r.ConnectEast;
            r.west = r.ConnectSouth;
            r.east = r.ConnectNorth;
        }
        private static void RotateFlipVertical(ConnectFlip r)
        {
            r.north = r.ConnectEast;
            r.south = r.ConnectWest;
            r.west = r.ConnectSouth;
            r.east = r.ConnectNorth;
        }
        private static void RotateFlipHorizontal(ConnectFlip r)
        {
            r.north = r.ConnectWest;
            r.south = r.ConnectEast;
            r.west = r.ConnectNorth;
            r.east = r.ConnectSouth;
        }
        private static void RotateFlipBothWays(ConnectFlip r)
        {
            r.north = r.ConnectEast;
            r.south = r.ConnectWest;
            r.west = r.ConnectNorth;
            r.east = r.ConnectSouth;
        }
        #endregion
    }
}
