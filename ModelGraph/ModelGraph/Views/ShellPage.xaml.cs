using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ModelGraph.Controls;
using ModelGraph.Helpers;
using ModelGraph.Services;
using ModelGraphSTD;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ModelGraph.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page, INotifyPropertyChanged
    {
        private NavigationViewItem _selected;

        public NavigationViewItem Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ShellPage()
        {
            InitializeComponent();
            HideNavViewBackButton();
            DataContext = this;
            Initialize();
        }

        private void Initialize()
        {
            NavigationService.Frame = shellFrame;
            NavigationService.Navigated += Frame_Navigated;
            ModelPageService.InsertModelPage = InsertModelPage;
            ModelPageService.RemoveModelPage = RemoveModelPage;
            KeyboardAccelerators.Add(ActivationService.AltLeftKeyboardAccelerator);
            KeyboardAccelerators.Add(ActivationService.BackKeyboardAccelerator);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = navigationView.SettingsItem as NavigationViewItem;
                return;
            }

            Selected = navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .FirstOrDefault(menuItem => IsMenuItemForPageType(menuItem, e.SourcePageType, e.Parameter));
        }

        private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType, object parameter)
        {
            if (sourcePageType == typeof(ModelPage) && menuItem.Tag == parameter)
                return true;

            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        #region ModelPageService  =============================================
        //
        #region InsertModelPage  ==============================================
        public void InsertModelPage(RootModel model)
        {
            model.Chef.SetLocalizer(Helpers.ResourceExtensions.GetLocalizer());

            var item = navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .FirstOrDefault(menuItem => (menuItem.Name == "Home"));

            if (item is null) return;

            var index = navigationView.MenuItems.IndexOf(item) + 1;
            var navItem = new NavigationViewItem
            {
                Content = model.TitleName,
                Icon = new SymbolIcon(Symbol.AllApps),
                Tag = model
            };
            ToolTipService.SetToolTip(navItem, model.TitleSummary);
            
            navItem.Loaded += NavItem_Loaded;
            navigationView.MenuItems.Insert(index, navItem);

            Selected = navItem;
            NavigationService.Navigate(typeof(ModelPage), model);
        }

        private static void NavItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is NavigationViewItem navItem)
            {
                navItem.Loaded -= NavItem_Loaded;
                navItem.IsSelected = true;                
            }
        }
        #endregion
        //
        #region RemoveModelPage  ==============================================
        public void RemoveModelPage(RootModel model)
        {
            var item = navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .FirstOrDefault(menuItem => (menuItem.Tag == model));

            if (item is null) return;
            navigationView.MenuItems.Remove(item);

            var home = navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .FirstOrDefault(menuItem => (menuItem.Name == "Home"));

            if (!(home is null))
            {
                home.IsSelected = true;                
                NavigationService.Navigate(typeof(MainPage));
            }

        }
        #endregion
        #endregion

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.Navigate(typeof(SettingsPage));
                return;
            }

            var item = navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .First(menuItem => (string)menuItem.Content == (string)args.InvokedItem);


            if (item.Tag is IModelControl pageControl)
            {
                NavigationService.Navigate(typeof(ModelPage), pageControl);
            }
            else
            {
                var pageType = item.GetValue(NavHelper.NavigateToProperty) as Type;
                NavigationService.Navigate(pageType);
            }
        }

        private void HideNavViewBackButton()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                navigationView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
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
    }
}
