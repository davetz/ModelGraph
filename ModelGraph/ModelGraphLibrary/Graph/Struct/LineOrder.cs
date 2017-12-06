using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{/*
    Construct an optimumly ordered line list for the given node.
    Consider the given node is at the center of a circle and the
    line connections are represented as a sequence of radial vectors. 
    Order the lines so that the radial vectors progress arround the circle
    in a clockwise direction. The circle has 8 sectors and 4 qaudrants as
    shown below. Keep track of the number of lines in each quadrant and
    sector.

         sect         quad       direction
        =======       ====       =========
        5\6|7/8        3|4           N
        ~~~+~~~        ~+~         W + E
        4/3|2\1        2|1           S
 */
    public struct LineOrder
    {
        internal Node Node;         // the given node
        internal int Count;         // the number of closed lines

        internal Edge[] Opens;      // list of open ended lines
        internal Edge[] Lines;      // list of closed lines (connects to someother node)
        internal Node[] Other;      // list of nodes at the other end of the line
        internal XYPoint[] Bends;  // list of inflection points closest to the node
        internal EdgeRotator[] Conn;// flip and rotate edge connects

        internal int[] Quad;        // the line's quadrant code (1,2,3,4)
        internal int[] Sect;        // the line's sector code (1,2,3,4,5,6,7,8)
        internal int[] NQuad;       // the total number of lines in quadrants 1,2,3,4 
        internal int[] NSect;       // the total number of lines in sectors 1,2,3,4,5,6,7,8
        internal int[] SectLine;    // iine index range for each section


        internal LineOrder(Node node, List<Edge> edges)
        {
            #region Initialize  ===============================================
            Node = node;

            var opens = new List<Edge>();
            var lines = new List<Edge>();
            var bends = new List<XYPoint>();
            var others = new List<Node>();

            foreach (var edge in edges)
            {
                var other = edge.OtherNode(Node);
                if (other == null)
                {
                    opens.Add(edge);
                }
                else
                {
                    others.Add(other);
                    lines.Add(edge);
                    bends.Add(edge.GetClosestBend(Node));
                }
            }

            Count = lines.Count;

            Opens = opens.ToArray();    // list of open ended lines
            Lines = lines.ToArray();    // list of lines
            Other = others.ToArray();   // list of the other end line node
            Bends = bends.ToArray();    // list of closest line bend;

            Conn = new EdgeRotator[Count];// flip and rotate edge connects

            Quad = new int[Count];	    //cartesian quadrant code 1,2,3,4
            Sect = new int[Count];	    //sector code 1-8
            NQuad = new int[5];	        //number of connections in the quadrant
            NSect = new int[9];	        //number of connections in the sector
            SectLine = new int[10];	    //Line index range for each section 
            #endregion

            var ext = new Extent();
            var slope = new float[Count];  //slope of connecting line's radial vecotor
            var pref = new int[Count];	   //ordering preference (for parallel lines)

            ext.Point1 = Node.Core.Center;
            for (int i = 0; i < Count; i++)
            {
                //	ordering preferance ensures parallel lines don't cross
                pref[i] = i;

                ext.Point2 = Bends[i];

                //	find the line slope and radial quadrant
                var dt = ext.Delta;
                var qd = dt.Quad;
                Quad[i] = qd;
                NQuad[qd] += 1;
                slope[i] = dt.Slope;

                //	count the number of connections in each radial section
                if (Quad[i] == 1)
                {
                    if (slope[i] > 1)
                    { Sect[i] = 2; NSect[2] += 1; }
                    else
                    { Sect[i] = 1; NSect[1] += 1; }
                }
                else if (Quad[i] == 2)
                {
                    if (slope[i] < -1)
                    { Sect[i] = 3; NSect[3] += 1; }
                    else
                    { Sect[i] = 4; NSect[4] += 1; }
                }
                else if (Quad[i] == 3)
                {
                    if (slope[i] > 1)
                    { Sect[i] = 6; NSect[6] += 1; }
                    else
                    { Sect[i] = 5; NSect[5] += 1; }
                }
                else
                {
                    if (slope[i] < -1)
                    { Sect[i] = 7; NSect[7] += 1; }
                    else
                    { Sect[i] = 8; NSect[8] += 1; }
                }
            }

            //	Reorder the connections based on the ordering preferece and on
            //	the radial direction to its destination line end
            //	quad[] identifies the radial quadrant (1,2,3,4 clockwise from horz-right)
            //	slope[] is the radial direction within that quadrant
            for (int i = 0; i < Count; i++)
            {
                for (int j = i + 1; j < Count; j++)
                {
                    bool swap = false;
                    switch (Quad[i])
                    {
                        case 1:
                            if ((Other[j] == Other[i]) && (pref[j] > pref[i]) && (slope[j] == slope[i])) { swap = true; }
                            else if ((Quad[j] == 1) && (slope[j] < slope[i])) { swap = true; }
                            break;
                        case 2:
                            if (Quad[j] < 2) { swap = true; }
                            else if ((Other[j] == Other[i]) && (pref[j] < pref[i]) && (slope[j] == slope[i])) { swap = true; }
                            else if ((Quad[j] == 2) && (slope[j] < slope[i])) { swap = true; }
                            break;
                        case 3:
                            if (Quad[j] < 3) { swap = true; }
                            else if ((Other[j] == Other[i]) && (pref[j] < pref[i]) && (slope[j] == slope[i])) { swap = true; }
                            else if ((Quad[j] == 3) && (slope[j] < slope[i])) { swap = true; }
                            break;
                        case 4:
                            if (Quad[j] < 4) { swap = true; }
                            else if ((Other[j] == Other[i]) && (pref[j] > pref[i]) && (slope[j] == slope[i])) { swap = true; }
                            else if ((Quad[j] == 4) && (slope[j] < slope[i])) { swap = true; }
                            break;
                    }
                    if (swap)
                    {
                        var td = slope[i]; slope[i] = slope[j]; slope[j] = td;
                        var nd = Other[i]; Other[i] = Other[j]; Other[j] = nd;
                        var ti = pref[i]; pref[i] = pref[j]; pref[j] = ti;
                        var ln = Lines[i]; Lines[i] = Lines[j]; Lines[j] = ln;
                        ti = Quad[i]; Quad[i] = Quad[j]; Quad[j] = ti;
                        ti = Sect[i]; Sect[i] = Sect[j]; Sect[j] = ti;
                    }
                }
            }

            //  Computed the line index ranges for each section
            int h = 0, k = 0, N = 10;
            for (int i = 0; i < Count; i++)
            {
                if (k == Sect[i]) continue;
                k = Sect[i];
                for (; h <= k; h++) { SectLine[h] = i; }
            }
            for (; h < N; h++) { SectLine[h] = Count; }

            //  Initialize the edge connect rotator array
            for (int i = 0; i < Count; i++)
            {
                Conn[i] = new EdgeRotator(Lines[i].GetConnect(Node));
            }
        }
    }
}
