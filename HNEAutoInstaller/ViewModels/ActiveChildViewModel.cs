// <copyright file="ActiveChildViewModel.cs" company="Kristof Solbrig - HNE">
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
    /// ChildViewModel for user output.
    /// </summary>
    public class ActiveChildViewModel : Screen
    {
        private String _acvmOutput = String.Empty;
        private List<String> _selectedAcvmFiles = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveChildViewModel"/> class.
        /// </summary>
        public ActiveChildViewModel()
        {
            FileHandler fileHandler = Singleton<FileHandler>.Instance;
            fileHandler.LogToViewModel += (s) => this.AcvmOutput += s;
        }

        /// <summary>
        /// Gets string-list of all files in installation folder.
        /// </summary>
        public static List<String> AcvmFileList
        {
            get
            {
                FileHandler fileHandler = Singleton<FileHandler>.Instance;
                return fileHandler.FetchAllFiles();
            }
        }

        /// <summary>
        /// Gets or Sets selected items/files in child view.
        /// </summary>
        public List<String> SelectedAcvmFiles
        {
            get
            {
                return this._selectedAcvmFiles;
            }

            set
            {
                if (this._selectedAcvmFiles != value)
                {
                    FileHandler fileHandler = Singleton<FileHandler>.Instance;
                    this._selectedAcvmFiles = fileHandler.FetchPresetFiles();
                    this._selectedAcvmFiles = value;

                    this.NotifyOfPropertyChange(nameof(this.SelectedAcvmFiles));
                }
            }
        }

        /// <summary>
        /// Gets or sets output.
        /// </summary>
        public String AcvmOutput
        {
            get
            {
                return this._acvmOutput;
            }

            set
            {
                this._acvmOutput = value;
                this.NotifyOfPropertyChange(() => this.AcvmOutput);
            }
        }
    }
}