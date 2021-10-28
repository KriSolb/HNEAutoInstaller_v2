// <copyright file="ActiveChildView.xaml.cs" company="Kristof Solbrig - HNE">
// Copyright (c) Kristof Solbrig - HNE. All rights reserved.
// </copyright>

using HNEAutoInstaller.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HNEAutoInstaller.Views
{
    /// <summary>
    /// Interaction logic for ActiveChildView.xaml.
    /// </summary>
    public partial class ActiveChildView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveChildView"/> class.
        /// </summary>
        public ActiveChildView()
        {
            this.InitializeComponent();
            this.DataContext = new ActiveChildViewModel();
        }
    }
}