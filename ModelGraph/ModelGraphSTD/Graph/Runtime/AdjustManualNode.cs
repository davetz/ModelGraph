
namespace ModelGraphSTD
{
    public partial class Graph
    {
        private void AdjustManualNode(Node node)
        {
            var E = Layout.ConnectedEdges(node);
            var N = (E is null) ? 0 : E.Length;
            if (N == 0) return;

            var gx = node.Graph.GraphX;
            var spSkew = 8;// gx.SurfaceSkew;
            var tpSkew = gx.TerminalSkew;
            var tmLen = gx.TerminalLength;

            var (x, y, w, h) = node.Values();
            var d0 = 2;
            var xE = x + w - d0; // east side
            var xW = x - w + d0; // west side
            var yS = y + h - d0; // south side
            var yN = y - h + d0; // north side

            var dx0 = w - d0;
            var dx1 = w;
            var dx2 = dx1 + spSkew;
            var dx3 = dx2 + tmLen;
            var dx4 = dx3 + spSkew;

            var dy0 = h - d0;
            var dy1 = h;
            var dy2 = dy1 + spSkew;
            var dy3 = dy2 + tmLen;
            var dy4 = dy3 + spSkew;
            /*
             * delta dx1,dy1 determine the surface of the node
             * ================================================
                (-dx4, -dy4)  o             o  (+dx4, -dy4)
                 (-dx3, -dy3)  o           o  (+dx3, -dy3)
                  (-dx2, -dy2)  o         o  (+dx2, -dy2)
                   (-dx1, -dy1)  o-------o  (+dx1, -dy1) <== upper left,right corner of node
                    (-dx0, -dy0) |o     o| (+dx0, -dy0)
                                 |       |
                                 | (x,y) |  <== center of node (x,y)
                                 |       |
                    (-dx0, +dy0) |o     o| (+dx0, +dy0)
                   (-dx1, +dy1)  o-------o  (+dx1, +dy1) <== lower left,right corner of node
                  (-dx2, +dy2)  o         o  (+dx2, +dy2)
                 (-dx3, +dy3)  o           o  (+dx3, +dy3)
                (-dx4, +dy4)  o             o  (+dx4, +dy4)                          
             * ================================================
             */
            for (int i = 0; i < N; i++)
            {
                var (x2, y2) = E[i].bend; // closest edge bend point towards the destination node

                var dx = x2 - x;
                var dy = y2 - y;

                var dxE = (double)(xE - x2);
                var dxW = (double)(xW - x2);
                var dyS = (double)(yS - y2);
                var dyN = (double)(yN - y2);

                if (node.Aspect == Aspect.Vertical)
                {
                    if (x2 > xE)
                    {//========================== on the east side of major axis
                        if (y2 > yS)
                        {//========================== going south
                            if (dyS / dxE < 1)
                                E[i].edge.SetFace(node, (dx1, dy0), (dx2, dy), (dx4, dy), TupleSort.East);      // skewed south east heading east 
                            else
                                E[i].edge.SetFace(node, (0, dy1), (0, dy3), TupleSort.South);    // off south end heading south
                        }
                        else if (y2 < yN)
                        {//========================== going north
                            if (dyN / dxE > -1)
                                E[i].edge.SetFace(node, (dx1, -dy0), (dx2, dy), (dx4, dy), TupleSort.East);   // skewed north east heading east 
                            else
                                E[i].edge.SetFace(node, (0, -dy1), (0, -dy3), TupleSort.North);    // off north end heading north
                        }
                        else
                            E[i].edge.SetFace(node, (dx1, dy), (dx3, dy), TupleSort.East);        // off east side heading straight east
                    }
                    else if (x2 < xW)
                    {//========================== on the west side of major axis
                        if (y2 > yS)
                        {//========================== going south
                            if (dyS / dxW > -1)
                                E[i].edge.SetFace(node, (-dx1, dy0), (-dx2, dy), (-dx4, dy), TupleSort.West);      // skewed south west heading west 
                            else
                                E[i].edge.SetFace(node, (0, dy1), (0, dy3), TupleSort.South);    // off south end heading south
                        }
                        else if (y2 < yN)
                        {//========================== going north
                            if (dyN / dxW < 1)
                                E[i].edge.SetFace(node, (-dx1, -dy0), (-dx2, dy), (-dx4, dy), TupleSort.West);   // skewed north west heading west 
                            else
                                E[i].edge.SetFace(node, (0, -dy1), (0, -dy3), TupleSort.North);    // off north end heading north
                        }
                        else
                            E[i].edge.SetFace(node, (-dx1, dy), (-dx3, dy), TupleSort.West);        // off west side heading straight west
                    }
                    else
                    {//========================== directly on the major axis
                        if (y2 > yS)
                            E[i].edge.SetFace(node, (0, dy1), (0, dy3), TupleSort.South);    // off south end heading south
                        else
                            E[i].edge.SetFace(node, (0, -dy1), (0, -dy3), TupleSort.North);  // off north end heading north
                    }
                }
                else if (node.Aspect == Aspect.Horizontal)
                {
                    if (y2 > yS)
                    {//========================== on the south side of major axis
                        if (x2 > xE)
                        {//========================== heading east
                            if (dyS / dxE < 1)
                                E[i].edge.SetFace(node, (dx1, 0), (dx3, 0), TupleSort.East);    // off east end heading east
                            else
                                E[i].edge.SetFace(node, (dx0, dy1), (dx, dy2), (dx, dy4), TupleSort.South);      // skewed south east heading south 
                        }
                        else if (x2 < xW)
                        {//========================== heading west
                            if (dyN / dxE > -1)
                                E[i].edge.SetFace(node, (-dx1, 0), (-dx3, 0), TupleSort.West);  // off west end heading west
                            else
                                E[i].edge.SetFace(node, (-dx0, dy1), (dx, dy2), (dx, dy4), TupleSort.South);   // skewed south west heading south 
                        }
                        else
                            E[i].edge.SetFace(node, (dx, dy1), (dx, dy3), TupleSort.South);      // off south side heading straight south
                    }
                    else if (y2 < yN)
                    {//========================== on the north side of major axis
                        if (x2 > xE)
                        {//========================== heading east
                            if (dyN / dxE < -1)
                                E[i].edge.SetFace(node, (dx0, -dy1), (dx, -dy2), (dx, -dy4), TupleSort.North);      // skewed north east heading north 
                            else
                                E[i].edge.SetFace(node, (dx1, 0), (dx3, 0), TupleSort.East);    // off east end heading east
                        }
                        else if (x2 < xW)
                        {//========================== heading west
                            if (dyN / dxW < 1)
                                E[i].edge.SetFace(node, (-dx1, 0), (-dx3, 0), TupleSort.West);  // off west end heading west
                            else
                                E[i].edge.SetFace(node, (-dx0, -dy1), (dx, -dy2), (dx, -dy4), TupleSort.North);   // skewed north west heading north 
                        }
                        else
                            E[i].edge.SetFace(node, (dx, -dy1), (dx, -dy3), TupleSort.North);      // off north side heading straight north
                    }
                    else
                    {//========================== directly on the major axis
                        if (x2 > xE)
                            E[i].edge.SetFace(node, (dx1, 0), (dx3, 0), TupleSort.East);    // off east end heading east
                        else
                            E[i].edge.SetFace(node, (-dx1, 0), (-dx3, 0), TupleSort.West);  // off west end heading west
                    }
                }
                else if (node.Aspect == Aspect.Central)
                {
                    if (x2 > xE)
                    {//========================== on the east side of major axis
                        if (y2 > yS)
                        {//========================== going south
                            if (dyS / dxE < 1)
                                E[i].edge.SetFace(node, (dx1, dy0), (dx2, dy), (dx4, dy), TupleSort.East);      // skewed south east heading east 
                            else
                                E[i].edge.SetFace(node, (dx0, dy1), (dx, dy2), (dx, dy4), TupleSort.South);      // skewed south east heading south 
                        }
                        else if (y2 < yN)
                        {//========================== going north
                            if (dyN / dxE > -1)
                                E[i].edge.SetFace(node, (dx1, -dy0), (dx2, dy), (dx4, dy), TupleSort.East);   // skewed north east heading east 
                            else
                                E[i].edge.SetFace(node, (dx0, -dy1), (dx, -dy2), (dx, -dy4), TupleSort.North);      // skewed north east heading north 
                        }
                        else
                            E[i].edge.SetFace(node, (dx1, dy), (dx3, dy), TupleSort.East);        // off east side heading straight east
                    }
                    else if (x2 < xW)
                    {//========================== on the west side of major axis
                        if (y2 > yS)
                        {//========================== going south
                            if (dyS / dxW < -1)
                                E[i].edge.SetFace(node, (-dx0, dy1), (dx, dy2), (dx, dy4), TupleSort.South);   // skewed south west heading south 
                            else
                                E[i].edge.SetFace(node, (-dx1, dy0), (-dx2, dy), (-dx4, dy), TupleSort.West);      // skewed south west heading west 
                        }
                        else if (y2 < yN)
                        {//========================== going north
                            if (dyN / dxW < 1)
                                E[i].edge.SetFace(node, (-dx1, -dy0), (-dx2, dy), (-dx4, dy), TupleSort.West);   // skewed north west heading west 
                            else
                                E[i].edge.SetFace(node, (-dx0, -dy1), (dx, -dy2), (dx, -dy4), TupleSort.North);   // skewed north west heading north 
                        }
                        else
                            E[i].edge.SetFace(node, (-dx1, dy), (-dx3, dy), TupleSort.West);        // off west side heading straight west
                    }
                    else
                    {//========================== directly on the major axis
                        if (y2 > yS)
                            E[i].edge.SetFace(node, (dx, dy1), (dx, dy3), TupleSort.South);      // off south side heading straight south
                        else
                            E[i].edge.SetFace(node, (dx, -dy1), (dx, -dy3), TupleSort.North);      // off north side heading straight north
                    }
                }
                else
                {
                    E[i].edge.SetFace(node, (0,0), TupleSort.Any);
                }
            }
        }
    }
}
