// <copyright file="FileHandler.cs" company="Kristof Solbrig - HNE">
// Copyright (c) Kristof Solbrig - HNE. All rights reserved.
// </copyright>

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
        private const String InstallFilesFolder = "InstallFiles";   // ok.
        private const String DatabaseFolder = "Database";           // ok.
        private static List<String> _fileList = new();
        private static List<String> _presetFileList = new();
        private static String _fullFileName = String.Empty;
        private static String _installArgument = String.Empty;
        private static String _fileExtension = String.Empty;
        private static String _zipSource = String.Empty;
        private static String _zipTarget = String.Empty;
        private static Int32 _installerPreset = 1;
        private static List<String> _dbFileList = new();

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

                DatabaseService dbObject = new();
                String query = @"SELECT * FROM Files";
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
                List<String> tempListExcept = _fileList.Except(_dbFileList).ToList();

                foreach (String x in tempListExcept)
                {
                    this.LogToViewModel?.Invoke("\n" + x + " not found\n");
                }

                List<String> tempListIntersect = _dbFileList.Intersect(_fileList).ToList();
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
        /// <returns> Returns filenames with specific preset as string-list.</returns>
        public List<String> FetchPresetFiles()
        {
            try
            {
                List<String> presetFileList = new();
                DatabaseService dbObject = new();

                String query = @"SELECT Files.FullFileName FROM Files INNER JOIN Files_Presets ON Files_Presets.presets_id = @presets_id WHERE Files_Presets.files_id = Files.files_id;";

                dbObject.OpenConnection();

                SQLiteCommand installArgumentCommand = new(query, dbObject.DbConnection);

                installArgumentCommand.Parameters.AddWithValue("@presets_id", _installerPreset);

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
        /// Install all files from param string-list. Fetch filename, install args and destination from database.
        /// </summary>
        /// <param name="installList">Given string list.</param>
        public void InstallAllFiles(List<String> installList)
        {
            DatabaseService dbObject = new();

            String query = @"SELECT * FROM Files WHERE FullFileName = @fullFileName;";

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

        /// <summary>
        /// Method for creating folders, if they dont already exist.
        /// </summary>
        public void CreateFolders()
        {
            try
            {
                DirectoryInfo working = new(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                working.CreateSubdirectory(InstallFilesFolder);
                this.LogToViewModel?.Invoke(InstallFilesFolder + " folder created.\n");
                working.CreateSubdirectory(DatabaseFolder);
                this.LogToViewModel?.Invoke(DatabaseFolder + " folder created.\n");
            }
            catch (Exception e)
            {
                this.LogToViewModel?.Invoke(e.ToString());
            }
        }

        /// <summary>
        /// Install/Execute all files from folder after fetching all files from folder.
        /// </summary>
        public void InstallAllFolderFiles() => this.InstallAllFiles(this.FetchAllFiles());

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
                    p.WaitForExit();
                    this.LogToViewModel?.Invoke("Installed succesfully: " + file + "\n");
                    this.LogToViewModel?.Invoke("...");
                }
                catch (Exception e)
                {
                    this.LogToViewModel?.Invoke("..." + e.ToString());
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
                catch (Exception e)
                {
                    this.LogToViewModel?.Invoke("..." + e.ToString());
                }
            }
        }
    }
}