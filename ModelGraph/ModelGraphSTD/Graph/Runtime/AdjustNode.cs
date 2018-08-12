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
            if (!Node_Edges.TryGetValue(node, out List<Edge> edges)) return;

            var gx = node.Graph.GraphX;
            var spacing = gx.TerminalSpacing;
            var barSize = ((node.BarWidth == BarWidth.Thin) ? gx.ThinBusSize : (node.BarWidth == BarWidth.Wide) ? gx.WideBusSize : gx.ExtraWideBusSize) / 2;


            var order = new LineOrder(node, edges);

            //var node = order.Node;
            var count = order.Count;
            var lines = order.Lines;
            var nodes = order.Other;
            var quad = order.Quad;
            var sect = order.Sect;
            var nquad = order.NQuad;
            var nsect = order.NSect;

            //int x, y, w, h;
            //node.GetValues(out x, out y, out w, out h);
            (int x, int y, int w, int h) = node.Values();

            //  Assign line end terminal points  
            //	based on the just determined order of connections
            //	and on the line end and node termination styles
            int iNorth = 0, iEast = 0, iWest = 0, iSouth = 0, nNorth, nSouth, nEast, nWest, nVert, nHorz, Width, Height;

            var East = Side.East;
            var West = Side.West;
            var North = Side.North;
            var South = Side.South;

            if (node.Orient == Orient.Vertical)
            {
                nEast = nquad[1] + nquad[4]; //the right side connection count
                nWest = nquad[2] + nquad[3]; //the left side  connection count

                nVert = (nEast > nWest) ? nEast : nWest; //determines stretch bar length
                Height = (nVert * spacing) / 2;
                node.SetSize(barSize, Height);

                for (int i = 0; i < count; i++)
                {
                    if (quad[i] == 4) lines[i].SetFace(node, East, iEast++, nEast);
                }
                for (int i = 0; i < count; i++)
                {
                    if (quad[i] == 1) lines[i].SetFace(node, East, iEast++, nEast);
                }
                for (int i = count - 1; i >= 0; i--)
                {
                    if ((quad[i] == 3) || (quad[i] == 2)) lines[i].SetFace(node, West, iWest++, nWest);
                }
            }
            else if (node.Orient == Orient.Horizontal)
            {
                nSouth = nquad[1] + nquad[2];
                nNorth = nquad[3] + nquad[4];

                nHorz = (nSouth > nNorth) ? nSouth : nNorth;
                Width = (nHorz * spacing) / 2;
                node.SetSize(Width, barSize);

                for (int i = 0; i < count; i++)
                {
                    if ((quad[i] == 3) || (quad[i] == 4)) lines[i].SetFace(node, North, iNorth++, nNorth);
                }
                for (int i = count - 1; i >= 0; i--)
                {
                    if ((quad[i] == 2) || (quad[i] == 1)) lines[i].SetFace(node, South, iSouth++, nSouth);
                }
            }
            else if (node.Orient == Orient.Central)
            {
                nSouth = nsect[2] + nsect[3];
                nWest = nsect[4] + nsect[5];
                nNorth = nsect[6] + nsect[7];
                nEast = nsect[8] + nsect[1];

                nVert = (nEast > nWest) ? nEast : nWest;
                Height = (nVert * spacing) / 2;
                if (Height < barSize) Height = barSize;

                nHorz = (nSouth > nNorth) ? nSouth : nNorth;
                Width = (nHorz * spacing) / 2;
                if (Width < barSize) Width = barSize;

                node.SetSize(Width, Height);

                for (int i = 0; i < count; i++)
                {
                    if (sect[i] == 8) lines[i].SetFace(node, East, iEast++, nEast);
                }
                for (int i = 0; i < count; i++)
                {
                    if (sect[i] == 1) lines[i].SetFace(node, East, iEast++, nEast);
                    if (sect[i] == 6 || sect[i] == 7) lines[i].SetFace(node, North, iNorth++, nNorth);
                }
                for (int i = count - 1; i >= 0; i--)
                {
                    if (sect[i] == 5 || sect[i] == 4) lines[i].SetFace(node, West, iWest++, nWest);
                    if (sect[i] == 3 || sect[i] == 2) lines[i].SetFace(node, South, iSouth++, nSouth);
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
