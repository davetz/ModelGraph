using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Documents;
using Windows.UI.ViewManagement;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using ModelGraph.Internals;
using System.Threading.Tasks;
using ModelGraph.Helpers;

namespace ModelGraph
{
    public sealed partial class ModelTreeControl : UserControl, IModelControl
    {
        public ModelTreeControl(ModelRoot root)
        {
            _root = root;
            _root.ModelControl = this;

            InitializeComponent();

            ApplicationView.GetForCurrentView().TryResizeView(new Size(320, 320));

            Initialize();
        }

        #region SetSize  ======================================================
        public void SetSize(double width, double height)
        {
            TreeCanvas.Width = Width = width;
            TreeCanvas.Height = Height = height;

            _root.ViewCapacity =(int)(Height / _elementHieght);
            RefreshVisibleModels();
        }
        private void TreeCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoaded = true;

            TreeCanvas.Loaded -= TreeCanvas_Loaded;
            _root.ViewCapacity = (int)(ActualHeight / _elementHieght);
            RefreshVisibleModels();
        }
        private bool IsLoaded;

        #endregion

        #region Fields  =======================================================
        private ModelRoot _root;

        private ModelTree _previousModel;

        private ToolTip _itemIdentityTip;
        private ToolTip _modelIdentityTip;

        private int _levelIndent;
        private int _elementHieght;

        private Style _expanderStyle;
        private Style _itemKindStyle;
        private Style _itemNameStyle;
        private Style _itemInfoStyle;
        private Style _sortModeStyle;
        private Style _usageModeStyle;
        private Style _totalCountStyle;
        private Style _indentTreeStyle;
        private Style _filterModeStyle;
        private Style _filterTextStyle;
        private Style _filterCountStyle;
        private Style _propertyNameStyle;
        private Style _textPropertyStyle;
        private Style _checkPropertyStyle;
        private Style _comboPropertyStyle;
        private Style _modelIdentityStyle;
        private Style _propertyBorderStyle;

        private ToolTip[] _menuItemTips;
        private ToolTip[] _itemButtonTips;

        private Button[] _itemButtons;
        private MenuFlyoutItem[] _menuItems;
        private int _menuItemsCount;

        private ModelCommand _insertCommand;
        private ModelCommand _removeCommand;

        private int Count => (_root.ViewModels == null) ? 0 : _root.ViewModels.Length;

        // segoe ui symbol font glyphs  =====================
        private const string _fontFamily = "Segoe UI Symbol";
        private const string _leftCanExtend = "\u25b7";
        private const string _leftIsExtended = "\u25e2";

        private const string _rightCanExtend = "\u25c1";
        private const string _rightIsExtended = "\u25e3";

        private const string _sortNone = "\u2012";
        private const string _sortAscending = "\u2228";
        private const string _sortDescending = "\u2227";

        private const string _usageAll = "a";
        private const string _usageIsUsed = "u";
        private const string _usageIsNotUsed = "n";

        private const string _filterCanShow = "\u25BD";
        private const string _filterIsShowing = "\uE16E";

        private string _sortModeTip;
        private string _usageModeTip;
        private string _leftExpandTip;
        private string _totalCountTip;
        private string _filterTextTip;
        private string _filterCountTip;
        private string _rightExpandTip;
        private string _filterExpandTip;
        private ResourceLoader _resourceLoader;
        #endregion

        #region Close  ========================================================
        public void Close()
        {
        }
        #endregion

        #region Initialize  ===================================================
        private void Initialize()
        {

            _itemIdentityTip = new ToolTip();
            _itemIdentityTip.Opened += ItemIdentityTip_Opened;

            _modelIdentityTip = new ToolTip();
            _modelIdentityTip.Opened += ModelIdentityTip_Opened;

            _levelIndent = (int)(Resources["LevelIndent"] as Double?).Value;
            _elementHieght = (int)(Resources["ElementHieght"] as Double?).Value;

            _expanderStyle = Resources["ExpanderStyle"] as Style;
            _itemKindStyle = Resources["ItemKindStyle"] as Style;
            _itemNameStyle = Resources["ItemNameStyle"] as Style;
            _itemInfoStyle = Resources["ItemInfoStyle"] as Style;
            _sortModeStyle = Resources["SortModeStyle"] as Style;
            _usageModeStyle = Resources["UsageModeStyle"] as Style;
            _totalCountStyle = Resources["TotalCountStyle"] as Style;
            _indentTreeStyle = Resources["IndentTreeStyle"] as Style;
            _filterModeStyle = Resources["FilterModeStyle"] as Style;
            _filterTextStyle = Resources["FilterTextStyle"] as Style;
            _filterCountStyle = Resources["FilterCountStyle"] as Style;
            _propertyNameStyle = Resources["PropertyNameStyle"] as Style;
            _textPropertyStyle = Resources["TextPropertyStyle"] as Style;
            _checkPropertyStyle = Resources["CheckPropertyStyle"] as Style;
            _comboPropertyStyle = Resources["ComboPropertyStyle"] as Style;
            _modelIdentityStyle = Resources["ModelIdentityStyle"] as Style;
            _propertyBorderStyle = Resources["PropertyBorderStyle"] as Style;

            _sortModeTip = "005S".GetLocalized();
            _usageModeTip = "00ES".GetLocalized();
            _leftExpandTip = "006S".GetLocalized();
            _totalCountTip = "007S".GetLocalized();
            _filterTextTip = "008S".GetLocalized();
            _filterCountTip = "009S".GetLocalized();
            _rightExpandTip = "00AS".GetLocalized();
            _filterExpandTip = "00BS".GetLocalized();

            _itemButtons = new Button[]
            {
                ItemButton1,
                ItemButton2,
                ItemButton3
            };
            _menuItems = new MenuFlyoutItem[]
            {
                MenuItem1,
                MenuItem2,
                MenuItem3,
                MenuItem4,
                MenuItem5,
                MenuItem6,
            };

            _itemButtonTips = new ToolTip[_itemButtons.Length];
            for (int i = 0; i < _itemButtons.Length; i++)
            {
                var tip = new ToolTip();
                _itemButtonTips[i] = tip;
                ToolTipService.SetToolTip(_itemButtons[i], tip);
            }

            _menuItemTips = new ToolTip[_menuItems.Length];
            for (int i = 0; i < _menuItems.Length; i++)
            {
                var tip = new ToolTip();
                _menuItemTips[i] = tip;
                ToolTipService.SetToolTip(_menuItems[i], tip);
            }
        }

        private void _itemIdentityTip_Opened(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Button_Click  =================================================
        private void ItemButton_Click(object sender, RoutedEventArgs e)
        {
            var obj = sender as Button;
            var cmd = obj.DataContext as ModelCommand;
            cmd.Execute();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var obj = sender as MenuFlyoutItem;
            var cmd = obj.DataContext as ModelCommand;
            cmd.Execute();
        }
        #endregion

        #region TailButton  ===================================================
        private bool _isCtrlDown;
        private bool _isShiftDown;
        private void TailButton_LostFocus(object sender, RoutedEventArgs e)
        {
            // there is a rough scrollViewer in the visual tree
            // that takes the keyboard focus on pointerRelease events
            var obj = FocusManager.GetFocusedElement();
            if (obj != null && obj.GetType() == typeof(ScrollViewer))
                Focus(FocusState.Keyboard);
        }


        private void TailButton_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Shift)
            {
                _isShiftDown = false;
            }
            else if (e.Key == Windows.System.VirtualKey.Control)
            {
                _isCtrlDown = false;
            }
        }

        private void TailButton_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var mdl = TailButton.DataContext as ModelTree;
            if (mdl == null) return;
            _root.ViewSelectModel = mdl;

            if (e.Key == Windows.System.VirtualKey.Shift)
            {
                _isShiftDown = true;
            }
            else if (e.Key == Windows.System.VirtualKey.Control)
            {
                _isCtrlDown = true;
            }
            else if (e.Key == Windows.System.VirtualKey.PageDown)
            {
                ChangeScroll(_root.ViewCapacity - 2);
                SelectMidleModel();
                e.Handled = true;
            }
            else if (e.Key == Windows.System.VirtualKey.PageUp)
            {
                ChangeScroll(2 - _root.ViewCapacity);
                SelectMidleModel();
                e.Handled = true;
            }
            if (e.Key == Windows.System.VirtualKey.Down)
            {
                TryGetNextModel();
                e.Handled = true; 
            }
            else if (e.Key == Windows.System.VirtualKey.Up)
            {
                TryGetPrevModel();
                e.Handled = true;
            }
            else if (e.Key == Windows.System.VirtualKey.Left)
            {
                if (_isCtrlDown && mdl.ParentModel != null && mdl.ParentModel != _root) mdl = _root.ViewSelectModel = mdl.ParentModel;
                if (CanToggleLeft(mdl)) RefreshTree(ChangeType.ToggleLeft);
            }
            else if (e.Key == Windows.System.VirtualKey.Right)
            {
                if (mdl.CanExpandRight) RefreshTree(ChangeType.ToggleRight);
            }
            else if (e.Key == Windows.System.VirtualKey.Insert)
            {
                if (_insertCommand != null) _insertCommand.Execute();
            }
            else if (e.Key == Windows.System.VirtualKey.Delete)
            {
                if (_removeCommand != null) _removeCommand.Execute();
            }
            else if (e.KeyStatus.IsMenuKeyDown)
            {
                if (e.Key != Windows.System.VirtualKey.Menu)
                {
                    foreach (var cmd in _root.ButtonCommands)
                    {
                        if (IsFirstLetterOfCommandName(e.Key, cmd.Name))
                        {
                            cmd.Execute();
                        }
                    }
                }
            }
            else if (e.Key == Windows.System.VirtualKey.S)
            {
                if (_sortControl != null) ExecuteSort(_sortControl);
            }
            else if (e.Key == Windows.System.VirtualKey.F)
            {
                if (_filterControl != null) ExecuteFilterMode(_filterControl);
            }
            else if (e.Key == Windows.System.VirtualKey.C)
            {
                if(mdl.CanDrag) _root.SetDragDropSource(mdl);
            }
            else if (e.Key == Windows.System.VirtualKey.P)
            {
                if (_root.CanModelAcceptDrop(mdl) != DropAction.None) _root.PostModelDrop(mdl);
            }
        }

        #region VirtualKeys  ==================================================
        private bool IsFirstLetterOfCommandName(Windows.System.VirtualKey key, string commandName)
        {
            var c = char.ToUpper(commandName[0]);
            var i = _keyCodes.IndexOf(c);
            if (i < 0) return false;
            return (key == _virtualKeys[i]);
        }
        private string _keyCodes = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private Windows.System.VirtualKey[] _virtualKeys =
        {
            Windows.System.VirtualKey.A,
            Windows.System.VirtualKey.B,
            Windows.System.VirtualKey.C,
            Windows.System.VirtualKey.D,
            Windows.System.VirtualKey.E,
            Windows.System.VirtualKey.F,
            Windows.System.VirtualKey.G,
            Windows.System.VirtualKey.H,
            Windows.System.VirtualKey.I,
            Windows.System.VirtualKey.J,
            Windows.System.VirtualKey.K,
            Windows.System.VirtualKey.L,
            Windows.System.VirtualKey.M,
            Windows.System.VirtualKey.N,
            Windows.System.VirtualKey.O,
            Windows.System.VirtualKey.P,
            Windows.System.VirtualKey.Q,
            Windows.System.VirtualKey.R,
            Windows.System.VirtualKey.S,
            Windows.System.VirtualKey.T,
            Windows.System.VirtualKey.U,
            Windows.System.VirtualKey.V,
            Windows.System.VirtualKey.W,
            Windows.System.VirtualKey.X,
            Windows.System.VirtualKey.Y,
            Windows.System.VirtualKey.Z,
        };
        #endregion

        void SelectMidleModel()
        {
            if (Count == 0) return;
            var i = (int)((_root.ViewIndex1 + _root.ViewIndex2) / 2);
            _root.ViewSelectModel = _root.ViewModels[i];
            RefreshSelectionGrid();
        }
        private void TryGetPrevModel()
        {
            ValidateScroll();
            for (int i = _root.ViewIndex1; i < _root.ViewIndex2; i++)
            {
                if (_root.ViewModels[i] != _root.ViewSelectModel) continue;

                if (i > 0) _root.ViewSelectModel = _root.ViewModels[i - 1];

                if (i <= _root.ViewIndex1) ChangeScroll(-1);
                RefreshSelectionGrid();
                return;
            }
        }
        private void TryGetNextModel()
        {
            ValidateScroll();
            var N = Count - 1;
            for (int i = _root.ViewIndex1; i < _root.ViewIndex2; i++)
            {
                if (_root.ViewModels[i] != _root.ViewSelectModel) continue;

                if (i < N) _root.ViewSelectModel = _root.ViewModels[i + 1];

                if (i >= (_root.ViewIndex2 - 2)) ChangeScroll(1);
                RefreshSelectionGrid();
                return;
            }
        }

        private bool CanToggleLeft(ModelTree model)
        {
            _root.GetModelItemData(model);
            return model.CanExpandLeft;
        }
        #endregion

        #region PointerWheelChanged  ==========================================
        private void TreeCanvas_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;

            var cp = e.GetCurrentPoint(TreeCanvas);

            if (cp.Properties.MouseWheelDelta < 0)
                ChangeScroll(3);
            else
                ChangeScroll(-3);
        }
        private void ChangeScroll(int delta)
        {
            _root.ViewIndex1 += delta;
            ValidateScroll();
            RefreshVisibleModels();
        }
        private void ValidateScroll()
        {
            if (_root.ViewIndex1 < 0) _root.ViewIndex1 = 0;
            var max1 = 1 + Count -  (_root.ViewCapacity / 2);
            if (max1 < 0) max1 = 0;
            if (_root.ViewIndex1 > max1) _root.ViewIndex1 = max1;

            _root.ViewIndex2 = _root.ViewIndex1 + _root.ViewCapacity + 1;
            if (_root.ViewIndex2 > Count) _root.ViewIndex2 = Count;
        }
        #endregion

        #region ToolTip_Opened  ===============================================
        private void ItemIdentityTip_Opened(object sender, RoutedEventArgs e)
        {
            var tip = sender as ToolTip;
            var mdl = tip.DataContext as ModelTree;
            _root.GetPointerOverData(mdl);
            var content = _root.ModelSummary;
            tip.Content = string.IsNullOrWhiteSpace(content) ? null : content;
        }
        private void ModelIdentityTip_Opened(object sender, RoutedEventArgs e)
        {
            var tip = sender as ToolTip;
            var mdl = tip.DataContext as ModelTree;
            tip.Content = mdl.ModelIdentity;
        }
        #endregion

        #region MenuFlyout_Opening  ===========================================
        private void MenuFlyout_Opening(object sender, object e)
        {
            var fly = sender as MenuFlyout;
            fly.Items.Clear();
            for (int i = 0; i < _menuItemsCount; i++)
            {
                fly.Items.Add(_menuItems[i]);
            }
        }

        #endregion

        #region PointerPressed  ===============================================
        private void TreeGrid_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _previousModel = _root.ViewSelectModel = PointerModel(e);
            TailButton.Focus(FocusState.Keyboard);
            RefreshSelectionGrid();
        }
        private ModelTree PointerModel(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(TreeCanvas);
            var i = (int)(p.Position.Y / _elementHieght) + _root.ViewIndex1;

            return (Count == 0 || i < 0 || i >= Count) ? null : _root.ViewModels[i];
        }
        #endregion


        #region KeyboardFocus  ================================================
        private object _focusElement;
        private void SaveKeyboardFocus()
        {
            _focusElement = FocusManager.GetFocusedElement();
        }
        private void RestoreKeyboardFocus()
        {
            if (_focusElement == null)
                TailButton.Focus(FocusState.Keyboard);
            else
            {
                var type = _focusElement.GetType();
                if (type == typeof(Button))
                    (_focusElement as Button).Focus(FocusState.Keyboard);
                else if (type == typeof(TextBox))
                    (_focusElement as TextBox).Focus(FocusState.Keyboard);
                else if (type == typeof(CheckBox))
                    (_focusElement as CheckBox).Focus(FocusState.Keyboard);
                else if (type == typeof(ComboBox))
                    (_focusElement as ComboBox).Focus(FocusState.Keyboard);
                else
                    TailButton.Focus(FocusState.Keyboard);
            }

            _focusElement = FocusManager.GetFocusedElement();
            if (_focusElement == null)
                TailButton.Focus(FocusState.Keyboard);
        }
        #endregion

        #region Refresh  ======================================================
        public void Refresh()
        {
            if (_root.ViewModels == null) return;
            RefreshVisibleModels();
        }
        #endregion

        #region RefreshVisibleModels  =========================================
        private void RefreshVisibleModels()
        {
            if (IsLoaded == false) return;
            SaveKeyboardFocus();

            var obj = _root.ViewSelectModel;

            var cacheReset = ValidateCache();
            for (int i = 0, j = _root.ViewIndex1; j < _root.ViewIndex2; i++, j++)
            {
                var model = _root.ViewModels[j];
                _root.GetModelItemData(model);
                AddStackPanel(i, model);
            }
            RefreshSelectionGrid();
            RestoreKeyboardFocus();
        }
        #endregion

        #region RefreshSelectionGrid  =========================================
        private void RefreshSelectionGrid()
        {
            _sortControl = _filterControl = null;
            _insertCommand = _removeCommand = null;

            var select = _root.ViewSelectModel;
            if (Count == 0 || select == null)
            {
                // hide leftover buttons
                foreach (var btn in _itemButtons)
                {
                    btn.Visibility = Visibility.Collapsed;
                }
                return;
            }

            //find stackPanel index of selected model
            var index = -1;
            var N = _root.ViewCapacity + 1;
            if (N >= _cacheSize) N = _cacheSize;
            for (int i = 0; i < N; i++)
            {
                if (i >= _cacheSize) return;
                if (_stackPanelCache[i] == null) return;
                if (_stackPanelCache[i].DataContext == select)
                {
                    index = i;
                    break;
                }
            }
            if (index < 0) return;

            SelectionGrid.Width = ActualWidth;
            Canvas.SetTop(SelectionGrid, (index * _elementHieght));

            TailButton.DataContext = select;
            _root.GetModelSelectData(select);

            if (_sortModeCache[index] != null && _sortModeCache[index].DataContext != null) _sortControl = _sortModeCache[index];
            if (_filterModeCache[index] != null && _filterModeCache[index].DataContext != null) _filterControl = _filterModeCache[index]; if (select.IsFilterFocus) { select.IsFilterFocus = false; _focusElement = _filterTextCache[index]; }

            if (_root.ModelDescription != null)
            {
                HelpButton.Visibility = Visibility.Visible;
                PopulateItemHelp(_root.ModelDescription);
            }
            else
            {
                HelpButton.Visibility = Visibility.Collapsed;
            }

            var cmds = _root.ButtonCommands;
            var len1 = cmds.Count;
            var len2 = _itemButtons.Length;

            for (int i = 0; i < len2; i++)
            {
                if (i < len1)
                {
                    var cmd = cmds[i];
                    _itemButtons[i].DataContext = cmd;
                    _itemButtons[i].Content = cmd.Name;
                    _itemButtonTips[i].Content = cmd.Summary;
                    _itemButtons[i].Visibility = Visibility.Visible;
                    if (cmd.IsInsertCommand) _insertCommand = cmd;
                    if (cmd.IsRemoveCommand) _removeCommand = cmd;
                }
                else
                {
                    _itemButtons[i].Visibility = Visibility.Collapsed;
                }
            }

            cmds = _root.MenuCommands;
            if (cmds.Count == 0)
            {
                MenuButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                MenuButton.Visibility = Visibility.Visible;

                _menuItemsCount = len1 = cmds.Count;
                len2 = _menuItems.Length;

                for (int i = 0; i < len2; i++)
                {
                    if (i < len1)
                    {
                        var cmd = cmds[i];
                        _menuItems[i].DataContext = cmd;
                        _menuItems[i].Text = cmd.Name;
                        _menuItemTips[i].Content = cmd.Summary;
                    }
                }
            }
        }
        private void PopulateItemHelp(string input)
        {
            var strings = SplitOnNewLines(input);
            ItemHelp.Blocks.Clear();
            if (strings.Length == 0) return;
            var spacing = new Thickness(0, 0, 0, 6);

            foreach (var str in strings)
            {
                var run = new Run();
                run.Text = str;
                var para = new Paragraph();
                para.Inlines.Add(run);
                para.Margin = spacing;
                ItemHelp.Blocks.Add(para);
            }
        }
        private string[] SplitOnNewLines(string input)
        {
            var chars = input.ToCharArray();
            var output = new List<string>();
            var len = chars.Length;
            int j, i = 0;
            while (i < len)
            {
                if (chars[i] < ' ') { i += 1; continue; }
                for (j = i; j < len; j++)
                {
                    if (chars[j] >= ' ') continue;
                    output.Add(input.Substring(i, (j - i)));
                    i = j;
                    break;
                }
                if (i != j)
                {
                    output.Add(input.Substring(i, (len - i)));
                    break;
                }
            }
            return output.ToArray();
        }
        #endregion


        #region ItemName  =====================================================
        private void ItemName_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_previousModel == PointerModel(e))
            {
                _root.ViewSelectModel = _previousModel;
                RefreshSelectionGrid();
            }
        }
        private void ItemName_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _previousModel = PointerModel(e);
            //e.Handled = true;
        }
        private void RefreshItemName(ModelTree model, TextBlock obj)
        {
            obj.Text = _root.ModelName;
        }
        private void ItemName_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var obj = sender as TextBlock;
            _itemIdentityTip.DataContext = obj.DataContext;
        }
        private void ItemName_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            //args.DragUI.SetContentFromDataPackage();
            var obj = sender as TextBlock;
            var mdl = obj.DataContext as ModelTree;
            if (mdl.CanDrag)
                _root.SetDragDropSource(mdl);
            else
            {
                args.Cancel = true;
                _root.SetDragDropSource(null);
            }
        }
        private void ItemName_DragOver(object sender, DragEventArgs e)
        {
            e.DragUIOverride.IsContentVisible = false;
            var obj = sender as TextBlock;
            var mdl = obj.DataContext as ModelTree;

            var type = _root.CanModelAcceptDrop(mdl);
            switch (type)
            {
                case DropAction.None:
                    e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.None;
                    break;
                case DropAction.Move:
                    e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
                    break;
                case DropAction.Link:
                    e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Link;
                    break;
                case DropAction.Copy:
                    e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
                    break;
                default:
                    e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.None;
                    break;
            }
        }
        private void ItemName_Drop(object sender, DragEventArgs e)
        {
            var obj = sender as TextBlock;
            var mdl = obj.DataContext as ModelTree;
            _root.PostModelDrop(mdl);
        }
        #endregion

        #region ExpandLeft  ===================================================
        private void TextBlockHightlight_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var obj = sender as TextBlock;
            obj.Opacity = 1.0;
        }
        private void TextBlockHighlight_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var obj = sender as TextBlock;
            obj.Opacity = 0.5;
        }

        private void RefreshExpandTree(ModelTree model, TextBlock obj)
        {
            if (model.CanExpandLeft)
            {
                obj.Text = model.IsExpandedLeft ? _leftIsExtended : _leftCanExtend;
            }
            else
            {
                obj.Text = " ";
            }
        }
        private void ExpandTree_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_previousModel == PointerModel(e))
            {
                var obj = sender as TextBlock;
                _root.ViewSelectModel = obj.DataContext as ModelTree;
                RefreshTree(ChangeType.ToggleLeft);
            }
        }
        #endregion

        #region ExpandRight  ==================================================
        private void ExpandChoice_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_previousModel == PointerModel(e))
            {
                var obj = sender as TextBlock;
                _root.ViewSelectModel = obj.DataContext as ModelTree;
                RefreshTree(ChangeType.ToggleRight);
            }
        }
        #endregion

        #region ModelIdentity  ================================================
        private void ModelIdentity_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var obj = sender as TextBlock;
            _modelIdentityTip.DataContext = obj.DataContext as ModelTree;
        }
        #endregion

        #region SortMode  =====================================================
        private TextBlock _sortControl;
        private void SortMode_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_previousModel == PointerModel(e))
            {
                var obj = sender as TextBlock;
                ExecuteSort(obj);
            }
        }
        private void ExecuteSort(TextBlock obj)
        {
            if (obj == null) return;
            var mdl = obj.DataContext as ModelTree;
            if (mdl.IsSortAscending)
            {
                mdl.IsSortAscending = false;
                mdl.IsSortDescending = true;
                obj.Text = _sortDescending;
            }
            else if (mdl.IsSortDescending)
            {
                mdl.IsSortAscending = false;
                mdl.IsSortDescending = false;
                obj.Text = _sortNone;
            }
            else
            {
                mdl.IsSortAscending = true;
                obj.Text = _sortAscending;
            }
            RefreshTree(ChangeType.FilterSortChanged);
        }
        #endregion

        #region UsageMode  ====================================================
        private TextBlock _usageControl;
        private void UsageMode_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_previousModel == PointerModel(e))
            {
                var obj = sender as TextBlock;
                ExecuteUsage(obj);
            }
        }
        private void ExecuteUsage(TextBlock obj)
        {
            if (obj == null) return;
            var mdl = obj.DataContext as ModelTree;
            if (mdl.IsUsedFilter)
            {
                mdl.IsUsedFilter = false;
                mdl.IsNotUsedFilter = true;
                obj.Text = _usageIsNotUsed;
            }
            else if (mdl.IsNotUsedFilter)
            {
                mdl.IsUsedFilter = false;
                mdl.IsNotUsedFilter = false;
                obj.Text = _usageAll;
            }
            else
            {
                mdl.IsUsedFilter = true;
                obj.Text = _usageIsUsed;
            }
            RefreshTree(ChangeType.FilterSortChanged);
        }
        #endregion

        #region FilterMode  ===================================================
        private TextBlock _filterControl;
        private void FilterMode_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_previousModel == PointerModel(e))
            {
                var obj = sender as TextBlock;
                ExecuteFilterMode(obj);
            }
        }
        private void ExecuteFilterMode(TextBlock obj)
        {
            if (obj == null) return;
            var mdl = obj.DataContext as ModelTree;

            RefreshTree(ChangeType.ToggleFilter);
        }
        #endregion

        #region FilterText  ===================================================
        private void FilterText_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            var obj = sender as TextBox;
            var mdl = obj.DataContext as ModelTree;

            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Tab)
            {
                var str = obj.Text;
                if (string.IsNullOrWhiteSpace(str))
                    _root.ViewFilter.Remove(mdl);
                else
                {
                    _root.ViewFilter[mdl] = str;
                    mdl.IsExpandedLeft = true;
                }

                RefreshTree(ChangeType.FilterSortChanged);
            }
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                _root.ViewFilter.Remove(mdl);
                mdl.IsExpandedFilter = false;

                RefreshTree(ChangeType.FilterSortChanged);
            }
        }
        #endregion

        #region TextProperty  =================================================
        private void TextProperty_LostFocus(object sender, RoutedEventArgs e)
        {
            var obj = sender as TextBox;
            var mdl = obj.DataContext as ModelTree;
            if ((string)obj.Tag != obj.Text)
                _root.PostSetValue(mdl, obj.Text);
        }
        private void TextProperty_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Tab)
            {
                var obj = sender as TextBox;
                var mdl = obj.DataContext as ModelTree;
                if ((string)obj.Tag != obj.Text)
                    _root.PostSetValue(mdl, obj.Text);
            }
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                var obj = sender as TextBox;
                var mdl = obj.DataContext as ModelTree;
                if ((string)obj.Tag != obj.Text)
                    _root.GetModelItemData(mdl);
                obj.Text = (_root.ModelValue == null) ? string.Empty : _root.ModelValue;
            }

        }
        #endregion

        #region CheckProperty  ================================================
        private void Check_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            var obj = sender as CheckBox;
            var mdl = obj.DataContext as ModelTree;
            var val = obj.IsChecked.HasValue ? obj.IsChecked.Value : false;
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                _ignoreNextCheckBoxEvent = true;
                _root.PostSetIsChecked(mdl, !val);
            }
        }
        private bool _ignoreNextCheckBoxEvent;
        private void CheckProperty_Checked(object sender, RoutedEventArgs e)
        {
            if (_ignoreNextCheckBoxEvent)
            {
                _ignoreNextCheckBoxEvent = false;
            }
            else
            {
                var obj = sender as CheckBox;
                var mdl = obj.DataContext as ModelTree;
                var val = obj.IsChecked.HasValue ? obj.IsChecked.Value : false;
                _root.PostSetIsChecked(mdl, val);
            }
        }
        #endregion

        #region ComboProperty  ================================================
        private void ComboProperty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var obj = sender as ComboBox;
            var mdl = obj.DataContext as ModelTree;
            _root.PostSetValueIndex(mdl, obj.SelectedIndex);
        }
        #endregion

        #region PropertyBorder  ===============================================
        private void PropertyBorder_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var obj = sender as Border;
            _itemIdentityTip.DataContext = obj.DataContext as ModelTree;
        }
        #endregion

        async Task RefreshTree(ChangeType change)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                if (_root.Chef.ValidateModelTree(_root, change)) Refresh();
            });
        }
    }
}
