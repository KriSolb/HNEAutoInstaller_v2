using HNEAutoInstaller.Services;
using System;
using System.Data.SQLite;

namespace HNEAutoInstaller.Models
{
    public static class DatabaseHandler
    {
        public static String[] FetchInstallArguments(String filename)
        {

            _dbObject.OpenConnection();
            SQLiteCommand installArgumentCommand = new(_queryFullFileName, _dbObject.DbConnection);
            installArgumentCommand.Parameters.AddWithValue("@fullFileName", filename);
            _dbObject.CloseConnection();

            SQLiteDataReader _result = installArgumentCommand.ExecuteReader();

            if (_result.HasRows)
            {
                while (_result.Read())
                {
                    _fileOptions[0] = _result["FileExtension"].ToString();
                    _fileOptions[1] = _result["Arguments"].ToString();
                    _fileOptions[2] = _result["Destination"].ToString();
                }
            }

            return _fileOptions;
        }

        private static DatabaseService _dbObject = new();
        private static String _queryFullFileName = @"SELECT * FROM Files WHERE FullFileName = @fullFileName;";
        private static String[] _fileOptions;
    }
}