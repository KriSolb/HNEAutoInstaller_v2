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
    public static class FileHandler
    {
        // basic ressources
        private const String InstallFilesFolder = "InstallFiles";

        private const String DatabaseFolder = "Database";

        // for fetching
        private static List<String> _fileList = new();

        private static List<String> _presetFileList = new();

        // for installer
        private static String _fullFileName = String.Empty;

        private static String _installArgument = String.Empty;
        private static String _fileExtension = String.Empty;
        private static String _zipSource = String.Empty;
        private static String _zipTarget = String.Empty;

        // testing
        private static Int32 _installerPreset = 0;

        /// <summary>
        /// Method for creating folders, if they dont already exist.
        /// </summary>
        public static void CreateFolders()
        {
            try
            {
                DirectoryInfo working = new(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                working.CreateSubdirectory(InstallFilesFolder);
                working.CreateSubdirectory(DatabaseFolder);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Fetch all files from InstallFiles folder.
        /// </summary>
        /// <returns> Returns filenames in folder as string-list.</returns>
        public static List<String> FetchAllFiles()
        {
            try
            {
                _fileList = Directory.GetFiles(InstallFilesFolder).Select(Path.GetFileName).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return _fileList;
        }

        /// <summary>
        /// Fetch all files with a specific preset.
        /// </summary>
        /// <returns> Returns filenames with specific preset as string-list.</returns>
        public static List<String> FetchPresetFiles()
        {
            try
            {
                DatabaseService dbObject = new();
                String query = @"SELECT * FROM Files WHERE Preset = @preset;";

                dbObject.OpenConnection();
                SQLiteCommand installArgumentCommand = new(query, dbObject.DbConnection);
                installArgumentCommand.Parameters.AddWithValue("@preset", _installerPreset);

                SQLiteDataReader result = installArgumentCommand.ExecuteReader();

                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        _presetFileList.Add(Convert.ToString(result["FullFileName"]));
                    }
                }

                dbObject.CloseConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return _presetFileList;
        }

        /// <summary>
        /// Install all files from param string-list. Fetch filename, install args and destination from database.
        /// </summary>
        /// <param name="installList">Given string list.</param>
        public static void InstallAllFiles(List<String> installList)
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
                        }

                        InstallFile(_fileExtension, _fullFileName, _installArgument, _zipTarget);
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
        /// Install/Execute all files from folder after fetching all files from folder.
        /// </summary>
        public static void InstallAllFolderFiles() => InstallAllFiles(FetchAllFiles());

        /// <summary>
        /// Install/Execute all files with a specific preset after fetching preset files from database.
        /// </summary>
        public static void InstallPresetFiles() => InstallAllFiles(FetchPresetFiles());

        /// <summary>
        /// Install/execute/unpack files, check what kind of file it is.
        /// </summary>
        /// <param name="ext">file extention.</param>
        /// <param name="file">file name.</param>
        /// <param name="instArgs">install arguments.</param>
        /// <param name="desti">destination (for zip).</param>
        public static void InstallFile(String ext, String file, String instArgs, String desti)
        {
            if (ext == "exe")
            {
                Process p = new();
                p.StartInfo.FileName = $"{InstallFilesFolder}" + "\\" + $"{file}";
                p.StartInfo.Arguments = $"{instArgs}";
                p.Start();
                p.WaitForExit();
            }

            if (ext == "zip")
            {
                _zipSource = $"{InstallFilesFolder}" + "\\" + $"{file}";

                System.IO.Compression.ZipFile.ExtractToDirectory(_zipSource, desti);
            }
        }
    }
}