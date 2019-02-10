using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ModelGraphSTD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Color = Windows.UI.Color;


namespace ModelGraph.Controls
{
    public sealed partial class SymbolEditControl
    {

        #region DrawingStyles  ================================================
        public static List<T> GetEnumAsList<T>() { return Enum.GetValues(typeof(T)).Cast<T>().ToList(); }
        public List<CanvasDashStyle> DashStyleList { get { return GetEnumAsList<CanvasDashStyle>(); } }
        public List<CanvasCapStyle> CapStyleList { get { return GetEnumAsList<CanvasCapStyle>(); } }
        public List<CanvasLineJoin> LineJoinList { get { return GetEnumAsList<CanvasLineJoin>(); } }
        public List<Fill_Stroke> FillStrokeList { get { return GetEnumAsList<Fill_Stroke>(); } }
        public List<Edit_Contact> EditContactList { get { return GetEnumAsList<Edit_Contact>(); } }
        public List<Contact> ContactList { get { return GetEnumAsList<Contact>(); } }
        #endregion

        #region PickerCanvas_Draw  ============================================
        private void PickerCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            var W = (float)sender.Width;
            var HW = W / 2;

            var strokeWidth = 3;

            var n = PickerShapes.Count;
            for (int i = 0; i < n; i++)
            {
                var a = i * W;
                var b = (i + 1) * W;
                var center = new Vector2(HW, a + HW);
                var shape = PickerShapes[i];

                if (shape == PickerShape) Shape.HighLight(ds, W, i);
                shape.Draw(sender, ds, HW, center, strokeWidth);
                if (i == 3 || i == 6)
                    ds.DrawLine(0, b, b, b, Colors.LightGray, 1);
            }
        }
        #endregion

        #region SymbolCanvas_Draw  ============================================
        private void SymbolCanvas_Draw(CanvasControl canvas, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            var W = (float)canvas.Width;
            var HW = W / 2;
            var n = SymbolShapes.Count;

            var center = new Vector2(HW, HW);
            for (int i = 0; i < n; i++)
            {
                var shape = SymbolShapes[i];
                var strokeWidth = shape.StrokeWidth;

                shape.Draw(canvas, ds, HW, center, strokeWidth);
            }
        }
        #endregion

        #region SelectorCanvas_Draw  ==========================================
        private void SelectorCanvas_Draw(CanvasControl canvas, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            var W = (float)canvas.Width;
            var HW = W / 2;
            var n = SymbolShapes.Count;

            var m = SelectorCanvas.MinHeight;
            var v = (n + 1) * W;
            SelectorCanvas.Height = (v > m) ? v : m;

            var center_0 = new Vector2(HW, HW);
            for (int i = 0; i < n; i++)
            {
                var a = i * W;
                var b = (i + 1) * W;
                var center = new Vector2(HW, a + HW);
                var shape = SymbolShapes[i];
                var strokeWidth = shape.StrokeWidth;
                if (SelectedShapes.Contains(shape)) Shape.HighLight(ds, W, i);

                shape.Draw(canvas, ds, HW, center, strokeWidth);
                ds.DrawLine(0, b, b, b, Colors.LightGray, 1);
            }
        }
        #endregion

        #region EditorCanvas_Draw  ============================================
        private void EditorCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            var scale = EditSize / 2;
            DrawEditorBackgroundGrid(ds);

            if (EditContact == Edit_Contact.Contacts)
            {
                foreach (var shape in SymbolShapes)
                {
                    var strokeWidth = shape.StrokeWidth * 5;
                    shape.Draw(EditorCanvas, ds, scale, Center, strokeWidth, Shape.Coloring.Light);
                }

                DrawTargetContacts(ds);
            }
            else
            {
                if (SelectedShapes.Count > 0)
                {
                    foreach (var shape in SymbolShapes)
                    {
                        var coloring = SelectedShapes.Contains(shape) ? Shape.Coloring.Light : Shape.Coloring.Gray;
                        var strokeWidth = shape.StrokeWidth * 5;
                        shape.Draw(EditorCanvas, ds, scale, Center, strokeWidth, coloring);
                    }

                    _polylineTarget = SelectedShapes.First() as Polyline;
                    _targetPoints.Clear();
                    Shape.DrawTargets(SelectedShapes, _targetPoints, ds, scale, Center);
                }
                else
                {
                    foreach (var shape in SymbolShapes)
                    {
                        var strokeWidth = shape.StrokeWidth * 5;
                        shape.Draw(EditorCanvas, ds, scale, Center, strokeWidth, Shape.Coloring.Normal);
                    }
                }
                SymbolCanvas.Invalidate();
                SelectorCanvas.Invalidate();
            }
        }
        private Polyline _polylineTarget;
        private List<Vector2> _targetPoints = new List<Vector2>();

        #endregion

        #region DrawEditorBackgroundGrid  =====================================
        private const int _workAxis = (int)(EditSize / 4);
        private const int _workGrid = (int)(EditSize / 16);
        private void DrawEditorBackgroundGrid(CanvasDrawingSession ds)
        {
            var color1 = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
            var color2 = Color.FromArgb(0xff, 0xff, 0xff, 0x80);
            var color3 = Color.FromArgb(0x80, 0xff, 0xff, 0x00);
            var color4 = Color.FromArgb(0x40, 0xff, 0xff, 0xff);

            var a = EditMargin;   //north or west axis line
            var b = a + EditSize; //south or east axis line
            var c = EDITCenter;   //center axis line
            var r = EditSize / 2;

            var d = r * Math.Sin(Math.PI / 8);
            var e = (float)(c - d);
            var f = (float)(c + d);

            for (int i = 0; i <= EditSize; i += _workGrid)
            {
                var z = a + i;
                ds.DrawLine(z, a, z, b, color3);
                ds.DrawLine(a, z, b, z, color3);
            }
            ds.DrawLine(2, 2, 10, 14, color1, 3);
            ds.DrawLine(2, 2, 14, 10, color1, 3);
            ds.DrawLine(0, 0, b, b, color1);
            ds.DrawLine(0, 0, b, b, color1);
            ds.DrawLine(a, b, b, a, color1);

            ds.DrawLine(a, e, b, f, color4);
            ds.DrawLine(e, a, f, b, color4);

            ds.DrawLine(a, f, b, e, color4);
            ds.DrawLine(f, a, e, b, color4);

            ds.DrawCircle(c, c, r, color2);
            ds.DrawCircle(c, c, r / 2, color4);

            for (int i = 0; i <= EditSize; i += _workAxis)
            {
                var z = a + i;
                ds.DrawLine(z, a, z, b, color1);
                ds.DrawLine(a, z, b, z, color1);
            }
            var xC = c - 6;
            var yN = -2;
            var yS = b - 3;
            var xe = b - _workAxis - 10;
            var xw = a + _workAxis - 10;
            var xec = b - 10;
            var xwc = a - 16;
            ds.DrawText("nec", xec, yN, color3);
            ds.DrawText("ne", xe, yN, color3);
            ds.DrawText("N", xC, yN, color1);
            ds.DrawText("nw", xw, yN, color3);
            ds.DrawText("nwc", xwc, yN, color3);

            ds.DrawText("sec", xec, yS, color3);
            ds.DrawText("se", xe, yS, color3);
            ds.DrawText("S", xC, yS, color1);
            ds.DrawText("sw", xw, yS, color3);
            ds.DrawText("swc", xwc, yS, color3);


            var xE = b + 3;
            var xW = 8;
            var yC = c - 14;
            var yn = a + _workAxis - 14;
            var ys = b - _workAxis - 14;

            ds.DrawText("en", xE, yn, color3);
            ds.DrawText("E", xE, yC, color1);
            ds.DrawText("es", xE, ys, color3);

            ds.DrawText("wn", xW - 4, yn, color3);
            ds.DrawText("W", xW, yC, color1);
            ds.DrawText("ws", xW - 4, ys, color3);
        }
        #endregion

        #region DrawTargetContacts  ===========================================
        private void DrawTargetContacts(CanvasDrawingSession ds)
        {
            const float Z = 0;
            const float E = EditMargin + EditSize + EditMargin;

            var cp = Center;

            CheckContacts();
            var N = _contactTargets.Count;

            _targetPoints.Clear();
            for (int i = 0; i < N; i++)
            {
                var (cont, targ, point, size) = _contactTargets[i];
                var p = point * EditScale;
                var c = Center + p;
                _targetPoints.Add(c);
                ds.DrawCircle(c, 8, Colors.Cyan, 5);

                #region DrawSimulatedEdge  ====================================
                switch (targ)
                {
                    case Target.N:
                        ds.DrawLine(c.X, c.Y, c.X, Z, Colors.Cyan, 3);
                        break;
                    case Target.S:
                        ds.DrawLine(c.X, c.Y, c.X, E, Colors.Cyan, 3);
                        break;
                    case Target.E:
                        ds.DrawLine(c.X, c.Y, E, c.Y, Colors.Cyan, 3);
                        break;
                    case Target.W:
                        ds.DrawLine(c.X, c.Y, Z, c.Y, Colors.Cyan, 3);
                        break;
                    case Target.NE:
                        ds.DrawLine(c.X, c.Y, c.X, Z, Colors.Cyan, 3);
                        break;
                    case Target.NW:
                        ds.DrawLine(c.X, c.Y, c.X, Z, Colors.Cyan, 3);
                        break;
                    case Target.SE:
                        ds.DrawLine(c.X, c.Y, c.X, E, Colors.Cyan, 3);
                        break;
                    case Target.SW:
                        ds.DrawLine(c.X, c.Y, c.X, E, Colors.Cyan, 3);
                        break;
                    case Target.EN:
                        ds.DrawLine(c.X, c.Y, E, c.Y, Colors.Cyan, 3);
                        break;
                    case Target.ES:
                        ds.DrawLine(c.X, c.Y, E, c.Y, Colors.Cyan, 3);
                        break;
                    case Target.WN:
                        ds.DrawLine(c.X, c.Y, Z, c.Y, Colors.Cyan, 3);
                        break;
                    case Target.WS:
                        ds.DrawLine(c.X, c.Y, Z, c.Y, Colors.Cyan, 3);
                        break;
                    case Target.NEC:
                        ds.DrawLine(c.X, c.Y, E, Z, Colors.Cyan, 3);
                        break;
                    case Target.NWC:
                        ds.DrawLine(c.X, c.Y, Z, Z, Colors.Cyan, 3);
                        break;
                    case Target.SEC:
                        ds.DrawLine(c.X, c.Y, E, E, Colors.Cyan, 3);
                        break;
                    case Target.SWC:
                        ds.DrawLine(c.X, c.Y, Z, E, Colors.Cyan, 3);
                        break;
                }
                #endregion

                if (cont == Contact.Any && size > 0)
                {
                    var (p1, p2) = XYPair.GetScaledNormal(targ, point, size, Center, EditScale);
                    ds.DrawLine(p1, p2, Color.FromArgb(0x80, 0xFF, 0, 0), 20);
                }
            }
        }
        #endregion

        #region Target_Contacts  ==============================================
        private Dictionary<Target, (Contact contact, (sbyte dx, sbyte dy) point, byte size)> Target_Contacts = new Dictionary<Target, (Contact, (sbyte, sbyte), byte)>(5);
        private List<(Contact cont, Target targ, Vector2 point, float size)> _contactTargets = new List<(Contact, Target, Vector2, float)>();

        #region InitContactControls  ==========================================
        private void InitContactControls()
        {
            foreach (var e in Target_Contacts)
            {
                switch (e.Key)
                {
                    case Target.N:
                        Contact_N = e.Value.contact;
                        SetContactHighlight(Contact_N, ContactComboBox_N, ContactSizeSlider_N, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.S:
                        Contact_S = e.Value.contact;
                        SetContactHighlight(Contact_S, ContactComboBox_S, ContactSizeSlider_S, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.E:
                        Contact_E = e.Value.contact;
                        SetContactHighlight(Contact_E, ContactComboBox_E, ContactSizeSlider_E, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.W:
                        Contact_W = e.Value.contact;
                        SetContactHighlight(Contact_W, ContactComboBox_W, ContactSizeSlider_W, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.NE:
                        Contact_NE = e.Value.contact;
                        SetContactHighlight(Contact_NE, ContactComboBox_NE, ContactSizeSlider_NE, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.NW:
                        Contact_NW = e.Value.contact;
                        SetContactHighlight(Contact_NW, ContactComboBox_NW, ContactSizeSlider_NW, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.SE:
                        Contact_SE = e.Value.contact;
                        SetContactHighlight(Contact_SE, ContactComboBox_SE, ContactSizeSlider_SE, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.SW:
                        Contact_SW = e.Value.contact;
                        SetContactHighlight(Contact_SW, ContactComboBox_SW, ContactSizeSlider_SW, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.EN:
                        Contact_EN = e.Value.contact;
                        SetContactHighlight(Contact_EN, ContactComboBox_EN, ContactSizeSlider_EN, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.ES:
                        Contact_ES = e.Value.contact;
                        SetContactHighlight(Contact_ES, ContactComboBox_ES, ContactSizeSlider_ES, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.WN:
                        Contact_WN = e.Value.contact;
                        SetContactHighlight(Contact_WN, ContactComboBox_WN, ContactSizeSlider_WN, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.WS:
                        Contact_WS = e.Value.contact;
                        SetContactHighlight(Contact_WS, ContactComboBox_WS, ContactSizeSlider_WS, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.NEC:
                        Contact_NEC = e.Value.contact;
                        SetContactHighlight(Contact_NEC, ContactComboBox_NEC, ContactSizeSlider_NEC, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.NWC:
                        Contact_NWC = e.Value.contact;
                        SetContactHighlight(Contact_NWC, ContactComboBox_NWC, ContactSizeSlider_NWC, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.SEC:
                        Contact_SEC = e.Value.contact;
                        SetContactHighlight(Contact_SEC, ContactComboBox_SEC, ContactSizeSlider_SEC, Shape.ToFloat(e.Value.size));
                        break;
                    case Target.SWC:
                        Contact_SWC = e.Value.contact;
                        SetContactHighlight(Contact_SWC, ContactComboBox_SWC, ContactSizeSlider_SWC, Shape.ToFloat(e.Value.size));
                        break;
                }
            }
        }
        #endregion

        #region CheckContacts  ================================================
        private void CheckContacts()
        {
            _contactTargets.Clear();

            CheckContact(Contact_NWC, Target.NWC, new Vector2(-1, -1), 0);
            CheckContact(Contact_NW, Target.NW, new Vector2(-.5f, -1), 0);
            CheckContact(Contact_N, Target.N, new Vector2(0, -1), 0);
            CheckContact(Contact_NE, Target.NE, new Vector2(.5f, -1), 0);
            CheckContact(Contact_NEC, Target.NEC, new Vector2(1, -1), 0);

            CheckContact(Contact_EN, Target.EN, new Vector2(1, -.5f), 0);
            CheckContact(Contact_E, Target.E, new Vector2(1, 0), 0);
            CheckContact(Contact_ES, Target.ES, new Vector2(1, .5f), 0);

            CheckContact(Contact_WN, Target.WN, new Vector2(-1, -.5f), 0);
            CheckContact(Contact_W, Target.W, new Vector2(-1, 0), 0);
            CheckContact(Contact_WS, Target.WS, new Vector2(-1, .5f), 0);

            CheckContact(Contact_SWC, Target.SWC, new Vector2(-1, 1), 0);
            CheckContact(Contact_SW, Target.SW, new Vector2(-.5f, 1), 0);
            CheckContact(Contact_S, Target.S, new Vector2(0, 1), 0);
            CheckContact(Contact_SE, Target.SE, new Vector2(.5f, 1), 0);
            CheckContact(Contact_SEC, Target.SEC, new Vector2(1, 1), 0);

            void CheckContact(Contact cont, Target targ, Vector2 point, float size)
            {
                if (cont == Contact.None)
                {
                    Target_Contacts.Remove(targ);
                }
                else
                {
                    if (Target_Contacts.TryGetValue(targ, value: out (Contact c, (sbyte, sbyte) p, byte s) e))
                    {
                        Target_Contacts[targ] = (cont, e.p, e.s);

                        point = Shape.ToVector(e.p);
                        size = Shape.ToFloat(e.s);
                    }
                    else
                    {
                        Target_Contacts.Add(targ, (cont, Shape.ToSByte(point), Shape.ToByte(size)));
                    }
                    _contactTargets.Add((cont, targ, point, size));
                }
            }
        }
        #endregion
        #endregion
    }
}
