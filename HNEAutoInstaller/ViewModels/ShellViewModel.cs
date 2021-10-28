// <copyright file="ShellViewModel.cs" company="Kristof Solbrig - HNE">
// Copyright (c) Kristof Solbrig - HNE. All rights reserved.
// </copyright>

using Caliburn.Micro;
using HNEAutoInstaller.Models;
using Narumikazuchi;
using System;
using System.Collections.Generic;

namespace HNEAutoInstaller.ViewModels
{
    /// <summary>
    /// Caliburn.Micro main view model.
    /// </summary>
    public class ShellViewModel : Conductor<Object>
    {
        private BindableCollection<String> _svmFilePreset = new() { "Standard", "GIS" };

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel()
        {
            FileHandler fileHandler = Singleton<FileHandler>.Instance;
            fileHandler.CreateFolders();
            this.SvmReload();
        }

        /// <summary>
        /// Gets or sets presets.
        /// </summary>
        public BindableCollection<String> SvmFilesPreset
        {
            get
            {
                return this._svmFilePreset;
            }

            set
            {
                this._svmFilePreset = value;
                this.NotifyOfPropertyChange(() => this.SvmFilesPreset);
            }
        }

        /// <summary>
        /// Call function for installing all files from folder.
        /// </summary>
        public static void SvmInstallButton()
        {
            FileHandler fileHandler = Singleton<FileHandler>.Instance;

            List<String> templist = fileHandler.FetchPresetFiles(1);

            fileHandler.InstallAllFiles(templist);
        }

        /// <summary>
        /// Activate child view in main view.
        /// </summary>
        public void SvmReload()
        {
            this.ActivateItemAsync(new ActiveChildViewModel());
        }
    }
}