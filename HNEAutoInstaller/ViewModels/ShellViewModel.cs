using Caliburn.Micro;
using HNEAutoInstaller.Models;
using System;

namespace HNEAutoInstaller.ViewModels
{
    public class ShellViewModel : Conductor<Object>
    {
        public ShellViewModel()
        {
            FileHandler.CreateFolders();
        }
    }
}