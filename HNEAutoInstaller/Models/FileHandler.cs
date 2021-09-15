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
    public static partial class FileHandler
    {
        // Creates folders for install files and database
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

        // Fetch all files in install folder
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

        // Install all files from directory InstallFiles
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
                                _zipDestination = result["Destination"].ToString();
                            }
                        }

                        InstallFile(_fileExtension, _fullFileName, _installArgument, _zipDestination);
                    }
                }
                dbObject.CloseConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void InstallAllFolderFiles() => InstallAllFiles(FetchAllFiles());

        public static void InstallPresetFiles() => InstallAllFiles(FetchPresetFiles());

        // Check the file extension then install the files
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
                _zipFrom = $"{InstallFilesFolder}" + "\\" + $"{file}";

                System.IO.Compression.ZipFile.ExtractToDirectory(_zipFrom, desti);
            }
        }

        private const String InstallFilesFolder = "InstallFiles";
        private const String DatabaseFolder = "Database";

        private static List<String> _fileList = new();
        private static List<String> _presetFileList = new();

        private static String _fullFileName = String.Empty;
        private static String _installArgument = String.Empty;
        private static String _fileExtension = String.Empty;
        private static String _zipFrom = String.Empty;
        private static String _zipDestination = String.Empty;

        private static Int32 _installerPreset = 0;
    }
}