// <copyright file="Bootstrapper.cs" company="Kristof Solbrig - HNE">
// Copyright (c) Kristof Solbrig - HNE. All rights reserved.
// </copyright>

using Caliburn.Micro;
using HNEAutoInstaller.ViewModels;
using System;
using System.Windows;

namespace HNEAutoInstaller
{
    /// <summary>
    /// Caliburn.Micro class for entry-point of app.
    /// </summary>
    public class Bootstrapper : BootstrapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// </summary>
        public Bootstrapper()
        {
            this.Initialize();
        }

        /// <summary>
        /// OnStartup, define what view model to use, when app starts.
        /// </summary>
        /// <param name="sender"> Object sender. </param>
        /// <param name="e"> Startup event arguments. </param>
        protected override void OnStartup(Object sender, StartupEventArgs e)
        {
            this.DisplayRootViewFor<ShellViewModel>();
        }
    }
}