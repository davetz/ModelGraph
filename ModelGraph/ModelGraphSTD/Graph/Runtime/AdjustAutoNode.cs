
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
            var tmSpc = gx.TerminalSpacing / 2;
            var tmLen = gx.TerminalLength;
            var barSize = ((node.BarWidth == BarWidth.Thin) ? gx.ThinBusSize : (node.BarWidth == BarWidth.Wide) ? gx.WideBusSize : gx.ExtraBusSize) / 2;          

            var (x, y, w, h) = node.Values();

            var d1East = (short)w;
            var d1West = (short)-w;
            var d1South = (short)h;
            var d1North = (short)-h;

            var d2East = (short)(tmLen + w);
            var d2West = (short)-d2East;
            var d2South = (short)(tmLen + h);
            var d2North = (short)-d2South;

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
                d1East = (short)barSize;
                d1West = (short)-d1East;
                d2East = (short)(d1East + tmLen);
                d2West = (short)-d2East;

                for (int i = 0; i < count; i++)
                {
                    if (E[i].quad == Quad.Q4) SetEast(i);
                }
                for (int i = 0; i < count; i++)
                {
                    if (E[i].quad == Quad.Q1) SetEast(i);
                    else if (E[i].quad == Quad.Q2 || E[i].quad == Quad.Q3) SetWest(i);
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
                d1South = (short)barSize;
                d1North = (short)-d1South;
                d2South = (short)(d1South + tmLen);
                d2North = (short)-d2South;


                for (int i = 0; i < count; i++)
                {
                    if ((E[i].quad == Quad.Q1) || (E[i].quad == Quad.Q2)) SetSouth(i);
                    else if ((E[i].quad == Quad.Q3) || (E[i].quad == Quad.Q4)) SetNorth(i);
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
                d1East = (short)width;
                d1West = (short)-d1East;
                d2East = (short)(d1East + tmLen);
                d2West = (short)-d2East;
                d1South = (short)height;
                d1North = (short)-d1South;
                d2South = (short)(d1South + tmLen);
                d2North = (short)-d2South;

                for (int i = 0; i < count; i++)
                {
                    if (E[i].sect == Sect.S8) SetEast(i);
                }
                for (int i = 0; i < count; i++)
                {
                    if (E[i].sect == Sect.S1) SetEast(i);
                    else if (E[i].sect == Sect.S2 || E[i].sect == Sect.S3) SetSouth(i);
                    else if (E[i].sect == Sect.S4 || E[i].sect == Sect.S5) SetWest(i);
                    else if (E[i].sect == Sect.S6 || E[i].sect == Sect.S7) SetNorth(i);
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
                E[e].edge.SetFace(node, (d1East, ds), (d2East, ds));
            }
            void SetWest(int e)
            {
                var ds = (short)(tmSpc * Layout.Offset(iWest++, nWest));
                E[e].edge.SetFace(node, (d1West, ds), (d2West, ds));
            }
            void SetSouth(int e)
            {
                var ds = (short)(tmSpc * Layout.Offset(iSouth++, nSouth));
                E[e].edge.SetFace(node, (ds, d1South), (ds, d2South));
            }
            void SetNorth(int e)
            {
                var ds = (short)(tmSpc * Layout.Offset(iNorth++, nNorth));
                E[e].edge.SetFace(node, (d1North, ds), (d2North, ds));
            }
        }
    }
}
