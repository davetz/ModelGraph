using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ModelGraphSTD
{
    public class Graphic
    {
        public static int NSF = 8;                  // number of symbol faces
        public static int NFR = 16;                 // number of FlipRotate states
        public (float X, float Y) Center;

        public (byte A, byte R, byte G, byte B, byte W, byte SC, byte EC, byte DC, byte DS)[] Styles;

        // NFR version of the symbol's faces and lines, one for each FlipRotate state
        public Contact[][] Contacts;
        public (float X1, float Y1, float X2, float Y2)[][] Faces;
        public (float DX, float DY)[][][] Points;
        public bool IsInvalid;
        public bool IsValid => !IsInvalid;

        internal Graphic(SymbolX symbol)
        {
            Initialize(symbol.Data);
        }

        #region Initialize  ===================================================
        private void Initialize(byte[] data)
        {
            Contacts = new Contact[NFR][];

            Faces = new (float X1, float Y1, float X2, float Y2)[NFR][];

            Points = new (float DX, float DY)[NFR][][];

            var I = ReadHeader(data);
            if (IsValid)
            {
                var LC = LineCount(I, data);

                Styles = new (byte A, byte R, byte G, byte B, byte W, byte SC, byte EC, byte DC, byte DS)[LC];
                Points[0][0] = new (float DX, float DY)[LC];

                for (int j = 0; j < LC; j++) { I = ReadLine(I, j, data); }

                CreateRoations();
            }
        }
        #endregion

        #region CreateRotations  ==============================================
        private void CreateRoations()
        {
            var vc = new Vector2(Center.X, Center.Y);
            var rs = new Matrix3x2[NFR];
            rs[1] = XYVector.Rotation45(vc);
            rs[2] = XYVector.Rotation90(vc);
            rs[3] = XYVector.Rotation135(vc);
            rs[4] = XYVector.Rotation180(vc);
            rs[5] = XYVector.Rotation225(vc);
            rs[6] = XYVector.Rotation270(vc);
            rs[7] = XYVector.Rotation315(vc);
            rs[8] = XYVector.FlipVertical(vc);
            rs[9] = XYVector.FlipRotation45(vc);
            rs[10] = XYVector.FlipRotation90(vc);
            rs[11] = XYVector.FlipRotation135(vc);
            rs[12] = XYVector.FlipRotation180(vc);
            rs[13] = XYVector.FlipRotation225(vc);
            rs[14] = XYVector.FlipRotation270(vc);
            rs[15] = XYVector.FlipRotation315(vc);

            for (int i = 1; i < NFR; i++)
            {
                Contacts[i] = new Contact[NSF];
                for (int j = 0; j < NSF; j++)
                {
                    Contacts[i][j] = Contacts[i - 1][(j + 1) % NSF];
                }

                var ru = rs[i];
                for (int j = 0; j < LC; j++)
                {
                    var PC = Points[0][j].Length;
                    for (int k = 0; k < PC; k++)
                    {
                        var v = new Vector2(Points[0][j][k].DX, Points[0][j][k].DY);
                        var u = Vector2.Transform(v, ru);
                        Points[i][j][k] = (u.X, u.Y);
                    }
                }
            }

        }
        #endregion

        #region ReadHeader  ===================================================
        const int DHL = 37; // data header byte count + 1; 
        private int ReadHeader(byte[] data)
        {
            var N = data.Length;
            if (N < DHL) goto Abort;

            var i = 0;
            var vc = new Vector2(data[i++], data[i++]);
            var r90 = XYVector.Rotation90(vc);
            Center = (vc.X, vc.Y);

            Contacts[0] = new Contact[NSF];
            for (int j = 0; j < NSF; j++)
            {
                Contacts[0][j] = (Contact)data[i++];
            }

            Faces[0] = new (float X1, float Y1, float X2, float Y2)[NSF];
            for (int j = 0; j < NSF; j++)
            {
                var v1 = new Vector2((sbyte)data[i++], (sbyte)data[i++]);
                var vu = v1 / v1.Length();              // unit vector 
                var v2 = Vector2.Transform(vu, r90);    // rotated 90 degrees
                Faces[0][j] = (v1.X, v2.Y,  v2.X, v2.Y);
            }

            return i;

            Abort: IsInvalid = true; return 0;
        }
        #endregion

        #region LineCount  ====================================================
        const int DLC = 14;             // minimum line data byte count
        const int IPC = 9;              // advance into the line data to position of the point count
        private int LineCount(int I, byte[] data)
        {
            var n = 0;
            var i = I;
            var N = data.Length;
            while (i + DLC <= N)
            {
                var j = i + IPC;
                var pc = data[j];
                i = j + 2 * pc;
                if (i < N ) n++;               
            }
            return n;
        }
        #endregion

        #region ReadLine  =====================================================
        private int ReadLine(int I, int J, byte[] data)
        {
            var i = I;
            var A = data[i++];  // 0
            var R = data[i++];  // 1
            var G = data[i++];  // 2
            var B = data[i++];  // 3
            var W = data[i++];  // 4
            var SC = data[i++]; // 5
            var EC = data[i++]; // 6
            var DC = data[i++]; // 7 
            var DS = data[i++]; // 8

            Styles[J] = (A, R, G, B, W, SC, EC, DC, DS);

            var PC = data[i++]; // 9

            Points[0][J] = new (float, float)[PC];

            for (int k = 0; k < PC; k++)
            {
                Points[0][J][k]= ((sbyte)data[i++], (sbyte)data[i++]);
            }
            return i;
        }
        #endregion
    }
}
