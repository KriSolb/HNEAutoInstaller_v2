// <copyright file="ShellViewModel.cs" company="Kristof Solbrig - HNE">
// Copyright (c) Kristof Solbrig - HNE. All rights reserved.
// </copyright>

using Caliburn.Micro;
using HNEAutoInstaller.Models;
using System;

namespace HNEAutoInstaller.ViewModels
{
    /// <summary>
    /// Caliburn.Micro main view model.
    /// </summary>
    public class ShellViewModel : Conductor<Object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel()
        {
            FileHandler.CreateFolders();
        }
    }
}