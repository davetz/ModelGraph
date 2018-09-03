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
            var (count, nquad, nsect, sectEdge, E) = Layout.FarNodeParms(node);
            if (count == 0) return;

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
                    if (E[i].quad == 4) E[i].edge.SetFace(node, Side.East, iEast++, nEast);
                }
                for (int i = 0; i < count; i++)
                {
                    if (E[i].quad == 1) E[i].edge.SetFace(node, Side.East, iEast++, nEast);
                    else if (E[i].quad == 2 || E[i].quad == 3) E[i].edge.SetFace(node, Side.West, iWest++, nWest);
                }
            }
            else if (node.Orient == Orient.Horizontal)
            {
                var nSouth = nquad[1] + nquad[2];
                var nNorth = nquad[3] + nquad[4];
                var iSouth = 0;
                var iNorth = 0;

                var nHorz = (nSouth > nNorth) ? nSouth : nNorth;
                var width = (nHorz * spacing) / 2;
                node.SetSize(width, barSize);

                for (int i = 0; i < count; i++)
                {
                    if ((E[i].quad == 1) || (E[i].quad == 2)) E[i].edge.SetFace(node, Side.South, iSouth++, nSouth);
                    else if ((E[i].quad == 3) || (E[i].quad == 4)) E[i].edge.SetFace(node, Side.North, iNorth++, nNorth);
                }
            }
            else if (node.Orient == Orient.Central)
            {
                var nSouth = nsect[2] + nsect[3];
                var nWest = nsect[4] + nsect[5];
                var nNorth = nsect[6] + nsect[7];
                var nEast = nsect[8] + nsect[1];

                var iSouth = 0;
                var iWest = 0;
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
                    if (E[i].sect == 8) E[i].edge.SetFace(node, Side.East, iEast++, nEast);
                }
                for (int i = 0; i < count; i++)
                {
                    if (E[i].sect == 1) E[i].edge.SetFace(node, Side.East, iEast++, nEast);
                    else if (E[i].sect == 2 || E[i].sect == 3) E[i].edge.SetFace(node, Side.South, iSouth++, nSouth);
                    else if (E[i].sect == 4 || E[i].sect == 5) E[i].edge.SetFace(node, Side.West, iWest++, nWest);
                    else if (E[i].sect == 6 || E[i].sect == 7) E[i].edge.SetFace(node, Side.North, iNorth++, nNorth);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (E[i].sect == 8 || E[i].sect == 1) E[i].edge.SetFace(node, Side.East);
                    else if (E[i].sect == 2 || E[i].sect == 3) E[i].edge.SetFace(node, Side.South);
                    else if (E[i].sect == 4 || E[i].sect == 5) E[i].edge.SetFace(node, Side.West);
                    else if (E[i].sect == 6 || E[i].sect == 7) E[i].edge.SetFace(node, Side.North);
                }
            }
        }

        private void AdjustFixedNode(Node node)
        {
            var (count, nquad, nsect, sectEdge, E) = Layout.FarNodeParms(node);
            if (count == 0) return;

            (int x, int y, int w, int h) = node.Values();

            var xL = x - w;
            var xR = x + w;
            var yT = y - h;
            var yB = y + h;

            for (int i = 0; i < count; i++)
            {
                var (x2, y2) = E[i].bend;

                if (x2 < xL)
                {
                    if (y2 < yT)
                        E[i].edge.SetFace(node, Side.North);
                    else if (y2 > yB)
                        E[i].edge.SetFace(node, Side.South);
                    else
                        E[i].edge.SetFace(node, Side.West);

                }
                else if (x2 > xR)
                {
                    if (y2 < yT)
                        E[i].edge.SetFace(node, Side.North);
                    else if (y2 > yB)
                        E[i].edge.SetFace(node, Side.South);
                    else
                        E[i].edge.SetFace(node, Side.East);

                }
                else
                {
                    if (y2 < y)
                        E[i].edge.SetFace(node, Side.North);
                    else
                        E[i].edge.SetFace(node, Side.South);
                }
            }
        }
    }
}
