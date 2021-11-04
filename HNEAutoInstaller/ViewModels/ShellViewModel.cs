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
        private String _selectedSvmPresets = "Standard";

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
        /// Gets preset names.
        /// </summary>
        public BindableCollection<String> SvmPresets
        {
            get
            {
                FileHandler fileHandler = Singleton<FileHandler>.Instance;
                return new BindableCollection<String>(fileHandler.FetchPresetNames());
            }
        }

        /// <summary>
        /// Gets or Sets preset IDs.
        /// </summary>
        public String SelectedSvmPresets
        {
            get
            {
                return this._selectedSvmPresets;
            }

            set
            {
                this._selectedSvmPresets = value;
                this.NotifyOfPropertyChange(() => this.SelectedSvmPresets);
            }
        }

        /// <summary>
        /// Call function for installing all presets files from folder.
        /// </summary>
        public void SvmInstallButton()
        {
            FileHandler fileHandler = Singleton<FileHandler>.Instance;
            Int64 tempInt = fileHandler.FetchPresetID(this._selectedSvmPresets);

            List<String> templist = fileHandler.FetchPresetFiles((Int32)tempInt);

            fileHandler.InstallAllFiles(templist, this._selectedSvmPresets);
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