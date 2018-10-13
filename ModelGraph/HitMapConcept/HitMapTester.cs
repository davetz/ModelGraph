using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HitMapConcept
{
    internal class HitMapTester
    {
        private HashSet<long> _nodeHitX = new HashSet<long>();
        private HashSet<long> _nodeHitY = new HashSet<long>();
        private HashSet<long> _edgeHitX = new HashSet<long>();
        private HashSet<long> _edgeHitY = new HashSet<long>();

        internal void Clear()
        {
            _nodeHitX.Clear();
            _nodeHitY.Clear();
            _edgeHitX.Clear();
            _edgeHitY.Clear();
        }

        internal bool HitNode(int x, int y)
        {
            return (_nodeHitX.Contains(x) && _nodeHitY.Contains(y));
        }
        internal bool HitEdge(int x, int y)
        {
            return (_edgeHitX.Contains(x) && _edgeHitY.Contains(y));
        }
        internal void RecordNode(int x, int y, int w, int h)
        {
            var x
        }

        internal void RecordSegment(int xi1, int yi1, int xi2, int yi2)
        {
            var x1 = xi1 / 5.0;
            var y1 = yi1 / 5.0;
            var x2 = xi2 / 5.0;
            var y2 = yi2 / 5.0;

            var dx = x2 - x1;
            var dy = y2 - y1;

            if (dx > 0)
                PosDX();
            else if (dx < 0)
                NegDX();
            else
                _edgeHitX.Add((int)x1);

            if (dy > 0)
                PosDY();
            else if (dy < 0)
                NegDY();
            else
                _edgeHitY.Add((int)y1);

            void PosDX()
            {
                for (var x = x1; x <= x2; x++)
                {
                    _edgeHitX.Add((int)x);
                }
            }
            void PosDY()
            {
                for (var y = y1; y <= y2; y++)
                {
                    _edgeHitY.Add((int)y);
                }
            }
            void NegDX()
            {
                for (var x = x2; x >= x1; x--)
                {
                    _edgeHitX.Add((int)x);
                }
            }
            void NegDY()
            {
                for (var y = y2; y >= y1; y--)
                {
                    _edgeHitY.Add((int)y);
                }
            }
        }
    }
}
