﻿using ModelGraph.Services;
using ModelGraphSTD;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

namespace ModelGraph.Views
{
    public sealed partial class HomePage : Page, INotifyPropertyChanged
    {
        public HomePage()
        {
            InitializeComponent();
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

        private void NewButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var root = new RootModel();
            var ctrl = new ModelPageControl(root);
            ModelPageService.InsertModelPage(ctrl);
        }
    }
}
