using System;
using Windows.UI.Xaml.Controls;
using ModelGraphSTD;

namespace ModelGraphUWP
{
    public sealed partial class ModelTreeControl
    {
        static int initialSize = 40; // visible lines on the screen
        int _cacheSize = initialSize;

        TextBlock[] _itemKindCache = new TextBlock[initialSize];
        TextBlock[] _itemNameCache = new TextBlock[initialSize];
        TextBlock[] _itemInfoCache = new TextBlock[initialSize];
        TextBlock[] _totalCountCache = new TextBlock[initialSize];
        TextBlock[] _indentTreeCache = new TextBlock[initialSize];
        TextBlock[] _expandLeftCache = new TextBlock[initialSize];
        TextBlock[] _expandRightCache = new TextBlock[initialSize];
        TextBlock[] _sortModeCache = new TextBlock[initialSize];
        TextBlock[] _usageModeCache = new TextBlock[initialSize];
        TextBlock[] _filterModeCache = new TextBlock[initialSize];
        TextBlock[] _filterCountCache = new TextBlock[initialSize];
        TextBlock[] _propertyNameCache = new TextBlock[initialSize];
        TextBlock[] _modelIdentityCache = new TextBlock[initialSize];

        TextBox[] _filterTextCache = new TextBox[initialSize];
        TextBox[] _textPropertyCache = new TextBox[initialSize];
        CheckBox[] _checkPropertyCache = new CheckBox[initialSize];
        ComboBox[] _comboPropertyCache = new ComboBox[initialSize];
        Border[] _propertyBorderCache = new Border[initialSize];
        StackPanel[] _stackPanelCache = new StackPanel[initialSize];

        #region ValidateCache  ================================================
        void ValidateCache(int length)
        {
            ValidateCacheSize();

            var hidden = -8 * _elementHieght;
            var N = _stackPanelCache.Length;

            for (int i = 0; i < N; i++)
            {
                var sp = _stackPanelCache[i];
                if (sp == null) return;         // end of cache
                if (i < length) continue;       // visible element

                sp.Children.Clear();
            }

            #region ValidateCacheSize  ============================================
            void ValidateCacheSize()
            {
                if (length < _cacheSize) return;

                var size = length + 30; // new size of the cache

                _itemKindCache = ExpandTextBlockCache(_itemKindCache);
                _itemNameCache = ExpandTextBlockCache(_itemNameCache);
                _itemInfoCache = ExpandTextBlockCache(_itemInfoCache);
                _totalCountCache = ExpandTextBlockCache(_totalCountCache);
                _indentTreeCache = ExpandTextBlockCache(_indentTreeCache);
                _expandLeftCache = ExpandTextBlockCache(_expandLeftCache);
                _expandRightCache = ExpandTextBlockCache(_expandRightCache);
                _sortModeCache = ExpandTextBlockCache(_sortModeCache);
                _usageModeCache = ExpandTextBlockCache(_usageModeCache);
                _filterModeCache = ExpandTextBlockCache(_filterModeCache);
                _filterCountCache = ExpandTextBlockCache(_filterCountCache);
                _propertyNameCache = ExpandTextBlockCache(_propertyNameCache);
                _modelIdentityCache = ExpandTextBlockCache(_modelIdentityCache);

                _filterTextCache = ExpandTextBoxCache(_filterTextCache);
                _textPropertyCache = ExpandTextBoxCache(_textPropertyCache);
                _checkPropertyCache = ExpandCheckBoxCache(_checkPropertyCache);
                _comboPropertyCache = ExpandComboBoxCache(_comboPropertyCache);
                _propertyBorderCache = ExpandBorderCache(_propertyBorderCache);
                _stackPanelCache = ExpandStackPanelCache(_stackPanelCache);

                _cacheSize = size;  //update the size
                return;

                #region ExpandElementArray   ==================================
                TextBlock[] ExpandTextBlockCache(TextBlock[] cache)
                {
                    var oldCache = cache;
                    var newCache = new TextBlock[size];
                    Array.Copy(oldCache, newCache, oldCache.Length);
                    return newCache;
                }
                TextBox[] ExpandTextBoxCache(TextBox[] cache)
                {
                    var oldCache = cache;
                    var newCache = new TextBox[size];
                    Array.Copy(oldCache, newCache, oldCache.Length);
                    return newCache;
                }
                CheckBox[] ExpandCheckBoxCache(CheckBox[] cache)
                {
                    var oldCache = cache;
                    var newCache = new CheckBox[size];
                    Array.Copy(oldCache, newCache, oldCache.Length);
                    return newCache;
                }
                ComboBox[] ExpandComboBoxCache(ComboBox[] cache)
                {
                    var oldCache = cache;
                    var newCache = new ComboBox[size];
                    Array.Copy(oldCache, newCache, oldCache.Length);
                    return newCache;
                }
                Border[] ExpandBorderCache(Border[] cache)
                {
                    var oldCache = cache;
                    var newCache = new Border[size];
                    Array.Copy(oldCache, newCache, oldCache.Length);
                    return newCache;
                }
                StackPanel[] ExpandStackPanelCache(StackPanel[] cache)
                {
                    var oldCache = cache;
                    var newCache = new StackPanel[size];
                    Array.Copy(oldCache, newCache, oldCache.Length);
                    return newCache;
                }
                #endregion
            }
            #endregion
        }

        #endregion


        #region AddItemKind  ==================================================
        void AddItemKind(int index, string kind, ItemModel model)
        {
            var obj = _itemKindCache[index];
            if (obj == null)
            {
                obj = _itemKindCache[index] = new TextBlock();

                obj.Style = _itemKindStyle;
                obj.DragStarting += ItemName_DragStarting;
                obj.PointerPressed += ItemName_PointerPressed;
                obj.PointerReleased += ItemName_PointerReleased;
                obj.DragOver += ItemName_DragOver;
                obj.Drop += ItemName_Drop;
                obj.PointerEntered += ItemName_PointerEntered;
                ToolTipService.SetToolTip(obj, _itemIdentityTip);
            }

            obj.Text = kind;
            obj.CanDrag = model.CanDrag;
            obj.AllowDrop = true;
            obj.DataContext = model;

            _stackPanelCache[index].Children.Add(obj);
        }
        #endregion

        #region AddItemName  ==================================================
        private void AddItemName(int index, string name, ItemModel model)
        {
            var obj = _itemNameCache[index];
            if (obj == null)
            {
                obj = _itemNameCache[index] = new TextBlock();;

                obj.Style = _itemNameStyle;
                obj.DragStarting += ItemName_DragStarting;
                obj.PointerPressed += ItemName_PointerPressed;
                obj.PointerReleased += ItemName_PointerReleased;
                obj.DragOver += ItemName_DragOver;
                obj.Drop += ItemName_Drop;
                obj.PointerEntered += ItemName_PointerEntered;
                ToolTipService.SetToolTip(obj, _itemIdentityTip);
            }

            obj.Text = name;
            obj.CanDrag = model.CanDrag;
            obj.AllowDrop = true;
            obj.DataContext = model;

            _stackPanelCache[index].Children.Add(obj);
        }
        #endregion

        #region AddItemInfo  ==================================================
        private void AddItemInfo(int index, ItemModel model)
        {
            var obj = _itemInfoCache[index];
            if (obj == null)
            {
                obj = _itemInfoCache[index] = new TextBlock();

                obj.Style = _itemInfoStyle;
            }

            obj.Text = _root.ModelInfo;
            obj.DataContext = model;

            _stackPanelCache[index].Children.Add(obj);
        }
        #endregion

        #region AddTotalCount  ================================================
        void AddTotalCount(int index, int count, ItemModel model)
        {
            var obj = _totalCountCache[index];
            if (obj == null)
            {
                obj = _totalCountCache[index] = new TextBlock();

                obj.Style = _totalCountStyle;
                ToolTipService.SetToolTip(obj, _totalCountTip);
            }

            obj.Text = count.ToString();

            _stackPanelCache[index].Children.Add(obj);
        }
        #endregion

        #region AddTreeIndent  ================================================
        private void AddTreeIndent(int index, ItemModel model)
        {
            var obj = _indentTreeCache[index];
            if (obj == null)
            {
                obj = _indentTreeCache[index] = new TextBlock();

                obj.Style = _indentTreeStyle;
                obj.PointerReleased += ExpandTree_PointerReleased;
            }

            obj.Text = " ";
            obj.DataContext = model;
            obj.MinWidth = model.Depth * _levelIndent;

            _stackPanelCache[index].Children.Add(obj);
        }
        #endregion

        #region AddExpandLeft  ================================================
        private void AddExpandLeft(int index, ItemModel model)
        {
            var obj = _expandLeftCache[index];
            if (obj == null)
            {
                obj = _expandLeftCache[index] = new TextBlock();

                obj.Style = _expanderStyle;
                obj.PointerExited += TextBlockHightlight_PointerExited;
                obj.PointerEntered += TextBlockHighlight_PointerEntered;
                obj.PointerReleased += ExpandTree_PointerReleased;
                ToolTipService.SetToolTip(obj, _leftExpandTip);
            }

            if (model.CanExpandLeft)
            {
                obj.Text = model.IsExpandedLeft ? _leftIsExtended : _leftCanExtend;
            }
            else
            {
                obj.Text = " ";
            }

            obj.DataContext = model;

            _stackPanelCache[index].Children.Add(obj);
        }

        #endregion

        #region AddExpandRight  ===============================================
        private void AddExpandRight(int index, ItemModel model)
        {
            var obj = _expandRightCache[index];
            if (obj == null)
            {
                obj = _expandRightCache[index] = new TextBlock();

                obj.Style = _expanderStyle;
                obj.PointerExited += TextBlockHightlight_PointerExited;
                obj.PointerEntered += TextBlockHighlight_PointerEntered;
                obj.PointerReleased += ExpandChoice_PointerReleased;
                ToolTipService.SetToolTip(obj, _rightExpandTip);
            }

            obj.Text = model.IsExpandedRight ? _rightIsExtended : _rightCanExtend;
            obj.DataContext = model;

            _stackPanelCache[index].Children.Add(obj);
        }
        #endregion


        #region AddSortMode  ==================================================
        private void AddSortMode(int index, ItemModel model, bool canSort)
        {
            var obj = _sortModeCache[index];
            if (canSort)
            {
                if (obj == null)
                {
                    obj = _sortModeCache[index] = new TextBlock();

                    obj.Style = _sortModeStyle;
                    obj.PointerExited += TextBlockHightlight_PointerExited;
                    obj.PointerEntered += TextBlockHighlight_PointerEntered;
                    obj.PointerReleased += SortMode_PointerReleased;
                    ToolTipService.SetToolTip(obj, _sortModeTip);
                }
                obj.DataContext = model;
                obj.Text = model.IsSortAscending ?
                    _sortAscending : (model.IsSortDescending ? _sortDescending : _sortNone);

                _stackPanelCache[index].Children.Add(obj);
            }
            else if (obj != null)
            {
                obj.DataContext = null; // needed for "S" keyboard shortcut (TailButton)
            }
        }
        #endregion

        #region AddUsageMode  ==================================================
        private void AddUsageMode(int index, ItemModel model, bool canFilterUsage)
        {
            var obj = _usageModeCache[index];
            if (canFilterUsage)
            {
                if (obj == null)
                {
                    obj = _usageModeCache[index] = new TextBlock();

                    obj.Style = _usageModeStyle;
                    obj.PointerExited += TextBlockHightlight_PointerExited;
                    obj.PointerEntered += TextBlockHighlight_PointerEntered;
                    obj.PointerReleased += UsageMode_PointerReleased;
                    ToolTipService.SetToolTip(obj, _usageModeTip);
                }
                obj.DataContext = model;
                obj.Text = model.IsUsedFilter ?
                    _usageIsUsed : (model.IsNotUsedFilter ? _usageIsNotUsed : _usageAll);

                _stackPanelCache[index].Children.Add(obj);
            }
            else if (obj != null)
            {
                obj.DataContext = null; // needed for "U" keyboard shortcut (TailButton)
            }
        }
        #endregion

        #region AddFilterMode  ================================================
        private void AddFilterMode(int index, ItemModel model, bool canFilter)
        {
            var obj = _filterModeCache[index];
            if (canFilter)
            {
                if (obj == null)
                {
                    obj = _filterModeCache[index] = new TextBlock();

                    obj.Style = _filterModeStyle;
                    obj.PointerExited += TextBlockHightlight_PointerExited;
                    obj.PointerEntered += TextBlockHighlight_PointerEntered;
                    obj.PointerReleased += FilterMode_PointerReleased;
                    ToolTipService.SetToolTip(obj, _filterExpandTip);
                }

                obj.DataContext = model;
                obj.Text = model.IsExpandedFilter ? _filterIsShowing : _filterCanShow;

                _stackPanelCache[index].Children.Add(obj);
            }
            else if (obj != null)
            {
                obj.DataContext = null; // needed for "F" keyboard shortcut (TailButton)
            }
        }
        #endregion

        #region AddFilterText  ================================================
        private void AddFilterText(int index, ItemModel model)
        {
            var obj = _filterTextCache[index];
            if (obj == null)
            {
                obj = _filterTextCache[index] = new TextBox();

                obj.Style = _filterTextStyle;
                obj.KeyDown += FilterText_KeyDown;
                ToolTipService.SetToolTip(obj, _filterTextTip);
            }

            obj.DataContext = model;
            var str = string.IsNullOrWhiteSpace(model.ViewFilter) ? string.Empty : model.ViewFilter;
            obj.Text = str;
            obj.Tag = str; //save an initial (unmodified) version of the view filter text

            _stackPanelCache[index].Children.Add(obj);
        }
        #endregion

        #region AddFilterCount  ===============================================
        private void AddFilterCount(int index, ItemModel model)
        {
            var obj = _filterCountCache[index];
            if (obj == null)
            {
                obj = _filterCountCache[index] = new TextBlock();

                obj.Style = _filterCountStyle;
                ToolTipService.SetToolTip(obj, _filterCountTip);
            }

            obj.Text = model.FilterCount.ToString();

            _stackPanelCache[index].Children.Add(obj);
        }
        #endregion


        #region AddPropertyName  ==============================================
        private void AddPropertyName(int index, string name, ItemModel model)
        {
            var obj = _propertyNameCache[index];
            var bdr = _propertyBorderCache[index];
            if (obj == null)
            {
                obj = _propertyNameCache[index] = new TextBlock();
                bdr = _propertyBorderCache[index] = new Border();

                bdr.Style = _propertyBorderStyle;
                bdr.PointerEntered += PropertyBorder_PointerEntered;
                ToolTipService.SetToolTip(bdr, _itemIdentityTip);

                obj.Style = _propertyNameStyle;
                obj.PointerEntered += ItemName_PointerEntered;
                ToolTipService.SetToolTip(obj, _itemIdentityTip);

                bdr.Child = obj;
            }

            bdr.DataContext = model;
            obj.DataContext = model;
            obj.Text = name;

            _stackPanelCache[index].Children.Add(bdr);
        }
        #endregion

        #region AddTextProperty  ==============================================
        private void AddTextProperty(int index, ItemModel model)
        {
            var obj = _textPropertyCache[index];
            if (obj == null)
            {
                obj = _textPropertyCache[index] = new TextBox();

                obj.Style = _textPropertyStyle;
                obj.KeyDown += TextProperty_KeyDown;
                obj.LostFocus += TextProperty_LostFocus;
            }

            obj.DataContext = model;
            var txt = model.TextValue;
            obj.Text = txt ?? string.Empty;
            obj.Tag = obj.Text;
            obj.IsReadOnly = model.IsReadOnly;

            _stackPanelCache[index].Children.Add(obj);
        }
        #endregion

        #region AddCheckProperty  =============================================
        private void AddCheckProperty(int index, ItemModel model)
        {
            var obj = _checkPropertyCache[index];
            if (obj == null)
            {
                obj = _checkPropertyCache[index] = new CheckBox();

                obj.Style = _checkPropertyStyle;
                obj.Checked += CheckProperty_Checked;
                obj.Unchecked += CheckProperty_Checked;
                obj.KeyDown += Check_KeyDown;
            }

            obj.DataContext = model;
            obj.IsChecked = model.BoolValue;

            _stackPanelCache[index].Children.Add(obj);
        }
        #endregion

        #region AddComboProperty  =============================================
        private void AddComboProperty(int index, ItemModel model)
        {
            var obj = _comboPropertyCache[index];
            if (obj == null)
            {
                obj = _comboPropertyCache[index] = new ComboBox();

                obj.Style = _comboPropertyStyle;
                obj.SelectionChanged += ComboProperty_SelectionChanged;
                obj.KeyDown += ComboProperty_KeyDown;
            }

            obj.DataContext = model;
            obj.ItemsSource = model.ListValue;
            obj.SelectedIndex = model.IndexValue;

            _stackPanelCache[index].Children.Add(obj);
        }
        #endregion


        #region AddModelIdentity  =============================================
        private void AddModelIdentity(int index, ItemModel model)
        {
            var obj = _modelIdentityCache[index];
            if (obj == null)
            {
                obj = _modelIdentityCache[index] = new TextBlock();

                obj.Style = _modelIdentityStyle;
                obj.PointerEntered += ModelIdentity_PointerEntered;
                ToolTipService.SetToolTip(obj, _modelIdentityTip);
            }

            obj.DataContext = model;

            _stackPanelCache[index].Children.Add(obj);
        }
        #endregion


        #region AddStackPanel  ================================================
        private void AddStackPanel(int index, ItemModel m)
        {
            var sp = _stackPanelCache[index];
            if (sp == null)
            {
                sp = _stackPanelCache[index] = new StackPanel();

                sp.MaxHeight = _elementHieght;
                sp.Orientation = Windows.UI.Xaml.Controls.Orientation.Horizontal;

                TreeCanvas.Children.Add(sp);
                Canvas.SetTop(sp, index * _elementHieght);
            }

            sp.Children.Clear();
            sp.DataContext = m;

            var (kind, name, count, type) = m.ModelParms;

            AddModelIdentity(index, m);
            AddTreeIndent(index, m);
            AddExpandLeft(index, m);

            switch (type)
            {
                case ModelType.TextProperty:
                    //=========================================================
                    AddPropertyName(index, name, m);
                    AddTextProperty(index, m);
                    break;

                case ModelType.CheckProperty:
                    //=========================================================
                    AddPropertyName(index, name, m);
                    AddCheckProperty(index, m);
                    break;

                case ModelType.ComboProperty:
                    //=========================================================
                    AddPropertyName(index, name, m);
                    AddComboProperty(index, m);
                    break;

                default:
                    //=========================================================
                    AddItemKind(index, kind, m);
                    AddItemName(index, name, m);
                    if (m.CanExpandRight)
                    {
                        AddExpandRight(index, m);
                    }

                    if (count > 0)
                    {
                        AddSortMode(index, m, (m.CanSort));

                        AddTotalCount(index, count, m);
                        AddUsageMode(index, m, (m.CanFilterUsage));
                        AddFilterMode(index, m, m.CanFilter);

                        if (m.CanFilter)
                        {
                            if (m.IsExpandedFilter)
                            {
                                AddFilterText(index, m);
                                AddFilterCount(index, m);
                            }
                        }
                        if (_root.ModelInfo != null)
                        {
                            AddItemInfo(index, m);
                        }
                    }
                    break;
            }
        }
        #endregion
    }

}
