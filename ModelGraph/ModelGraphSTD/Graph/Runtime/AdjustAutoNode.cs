
namespace ModelGraphSTD
{/*
 */
    public partial class Graph
    {
        private void AdjustAutoNode(Node node)
        {
            var (count, nquad, nsect, sectEdge, E) = Layout.SortedEdges(node);
            if (count == 0) return;
            var last = count - 1;

            var gx = node.Graph.GraphX;
            var tmSpc = gx.TerminalSpacing / 2;
            var tmLen = gx.TerminalLength;
            var barSize = ((node.BarWidth == BarWidth.Thin) ? gx.ThinBusSize : (node.BarWidth == BarWidth.Wide) ? gx.WideBusSize : gx.ExtraBusSize) / 2;          

            var (x, y, w, h) = node.Values();

            var dx1 = w;
            var dx2 = w + tmLen;
            var dy1 = h;
            var dy2 = h + tmLen;

            int iEast, nEast, iWest, nWest, iSouth, nSouth, iNorth, nNorth;

            //  Assign line end terminal points based  
            //	on the just determined order of connections
            //	and on the line end and node termination styles
            if (node.Aspect == Aspect.Vertical)
            {
                nEast = nquad[1] + nquad[4]; //the right side connection count
                nWest = nquad[2] + nquad[3]; //the left side  connection count
                iEast = 0;
                iWest = 0;

                var nVert = (nEast > nWest) ? nEast : nWest; //determine the resizable bar length
                var height = nVert * tmSpc;

                node.SetSize(barSize, height);
                dx1 = barSize;
                dx2 = dx1 + tmLen;
                dy1 = height;
                dy2 = dy1 + tmLen;

                for (int i = 0; i < count; i++)
                {
                    if (E[i].quad == Quad.Q4) SetEast(i);
                }
                for (int i = 0; i < count; i++)
                {
                    if (E[i].quad == Quad.Q1) SetEast(i);
                }
                for (int i = last; i >= 0; i--)
                {
                    if (E[i].quad == Quad.Q2 || E[i].quad == Quad.Q3) SetWest(i);
                }
            }
            else if (node.Aspect == Aspect.Horizontal)
            {
                nSouth = nquad[1] + nquad[2];
                nNorth = nquad[3] + nquad[4];
                iSouth = 0;
                iNorth = 0;

                var nHorz = (nSouth > nNorth) ? nSouth : nNorth;
                var width = nHorz * tmSpc;

                node.SetSize(width, barSize);
                dx1 = width;
                dx2 = dx1 + tmLen;
                dy1 = barSize;
                dy2 = dy1 + tmLen;

                for (int i = 0; i < count; i++)
                {
                    if ((E[i].quad == Quad.Q3) || (E[i].quad == Quad.Q4)) SetNorth(i);
                }
                for (int i = last; i >= 0; i--)
                {
                    if (E[i].quad == Quad.Q1 || E[i].quad == Quad.Q2) SetSouth(i);
                }
            }
            else if (node.Aspect == Aspect.Central)
            {
                nSouth = nsect[2] + nsect[3];
                nWest = nsect[4] + nsect[5];
                nNorth = nsect[6] + nsect[7];
                nEast = nsect[8] + nsect[1];

                iSouth = 0;
                iWest = 0;
                iNorth = 0;
                iEast = 0;

                var nVert = (nEast > nWest) ? nEast : nWest;
                var height = (nVert * tmSpc);
                if (height < barSize) height = barSize;

                var nHorz = (nSouth > nNorth) ? nSouth : nNorth;
                var width = (nHorz * tmSpc);
                if (width < barSize) width = barSize;

                node.SetSize(width, height);
                dx1 = width;
                dx2 = dx1 + tmLen;
                dy1 = height;
                dy2 = dy1 + tmLen;

                for (int i = 0; i < count; i++)
                {
                    if (E[i].sect == Sect.S6 || E[i].sect == Sect.S7) SetNorth(i);
                    else if (E[i].sect == Sect.S8) SetEast(i);
                }
                for (int i = 0; i < count; i++)
                {
                    if (E[i].sect == Sect.S1) SetEast(i);
                }
                for (int i = last; i >= 0; i--)
                {
                    if (E[i].sect == Sect.S2 || E[i].sect == Sect.S3) SetSouth(i);
                    else if (E[i].sect == Sect.S4 || E[i].sect == Sect.S5) SetWest(i);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (E[i].sect == Sect.S8 || E[i].sect == Sect.S1) E[i].edge.SetFace(node, (0,0));
                    else if (E[i].sect == Sect.S2 || E[i].sect == Sect.S3) E[i].edge.SetFace(node, (0, 0));
                    else if (E[i].sect == Sect.S4 || E[i].sect == Sect.S5) E[i].edge.SetFace(node, (0, 0));
                    else if (E[i].sect == Sect.S6 || E[i].sect == Sect.S7) E[i].edge.SetFace(node, (0, 0));
                }
            }

            void SetEast(int e)
            {
                var ds = (short)(tmSpc * Layout.Offset(iEast++, nEast));
                E[e].edge.SetFace(node, (dx1, ds), (dx2, ds));
            }
            void SetWest(int e)
            {
                var ds = (short)(tmSpc * Layout.Offset(iWest++, nWest));
                E[e].edge.SetFace(node, (-dx1, ds), (-dx2, ds));
            }
            void SetSouth(int e)
            {
                var ds = (short)(tmSpc * Layout.Offset(iSouth++, nSouth));
                E[e].edge.SetFace(node, (ds, dy1), (ds, dy2));
            }
            void SetNorth(int e)
            {
                var ds = (short)(tmSpc * Layout.Offset(iNorth++, nNorth));
                E[e].edge.SetFace(node, (ds, -dy1), (ds, -dy2));
            }
        }
    }
}
