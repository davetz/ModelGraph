using ModelGraph.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ModelGraph.Controls
{
    public sealed partial class SymbolEditControl
    {

        #region PropertyChanged  ==============================================
        private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (_changesEnabled)
            {
                _shapeColor = ColorPicker.Color;
                SetProperty(ProertyId.ShapeColor);
            }
        }
        private void StrokeWidthSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (_changesEnabled)
            {
                SetProperty(ProertyId.StrokeWidth);
            }
        }
        private void FillStroke_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_changesEnabled)
            {
                SetProperty(ProertyId.FillStroke);
            }
        }
        private void DashStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_changesEnabled)
            {
                SetProperty(ProertyId.DashStyle);
            }
        }
        private void LineJoin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_changesEnabled)
            {
                SetProperty(ProertyId.LineJoin);
            }
        }
        private void StartCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_changesEnabled)
            {
                SetProperty(ProertyId.StartCap);
            }
        }
        private void EndCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_changesEnabled)
            {
                SetProperty(ProertyId.EndCap);
            }
        }
        private void DashCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_changesEnabled)
            {
                SetProperty(ProertyId.DashCap);
            }
        }
        #endregion

        #region CentralSize  ==================================================
        private void CentralSizeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var value = (float)CentralSizeSlider.Value;
            if (Changed(value, _centralSize))
            {
                Shape.ResizeCentral(SelectedShapes, value);
                SetSizeSliders();
                EditorCanvas.Invalidate();
            }
        }
        private void SetCentralSize(float value)
        {
            _centralSize = value;
            CentralSizeSlider.Value = value;
        }
        private float _centralSize;
        #endregion

        #region VerticalSize  =================================================
        private void VerticalSizeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var value = (float)VerticalSizeSlider.Value;
            if (Changed(value, _verticalSize))
            {
                Shape.ResizeVertical(SelectedShapes, value);
                SetSizeSliders();
                EditorCanvas.Invalidate();
            }
        }
        private void SetVerticalSize(float value)
        {
            _verticalSize = value;
            VerticalSizeSlider.Value = value;
        }
        private float _verticalSize;
        #endregion

        #region HorizontalSize  ===============================================
        private void HorizontalSizeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var value = (float)HorizontalSizeSlider.Value;
            if (Changed(value, _horizontalSize))
            {
                Shape.ResizeHorizontal(SelectedShapes, value);
                SetSizeSliders();
                EditorCanvas.Invalidate();
            }
        }
        private void SetHorizontalSize(float value)
        {
            _horizontalSize = value;
            HorizontalSizeSlider.Value = value;
        }
        private float _horizontalSize;
        #endregion

        #region MajorSize  ====================================================
        private void MajorSizeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var value = (float)MajorSizeSlider.Value;
            if (Changed(value, _majorSize))
            {
                Shape.ResizeMajorAxis(SelectedShapes, value);
                SetSizeSliders();
                EditorCanvas.Invalidate();
            }
        }
        private void SetMajorSize(float value)
        {
            _majorSize = value;
            MajorSizeSlider.Value = value;
        }
        private float _majorSize;
        #endregion

        #region MinorAxisSize  ================================================
        private void MinorSizeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var value = (float)MinorSizeSlider.Value;
            if (Changed(value, _minorSize))
            {
                Shape.ResizeMinorAxis(SelectedShapes, value);
                SetSizeSliders();
                EditorCanvas.Invalidate();
            }
        }
        private void SetMinorSize(float value)
        {
            _minorSize = value;
            MinorSizeSlider.Value = value;
        }
        private float _minorSize;
        #endregion

        #region PolyDimension  ================================================
        private void DimensionSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var value = (float)DimensionSlider.Value;
            if (Changed(value, _polyDimension))
            {
                Shape.SetDimension(SelectedShapes, value);
                SetSizeSliders();
                EditorCanvas.Invalidate();
            }
        }
        private void SetDemeinsonSize(float min, float max, float value)
        {
            _polyDimension = value;
            DimensionSlider.Minimum = min;
            DimensionSlider.Maximum = max;
            DimensionSlider.Value = value;
        }
        private float _polyDimension;
        #endregion

        #region TernarySize  ==================================================
        private void TernarySizeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var value = (float)TernarySizeSlider.Value;
            if (Changed(value, _ternarySize))
            {
                Shape.ResizeTernaryAxis(SelectedShapes, value);
                SetSizeSliders();
                EditorCanvas.Invalidate();
            }
        }
        private void SetTernarySize(float value)
        {
            _ternarySize = value;
            TernarySizeSlider.Value = value;
        }
        private float _ternarySize;
        #endregion

        #region SetSizeSliders  ===============================================
        private void SetSizeSliders()
        {
            if (SelectedShapes.Count > 0)
            {
                _changesEnabled = false;

                var (min, max, dim, aux, major, minor, cent, vert, horz) = Shape.GetSliders(SelectedShapes);
                SetCentralSize(cent);
                SetVerticalSize(vert);
                SetHorizontalSize(horz);
                SetMajorSize(major);
                SetMinorSize(minor);
                SetTernarySize(aux);
                SetDemeinsonSize(min, max, dim);

                _changesEnabled = true;
            }
        }
        private bool Changed(float v1, float v2) => _changesEnabled && v1 != v2;
        bool _changesEnabled;
        #endregion

        #region LeftButtonClick  ==============================================
        private void OneManyButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToggleOneManyButton();
        }
        private void ToggleOneManyButton()
        {
            if (IsSelectOneOrMoreShapeMode)
            {
                OneManyButton.Content = "\uE8C5";
                ToolTipService.SetToolTip(OneManyButton, "_001A".GetLocalized());
                IsSelectOneOrMoreShapeMode = false;
            }
            else
            {
                OneManyButton.Content = "\uE8C4";
                ToolTipService.SetToolTip(OneManyButton, "_001B".GetLocalized());
                IsSelectOneOrMoreShapeMode = true;
            }
        }
        private bool IsSelectOneOrMoreShapeMode = true;

        private void CutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (SelectedShapes.Count > 0)
            {
                CutCopyShapes.Clear();
                foreach (var shape in SelectedShapes)
                {
                    CutCopyShapes.Add(shape);
                    SymbolShapes.Remove(shape);
                }
                SelectedShapes.Clear();

                EditorCanvas.Invalidate();
            }
        }

        private void CopyButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (SelectedShapes.Count > 0)
            {
                CutCopyShapes.Clear();
                foreach (var shape in SelectedShapes)
                {
                    CutCopyShapes.Add(shape.Clone());
                }
                SelectedShapes.Clear();

                EditorCanvas.Invalidate();
            }
        }

        private void PasteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (CutCopyShapes.Count > 0)
            {
                SelectedShapes.Clear();
                foreach (var template in CutCopyShapes)
                {
                    var shape = template.Clone();
                    SymbolShapes.Add(shape);
                    SelectedShapes.Add(shape);
                }
                SetShapesAreSelected();
                EditorCanvas.Invalidate();
            }
        }

        private void RecenterButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (SelectedShapes.Count > 0)
            {
                Shape.SetCenter(SelectedShapes, Vector2.Zero);
                EditorCanvas.Invalidate();
            }
        }
        private void RotateLeftButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => RotateLeft();
        private void RotateRightButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => RotateRight();
        private void RotateAngleButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_use30degreeDelta)
            {
                _use30degreeDelta = false;
                RotateAngleButton.Content = "22.5";
            }
            else
            {
                _use30degreeDelta = true;
                RotateAngleButton.Content = "30.0";
            }
        }
        private void FlipVerticalButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => FlipVertical();
        private void FlipHorizontalButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) => FlipHorizontal();
        #endregion

        #region EditorKeyboardAccelerators  ===================================
        private void KeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            TryInvokeEventAction(args.KeyboardAccelerator.Key, args.KeyboardAccelerator.Modifiers);
        }
        #endregion

        #region Picker_PointerEvents  =========================================
        private int _pickerIndex = -1;
        private void PickerCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PickerShape = null;

            _pickerIndex = GetPickerShapeIndex(e);

            PickerCanvas.Invalidate();
        }

        private void PickerCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var index = GetPickerShapeIndex(e);
            if (index < 0 || (index != _pickerIndex)) return;

            SetPicker(PickerShapes[index]);

            PickerCanvas.Invalidate();
            EditorCanvas.Invalidate();
        }
        private int GetPickerShapeIndex(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(PickerCanvas).Position;
            int index = (int)(p.Y / PickerCanvas.Width);
            return (index < 0 || index >= PickerShapes.Count) ? -1 : index;
        }
        #endregion

        #region Symbol_PointerEvents  =========================================
        private bool _symbolPointerPressed;
        private void SymbolCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _symbolPointerPressed = true;
        }

        private void SymbolCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_symbolPointerPressed)
            {
                _symbolPointerPressed = false;

                if (SelectedShapes.Count > 0)
                {
                    SelectedShapes.Clear();
                    SetIdleOnVoid();
                }
                else
                {
                    foreach (var shape in SymbolShapes) { SelectedShapes.Add(shape); }
                    SetShapesAreSelected();
                }


                PickerShape = null;
                PickerCanvas.Invalidate();
            }
        }
        #endregion

        #region Selector_PointerEvents  =======================================
        private int _selectorIndex = -1;
        private bool _selectPointerPressed;
        private bool _ignoreSelectPointerReleased;
        private void SelectorCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _selectPointerPressed = true;
            _selectorIndex = GetSelectorIndex(e);
        }

        private void SelectorCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_selectPointerPressed && _selectorIndex >= 0 && SelectedShapes.Count == 0)
            {
                var index = GetSelectorIndex(e);
                if (index >= 0 && index != _selectorIndex && SelectedShapes.Count == 0)
                {
                    var shape = SymbolShapes[_selectorIndex];
                    SymbolShapes[_selectorIndex] = SymbolShapes[index];
                    SymbolShapes[index] = shape;
                    _selectorIndex = index;
                    _ignoreSelectPointerReleased = true;
                    EditorCanvas.Invalidate();
                }
            }
        }

        private void SelectorCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_selectPointerPressed && !_ignoreSelectPointerReleased)
            {
                var index = GetSelectorIndex(e);
                if (index < 0 || _selectorIndex < 0)
                    ShapeSelectorMiss();
                else if (index == _selectorIndex)
                    ShapeSelectorHit(index);
            }
            _selectPointerPressed = false;
            _ignoreSelectPointerReleased = false;
        }
        private int GetSelectorIndex(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(SelectorCanvas).Position;
            int index = (int)(p.Y / SelectorCanvas.Width);
            return (index < 0 || index >= SymbolShapes.Count) ? -1 : index;
        }
        #endregion

        #region Editor_PointerEvents  =========================================
        private bool _editorPointerPressed;
        private void EditorCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _editorPointerPressed = true;
            RawPoint1 = GetRawPoint(e);
            ShapePoint1 = ShapePoint(RawPoint1);

            TryInvokeEventAction(PointerEvent.Begin);
        }
        private void EditorCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            RawPoint2 = GetRawPoint(e);
            ShapePoint2 = ShapePoint(RawPoint2);

            if (_editorPointerPressed)
                TryInvokeEventAction(PointerEvent.Drag);
            else
                TryInvokeEventAction(PointerEvent.Hover);
            RawPoint1 = RawPoint2;
            ShapePoint1 = ShapePoint(RawPoint1);
        }
        private void EditorCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _editorPointerPressed = false;
            RawPoint2 = GetRawPoint(e);
            ShapePoint2 = ShapePoint(RawPoint2);

            TryInvokeEventAction(PointerEvent.End);
        }
        private Vector2 GetRawPoint(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(EditorCanvas).Position;
            return new Vector2((float)p.X, (float)p.Y);
        }
        private Vector2 ShapePoint(Vector2 rawPoint) => (rawPoint - Center) / EditZoom;
        #endregion
    }
}
