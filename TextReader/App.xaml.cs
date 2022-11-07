﻿using System.Windows;
using Prism.Ioc;
using TextReader.ViewModels;
using TextReader.Views;

namespace TextReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<SettingWindow, SettingWindowViewModel>();
        }
    }
}