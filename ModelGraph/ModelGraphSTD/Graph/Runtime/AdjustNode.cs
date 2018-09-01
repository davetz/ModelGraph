using System;
using System.Linq;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public partial class Graph
    {
        private void AdjustAutoNode(Node node)
        {
            var (count, edge, quad, sect, slope, nquad, nsect, _, _) = Layout.FarNodeParms(node);
            if (count == 0) return;
            var last = count - 1;

            var gx = node.Graph.GraphX;
            var spacing = gx.TerminalSpacing;
            var barSize = ((node.BarWidth == BarWidth.Thin) ? gx.ThinBusSize : (node.BarWidth == BarWidth.Wide) ? gx.WideBusSize : gx.ExtraWideBusSize) / 2;          

            var (x, y, w, h) = node.Values();

            //  Assign line end terminal points based  
            //	on the just determined order of connections
            //	and on the line end and node termination styles
            if (node.Orient == Orient.Vertical)
            {
                var nEast = nquad[1] + nquad[4]; //the right side connection count
                var nWest = nquad[2] + nquad[3]; //the left side  connection count
                var iEast = 0;
                var iWest = 0;

                var nVert = (nEast > nWest) ? nEast : nWest; //determine the resizable bar length
                var height = (nVert * spacing) / 2;
                node.SetSize(barSize, height);

                for (int i = 0; i < count; i++)
                {
                    if (quad[i] == 4) edge[i].SetFace(node, Side.East, iEast++, nEast);
                }
                for (int i = 0; i < count; i++)
                {
                    if (quad[i] == 1) edge[i].SetFace(node, Side.East, iEast++, nEast);
                    else if (quad[i] == 2 || quad[i] == 3) edge[i].SetFace(node, Side.West, iWest++, nWest);
                }
            }
            else if (node.Orient == Orient.Horizontal)
            {
                var nSouth = nquad[1] + nquad[2];
                var nNorth = nquad[3] + nquad[4];
                var iSouth = nSouth - 1;
                var iNorth = 0;

                var nHorz = (nSouth > nNorth) ? nSouth : nNorth;
                var width = (nHorz * spacing) / 2;
                node.SetSize(width, barSize);

                for (int i = 0; i < count; i++)
                {
                    if ((quad[i] == 1) || (quad[i] == 2)) edge[i].SetFace(node, Side.South, iSouth--, nSouth);
                    else if ((quad[i] == 3) || (quad[i] == 4)) edge[i].SetFace(node, Side.North, iNorth++, nNorth);
                }
            }
            else if (node.Orient == Orient.Central)
            {
                var nSouth = nsect[2] + nsect[3];
                var nWest = nsect[4] + nsect[5];
                var nNorth = nsect[6] + nsect[7];
                var nEast = nsect[8] + nsect[1];

                var iSouth = nSouth - 1;
                var iWest = nWest - 1;
                var iNorth = 0;
                var iEast = 0;

                var nVert = (nEast > nWest) ? nEast : nWest;
                var height = (nVert * spacing) / 2;
                if (height < barSize) height = barSize;

                var nHorz = (nSouth > nNorth) ? nSouth : nNorth;
                var width = (nHorz * spacing) / 2;
                if (width < barSize) width = barSize;
                node.SetSize(width, height);

                for (int i = 0; i < count; i++)
                {
                    if (sect[i] == 8) edge[i].SetFace(node, Side.East, iEast++, nEast);
                }
                for (int i = 0; i < count; i++)
                {
                    if (sect[i] == 1) edge[i].SetFace(node, Side.East, iEast++, nEast);
                    else if (sect[i] == 2 || sect[i] == 3) edge[i].SetFace(node, Side.South, iSouth--, nSouth);
                    else if (sect[i] == 4 || sect[i] == 5) edge[i].SetFace(node, Side.West, iWest--, nWest);
                    else if (sect[i] == 6 || sect[i] == 7) edge[i].SetFace(node, Side.North, iNorth++, nNorth);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (sect[i] == 8 || sect[i] == 1) edge[i].SetFace(node, Side.East);
                    else if (sect[i] == 2 || sect[i] == 3) edge[i].SetFace(node, Side.South);
                    else if (sect[i] == 4 || sect[i] == 5) edge[i].SetFace(node, Side.West);
                    else if (sect[i] == 6 || sect[i] == 7) edge[i].SetFace(node, Side.North);
                }
            }
        }

        private void AdjustFixedNode(Node node)
        {
            if (!Node_Edges.TryGetValue(node, out List<Edge> edges)) return;

            var order = new LineOrder(node, edges);

            var count = order.Count;
            var lines = order.Lines;
            var nodes = order.Other;
            var bends = order.Bends;
            var quad = order.Quad;
            var sect = order.Sect;
            var nquad = order.NQuad;
            var nsect = order.NSect;

            (int x, int y, int w, int h) = node.Values();

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
