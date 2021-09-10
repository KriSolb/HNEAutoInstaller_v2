using Caliburn.Micro;
using HNEAutoInstaller.ViewModels;
using System;
using System.Windows;

namespace HNEAutoInstaller
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            this.Initialize();
        }

        protected override void OnStartup(Object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}