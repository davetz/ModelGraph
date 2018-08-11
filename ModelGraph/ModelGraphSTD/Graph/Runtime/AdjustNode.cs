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
            if (!Node_Edges.TryGetValue(node, out List<Edge> edges)) return;

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
            //node.GetValues(out x, out y, out w, out h);
            (int x, int y, int w, int h) = node.Values();

            //  Assign line end terminal points  
            //	based on the just determined order of connections
            //	and on the line end and node termination styles
            int NI, EI, WI, SI, NC, SC, EC, WC, VC, HC;

            var East = Side.East;
            var West = Side.West;
            var North = Side.North;
            var South = Side.South;

            if (node.Orientation == Orientation.Vertical)
            {
                EI = 0;
                WI = 0;
                EC = nquad[1] + nquad[4]; //the right side connection count
                WC = nquad[2] + nquad[3]; //the left side  connection count
                VC = (EC > WC) ? EC : WC; //determines stretch bar length

                node.SetSize(w, (VC * spL) / 2);

                for (int i = 0; i < count; i++)
                {
                    if (quad[i] == 4) lines[i].SetFace(node, East, EI++, EC);
                }
                for (int i = 0; i < count; i++)
                {
                    if (quad[i] == 1) lines[i].SetFace(node, East, EI++, EC);
                }
                for (int i = count - 1; i >= 0; i--)
                {
                    if ((quad[i] == 2) || (quad[i] == 2)) lines[i].SetFace(node, West, WI++, WC);
                }
            }
            else if (node.Orientation == Orientation.Horizontal)
            {
                SI = 0;
                NI = 0;
                SC = nquad[1] + nquad[2];
                NC = nquad[3] + nquad[4];
                HC = (SC > NC) ? SC : NC;

                node.SetSize((HC * spL) / 2, h);

                for (int i = 0; i < count; i++)
                {
                    if ((quad[i] == 3) || (quad[i] == 3)) lines[i].SetFace(node, North, NI++, NC);
                }
                for (int i = count - 1; i >= 0; i--)
                {
                    if ((quad[i] == 3) || (quad[i] == 4)) lines[i].SetFace(node, South, SI++, SC);
                }
            }
            else if (node.Orientation == Orientation.Central)
            {
                SI = 0;
                EI = 0;
                WI = 0;               
                NI = 0;
                SC = nsect[2] + nsect[3];
                EC = nsect[4] + nsect[5];
                NC = nsect[6] + nsect[7];
                WC = nsect[8] + nsect[1];

                VC = (EC > WC) ? EC : WC; 
                HC = (SC > NC) ? SC : NC;


                node.SetSize((HC * spL) / 2, (VC * spL) / 2);

                for (int i = 0; i < count; i++)
                {
                    if (sect[i] == 8) lines[i].SetFace(node, East, EI++, EC);
                }
                for (int i = 0; i < count; i++)
                {
                    if (sect[i] == 1) lines[i].SetFace(node, East, EI++, EC);
                    if (sect[i] == 2 || sect[i] == 3) lines[i].SetFace(node, South, SI++, SC);
                }
                for (int i = count - 1; i >= 0; i--)
                {
                    if (sect[i] == 7 || sect[i] == 6)  lines[i].SetFace(node, North, NI++, NC);
                    if (sect[i] == 5 || sect[i] == 4) lines[i].SetFace(node, West, WI++, WC);
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
            var open = order.Opens;
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
