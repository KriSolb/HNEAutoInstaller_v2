using HNEAutoInstaller.Services;
using System;
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
                DirectoryInfo working = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                working.CreateSubdirectory(InstallFilesFolder);
                working.CreateSubdirectory(DatabaseFolder);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Fetch all files in install folder
        public static String[] FetchAllFiles()
        {
            try
            {
                _fileList = Directory.GetFiles(InstallFilesFolder).Select(Path.GetFileName).ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return _fileList;
        }

        public static void InstallFiles()
        {
            _fileList = FetchAllFiles();

            DatabaseService dbObject = new DatabaseService();
            dbObject.OpenConnection();
            String query = @"SELECT * FROM Files WHERE FullFileName = @fullFileName;";

            try
            {
                if (_fileList != null)
                {
                    for (Int32 i = 0; i < _fileList.Length; i++)
                    {
                        _fullFileName = _fileList[i];

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

                        if (_fileExtension == "exe")
                        {
                            Process p = new Process();
                            p.StartInfo.FileName = $"{InstallFilesFolder}" + "\\" + $"{_fileList[i]}";
                            p.StartInfo.Arguments = $"{_installArgument}";
                            p.Start();
                            p.WaitForExit();
                        }

                        if (_fileExtension == "zip")
                        {
                            _zipFrom = $"{InstallFilesFolder}" + "\\" + $"{_fileList[i]}";

                            System.IO.Compression.ZipFile.ExtractToDirectory(_zipFrom, _zipDestination);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            dbObject.CloseConnection();
        }

        private const String InstallFilesFolder = "InstallFiles";
        private const String DatabaseFolder = "Database";
        private const String DatabaseFile = "Database.db";

        private static String[] _fileList = Array.Empty<String>();
        private static String _fullFileName = String.Empty;
        private static String _installArgument = String.Empty;
        private static String _fileExtension = String.Empty;
        private static String _zipFrom = String.Empty;
        private static String _zipDestination = String.Empty;
    }
}