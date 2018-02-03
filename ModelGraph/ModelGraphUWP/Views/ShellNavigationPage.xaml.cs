using ModelGraphSTD;
using ModelGraphUWP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ModelGraphUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellNavigationPage : Page, INotifyPropertyChanged
    {
        private object _lastSelectedItem;

        private ObservableCollection<ShellNavigationItem> _primaryItems = new ObservableCollection<ShellNavigationItem>();

        public ObservableCollection<ShellNavigationItem> PrimaryItems
        {
            get { return _primaryItems; }
            set { Set(ref _primaryItems, value); }
        }

        private ObservableCollection<ShellNavigationItem> _secondaryItems = new ObservableCollection<ShellNavigationItem>();

        public ObservableCollection<ShellNavigationItem> SecondaryItems
        {
            get { return _secondaryItems; }
            set { Set(ref _secondaryItems, value); }
        }
        private object _selected;

        public object Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ShellNavigationPage()
        {
            this.InitializeComponent();
            Initialize();
        }
        void Initialize()
        {
            NavigationService.Frame = ContentFrame;
            NavigationService.Navigated += Frame_Navigated;
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
        }

        internal void UpdateNavigationPane(ModelRoot root, List<ModelRoot> models)
        {
            var hitList = new List<NavigationViewItem>();
            var first = true;
            foreach (var item in NavView.MenuItems)
            {
                if (item is NavigationViewItem n)
                {
                    if (first)
                    {
                        first = false;
                        n.Tag = root;
                    }
                    else if (n.Tag is ModelRoot)
                    {
                        hitList.Add(n);
                    }
                }
            }
            foreach (var item in hitList)
            {
                NavView.MenuItems.Remove(item);
            }
            foreach (var model in models)
            {
                NavView.MenuItems.Add(new NavigationViewItem()
                { Content = model.TitleName, Icon = new SymbolIcon(Symbol.AllApps), Tag = model });
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            ModelPageService.Current.RegesterNavigaionPaneUpdateMethod(UpdateNavigationPane);
        }

        private void PopulateNavItems()
        {
            _primaryItems.Clear();
            _secondaryItems.Clear();
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            var navigationItem = PrimaryItems?.FirstOrDefault(i => i.PageType == e?.SourcePageType);
            if (navigationItem == null)
            {
                navigationItem = SecondaryItems?.FirstOrDefault(i => i.PageType == e?.SourcePageType);
            }

            if (navigationItem != null)
            {
                ChangeSelected(_lastSelectedItem, navigationItem);
                _lastSelectedItem = navigationItem;
            }
        }

        private void ChangeSelected(object oldValue, object newValue)
        {
            if (oldValue != null)
            {
                (oldValue as ShellNavigationItem).IsSelected = false;
            }

            if (newValue != null)
            {
                (newValue as ShellNavigationItem).IsSelected = true;
                Selected = newValue;
            }
        }

        private void Navigate(object item)
        {
            var navigationItem = item as ShellNavigationItem;
            if (navigationItem != null)
            {
                NavigationService.Navigate(navigationItem.PageType);
            }
        }

        //private void ItemInvoked(object sender, HamburgetMenuItemInvokedEventArgs e)
        //{
        //    if (DisplayMode == SplitViewDisplayMode.CompactOverlay || DisplayMode == SplitViewDisplayMode.Overlay)
        //    {
        //        IsPaneOpen = false;
        //    }

        //    Navigate(e.InvokedItem);
        //}

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                //ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                switch (args.InvokedItem)
                {
                }
            }
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                //ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                var item = args.SelectedItem as NavigationViewItem;
                if (item == null) return;

                var model = item.Tag as ModelRoot;
                if (model == null) return;

                ModelPageService.Current.ShowModelPage(model);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var val = await Services.WindowManagerService.Current.TryShowAsViewModeAsync("BlankPage", typeof(Views.BlankPage), ApplicationViewMode.CompactOverlay);
            val.Released += Val_Released;
        }

        private void Val_Released(object sender, EventArgs e)
        {
            var donothing = true;
        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            var val = await Services.WindowManagerService.Current.TryShowAsViewModeAsync("BlankPage", typeof(Views.BlankPage), ApplicationViewMode.Default);
            val.Released += Val_Released;
        }
    }
}
