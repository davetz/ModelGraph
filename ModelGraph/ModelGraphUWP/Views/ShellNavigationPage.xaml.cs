﻿using ModelGraphSTD;
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
    public sealed partial class ShellNavigationPage : Page
    {
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

        #region UpdateNavigationPane  =========================================
        internal void UpdateNavigationPane(ModelRoot root, ModelRoot compare, List<ModelRoot> modelList, ModelRoot select)
        {
            object insertionPoint = null;
            var hitList = new List<NavigationViewItem>();
            var canCompare = (modelList.Count < 2) ? false : true;

            foreach (var item in NavView.MenuItems)
            {
                if (item is NavigationViewItem n)
                {
                    var model = n.Tag as ModelRoot;
                    if (n.Name == "Home")
                    {
                        n.Tag = root;
                    }
                    else if (n.Name == "Compare")
                    {
                        n.Tag = compare;
                        n.IsEnabled = canCompare;                              
                    }
                    else if (!modelList.Contains(model))
                    {
                        hitList.Add(n);
                    }
                    else
                    {
                        modelList.Remove(model);
                    }
                }
                else if (item is NavigationViewItemSeparator s && s.Name == "InsertionPoint")
                {
                    insertionPoint = item;
                }
            }

            foreach (var item in hitList)
            {
                NavView.MenuItems.Remove(item);
            }

            var i = NavView.MenuItems.IndexOf(insertionPoint);
            if (i > 0)
            {
                foreach (var model in modelList)
                {
                    NavView.MenuItems.Insert(i++, new NavigationViewItem()
                    { Content = model.TitleName, Icon = new SymbolIcon(Symbol.AllApps), Tag = model });
                }
            }
            else
            {
                foreach (var model in modelList)
                {
                    NavView.MenuItems.Add(new NavigationViewItem()
                    { Content = model.TitleName, Icon = new SymbolIcon(Symbol.AllApps), Tag = model });
                }
            }

            foreach (var item in NavView.MenuItems)
            {
                NavView.SelectedItem = null;
                if (item is NavigationViewItem n)
                {
                    if (n.Tag != select) continue;

                    if (modelList.Contains(n.Tag))
                        n.Loaded += NewNavItem_Loaded;  // for new navItems
                    else
                        n.IsSelected = true;            // for existing navItems

                    break;
                }
            }
        }
        private void NewNavItem_Loaded(object sender, RoutedEventArgs e)
        {/*
            wait for navItem to load before setting the IsSelected property
         */
            var n = sender as NavigationViewItem;
            n.Loaded -= NewNavItem_Loaded;
            n.IsSelected = true;
        }
        #endregion

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            ModelPageService.Current.RegesterNavigaionPaneUpdateMethod(UpdateNavigationPane);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
        }

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
    }
}
