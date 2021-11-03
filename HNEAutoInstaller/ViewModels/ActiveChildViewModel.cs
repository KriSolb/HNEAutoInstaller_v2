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
        private List<String> _acvmFileList = new();
        private List<String> _selectedAcvmFileList = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveChildViewModel"/> class.
        /// </summary>
        public ActiveChildViewModel()
        {
            FileHandler fileHandler = Singleton<FileHandler>.Instance;
            fileHandler.LogToViewModel += (s) => this.AcvmOutput += s;
        }

        /// <summary>
        /// Gets or Sets string-list of all files in installation folder.
        /// </summary>
        public List<String> AcvmFileList
        {
            get
            {
                FileHandler fileHandler = Singleton<FileHandler>.Instance;
                this._acvmFileList = fileHandler.FetchAllFiles();

                return this._acvmFileList;
            }

            set
            {
                this._acvmFileList = value;
                this.NotifyOfPropertyChange(() => this.AcvmFileList);
            }
        }

        /// <summary>
        /// Gets or Sets selected items/files in child view.
        /// </summary>
        public List<String> SelectedAcvmFileList // macht gerade goar nüscht, außer bytes belegen.
        {
            get
            {
                return this._selectedAcvmFileList;
            }

            set
            {
                if (this._selectedAcvmFileList != value)
                {
                    this._selectedAcvmFileList = value;

                    this.NotifyOfPropertyChange(nameof(this.SelectedAcvmFileList));
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