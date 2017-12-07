using System;
using System.Linq;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public partial class Graph
    {
        private void AdjustAutoNode(Node node, int spL)
        {
            List<Edge> edges;
            if (!Node_Edges.TryGetValue(node, out edges)) return;

            var order = new LineOrder(node, edges);

            //var node = order.Node;
            var count = order.Count;
            var open = order.Opens;
            var lines = order.Lines;
            var nodes = order.Other;
            var quad = order.Quad;
            var sect = order.Sect;
            var nquad = order.NQuad;
            var nsect = order.NSect;

            //int x, y, w, h;
            //node.Core.GetValues(out x, out y, out w, out h);
            (int x, int y, int w, int h) = node.Core.Values();

            //  Assign line end terminal points  
            //	based on the just determined order of connections
            //	and on the line end and node termination styles
            int Idx, Idy, Pdx, Pdy, Ndx, Ndy, Pcnt, Ncnt, Tcnt;

            var East = Side.East;
            var West = Side.West;
            var North = Side.North;
            var South = Side.South;

            if (node.Core.IsVertical)
            {
                Idy = spL;
                Pdy = 0;
                Ndy = 0;
                Pcnt = nquad[1] + nquad[4]; //the right side connection count
                Ncnt = nquad[2] + nquad[3]; //the left side  connection count
                Tcnt = (Pcnt > Ncnt) ? (Pcnt) : (Ncnt); //determines stretch bar length

                node.Core.SetSize(w, (Tcnt * Idy) / 2);

                for (int i = 0; i < count; i++)
                {
                    if (quad[i] != 4) continue;
                    lines[i].SetFace(node, East, Pdy, Pcnt);
                    Pdy += 1;
                }
                for (int i = 0; i < count; i++)
                {
                    if (quad[i] != 1) continue;
                    lines[i].SetFace(node, East, Pdy, Pcnt);
                    Pdy += 1;
                }
                for (int i = count - 1; i >= 0; i--)
                {
                    if ((quad[i] == 1) || (quad[i] == 4)) continue;
                    lines[i].SetFace(node, West, Ndy, Ncnt);
                    Ndy += 1;
                }
            }
            else if (node.Core.IsHorizontal)
            {
                Pdx = 0;
                Ndx = 0;
                Idx = spL;
                Pcnt = nquad[1] + nquad[2];
                Ncnt = nquad[3] + nquad[4];
                Tcnt = (Pcnt > Ncnt) ? (Pcnt) : (Ncnt);

                node.Core.SetSize((Tcnt * Idx) / 2, h);

                for (int i = 0; i < count; i++)
                {
                    if ((quad[i] == 1) || (quad[i] == 2)) continue;
                    lines[i].SetFace(node, North, Ndx, Ncnt);
                    Ndx += 1;
                }
                for (int i = count - 1; i >= 0; i--)
                {
                    if ((quad[i] == 3) || (quad[i] == 4)) continue;
                    lines[i].SetFace(node, South, Pdx, Pcnt);
                    Pdx += 1;
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    switch (sect[i])
                    {
                        case 1:
                        case 8:
                            lines[i].SetFace(node, East); break;
                        case 2:
                        case 3:
                            lines[i].SetFace(node, South); break;
                        case 4:
                        case 5:
                            lines[i].SetFace(node, West); break;
                        case 6:
                        case 7:
                            lines[i].SetFace(node, North); break;
                    }
                }
            }
        }

        private void AdjustFixedNode(Node node)
        {
            List<Edge> edges;
            if (!Node_Edges.TryGetValue(node, out edges)) return;

            var order = new LineOrder(node, edges);

            var count = order.Count;
            var open = order.Opens;
            var lines = order.Lines;
            var nodes = order.Other;
            var bends = order.Bends;
            var quad = order.Quad;
            var sect = order.Sect;
            var nquad = order.NQuad;
            var nsect = order.NSect;

            (int x, int y, int w, int h) = node.Core.Values();

            var xL = x - w;
            var xR = x + w;
            var yT = y - h;
            var yB = y + h;

            for (int i = 0; i < count; i++)
            {
                var x2 = bends[i].X;
                var y2 = bends[i].Y;

                if (x2 < xL)
                {
                    if (y2 < yT)
                        lines[i].SetFace(node, Side.North);
                    else if (y2 > yB)
                        lines[i].SetFace(node, Side.South);
                    else
                        lines[i].SetFace(node, Side.West);

                }
                else if (x2 > xR)
                {
                    if (y2 < yT)
                        lines[i].SetFace(node, Side.North);
                    else if (y2 > yB)
                        lines[i].SetFace(node, Side.South);
                    else
                        lines[i].SetFace(node, Side.East);

                }
                else
                {
                    if (y2 < y)
                        lines[i].SetFace(node, Side.North);
                    else
                        lines[i].SetFace(node, Side.South);
                }
            }
        }
    }
}
