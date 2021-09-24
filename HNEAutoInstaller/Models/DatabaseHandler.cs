// <copyright file="DatabaseHandler.cs" company="Kristof Solbrig - HNE">
// Copyright (c) Kristof Solbrig - HNE. All rights reserved.
// </copyright>

using HNEAutoInstaller.Services;
using System;
using System.Data.SQLite;

namespace HNEAutoInstaller.Models
{
    /// <summary>
    /// Handling database requests.
    /// </summary>
    public static class DatabaseHandler
    {
        private static DatabaseService _dbObject = new();
        private static String _queryFullFileName = @"SELECT * FROM Files WHERE FullFileName = @fullFileName;";
        private static String[] _fileOptions = Array.Empty<String>();

        /// <summary>
        /// Fetch install arguments for files from database.
        /// </summary>
        /// <param name="filename"> Given file name. </param>
        /// <returns> String array. </returns>
        public static String[] FetchInstallArguments(String filename)
        {
            _dbObject.OpenConnection();
            SQLiteCommand installArgumentCommand = new(_queryFullFileName, _dbObject.DbConnection);
            installArgumentCommand.Parameters.AddWithValue("@fullFileName", filename);
            _dbObject.CloseConnection();

            SQLiteDataReader result = installArgumentCommand.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    _fileOptions[0] = result["FileExtension"].ToString();
                    _fileOptions[1] = result["Arguments"].ToString();
                    _fileOptions[2] = result["Destination"].ToString();
                }
            }

            return _fileOptions;
        }
    }
}