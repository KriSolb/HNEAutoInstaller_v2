// <copyright file="FileHandler.cs" company="Kristof Solbrig - HNE">
// Copyright (c) Kristof Solbrig - HNE. All rights reserved.
// </copyright>

using Caliburn.Micro;
using HNEAutoInstaller.Services;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HNEAutoInstaller.Models
{
    /// <summary>
    /// Class FileHandler models file handling specific methods.
    /// </summary>
    public class FileHandler : Narumikazuchi.Singleton
    {
        private const String InstallFilesFolder = "InstallFiles";
        private const String DatabaseFolder = "Database";

        private static List<String> _fileList = new();
        private static List<String> _presetFileList = new();
        private static List<String> _dbFileList = new();

        private static String _fullFileName = String.Empty;
        private static String _installArgument = String.Empty;
        private static String _fileExtension = String.Empty;
        private static String _zipSource = String.Empty;
        private static String _zipTarget = String.Empty;

        private FileHandler()
        {
        }

        /// <summary>
        /// Event action for logging to child view.
        /// </summary>
        public event Action<String> LogToViewModel;

        /// <summary>
        /// Fetch all files from InstallFiles folder. Compare to database.
        /// </summary>
        /// <returns> Returns filenames in folder as string-list.</returns>
        public List<String> FetchAllFiles()
        {
            try
            {
                _fileList = Directory.GetFiles(InstallFilesFolder).Select(Path.GetFileName).ToList();

                String query = Properties.Resources.FetchAllFiles;

                DatabaseService dbObject = new();
                dbObject.OpenConnection();
                SQLiteCommand installArgumentCommand = new(query, dbObject.DbConnection);
                SQLiteDataReader result = installArgumentCommand.ExecuteReader();

                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        _dbFileList.Add(result["FullFileName"].ToString());
                    }
                }

                dbObject.CloseConnection();

                this.LogToViewModel?.Invoke("All files in folder:\n");
                foreach (String x in _fileList)
                {
                    this.LogToViewModel?.Invoke(x + "\n");
                }

                this.LogToViewModel?.Invoke("\nFiles found in folder, but not in database:\n");
                List<String> tempListExcept = _fileList.Except(_dbFileList).ToList();
                foreach (String x in tempListExcept)
                {
                    this.LogToViewModel?.Invoke(x + "\n");
                }

                this.LogToViewModel?.Invoke("\nUsable:\n");
                List<String> tempListIntersect = _dbFileList.Intersect(_fileList).ToList();
                foreach (String x in tempListIntersect)
                {
                    this.LogToViewModel?.Invoke("OK: " + x + "\n");
                }

                return tempListIntersect;
            }
            catch (Exception e)
            {
                this.LogToViewModel?.Invoke(e.ToString());
            }

            return _fileList;
        }

        /// <summary>
        /// Fetch all files with a specific preset.
        /// </summary>
        /// <param name="preset">integer of presets_id.</param>
        /// <returns> Returns filenames with specific preset as string-list.</returns>
        public List<String> FetchPresetFiles(Int32 preset)
        {
            try
            {
                List<String> presetFileList = new();
                DatabaseService dbObject = new();

                String query = Properties.Resources.FetchPresetFiles;

                dbObject.OpenConnection();

                SQLiteCommand installArgumentCommand = new(query, dbObject.DbConnection);

                installArgumentCommand.Parameters.AddWithValue("@presets_id", preset);

                SQLiteDataReader result = installArgumentCommand.ExecuteReader();

                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        presetFileList.Add(result["FullFileName"].ToString());
                    }
                }

                dbObject.CloseConnection();

                _presetFileList = presetFileList;
            }
            catch (Exception e)
            {
                this.LogToViewModel?.Invoke(e.ToString());
            }

            return _presetFileList;
        }

        /// <summary>
        /// Fetch all preset names from database.
        /// </summary>
        /// <returns> Returns filenames with specific preset as bindable collection.</returns>
        public BindableCollection<String> FetchPresetNames()
        {
            BindableCollection<String> presetNames = new();
            String query = Properties.Resources.FetchAllPresets;

            DatabaseService dbObject = new();
            dbObject.OpenConnection();
            SQLiteCommand installArgumentCommand = new(query, dbObject.DbConnection);

            SQLiteDataReader result = installArgumentCommand.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    presetNames.Add(result["PresetName"].ToString());
                }
            }

            dbObject.CloseConnection();
            return presetNames;
        }

        /// <summary>
        /// Fetch the ID of a preset by its name.
        /// </summary>
        /// <param name="presetName">given preset name.</param>
        /// <returns> Returns filenames with specific preset as string-list.</returns>
        public Int64 FetchPresetID(String presetName)
        {
            Int64 presetID = 0;
            String query = Properties.Resources.FetchAllPresetNames;

            DatabaseService dbObject = new();
            dbObject.OpenConnection();
            SQLiteCommand installArgumentCommand = new(query, dbObject.DbConnection);

            installArgumentCommand.Parameters.AddWithValue("@presetName", presetName);

            SQLiteDataReader result = installArgumentCommand.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    presetID = (Int64)result["presets_id"];
                }
            }

            dbObject.CloseConnection();
            return presetID;
        }

        /// <summary>
        /// Install all files from param string-list. Fetch filename, install args and destination from database.
        /// </summary>
        /// <param name="installList">Given string list.</param>
        /// <param name="preset">Given string preset name.</param>
        public void InstallAllFiles(List<String> installList, String preset)
        {
            this.LogToViewModel?.Invoke("\nInstalling Preset: " + preset + "\n");
            DatabaseService dbObject = new();

            String query = Properties.Resources.FetchAllFilesByFullFileName;

            try
            {
                dbObject.OpenConnection();
                if (installList != null)
                {
                    for (Int32 i = 0; i < installList.Count; i++)
                    {
                        _fullFileName = installList[i];
                        _fileExtension = String.Empty;
                        _installArgument = String.Empty;
                        _zipTarget = String.Empty;

                        SQLiteCommand installArgumentCommand = new(query, dbObject.DbConnection);
                        installArgumentCommand.Parameters.AddWithValue("@fullFileName", _fullFileName);

                        SQLiteDataReader result = installArgumentCommand.ExecuteReader();

                        if (result.HasRows)
                        {
                            while (result.Read())
                            {
                                _fileExtension = result["FileExtension"].ToString();
                                _installArgument = result["Arguments"].ToString();
                                _zipTarget = result["Destination"].ToString();
                            }

                            this.InstallFile(_fileExtension, _fullFileName, _installArgument, _zipTarget);
                        }
                    }
                }

                dbObject.CloseConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // public void InstallAllFolderFiles() => this.InstallAllFiles(this.FetchAllFiles());

        /// <summary>
        /// Install/execute/unpack files, check what kind of file it is.
        /// </summary>
        /// <param name="ext">file extention.</param>
        /// <param name="file">file name.</param>
        /// <param name="instArgs">install arguments.</param>
        /// <param name="desti">destination (for zip).</param>
        public void InstallFile(String ext, String file, String instArgs, String desti)
        {
            if (ext == "exe")
            {
                try
                {
                    this.LogToViewModel?.Invoke("\nStarting: " + file + "\n");
                    Process p = new();
                    p.StartInfo.FileName = $"{InstallFilesFolder}" + "\\" + $"{file}";
                    p.StartInfo.Arguments = $"{instArgs}";
                    p.Start();
                    this.LogToViewModel?.Invoke("...\n");
                    p.WaitForExit();
                    this.LogToViewModel?.Invoke("Installed succesfully: " + file + "\n");
                }
                catch (Exception)
                {
                    this.LogToViewModel?.Invoke("\n..." + "Failed to install: " + file);
                }
            }
            else if (ext == "zip")
            {
                try
                {
                    this.LogToViewModel?.Invoke("\nUnpacking: " + file + "." + " to " + desti);
                    _zipSource = $"{InstallFilesFolder}" + "\\" + $"{file}";

                    System.IO.Compression.ZipFile.ExtractToDirectory(_zipSource, desti);
                }
                catch (Exception)
                {
                    this.LogToViewModel?.Invoke("\n..." + "Failed zu unpack: " + file);
                }
            }
            else
            {
                this.LogToViewModel?.Invoke("\nCan't handle file extension. Failure: " + file);
            }
        }

        /// <summary>
        /// Method for creating folders, if they dont already exist.
        /// </summary>
        public void CreateFolders()
        {
            try
            {
                DirectoryInfo working = new(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                working.CreateSubdirectory(InstallFilesFolder);
                working.CreateSubdirectory(DatabaseFolder);
            }
            catch (Exception e)
            {
                this.LogToViewModel?.Invoke(e.ToString());
            }
        }
    }
}