using ModelGraphSTD;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace ModelGraph.Controls
{
    public sealed partial class ModelGraphControl
    {
        #region Parameters  ===================================================
        internal enum EventAction
        {
            Invalid,

            IdleOnVoid,
            AutoPanning,

            IdleOnNode,
            MovingNode,
            ResizingNode,
            IdleOnNodeResize,

            IdleOnEdge,

            IdleOnRegion,
            MovingRegion,
            TracingRegion,
        }
        private EventAction _eventAction;
        private bool SetEventAction(EventAction eventAction)
        {
            if (_eventAction == eventAction)
            {
                return false;
            }

            Debug.WriteLine($"{eventAction}");
            _eventAction = eventAction;
            return true;
        }
        private bool _enableHitTest;

        private Extent _rootDelta = new Extent(); // rolling point1, point2 delta (RoodCanvas)
        private Extent _drawDelta = new Extent(); // rolling point1, point2 delta (DrawCanvas)
        private Extent _dragDelta = new Extent(); // rolling point1, point2 delta (DrawCanvas)

        private Extent _rootRef = new Extent(); // point1 pointerPressed, point2 = poinnterMoved, pointerReleased (RoodCanvas)
        private Extent _drawRef = new Extent(); // point1 pointerPressed, point2 = poinnterMoved, pointerReleased (DrawCanvas)

        private Modifier _modifier;
        private string _keyName;
        private (int X, int Y) _arrowDelta;
        private int _wheelDelta;

        private Region _traceRegion;
        private Selector _selector;

        private Action EndAction;      // mouse button up
        private Action DragAction;     // mouse move with button 1 down
        private Action HoverAction;    // mouse move with no buttons down
        private Action WheelAction;    // mouse wheel changed
        private Action ArrowAction;    // arrow key pressed
        private Action CancelAction;   // escape key pressd
        private Action Begin1Action;   // mouse button 1 down
        private Action Begin3Action;   // mouse button 3 down
        private Action ExecuteAction;  // mouse double click
        private Action ShortCutAction; // keyboard key press A..Z, Insert, Delete,..
        #endregion

        #region OnVoid  =======================================================

        #region SetIdleOnVoid  ================================================
        private void SetIdleOnVoid()
        {
            if (!SetEventAction(EventAction.IdleOnVoid))
            {
                return;
            }

            HideTootlip();
            //DisableAutoPan();
            //Cursor = Cursors.Arrow;
            _enableHitTest = true;

            EndAction = () => { RemoveSelectors(); };
            DragAction = () => { if (_drawRef.Length > 3) { _enableHitTest = false; _traceRegion = new Region(_drawRef.Point1); SetTracingRegion(); } };
            HoverAction = IdleHitTest;
            WheelAction = WheelPanZoom;
            ArrowAction = null;
            CancelAction = () => { RemoveSelectors(); };
            Begin1Action = () => { if (_modifier == Modifier.Shift) { SetAutoPanning(); } };
            Begin3Action = null;
            ExecuteAction = () => { RemoveSelectors(); _ignorePointerMoved = true; ZoomToExtent(_graph.Extent); };
            ShortCutAction = null;
        }
        #endregion

        #region SetAutoPanning  ===============================================
        private void SetAutoPanning()
        {
            if (!SetEventAction(EventAction.AutoPanning))
            {
                return;
            }

            //HideTootlip();
            //Cursor = Cursors.Arrow;
            _enableHitTest = false;
            //EnableAutoPan();

            EndAction = SetIdleOnVoid;
            //DragAction = PositionAutoPan;
            HoverAction = null;
            WheelAction = null;
            ArrowAction = null;
            CancelAction = SetIdleOnVoid;
            Begin1Action = null;
            Begin3Action = null;
            ExecuteAction = null;
            ShortCutAction = null;
        }
        #endregion

        #region WheelPanZoom  =================================================
        private void WheelPanZoom()
        {
            if (_selector.IsVoidHit)
            {
                // Scroll towards and away from the mouse location when using the mouse wheel
                if ((_modifier & Modifier.Ctrl) != 0)
                {
                    if (_wheelDelta > 0) { ZoomIn(); }
                    else { ZoomOut(); }
                }
                else if ((_modifier & Modifier.Shift) != 0)
                {
                    if (_wheelDelta > 0) { PanLeft(); }
                    else { PanRight(); }
                }
                else
                {
                    if (_wheelDelta > 0) { PanUP(); }
                    else { PanDown(); }
                }
            }
        }
        #endregion

        #endregion

        #region OnNode  =======================================================

        #region SetIdleOnNode  ================================================
        private void SetIdleOnNode()
        {
            SetEventAction(EventAction.IdleOnNode);

            _enableHitTest = true;

            ShowNodeTooltip(_selector.HitNode);
            //Cursor = Cursors.Hand;

            EndAction = null;
            DragAction = null;
            HoverAction = IdleHitTest;
            WheelAction = null;
            ArrowAction = () => { _selector.Move(_arrowDelta); PostRefresh(); }; 
            CancelAction = null;
            Begin1Action = SetMovingNode;
            Begin3Action = null;
            ExecuteAction = null;
            ShortCutAction = IdleOnNodeShortCut;
        }
        private void IdleOnNodeShortCut()
        {
            //if (_modifier == Modifier.Zip)
            //{
            //    if (_keyName == "V") _hitNode.Node.Aspect = Aspect.Vertical;
            //    else if (_keyName == "H") _hitNode.Node.Aspect = Aspect.Horizontal;
            //    else if (_keyName == "C") _hitNode.Node.Aspect = Aspect.Central;
            //    else if (_keyName == "A") _hitNode.Node.Resizing = Resizing.Auto;
            //    else if (_keyName == "M") _hitNode.Node.Resizing = Resizing.Manual;
            //}
            //if (_hitNode.IsModified) Model.PostRefreshGraph();
        }
        #endregion

        #region SetIdleOnNodeResize  ==========================================
        private void SetIdleOnNodeResize()
        {
            SetEventAction(EventAction.IdleOnNodeResize);

            //HideTootlip();
            //Cursor = (_hitNode.Node.IsHorizontal) ? Cursors.SizeWE : Cursors.SizeNS;
            _enableHitTest = true;

            EndAction = null;
            DragAction = null;
            HoverAction = IdleHitTest;
            WheelAction = null;
            //ArrowAction = () => { NodeResize(_arrowDelta * 2); };
            CancelAction = null;
            Begin1Action = SetResizingNode;
            Begin3Action = null;
            ExecuteAction = null;
            ShortCutAction = null;
        }
        #endregion

        #region SetResizingNode  ==============================================
        private void SetResizingNode()
        {
            SetEventAction(EventAction.ResizingNode);

            //HideTootlip();
            _enableHitTest = false;

            EndAction = () => { IdleHitTest(); };
            //DragAction = () => { if (NodeResize(_moveDelta)) _prevMovePoint = _moveViewPoint; };
            HoverAction = null;
            WheelAction = null;
            ArrowAction = null;
            CancelAction = null;
            Begin1Action = null;
            Begin3Action = null;
            ExecuteAction = null;
            ShortCutAction = null;
        }
        //private bool NodeResize(Vector d) { var anyChange = _hitNode.Node.AddLength(d.X, d.Y, HitTop, HitLeft); if (anyChange) Model.PostRefreshGraph(); return anyChange; }
        #endregion

        #region SetMovingNode  ================================================
        private void SetMovingNode()
        {
            if (!SetEventAction(EventAction.MovingNode))
            {
                return;
            }

            HideTootlip();
            //Cursor = Cursors.ScrollAll;
            _enableHitTest = false;

            EndAction = () => { SetIdleOnNode(); PostRefresh(); };
            DragAction = () => { _selector.Move(_dragDelta.Delta); _dragDelta.Record(_drawRef.Point2); };
            HoverAction = null;
            WheelAction = null;
            ArrowAction = () => { _selector.Move(_arrowDelta); PostRefresh(); };
            CancelAction = null;
            Begin1Action = null;
            Begin3Action = null;
            ExecuteAction = null;
            ShortCutAction = null;
        }
        #endregion

        #endregion

        #region OnEdge  =======================================================

        #region SetIdleOnEdge  ================================================
        private void SetIdleOnEdge()
        {
            if (!SetEventAction(EventAction.IdleOnEdge))
            {
                return;
            }

            ShowEdgeTooltip(_selector.HitEdge);
            //Cursor = Cursors.Hand;
            _enableHitTest = true;

            EndAction = null;
            DragAction = null;
            HoverAction = IdleHitTest;
            WheelAction = null;
            ArrowAction = null;
            CancelAction = null;
            Begin1Action = null;
            Begin3Action = null;
            ExecuteAction = null;
            ShortCutAction = CycleEdgeParams;
        }
        private void CycleEdgeParams()
        {
            if (_keyName == "Delete")
            {
                //Model.DeleteEdge(_hitEdge.Edge);
                //Model.PostGraphRefresh();
                return;
            }
            //if (HitNearEnd1)
            //{
            //    switch (_keyName)
            //    {
            //        case "G":
            //            var n = (int)_hitEdge.Edge.Gnarl1 + 1;
            //            if (n > 7) n = 0;
            //            _hitEdge.Edge.Gnarl1 = (FacetOf)n;
            //            _hitEdge.Edge.SetIsModified();
            //            Model.PostRefreshGraph();
            //            break;
            //        case "C":
            //            n = (int)_hitEdge.Edge.Contact1 + 1;
            //            if (n > 15) n = 0;
            //            _hitEdge.Edge.Contact1 = (Contact)n;
            //            _hitEdge.Edge.SetIsModified();
            //            Model.PostRefreshGraph();
            //            break;
            //    }
            //}
            //else if (HitNearEnd2)
            //{
            //    switch (_keyName)
            //    {
            //        case "G":
            //            var n = (int)_hitEdge.Edge.Gnarl2 + 1;
            //            if (n > 7) n = 0;
            //            _hitEdge.Edge.Gnarl2 = (FacetOf)n;
            //            _hitEdge.Edge.SetIsModified();
            //            Model.PostRefreshGraph();
            //            break;
            //        case "C":
            //            n = (int)_hitEdge.Edge.Contact2 + 1;
            //            if (n > 15) n = 0;
            //            _hitEdge.Edge.Contact2 = (Contact)n;
            //            _hitEdge.Edge.SetIsModified();
            //            Model.PostRefreshGraph();
            //            break;
            //    }
            //}
        }
        #endregion

        #endregion

        #region OnRegion  =====================================================

        #region SetIdleOnRegion  ==============================================
        private void SetIdleOnRegion()
        {
            if (!SetEventAction(EventAction.IdleOnRegion))
            {
                return;
            }

            HideTootlip();
            //Cursor = Cursors.Hand;
            _enableHitTest = true;

            EndAction = null;
            DragAction = null;
            HoverAction = IdleHitTest;
            WheelAction = null;
            ArrowAction = () => { _selector.Move(_arrowDelta); PostRefresh(); };
            CancelAction = () => { RemoveSelectors(); SetIdleOnVoid(); };
            Begin1Action = SetMovingRegion;
            Begin3Action = null;
            ExecuteAction = () => { var e = Extent.Create(_selector.Nodes, 16); _ignorePointerMoved = true; ZoomToExtent(e); };
            ShortCutAction = IdleOnRegionShortCuts;
        }
        private void IdleOnRegionShortCuts()
        {
            if (_modifier == Modifier.None)
            {
                switch (_keyName)
                {
                //    case "A": Allign(); break;
                    case "V": _selector.AlignVertical(); PostRefresh(); break;
                    case "H": _selector.AlignHorizontal(); PostRefresh(); break;
                    case "R": _selector.Rotate(); PostRefresh();  break;
                //    case "Delete": DeleteRegionNodes(); break;
                }
            }
        }
        #endregion

        #region SetMovingRegion  ==============================================
        private void SetMovingRegion()
        {
            if (!SetEventAction(EventAction.MovingRegion))
            {
                return;
            }

            //Cursor = Cursors.ScrollAll;
            _enableHitTest = false;

            EndAction = () => { SetIdleOnRegion(); PostRefresh(); };
            DragAction = () => { _selector.Move(_dragDelta.Delta); _dragDelta.Record(_drawRef.Point2); };
            HoverAction = null;
            WheelAction = null;
            ArrowAction = () => { _selector.Move(_arrowDelta); };
            CancelAction = () => { RemoveSelectors(); SetIdleOnRegion(); }; ;
            Begin1Action = null;
            Begin3Action = null;
            ExecuteAction = null;
            ShortCutAction = null;
        }
        #endregion

        #region SetTracingRegion  =============================================
        private void SetTracingRegion()
        {
            if (!SetEventAction(EventAction.TracingRegion))
            {
                return;
            }

            // Cursor = Cursors.Pen;
            _enableHitTest = false;

            EndAction = () => { CloseRegion();  };
            DragAction = () => { _traceRegion?.Add(_drawRef.Point2); };
            HoverAction = null;
            WheelAction = null;
            ArrowAction = null;
            CancelAction = () => { RemoveSelectors(); SetIdleOnVoid(); }; ;
            Begin1Action = null;
            Begin3Action = null;
            ExecuteAction = null;
            ShortCutAction = null;
        }
        private void TraceRegion()
        {
            if (_traceRegion == null)
            {
                return;
            }

            if (_traceRegion.Add(_drawRef.Point2))
            {
                DrawCanvas.Invalidate();
            }
        }
        private void CloseRegion()
        {
            if (_traceRegion == null)
            {
                return;
            }

            _traceRegion.Close(_drawRef.Point2);
            _selector.TryAddRegion(_traceRegion);
            _traceRegion = null;

            DrawCanvas.Invalidate();
            SetIdleOnVoid();
        }
        #endregion

        #endregion

        #region PostRefresh  ==================================================
        void PostRefresh()
        {
            HideTootlip();
            _graph.AdjustGraph(_selector);
            _model.PostRefresh();
        }
        #endregion

        #region IdleHitTest  ==================================================
        private void IdleHitTest()
        {
            if (_selector.IsVoidHit)
            {
                SetIdleOnVoid();
            }
            else if (_selector.IsRegionHit)
            {
                SetIdleOnRegion();
            }
            else if (_selector.IsNodeHit)
            {
                //if (_hitNode.Node.IsNode && ((_hitNode.Node.IsVertical && (HitTop || HitBottom)) || (_hitNode.Node.IsHorizontal && (HitLeft || HitRignt))))
                //    IdleOnNodeResize();

                //else
                    SetIdleOnNode();
            }

            else if (_selector.IsEdgeHit)
            {
                SetIdleOnEdge();
            }
        }
        #endregion

        #region Commands  =====================================================
        #region Pan
        //=====================================================================
        const double scrollDelta = 120;

        internal void PanUP() { ScrollVerticalDelta(-scrollDelta); }
        internal void PanDown() { ScrollVerticalDelta(scrollDelta); }
        internal void PanLeft() { ScrollHorizontalDelta(-scrollDelta); }
        internal void PanRight() { ScrollHorizontalDelta(scrollDelta); }

        internal bool CanPanUp() { return true; }
        internal bool CanPanDown() { return true; }
        internal bool CanPanLeft() { return true; }
        internal bool CanPanRight() { return true; }
        //=====================================================================
        #endregion

        #region Zoom
        //=====================================================================

        internal bool CanZoomIn() { return true; }
        internal bool CanZoomOut() { return true; }
        internal bool CanZoomToObject() { return true; }
        internal bool CanZoomToExtent(Extent extent) { return true; }
        //=====================================================================
        #endregion

        #region Cut
        //=====================================================================
        internal void Cut()
        {
        }
        internal bool CanCut()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region Copy
        //=====================================================================
        internal void Copy()
        {
        }
        internal bool CanCopy()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region Undo
        //=====================================================================
        internal void Undo()
        {
        }
        internal bool CanUndo()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region Redo
        //=====================================================================
        internal void Redo()
        {
        }
        internal bool CanRedo()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region Find
        //=====================================================================
        internal bool Find(string item)
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region Paste
        //=====================================================================
        internal void Paste()
        {
        }
        internal bool CanPaste()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region Delete
        //=====================================================================
        internal void Delete()
        {
        }
        internal bool CanDelete()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region JumpTo
        //=====================================================================
        internal void JumpTo()
        {
        }
        internal bool CanJumpTo()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region Export
        //=====================================================================
        internal void Export()
        {
        }
        internal bool CanExport()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region Import
        //=====================================================================
        internal void Import()
        {
        }
        internal bool CanImport()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region SetName
        //=====================================================================
        internal void SetName(string name)
        {
        }
        internal bool CanSetName(string name)
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region SelectAll
        //=====================================================================
        internal void SelectAll()
        {
        }
        internal bool CanSelectAll()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region Disconnect
        //=====================================================================
        internal void Disconnect()
        {
        }
        internal bool CanDisconnect()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region PanToOther
        //=====================================================================
        internal void PanToOther()
        {
        }
        internal bool CanPanTo()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region AutoLayout
        //=====================================================================
        internal void AutoLayout()
        {
        }

        internal bool CanAutoLayout()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region PixelNudge
        //=====================================================================
        internal void PixelNudgeUp()
        {
            PixelNudge(new Vector2(0, 1));
        }
        internal void PixelNudgeDown()
        {
            PixelNudge(new Vector2(0, -1));
        }
        internal void PixelNudgeLeftOne()
        {
            PixelNudge(new Vector2(1, 0));
        }
        internal void PixelNudgeRightOne()
        {
            PixelNudge(new Vector2(-1, 0));
        }
        internal void PixelNudgeUpTend()
        {
            PixelNudge(new Vector2(0, 10));
        }
        internal void PixelNudgeDownTen()
        {
            PixelNudge(new Vector2(0, -10));
        }
        internal void PixelNudgeLeftTen()
        {
            PixelNudge(new Vector2(10, 0));
        }
        internal void PixelNudgeRightTen()
        {
            PixelNudge(new Vector2(-10, 0));
        }

        private void PixelNudge(Vector2 delta)
        {
        }

        private bool CanPixelNudge()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region QueryRoot
        //=====================================================================
        internal void QueryRoot()
        {
        }
        internal bool CanQueryRoot()
        {
            return true;
        }
        //=====================================================================
        #endregion

        #region GoForward
        //=====================================================================
        internal void GoForward()
        {
        }
        internal bool CanGoForward()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region GoBackward
        //=====================================================================
        internal void GoBackward()
        {
        }
        internal bool CanGoBackward()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region SetBarWidth
        //=====================================================================
        internal void SetBarWidth()
        {
        }
        internal bool CanSetBarWidth()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region SetResizing
        //=====================================================================
        internal void SetResizing()
        {
        }
        internal bool CanSetResizing()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region SetLabeling
        //=====================================================================
        internal void SetLabeling()
        {
        }
        internal bool CanSetLabeling()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region CenterInView
        //=====================================================================
        internal void CenterInView()
        {
        }
        internal bool CanCenterInView()
        {
            return true;
        }
        //=====================================================================
        #endregion

        #region ApplyGravity
        //=====================================================================
        internal void ApplyGravity()
        {
        }
        internal bool CanApplyGravity()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region Set<Central,Vertical,Horizontal>
        //=====================================================================
        internal void SetCentral()
        {
        }
        internal bool CanSetCentral()
        {
            return false;
        }

        internal void SetVertical()
        {
        }
        internal bool CanSetVertical()
        {
            return false;
        }

        internal void SetHorizontal()
        {
        }
        internal bool CanSetHorizontal()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region SetAxisStyle
        //=====================================================================
        internal void SetAxisStyle()
        {
        }
        internal bool CanSetAxisStyle()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region SetAlignment
        //=====================================================================
        internal void SetAlignment()
        {
        }
        internal bool CanSetAlignment()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region SetRegionNode
        //=====================================================================
        internal void SetRegionNode()
        {
        }
        internal bool CanSetRegionNode()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region SetOrientation
        //=====================================================================
        internal void SetOrientation()
        {
        }
        internal bool CanSetOrientation()
        {
            return false;
        }
        //=====================================================================
        #endregion

        #region BendPoint
        //===================================================
        internal void AddBendPoint()
        {
        }
        internal bool CanAddBendPoint()
        {
            return false;
        }
        internal void RemoveBendPoint()
        {
        }
        internal bool CanRemoveBendPoint()
        {
            return false;
        }
        internal void RemoveAllBendPoints()
        {
        }
        internal bool CanRemoveAllBendPoints()
        {
            return false;
        }
        //=====================================================================
        #endregion
        #endregion
    }
}
